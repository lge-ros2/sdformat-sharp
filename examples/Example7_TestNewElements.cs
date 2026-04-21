// Test: Verify all newly implemented SDF 1.12 elements
//
// Covers:
//   - State (sim_time, wall_time, real_time, iterations, insertions, deletions, model/link/joint/light states)
//   - Road (name, width, points, material)
//   - Population (name, model_count, distribution, pose, box region)
//   - Gripper (grasp_check, gripper_links, palm_link)
//   - Physics engine-specific params (ODE, Bullet, Simbody, DART)
//   - Joint (gearbox_ratio, gearbox_reference_body, JointPhysics with ODE/Simbody)
//   - Link (audio_sink, audio_source, fluid_added_mass)
//   - Surface (OdeContact, BulletContact, DartSoftContact)
//   - Geometry (empty, image, mesh convex_decomposition)
//   - Sensor (contact, wireless_transmitter/transceiver)

using System;
using System.IO;
using SDFormat;
using SDFormat.Math;

namespace Examples
{

public static class Example7_TestNewElements
{
    private static int _pass;
    private static int _fail;

    public static void Run()
    {
        Console.WriteLine("=== Test: Verify All New SDF 1.12 Elements ===\n");
        _pass = 0;
        _fail = 0;

        var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
        var sdfPath = Path.Combine(dataDir, "test_new_elements.sdf");

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
        Check("WorldCount", root.WorldCount, 1);

        var world = root.WorldByIndex(0)!;
        Check("World.Name", world.Name, "test_new_elements");

        VerifyPhysicsEngineParams(world);
        VerifyRoad(world);
        VerifyPopulation(world);
        VerifyState(world);
        VerifyModel(world);

        Console.WriteLine($"\n  ========================================");
        Console.WriteLine($"  Results: {_pass} passed, {_fail} failed, {_pass + _fail} total");
        Console.WriteLine($"  ========================================\n");

        if (_fail > 0)
            Environment.ExitCode = 1;
    }

    // ----------------------------------------------------------------
    // Physics with engine-specific params
    // ----------------------------------------------------------------

    private static void VerifyPhysicsEngineParams(World world)
    {
        Console.WriteLine("\n  --- Physics Engine Params ---");
        Check("PhysicsCount", world.PhysicsCount, 4);

        // ODE physics
        var ode = world.PhysicsByIndex(0)!;
        Check("ODE.Name", ode.Name, "ode_physics");
        Check("ODE.EngineType", ode.EngineType, "ode");
        Check("ODE.IsDefault", ode.IsDefault, true);
        CheckClose("ODE.MaxStepSize", ode.MaxStepSize, 0.002);
        CheckClose("ODE.RealTimeFactor", ode.RealTimeFactor, 1.5);
        CheckClose("ODE.RealTimeUpdateRate", ode.RealTimeUpdateRate, 500);
        Check("ODE.MaxContacts", ode.MaxContacts, 30);
        CheckNotNull("ODE.Ode", ode.Ode);
        if (ode.Ode != null)
        {
            Check("ODE.Solver.Type", ode.Ode.SolverType, "quick");
            Check("ODE.Solver.Iters", ode.Ode.SolverIters, 100);
            CheckClose("ODE.Solver.Sor", ode.Ode.SolverSor, 1.4);
            CheckClose("ODE.Constraints.Cfm", ode.Ode.ConstraintsCfm, 0.001);
            CheckClose("ODE.Constraints.Erp", ode.Ode.ConstraintsErp, 0.3);
            CheckClose("ODE.Constraints.MaxCorrVel", ode.Ode.ConstraintsContactMaxCorrectingVel, 50);
            CheckClose("ODE.Constraints.SurfLayer", ode.Ode.ConstraintsContactSurfaceLayer, 0.002);
        }

        // Bullet physics
        var bullet = world.PhysicsByIndex(1)!;
        Check("Bullet.Name", bullet.Name, "bullet_physics");
        Check("Bullet.IsDefault", bullet.IsDefault, false);
        CheckNotNull("Bullet.BulletEngine", bullet.BulletEngine);
        if (bullet.BulletEngine != null)
        {
            Check("Bullet.Solver.Type", bullet.BulletEngine.SolverType, "sequential_impulse");
            Check("Bullet.Solver.Iters", bullet.BulletEngine.SolverIters, 75);
            CheckClose("Bullet.Solver.Sor", bullet.BulletEngine.SolverSor, 1.2);
            CheckClose("Bullet.Solver.MinStep", bullet.BulletEngine.SolverMinStepSize, 0.0002);
            CheckClose("Bullet.Constraints.Cfm", bullet.BulletEngine.ConstraintsCfm, 0.002);
            CheckClose("Bullet.Constraints.Erp", bullet.BulletEngine.ConstraintsErp, 0.25);
            CheckClose("Bullet.Constraints.SurfLayer", bullet.BulletEngine.ConstraintsContactSurfaceLayer, 0.003);
            Check("Bullet.Constraints.SplitImpulse", bullet.BulletEngine.ConstraintsSplitImpulse, true);
            CheckClose("Bullet.Constraints.SplitThresh", bullet.BulletEngine.ConstraintsSplitImpulsePenetrationThreshold, -0.02);
        }

        // Simbody physics
        var simbody = world.PhysicsByIndex(2)!;
        Check("Simbody.Name", simbody.Name, "simbody_physics");
        CheckNotNull("Simbody.SimbodyEngine", simbody.SimbodyEngine);
        if (simbody.SimbodyEngine != null)
        {
            CheckClose("Simbody.Accuracy", simbody.SimbodyEngine.Accuracy, 0.002);
            CheckClose("Simbody.MaxTransient", simbody.SimbodyEngine.MaxTransientVelocity, 0.05);
            CheckClose("Simbody.Contact.Stiffness", simbody.SimbodyEngine.ContactStiffness, 2e8);
            CheckClose("Simbody.Contact.Dissipation", simbody.SimbodyEngine.ContactDissipation, 200);
            CheckClose("Simbody.Contact.PlasticCoef", simbody.SimbodyEngine.ContactPlasticCoefRestitution, 0.6);
            CheckClose("Simbody.Contact.PlasticVel", simbody.SimbodyEngine.ContactPlasticImpactVelocity, 0.4);
            CheckClose("Simbody.Contact.StaticFriction", simbody.SimbodyEngine.ContactStaticFriction, 0.8);
            CheckClose("Simbody.Contact.DynamicFriction", simbody.SimbodyEngine.ContactDynamicFriction, 0.7);
            CheckClose("Simbody.Contact.ViscousFriction", simbody.SimbodyEngine.ContactViscousFriction, 0.1);
            CheckClose("Simbody.Contact.OverrideCapture", simbody.SimbodyEngine.ContactOverrideImpactCaptureVelocity, 0.002);
            CheckClose("Simbody.Contact.OverrideStiction", simbody.SimbodyEngine.ContactOverrideStictionTransitionVelocity, 0.003);
        }

        // DART physics
        var dart = world.PhysicsByIndex(3)!;
        Check("DART.Name", dart.Name, "dart_physics");
        CheckNotNull("DART.DartEngine", dart.DartEngine);
        if (dart.DartEngine != null)
        {
            Check("DART.Solver", dart.DartEngine.Solver, "dantzig");
            Check("DART.CollisionDetector", dart.DartEngine.CollisionDetector, "bullet");
        }
    }

    // ----------------------------------------------------------------
    // Road
    // ----------------------------------------------------------------

    private static void VerifyRoad(World world)
    {
        Console.WriteLine("\n  --- Road ---");
        Check("Roads.Count", world.Roads.Count, 1);
        var road = world.Roads[0];
        Check("Road.Name", road.Name, "main_road");
        CheckClose("Road.Width", road.Width, 7.5);
        Check("Road.Points.Count", road.Points.Count, 3);
        CheckClose("Road.Point[0].X", road.Points[0].X, 0);
        CheckClose("Road.Point[1].X", road.Points[1].X, 100);
        CheckClose("Road.Point[2].Y", road.Points[2].Y, 50);
        CheckNotNull("Road.Material", road.RoadMaterial);
        if (road.RoadMaterial != null)
        {
            CheckClose("Road.Material.Ambient.R", road.RoadMaterial.Ambient.R, 0.3f, 0.01f);
            CheckClose("Road.Material.Diffuse.G", road.RoadMaterial.Diffuse.G, 0.5f, 0.01f);
        }
    }

    // ----------------------------------------------------------------
    // Population
    // ----------------------------------------------------------------

    private static void VerifyPopulation(World world)
    {
        Console.WriteLine("\n  --- Population ---");
        Check("Populations.Count", world.Populations.Count, 1);
        var pop = world.Populations[0];
        Check("Pop.Name", pop.Name, "tree_population");
        Check("Pop.ModelCount", pop.ModelCount, 25);
        Check("Pop.Distribution", pop.Distribution, "random");
        CheckClose("Pop.Pose.X", pop.RawPose.Position.X, 50);
        CheckClose("Pop.Pose.Y", pop.RawPose.Position.Y, 50);
        CheckClose("Pop.BoxSize.X", pop.BoxSize.X, 100);
        CheckClose("Pop.BoxSize.Y", pop.BoxSize.Y, 100);
    }

    // ----------------------------------------------------------------
    // State
    // ----------------------------------------------------------------

    private static void VerifyState(World world)
    {
        Console.WriteLine("\n  --- State ---");
        Check("States.Count", world.States.Count, 1);
        var state = world.States[0];
        Check("State.WorldName", state.WorldName, "test_new_elements");
        CheckClose("State.SimTime", state.SimTime, 10.5);
        CheckClose("State.WallTime", state.WallTime, 12.3);
        CheckClose("State.RealTime", state.RealTime, 11.1);
        Check("State.Iterations", state.Iterations, (ulong)10500);

        // Insertions
        Check("State.Insertions.Count", state.Insertions.Count, 1);

        // Deletions
        Check("State.Deletions.Count", state.Deletions.Count, 2);
        Check("State.Deletions[0]", state.Deletions[0], "deleted_sphere");
        Check("State.Deletions[1]", state.Deletions[1], "deleted_cylinder");

        // Model state
        Check("State.ModelStates.Count", state.ModelStates.Count, 1);
        var ms = state.ModelStates[0];
        Check("ModelState.Name", ms.Name, "robot_state");
        CheckClose("ModelState.Pose.X", ms.RawPose.Position.X, 1);
        CheckClose("ModelState.Pose.Y", ms.RawPose.Position.Y, 2);
        CheckClose("ModelState.Scale.X", ms.Scale.X, 1.5);

        // Joint state
        Check("ModelState.JointStates.Count", ms.JointStates.Count, 1);
        var js = ms.JointStates[0];
        Check("JointState.Name", js.Name, "joint1");
        Check("JointState.Angles.Count", js.Angles.Count, 2);
        CheckClose("JointState.Angles[0]", js.Angles[0], 1.57);
        CheckClose("JointState.Angles[1]", js.Angles[1], 0.5);

        // Link state
        Check("ModelState.LinkStates.Count", ms.LinkStates.Count, 1);
        var ls = ms.LinkStates[0];
        Check("LinkState.Name", ls.Name, "base_link");
        CheckClose("LinkState.Pose.Z", ls.RawPose.Position.Z, 0.5);
        CheckClose("LinkState.Velocity.X", ls.Velocity.Position.X, 0.1);
        CheckClose("LinkState.Acceleration.Z", ls.Acceleration.Position.Z, -9.81);
        CheckClose("LinkState.Wrench.Z", ls.Wrench.Position.Z, -98.1);

        // Light state
        Check("State.LightStates.Count", state.LightStates.Count, 1);
        var lightState = state.LightStates[0];
        Check("LightState.Name", lightState.Name, "sun_state");
        CheckClose("LightState.Pose.Z", lightState.RawPose.Position.Z, 10);
    }

    // ----------------------------------------------------------------
    // Model: Gripper, Link audio/fluid, Joint gearbox/physics, Geometry, Sensors
    // ----------------------------------------------------------------

    private static void VerifyModel(World world)
    {
        var model = world.ModelByName("test_robot")!;
        CheckNotNull("Model test_robot", model);
        if (model == null) return;

        VerifyGripper(model);
        VerifyLinkAudio(model);
        VerifyFluidAddedMass(model);
        VerifySurfaceContacts(model);
        VerifyEmptyGeometry(model);
        VerifyContactSensor(model);
        VerifyTransceiverSensor(model);
        VerifyImageGeometry(model);
        VerifyMeshConvexDecomp(model);
        VerifyGearboxJoint(model);
    }

    private static void VerifyGripper(Model model)
    {
        Console.WriteLine("\n  --- Gripper ---");
        Check("Grippers.Count", model.Grippers.Count, 1);
        var gripper = model.Grippers[0];
        Check("Gripper.Name", gripper.Name, "left_gripper");
        Check("Gripper.MinContactCount", gripper.GraspCheckMinContactCount, 3);
        Check("Gripper.AttachSteps", gripper.GraspCheckAttachSteps, 30);
        Check("Gripper.Links.Count", gripper.GripperLinks.Count, 2);
        Check("Gripper.Links[0]", gripper.GripperLinks[0], "finger_left");
        Check("Gripper.Links[1]", gripper.GripperLinks[1], "finger_right");
        Check("Gripper.PalmLink", gripper.PalmLink, "palm");
    }

    private static void VerifyLinkAudio(Model model)
    {
        Console.WriteLine("\n  --- Link Audio ---");
        var link = model.LinkByName("base_link")!;
        Check("Link.HasAudioSink", link.HasAudioSink, true);
        Check("Link.AudioSources.Count", link.AudioSources.Count, 1);

        var src = link.AudioSources[0];
        Check("AudioSource.Uri", src.Uri, "file://sounds/motor.wav");
        CheckClose("AudioSource.Pitch", src.Pitch, 1.2);
        CheckClose("AudioSource.Gain", src.Gain, 0.8);
        Check("AudioSource.Loop", src.Loop, true);
        CheckClose("AudioSource.InnerAngle", src.InnerAngle, 1.57);
        CheckClose("AudioSource.OuterAngle", src.OuterAngle, 3.14);
        CheckClose("AudioSource.Falloff", src.Falloff, 1.5);
        Check("AudioSource.ContactSurfaces.Count", src.ContactSurfaces.Count, 2);
        Check("AudioSource.ContactSurfaces[0]", src.ContactSurfaces[0], "base_collision");
        Check("AudioSource.ContactSurfaces[1]", src.ContactSurfaces[1], "arm_collision");
    }

    private static void VerifyFluidAddedMass(Model model)
    {
        Console.WriteLine("\n  --- Fluid Added Mass ---");
        var link = model.LinkByName("base_link")!;
        Check("Link.AutoInertia", link.AutoInertia, true);
        CheckNotNull("Link.FluidAddedMass", link.FluidAddedMass);
        if (link.FluidAddedMass != null)
        {
            Check("FluidAddedMass.Length", link.FluidAddedMass.Length, 21);
            CheckClose("FluidAddedMass[xx]", link.FluidAddedMass[0], 10.0);
            CheckClose("FluidAddedMass[xy]", link.FluidAddedMass[1], 0.1);
            CheckClose("FluidAddedMass[yy]", link.FluidAddedMass[6], 11.0);
            CheckClose("FluidAddedMass[zz]", link.FluidAddedMass[11], 12.0);
            CheckClose("FluidAddedMass[pp]", link.FluidAddedMass[15], 13.0);
            CheckClose("FluidAddedMass[qq]", link.FluidAddedMass[18], 14.0);
            CheckClose("FluidAddedMass[rr]", link.FluidAddedMass[20], 15.0);
        }
    }

    private static void VerifySurfaceContacts(Model model)
    {
        Console.WriteLine("\n  --- Surface Contact Params ---");
        var link = model.LinkByName("base_link")!;
        var col = link.CollisionByName("base_collision")!;
        var surface = col.SurfaceInfo!;
        var contact = surface.ContactInfo!;

        // ODE contact
        CheckNotNull("OdeContact", contact.OdeParams);
        if (contact.OdeParams != null)
        {
            CheckClose("OdeContact.SoftCfm", contact.OdeParams.SoftCfm, 0.01);
            CheckClose("OdeContact.SoftErp", contact.OdeParams.SoftErp, 0.3);
            CheckClose("OdeContact.Kp", contact.OdeParams.Kp, 1e10);
            CheckClose("OdeContact.Kd", contact.OdeParams.Kd, 2.0);
            CheckClose("OdeContact.MaxVel", contact.OdeParams.MaxVel, 0.1);
            CheckClose("OdeContact.MinDepth", contact.OdeParams.MinDepth, 0.002);
        }

        // Bullet contact
        CheckNotNull("BulletContact", contact.BulletParams);
        if (contact.BulletParams != null)
        {
            CheckClose("BulletContact.SoftCfm", contact.BulletParams.SoftCfm, 0.02);
            CheckClose("BulletContact.SoftErp", contact.BulletParams.SoftErp, 0.25);
            CheckClose("BulletContact.Kp", contact.BulletParams.Kp, 1e11);
            CheckClose("BulletContact.Kd", contact.BulletParams.Kd, 3.0);
            Check("BulletContact.SplitImpulse", contact.BulletParams.SplitImpulse, false);
            CheckClose("BulletContact.SplitThresh", contact.BulletParams.SplitImpulsePenetrationThreshold, -0.05);
        }

        // DART soft contact
        CheckNotNull("SoftContact", surface.SoftContact);
        if (surface.SoftContact != null)
        {
            CheckClose("DART.BoneAttach", surface.SoftContact.BoneAttachment, 50);
            CheckClose("DART.Stiffness", surface.SoftContact.Stiffness, 200);
            CheckClose("DART.Damping", surface.SoftContact.Damping, 20);
            CheckClose("DART.FleshMass", surface.SoftContact.FleshMassFraction, 0.1);
        }
    }

    private static void VerifyEmptyGeometry(Model model)
    {
        Console.WriteLine("\n  --- Empty Geometry ---");
        var link = model.LinkByName("base_link")!;
        var vis = link.VisualByName("empty_visual")!;
        Check("EmptyGeom.Type", vis.Geom.Type, GeometryType.Empty);
    }

    private static void VerifyContactSensor(Model model)
    {
        Console.WriteLine("\n  --- Contact Sensor ---");
        var link = model.LinkByName("base_link")!;
        var sensor = link.SensorByName("contact_sensor")!;
        Check("Sensor.Type", sensor.Type, SensorType.Contact);
        CheckNotNull("ContactSensorData", sensor.ContactSensorData);
        if (sensor.ContactSensorData != null)
        {
            Check("Contact.Collision", sensor.ContactSensorData.CollisionName, "base_collision");
            Check("Contact.Topic", sensor.ContactSensorData.Topic, "/contact_data");
        }
    }

    private static void VerifyTransceiverSensor(Model model)
    {
        Console.WriteLine("\n  --- Transceiver Sensor ---");
        var link = model.LinkByName("base_link")!;
        var sensor = link.SensorByName("wifi_tx")!;
        Check("Sensor.Type", sensor.Type, SensorType.WirelessTransmitter);
        CheckNotNull("Transceiver", sensor.Transceiver);
        if (sensor.Transceiver != null)
        {
            Check("Transceiver.Essid", sensor.Transceiver.Essid, "my_network");
            CheckClose("Transceiver.Frequency", sensor.Transceiver.Frequency, 2412);
            CheckClose("Transceiver.Gain", sensor.Transceiver.Gain, 3.0);
            CheckClose("Transceiver.Power", sensor.Transceiver.Power, 15.5);
            CheckClose("Transceiver.Sensitivity", sensor.Transceiver.Sensitivity, -85);
        }
    }

    private static void VerifyImageGeometry(Model model)
    {
        Console.WriteLine("\n  --- Image Geometry ---");
        var link = model.LinkByName("arm_link")!;
        var vis = link.VisualByName("image_visual")!;
        Check("ImageGeom.Type", vis.Geom.Type, GeometryType.Image);
        CheckNotNull("ImageShape", vis.Geom.ImageShapeData);
        if (vis.Geom.ImageShapeData != null)
        {
            Check("Image.Uri", vis.Geom.ImageShapeData.Uri, "file://images/maze.png");
            CheckClose("Image.Scale", vis.Geom.ImageShapeData.Scale, 0.5);
            Check("Image.Threshold", vis.Geom.ImageShapeData.Threshold, 128);
            CheckClose("Image.Height", vis.Geom.ImageShapeData.Height, 2.0);
            Check("Image.Granularity", vis.Geom.ImageShapeData.Granularity, 2);
        }
    }

    private static void VerifyMeshConvexDecomp(Model model)
    {
        Console.WriteLine("\n  --- Mesh Convex Decomposition ---");
        var link = model.LinkByName("arm_link")!;
        var vis = link.VisualByName("mesh_visual")!;
        Check("MeshGeom.Type", vis.Geom.Type, GeometryType.Mesh);
        var mesh = vis.Geom.MeshShape!;
        Check("Mesh.Optimization", mesh.OptimizationStr, "convex_decomposition");
        Check("Mesh.Uri", mesh.Uri, "model://robot/mesh.dae");
        CheckNotNull("Mesh.ConvexDecomp", mesh.ConvexDecomp);
        if (mesh.ConvexDecomp != null)
        {
            Check("ConvexDecomp.MaxHulls", mesh.ConvexDecomp.MaxConvexHulls, (uint)32);
            Check("ConvexDecomp.VoxelRes", mesh.ConvexDecomp.VoxelResolution, (uint)400000);
        }
    }

    private static void VerifyGearboxJoint(Model model)
    {
        Console.WriteLine("\n  --- Gearbox Joint + Physics ---");
        var joint = model.JointByName("gearbox_joint")!;
        Check("Joint.Name", joint.Name, "gearbox_joint");
        Check("Joint.Type", joint.Type, JointType.Gearbox);
        Check("Joint.Parent", joint.ParentName, "base_link");
        Check("Joint.Child", joint.ChildName, "arm_link");
        CheckClose("Joint.GearboxRatio", joint.GearboxRatio, 2.5);
        Check("Joint.GearboxRefBody", joint.GearboxReferenceBody, "world");

        // Joint physics
        CheckNotNull("Joint.Physics", joint.PhysicsSettings);
        if (joint.PhysicsSettings != null)
        {
            var jp = joint.PhysicsSettings;
            Check("JointPhysics.ProvideFeedback", jp.ProvideFeedback, true);

            // ODE
            Check("JointPhysics.OdeCfmDamping", jp.OdeCfmDamping, true);
            Check("JointPhysics.OdeImplicitSpring", jp.OdeImplicitSpringDamper, true);
            CheckClose("JointPhysics.OdeFudgeFactor", jp.OdeFudgeFactor, 0.5);
            CheckClose("JointPhysics.OdeCfm", jp.OdeCfm, 0.01);
            CheckClose("JointPhysics.OdeErp", jp.OdeErp, 0.3);
            CheckClose("JointPhysics.OdeBounce", jp.OdeBounce, 0.1);
            CheckClose("JointPhysics.OdeMaxForce", jp.OdeMaxForce, 100);
            CheckClose("JointPhysics.OdeVelocity", jp.OdeVelocity, 5.0);
            CheckClose("JointPhysics.OdeLimitCfm", jp.OdeLimitCfm, 0.02);
            CheckClose("JointPhysics.OdeLimitErp", jp.OdeLimitErp, 0.25);
            CheckClose("JointPhysics.OdeSusCfm", jp.OdeSuspensionCfm, 0.03);
            CheckClose("JointPhysics.OdeSusErp", jp.OdeSuspensionErp, 0.35);

            // Simbody
            Check("JointPhysics.SimbodyLoop", jp.SimbodyMustBeLoopJoint, true);
        }
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
