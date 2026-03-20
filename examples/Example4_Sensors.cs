// Example 4: Create a sensor-rich link with various sensor types
//
// Demonstrates:
//   - Creating Camera, IMU, Lidar, ForceTorque, Contact, Altimeter sensors
//   - Configuring sensor noise, update rates, and camera/lidar optics
//   - Attaching sensors to links and printing their configurations
//   - Serializing the result

using SdFormat;
using SdFormat.Math;

namespace Examples;

public static class Example4_Sensors
{
    public static void Run()
    {
        Console.WriteLine("=== Example 4: Sensor Configuration ===\n");

        var model = new Model { Name = "sensor_testbed", Static = true };
        var sensorLink = new Link { Name = "sensor_bar" };
        sensorLink.Inertial.Mass = 2.0;
        sensorLink.Inertial.Ixx = 0.01;
        sensorLink.Inertial.Iyy = 0.1;
        sensorLink.Inertial.Izz = 0.1;

        // ---- Camera sensor ----
        var cameraSensor = new Sensor { Name = "front_camera", Type = SensorType.Camera };
        cameraSensor.UpdateRate = 30.0;
        cameraSensor.RawPose = new Pose3d(0.3, 0, 0.1, 0, 0, 0);
        cameraSensor.Camera = new CameraSensor
        {
            Name = "rgb_cam",
            HorizontalFov = 1.396, // ~80°
            ImageWidth = 1280,
            ImageHeight = 720,
            ImageFormat = "R8G8B8",
            NearClip = 0.05,
            FarClip = 100.0,
            DepthNearClip = 0.1,
            DepthFarClip = 50.0,
        };
        cameraSensor.Camera.ImageNoise = new Noise
        {
            Type = NoiseType.Gaussian,
            Mean = 0.0,
            StdDev = 0.005,
        };
        sensorLink.AddSensor(cameraSensor);

        // ---- Depth camera sensor ----
        var depthSensor = new Sensor { Name = "depth_camera", Type = SensorType.DepthCamera };
        depthSensor.UpdateRate = 15.0;
        depthSensor.RawPose = new Pose3d(0.3, 0.05, 0.1, 0, 0, 0);
        depthSensor.Camera = new CameraSensor
        {
            Name = "depth_cam",
            HorizontalFov = 1.047, // 60°
            ImageWidth = 640,
            ImageHeight = 480,
            ImageFormat = "R_FLOAT32",
            NearClip = 0.1,
            FarClip = 10.0,
        };
        sensorLink.AddSensor(depthSensor);

        // ---- IMU sensor ----
        var imuSensor = new Sensor { Name = "body_imu", Type = SensorType.Imu };
        imuSensor.UpdateRate = 200.0;
        imuSensor.RawPose = new Pose3d(0, 0, 0.05, 0, 0, 0);
        // IMU-specific configuration is stored in Imu sub-element; for now we show
        // that it's a sensor attached to the link with correct type and rate.
        sensorLink.AddSensor(imuSensor);

        // ---- GPU Lidar sensor ----
        var lidarSensor = new Sensor { Name = "lidar_360", Type = SensorType.GpuLidar };
        lidarSensor.UpdateRate = 10.0;
        lidarSensor.RawPose = new Pose3d(0, 0, 0.2, 0, 0, 0);
        // Lidar configuration through generic properties
        sensorLink.AddSensor(lidarSensor);

        // ---- Force-torque sensor ----
        var ftSensor = new Sensor { Name = "wrist_ft", Type = SensorType.ForceTorque };
        ftSensor.UpdateRate = 500.0;
        sensorLink.AddSensor(ftSensor);

        // ---- Contact sensor ----
        var contactSensor = new Sensor { Name = "bumper", Type = SensorType.Contact };
        contactSensor.UpdateRate = 100.0;
        sensorLink.AddSensor(contactSensor);

        // ---- Altimeter sensor ----
        var altSensor = new Sensor { Name = "altitude", Type = SensorType.Altimeter };
        altSensor.UpdateRate = 20.0;
        sensorLink.AddSensor(altSensor);

        // ---- Air pressure sensor ----
        var pressureSensor = new Sensor { Name = "baro", Type = SensorType.AirPressure };
        pressureSensor.UpdateRate = 50.0;
        sensorLink.AddSensor(pressureSensor);

        model.AddLink(sensorLink);

        // ----- Print summary -----
        Console.WriteLine($"  Model: {model.Name}");
        Console.WriteLine($"  Sensors on '{sensorLink.Name}':\n");

        for (int i = 0; i < sensorLink.SensorCount; i++)
        {
            var s = sensorLink.SensorByIndex(i)!;
            Console.WriteLine($"  [{i + 1}] {s.Name}");
            Console.WriteLine($"      Type       : {s.Type}");
            Console.WriteLine($"      Update rate : {s.UpdateRate} Hz");
            Console.WriteLine($"      Pose        : {s.RawPose}");

            if (s.Camera != null)
            {
                var cam = s.Camera;
                Console.WriteLine($"      Camera      : {cam.ImageWidth}x{cam.ImageHeight} {cam.ImageFormat}");
                Console.WriteLine($"                    hfov={cam.HorizontalFov:F3} rad, " +
                                  $"clip=[{cam.NearClip}, {cam.FarClip}]");
                if (cam.ImageNoise != null)
                {
                    Console.WriteLine($"      Noise       : {cam.ImageNoise.Type}, " +
                                      $"mean={cam.ImageNoise.Mean}, stddev={cam.ImageNoise.StdDev}");
                }
            }
            Console.WriteLine();
        }

        // Serialize
        Console.WriteLine("  --- Sensor Testbed SDF XML ---");
        var rootElem = SdfDocument.WrapInRoot(model.ToElement());
        Console.WriteLine(rootElem.ToString("  "));
    }
}
