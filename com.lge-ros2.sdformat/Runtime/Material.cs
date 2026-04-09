// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Material.hh, Pbr.hh

#nullable enable

using System.Collections.Generic;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>
    /// PBR workflow properties (metallic or specular).
    /// </summary>
    public class PbrWorkflow : SdfElement
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

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var albedo = sdf.FindElement("albedo_map");
            if (albedo?.Value != null) AlbedoMap = albedo.Value.GetAsString();
            var normal = sdf.FindElement("normal_map");
            if (normal?.Value != null)
            {
                NormalMap = normal.Value.GetAsString();
                var typeAttr = normal.GetAttribute("type");
                if (typeAttr != null)
                {
                    NormalMapType = typeAttr.GetAsString().ToLowerInvariant() switch
                    {
                        "object" => NormalMapSpace.Object,
                        _ => NormalMapSpace.Tangent,
                    };
                }
            }
            var envMap = sdf.FindElement("environment_map");
            if (envMap?.Value != null) EnvironmentMap = envMap.Value.GetAsString();
            var aoMap = sdf.FindElement("ambient_occlusion_map");
            if (aoMap?.Value != null) AmbientOcclusionMap = aoMap.Value.GetAsString();
            var roughMap = sdf.FindElement("roughness_map");
            if (roughMap?.Value != null) RoughnessMap = roughMap.Value.GetAsString();
            var metalMap = sdf.FindElement("metalness_map");
            if (metalMap?.Value != null) MetalnessMap = metalMap.Value.GetAsString();
            var emissiveMap = sdf.FindElement("emissive_map");
            if (emissiveMap?.Value != null) EmissiveMap = emissiveMap.Value.GetAsString();
            var lightMap = sdf.FindElement("light_map");
            if (lightMap?.Value != null)
            {
                LightMap = lightMap.Value.GetAsString();
                var uvSet = lightMap.GetAttribute("uv_set");
                if (uvSet != null && uint.TryParse(uvSet.GetAsString(), out var uv))
                    LightMapTexCoordSet = uv;
            }
            var metalness = sdf.FindElement("metalness");
            if (metalness?.Value != null) Metalness = metalness.Value.DoubleValue;
            var roughness = sdf.FindElement("roughness");
            if (roughness?.Value != null) Roughness = roughness.Value.DoubleValue;
            var glossMap = sdf.FindElement("glossiness_map");
            if (glossMap?.Value != null) GlossinessMap = glossMap.Value.GetAsString();
            var glossiness = sdf.FindElement("glossiness");
            if (glossiness?.Value != null) Glossiness = glossiness.Value.DoubleValue;
            var specMap = sdf.FindElement("specular_map");
            if (specMap?.Value != null) SpecularMap = specMap.Value.GetAsString();

            return errors;
        }
    }
    public class Pbr : SdfElement
    {
        private readonly Dictionary<PbrWorkflowType, PbrWorkflow> _workflows = new();

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
    public class Material : SdfElement
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
            var shininess = sdf.FindElement("shininess");
            if (shininess?.Value != null) Shininess = shininess.Value.DoubleValue;
            var renderOrder = sdf.FindElement("render_order");
            if (renderOrder?.Value != null) RenderOrder = (float)renderOrder.Value.DoubleValue;

            // Parse <script> element for Ogre material scripts
            var scriptElem = sdf.FindElement("script");
            if (scriptElem != null)
            {
                var uriElem = scriptElem.FindElement("uri");
                if (uriElem?.Value != null) ScriptUri = uriElem.Value.StringValue;
                var nameElem = scriptElem.FindElement("name");
                if (nameElem?.Value != null) ScriptName = nameElem.Value.StringValue;
            }

            // Parse <shader> element
            var shaderElem = sdf.FindElement("shader");
            if (shaderElem != null)
            {
                var typeAttr = shaderElem.GetAttribute("type");
                if (typeAttr != null)
                {
                    var typeStr = typeAttr.GetAsString();
                    Shader = typeStr switch
                    {
                        "vertex" => ShaderType.Vertex,
                        "normal_map_objectspace" => ShaderType.NormalMapObjectSpace,
                        "normal_map_tangentspace" => ShaderType.NormalMapTangentSpace,
                        _ => ShaderType.Pixel,
                    };
                }
                var normalMapElem = shaderElem.FindElement("normal_map");
                if (normalMapElem?.Value != null) NormalMap = normalMapElem.Value.StringValue;
            }

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
}
