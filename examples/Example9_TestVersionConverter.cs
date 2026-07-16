// Test: Verify the SDF version converter (1.4 -> 1.12) on real sample files
//
// Covers, per sample file loaded from disk via Root.Load():
//   - test_converter_v1_4.sdf: <add> (use_parent_model_frame), <rename>
//     (actor static attribute -> element)
//   - test_converter_v1_5.sdf: <move> (physics/gravity, physics/magnetic_field),
//     <copy>/<move> chain (IMU noise restructuring)
//   - test_converter_v1_6.sdf: descendant_name="pose" <move> (frame -> relative_to),
//     <map> (use_parent_model_frame -> axis/xyz/@expressed_in)
//   - test_converter_v1_7.sdf: <unflatten/> (submodel reconstruction,
//     canonical_link/pose promotion, joint/sensor/camera/gripper handling),
//     <remove_empty attribute="relative_to"/>
//
// Every sample is loaded through the normal file-loading path
// (Root.Load -> SdfParser.Parse -> Converter.ConvertToLatest), so this
// exercises the same code path a real user hits, not just the converter
// in isolation.

using System;
using System.IO;
using SDFormat;

namespace Examples
{

public static class Example9_TestVersionConverter
{
    private static int _pass;
    private static int _fail;

    public static void Run()
    {
        Console.WriteLine("=== Test: SDF Version Converter (1.4 -> 1.12) ===\n");
        _pass = 0;
        _fail = 0;

        var dataDir = Path.Combine(AppContext.BaseDirectory, "data");

        VerifyV14(Path.Combine(dataDir, "test_converter_v1_4.sdf"));
        VerifyV15(Path.Combine(dataDir, "test_converter_v1_5.sdf"));
        VerifyV16(Path.Combine(dataDir, "test_converter_v1_6.sdf"));
        VerifyV17(Path.Combine(dataDir, "test_converter_v1_7.sdf"));

        Console.WriteLine();
        Console.WriteLine("  ========================================");
        Console.WriteLine($"  Results: {_pass} passed, {_fail} failed, {_pass + _fail} total");
        Console.WriteLine("  ========================================");

        if (_fail > 0) Environment.ExitCode = 1;
    }

    private static void VerifyV14(string sdfPath)
    {
        Console.WriteLine("  --- test_converter_v1_4.sdf (add, rename) ---");
        var root = new Root();
        var errors = root.Load(sdfPath);
        ReportErrors(errors);

        Check("SDF version", root.Version, "1.12");
        var world = root.WorldByIndex(0);
        var model = world?.ModelByIndex(0);
        var joint = model?.JointByIndex(0);

        // Fully converged to latest: the use_parent_model_frame flag added at
        // 1.5 is itself consumed by the 1.6->1.7 step into axis/xyz/@expressed_in.
        var axis = joint?.GetAxis(0);
        var axis2 = joint?.GetAxis(1);
        Check("joint.axis.XyzExpressedIn", axis?.XyzExpressedIn, "__model__");
        Check("joint.axis2.XyzExpressedIn", axis2?.XyzExpressedIn, "__model__");

        var actorElem = world?.Element?.FindElement("actor");
        Check("actor exists", (actorElem != null).ToString(), "True");
        Check("actor.static attribute removed", (actorElem?.GetAttribute("static") == null).ToString(), "True");
        Check("actor.static element", actorElem?.FindElement("static")?.Value?.GetAsString(), "true");
    }

    private static void VerifyV15(string sdfPath)
    {
        Console.WriteLine("\n  --- test_converter_v1_5.sdf (move, copy) ---");
        var root = new Root();
        var errors = root.Load(sdfPath);
        ReportErrors(errors);

        Check("SDF version", root.Version, "1.12");
        var world = root.WorldByIndex(0);
        Check("world.Gravity.Z", world?.Gravity.Z.ToString("0.0"), "-9.8");

        var imu = world?.ModelByIndex(0)?.LinkByIndex(0)?.SensorByIndex(0)?.Imu;
        Check("imu exists", (imu != null).ToString(), "True");
        Check("imu.AngularVelocityXNoise.Type", imu?.AngularVelocityXNoise.Type.ToString(), "Gaussian");
        Check("imu.AngularVelocityXNoise.Mean", imu?.AngularVelocityXNoise.Mean.ToString(), "0");
        Check("imu.LinearAccelerationZNoise.StdDev", imu?.LinearAccelerationZNoise.StdDev.ToString(), "0.017");
    }

    private static void VerifyV16(string sdfPath)
    {
        Console.WriteLine("\n  --- test_converter_v1_6.sdf (descendant_name move, map) ---");
        var root = new Root();
        var errors = root.Load(sdfPath);
        ReportErrors(errors);

        Check("SDF version", root.Version, "1.12");
        var model = root.WorldByIndex(0)?.ModelByIndex(0);

        Check("model.PoseRelativeTo", model?.PoseRelativeTo, "world");

        var childLink = model?.LinkByIndex(1);
        Check("child link name", childLink?.Name, "child");
        Check("child link.PoseRelativeTo", childLink?.PoseRelativeTo, "joint");

        var fixedJoint = model?.JointByIndex(0);
        Check("fixed joint.PoseRelativeTo", fixedJoint?.PoseRelativeTo, "parent");

        var revoluteJoint = model?.JointByIndex(1);
        var axis = revoluteJoint?.GetAxis(0);
        Check("revolute joint.axis.XyzExpressedIn", axis?.XyzExpressedIn, "__model__");
    }

    private static void VerifyV17(string sdfPath)
    {
        Console.WriteLine("\n  --- test_converter_v1_7.sdf (unflatten, remove_empty) ---");
        var root = new Root();
        var errors = root.Load(sdfPath);
        ReportErrors(errors);

        Check("SDF version", root.Version, "1.12");

        var parentModel = root.WorldByIndex(0)?.ModelByIndex(0);
        Check("parentModel.Name", parentModel?.Name, "ParentModel");

        var childModelElem = parentModel?.Element?.FindElement("model");
        Check("childModel exists", (childModelElem != null).ToString(), "True");
        Check("childModel.name", childModelElem?.GetAttribute("name")?.GetAsString(), "ChildModel");
        Check("childModel.canonical_link", childModelElem?.GetAttribute("canonical_link")?.GetAsString(), "L1");
        Check("childModel.pose.relative_to", childModelElem?.FindElement("pose")?.GetAttribute("relative_to")?.GetAsString(), "__model__");

        var newFrameElem = childModelElem?.FindElement("frame");
        Check("NewFrame.name", newFrameElem?.GetAttribute("name")?.GetAsString(), "NewFrame");
        Check("NewFrame.attached_to", newFrameElem?.GetAttribute("attached_to")?.GetAsString(), "L1");

        var l1Elem = FindChildByName(childModelElem, "link", "L1");
        var unflattenedLinkElem = FindChildByName(childModelElem?.Parent, "link", "unflattened_link");
        Check("unflattened_link.inertial.pose.relative_to removed",
            (unflattenedLinkElem?.FindElement("inertial")?.FindElement("pose")?.GetAttribute("relative_to") == null).ToString(), "True");
        var cameraPose = l1Elem?.FindElement("sensor")?.FindElement("camera")?.FindElement("pose");
        Check("L1.sensor.camera.pose.relative_to", cameraPose?.GetAttribute("relative_to")?.GetAsString(), "__model__");

        var j1Elem = FindChildByName(childModelElem, "joint", "J1");
        Check("J1.parent", j1Elem?.FindElement("parent")?.Value?.GetAsString(), "L1");
        Check("J1.child", j1Elem?.FindElement("child")?.Value?.GetAsString(), "L2");
        Check("J1.axis.xyz.expressed_in", j1Elem?.FindElement("axis")?.FindElement("xyz")?.GetAttribute("expressed_in")?.GetAsString(), "NewFrame");

        var gripper2Elem = FindChildByName(childModelElem, "gripper", "gripper2");
        Check("gripper2.gripper_link", gripper2Elem?.FindElement("gripper_link")?.Value?.GetAsString(), "L1");
        Check("gripper2.palm_link", gripper2Elem?.FindElement("palm_link")?.Value?.GetAsString(), "L2");
    }

    private static Element? FindChildByName(Element? parent, string tag, string name)
    {
        if (parent == null) return null;
        foreach (var child in parent.Children)
        {
            if (child.Name == tag && child.GetAttribute("name")?.GetAsString() == name)
                return child;
        }
        return null;
    }

    private static void ReportErrors(System.Collections.Generic.List<SdfError> errors)
    {
        if (errors.Count > 0)
        {
            Console.WriteLine($"    Parse errors ({errors.Count}):");
            foreach (var e in errors)
                Console.WriteLine($"      {e}");
        }
    }

    private static void Check(string label, string? actual, string expected)
    {
        bool ok = actual == expected;
        if (ok) _pass++; else _fail++;
        Console.WriteLine($"    [{(ok ? "PASS" : "FAIL")}] {label}: {actual}" + (ok ? "" : $" (expected {expected})"));
    }
}
}
