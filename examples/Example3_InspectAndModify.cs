// Example 3: Load an SDF model, inspect it, modify properties, and re-serialize
//
// Demonstrates:
//   - Loading a model from file
//   - Walking the DOM tree to read properties
//   - Modifying joint limits, link masses, adding frames
//   - Serializing the modified model back to XML

using SDFormat;
using SDFormat.Math;

namespace Examples;

public static class Example3_InspectAndModify
{
    public static void Run()
    {
        Console.WriteLine("=== Example 3: Inspect & Modify DOM ===\n");

        // Load the robot arm model
        var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
        var sdfPath = Path.Combine(dataDir, "arm.sdf");

        if (!File.Exists(sdfPath))
        {
            Console.WriteLine($"  ERROR: {sdfPath} not found. Run from project directory.");
            return;
        }

        var root = new Root();
        var errors = root.Load(sdfPath);
        if (errors.Count > 0)
        {
            foreach (var e in errors)
                Console.WriteLine($"  Error: {e}");
            return;
        }

        // The arm.sdf has a standalone <model> (no <world>), get it from Root
        var arm = root.StandaloneModel;
        if (arm == null)
        {
            Console.WriteLine("  No top-level model found.");
            return;
        }

        Console.WriteLine($"  Loaded model: {arm.Name}");
        Console.WriteLine($"  Links: {arm.LinkCount}, Joints: {arm.JointCount}, Frames: {arm.FrameCount}\n");

        // --- Inspect: print total mass ---
        double totalMass = 0;
        for (int i = 0; i < arm.LinkCount; i++)
        {
            var link = arm.LinkByIndex(i)!;
            totalMass += link.Inertial.Mass;
            Console.WriteLine($"  Link '{link.Name}': mass = {link.Inertial.Mass:F2} kg");
        }
        Console.WriteLine($"  Total mass: {totalMass:F2} kg\n");

        // --- Inspect: print joint limits ---
        Console.WriteLine("  Joint limits (before modification):");
        for (int i = 0; i < arm.JointCount; i++)
        {
            var joint = arm.JointByIndex(i)!;
            var axis = joint.Axis;
            if (axis != null)
            {
                Console.WriteLine($"    {joint.Name}: lower={axis.Lower:F3} rad, upper={axis.Upper:F3} rad, " +
                                  $"effort={axis.Effort:F1} N·m");
            }
        }

        // --- Modify: widen the shoulder joint limit ---
        var shoulder = arm.JointByName("shoulder_joint");
        if (shoulder?.Axis != null)
        {
            Console.WriteLine($"\n  Modifying shoulder_joint limits...");
            shoulder.Axis.Lower = -2.5;
            shoulder.Axis.Upper = 2.5;
            shoulder.Axis.Effort = 200;
        }

        // --- Modify: increase base_link mass by 50% ---
        var baseLink = arm.LinkByName("base_link");
        if (baseLink != null)
        {
            double oldMass = baseLink.Inertial.Mass;
            baseLink.Inertial.Mass = oldMass * 1.5;
            Console.WriteLine($"  base_link mass: {oldMass:F2} -> {baseLink.Inertial.Mass:F2} kg");
        }

        // --- Modify: add a new tool_center frame ---
        var tcpFrame = new Frame
        {
            Name = "tcp",
            AttachedTo = "gripper",
        };
        tcpFrame.RawPose = new Pose3d(0, 0, 0.12, 0, 0, 0);
        arm.AddFrame(tcpFrame);
        Console.WriteLine($"  Added frame 'tcp' attached to 'gripper'");

        // --- Print modified joint limits ---
        Console.WriteLine("\n  Joint limits (after modification):");
        for (int i = 0; i < arm.JointCount; i++)
        {
            var joint = arm.JointByIndex(i)!;
            var axis = joint.Axis;
            if (axis != null)
            {
                Console.WriteLine($"    {joint.Name}: lower={axis.Lower:F3} rad, upper={axis.Upper:F3} rad, " +
                                  $"effort={axis.Effort:F1} N·m");
            }
        }

        // --- Serialize the modified model ---
        Console.WriteLine("\n  --- Modified SDF XML ---");
        var rootElem = SdfDocument.WrapInRoot(arm.ToElement());
        Console.WriteLine(rootElem.ToString("  "));
    }
}
