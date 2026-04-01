// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Sensor.hh + sensor sub-types

using System;
using System.Collections.Generic;
using SdFormat.Math;

namespace SdFormat
{
    // ---- Sensor sub-type classes ----

    /// <summary>Altimeter sensor properties.</summary>
    public class AltimeterSensor
    {
        public Noise VerticalPositionNoise { get; set; } = new();
        public Noise VerticalVelocityNoise { get; set; } = new();
        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            return errors;
        }
    }

    /// <summary>Air pressure sensor properties.</summary>
    public class AirPressureSensor
    {
        public double ReferenceAltitude { get; set; }
        public Noise PressureNoise { get; set; } = new();
        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            return errors;
        }
    }

    /// <summary>Air speed sensor properties.</summary>
    public class AirSpeedSensor
    {
        public Noise PressureNoise { get; set; } = new();
        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            return errors;
        }
    }

    /// <summary>Camera sensor properties.</summary>
    public class CameraSensor
    {
        public string Name { get; set; } = string.Empty;
        public double HorizontalFov { get; set; } = 1.047;
        public uint ImageWidth { get; set; } = 320;
        public uint ImageHeight { get; set; } = 240;
        public string ImageFormat { get; set; } = "R8G8B8";
        public double NearClip { get; set; } = 0.1;
        public double FarClip { get; set; } = 100.0;
        public bool SaveFrames { get; set; }
        public string SavePath { get; set; } = string.Empty;
        public double DepthNearClip { get; set; } = 0.1;
        public double DepthFarClip { get; set; } = 10.0;
        public Noise ImageNoise { get; set; } = new();
        public uint AntiAliasingValue { get; set; } = 4;
        public uint VisibilityMask { get; set; } = 0xFFFFFFFF;
        public bool HasTriggeredCamera { get; set; }
        public string TriggerTopic { get; set; } = string.Empty;
        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var hfov = sdf.FindElement("horizontal_fov");
            if (hfov?.Value != null) HorizontalFov = hfov.Value.DoubleValue;

            var image = sdf.FindElement("image");
            if (image != null)
            {
                var w = image.FindElement("width");
                if (w?.Value != null) ImageWidth = (uint)w.Value.IntValue;
                var h = image.FindElement("height");
                if (h?.Value != null) ImageHeight = (uint)h.Value.IntValue;
                var fmt = image.FindElement("format");
                if (fmt?.Value != null) ImageFormat = fmt.Value.GetAsString();
            }

            var clip = sdf.FindElement("clip");
            if (clip != null)
            {
                var near = clip.FindElement("near");
                if (near?.Value != null) NearClip = near.Value.DoubleValue;
                var far = clip.FindElement("far");
                if (far?.Value != null) FarClip = far.Value.DoubleValue;
            }

            return errors;
        }
    }

    /// <summary>Force-torque sensor properties.</summary>
    public class ForceTorqueSensor
    {
        public enum FrameType { Child, Parent, Sensor }
        public enum MeasureDirectionType { ChildToParent, ParentToChild }

        public FrameType Frame { get; set; } = FrameType.Child;
        public MeasureDirectionType MeasureDirection { get; set; } = MeasureDirectionType.ChildToParent;
        public Noise ForceXNoise { get; set; } = new();
        public Noise ForceYNoise { get; set; } = new();
        public Noise ForceZNoise { get; set; } = new();
        public Noise TorqueXNoise { get; set; } = new();
        public Noise TorqueYNoise { get; set; } = new();
        public Noise TorqueZNoise { get; set; } = new();
        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            return errors;
        }
    }

    /// <summary>IMU sensor properties.</summary>
    public class ImuSensor
    {
        public Noise LinearAccelerationXNoise { get; set; } = new();
        public Noise LinearAccelerationYNoise { get; set; } = new();
        public Noise LinearAccelerationZNoise { get; set; } = new();
        public Noise AngularVelocityXNoise { get; set; } = new();
        public Noise AngularVelocityYNoise { get; set; } = new();
        public Noise AngularVelocityZNoise { get; set; } = new();
        public Vector3d GravityDirX { get; set; } = Vector3d.UnitX;
        public string GravityDirXParentFrame { get; set; } = string.Empty;
        public Vector3d CustomRpy { get; set; } = Vector3d.Zero;
        public string CustomRpyParentFrame { get; set; } = string.Empty;
        public bool OrientationEnabled { get; set; } = true;
        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            return errors;
        }
    }

    /// <summary>Lidar (ray) sensor properties.</summary>
    public class LidarSensor
    {
        public uint HorizontalScanSamples { get; set; } = 640;
        public double HorizontalScanResolution { get; set; } = 1.0;
        public Angle HorizontalScanMinAngle { get; set; } = Angle.Zero;
        public Angle HorizontalScanMaxAngle { get; set; } = Angle.Zero;
        public uint VerticalScanSamples { get; set; } = 1;
        public double VerticalScanResolution { get; set; } = 1.0;
        public Angle VerticalScanMinAngle { get; set; } = Angle.Zero;
        public Angle VerticalScanMaxAngle { get; set; } = Angle.Zero;
        public double RangeMin { get; set; } = 0.0;
        public double RangeMax { get; set; } = 0.0;
        public double RangeResolution { get; set; } = 0.0;
        public Noise RangeNoise { get; set; } = new();
        public uint VisibilityMask { get; set; } = 0xFFFFFFFF;
        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            return errors;
        }
    }

    /// <summary>Magnetometer sensor properties.</summary>
    public class MagnetometerSensor
    {
        public Noise XNoise { get; set; } = new();
        public Noise YNoise { get; set; } = new();
        public Noise ZNoise { get; set; } = new();
        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            return errors;
        }
    }

    /// <summary>NavSat (GPS) sensor properties.</summary>
    public class NavSatSensor
    {
        public Noise HorizontalPositionNoise { get; set; } = new();
        public Noise VerticalPositionNoise { get; set; } = new();
        public Noise HorizontalVelocityNoise { get; set; } = new();
        public Noise VerticalVelocityNoise { get; set; } = new();
        public Element? Element { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            return errors;
        }
    }

    // ---- Main Sensor class ----

    /// <summary>
    /// Information about an SDF sensor. A sensor can be attached to a link or joint.
    /// </summary>
    public class Sensor
    {
        /// <summary>Name of the sensor.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Sensor type.</summary>
        public SensorType Type { get; set; } = SensorType.None;

        /// <summary>Topic for sensor data publishing.</summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>Frame ID.</summary>
        public string FrameId { get; set; } = string.Empty;

        /// <summary>Whether metrics are enabled.</summary>
        public bool EnableMetrics { get; set; }

        /// <summary>Update rate in Hz.</summary>
        public double UpdateRate { get; set; }

        /// <summary>The raw pose.</summary>
        public Pose3d RawPose { get; set; } = Pose3d.Zero;

        /// <summary>Name of the frame this pose is relative to.</summary>
        public string PoseRelativeTo { get; set; } = string.Empty;

        /// <summary>The SDF element.</summary>
        public Element? Element { get; set; }

        // Sensor-specific data
        public AltimeterSensor? Altimeter { get; set; }
        public AirPressureSensor? AirPressure { get; set; }
        public AirSpeedSensor? AirSpeed { get; set; }
        public CameraSensor? Camera { get; set; }
        public ForceTorqueSensor? ForceTorque { get; set; }
        public ImuSensor? Imu { get; set; }
        public LidarSensor? Lidar { get; set; }
        public MagnetometerSensor? Magnetometer { get; set; }
        public NavSatSensor? NavSat { get; set; }

        /// <summary>Plugins.</summary>
        public List<Plugin> Plugins { get; } = new();

        /// <summary>Set the sensor type from a string.</summary>
        public bool SetType(string typeStr)
        {
            Type = typeStr.ToLowerInvariant() switch
            {
                "altimeter" => SensorType.Altimeter,
                "camera" => SensorType.Camera,
                "contact" => SensorType.Contact,
                "depth" or "depth_camera" => SensorType.DepthCamera,
                "force_torque" => SensorType.ForceTorque,
                "gps" => SensorType.Gps,
                "gpu_lidar" => SensorType.GpuLidar,
                "imu" => SensorType.Imu,
                "logical_camera" => SensorType.LogicalCamera,
                "magnetometer" => SensorType.Magnetometer,
                "multicamera" => SensorType.Multicamera,
                "lidar" or "ray" or "gpu_ray" => SensorType.Lidar,
                "rfid" => SensorType.Rfid,
                "rfidtag" => SensorType.RfidTag,
                "sonar" => SensorType.Sonar,
                "wireless_receiver" => SensorType.WirelessReceiver,
                "wireless_transmitter" => SensorType.WirelessTransmitter,
                "air_pressure" => SensorType.AirPressure,
                "rgbd_camera" or "rgbd" => SensorType.RgbdCamera,
                "thermal_camera" or "thermal" => SensorType.ThermalCamera,
                "navsat" => SensorType.NavSat,
                "segmentation" or "segmentation_camera" => SensorType.SegmentationCamera,
                "boundingbox_camera" or "boundingbox" => SensorType.BoundingBoxCamera,
                "custom" => SensorType.Custom,
                "wide_angle_camera" or "wideanglecamera" => SensorType.WideAngleCamera,
                "air_speed" => SensorType.AirSpeed,
                _ => SensorType.None,
            };
            return Type != SensorType.None;
        }

        /// <summary>Get the type as a string.</summary>
        public string TypeStr => Type switch
        {
            SensorType.Altimeter => "altimeter",
            SensorType.Camera => "camera",
            SensorType.Contact => "contact",
            SensorType.DepthCamera => "depth_camera",
            SensorType.ForceTorque => "force_torque",
            SensorType.Gps => "gps",
            SensorType.GpuLidar => "gpu_lidar",
            SensorType.Imu => "imu",
            SensorType.LogicalCamera => "logical_camera",
            SensorType.Magnetometer => "magnetometer",
            SensorType.Multicamera => "multicamera",
            SensorType.Lidar => "lidar",
            SensorType.Rfid => "rfid",
            SensorType.RfidTag => "rfidtag",
            SensorType.Sonar => "sonar",
            SensorType.WirelessReceiver => "wireless_receiver",
            SensorType.WirelessTransmitter => "wireless_transmitter",
            SensorType.AirPressure => "air_pressure",
            SensorType.RgbdCamera => "rgbd_camera",
            SensorType.ThermalCamera => "thermal_camera",
            SensorType.NavSat => "navsat",
            SensorType.SegmentationCamera => "segmentation_camera",
            SensorType.BoundingBoxCamera => "boundingbox_camera",
            SensorType.Custom => "custom",
            SensorType.WideAngleCamera => "wide_angle_camera",
            SensorType.AirSpeed => "air_speed",
            _ => "none",
        };

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var typeAttr = sdf.GetAttribute("type");
            if (typeAttr != null) SetType(typeAttr.GetAsString());

            var poseElem = sdf.FindElement("pose");
            if (poseElem?.Value != null)
            {
                RawPose = poseElem.Value.Pose3dValue;
                var relTo = poseElem.GetAttribute("relative_to");
                if (relTo != null) PoseRelativeTo = relTo.GetAsString();
            }

            var topicElem = sdf.FindElement("topic");
            if (topicElem?.Value != null) Topic = topicElem.Value.GetAsString();

            var updateRate = sdf.FindElement("update_rate");
            if (updateRate?.Value != null) UpdateRate = updateRate.Value.DoubleValue;

            // Load sensor-specific data
            switch (Type)
            {
                case SensorType.Altimeter:
                    if (sdf.HasElement("altimeter"))
                    {
                        Altimeter = new AltimeterSensor();
                        errors.AddRange(Altimeter.Load(sdf.FindElement("altimeter")!));
                    }
                    break;
                case SensorType.Camera:
                case SensorType.DepthCamera:
                case SensorType.RgbdCamera:
                case SensorType.ThermalCamera:
                case SensorType.SegmentationCamera:
                case SensorType.BoundingBoxCamera:
                case SensorType.WideAngleCamera:
                    if (sdf.HasElement("camera"))
                    {
                        Camera = new CameraSensor();
                        errors.AddRange(Camera.Load(sdf.FindElement("camera")!));
                    }
                    break;
                case SensorType.ForceTorque:
                    if (sdf.HasElement("force_torque"))
                    {
                        ForceTorque = new ForceTorqueSensor();
                        errors.AddRange(ForceTorque.Load(sdf.FindElement("force_torque")!));
                    }
                    break;
                case SensorType.Imu:
                    if (sdf.HasElement("imu"))
                    {
                        Imu = new ImuSensor();
                        errors.AddRange(Imu.Load(sdf.FindElement("imu")!));
                    }
                    break;
                case SensorType.Lidar:
                case SensorType.GpuLidar:
                    if (sdf.HasElement("lidar") || sdf.HasElement("ray"))
                    {
                        Lidar = new LidarSensor();
                        var lidarElem = sdf.FindElement("lidar") ?? sdf.FindElement("ray");
                        if (lidarElem != null)
                            errors.AddRange(Lidar.Load(lidarElem));
                    }
                    break;
                case SensorType.Magnetometer:
                    if (sdf.HasElement("magnetometer"))
                    {
                        Magnetometer = new MagnetometerSensor();
                        errors.AddRange(Magnetometer.Load(sdf.FindElement("magnetometer")!));
                    }
                    break;
                case SensorType.AirPressure:
                    if (sdf.HasElement("air_pressure"))
                    {
                        AirPressure = new AirPressureSensor();
                        errors.AddRange(AirPressure.Load(sdf.FindElement("air_pressure")!));
                    }
                    break;
                case SensorType.NavSat:
                    if (sdf.HasElement("navsat"))
                    {
                        NavSat = new NavSatSensor();
                        errors.AddRange(NavSat.Load(sdf.FindElement("navsat")!));
                    }
                    break;
                case SensorType.AirSpeed:
                    if (sdf.HasElement("air_speed"))
                    {
                        AirSpeed = new AirSpeedSensor();
                        errors.AddRange(AirSpeed.Load(sdf.FindElement("air_speed")!));
                    }
                    break;
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
            var elem = new Element { Name = "sensor" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);
            elem.AddAttribute("type", "string", "", true);
            elem.GetAttribute("type")!.SetFromString(TypeStr);
            return elem;
        }

        public void ClearPlugins() => Plugins.Clear();
        public void AddPlugin(Plugin plugin) => Plugins.Add(plugin);
    }
}
