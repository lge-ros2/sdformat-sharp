// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Geometry.hh

using System.Collections.Generic;
using SdFormat.Math;

namespace SdFormat;

/// <summary>
/// Geometry provides access to a shape, such as a Box, Sphere, Cylinder, etc.
/// Use the Type property to determine which shape, then access via the appropriate Shape property.
/// </summary>
public class Geometry
{
    /// <summary>Type of the geometry shape.</summary>
    public GeometryType Type { get; set; } = GeometryType.Empty;

    /// <summary>Box shape (valid when Type == Box).</summary>
    public Box? BoxShape { get; set; }

    /// <summary>Sphere shape (valid when Type == Sphere).</summary>
    public Sphere? SphereShape { get; set; }

    /// <summary>Cylinder shape (valid when Type == Cylinder).</summary>
    public Cylinder? CylinderShape { get; set; }

    /// <summary>Capsule shape (valid when Type == Capsule).</summary>
    public Capsule? CapsuleShape { get; set; }

    /// <summary>Cone shape (valid when Type == Cone).</summary>
    public Cone? ConeShape { get; set; }

    /// <summary>Ellipsoid shape (valid when Type == Ellipsoid).</summary>
    public Ellipsoid? EllipsoidShape { get; set; }

    /// <summary>Plane shape (valid when Type == Plane).</summary>
    public Plane? PlaneShape { get; set; }

    /// <summary>Mesh shape (valid when Type == Mesh).</summary>
    public Mesh? MeshShape { get; set; }

    /// <summary>Heightmap shape (valid when Type == Heightmap).</summary>
    public Heightmap? HeightmapShape { get; set; }

    /// <summary>Polyline shapes (valid when Type == Polyline).</summary>
    public List<Polyline> PolylineShape { get; set; } = new();

    /// <summary>The SDF element from which this was loaded.</summary>
    public Element? Element { get; set; }

    /// <summary>Load geometry from an SDF element.</summary>
    public List<SdfError> Load(Element sdf)
    {
        var errors = new List<SdfError>();
        Element = sdf;

        if (sdf.HasElement("box"))
        {
            Type = GeometryType.Box;
            BoxShape = new Box();
            errors.AddRange(BoxShape.Load(sdf.FindElement("box")!));
        }
        else if (sdf.HasElement("sphere"))
        {
            Type = GeometryType.Sphere;
            SphereShape = new Sphere();
            errors.AddRange(SphereShape.Load(sdf.FindElement("sphere")!));
        }
        else if (sdf.HasElement("cylinder"))
        {
            Type = GeometryType.Cylinder;
            CylinderShape = new Cylinder();
            errors.AddRange(CylinderShape.Load(sdf.FindElement("cylinder")!));
        }
        else if (sdf.HasElement("capsule"))
        {
            Type = GeometryType.Capsule;
            CapsuleShape = new Capsule();
            errors.AddRange(CapsuleShape.Load(sdf.FindElement("capsule")!));
        }
        else if (sdf.HasElement("cone"))
        {
            Type = GeometryType.Cone;
            ConeShape = new Cone();
            errors.AddRange(ConeShape.Load(sdf.FindElement("cone")!));
        }
        else if (sdf.HasElement("ellipsoid"))
        {
            Type = GeometryType.Ellipsoid;
            EllipsoidShape = new Ellipsoid();
            errors.AddRange(EllipsoidShape.Load(sdf.FindElement("ellipsoid")!));
        }
        else if (sdf.HasElement("plane"))
        {
            Type = GeometryType.Plane;
            PlaneShape = new Plane();
            errors.AddRange(PlaneShape.Load(sdf.FindElement("plane")!));
        }
        else if (sdf.HasElement("mesh"))
        {
            Type = GeometryType.Mesh;
            MeshShape = new Mesh();
            errors.AddRange(MeshShape.Load(sdf.FindElement("mesh")!));
        }
        else if (sdf.HasElement("heightmap"))
        {
            Type = GeometryType.Heightmap;
            HeightmapShape = new Heightmap();
            errors.AddRange(HeightmapShape.Load(sdf.FindElement("heightmap")!));
        }
        else if (sdf.HasElement("polyline"))
        {
            Type = GeometryType.Polyline;
            // Load all polyline children
            var polyElem = sdf.FindElement("polyline");
            while (polyElem != null)
            {
                var poly = new Polyline();
                errors.AddRange(poly.Load(polyElem));
                PolylineShape.Add(poly);
                polyElem = polyElem.GetNextElement("polyline");
            }
        }

        return errors;
    }

    /// <summary>Convert to an SDF element.</summary>
    public Element ToElement()
    {
        var elem = new Element { Name = "geometry" };

        switch (Type)
        {
            case GeometryType.Box when BoxShape != null:
                elem.InsertElement(BoxShape.ToElement());
                break;
            case GeometryType.Sphere when SphereShape != null:
                elem.InsertElement(SphereShape.ToElement());
                break;
            case GeometryType.Cylinder when CylinderShape != null:
                elem.InsertElement(CylinderShape.ToElement());
                break;
            case GeometryType.Capsule when CapsuleShape != null:
                elem.InsertElement(CapsuleShape.ToElement());
                break;
            case GeometryType.Cone when ConeShape != null:
                elem.InsertElement(ConeShape.ToElement());
                break;
            case GeometryType.Ellipsoid when EllipsoidShape != null:
                elem.InsertElement(EllipsoidShape.ToElement());
                break;
            case GeometryType.Plane when PlaneShape != null:
                elem.InsertElement(PlaneShape.ToElement());
                break;
            case GeometryType.Mesh when MeshShape != null:
                elem.InsertElement(MeshShape.ToElement());
                break;
            case GeometryType.Heightmap when HeightmapShape != null:
                elem.InsertElement(HeightmapShape.ToElement());
                break;
        }

        return elem;
    }
}
