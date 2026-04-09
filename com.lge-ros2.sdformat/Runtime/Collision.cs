// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Collision.hh

#nullable enable

using System.Collections.Generic;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>
    /// A collision element describes the collision properties of a link,
    /// including its geometry and surface contact parameters.
    /// </summary>
    public class Collision : SdfNamedPosedElement
    {
        /// <summary>Default density value (kg/m^3, water).</summary>
        public const double DefaultDensity = 1000.0;

        /// <summary>Density (kg/m^3).</summary>
        public double Density { get; set; } = DefaultDensity;

        /// <summary>Laser retro intensity.</summary>
        public double LaserRetro { get; set; }

        /// <summary>Maximum contacts.</summary>
        public int MaxContacts { get; set; } = 10;

        /// <summary>The geometry shape.</summary>
        public Geometry Geom { get; set; } = new();

        /// <summary>Surface properties.</summary>
        public Surface? SurfaceInfo { get; set; }

        /// <summary>Auto-inertia parameters element.</summary>
        public Element? AutoInertiaParams { get; set; }

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var densityElem = sdf.FindElement("density");
            if (densityElem?.Value != null) Density = densityElem.Value.DoubleValue;

            var laserRetroElem = sdf.FindElement("laser_retro");
            if (laserRetroElem?.Value != null) LaserRetro = laserRetroElem.Value.DoubleValue;

            var maxContactsElem = sdf.FindElement("max_contacts");
            if (maxContactsElem?.Value != null) MaxContacts = maxContactsElem.Value.IntValue;

            var autoInertiaElem = sdf.FindElement("auto_inertia_params");
            if (autoInertiaElem != null) AutoInertiaParams = autoInertiaElem;

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
