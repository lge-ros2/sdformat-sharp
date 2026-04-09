// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Visual.hh

#nullable enable

using System.Collections.Generic;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>
    /// A visual element describes the appearance of a link.
    /// </summary>
    public class Visual : SdfNamedPosedElement
    {
        /// <summary>Whether the visual casts shadows.</summary>
        public bool CastShadows { get; set; } = true;

        /// <summary>Transparency (0 = opaque, 1 = fully transparent).</summary>
        public float Transparency { get; set; }

        /// <summary>The geometry shape.</summary>
        public Geometry Geom { get; set; } = new();

        /// <summary>Material properties.</summary>
        public Material? MaterialInfo { get; set; }

        /// <summary>Visibility flags bitmask.</summary>
        public uint VisibilityFlags { get; set; } = 0xFFFFFFFF;

        /// <summary>Whether the visual has a laser retro value.</summary>
        public bool HasLaserRetro { get; set; }

        /// <summary>Laser retro value.</summary>
        public double LaserRetro { get; set; }

        /// <summary>Plugins attached to this visual.</summary>
        public List<Plugin> Plugins { get; } = new();

        /// <summary>Meta layer index (for grouping visuals).</summary>
        public int MetaLayer { get; set; }

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var castShadows = sdf.FindElement("cast_shadows");
            if (castShadows?.Value != null) CastShadows = castShadows.Value.BoolValue;

            var transparency = sdf.FindElement("transparency");
            if (transparency?.Value != null) Transparency = (float)transparency.Value.DoubleValue;

            var poseElem = sdf.FindElement("pose");
            if (poseElem?.Value != null)
            {
                RawPose = poseElem.Value.Pose3dValue;
                var relTo = poseElem.GetAttribute("relative_to");
                if (relTo != null) PoseRelativeTo = relTo.GetAsString();
            }

            if (sdf.HasElement("geometry"))
            {
                errors.AddRange(Geom.Load(sdf.FindElement("geometry")!));
            }

            if (sdf.HasElement("material"))
            {
                MaterialInfo = new Material();
                errors.AddRange(MaterialInfo.Load(sdf.FindElement("material")!));
            }

            // Laser retro
            var laserRetro = sdf.FindElement("laser_retro");
            if (laserRetro?.Value != null)
            {
                HasLaserRetro = true;
                LaserRetro = laserRetro.Value.DoubleValue;
            }

            // Visibility flags
            var visFlags = sdf.FindElement("visibility_flags");
            if (visFlags?.Value != null) VisibilityFlags = (uint)visFlags.Value.IntValue;

            // Meta
            var meta = sdf.FindElement("meta");
            if (meta != null)
            {
                var layer = meta.FindElement("layer");
                if (layer?.Value != null) MetaLayer = layer.Value.IntValue;
            }

            // Load plugins
            var pluginElem = sdf.FindElement("plugin");
            while (pluginElem != null)
            {
                var plugin = new Plugin();
                errors.AddRange(plugin.Load(pluginElem));
                Plugins.Add(plugin);
                pluginElem = pluginElem.GetNextElement("plugin");
            }

            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "visual" };
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

            elem.InsertElement(Geom.ToElement());

            if (MaterialInfo != null)
                elem.InsertElement(MaterialInfo.ToElement());

            return elem;
        }

        /// <summary>Clear plugins.</summary>
        public void ClearPlugins() => Plugins.Clear();

        /// <summary>Add a plugin.</summary>
        public void AddPlugin(Plugin plugin) => Plugins.Add(plugin);
    }
}
