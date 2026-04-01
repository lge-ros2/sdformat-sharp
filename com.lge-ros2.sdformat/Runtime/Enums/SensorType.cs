// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Sensor.hh

namespace SdFormat
{
    /// <summary>
    /// The set of sensor types.
    /// </summary>
    public enum SensorType
    {
        /// <summary>An unspecified sensor type.</summary>
        None = 0,
        /// <summary>An altimeter sensor.</summary>
        Altimeter = 1,
        /// <summary>A monocular camera sensor.</summary>
        Camera = 2,
        /// <summary>A contact sensor.</summary>
        Contact = 3,
        /// <summary>A depth camera sensor.</summary>
        DepthCamera = 4,
        /// <summary>A force-torque sensor.</summary>
        ForceTorque = 5,
        /// <summary>A GPS sensor.</summary>
        Gps = 6,
        /// <summary>A GPU-based lidar sensor.</summary>
        GpuLidar = 7,
        /// <summary>An IMU sensor.</summary>
        Imu = 8,
        /// <summary>A logical camera sensor.</summary>
        LogicalCamera = 9,
        /// <summary>A magnetometer sensor.</summary>
        Magnetometer = 10,
        /// <summary>A multicamera sensor.</summary>
        Multicamera = 11,
        /// <summary>A CPU-based lidar sensor.</summary>
        Lidar = 12,
        /// <summary>An RFID sensor.</summary>
        Rfid = 13,
        /// <summary>An RFID tag sensor.</summary>
        RfidTag = 14,
        /// <summary>A sonar sensor.</summary>
        Sonar = 15,
        /// <summary>A wireless receiver.</summary>
        WirelessReceiver = 16,
        /// <summary>A wireless transmitter.</summary>
        WirelessTransmitter = 17,
        /// <summary>An air pressure sensor.</summary>
        AirPressure = 18,
        /// <summary>An RGBD camera sensor.</summary>
        RgbdCamera = 19,
        /// <summary>A thermal camera sensor.</summary>
        ThermalCamera = 20,
        /// <summary>A NavSat (GPS) sensor.</summary>
        NavSat = 21,
        /// <summary>A segmentation camera sensor.</summary>
        SegmentationCamera = 22,
        /// <summary>A bounding box camera sensor.</summary>
        BoundingBoxCamera = 23,
        /// <summary>A custom sensor.</summary>
        Custom = 24,
        /// <summary>A wide-angle camera sensor.</summary>
        WideAngleCamera = 25,
        /// <summary>An air speed sensor.</summary>
        AirSpeed = 26,
    }
}
