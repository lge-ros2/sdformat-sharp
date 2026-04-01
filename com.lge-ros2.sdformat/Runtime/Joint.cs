// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Joint.hh

#nullable enable

using System.Collections.Generic;
using System.Linq;
using SdFormat.Math;

namespace SdFormat
{
    /// <summary>
    /// Description of a joint connecting two links within a model.
    /// </summary>
    public class Joint
    {
        /// <summary>Name of the joint.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Type of joint (revolute, prismatic, etc.).</summary>
        public JointType Type { get; set; } = JointType.Invalid;

        /// <summary>Name of the parent link or frame.</summary>
        public string ParentName { get; set; } = string.Empty;

        /// <summary>Name of the child link or frame.</summary>
        public string ChildName { get; set; } = string.Empty;

        /// <summary>Primary joint axis.</summary>
        public JointAxis? Axis { get; set; }

        /// <summary>Secondary joint axis (for revolute2 and universal).</summary>
        public JointAxis? Axis2 { get; set; }

        /// <summary>The raw pose.</summary>
        public Pose3d RawPose { get; set; } = Pose3d.Zero;

        /// <summary>Name of the frame this pose is relative to.</summary>
        public string PoseRelativeTo { get; set; } = string.Empty;

        /// <summary>Screw thread pitch (for screw joints).</summary>
        public double ScrewThreadPitch { get; set; }

        /// <summary>Thread pitch (deprecated, use ScrewThreadPitch).</summary>
        public double ThreadPitch { get; set; }

        /// <summary>Sensors attached to this joint.</summary>
        public List<Sensor> Sensors { get; } = new();

        /// <summary>The SDF element.</summary>
        public Element? Element { get; set; }

        /// <summary>Get the primary or secondary axis by index (0 or 1).</summary>
        public JointAxis? GetAxis(int index)
        {
            return index switch
            {
                0 => Axis,
                1 => Axis2,
                _ => null,
            };
        }

        /// <summary>Set the primary or secondary axis by index (0 or 1).</summary>
        public void SetAxis(int index, JointAxis axis)
        {
            switch (index)
            {
                case 0: Axis = axis; break;
                case 1: Axis2 = axis; break;
            }
        }

        /// <summary>Number of sensors.</summary>
        public int SensorCount => Sensors.Count;

        /// <summary>Get sensor by index.</summary>
        public Sensor? SensorByIndex(int index) =>
            index >= 0 && index < Sensors.Count ? Sensors[index] : null;

        /// <summary>Get sensor by name.</summary>
        public Sensor? SensorByName(string name) =>
            Sensors.FirstOrDefault(s => s.Name == name);

        /// <summary>Check if a sensor with the given name exists.</summary>
        public bool SensorNameExists(string name) =>
            Sensors.Any(s => s.Name == name);

        /// <summary>Add a sensor.</summary>
        public bool AddSensor(Sensor sensor)
        {
            if (SensorNameExists(sensor.Name)) return false;
            Sensors.Add(sensor);
            return true;
        }

        /// <summary>Clear all sensors.</summary>
        public void ClearSensors() => Sensors.Clear();

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
                    "ball" => JointType.Ball,
                    "continuous" => JointType.Continuous,
                    "fixed" => JointType.Fixed,
                    "gearbox" => JointType.Gearbox,
                    "prismatic" => JointType.Prismatic,
                    "revolute" => JointType.Revolute,
                    "revolute2" => JointType.Revolute2,
                    "screw" => JointType.Screw,
                    "universal" => JointType.Universal,
                    _ => JointType.Invalid,
                };
            }

            var parent = sdf.FindElement("parent");
            if (parent?.Value != null) ParentName = parent.Value.GetAsString();

            var child = sdf.FindElement("child");
            if (child?.Value != null) ChildName = child.Value.GetAsString();

            var poseElem = sdf.FindElement("pose");
            if (poseElem?.Value != null)
            {
                RawPose = poseElem.Value.Pose3dValue;
                var relTo = poseElem.GetAttribute("relative_to");
                if (relTo != null) PoseRelativeTo = relTo.GetAsString();
            }

            if (sdf.HasElement("axis"))
            {
                Axis = new JointAxis();
                errors.AddRange(Axis.Load(sdf.FindElement("axis")!));
            }

            if (sdf.HasElement("axis2"))
            {
                Axis2 = new JointAxis();
                errors.AddRange(Axis2.Load(sdf.FindElement("axis2")!));
            }

            // Load sensors
            var sensorElem = sdf.FindElement("sensor");
            while (sensorElem != null)
            {
                var sensor = new Sensor();
                errors.AddRange(sensor.Load(sensorElem));
                Sensors.Add(sensor);
                sensorElem = sensorElem.GetNextElement("sensor");
            }

            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "joint" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);

            string typeStr = Type switch
            {
                JointType.Ball => "ball",
                JointType.Continuous => "continuous",
                JointType.Fixed => "fixed",
                JointType.Gearbox => "gearbox",
                JointType.Prismatic => "prismatic",
                JointType.Revolute => "revolute",
                JointType.Revolute2 => "revolute2",
                JointType.Screw => "screw",
                JointType.Universal => "universal",
                _ => "invalid",
            };
            elem.AddAttribute("type", "string", "", true);
            elem.GetAttribute("type")!.SetFromString(typeStr);

            var parentChild = new Element { Name = "parent" };
            parentChild.AddValue("string", "", true);
            parentChild.Set(ParentName);
            elem.InsertElement(parentChild);

            var childChild = new Element { Name = "child" };
            childChild.AddValue("string", "", true);
            childChild.Set(ChildName);
            elem.InsertElement(childChild);

            if (Axis != null)
                elem.InsertElement(Axis.ToElement(0));
            if (Axis2 != null)
                elem.InsertElement(Axis2.ToElement(1));

            return elem;
        }
    }
}
