// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Box.hh, Sphere.hh, Cylinder.hh, Capsule.hh,
// Cone.hh, Ellipsoid.hh, Plane.hh, Mesh.hh, Heightmap.hh, Polyline.hh

#nullable enable

using System;
using System.Collections.Generic;
using SdFormat.Math;

namespace SdFormat
{
    /// <summary>Box geometry defined by its three dimensions.</summary>
    public class Box
    {
        /// <summary>The three dimensional size of the box (x, y, z).</summary>
        public Vector3d Size { get; set; } = new(1, 1, 1);

        /// <summary>The SDF element from which this was loaded.</summary>
        public Element? Element { get; set; }

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var sizeElem = sdf.FindElement("size");
            if (sizeElem?.Value != null)
                Size = sizeElem.Value.Vector3dValue;
            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "box" };
            var sizeChild = new Element { Name = "size" };
            sizeChild.AddValue("vector3", "1 1 1", true);
            sizeChild.Set(Size.ToString());
            elem.InsertElement(sizeChild);
            return elem;
        }
    }

    /// <summary>Sphere geometry defined by its radius.</summary>
    public class Sphere
    {
        /// <summary>The radius of the sphere in meters.</summary>
        public double Radius { get; set; } = 1.0;

        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var radiusElem = sdf.FindElement("radius");
            if (radiusElem?.Value != null)
                Radius = radiusElem.Value.DoubleValue;
            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "sphere" };
            var r = new Element { Name = "radius" };
            r.AddValue("double", "1", true);
            r.Set(Radius.ToString());
            elem.InsertElement(r);
            return elem;
        }
    }

    /// <summary>Cylinder geometry defined by radius and length.</summary>
    public class Cylinder
    {
        /// <summary>The radius of the cylinder in meters.</summary>
        public double Radius { get; set; } = 0.5;

        /// <summary>The length of the cylinder in meters.</summary>
        public double Length { get; set; } = 1.0;

        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var r = sdf.FindElement("radius");
            if (r?.Value != null) Radius = r.Value.DoubleValue;
            var l = sdf.FindElement("length");
            if (l?.Value != null) Length = l.Value.DoubleValue;
            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "cylinder" };
            var r = new Element { Name = "radius" };
            r.AddValue("double", "0.5", true);
            r.Set(Radius.ToString());
            elem.InsertElement(r);
            var l = new Element { Name = "length" };
            l.AddValue("double", "1", true);
            l.Set(Length.ToString());
            elem.InsertElement(l);
            return elem;
        }
    }

    /// <summary>Capsule geometry (cylinder with hemispherical end caps).</summary>
    public class Capsule
    {
        /// <summary>The radius of the capsule in meters.</summary>
        public double Radius { get; set; } = 0.5;

        /// <summary>The length of the cylindrical portion in meters.</summary>
        public double Length { get; set; } = 1.0;

        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var r = sdf.FindElement("radius");
            if (r?.Value != null) Radius = r.Value.DoubleValue;
            var l = sdf.FindElement("length");
            if (l?.Value != null) Length = l.Value.DoubleValue;
            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "capsule" };
            var r = new Element { Name = "radius" };
            r.AddValue("double", "0.5", true);
            r.Set(Radius.ToString());
            elem.InsertElement(r);
            var l = new Element { Name = "length" };
            l.AddValue("double", "1", true);
            l.Set(Length.ToString());
            elem.InsertElement(l);
            return elem;
        }
    }

    /// <summary>Cone geometry defined by radius and length.</summary>
    public class Cone
    {
        /// <summary>The radius of the cone base in meters.</summary>
        public double Radius { get; set; } = 0.5;

        /// <summary>The length (height) of the cone in meters.</summary>
        public double Length { get; set; } = 1.0;

        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var r = sdf.FindElement("radius");
            if (r?.Value != null) Radius = r.Value.DoubleValue;
            var l = sdf.FindElement("length");
            if (l?.Value != null) Length = l.Value.DoubleValue;
            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "cone" };
            var r = new Element { Name = "radius" };
            r.AddValue("double", "0.5", true);
            r.Set(Radius.ToString());
            elem.InsertElement(r);
            var l = new Element { Name = "length" };
            l.AddValue("double", "1", true);
            l.Set(Length.ToString());
            elem.InsertElement(l);
            return elem;
        }
    }

    /// <summary>Ellipsoid geometry defined by its three radii.</summary>
    public class Ellipsoid
    {
        /// <summary>The three radii of the ellipsoid (x, y, z).</summary>
        public Vector3d Radii { get; set; } = new(1, 1, 1);

        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var r = sdf.FindElement("radii");
            if (r?.Value != null) Radii = r.Value.Vector3dValue;
            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "ellipsoid" };
            var r = new Element { Name = "radii" };
            r.AddValue("vector3", "1 1 1", true);
            r.Set(Radii.ToString());
            elem.InsertElement(r);
            return elem;
        }
    }

    /// <summary>Plane geometry defined by a normal vector and a size.</summary>
    public class Plane
    {
        /// <summary>The normal direction of the plane.</summary>
        public Vector3d Normal { get; set; } = Vector3d.UnitZ;

        /// <summary>The 2D size of the plane (width and height).</summary>
        public Vector2d Size { get; set; } = new(1, 1);

        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var n = sdf.FindElement("normal");
            if (n?.Value != null) Normal = n.Value.Vector3dValue;
            var s = sdf.FindElement("size");
            if (s?.Value != null)
            {
                try { Size = Vector2d.Parse(s.Value.GetAsString()); }
                catch { /* ignore parse errors */ }
            }
            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "plane" };
            var n = new Element { Name = "normal" };
            n.AddValue("vector3", "0 0 1", true);
            n.Set(Normal.ToString());
            elem.InsertElement(n);
            var s = new Element { Name = "size" };
            s.AddValue("vector2d", "1 1", true);
            s.Set(Size.ToString());
            elem.InsertElement(s);
            return elem;
        }
    }

    /// <summary>Mesh geometry referencing an external mesh file.</summary>
    public class Mesh
    {
        /// <summary>The URI of the mesh file.</summary>
        public string Uri { get; set; } = string.Empty;

        /// <summary>The submesh name to use.</summary>
        public string Submesh { get; set; } = string.Empty;

        /// <summary>Whether to center the submesh.</summary>
        public bool CenterSubmesh { get; set; }

        /// <summary>The scale applied to the mesh.</summary>
        public Vector3d Scale { get; set; } = Vector3d.One;

        /// <summary>The file path for resolved URI.</summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>Optimization level (none, convex_decomposition, convex_hull).</summary>
        public string OptimizationStr { get; set; } = string.Empty;

        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var uriElem = sdf.FindElement("uri");
            if (uriElem?.Value != null) Uri = uriElem.Value.GetAsString();

            var scaleElem = sdf.FindElement("scale");
            if (scaleElem?.Value != null) Scale = scaleElem.Value.Vector3dValue;

            var submeshElem = sdf.FindElement("submesh");
            if (submeshElem != null)
            {
                var nameElem = submeshElem.FindElement("name");
                if (nameElem?.Value != null) Submesh = nameElem.Value.GetAsString();
                var centerElem = submeshElem.FindElement("center");
                if (centerElem?.Value != null) CenterSubmesh = centerElem.Value.BoolValue;
            }

            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "mesh" };
            var uriChild = new Element { Name = "uri" };
            uriChild.AddValue("string", "", true);
            uriChild.Set(Uri);
            elem.InsertElement(uriChild);

            if (Scale != Vector3d.One)
            {
                var s = new Element { Name = "scale" };
                s.AddValue("vector3", "1 1 1", false);
                s.Set(Scale.ToString());
                elem.InsertElement(s);
            }

            return elem;
        }
    }

    /// <summary>Heightmap geometry (terrain from heightmap image).</summary>
    public class Heightmap
    {
        /// <summary>The URI of the heightmap image.</summary>
        public string Uri { get; set; } = string.Empty;

        /// <summary>The file path for the resolved URI.</summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>The size of the heightmap (x, y, z).</summary>
        public Vector3d Size { get; set; } = new(1, 1, 1);

        /// <summary>The position offset.</summary>
        public Vector3d Position { get; set; } = Vector3d.Zero;

        /// <summary>Whether to use terrain paging.</summary>
        public bool UseTerrainPaging { get; set; }

        /// <summary>Sampling rate for the heightmap.</summary>
        public uint Sampling { get; set; } = 1;

        public Element? Element { get; set; }

        /// <summary>Heightmap texture blend specification.</summary>
        public class Texture
        {
            public string Diffuse { get; set; } = string.Empty;
            public string Normal { get; set; } = string.Empty;
            public double Size { get; set; } = 10;
        }

        /// <summary>Heightmap blend specification.</summary>
        public class Blend
        {
            public double MinHeight { get; set; }
            public double FadeDistance { get; set; }
        }

        public List<Texture> Textures { get; } = new();
        public List<Blend> Blends { get; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var uriElem = sdf.FindElement("uri");
            if (uriElem?.Value != null) Uri = uriElem.Value.GetAsString();
            var sizeElem = sdf.FindElement("size");
            if (sizeElem?.Value != null) Size = sizeElem.Value.Vector3dValue;
            var posElem = sdf.FindElement("pos");
            if (posElem?.Value != null) Position = posElem.Value.Vector3dValue;
            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "heightmap" };
            var u = new Element { Name = "uri" };
            u.AddValue("string", "", true);
            u.Set(Uri);
            elem.InsertElement(u);
            return elem;
        }
    }

    /// <summary>A 2D polyline defined by a series of points and a height.</summary>
    public class Polyline
    {
        /// <summary>The height of the polyline extrusion.</summary>
        public double Height { get; set; } = 1.0;

        /// <summary>The 2D points of the polyline.</summary>
        public List<Vector2d> Points { get; } = new();

        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var heightElem = sdf.FindElement("height");
            if (heightElem?.Value != null) Height = heightElem.Value.DoubleValue;
            // Points would be loaded from child <point> elements
            return errors;
        }
    }
}
