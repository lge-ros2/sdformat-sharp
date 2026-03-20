// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Light.hh

using System.Collections.Generic;
using SdFormat.Math;

namespace SdFormat;

/// <summary>
/// Provides a description of a light source. A light can be point, spot, or directional.
/// </summary>
public class Light
{
    /// <summary>Type of this light.</summary>
    public LightType Type { get; set; } = LightType.Point;

    /// <summary>Name of the light.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>The raw pose.</summary>
    public Pose3d RawPose { get; set; } = Pose3d.Zero;

    /// <summary>Name of the frame this pose is relative to.</summary>
    public string PoseRelativeTo { get; set; } = string.Empty;

    /// <summary>Whether the light casts shadows.</summary>
    public bool CastShadows { get; set; }

    /// <summary>Whether the light is on.</summary>
    public bool LightOn { get; set; } = true;

    /// <summary>Whether the light is visualized in the GUI.</summary>
    public bool Visualize { get; set; }

    /// <summary>Light intensity.</summary>
    public double Intensity { get; set; } = 1.0;

    /// <summary>Diffuse color.</summary>
    public Color Diffuse { get; set; } = Color.White;

    /// <summary>Specular color.</summary>
    public Color Specular { get; set; } = new(0.1f, 0.1f, 0.1f, 1f);

    /// <summary>Attenuation range.</summary>
    public double AttenuationRange { get; set; } = 10.0;

    /// <summary>Constant attenuation factor.</summary>
    public double ConstantAttenuationFactor { get; set; } = 1.0;

    /// <summary>Linear attenuation factor.</summary>
    public double LinearAttenuationFactor { get; set; }

    /// <summary>Quadratic attenuation factor.</summary>
    public double QuadraticAttenuationFactor { get; set; }

    /// <summary>Direction of the light (for directional and spot).</summary>
    public Vector3d Direction { get; set; } = new(0, 0, -1);

    /// <summary>Spot inner angle.</summary>
    public Angle SpotInnerAngle { get; set; } = Angle.Zero;

    /// <summary>Spot outer angle.</summary>
    public Angle SpotOuterAngle { get; set; } = Angle.Zero;

    /// <summary>Spot falloff.</summary>
    public double SpotFalloff { get; set; }

    /// <summary>The SDF element.</summary>
    public Element? Element { get; set; }

    /// <summary>Load from an SDF element.</summary>
    public List<SdfError> Load(Element sdf)
    {
        var errors = new List<SdfError>();
        Element = sdf;

        var nameAttr = sdf.GetAttribute("name");
        if (nameAttr != null) Name = nameAttr.GetAsString();

        var typeAttr = sdf.GetAttribute("type");
        if (typeAttr != null)
        {
            Type = typeAttr.GetAsString().ToLowerInvariant() switch
            {
                "point" => LightType.Point,
                "spot" => LightType.Spot,
                "directional" => LightType.Directional,
                _ => LightType.Invalid,
            };
        }

        var poseElem = sdf.FindElement("pose");
        if (poseElem?.Value != null)
        {
            RawPose = poseElem.Value.Pose3dValue;
            var relTo = poseElem.GetAttribute("relative_to");
            if (relTo != null) PoseRelativeTo = relTo.GetAsString();
        }

        var castShadows = sdf.FindElement("cast_shadows");
        if (castShadows?.Value != null) CastShadows = castShadows.Value.BoolValue;

        var diffuse = sdf.FindElement("diffuse");
        if (diffuse?.Value != null) Diffuse = diffuse.Value.ColorValue;

        var specular = sdf.FindElement("specular");
        if (specular?.Value != null) Specular = specular.Value.ColorValue;

        var direction = sdf.FindElement("direction");
        if (direction?.Value != null) Direction = direction.Value.Vector3dValue;

        return errors;
    }

    /// <summary>Convert to an SDF element.</summary>
    public Element ToElement()
    {
        var elem = new Element { Name = "light" };
        elem.AddAttribute("name", "string", "", true);
        elem.GetAttribute("name")!.SetFromString(Name);

        string typeStr = Type switch
        {
            LightType.Point => "point",
            LightType.Spot => "spot",
            LightType.Directional => "directional",
            _ => "point",
        };
        elem.AddAttribute("type", "string", "point", true);
        elem.GetAttribute("type")!.SetFromString(typeStr);

        return elem;
    }
}
