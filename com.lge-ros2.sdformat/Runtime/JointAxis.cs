// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - JointAxis.hh

using System.Collections.Generic;
using SdFormat.Math;

namespace SdFormat
{
    /// <summary>
    /// A mimic constraint for a joint axis, connecting the motion of
    /// two joint axes.
    /// </summary>
    public class MimicConstraint
    {
        /// <summary>Name of the joint to mimic.</summary>
        public string Joint { get; set; } = string.Empty;

        /// <summary>Name of the axis to mimic ("axis" or "axis2").</summary>
        public string Axis { get; set; } = "axis";

        /// <summary>Multiplier: follower_position = multiplier * leader_position + offset.</summary>
        public double Multiplier { get; set; }

        /// <summary>Position offset.</summary>
        public double Offset { get; set; }

        /// <summary>Reference position.</summary>
        public double Reference { get; set; }

        public MimicConstraint() { }

        public MimicConstraint(string joint, string axis = "axis",
            double multiplier = 0.0, double offset = 0.0, double reference = 0.0)
        {
            Joint = joint;
            Axis = axis;
            Multiplier = multiplier;
            Offset = offset;
            Reference = reference;
        }
    }

    /// <summary>
    /// Parameters for a joint axis, such as the axis direction, limits, dynamics.
    /// </summary>
    public class JointAxis
    {
        /// <summary>The axis direction unit vector in the frame specified by XyzExpressedIn.</summary>
        public Vector3d Xyz { get; set; } = Vector3d.UnitZ;

        /// <summary>Name of the frame in which the axis direction is expressed.</summary>
        public string XyzExpressedIn { get; set; } = string.Empty;

        /// <summary>Damping coefficient (N*s/m for prismatic, N*m*s/rad for revolute).</summary>
        public double Damping { get; set; }

        /// <summary>Static friction force/torque.</summary>
        public double Friction { get; set; }

        /// <summary>Spring reference position.</summary>
        public double SpringReference { get; set; }

        /// <summary>Spring stiffness.</summary>
        public double SpringStiffness { get; set; }

        /// <summary>Lower joint limit (m or rad).</summary>
        public double Lower { get; set; } = -1e16;

        /// <summary>Upper joint limit (m or rad).</summary>
        public double Upper { get; set; } = 1e16;

        /// <summary>Maximum effort (force or torque).</summary>
        public double Effort { get; set; } = double.PositiveInfinity;

        /// <summary>Maximum velocity (m/s or rad/s).</summary>
        public double MaxVelocity { get; set; } = double.PositiveInfinity;

        /// <summary>Joint stop stiffness.</summary>
        public double Stiffness { get; set; } = 1e8;

        /// <summary>Joint stop dissipation.</summary>
        public double Dissipation { get; set; } = 1.0;

        /// <summary>Optional mimic constraint.</summary>
        public MimicConstraint? Mimic { get; set; }

        /// <summary>The SDF element.</summary>
        public Element? Element { get; set; }

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var xyzElem = sdf.FindElement("xyz");
            if (xyzElem?.Value != null)
            {
                Xyz = xyzElem.Value.Vector3dValue;
                var expressedIn = xyzElem.GetAttribute("expressed_in");
                if (expressedIn != null) XyzExpressedIn = expressedIn.GetAsString();
            }

            var limit = sdf.FindElement("limit");
            if (limit != null)
            {
                var lower = limit.FindElement("lower");
                if (lower?.Value != null) Lower = lower.Value.DoubleValue;
                var upper = limit.FindElement("upper");
                if (upper?.Value != null) Upper = upper.Value.DoubleValue;
                var effort = limit.FindElement("effort");
                if (effort?.Value != null) Effort = effort.Value.DoubleValue;
                var velocity = limit.FindElement("velocity");
                if (velocity?.Value != null) MaxVelocity = velocity.Value.DoubleValue;
                var stiffness = limit.FindElement("stiffness");
                if (stiffness?.Value != null) Stiffness = stiffness.Value.DoubleValue;
                var dissipation = limit.FindElement("dissipation");
                if (dissipation?.Value != null) Dissipation = dissipation.Value.DoubleValue;
            }

            var dynamics = sdf.FindElement("dynamics");
            if (dynamics != null)
            {
                var damping = dynamics.FindElement("damping");
                if (damping?.Value != null) Damping = damping.Value.DoubleValue;
                var friction = dynamics.FindElement("friction");
                if (friction?.Value != null) Friction = friction.Value.DoubleValue;
                var springRef = dynamics.FindElement("spring_reference");
                if (springRef?.Value != null) SpringReference = springRef.Value.DoubleValue;
                var springStiff = dynamics.FindElement("spring_stiffness");
                if (springStiff?.Value != null) SpringStiffness = springStiff.Value.DoubleValue;
            }

            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement(int index = 0)
        {
            var elem = new Element { Name = index == 0 ? "axis" : "axis2" };

            var xyzChild = new Element { Name = "xyz" };
            xyzChild.AddValue("vector3", "0 0 1", true);
            xyzChild.Set(Xyz.ToString());
            if (!string.IsNullOrEmpty(XyzExpressedIn))
            {
                xyzChild.AddAttribute("expressed_in", "string", "", false);
                xyzChild.GetAttribute("expressed_in")!.SetFromString(XyzExpressedIn);
            }
            elem.InsertElement(xyzChild);

            return elem;
        }
    }
}
