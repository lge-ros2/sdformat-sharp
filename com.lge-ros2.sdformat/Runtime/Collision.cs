// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Collision.hh

#nullable enable

using System.Collections.Generic;
using SdFormat.Math;

namespace SdFormat
{
    /// <summary>
    /// A collision element describes the collision properties of a link,
    /// including its geometry and surface contact parameters.
    /// </summary>
    public class Collision
    {
        /// <summary>Default density value (kg/m^3, water).</summary>
        public const double DefaultDensity = 1000.0;

        /// <summary>Name of the collision.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Density (kg/m^3).</summary>
        public double Density { get; set; } = DefaultDensity;

        /// <summary>The geometry shape.</summary>
        public Geometry Geom { get; set; } = new();

        /// <summary>Surface properties.</summary>
        public Surface? SurfaceInfo { get; set; }

        /// <summary>The raw pose relative to a frame.</summary>
        public Pose3d RawPose { get; set; } = Pose3d.Zero;

        /// <summary>Name of the frame this pose is relative to.</summary>
        public string PoseRelativeTo { get; set; } = string.Empty;

        /// <summary>Auto-inertia parameters element.</summary>
        public Element? AutoInertiaParams { get; set; }

        /// <summary>The SDF element.</summary>
        public Element? Element { get; set; }

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

            if (sdf.HasElement("geometry"))
            {
                errors.AddRange(Geom.Load(sdf.FindElement("geometry")!));
            }

            if (sdf.HasElement("surface"))
            {
                SurfaceInfo = new Surface();
                errors.AddRange(SurfaceInfo.Load(sdf.FindElement("surface")!));
            }

            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "collision" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);

            // Pose
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

            return elem;
        }
    }
}
