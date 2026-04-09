// Example 5: Comprehensive parse verification
//
// Loads a complex SDF file that exercises every major element type and
// verifies each field was parsed correctly. Prints PASS / FAIL per check.
//
// Covers: World, Atmosphere, Scene+Sky, Physics, SphericalCoordinates, Gui,
//         Model (static, nested), Link (inertial, wind, gravity, kinematic),
//         Joint (revolute, ball, fixed, prismatic) + axis dynamics/limits,
//         Visual (material, PBR, transparency, laser_retro, visibility_flags),
//         Collision + Surface (ODE/Bullet/Torsional friction, Contact),
//         All geometry shapes: Box, Sphere, Cylinder, Capsule, Ellipsoid,
//         Mesh, Heightmap, Plane,
//         Sensors: Camera, DepthCamera, IMU, GPU-Lidar, ForceTorque,
//         Contact, Altimeter, AirPressure, Magnetometer, NavSat,
//         Noise (gaussian + bias + dynamic bias),
//         Light (directional, point, spot) + attenuation,
//         Actor + Animation + Trajectory + Waypoints,
//         Frame, Plugin

using System;
using System.IO;
using SDFormat;
using SDFormat.Math;

namespace Examples
{

public static class Example5_ComprehensiveParse
{
    private static int _pass;
    private static int _fail;

    public static void Run()
    {
        Console.WriteLine("=== Example 5: Comprehensive Parse Verification ===\n");
        _pass = 0;
        _fail = 0;

        var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
        var sdfPath = Path.Combine(dataDir, "complex_world.sdf");

        if (!File.Exists(sdfPath))
        {
            Console.WriteLine($"  ERROR: {sdfPath} not found.");
            return;
        }

        var root = new Root();
        var errors = root.Load(sdfPath);

        if (errors.Count > 0)
        {
            Console.WriteLine($"  Parse errors ({errors.Count}):");
            foreach (var e in errors)
                Console.WriteLine($"    {e}");
        }

        Check("SDF version", root.Version, "1.12");
        Check("WorldCount", root.WorldCount, 1);

        var world = root.WorldByIndex(0)!;
        VerifyWorld(world);
        VerifyAtmosphere(world);
        VerifyScene(world);
        VerifyPhysics(world);
        VerifySphericalCoordinates(world);
        VerifyGui(world);
        VerifyGroundPlane(world);
        VerifyGeometryShowcase(world);
        VerifySensorRobot(world);
        VerifyWarehouse(world);
        VerifyLights(world);
        VerifyActor(world);
        VerifyWorldFrames(world);
        VerifyNewElements(world);

        Console.WriteLine($"\n  ========================================");
        Console.WriteLine($"  Results: {_pass} passed, {_fail} failed, {_pass + _fail} total");
        Console.WriteLine($"  ========================================");
    }

    // ----------------------------------------------------------------
    // World level
    // ----------------------------------------------------------------

    private static void VerifyWorld(World world)
    {
        Console.WriteLine("\n  --- World ---");
        Check("World.Name", world.Name, "test_world");
        CheckClose("Gravity.Z", world.Gravity.Z, -9.81);
        CheckClose("MagneticField.X", world.MagneticField.X, 5.5645e-6, 1e-10);
        CheckClose("WindLinearVelocity.X", world.WindLinearVelocity.X, 1.5);
        CheckClose("WindLinearVelocity.Y", world.WindLinearVelocity.Y, 0.5);
    }

    private static void VerifyAtmosphere(World world)
    {
        Console.WriteLine("\n  --- Atmosphere ---");
        var atm = world.AtmosphereInfo;
        CheckNotNull("Atmosphere", atm);
        if (atm == null) return;

        Check("Atmosphere.Type", atm.Type, AtmosphereType.Adiabatic);
        CheckClose("Temperature (K)", atm.Temperature.Kelvin, 288.15);
        CheckClose("TemperatureGradient", atm.TemperatureGradient, -0.0065);
        CheckClose("Pressure", atm.Pressure, 101325);
    }

    private static void VerifyScene(World world)
    {
        Console.WriteLine("\n  --- Scene ---");
        var scene = world.SceneInfo;
        CheckNotNull("Scene", scene);
        if (scene == null) return;

        CheckClose("Ambient.R", scene.Ambient.R, 0.4f, 0.01f);
        Check("Shadows", scene.Shadows, true);
        Check("Grid", scene.Grid, true);
        Check("OriginVisual", scene.OriginVisual, true);
        CheckClose("Background.B", scene.Background.B, 0.95f, 0.01f);

        var sky = scene.SkySettings;
        CheckNotNull("Sky", sky);
        if (sky == null) return;
        CheckClose("Sky.Time", sky.Time, 12.0);
        CheckClose("Sky.Sunrise", sky.Sunrise, 6.0);
        CheckClose("Sky.Sunset", sky.Sunset, 18.0);
        CheckClose("CloudSpeed", sky.CloudSpeed, 5.0);
        CheckClose("CloudHumidity", sky.CloudHumidity, 0.6);
        CheckClose("CloudMeanSize", sky.CloudMeanSize, 0.5);
    }

    private static void VerifyPhysics(World world)
    {
        Console.WriteLine("\n  --- Physics ---");
        Check("PhysicsCount", world.PhysicsCount, 2);

        var fast = world.PhysicsByIndex(0)!;
        Check("Physics[0].Name", fast.Name, "fast_physics");
        Check("Physics[0].EngineType", fast.EngineType, "ode");
        CheckClose("Physics[0].MaxStepSize", fast.MaxStepSize, 0.002);
        Check("Physics[0].MaxContacts", fast.MaxContacts, 20);

        var precise = world.PhysicsByIndex(1)!;
        Check("Physics[1].Name", precise.Name, "precise_physics");
        Check("Physics[1].EngineType", precise.EngineType, "bullet");
        CheckClose("Physics[1].MaxStepSize", precise.MaxStepSize, 0.0005);
        CheckClose("Physics[1].RealTimeFactor", precise.RealTimeFactor, 0.5);
        Check("Physics[1].MaxContacts", precise.MaxContacts, 50);
        Check("Physics[1].IsDefault", precise.IsDefault, true);
    }

    private static void VerifySphericalCoordinates(World world)
    {
        Console.WriteLine("\n  --- SphericalCoordinates ---");
        var sc = world.SphericalCoordinatesInfo;
        CheckNotNull("SphericalCoordinates", sc);
        if (sc == null) return;
        CheckClose("Latitude", sc.LatitudeDeg, 37.386, 0.001);
        CheckClose("Longitude", sc.LongitudeDeg, -122.083, 0.001);
        CheckClose("Elevation", sc.ElevationM, 10.5);
        CheckClose("Heading", sc.HeadingDeg, 90);
        Check("WorldFrameOrientation", sc.WorldFrameOrientation, "ENU");
    }

    private static void VerifyGui(World world)
    {
        Console.WriteLine("\n  --- GUI ---");
        var gui = world.GuiInfo;
        CheckNotNull("Gui", gui);
        if (gui == null) return;
        Check("Gui.Fullscreen", gui.Fullscreen, false);
        Check("Gui.Plugins.Count", gui.Plugins.Count, 1);
        Check("GuiPlugin.Name", gui.Plugins[0].Name, "gui_plugin");
        Check("GuiPlugin.Filename", gui.Plugins[0].Filename, "libGuiPlugin.so");
    }

    // ----------------------------------------------------------------
    // Ground plane – Plane geometry + Surface/Friction + Material
    // ----------------------------------------------------------------

    private static void VerifyGroundPlane(World world)
    {
        Console.WriteLine("\n  --- Ground Plane (Surface/Friction) ---");
        var gp = world.ModelByIndex(0)!;
        Check("GroundPlane.Name", gp.Name, "ground_plane");
        Check("GroundPlane.Static", gp.Static, true);

        var link = gp.LinkByIndex(0)!;

        // Collision – plane geometry + friction
        var col = link.CollisionByIndex(0)!;
        Check("Col.Geom.Type", col.Geom.Type, GeometryType.Plane);
        var plane = col.Geom.PlaneShape!;
        CheckClose("Plane.Normal.Z", plane.Normal.Z, 1.0);
        CheckClose("Plane.Size.X", plane.Size.X, 200.0);

        var surf = col.SurfaceInfo;
        CheckNotNull("Surface", surf);
        if (surf != null)
        {
            CheckNotNull("Contact", surf.ContactInfo);
            if (surf.ContactInfo != null)
                Check("CollideBitmask", surf.ContactInfo.CollideBitmask, (ushort)0xffff);

            var fric = surf.FrictionInfo;
            CheckNotNull("Friction", fric);
            if (fric != null)
            {
                CheckNotNull("ODE", fric.Ode);
                if (fric.Ode != null)
                {
                    CheckClose("ODE.Mu", fric.Ode.Mu, 100);
                    CheckClose("ODE.Mu2", fric.Ode.Mu2, 50);
                    CheckClose("ODE.Slip1", fric.Ode.Slip1, 0.01);
                    CheckClose("ODE.Slip2", fric.Ode.Slip2, 0.01);
                    CheckClose("ODE.Fdir1.X", fric.Ode.Fdir1.X, 1.0);
                }

                CheckNotNull("Bullet", fric.Bullet);
                if (fric.Bullet != null)
                {
                    CheckClose("Bullet.Friction", fric.Bullet.Friction, 0.8);
                    CheckClose("Bullet.Friction2", fric.Bullet.Friction2, 0.6);
                    CheckClose("Bullet.RollingFriction", fric.Bullet.RollingFriction, 0.01);
                }

                CheckNotNull("Torsional", fric.TorsionalFriction);
                if (fric.TorsionalFriction != null)
                {
                    CheckClose("Torsional.Coefficient", fric.TorsionalFriction.Coefficient, 0.5);
                    Check("Torsional.UsePatchRadius", fric.TorsionalFriction.UsePatchRadius, true);
                    CheckClose("Torsional.PatchRadius", fric.TorsionalFriction.PatchRadius, 0.05);
                    CheckClose("Torsional.SurfaceRadius", fric.TorsionalFriction.SurfaceRadius, 0.02);
                    CheckClose("Torsional.OdeSlip", fric.TorsionalFriction.OdeSlip, 0.001);
                }
            }
        }

        // Visual – material properties
        var vis = link.VisualByIndex(0)!;
        var mat = vis.MaterialInfo;
        CheckNotNull("Material", mat);
        if (mat != null)
        {
            CheckClose("Mat.Ambient.R", mat.Ambient.R, 0.6f, 0.01f);
            CheckClose("Mat.Specular.R", mat.Specular.R, 0.1f, 0.01f);
            Check("Mat.Lighting", mat.Lighting, true);
            Check("Mat.DoubleSided", mat.DoubleSided, false);
            CheckClose("Mat.Shininess", mat.Shininess, 30.0);
        }
        Check("Visual.CastShadows", vis.CastShadows, false);
        CheckClose("Visual.Transparency", vis.Transparency, 0f, 0.01f);
        Check("Visual.HasLaserRetro", vis.HasLaserRetro, true);
        CheckClose("Visual.LaserRetro", vis.LaserRetro, 50.0);
        Check("Visual.VisibilityFlags", vis.VisibilityFlags, 1u);
    }

    // ----------------------------------------------------------------
    // Geometry showcase – all shape types
    // ----------------------------------------------------------------

    private static void VerifyGeometryShowcase(World world)
    {
        Console.WriteLine("\n  --- Geometry Showcase ---");
        var model = world.ModelByIndex(1)!;
        Check("Model.Name", model.Name, "geometry_showcase");
        Check("Model.Static", model.Static, true);

        // Box
        var boxLink = model.LinkByName("box_link")!;
        var boxVis = boxLink.VisualByIndex(0)!;
        Check("BoxGeom.Type", boxVis.Geom.Type, GeometryType.Box);
        CheckClose("Box.Size.X", boxVis.Geom.BoxShape!.Size.X, 1.0);

        // Inertia with products
        CheckClose("Box.Mass", boxLink.Inertial.Mass, 1.0);
        CheckClose("Box.Ixx", boxLink.Inertial.Ixx, 0.167);
        CheckClose("Box.Ixy", boxLink.Inertial.Ixy, 0.001);
        CheckClose("Box.Ixz", boxLink.Inertial.Ixz, 0.002);
        CheckClose("Box.Iyz", boxLink.Inertial.Iyz, 0.003);

        // Sphere
        var sphereLink = model.LinkByName("sphere_link")!;
        Check("SphereGeom.Type", sphereLink.VisualByIndex(0)!.Geom.Type, GeometryType.Sphere);
        CheckClose("Sphere.Radius", sphereLink.VisualByIndex(0)!.Geom.SphereShape!.Radius, 0.5);

        // Cylinder
        var cylLink = model.LinkByName("cylinder_link")!;
        var cylGeom = cylLink.VisualByIndex(0)!.Geom;
        Check("CylinderGeom.Type", cylGeom.Type, GeometryType.Cylinder);
        CheckClose("Cylinder.Radius", cylGeom.CylinderShape!.Radius, 0.3);
        CheckClose("Cylinder.Length", cylGeom.CylinderShape!.Length, 1.0);

        // Capsule
        var capLink = model.LinkByName("capsule_link")!;
        var capGeom = capLink.VisualByIndex(0)!.Geom;
        Check("CapsuleGeom.Type", capGeom.Type, GeometryType.Capsule);
        CheckClose("Capsule.Radius", capGeom.CapsuleShape!.Radius, 0.2);
        CheckClose("Capsule.Length", capGeom.CapsuleShape!.Length, 0.8);

        // Ellipsoid
        var ellLink = model.LinkByName("ellipsoid_link")!;
        var ellGeom = ellLink.VisualByIndex(0)!.Geom;
        Check("EllipsoidGeom.Type", ellGeom.Type, GeometryType.Ellipsoid);
        CheckClose("Ellipsoid.Radii.X", ellGeom.EllipsoidShape!.Radii.X, 0.5);
        CheckClose("Ellipsoid.Radii.Y", ellGeom.EllipsoidShape!.Radii.Y, 0.3);
        CheckClose("Ellipsoid.Radii.Z", ellGeom.EllipsoidShape!.Radii.Z, 0.2);

        // Mesh
        var meshLink = model.LinkByName("mesh_link")!;
        var meshGeom = meshLink.VisualByIndex(0)!.Geom;
        Check("MeshGeom.Type", meshGeom.Type, GeometryType.Mesh);
        Check("Mesh.Uri", meshGeom.MeshShape!.Uri, "model://my_robot/meshes/body.dae");
        Check("Mesh.Submesh", meshGeom.MeshShape!.Submesh, "wheel");
        Check("Mesh.CenterSubmesh", meshGeom.MeshShape!.CenterSubmesh, true);
        CheckClose("Mesh.Scale.X", meshGeom.MeshShape!.Scale.X, 0.5);

        // Heightmap
        var hmLink = model.LinkByName("heightmap_link")!;
        var hmGeom = hmLink.VisualByIndex(0)!.Geom;
        Check("HeightmapGeom.Type", hmGeom.Type, GeometryType.Heightmap);
        Check("Heightmap.Uri", hmGeom.HeightmapShape!.Uri, "file://media/terrain.png");
        CheckClose("Heightmap.Size.X", hmGeom.HeightmapShape!.Size.X, 129);
        CheckClose("Heightmap.Size.Z", hmGeom.HeightmapShape!.Size.Z, 10);
        Check("Heightmap.Textures.Count", hmGeom.HeightmapShape!.Textures.Count, 2);
        Check("Heightmap.Blends.Count", hmGeom.HeightmapShape!.Blends.Count, 1);
        if (hmGeom.HeightmapShape!.Textures.Count >= 2)
        {
            Check("Texture[0].Diffuse", hmGeom.HeightmapShape!.Textures[0].Diffuse, "file://media/dirt.png");
            Check("Texture[0].Normal", hmGeom.HeightmapShape!.Textures[0].Normal, "file://media/dirt_normal.png");
            CheckClose("Texture[0].Size", hmGeom.HeightmapShape!.Textures[0].Size, 10);
            Check("Texture[1].Diffuse", hmGeom.HeightmapShape!.Textures[1].Diffuse, "file://media/grass.png");
            CheckClose("Texture[1].Size", hmGeom.HeightmapShape!.Textures[1].Size, 20);
        }
        if (hmGeom.HeightmapShape!.Blends.Count >= 1)
        {
            CheckClose("Blend[0].MinHeight", hmGeom.HeightmapShape!.Blends[0].MinHeight, 5);
            CheckClose("Blend[0].FadeDistance", hmGeom.HeightmapShape!.Blends[0].FadeDistance, 2);
        }

        // Frame
        Check("FrameCount", model.FrameCount, 1);
        var frame = model.FrameByIndex(0)!;
        Check("Frame.Name", frame.Name, "showcase_origin");
        Check("Frame.AttachedTo", frame.AttachedTo, "box_link");
    }

    // ----------------------------------------------------------------
    // Sensor robot – sensors, joints, link properties, PBR, plugins
    // ----------------------------------------------------------------

    private static void VerifySensorRobot(World world)
    {
        Console.WriteLine("\n  --- Sensor Robot ---");
        var model = world.ModelByIndex(2)!;
        Check("Model.Name", model.Name, "sensor_robot");
        Check("SelfCollide", model.SelfCollide, true);

        var baseLink = model.LinkByName("base")!;
        CheckClose("Base.Mass", baseLink.Inertial.Mass, 10);
        CheckClose("Base.Ixx", baseLink.Inertial.Ixx, 0.833);
        Check("EnableWind", baseLink.EnableWind, true);
        Check("EnableGravity", baseLink.EnableGravity, true);
        Check("Kinematic", baseLink.Kinematic, false);

        // PBR material
        var baseVis = baseLink.VisualByIndex(0)!;
        var mat = baseVis.MaterialInfo!;
        CheckNotNull("PBR", mat.PbrMaterial);
        if (mat.PbrMaterial != null)
        {
            var metal = mat.PbrMaterial.GetWorkflow(PbrWorkflowType.Metal);
            CheckNotNull("PBR.Metal", metal);
            if (metal != null)
            {
                Check("PBR.AlbedoMap", metal.AlbedoMap, "textures/base_albedo.png");
                Check("PBR.NormalMap", metal.NormalMap, "textures/base_normal.png");
                Check("PBR.NormalMapType", metal.NormalMapType, NormalMapSpace.Tangent);
                Check("PBR.RoughnessMap", metal.RoughnessMap, "textures/base_roughness.png");
                Check("PBR.MetalnessMap", metal.MetalnessMap, "textures/base_metalness.png");
                CheckClose("PBR.Metalness", metal.Metalness, 0.7);
                CheckClose("PBR.Roughness", metal.Roughness, 0.3);
                Check("PBR.EnvironmentMap", metal.EnvironmentMap, "textures/env.dds");
                Check("PBR.EmissiveMap", metal.EmissiveMap, "textures/base_emissive.png");
                Check("PBR.LightMap", metal.LightMap, "textures/base_lightmap.png");
                Check("PBR.LightMapTexCoordSet", metal.LightMapTexCoordSet, 1u);
            }
        }
        CheckClose("Visual.Transparency", baseVis.Transparency, 0.1f, 0.01f);
        Check("Visual.HasLaserRetro", baseVis.HasLaserRetro, true);
        CheckClose("Visual.LaserRetro", baseVis.LaserRetro, 100.0);
        Check("Visual.VisibilityFlags", baseVis.VisibilityFlags, 3u);

        // ---- Sensors ----
        VerifyCameraSensors(baseLink);
        VerifyImuSensor(baseLink);
        VerifyLidarSensor(baseLink);
        VerifyForceTorqueSensor(baseLink);
        VerifyContactSensor(baseLink);
        VerifyAltimeterSensor(baseLink);
        VerifyAirPressureSensor(baseLink);
        VerifyMagnetometerSensor(baseLink);
        VerifyNavSatSensor(baseLink);

        // Link-attached light (spot)
        VerifyLinkLight(baseLink);

        // ---- Joints ----
        VerifyJoints(model);

        // ---- Frames ----
        Check("Model.FrameCount", model.FrameCount, 2);
        var camFrame = model.FrameByIndex(0)!;
        Check("CamFrame.Name", camFrame.Name, "camera_optical");
        Check("CamFrame.AttachedTo", camFrame.AttachedTo, "base");

        // ---- Plugins ----
        Check("Plugins.Count", model.Plugins.Count, 2);
        Check("Plugin[0].Name", model.Plugins[0].Name, "diff_drive");
        Check("Plugin[0].Filename", model.Plugins[0].Filename, "libDiffDrive.so");
        Check("Plugin[1].Name", model.Plugins[1].Name, "joint_controller");
    }

    private static void VerifyCameraSensors(Link link)
    {
        Console.WriteLine("\n  --- Camera Sensors ---");
        var cam = link.SensorByName("front_camera")!;
        Check("Camera.Type", cam.Type, SensorType.Camera);
        CheckClose("Camera.UpdateRate", cam.UpdateRate, 30);
        Check("Camera.Topic", cam.Topic, "/robot/camera");
        CheckNotNull("Camera.Camera", cam.Camera);
        if (cam.Camera != null)
        {
            Check("Camera.Name", cam.Camera.Name, "rgb_cam");
            CheckClose("Camera.HFov", cam.Camera.HorizontalFov, 1.396);
            Check("Camera.Width", cam.Camera.ImageWidth, 1920u);
            Check("Camera.Height", cam.Camera.ImageHeight, 1080u);
            Check("Camera.Format", cam.Camera.ImageFormat, "R8G8B8");
            CheckClose("Camera.Near", cam.Camera.NearClip, 0.05);
            CheckClose("Camera.Far", cam.Camera.FarClip, 300);
            CheckClose("Camera.DepthNear", cam.Camera.DepthNearClip, 0.1);
            CheckClose("Camera.DepthFar", cam.Camera.DepthFarClip, 50);
            Check("Camera.SaveFrames", cam.Camera.SaveFrames, true);
            Check("Camera.SavePath", cam.Camera.SavePath, "/tmp/camera_frames");
            CheckNotNull("Camera.Noise", cam.Camera.ImageNoise);
            if (cam.Camera.ImageNoise != null)
            {
                Check("CamNoise.Type", cam.Camera.ImageNoise.Type, NoiseType.Gaussian);
                CheckClose("CamNoise.StdDev", cam.Camera.ImageNoise.StdDev, 0.007);
            }
        }

        // Depth camera
        var depth = link.SensorByName("depth_cam")!;
        Check("DepthCam.Type", depth.Type, SensorType.DepthCamera);
        CheckClose("DepthCam.UpdateRate", depth.UpdateRate, 15);
        CheckNotNull("DepthCam.Camera", depth.Camera);
        if (depth.Camera != null)
        {
            Check("DepthCam.Width", depth.Camera.ImageWidth, 640u);
            Check("DepthCam.Height", depth.Camera.ImageHeight, 480u);
            Check("DepthCam.Format", depth.Camera.ImageFormat, "R_FLOAT32");
        }
    }

    private static void VerifyImuSensor(Link link)
    {
        Console.WriteLine("\n  --- IMU Sensor ---");
        var imu = link.SensorByName("body_imu")!;
        Check("IMU.Type", imu.Type, SensorType.Imu);
        CheckClose("IMU.UpdateRate", imu.UpdateRate, 200);

        CheckNotNull("IMU.Imu", imu.Imu);
        if (imu.Imu == null) return;

        // Linear acceleration noise
        CheckNotNull("AccelX.Noise", imu.Imu.LinearAccelerationXNoise);
        Check("AccelXNoise.Type", imu.Imu.LinearAccelerationXNoise.Type, NoiseType.Gaussian);
        CheckClose("AccelXNoise.StdDev", imu.Imu.LinearAccelerationXNoise.StdDev, 0.01);
        CheckClose("AccelXNoise.BiasMean", imu.Imu.LinearAccelerationXNoise.BiasMean, 0.001);
        CheckClose("AccelXNoise.BiasStdDev", imu.Imu.LinearAccelerationXNoise.BiasStdDev, 0.0001);
        CheckClose("AccelXNoise.DynBiasStdDev", imu.Imu.LinearAccelerationXNoise.DynamicBiasStdDev, 0.00002);
        CheckClose("AccelXNoise.DynBiasCorrTime", imu.Imu.LinearAccelerationXNoise.DynamicBiasCorrelationTime, 300);

        // Angular velocity noise
        Check("GyroZNoise.Type", imu.Imu.AngularVelocityZNoise.Type, NoiseType.Gaussian);
        CheckClose("GyroZNoise.StdDev", imu.Imu.AngularVelocityZNoise.StdDev, 0.005);

        Check("OrientationEnabled", imu.Imu.OrientationEnabled, true);
    }

    private static void VerifyLidarSensor(Link link)
    {
        Console.WriteLine("\n  --- Lidar Sensor ---");
        var lidar = link.SensorByName("lidar_360")!;
        Check("Lidar.Type", lidar.Type, SensorType.GpuLidar);
        Check("Lidar.Topic", lidar.Topic, "/robot/lidar");

        CheckNotNull("Lidar.Lidar", lidar.Lidar);
        if (lidar.Lidar == null) return;

        Check("H.Samples", lidar.Lidar.HorizontalScanSamples, 640u);
        CheckClose("H.Resolution", lidar.Lidar.HorizontalScanResolution, 1.0);
        CheckClose("H.MinAngle", lidar.Lidar.HorizontalScanMinAngle.Radians, -3.14159, 0.001);
        CheckClose("H.MaxAngle", lidar.Lidar.HorizontalScanMaxAngle.Radians, 3.14159, 0.001);
        Check("V.Samples", lidar.Lidar.VerticalScanSamples, 16u);
        CheckClose("V.MinAngle", lidar.Lidar.VerticalScanMinAngle.Radians, -0.2618, 0.001);
        CheckClose("V.MaxAngle", lidar.Lidar.VerticalScanMaxAngle.Radians, 0.2618, 0.001);
        CheckClose("Range.Min", lidar.Lidar.RangeMin, 0.08);
        CheckClose("Range.Max", lidar.Lidar.RangeMax, 100.0);
        CheckClose("Range.Resolution", lidar.Lidar.RangeResolution, 0.01);

        CheckNotNull("RangeNoise", lidar.Lidar.RangeNoise);
        Check("RangeNoise.Type", lidar.Lidar.RangeNoise.Type, NoiseType.Gaussian);
        CheckClose("RangeNoise.StdDev", lidar.Lidar.RangeNoise.StdDev, 0.02);
    }

    private static void VerifyForceTorqueSensor(Link link)
    {
        Console.WriteLine("\n  --- ForceTorque Sensor ---");
        var ft = link.SensorByName("wrist_ft")!;
        Check("FT.Type", ft.Type, SensorType.ForceTorque);
        CheckClose("FT.UpdateRate", ft.UpdateRate, 500);

        CheckNotNull("FT.ForceTorque", ft.ForceTorque);
        if (ft.ForceTorque == null) return;

        Check("FT.Frame", ft.ForceTorque.Frame, ForceTorqueSensor.FrameType.Child);
        Check("FT.MeasureDirection", ft.ForceTorque.MeasureDirection,
            ForceTorqueSensor.MeasureDirectionType.ChildToParent);

        Check("ForceXNoise.Type", ft.ForceTorque.ForceXNoise.Type, NoiseType.Gaussian);
        CheckClose("ForceXNoise.StdDev", ft.ForceTorque.ForceXNoise.StdDev, 0.1);
        Check("TorqueXNoise.Type", ft.ForceTorque.TorqueXNoise.Type, NoiseType.Gaussian);
        CheckClose("TorqueXNoise.StdDev", ft.ForceTorque.TorqueXNoise.StdDev, 0.01);
    }

    private static void VerifyContactSensor(Link link)
    {
        Console.WriteLine("\n  --- Contact Sensor ---");
        var contact = link.SensorByName("bumper")!;
        Check("Contact.Type", contact.Type, SensorType.Contact);
        CheckClose("Contact.UpdateRate", contact.UpdateRate, 100);
    }

    private static void VerifyAltimeterSensor(Link link)
    {
        Console.WriteLine("\n  --- Altimeter Sensor ---");
        var alt = link.SensorByName("altitude")!;
        Check("Altimeter.Type", alt.Type, SensorType.Altimeter);
        CheckClose("Altimeter.UpdateRate", alt.UpdateRate, 20);

        CheckNotNull("Altimeter.Altimeter", alt.Altimeter);
        if (alt.Altimeter == null) return;

        Check("AltPosNoise.Type", alt.Altimeter.VerticalPositionNoise.Type, NoiseType.Gaussian);
        CheckClose("AltPosNoise.StdDev", alt.Altimeter.VerticalPositionNoise.StdDev, 0.1);
        Check("AltVelNoise.Type", alt.Altimeter.VerticalVelocityNoise.Type, NoiseType.Gaussian);
        CheckClose("AltVelNoise.StdDev", alt.Altimeter.VerticalVelocityNoise.StdDev, 0.02);
    }

    private static void VerifyAirPressureSensor(Link link)
    {
        Console.WriteLine("\n  --- AirPressure Sensor ---");
        var ap = link.SensorByName("baro")!;
        Check("AirPressure.Type", ap.Type, SensorType.AirPressure);

        CheckNotNull("AP.AirPressure", ap.AirPressure);
        if (ap.AirPressure == null) return;

        CheckClose("AP.RefAltitude", ap.AirPressure.ReferenceAltitude, 100);
        Check("AP.Noise.Type", ap.AirPressure.PressureNoise.Type, NoiseType.Gaussian);
        CheckClose("AP.Noise.StdDev", ap.AirPressure.PressureNoise.StdDev, 10);
    }

    private static void VerifyMagnetometerSensor(Link link)
    {
        Console.WriteLine("\n  --- Magnetometer Sensor ---");
        var mag = link.SensorByName("mag")!;
        Check("Mag.Type", mag.Type, SensorType.Magnetometer);

        CheckNotNull("Mag.Magnetometer", mag.Magnetometer);
        if (mag.Magnetometer == null) return;

        Check("MagXNoise.Type", mag.Magnetometer.XNoise.Type, NoiseType.Gaussian);
        CheckClose("MagXNoise.StdDev", mag.Magnetometer.XNoise.StdDev, 0.001);
    }

    private static void VerifyNavSatSensor(Link link)
    {
        Console.WriteLine("\n  --- NavSat Sensor ---");
        var gps = link.SensorByName("gps")!;
        Check("NavSat.Type", gps.Type, SensorType.NavSat);
        CheckClose("NavSat.UpdateRate", gps.UpdateRate, 5);

        CheckNotNull("NavSat.NavSat", gps.NavSat);
        if (gps.NavSat == null) return;

        Check("HPosNoise.Type", gps.NavSat.HorizontalPositionNoise.Type, NoiseType.Gaussian);
        CheckClose("HPosNoise.StdDev", gps.NavSat.HorizontalPositionNoise.StdDev, 0.5);
        Check("VPosNoise.Type", gps.NavSat.VerticalPositionNoise.Type, NoiseType.Gaussian);
        CheckClose("VPosNoise.StdDev", gps.NavSat.VerticalPositionNoise.StdDev, 1.0);
        Check("HVelNoise.Type", gps.NavSat.HorizontalVelocityNoise.Type, NoiseType.Gaussian);
        CheckClose("HVelNoise.StdDev", gps.NavSat.HorizontalVelocityNoise.StdDev, 0.05);
        Check("VVelNoise.Type", gps.NavSat.VerticalVelocityNoise.Type, NoiseType.Gaussian);
        CheckClose("VVelNoise.StdDev", gps.NavSat.VerticalVelocityNoise.StdDev, 0.1);
    }

    private static void VerifyLinkLight(Link link)
    {
        Console.WriteLine("\n  --- Link Light (Spot) ---");
        Check("Link.LightCount", link.LightCount, 1);
        var light = link.LightByIndex(0)!;
        Check("Light.Name", light.Name, "headlight");
        Check("Light.Type", light.Type, LightType.Spot);
        Check("Light.CastShadows", light.CastShadows, true);
        CheckClose("Light.Intensity", light.Intensity, 2.5);
        CheckClose("Light.Diffuse.R", light.Diffuse.R, 1.0f, 0.01f);
        CheckClose("Light.Specular.G", light.Specular.G, 1.0f, 0.01f);
        CheckClose("Light.AttenuationRange", light.AttenuationRange, 30);
        CheckClose("Light.ConstantAttenuation", light.ConstantAttenuationFactor, 0.1);
        CheckClose("Light.LinearAttenuation", light.LinearAttenuationFactor, 0.01);
        CheckClose("Light.QuadraticAttenuation", light.QuadraticAttenuationFactor, 0.001);
        CheckClose("Light.SpotInnerAngle", light.SpotInnerAngle.Radians, 0.3);
        CheckClose("Light.SpotOuterAngle", light.SpotOuterAngle.Radians, 0.6);
        CheckClose("Light.SpotFalloff", light.SpotFalloff, 1.0);
    }

    private static void VerifyJoints(Model model)
    {
        Console.WriteLine("\n  --- Joints ---");
        Check("JointCount", model.JointCount, 6);

        // Revolute with full axis config
        var lw = model.JointByName("left_wheel_joint")!;
        Check("LW.Type", lw.Type, JointType.Revolute);
        Check("LW.Parent", lw.ParentName, "base");
        Check("LW.Child", lw.ChildName, "left_wheel");
        CheckNotNull("LW.Axis", lw.Axis);
        if (lw.Axis != null)
        {
            CheckClose("LW.Axis.Xyz.Z", lw.Axis.Xyz.Z, 1.0);
            Check("LW.Axis.XyzExpressedIn", lw.Axis.XyzExpressedIn, "__model__");
            CheckClose("LW.Axis.Effort", lw.Axis.Effort, 50);
            CheckClose("LW.Axis.MaxVelocity", lw.Axis.MaxVelocity, 20);
            CheckClose("LW.Axis.Damping", lw.Axis.Damping, 0.1);
            CheckClose("LW.Axis.Friction", lw.Axis.Friction, 0.05);
            CheckClose("LW.Axis.Stiffness", lw.Axis.Stiffness, 1e8);
            CheckClose("LW.Axis.Dissipation", lw.Axis.Dissipation, 1.0);
        }

        // Ball joint (no axis)
        var cj = model.JointByName("caster_joint")!;
        Check("CJ.Type", cj.Type, JointType.Ball);

        // Fixed joint
        var fix = model.JointByName("sensor_mount")!;
        Check("Fix.Type", fix.Type, JointType.Fixed);

        // Prismatic joint
        var lift = model.JointByName("lift_joint")!;
        Check("Lift.Type", lift.Type, JointType.Prismatic);
        CheckNotNull("Lift.Axis", lift.Axis);
        if (lift.Axis != null)
        {
            CheckClose("Lift.Lower", lift.Axis.Lower, 0);
            CheckClose("Lift.Upper", lift.Axis.Upper, 0.5);
            CheckClose("Lift.Effort", lift.Axis.Effort, 100);
        }

        // Screw joint
        var screw = model.JointByName("screw_joint")!;
        Check("Screw.Type", screw.Type, JointType.Screw);
        CheckClose("Screw.ScrewThreadPitch", screw.ScrewThreadPitch, 0.01);
        CheckClose("Screw.ThreadPitch", screw.ThreadPitch, 628.318);
    }

    // ----------------------------------------------------------------
    // Nested model (warehouse)
    // ----------------------------------------------------------------

    private static void VerifyWarehouse(World world)
    {
        Console.WriteLine("\n  --- Nested Model ---");
        var wh = world.ModelByIndex(3)!;
        Check("Warehouse.Name", wh.Name, "warehouse");
        Check("Warehouse.Static", wh.Static, true);
        Check("Warehouse.LinkCount", wh.LinkCount, 1);
        Check("Warehouse.ModelCount", wh.ModelCount, 1);

        var shelf = wh.ModelByIndex(0)!;
        Check("NestedModel.Name", shelf.Name, "shelf_unit");
        Check("NestedModel.LinkCount", shelf.LinkCount, 1);
        Check("ShelfLink.Name", shelf.LinkByIndex(0)!.Name, "shelf");
    }

    // ----------------------------------------------------------------
    // World-level lights (directional, point, spot)
    // ----------------------------------------------------------------

    private static void VerifyLights(World world)
    {
        Console.WriteLine("\n  --- World Lights ---");
        Check("LightCount", world.LightCount, 3);

        // Directional
        var sun = world.LightByIndex(0)!;
        Check("Sun.Name", sun.Name, "sun");
        Check("Sun.Type", sun.Type, LightType.Directional);
        Check("Sun.CastShadows", sun.CastShadows, true);
        CheckClose("Sun.Intensity", sun.Intensity, 1.0);
        CheckClose("Sun.Diffuse.R", sun.Diffuse.R, 0.9f, 0.01f);
        CheckClose("Sun.Direction.X", sun.Direction.X, -0.5);
        CheckClose("Sun.AttenuationRange", sun.AttenuationRange, 1000);
        CheckClose("Sun.ConstantAttenuation", sun.ConstantAttenuationFactor, 0.9);

        // Point
        var lamp = world.LightByIndex(1)!;
        Check("Lamp.Name", lamp.Name, "lamp");
        Check("Lamp.Type", lamp.Type, LightType.Point);
        CheckClose("Lamp.Intensity", lamp.Intensity, 1.5);
        Check("Lamp.CastShadows", lamp.CastShadows, false);

        // Spot
        var spot = world.LightByIndex(2)!;
        Check("Spot.Name", spot.Name, "spotlight");
        Check("Spot.Type", spot.Type, LightType.Spot);
        CheckClose("Spot.Intensity", spot.Intensity, 3.0);
        CheckClose("Spot.SpotInnerAngle", spot.SpotInnerAngle.Radians, 0.2);
        CheckClose("Spot.SpotOuterAngle", spot.SpotOuterAngle.Radians, 0.5);
        CheckClose("Spot.SpotFalloff", spot.SpotFalloff, 1.0);
    }

    // ----------------------------------------------------------------
    // Actor
    // ----------------------------------------------------------------

    private static void VerifyActor(World world)
    {
        Console.WriteLine("\n  --- Actor ---");
        Check("ActorCount", world.ActorCount, 1);
        var actor = world.ActorByIndex(0)!;
        Check("Actor.Name", actor.Name, "walking_person");
        Check("Actor.SkinFilename", actor.SkinFilename, "model://person/meshes/walking.dae");
        CheckClose("Actor.SkinScale", actor.SkinScale, 1.0);

        // Animations
        Check("AnimationCount", actor.AnimationCount, 2);
        var walkAnim = actor.AnimationByIndex(0)!;
        Check("Anim[0].Name", walkAnim.Name, "walk");
        Check("Anim[0].Filename", walkAnim.Filename, "model://person/meshes/walking.dae");
        Check("Anim[0].InterpolateX", walkAnim.InterpolateX, true);
        var idleAnim = actor.AnimationByIndex(1)!;
        Check("Anim[1].Name", idleAnim.Name, "idle");
        Check("Anim[1].InterpolateX", idleAnim.InterpolateX, false);

        // Script
        Check("ScriptLoop", actor.ScriptLoop, true);
        Check("ScriptAutoStart", actor.ScriptAutoStart, true);
        CheckClose("ScriptDelayStart", actor.ScriptDelayStart, 0);

        // Trajectory
        Check("TrajectoryCount", actor.Trajectories.Count, 1);
        var traj = actor.Trajectories[0];
        Check("Traj.Type", traj.Type, "walk");
        Check("WaypointCount", traj.WaypointCount, 3);
        CheckClose("WP[0].Time", traj.WaypointByIndex(0)!.Time, 0);
        CheckClose("WP[1].Time", traj.WaypointByIndex(1)!.Time, 5);
        CheckClose("WP[2].Time", traj.WaypointByIndex(2)!.Time, 10);

        // Actor plugin
        Check("Actor.Plugins.Count", actor.Plugins.Count, 1);
        if (actor.Plugins.Count >= 1)
        {
            Check("ActorPlugin.Name", actor.Plugins[0].Name, "actor_plugin");
            Check("ActorPlugin.Filename", actor.Plugins[0].Filename, "libFollowActor.so");
        }
    }

    // ----------------------------------------------------------------
    // World-level frames
    // ----------------------------------------------------------------

    private static void VerifyWorldFrames(World world)
    {
        Console.WriteLine("\n  --- World Frames ---");
        Check("World.FrameCount", world.FrameCount, 1);
        var frame = world.FrameByIndex(0)!;
        Check("Frame.Name", frame.Name, "world_origin");
    }

    private static void VerifyNewElements(World world)
    {
        Console.WriteLine("\n  --- New Elements ---");

        // Ground plane - Bounce, Contact extras
        var gp = world.ModelByName("ground_plane")!;
        var gpCol = gp.LinkByIndex(0)!.CollisionByIndex(0)!;
        var contact = gpCol.SurfaceInfo!.ContactInfo!;
        Check("Contact.CollideBitmask", contact.CollideBitmask, (ushort)0xFFFF);
        Check("Contact.CollideWithoutContact", contact.CollideWithoutContact, false);
        Check("Contact.CategoryBitmask", contact.CategoryBitmask, (ushort)0x00FF);
        CheckClose("Contact.PoissonsRatio", contact.PoissonsRatio, 0.3);
        CheckClose("Contact.ElasticModulus", contact.ElasticModulus, 1e8);
        var bounce = gpCol.SurfaceInfo!.BounceInfo;
        CheckNotNull("Bounce", bounce);
        if (bounce != null)
        {
            CheckClose("Bounce.RestitutionCoefficient", bounce.RestitutionCoefficient, 0.5);
            CheckClose("Bounce.Threshold", bounce.Threshold, 1000);
        }

        // Sensor robot - Collision density, laser_retro, max_contacts
        var robot = world.ModelByName("sensor_robot")!;
        var baseLink = robot.LinkByName("base")!;
        var baseCol = baseLink.CollisionByName("base_collision")!;
        CheckClose("Collision.Density", baseCol.Density, 2700);
        CheckClose("Collision.LaserRetro", baseCol.LaserRetro, 80);
        Check("Collision.MaxContacts", baseCol.MaxContacts, 15);

        // Link features
        Check("Link.SelfCollide", baseLink.SelfCollide, true);
        Check("Link.MustBeBaseLink", baseLink.MustBeBaseLink, false);
        CheckClose("Link.VelocityDecayLinear", baseLink.VelocityDecayLinear, 0.01);
        CheckClose("Link.VelocityDecayAngular", baseLink.VelocityDecayAngular, 0.02);
        Check("Link.AutoInertia", baseLink.AutoInertia, true);

        // Sensor frame_id, always_on, visualize
        var camSensor = baseLink.SensorByName("front_camera")!;
        Check("Sensor.FrameId", camSensor.FrameId, "camera_optical_frame");
        Check("Sensor.AlwaysOn", camSensor.AlwaysOn, true);
        Check("Sensor.Visualize", camSensor.Visualize, true);
        Check("Sensor.EnableMetrics", camSensor.EnableMetrics, false);

        // Camera distortion, lens, anti_aliasing, triggered
        var cam = camSensor.Camera!;
        Check("Camera.AntiAliasing", cam.AntiAliasingValue, (uint)8);
        Check("Camera.HasTriggered", cam.HasTriggeredCamera, true);
        Check("Camera.TriggerTopic", cam.TriggerTopic, "/camera/trigger");
        Check("Camera.CameraInfoTopic", cam.CameraInfoTopic, "/camera/info");
        Check("Camera.OpticalFrameId", cam.OpticalFrameId, "optical_frame");
        CheckClose("Camera.DistortionK1", cam.DistortionK1, 0.1);
        CheckClose("Camera.DistortionK2", cam.DistortionK2, 0.2);
        CheckClose("Camera.DistortionK3", cam.DistortionK3, 0.3);
        CheckClose("Camera.DistortionP1", cam.DistortionP1, 0.01);
        CheckClose("Camera.DistortionP2", cam.DistortionP2, 0.02);
        CheckClose("Camera.DistortionCenter.X", cam.DistortionCenter.X, 0.5);
        Check("Camera.LensType", cam.LensType, "stereographic");
        Check("Camera.LensScaleToHfov", cam.LensScaleToHfov, true);
        CheckClose("Camera.LensCutoffAngle", cam.LensCutoffAngle, 1.5707);
        Check("Camera.LensEnvTextureSize", cam.LensEnvTextureSize, 512);
        CheckClose("Camera.LensIntrinsicsFx", cam.LensIntrinsicsFx, 960);
        CheckClose("Camera.LensIntrinsicsFy", cam.LensIntrinsicsFy, 960);
        CheckClose("Camera.LensIntrinsicsCx", cam.LensIntrinsicsCx, 960);
        CheckClose("Camera.LensIntrinsicsCy", cam.LensIntrinsicsCy, 540);
        CheckClose("Camera.LensIntrinsicsS", cam.LensIntrinsicsS, 0);

        // IMU orientation_reference_frame
        var imuSensor = baseLink.SensorByName("body_imu")!;
        var imu = imuSensor.Imu!;
        Check("IMU.Localization", imu.Localization, "ENU");
        CheckClose("IMU.CustomRpy.X", imu.CustomRpy.X, 0);
        Check("IMU.CustomRpyParentFrame", imu.CustomRpyParentFrame, "world");
        CheckClose("IMU.GravityDirX.X", imu.GravityDirX.X, 1);
        Check("IMU.GravityDirXParentFrame", imu.GravityDirXParentFrame, "world");

        // Visual meta layer
        var baseVisual = baseLink.VisualByName("base_visual")!;
        Check("Visual.MetaLayer", baseVisual.MetaLayer, 2);

        // Mesh optimization
        var showcase = world.ModelByName("geometry_showcase")!;
        var meshLink = showcase.LinkByName("mesh_link")!;
        var meshVisual = meshLink.VisualByIndex(0)!;
        Check("Mesh.Optimization", meshVisual.Geom.MeshShape!.OptimizationStr, "convex_decomposition");
    }

    // ================================================================
    // Check helpers
    // ================================================================

    private static void Check<T>(string label, T actual, T expected)
    {
        bool ok = Equals(actual, expected);
        if (ok) _pass++;
        else _fail++;
        string status = ok ? "PASS" : "FAIL";
        string detail = ok ? $"{actual}" : $"expected={expected}, actual={actual}";
        Console.WriteLine($"    [{status}] {label}: {detail}");
    }

    private static void CheckClose(string label, double actual, double expected, double tol = 1e-6)
    {
        bool ok = System.Math.Abs(actual - expected) <= tol;
        if (ok) _pass++;
        else _fail++;
        string status = ok ? "PASS" : "FAIL";
        string detail = ok ? $"{actual}" : $"expected={expected}, actual={actual}";
        Console.WriteLine($"    [{status}] {label}: {detail}");
    }

    private static void CheckClose(string label, float actual, float expected, float tol = 1e-4f)
    {
        bool ok = System.Math.Abs(actual - expected) <= tol;
        if (ok) _pass++;
        else _fail++;
        string status = ok ? "PASS" : "FAIL";
        string detail = ok ? $"{actual}" : $"expected={expected}, actual={actual}";
        Console.WriteLine($"    [{status}] {label}: {detail}");
    }

    private static void CheckNotNull(string label, object? obj)
    {
        bool ok = obj != null;
        if (ok) _pass++;
        else _fail++;
        string status = ok ? "PASS" : "FAIL";
        Console.WriteLine($"    [{status}] {label}: {(ok ? "present" : "NULL")}");
    }
}

}
