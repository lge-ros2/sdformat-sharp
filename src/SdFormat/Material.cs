// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Material.hh, Pbr.hh

using System.Collections.Generic;
using SdFormat.Math;

namespace SdFormat;

/// <summary>
/// PBR workflow properties (metallic or specular).
/// </summary>
public class PbrWorkflow
{
    public PbrWorkflowType Type { get; set; } = PbrWorkflowType.None;
    public string AlbedoMap { get; set; } = string.Empty;
    public string NormalMap { get; set; } = string.Empty;
    public NormalMapSpace NormalMapType { get; set; } = NormalMapSpace.Tangent;
    public string EnvironmentMap { get; set; } = string.Empty;
    public string AmbientOcclusionMap { get; set; } = string.Empty;
    public string RoughnessMap { get; set; } = string.Empty;
    public string MetalnessMap { get; set; } = string.Empty;
    public string EmissiveMap { get; set; } = string.Empty;
    public string LightMap { get; set; } = string.Empty;
    public uint LightMapTexCoordSet { get; set; }
    public double Metalness { get; set; } = 0.5;
    public double Roughness { get; set; } = 0.5;
    public string GlossinessMap { get; set; } = string.Empty;
    public double Glossiness { get; set; }
    public string SpecularMap { get; set; } = string.Empty;
    public Element? Element { get; set; }

    public List<SdfError> Load(Element sdf)
    {
        var errors = new List<SdfError>();
        Element = sdf;
        return errors;
    }
}

/// <summary>
/// PBR material settings containing metal and/or specular workflows.
/// </summary>
public class Pbr
{
    private readonly Dictionary<PbrWorkflowType, PbrWorkflow> _workflows = new();

    public Element? Element { get; set; }

    public void SetWorkflow(PbrWorkflowType type, PbrWorkflow workflow)
    {
        workflow.Type = type;
        _workflows[type] = workflow;
    }

    public PbrWorkflow? GetWorkflow(PbrWorkflowType type) =>
        _workflows.TryGetValue(type, out var wf) ? wf : null;

    public List<SdfError> Load(Element sdf)
    {
        var errors = new List<SdfError>();
        Element = sdf;

        if (sdf.HasElement("metal"))
        {
            var metal = new PbrWorkflow { Type = PbrWorkflowType.Metal };
            errors.AddRange(metal.Load(sdf.FindElement("metal")!));
            _workflows[PbrWorkflowType.Metal] = metal;
        }

        if (sdf.HasElement("specular"))
        {
            var spec = new PbrWorkflow { Type = PbrWorkflowType.Specular };
            errors.AddRange(spec.Load(sdf.FindElement("specular")!));
            _workflows[PbrWorkflowType.Specular] = spec;
        }

        return errors;
    }
}

/// <summary>
/// Visual material properties, including color, shader, and PBR settings.
/// </summary>
public class Material
{
    /// <summary>Ambient color.</summary>
    public Color Ambient { get; set; } = new(0f, 0f, 0f, 1f);

    /// <summary>Diffuse color.</summary>
    public Color Diffuse { get; set; } = new(1f, 1f, 1f, 1f);

    /// <summary>Specular color.</summary>
    public Color Specular { get; set; } = new(0f, 0f, 0f, 1f);

    /// <summary>Shininess exponent.</summary>
    public double Shininess { get; set; }

    /// <summary>Emissive color.</summary>
    public Color Emissive { get; set; } = new(0f, 0f, 0f, 1f);

    /// <summary>Render order (lower = earlier).</summary>
    public float RenderOrder { get; set; }

    /// <summary>Whether lighting is applied.</summary>
    public bool Lighting { get; set; } = true;

    /// <summary>Whether the material is double-sided.</summary>
    public bool DoubleSided { get; set; }

    /// <summary>Script URI for Ogre material scripts.</summary>
    public string ScriptUri { get; set; } = string.Empty;

    /// <summary>Script name for Ogre material scripts.</summary>
    public string ScriptName { get; set; } = string.Empty;

    /// <summary>Shader type.</summary>
    public ShaderType Shader { get; set; } = ShaderType.Pixel;

    /// <summary>Normal map URI.</summary>
    public string NormalMap { get; set; } = string.Empty;

    /// <summary>PBR material properties.</summary>
    public Pbr? PbrMaterial { get; set; }

    /// <summary>The file path.</summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>The SDF element from which this was loaded.</summary>
    public Element? Element { get; set; }

    /// <summary>Load from an SDF element.</summary>
    public List<SdfError> Load(Element sdf)
    {
        var errors = new List<SdfError>();
        Element = sdf;

        var ambient = sdf.FindElement("ambient");
        if (ambient?.Value != null) Ambient = ambient.Value.ColorValue;
        var diffuse = sdf.FindElement("diffuse");
        if (diffuse?.Value != null) Diffuse = diffuse.Value.ColorValue;
        var specular = sdf.FindElement("specular");
        if (specular?.Value != null) Specular = specular.Value.ColorValue;
        var emissive = sdf.FindElement("emissive");
        if (emissive?.Value != null) Emissive = emissive.Value.ColorValue;
        var lighting = sdf.FindElement("lighting");
        if (lighting?.Value != null) Lighting = lighting.Value.BoolValue;
        var doubleSided = sdf.FindElement("double_sided");
        if (doubleSided?.Value != null) DoubleSided = doubleSided.Value.BoolValue;

        if (sdf.HasElement("pbr"))
        {
            PbrMaterial = new Pbr();
            errors.AddRange(PbrMaterial.Load(sdf.FindElement("pbr")!));
        }

        return errors;
    }

    /// <summary>Convert to an SDF element.</summary>
    public Element ToElement()
    {
        var elem = new Element { Name = "material" };
        return elem;
    }
}
