// Test: Verify nested elements are parsed correctly
//
// Covers:
//   - Deeply nested models (3 levels: top_robot > end_effector > sensor_module)
//   - Links with nested visuals, collisions, sensors
//   - Joints referencing nested model links (:: syntax)
//   - Frames with attached_to and pose relative_to
//   - Grippers referencing nested model links
//   - Surface nesting (friction > ode, contact > ode)
//   - Nested state (model state > nested model state > deeper nested model state)
//   - World-level lights and frames
//   - Multiple sensors in one link

using System;
using System.IO;
using SDFormat;
using SDFormat.Math;

namespace Examples
{

public static class Example8_TestNestedElements
{
    private static int _pass;
    private static int _fail;

    public static void Run()
    {
        Console.WriteLine("=== Test: Verify Nested Elements ===\n");
        _pass = 0;
        _fail = 0;

        var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
        var sdfPath = Path.Combine(dataDir, "test_nested_elements.sdf");

        if (!File.Exists(sdfPath))
        {
            Console.WriteLine($"  ERROR: {sdfPath} not found.");
            return;
        }

        var root = new Root();
        var errors = root.Load(sdfPath);

        Console.WriteLine($"  Parse errors: {errors.Count}");
        foreach (var e in errors)
            Console.WriteLine($"    {e}");

        Check("SDF version", root.Version, "1.12");
        Check("WorldCount", root.WorldCount.ToString(), "1");
        var world = root.WorldByIndex(0);
        Check("World.Name", world.Name, "nested_test_world");

        VerifyTopModel(world);
        VerifySensorTower(world);
        VerifyNestedState(world);
        VerifyWorldLightsAndFrames(world);

        Console.WriteLine();
        Console.WriteLine("  ========================================");
        Console.WriteLine($"  Results: {_pass} passed, {_fail} failed, {_pass + _fail} total");
        Console.WriteLine("  ========================================");

        if (_fail > 0) Environment.ExitCode = 1;
    }

    static void VerifyTopModel(World world)
    {
        Console.WriteLine("\n  --- Top-Level Model (Level 1) ---");

        var top = world.ModelByName("top_robot");
        CheckNotNull("top_robot", top);
        if (top == null) return;

        Check("top.Static", top.Static.ToString(), "False");
        Check("top.SelfCollide", top.SelfCollide.ToString(), "True");
        CheckClose("top.Pose.X", top.RawPose.Position.X, 1.0);
        CheckClose("top.Pose.Y", top.RawPose.Position.Y, 2.0);
        CheckClose("top.Pose.Z", top.RawPose.Position.Z, 3.0);

        // Links
        Check("top.LinkCount", top.LinkCount.ToString(), "2");
        var baseLink = top.LinkByName("base_link");
        CheckNotNull("base_link", baseLink);

        var armLink = top.LinkByName("arm_link");
        CheckNotNull("arm_link", armLink);

        // --- Link > Visual chain ---
        Console.WriteLine("\n  --- Link > Visual > Geometry > Material ---");
        Check("baseLink.Visuals.Count", baseLink!.Visuals.Count.ToString(), "1");
        var baseVisual = baseLink.Visuals[0];
        Check("baseVisual.Name", baseVisual.Name, "base_visual");
        Check("baseVisual.Geom.Type", baseVisual.Geom.Type.ToString(), "Box");
        CheckNotNull("baseVisual.Geom.BoxShape", baseVisual.Geom.BoxShape);
        CheckClose("baseVisual.Box.X", baseVisual.Geom.BoxShape!.Size.X, 1.0);
        CheckClose("baseVisual.Box.Y", baseVisual.Geom.BoxShape.Size.Y, 1.0);
        CheckClose("baseVisual.Box.Z", baseVisual.Geom.BoxShape.Size.Z, 0.5);
        CheckNotNull("baseVisual.MaterialInfo", baseVisual.MaterialInfo);
        CheckClose("baseVisual.Material.Ambient.R", baseVisual.MaterialInfo!.Ambient.R, 0.8);
        CheckClose("baseVisual.Material.Diffuse.G", baseVisual.MaterialInfo.Diffuse.G, 0.3);

        // --- Link > Collision > Surface chain ---
        Console.WriteLine("\n  --- Link > Collision > Surface > Friction/Contact ---");
        Check("baseLink.Collisions.Count", baseLink.Collisions.Count.ToString(), "1");
        var baseColl = baseLink.Collisions[0];
        Check("baseColl.Name", baseColl.Name, "base_collision");
        Check("baseColl.Geom.Type", baseColl.Geom.Type.ToString(), "Box");
        CheckNotNull("baseColl.SurfaceInfo", baseColl.SurfaceInfo);
        CheckNotNull("surface.FrictionInfo", baseColl.SurfaceInfo!.FrictionInfo);
        CheckNotNull("friction.Ode", baseColl.SurfaceInfo.FrictionInfo!.Ode);
        CheckClose("friction.Ode.Mu", baseColl.SurfaceInfo.FrictionInfo.Ode!.Mu, 0.8);
        CheckClose("friction.Ode.Mu2", baseColl.SurfaceInfo.FrictionInfo.Ode.Mu2, 0.6);
        CheckNotNull("surface.ContactInfo", baseColl.SurfaceInfo.ContactInfo);
        CheckNotNull("contact.OdeParams", baseColl.SurfaceInfo.ContactInfo!.OdeParams);
        CheckClose("contact.Ode.SoftCfm", baseColl.SurfaceInfo.ContactInfo.OdeParams!.SoftCfm, 0.01);
        CheckClose("contact.Ode.Kp", baseColl.SurfaceInfo.ContactInfo.OdeParams.Kp, 1e9);

        // --- Link > Sensor ---
        Console.WriteLine("\n  --- Link > Sensor (camera) ---");
        Check("baseLink.Sensors.Count", baseLink.Sensors.Count.ToString(), "1");
        var cam = baseLink.Sensors[0];
        Check("cam.Name", cam.Name, "base_camera");
        Check("cam.Type", cam.Type.ToString(), "Camera");
        CheckClose("cam.UpdateRate", cam.UpdateRate, 30.0);
        CheckNotNull("cam.Camera", cam.Camera);
        CheckClose("cam.HFov", cam.Camera!.HorizontalFov, 1.047);
        Check("cam.ImageWidth", cam.Camera.ImageWidth.ToString(), "640");
        Check("cam.ImageHeight", cam.Camera.ImageHeight.ToString(), "480");

        // --- Arm visual (cylinder) ---
        Console.WriteLine("\n  --- Arm link visual (cylinder) ---");
        Check("armLink.Visuals.Count", armLink!.Visuals.Count.ToString(), "1");
        var armVis = armLink.Visuals[0];
        Check("armVis.Geom.Type", armVis.Geom.Type.ToString(), "Cylinder");
        CheckNotNull("armVis.CylinderShape", armVis.Geom.CylinderShape);
        CheckClose("armVis.Cyl.Radius", armVis.Geom.CylinderShape!.Radius, 0.05);
        CheckClose("armVis.Cyl.Length", armVis.Geom.CylinderShape.Length, 1.0);

        // --- Joint ---
        Console.WriteLine("\n  --- Joint (top-level model) ---");
        Check("top.JointCount", top.JointCount.ToString(), "2");  // base_to_arm + arm_to_effector
        var baseToArm = top.JointByName("base_to_arm");
        CheckNotNull("base_to_arm", baseToArm);
        Check("baseToArm.Type", baseToArm!.Type.ToString(), "Revolute");
        Check("baseToArm.Parent", baseToArm.ParentName, "base_link");
        Check("baseToArm.Child", baseToArm.ChildName, "arm_link");
        CheckNotNull("baseToArm.Axis", baseToArm.Axis);
        CheckClose("baseToArm.Axis.Xyz.Y", baseToArm.Axis!.Xyz.Y, 1.0);
        CheckClose("baseToArm.Axis.Lower", baseToArm.Axis.Lower, -1.57);
        CheckClose("baseToArm.Axis.Upper", baseToArm.Axis.Upper, 1.57);
        CheckClose("baseToArm.Axis.Effort", baseToArm.Axis.Effort, 100.0);
        CheckClose("baseToArm.Axis.MaxVel", baseToArm.Axis.MaxVelocity, 2.0);

        // --- Frame ---
        Console.WriteLine("\n  --- Frame (top-level model) ---");
        Check("top.FrameCount", top.FrameCount.ToString(), "1");
        var toolFrame = top.FrameByName("tool_frame");
        CheckNotNull("tool_frame", toolFrame);
        Check("toolFrame.AttachedTo", toolFrame!.AttachedTo, "arm_link");
        CheckClose("toolFrame.Pose.Z", toolFrame.RawPose.Position.Z, 0.5);

        // ======== NESTED MODEL (level 2) ========
        Console.WriteLine("\n  --- Nested Model Level 2 (end_effector) ---");
        Check("top.ModelCount", top.ModelCount.ToString(), "1");
        var effector = top.ModelByName("end_effector");
        CheckNotNull("end_effector", effector);
        if (effector == null) return;

        Check("effector.PoseRelTo", effector.PoseRelativeTo, "tool_frame");
        CheckClose("effector.Pose.Z", effector.RawPose.Position.Z, 0.1);

        // Links in nested model
        Check("effector.LinkCount", effector.LinkCount.ToString(), "3");
        var gripperBase = effector.LinkByName("gripper_base");
        CheckNotNull("gripper_base", gripperBase);
        var fingerLeft = effector.LinkByName("finger_left");
        CheckNotNull("finger_left", fingerLeft);
        var fingerRight = effector.LinkByName("finger_right");
        CheckNotNull("finger_right", fingerRight);

        // Visual in nested model link
        Check("gripperBase.Visuals", gripperBase!.Visuals.Count.ToString(), "1");
        Check("gripperBase.Collisions", gripperBase.Collisions.Count.ToString(), "1");
        Check("gripperBaseVis.Geom", gripperBase.Visuals[0].Geom.Type.ToString(), "Box");
        CheckClose("gripperBaseVis.BoxX", gripperBase.Visuals[0].Geom.BoxShape!.Size.X, 0.1);

        // Joints in nested model
        Check("effector.JointCount", effector.JointCount.ToString(), "2");
        var leftJoint = effector.JointByName("left_finger_joint");
        CheckNotNull("left_finger_joint", leftJoint);
        Check("leftJoint.Type", leftJoint!.Type.ToString(), "Prismatic");
        Check("leftJoint.Parent", leftJoint.ParentName, "gripper_base");
        Check("leftJoint.Child", leftJoint.ChildName, "finger_left");
        CheckClose("leftJoint.Lower", leftJoint.Axis!.Lower, -0.04);

        // Frame in nested model
        Check("effector.FrameCount", effector.FrameCount.ToString(), "1");
        var graspFrame = effector.FrameByName("grasp_frame");
        CheckNotNull("grasp_frame", graspFrame);
        Check("graspFrame.AttTo", graspFrame!.AttachedTo, "gripper_base");
        CheckClose("graspFrame.Pose.Z", graspFrame.RawPose.Position.Z, 0.08);

        // ======== NESTED MODEL (level 3) ========
        Console.WriteLine("\n  --- Nested Model Level 3 (sensor_module) ---");
        Check("effector.ModelCount", effector.ModelCount.ToString(), "1");
        var sensorMod = effector.ModelByName("sensor_module");
        CheckNotNull("sensor_module", sensorMod);
        if (sensorMod == null) return;

        CheckClose("sensorMod.Pose.Z", sensorMod.RawPose.Position.Z, -0.03);

        // Link in level-3 nested model
        Check("sensorMod.LinkCount", sensorMod.LinkCount.ToString(), "1");
        var housing = sensorMod.LinkByName("sensor_housing");
        CheckNotNull("sensor_housing", housing);

        // Visual in level-3 nested model link (sphere)
        Check("housing.Visuals", housing!.Visuals.Count.ToString(), "1");
        Check("housing.Vis.Geom", housing.Visuals[0].Geom.Type.ToString(), "Sphere");
        CheckNotNull("housing.Sphere", housing.Visuals[0].Geom.SphereShape);
        CheckClose("housing.Sphere.R", housing.Visuals[0].Geom.SphereShape!.Radius, 0.02);

        // Sensor in level-3 nested model link
        Check("housing.Sensors", housing.Sensors.Count.ToString(), "1");
        var ftSensor = housing.Sensors[0];
        Check("ftSensor.Name", ftSensor.Name, "force_torque_sensor");
        Check("ftSensor.Type", ftSensor.Type.ToString(), "ForceTorque");
        CheckClose("ftSensor.Rate", ftSensor.UpdateRate, 1000.0);
        CheckNotNull("ftSensor.ForceTorque", ftSensor.ForceTorque);
        Check("ft.Frame", ftSensor.ForceTorque!.Frame.ToString(), "Child");
        Check("ft.MeasureDir", ftSensor.ForceTorque.MeasureDirection.ToString(), "ChildToParent");

        // Frame in level-3 nested model
        Check("sensorMod.FrameCount", sensorMod.FrameCount.ToString(), "1");
        var sensorOrigin = sensorMod.FrameByName("sensor_origin");
        CheckNotNull("sensor_origin", sensorOrigin);
        Check("sensorOrigin.AttTo", sensorOrigin!.AttachedTo, "sensor_housing");

        // --- Joint referencing nested model link with :: syntax ---
        Console.WriteLine("\n  --- Joint with :: scoped child ---");
        var armToEff = top.JointByName("arm_to_effector");
        CheckNotNull("arm_to_effector", armToEff);
        Check("armToEff.Type", armToEff!.Type.ToString(), "Fixed");
        Check("armToEff.Parent", armToEff.ParentName, "arm_link");
        Check("armToEff.Child", armToEff.ChildName, "end_effector::gripper_base");

        // --- Gripper referencing nested model links ---
        Console.WriteLine("\n  --- Gripper with :: scoped links ---");
        Check("top.Grippers.Count", top.Grippers.Count.ToString(), "1");
        var gripper = top.Grippers[0];
        Check("gripper.Name", gripper.Name, "main_gripper");
        Check("gripper.MinContact", gripper.GraspCheckMinContactCount.ToString(), "2");
        Check("gripper.AttachSteps", gripper.GraspCheckAttachSteps.ToString(), "20");
        Check("gripper.Links.Count", gripper.GripperLinks.Count.ToString(), "2");
        Check("gripper.Link[0]", gripper.GripperLinks[0], "end_effector::finger_left");
        Check("gripper.Link[1]", gripper.GripperLinks[1], "end_effector::finger_right");
        Check("gripper.PalmLink", gripper.PalmLink, "end_effector::gripper_base");
    }

    static void VerifySensorTower(World world)
    {
        Console.WriteLine("\n  --- Sensor Tower (static, multi-sensor) ---");

        var tower = world.ModelByName("sensor_tower");
        CheckNotNull("sensor_tower", tower);
        if (tower == null) return;

        Check("tower.Static", tower.Static.ToString(), "True");
        CheckClose("tower.Pose.X", tower.RawPose.Position.X, 5.0);
        Check("tower.LinkCount", tower.LinkCount.ToString(), "1");

        var towerBase = tower.LinkByName("tower_base");
        CheckNotNull("tower_base", towerBase);

        // Visual and collision
        Check("towerBase.Visuals", towerBase!.Visuals.Count.ToString(), "1");
        Check("towerBase.Collisions", towerBase.Collisions.Count.ToString(), "1");
        Check("vis.Geom", towerBase.Visuals[0].Geom.Type.ToString(), "Cylinder");
        Check("coll.Geom", towerBase.Collisions[0].Geom.Type.ToString(), "Cylinder");

        // Multiple sensors in one link
        Check("towerBase.Sensors", towerBase.Sensors.Count.ToString(), "2");

        var lidar = towerBase.Sensors.Find(s => s.Name == "top_lidar");
        CheckNotNull("top_lidar", lidar);
        Check("lidar.Type", lidar!.Type.ToString(), "GpuLidar");
        CheckClose("lidar.Rate", lidar.UpdateRate, 10.0);
        CheckNotNull("lidar.Lidar", lidar.Lidar);
        Check("lidar.HSamples", lidar.Lidar!.HorizontalScanSamples.ToString(), "640");
        CheckClose("lidar.RangeMin", lidar.Lidar.RangeMin, 0.1);
        CheckClose("lidar.RangeMax", lidar.Lidar.RangeMax, 100.0);

        var imu = towerBase.Sensors.Find(s => s.Name == "imu_sensor");
        CheckNotNull("imu_sensor", imu);
        Check("imu.Type", imu!.Type.ToString(), "Imu");
        CheckClose("imu.Rate", imu.UpdateRate, 200.0);
        CheckNotNull("imu.Imu", imu.Imu);

        // Frame with pose relative_to
        Console.WriteLine("\n  --- Frame with relative_to ---");
        Check("tower.FrameCount", tower.FrameCount.ToString(), "1");
        var antenna = tower.FrameByName("antenna_mount");
        CheckNotNull("antenna_mount", antenna);
        Check("antenna.PoseRelTo", antenna!.PoseRelativeTo, "tower_base");
        CheckClose("antenna.Pose.Z", antenna.RawPose.Position.Z, 1.5);
    }

    static void VerifyNestedState(World world)
    {
        Console.WriteLine("\n  --- Nested State (3 levels) ---");

        Check("world.States.Count", world.States.Count.ToString(), "1");
        var state = world.States[0];
        Check("state.WorldName", state.WorldName, "nested_test_world");
        CheckClose("state.SimTime", state.SimTime, 25.5);
        Check("state.Iterations", state.Iterations.ToString(), "25500");

        // Model state (level 1)
        Check("state.ModelStates", state.ModelStates.Count.ToString(), "1");
        var topState = state.ModelStates[0];
        Check("topState.Name", topState.Name, "top_robot");
        CheckClose("topState.Pose.X", topState.RawPose.Position.X, 1.1);
        CheckClose("topState.Pose.Y", topState.RawPose.Position.Y, 2.2);
        CheckClose("topState.Pose.Z", topState.RawPose.Position.Z, 3.3);

        // Link states
        Check("topState.LinkStates", topState.LinkStates.Count.ToString(), "2");
        var baseLinkState = topState.LinkStates.Find(l => l.Name == "base_link");
        CheckNotNull("baseLinkState", baseLinkState);
        CheckClose("baseLinkState.Vel.X", baseLinkState!.Velocity.Position.X, 0.05);

        var armLinkState = topState.LinkStates.Find(l => l.Name == "arm_link");
        CheckNotNull("armLinkState", armLinkState);

        // Joint state
        Check("topState.JointStates", topState.JointStates.Count.ToString(), "1");
        var jointState = topState.JointStates[0];
        Check("jointState.Name", jointState.Name, "base_to_arm");
        Check("jointState.Angles", jointState.Angles.Count.ToString(), "1");
        CheckClose("jointState.Angle[0]", jointState.Angles[0], 0.3);

        // Nested model state (level 2)
        Check("topState.ModelStates", topState.ModelStates.Count.ToString(), "1");
        var effState = topState.ModelStates[0];
        Check("effState.Name", effState.Name, "end_effector");
        CheckClose("effState.Pose.Z", effState.RawPose.Position.Z, 0.1);

        // Link states in nested model state
        Check("effState.LinkStates", effState.LinkStates.Count.ToString(), "1");
        Check("effState.Link.Name", effState.LinkStates[0].Name, "gripper_base");

        // Joint states in nested model state
        Check("effState.JointStates", effState.JointStates.Count.ToString(), "2");
        var leftFingerState = effState.JointStates.Find(j => j.Name == "left_finger_joint");
        CheckNotNull("leftFingerJState", leftFingerState);
        CheckClose("leftFinger.Angle", leftFingerState!.Angles[0], -0.02);
        var rightFingerState = effState.JointStates.Find(j => j.Name == "right_finger_joint");
        CheckNotNull("rightFingerJState", rightFingerState);
        CheckClose("rightFinger.Angle", rightFingerState!.Angles[0], 0.02);

        // Nested model state (level 3)
        Check("effState.ModelStates", effState.ModelStates.Count.ToString(), "1");
        var sensorState = effState.ModelStates[0];
        Check("sensorState.Name", sensorState.Name, "sensor_module");
        CheckClose("sensorState.Pose.Z", sensorState.RawPose.Position.Z, -0.03);

        // Link state in level 3
        Check("sensorState.Links", sensorState.LinkStates.Count.ToString(), "1");
        Check("sensorState.Link", sensorState.LinkStates[0].Name, "sensor_housing");
    }

    static void VerifyWorldLightsAndFrames(World world)
    {
        Console.WriteLine("\n  --- World Lights & Frames ---");

        Check("world.LightCount", world.LightCount.ToString(), "1");
        var sun = world.Lights.Find(l => l.Name == "sun");
        CheckNotNull("sun", sun);
        Check("sun.Type", sun!.Type.ToString(), "Directional");
        CheckClose("sun.Pose.Z", sun.RawPose.Position.Z, 20.0);
        CheckClose("sun.Diffuse.R", sun.Diffuse.R, 1.0);
        CheckClose("sun.Specular.R", sun.Specular.R, 0.5);
        CheckClose("sun.Direction.X", sun.Direction.X, -0.5);

        Check("world.FrameCount", world.FrameCount.ToString(), "2");
        var worldOrigin = world.Frames.Find(f => f.Name == "world_origin");
        CheckNotNull("world_origin", worldOrigin);

        var spawnPoint = world.Frames.Find(f => f.Name == "spawn_point");
        CheckNotNull("spawn_point", spawnPoint);
        Check("spawn.AttachedTo", spawnPoint!.AttachedTo, "world_origin");
        CheckClose("spawn.Pose.X", spawnPoint.RawPose.Position.X, 10.0);
        CheckClose("spawn.Pose.Y", spawnPoint.RawPose.Position.Y, 10.0);
    }

    // ---------- helpers ----------

    static void Check(string label, string actual, string expected)
    {
        if (actual == expected)
        {
            _pass++;
            Console.WriteLine($"    [PASS] {label}: {actual}");
        }
        else
        {
            _fail++;
            Console.WriteLine($"    [FAIL] {label}: expected={expected}, actual={actual}");
        }
    }

    static void CheckNotNull(string label, object? obj)
    {
        if (obj != null)
        {
            _pass++;
            Console.WriteLine($"    [PASS] {label}: present");
        }
        else
        {
            _fail++;
            Console.WriteLine($"    [FAIL] {label}: NULL");
        }
    }

    static void CheckClose(string label, double actual, double expected, double tol = 1e-6)
    {
        if (Math.Abs(actual - expected) < tol)
        {
            _pass++;
            Console.WriteLine($"    [PASS] {label}: {actual}");
        }
        else
        {
            _fail++;
            Console.WriteLine($"    [FAIL] {label}: expected={expected}, actual={actual}");
        }
    }
}

}
