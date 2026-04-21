// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - State.hh

#nullable enable

using System.Collections.Generic;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>State of a collision element within a link.</summary>
    public class CollisionState : SdfNamedPosedElement
    {
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

            return errors;
        }
    }

    /// <summary>State of a joint axis.</summary>
    public class JointAxisState : SdfElement
    {
        /// <summary>Angle of the joint axis in radians.</summary>
        public double Angle { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var angle = sdf.FindElement("angle");
            if (angle?.Value != null) Angle = angle.Value.DoubleValue;

            return errors;
        }
    }

    /// <summary>State of a joint.</summary>
    public class JointState : SdfElement
    {
        /// <summary>Joint name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Angle values for each axis.</summary>
        public List<double> Angles { get; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var angleElem = sdf.FindElement("angle");
            while (angleElem != null)
            {
                if (angleElem.Value != null)
                    Angles.Add(angleElem.Value.DoubleValue);
                angleElem = angleElem.GetNextElement("angle");
            }

            return errors;
        }
    }

    /// <summary>State of a link within a model.</summary>
    public class LinkState : SdfNamedPosedElement
    {
        /// <summary>Velocity (linear + angular as 6-vector).</summary>
        public Pose3d Velocity { get; set; } = Pose3d.Zero;

        /// <summary>Acceleration (linear + angular as 6-vector).</summary>
        public Pose3d Acceleration { get; set; } = Pose3d.Zero;

        /// <summary>Wrench (force + torque as 6-vector).</summary>
        public Pose3d Wrench { get; set; } = Pose3d.Zero;

        /// <summary>Collision states.</summary>
        public List<CollisionState> CollisionStates { get; } = new();

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

            var velocity = sdf.FindElement("velocity");
            if (velocity?.Value != null) Velocity = velocity.Value.Pose3dValue;

            var acceleration = sdf.FindElement("acceleration");
            if (acceleration?.Value != null) Acceleration = acceleration.Value.Pose3dValue;

            var wrench = sdf.FindElement("wrench");
            if (wrench?.Value != null) Wrench = wrench.Value.Pose3dValue;

            var collisionElem = sdf.FindElement("collision");
            while (collisionElem != null)
            {
                var cs = new CollisionState();
                errors.AddRange(cs.Load(collisionElem));
                CollisionStates.Add(cs);
                collisionElem = collisionElem.GetNextElement("collision");
            }

            return errors;
        }
    }

    /// <summary>State of a model.</summary>
    public class ModelState : SdfNamedPosedElement
    {
        /// <summary>Scale of the model.</summary>
        public Vector3d Scale { get; set; } = Vector3d.One;

        /// <summary>Joint states.</summary>
        public List<JointState> JointStates { get; } = new();

        /// <summary>Link states.</summary>
        public List<LinkState> LinkStates { get; } = new();

        /// <summary>Nested model states.</summary>
        public List<ModelState> ModelStates { get; } = new();

        /// <summary>Frames.</summary>
        public List<Frame> FrameStates { get; } = new();

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

            var scaleElem = sdf.FindElement("scale");
            if (scaleElem?.Value != null) Scale = scaleElem.Value.Vector3dValue;

            // Joint states
            var jointElem = sdf.FindElement("joint");
            while (jointElem != null)
            {
                var js = new JointState();
                errors.AddRange(js.Load(jointElem));
                JointStates.Add(js);
                jointElem = jointElem.GetNextElement("joint");
            }

            // Link states
            var linkElem = sdf.FindElement("link");
            while (linkElem != null)
            {
                var ls = new LinkState();
                errors.AddRange(ls.Load(linkElem));
                LinkStates.Add(ls);
                linkElem = linkElem.GetNextElement("link");
            }

            // Nested model states
            var modelElem = sdf.FindElement("model");
            while (modelElem != null)
            {
                var ms = new ModelState();
                errors.AddRange(ms.Load(modelElem));
                ModelStates.Add(ms);
                modelElem = modelElem.GetNextElement("model");
            }

            // Frame states
            var frameElem = sdf.FindElement("frame");
            while (frameElem != null)
            {
                var f = new Frame();
                errors.AddRange(f.Load(frameElem));
                FrameStates.Add(f);
                frameElem = frameElem.GetNextElement("frame");
            }

            return errors;
        }
    }

    /// <summary>State of a light.</summary>
    public class LightState : SdfNamedPosedElement
    {
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

            return errors;
        }
    }

    /// <summary>
    /// World state — a snapshot of the world at a particular time, including
    /// model states, light states, insertions and deletions.
    /// </summary>
    public class State : SdfElement
    {
        /// <summary>Name of the world this state applies to.</summary>
        public string WorldName { get; set; } = string.Empty;

        /// <summary>Simulation time.</summary>
        public double SimTime { get; set; }

        /// <summary>Wall clock time.</summary>
        public double WallTime { get; set; }

        /// <summary>Real time.</summary>
        public double RealTime { get; set; }

        /// <summary>Number of simulation iterations.</summary>
        public ulong Iterations { get; set; }

        /// <summary>Inserted elements (stored as raw SDF elements).</summary>
        public List<Element> Insertions { get; } = new();

        /// <summary>Names of deleted entities.</summary>
        public List<string> Deletions { get; } = new();

        /// <summary>Model states.</summary>
        public List<ModelState> ModelStates { get; } = new();

        /// <summary>Light states.</summary>
        public List<LightState> LightStates { get; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("world_name");
            if (nameAttr != null) WorldName = nameAttr.GetAsString();

            var simTime = sdf.FindElement("sim_time");
            if (simTime?.Value != null) SimTime = simTime.Value.DoubleValue;

            var wallTime = sdf.FindElement("wall_time");
            if (wallTime?.Value != null) WallTime = wallTime.Value.DoubleValue;

            var realTime = sdf.FindElement("real_time");
            if (realTime?.Value != null) RealTime = realTime.Value.DoubleValue;

            var iterations = sdf.FindElement("iterations");
            if (iterations?.Value != null && ulong.TryParse(iterations.Value.GetAsString(), out var iter))
                Iterations = iter;

            // Insertions
            var insertions = sdf.FindElement("insertions");
            if (insertions != null)
            {
                var child = insertions.FindElement("model");
                while (child != null)
                {
                    Insertions.Add(child);
                    child = child.GetNextElement("model");
                }
                child = insertions.FindElement("light");
                while (child != null)
                {
                    Insertions.Add(child);
                    child = child.GetNextElement("light");
                }
            }

            // Deletions
            var deletions = sdf.FindElement("deletions");
            if (deletions != null)
            {
                var nameElem = deletions.FindElement("name");
                while (nameElem != null)
                {
                    if (nameElem.Value != null)
                        Deletions.Add(nameElem.Value.GetAsString());
                    nameElem = nameElem.GetNextElement("name");
                }
            }

            // Model states
            var modelElem = sdf.FindElement("model");
            while (modelElem != null)
            {
                var ms = new ModelState();
                errors.AddRange(ms.Load(modelElem));
                ModelStates.Add(ms);
                modelElem = modelElem.GetNextElement("model");
            }

            // Light states
            var lightElem = sdf.FindElement("light");
            while (lightElem != null)
            {
                var ls = new LightState();
                errors.AddRange(ls.Load(lightElem));
                LightStates.Add(ls);
                lightElem = lightElem.GetNextElement("light");
            }

            return errors;
        }
    }
}
