// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0

using UnityEngine;

namespace SdFormat.Unity
{
    /// <summary>
    /// Spawns a Unity GameObject hierarchy from a parsed SDF <see cref="Root"/>.
    /// Primitive geometries (box, sphere, cylinder, capsule) are created with
    /// Unity built-in primitives. Mesh geometries log their URI for the user
    /// to resolve manually.
    /// </summary>
    public static class SdfSpawner
    {
        /// <summary>
        /// Spawn an entire SDF Root into the scene under a parent transform.
        /// If parent is null, spawns at scene root.
        /// </summary>
        public static GameObject Spawn(Root root, Transform parent = null)
        {
            var go = new GameObject($"SDF [{root.Version}]");
            if (parent != null) go.transform.SetParent(parent, false);

            // Spawn worlds
            for (int w = 0; w < root.WorldCount; w++)
            {
                var world = root.WorldByIndex(w);
                if (world != null) SpawnWorld(world, go.transform);
            }

            // Standalone model
            if (root.StandaloneModel != null)
                SpawnModel(root.StandaloneModel, go.transform);

            // Standalone light
            if (root.StandaloneLight != null)
                SpawnLight(root.StandaloneLight, go.transform);

            return go;
        }

        /// <summary>Spawn a world and its contents.</summary>
        public static GameObject SpawnWorld(World world, Transform parent = null)
        {
            var go = new GameObject($"World:{world.Name}");
            if (parent != null) go.transform.SetParent(parent, false);

            // Models
            for (int m = 0; m < world.ModelCount; m++)
            {
                var model = world.ModelByIndex(m);
                if (model != null) SpawnModel(model, go.transform);
            }

            // Lights
            for (int l = 0; l < world.LightCount; l++)
            {
                var light = world.LightByIndex(l);
                if (light != null) SpawnLight(light, go.transform);
            }

            return go;
        }

        /// <summary>Spawn a model as a hierarchy of link GameObjects.</summary>
        public static GameObject SpawnModel(Model model, Transform parent = null)
        {
            var go = new GameObject($"Model:{model.Name}");
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.ApplyPose(model.RawPose);

            if (model.Static)
                go.isStatic = true;

            // Links
            for (int l = 0; l < model.LinkCount; l++)
            {
                var link = model.LinkByIndex(l);
                if (link != null) SpawnLink(link, go.transform);
            }

            // Nested models
            for (int m = 0; m < model.ModelCount; m++)
            {
                var nested = model.ModelByIndex(m);
                if (nested != null) SpawnModel(nested, go.transform);
            }

            return go;
        }

        /// <summary>Spawn a link with its visuals and colliders.</summary>
        public static GameObject SpawnLink(Link link, Transform parent = null)
        {
            var go = new GameObject($"Link:{link.Name}");
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.ApplyPose(link.RawPose);

            // Rigidbody
            if (!link.Kinematic && link.EnableGravity)
            {
                var rb = go.AddComponent<Rigidbody>();
                rb.mass = (float)link.Inertial.Mass;
                rb.useGravity = link.EnableGravity;
                rb.isKinematic = link.Kinematic;
            }

            // Visuals
            for (int v = 0; v < link.VisualCount; v++)
            {
                var visual = link.VisualByIndex(v);
                if (visual != null) SpawnVisual(visual, go.transform);
            }

            // Collisions
            for (int c = 0; c < link.CollisionCount; c++)
            {
                var col = link.CollisionByIndex(c);
                if (col != null) SpawnCollision(col, go.transform);
            }

            return go;
        }

        /// <summary>Spawn a visual geometry as a MeshRenderer.</summary>
        public static GameObject SpawnVisual(Visual visual, Transform parent = null)
        {
            var go = CreateGeometryObject($"Visual:{visual.Name}", visual.Geom);
            if (go == null)
            {
                go = new GameObject($"Visual:{visual.Name}");
            }

            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.ApplyPose(visual.RawPose);

            // Apply material color
            if (visual.MaterialInfo != null)
            {
                var renderer = go.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    var mat = new UnityEngine.Material(Shader.Find("Standard"));
                    mat.color = visual.MaterialInfo.Diffuse.ToUnity();
                    renderer.sharedMaterial = mat;
                }
            }

            return go;
        }

        /// <summary>Spawn a collision geometry as a Collider.</summary>
        public static GameObject SpawnCollision(Collision collision, Transform parent = null)
        {
            var go = new GameObject($"Collision:{collision.Name}");
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.ApplyPose(collision.RawPose);

            var geom = collision.Geom;
            switch (geom.Type)
            {
                case GeometryType.Box when geom.BoxShape != null:
                    var box = go.AddComponent<BoxCollider>();
                    box.size = geom.BoxShape.Size.ToUnity();
                    break;

                case GeometryType.Sphere when geom.SphereShape != null:
                    var sphere = go.AddComponent<SphereCollider>();
                    sphere.radius = (float)geom.SphereShape.Radius;
                    break;

                case GeometryType.Cylinder when geom.CylinderShape != null:
                    // Unity has no CylinderCollider, approximate with CapsuleCollider
                    var cap = go.AddComponent<CapsuleCollider>();
                    cap.radius = (float)geom.CylinderShape.Radius;
                    cap.height = (float)geom.CylinderShape.Length;
                    break;

                case GeometryType.Capsule when geom.CapsuleShape != null:
                    var capsule = go.AddComponent<CapsuleCollider>();
                    capsule.radius = (float)geom.CapsuleShape.Radius;
                    capsule.height = (float)geom.CapsuleShape.Length;
                    break;

                case GeometryType.Plane:
                    // Plane → large box collider
                    var planeBox = go.AddComponent<BoxCollider>();
                    planeBox.size = new Vector3(100f, 0.01f, 100f);
                    break;

                case GeometryType.Mesh when geom.MeshShape != null:
                    Debug.Log($"[SdFormat] Mesh collider URI: {geom.MeshShape.Uri} — add MeshCollider manually.");
                    break;
            }

            return go;
        }

        /// <summary>Spawn an SDF Light as a Unity Light component.</summary>
        public static GameObject SpawnLight(Light sdfLight, Transform parent = null)
        {
            var go = new GameObject($"Light:{sdfLight.Name}");
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.ApplyPose(sdfLight.RawPose);

            var light = go.AddComponent<UnityEngine.Light>();
            light.color = sdfLight.Diffuse.ToUnity();
            light.intensity = (float)sdfLight.Intensity;
            light.shadows = sdfLight.CastShadows
                ? LightShadows.Soft
                : LightShadows.None;

            switch (sdfLight.Type)
            {
                case LightType.Point:
                    light.type = UnityEngine.LightType.Point;
                    light.range = (float)sdfLight.AttenuationRange;
                    break;
                case LightType.Directional:
                    light.type = UnityEngine.LightType.Directional;
                    break;
                case LightType.Spot:
                    light.type = UnityEngine.LightType.Spot;
                    light.range = (float)sdfLight.AttenuationRange;
                    light.spotAngle = (float)(sdfLight.SpotOuterAngle * 180.0 / System.Math.PI);
                    light.innerSpotAngle = (float)(sdfLight.SpotInnerAngle * 180.0 / System.Math.PI);
                    break;
            }

            return go;
        }

        // ──────────────── Geometry → Primitive ────────────────

        private static GameObject CreateGeometryObject(string name, Geometry geom)
        {
            switch (geom.Type)
            {
                case GeometryType.Box when geom.BoxShape != null:
                    var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.name = name;
                    box.transform.localScale = geom.BoxShape.Size.ToUnity();
                    Object.Destroy(box.GetComponent<Collider>());
                    return box;

                case GeometryType.Sphere when geom.SphereShape != null:
                    var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.name = name;
                    float d = (float)geom.SphereShape.Radius * 2f;
                    sphere.transform.localScale = new Vector3(d, d, d);
                    Object.Destroy(sphere.GetComponent<Collider>());
                    return sphere;

                case GeometryType.Cylinder when geom.CylinderShape != null:
                    var cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    cyl.name = name;
                    float r = (float)geom.CylinderShape.Radius * 2f;
                    float h = (float)geom.CylinderShape.Length;
                    cyl.transform.localScale = new Vector3(r, h / 2f, r);
                    Object.Destroy(cyl.GetComponent<Collider>());
                    return cyl;

                case GeometryType.Capsule when geom.CapsuleShape != null:
                    var cap = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    cap.name = name;
                    float cr = (float)geom.CapsuleShape.Radius * 2f;
                    float ch = (float)geom.CapsuleShape.Length;
                    cap.transform.localScale = new Vector3(cr, ch / 2f, cr);
                    Object.Destroy(cap.GetComponent<Collider>());
                    return cap;

                case GeometryType.Plane:
                    var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    plane.name = name;
                    Object.Destroy(plane.GetComponent<Collider>());
                    return plane;

                case GeometryType.Mesh when geom.MeshShape != null:
                    Debug.Log($"[SdFormat] Visual mesh URI: {geom.MeshShape.Uri} — assign mesh manually.");
                    return null;

                default:
                    return null;
            }
        }
    }
}
