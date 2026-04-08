// Example 1: Parse an SDF world file and print its contents
//
// Demonstrates:
//   - Loading an SDF file from disk
//   - Iterating over worlds, models, links, joints
//   - Accessing geometry and sensor information

using SDFormat;

namespace Examples
{

public static class Example1_ParseWorld
{
    public static void Run(string sdfPath)
    {
        Console.WriteLine("=== Example 1: Parse SDF World ===\n");

        var root = new Root();
        var errors = root.Load(sdfPath);

        // Print any parse errors
        if (errors.Count > 0)
        {
            Console.WriteLine($"  Errors ({errors.Count}):");
            foreach (var err in errors)
                Console.WriteLine($"    {err}");
        }

        Console.WriteLine($"  SDF Version: {root.Version}");
        Console.WriteLine($"  Worlds: {root.WorldCount}");

        for (int w = 0; w < root.WorldCount; w++)
        {
            var world = root.WorldByIndex(w)!;
            Console.WriteLine($"\n  World: '{world.Name}'");
            Console.WriteLine($"    Gravity: {world.Gravity}");
            Console.WriteLine($"    Models: {world.ModelCount}");
            Console.WriteLine($"    Lights: {world.LightCount}");
            Console.WriteLine($"    Physics profiles: {world.PhysicsCount}");

            // Physics
            for (int p = 0; p < world.PhysicsCount; p++)
            {
                var physics = world.PhysicsByIndex(p)!;
                Console.WriteLine($"\n    Physics '{physics.Name}':");
                Console.WriteLine($"      Engine: {physics.EngineType}");
                Console.WriteLine($"      Max step size: {physics.MaxStepSize}");
                Console.WriteLine($"      Real-time factor: {physics.RealTimeFactor}");
            }

            // Scene
            if (world.SceneInfo != null)
            {
                Console.WriteLine($"\n    Scene:");
                Console.WriteLine($"      Ambient: {world.SceneInfo.Ambient}");
                Console.WriteLine($"      Background: {world.SceneInfo.Background}");
                Console.WriteLine($"      Shadows: {world.SceneInfo.Shadows}");
            }

            // Models
            for (int m = 0; m < world.ModelCount; m++)
            {
                var model = world.ModelByIndex(m)!;
                Console.WriteLine($"\n    Model: '{model.Name}'");
                Console.WriteLine($"      Static: {model.Static}");
                Console.WriteLine($"      Pose: {model.RawPose}");
                Console.WriteLine($"      Links: {model.LinkCount}");
                Console.WriteLine($"      Joints: {model.JointCount}");
                Console.WriteLine($"      Plugins: {model.Plugins.Count}");

                // Links
                for (int l = 0; l < model.LinkCount; l++)
                {
                    var link = model.LinkByIndex(l)!;
                    Console.WriteLine($"\n      Link: '{link.Name}'");
                    Console.WriteLine($"        Mass: {link.Inertial.Mass} kg");
                    Console.WriteLine($"        Visuals: {link.VisualCount}");
                    Console.WriteLine($"        Collisions: {link.CollisionCount}");
                    Console.WriteLine($"        Sensors: {link.SensorCount}");

                    // Geometries
                    for (int v = 0; v < link.VisualCount; v++)
                    {
                        var visual = link.VisualByIndex(v)!;
                        Console.Write($"        Visual '{visual.Name}' -> ");
                        PrintGeometry(visual.Geom);
                    }

                    // Sensors
                    for (int s = 0; s < link.SensorCount; s++)
                    {
                        var sensor = link.SensorByIndex(s)!;
                        Console.WriteLine($"        Sensor '{sensor.Name}' (type: {sensor.TypeStr}, rate: {sensor.UpdateRate} Hz)");

                        if (sensor.Camera != null)
                        {
                            Console.WriteLine($"          Camera: {sensor.Camera.ImageWidth}x{sensor.Camera.ImageHeight}, " +
                                              $"FOV: {sensor.Camera.HorizontalFov:F3} rad");
                        }
                    }
                }

                // Joints
                for (int j = 0; j < model.JointCount; j++)
                {
                    var joint = model.JointByIndex(j)!;
                    Console.WriteLine($"\n      Joint: '{joint.Name}' ({joint.Type})");
                    Console.WriteLine($"        Parent: {joint.ParentName} -> Child: {joint.ChildName}");

                    if (joint.Axis != null)
                        Console.WriteLine($"        Axis: {joint.Axis.Xyz}");
                }

                // Plugins
                foreach (var plugin in model.Plugins)
                    Console.WriteLine($"\n      Plugin: '{plugin.Name}' ({plugin.Filename})");
            }

            // Lights
            for (int l = 0; l < world.LightCount; l++)
            {
                var light = world.LightByIndex(l)!;
                Console.WriteLine($"\n    Light: '{light.Name}' ({light.Type})");
                Console.WriteLine($"      Diffuse: {light.Diffuse}");
                Console.WriteLine($"      Direction: {light.Direction}");
                Console.WriteLine($"      Cast shadows: {light.CastShadows}");
            }
        }

        Console.WriteLine();
    }

    private static void PrintGeometry(Geometry geom)
    {
        switch (geom.Type)
        {
            case GeometryType.Box:
                Console.WriteLine($"Box (size: {geom.BoxShape!.Size})");
                break;
            case GeometryType.Sphere:
                Console.WriteLine($"Sphere (radius: {geom.SphereShape!.Radius})");
                break;
            case GeometryType.Cylinder:
                Console.WriteLine($"Cylinder (r: {geom.CylinderShape!.Radius}, l: {geom.CylinderShape.Length})");
                break;
            case GeometryType.Plane:
                Console.WriteLine($"Plane (normal: {geom.PlaneShape!.Normal}, size: {geom.PlaneShape.Size})");
                break;
            case GeometryType.Mesh:
                Console.WriteLine($"Mesh (uri: {geom.MeshShape!.Uri})");
                break;
            default:
                Console.WriteLine($"{geom.Type}");
                break;
        }
    }
}
}
