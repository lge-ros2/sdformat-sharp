// Example 2: Build a robot model entirely from code (no SDF file)
//
// Demonstrates:
//   - Constructing Models, Links, Joints, Visuals, Collisions programmatically
//   - Setting poses, inertial properties, geometry shapes
//   - Serializing the result to XML via ToElement()

using SDFormat;
using SDFormat.Math;

namespace Examples
{

public static class Example2_BuildModel
{
    public static void Run()
    {
        Console.WriteLine("=== Example 2: Build Model From Code ===\n");

        // --- Create a differential-drive robot ---

        var model = new Model { Name = "my_robot", Static = false };
        model.RawPose = new Pose3d(0, 0, 0.15, 0, 0, 0);

        // Chassis link
        var chassis = new Link { Name = "chassis" };
        chassis.Inertial.Mass = 5.0;
        chassis.Inertial.Ixx = 0.1;
        chassis.Inertial.Iyy = 0.2;
        chassis.Inertial.Izz = 0.1;
        chassis.EnableGravity = true;

        // Chassis visual - a box
        var chassisVisual = new Visual { Name = "body_visual", CastShadows = true };
        chassisVisual.Geom.Type = GeometryType.Box;
        chassisVisual.Geom.BoxShape = new Box { Size = new Vector3d(0.6, 0.4, 0.2) };
        chassisVisual.MaterialInfo = new Material
        {
            Ambient = new Color(0.1f, 0.1f, 0.8f),
            Diffuse = new Color(0.2f, 0.2f, 0.9f),
        };
        chassis.AddVisual(chassisVisual);

        // Chassis collision - same box
        var chassisCollision = new Collision { Name = "body_collision" };
        chassisCollision.Geom.Type = GeometryType.Box;
        chassisCollision.Geom.BoxShape = new Box { Size = new Vector3d(0.6, 0.4, 0.2) };
        chassis.AddCollision(chassisCollision);

        model.AddLink(chassis);

        // Create two wheel links
        var leftWheel = CreateWheel("left_wheel", new Pose3d(-0.15, 0.25, -0.05, -System.Math.PI / 2, 0, 0));
        var rightWheel = CreateWheel("right_wheel", new Pose3d(-0.15, -0.25, -0.05, -System.Math.PI / 2, 0, 0));
        model.AddLink(leftWheel);
        model.AddLink(rightWheel);

        // Caster wheel (sphere)
        var caster = new Link { Name = "caster" };
        caster.RawPose = new Pose3d(0.2, 0, -0.1, 0, 0, 0);
        caster.Inertial.Mass = 0.2;

        var casterVisual = new Visual { Name = "visual" };
        casterVisual.Geom.Type = GeometryType.Sphere;
        casterVisual.Geom.SphereShape = new Sphere { Radius = 0.04 };
        caster.AddVisual(casterVisual);

        var casterCollision = new Collision { Name = "collision" };
        casterCollision.Geom.Type = GeometryType.Sphere;
        casterCollision.Geom.SphereShape = new Sphere { Radius = 0.04 };
        caster.AddCollision(casterCollision);

        model.AddLink(caster);

        // Joints
        var leftJoint = new Joint
        {
            Name = "left_wheel_joint",
            Type = JointType.Revolute,
            ParentName = "chassis",
            ChildName = "left_wheel",
            Axis = new JointAxis
            {
                Xyz = Vector3d.UnitZ,
                Lower = double.NegativeInfinity,
                Upper = double.PositiveInfinity,
                Effort = 50,
                Damping = 0.1,
            }
        };
        model.AddJoint(leftJoint);

        var rightJoint = new Joint
        {
            Name = "right_wheel_joint",
            Type = JointType.Revolute,
            ParentName = "chassis",
            ChildName = "right_wheel",
            Axis = new JointAxis
            {
                Xyz = Vector3d.UnitZ,
                Lower = double.NegativeInfinity,
                Upper = double.PositiveInfinity,
                Effort = 50,
                Damping = 0.1,
            }
        };
        model.AddJoint(rightJoint);

        var casterJoint = new Joint
        {
            Name = "caster_joint",
            Type = JointType.Ball,
            ParentName = "chassis",
            ChildName = "caster",
        };
        model.AddJoint(casterJoint);

        // Print summary
        Console.WriteLine($"  Model: {model.Name}");
        Console.WriteLine($"  Links: {model.LinkCount}");
        Console.WriteLine($"  Joints: {model.JointCount}");
        Console.WriteLine();

        for (int i = 0; i < model.LinkCount; i++)
        {
            var link = model.LinkByIndex(i)!;
            Console.WriteLine($"  Link '{link.Name}': mass={link.Inertial.Mass} kg, " +
                              $"visuals={link.VisualCount}, collisions={link.CollisionCount}");
        }

        for (int i = 0; i < model.JointCount; i++)
        {
            var joint = model.JointByIndex(i)!;
            Console.WriteLine($"  Joint '{joint.Name}': {joint.Type}, {joint.ParentName} -> {joint.ChildName}");
        }

        // Serialize to SDF XML
        Console.WriteLine("\n  --- Generated SDF XML ---");
        var rootElem = SdfDocument.WrapInRoot(model.ToElement());
        Console.WriteLine(rootElem.ToString("  "));
    }

    private static Link CreateWheel(string name, Pose3d pose)
    {
        var link = new Link { Name = name };
        link.RawPose = pose;
        link.Inertial.Mass = 0.5;

        var visual = new Visual { Name = "visual" };
        visual.Geom.Type = GeometryType.Cylinder;
        visual.Geom.CylinderShape = new Cylinder { Radius = 0.1, Length = 0.04 };
        visual.MaterialInfo = new Material
        {
            Ambient = new Color(0.3f, 0.3f, 0.3f),
            Diffuse = new Color(0.4f, 0.4f, 0.4f),
        };
        link.AddVisual(visual);

        var collision = new Collision { Name = "collision" };
        collision.Geom.Type = GeometryType.Cylinder;
        collision.Geom.CylinderShape = new Cylinder { Radius = 0.1, Length = 0.04 };
        link.AddCollision(collision);

        return link;
    }
}
}
