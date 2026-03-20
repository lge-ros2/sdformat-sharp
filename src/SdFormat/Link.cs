// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Link.hh

using System.Collections.Generic;
using System.Linq;
using SdFormat.Math;

namespace SdFormat;

/// <summary>
/// A link contains the physical properties of a body within a model,
/// including collision geometry, visual appearance, and sensor attachments.
/// </summary>
public class Link
{
    /// <summary>Name of the link.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional density (kg/m^3).</summary>
    public double? Density { get; set; }

    /// <summary>Inertial properties.</summary>
    public Inertial Inertial { get; set; } = new();

    /// <summary>Whether auto-inertia is enabled.</summary>
    public bool AutoInertia { get; set; }

    /// <summary>Whether auto-inertia result has been saved.</summary>
    public bool AutoInertiaSaved { get; set; }

    /// <summary>Auto-inertia parameters element.</summary>
    public Element? AutoInertiaParams { get; set; }

    /// <summary>The raw pose.</summary>
    public Pose3d RawPose { get; set; } = Pose3d.Zero;

    /// <summary>Name of the frame this pose is relative to.</summary>
    public string PoseRelativeTo { get; set; } = string.Empty;

    /// <summary>Whether wind affects this link.</summary>
    public bool EnableWind { get; set; }

    /// <summary>Whether gravity affects this link.</summary>
    public bool EnableGravity { get; set; } = true;

    /// <summary>Whether the link is kinematic only.</summary>
    public bool Kinematic { get; set; }

    /// <summary>Visual elements.</summary>
    public List<Visual> Visuals { get; } = new();

    /// <summary>Collision elements.</summary>
    public List<Collision> Collisions { get; } = new();

    /// <summary>Sensors.</summary>
    public List<Sensor> Sensors { get; } = new();

    /// <summary>Lights.</summary>
    public List<Light> Lights { get; } = new();

    /// <summary>Particle emitters.</summary>
    public List<Element> ParticleEmitters { get; } = new();

    /// <summary>Projectors.</summary>
    public List<Element> Projectors { get; } = new();

    /// <summary>The SDF element.</summary>
    public Element? Element { get; set; }

    // ---- Visual accessors ----
    public int VisualCount => Visuals.Count;
    public Visual? VisualByIndex(int index) =>
        index >= 0 && index < Visuals.Count ? Visuals[index] : null;
    public Visual? VisualByName(string name) =>
        Visuals.FirstOrDefault(v => v.Name == name);
    public bool VisualNameExists(string name) =>
        Visuals.Any(v => v.Name == name);
    public bool AddVisual(Visual visual)
    {
        if (VisualNameExists(visual.Name)) return false;
        Visuals.Add(visual);
        return true;
    }
    public void ClearVisuals() => Visuals.Clear();

    // ---- Collision accessors ----
    public int CollisionCount => Collisions.Count;
    public Collision? CollisionByIndex(int index) =>
        index >= 0 && index < Collisions.Count ? Collisions[index] : null;
    public Collision? CollisionByName(string name) =>
        Collisions.FirstOrDefault(c => c.Name == name);
    public bool CollisionNameExists(string name) =>
        Collisions.Any(c => c.Name == name);
    public bool AddCollision(Collision collision)
    {
        if (CollisionNameExists(collision.Name)) return false;
        Collisions.Add(collision);
        return true;
    }
    public void ClearCollisions() => Collisions.Clear();

    // ---- Sensor accessors ----
    public int SensorCount => Sensors.Count;
    public Sensor? SensorByIndex(int index) =>
        index >= 0 && index < Sensors.Count ? Sensors[index] : null;
    public Sensor? SensorByName(string name) =>
        Sensors.FirstOrDefault(s => s.Name == name);
    public bool SensorNameExists(string name) =>
        Sensors.Any(s => s.Name == name);
    public bool AddSensor(Sensor sensor)
    {
        if (SensorNameExists(sensor.Name)) return false;
        Sensors.Add(sensor);
        return true;
    }
    public void ClearSensors() => Sensors.Clear();

    // ---- Light accessors ----
    public int LightCount => Lights.Count;
    public Light? LightByIndex(int index) =>
        index >= 0 && index < Lights.Count ? Lights[index] : null;
    public bool LightNameExists(string name) =>
        Lights.Any(l => l.Name == name);
    public bool AddLight(Light light)
    {
        if (LightNameExists(light.Name)) return false;
        Lights.Add(light);
        return true;
    }
    public void ClearLights() => Lights.Clear();

    /// <summary>Load from an SDF element.</summary>
    public List<SdfError> Load(Element sdf)
    {
        var errors = new List<SdfError>();
        Element = sdf;

        var nameAttr = sdf.GetAttribute("name");
        if (nameAttr != null) Name = nameAttr.GetAsString();

        var poseElem = sdf.FindElement("pose");
        if (poseElem?.Value != null)
        {
            RawPose = poseElem.Value.Pose3dValue;
            var relTo = poseElem.GetAttribute("relative_to");
            if (relTo != null) PoseRelativeTo = relTo.GetAsString();
        }

        // Gravity
        var gravity = sdf.FindElement("gravity");
        if (gravity?.Value != null) EnableGravity = gravity.Value.BoolValue;

        // Wind
        var wind = sdf.FindElement("enable_wind");
        if (wind?.Value != null) EnableWind = wind.Value.BoolValue;

        // Kinematic
        var kinematic = sdf.FindElement("kinematic");
        if (kinematic?.Value != null) Kinematic = kinematic.Value.BoolValue;

        // Inertial
        if (sdf.HasElement("inertial"))
        {
            var inertialElem = sdf.FindElement("inertial")!;
            var mass = inertialElem.FindElement("mass");
            if (mass?.Value != null) Inertial.Mass = mass.Value.DoubleValue;

            var inertia = inertialElem.FindElement("inertia");
            if (inertia != null)
            {
                var ixx = inertia.FindElement("ixx");
                if (ixx?.Value != null) Inertial.Ixx = ixx.Value.DoubleValue;
                var iyy = inertia.FindElement("iyy");
                if (iyy?.Value != null) Inertial.Iyy = iyy.Value.DoubleValue;
                var izz = inertia.FindElement("izz");
                if (izz?.Value != null) Inertial.Izz = izz.Value.DoubleValue;
                var ixy = inertia.FindElement("ixy");
                if (ixy?.Value != null) Inertial.Ixy = ixy.Value.DoubleValue;
                var ixz = inertia.FindElement("ixz");
                if (ixz?.Value != null) Inertial.Ixz = ixz.Value.DoubleValue;
                var iyz = inertia.FindElement("iyz");
                if (iyz?.Value != null) Inertial.Iyz = iyz.Value.DoubleValue;
            }

            var inertialPose = inertialElem.FindElement("pose");
            if (inertialPose?.Value != null)
                Inertial.Pose = inertialPose.Value.Pose3dValue;
        }

        // Visuals
        var visualElem = sdf.FindElement("visual");
        while (visualElem != null)
        {
            var visual = new Visual();
            errors.AddRange(visual.Load(visualElem));
            Visuals.Add(visual);
            visualElem = visualElem.GetNextElement("visual");
        }

        // Collisions
        var collisionElem = sdf.FindElement("collision");
        while (collisionElem != null)
        {
            var collision = new Collision();
            errors.AddRange(collision.Load(collisionElem));
            Collisions.Add(collision);
            collisionElem = collisionElem.GetNextElement("collision");
        }

        // Sensors
        var sensorElem = sdf.FindElement("sensor");
        while (sensorElem != null)
        {
            var sensor = new Sensor();
            errors.AddRange(sensor.Load(sensorElem));
            Sensors.Add(sensor);
            sensorElem = sensorElem.GetNextElement("sensor");
        }

        // Lights
        var lightElem = sdf.FindElement("light");
        while (lightElem != null)
        {
            var light = new Light();
            errors.AddRange(light.Load(lightElem));
            Lights.Add(light);
            lightElem = lightElem.GetNextElement("light");
        }

        return errors;
    }

    /// <summary>Convert to an SDF element.</summary>
    public Element ToElement()
    {
        var elem = new Element { Name = "link" };
        elem.AddAttribute("name", "string", "", true);
        elem.GetAttribute("name")!.SetFromString(Name);

        if (RawPose != Pose3d.Zero)
        {
            var poseChild = new Element { Name = "pose" };
            poseChild.AddValue("pose", "0 0 0 0 0 0", false);
            poseChild.Set(RawPose.ToString());
            if (!string.IsNullOrEmpty(PoseRelativeTo))
            {
                poseChild.AddAttribute("relative_to", "string", "", false);
                poseChild.GetAttribute("relative_to")!.SetFromString(PoseRelativeTo);
            }
            elem.InsertElement(poseChild);
        }

        foreach (var visual in Visuals)
            elem.InsertElement(visual.ToElement());
        foreach (var collision in Collisions)
            elem.InsertElement(collision.ToElement());
        foreach (var sensor in Sensors)
            elem.InsertElement(sensor.ToElement());
        foreach (var light in Lights)
            elem.InsertElement(light.ToElement());

        return elem;
    }
}
