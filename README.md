# SdFormat-Sharp

A pure C# port of [libsdformat](https://github.com/gazebosim/sdformat) — the
**SDF (Simulation Description Format)** parser and DOM library used by
[Gazebo](https://gazebosim.org) for robot simulation.

## Features

- **Full SDF DOM** — `Root`, `World`, `Model`, `Link`, `Joint`, `Visual`,
  `Collision`, `Sensor`, `Light`, `Frame`, `Actor`, and more.
- **Geometry types** — `Box`, `Sphere`, `Cylinder`, `Capsule`, `Cone`,
  `Ellipsoid`, `Plane`, `Mesh`, `Heightmap`, `Polyline`.
- **Sensor types** — Camera, IMU, Lidar, ForceTorque, GPS/NavSat, Altimeter,
  Air Pressure, Air Speed, Magnetometer, and more.
- **XML parsing** — Built-in parser using `System.Xml.Linq`; no native dependencies.
- **Math types** — Self-contained `Vector3d`, `Quaterniond`, `Pose3d`, `Color`,
  `Angle`, `Inertial`, etc.
- **Round-trip** — Load from file/string → DOM → `ToElement()` → XML string.
- **.NET 8.0** — Modern C# with nullable reference types.

## Quick Start

```csharp
using SdFormat;

// Load an SDF file
var root = new Root();
var errors = root.Load("my_robot.sdf");

foreach (var error in errors)
    Console.WriteLine(error);

// Access the DOM
for (int i = 0; i < root.WorldCount; i++)
{
    var world = root.WorldByIndex(i)!;
    Console.WriteLine($"World: {world.Name}");

    for (int j = 0; j < world.ModelCount; j++)
    {
        var model = world.ModelByIndex(j)!;
        Console.WriteLine($"  Model: {model.Name}, Links: {model.LinkCount}");
    }
}

// Or load a standalone model
var root2 = new Root();
root2.LoadSdfString(@"
<sdf version='1.12'>
  <model name='box_robot'>
    <link name='body'>
      <visual name='visual'>
        <geometry><box><size>1 1 1</size></box></geometry>
      </visual>
      <collision name='collision'>
        <geometry><box><size>1 1 1</size></box></geometry>
      </collision>
    </link>
  </model>
</sdf>");

var model = root2.StandaloneModel!;
Console.WriteLine($"Model: {model.Name}");
Console.WriteLine($"  Link: {model.Links[0].Name}");
Console.WriteLine($"  Box size: {model.Links[0].Visuals[0].Geom.BoxShape!.Size}");
```

## Project Structure

```
src/SdFormat/
├── Enums/           # ErrorCode, JointType, GeometryType, SensorType, LightType, ...
├── Math/            # Vector2d, Vector3d, Quaterniond, Pose3d, Color, Angle, Inertial, ...
├── Geometry/        # Box, Sphere, Cylinder, Capsule, Cone, Ellipsoid, Plane, Mesh, ...
├── Sensors/         # Sensor, CameraSensor, ImuSensor, LidarSensor, ...
├── Element.cs       # Core SDF element tree node
├── Param.cs         # SDF parameter (attribute/value)
├── Root.cs          # Root entry point (top-level SDF container)
├── World.cs         # World container
├── Model.cs         # Model (robot/object)
├── Link.cs          # Link (rigid body)
├── Joint.cs         # Joint (connection between links)
├── JointAxis.cs     # Joint axis parameters + MimicConstraint
├── Visual.cs        # Visual appearance
├── Collision.cs     # Collision geometry
├── Light.cs         # Light source
├── Frame.cs         # Explicit frame
├── Material.cs      # Material + PBR properties
├── Surface.cs       # Contact + Friction (ODE, Bullet, Torsional)
├── Noise.cs         # Noise model
├── Plugin.cs        # SDF plugin element
├── SdfParser.cs     # XML → Element tree parser
├── SdfDocument.cs   # SDF document (SDFImpl equivalent)
├── SdfError.cs      # Error class
├── SemanticPose.cs  # Pose resolution (simplified)
├── Config.cs        # ParserConfig, OutputConfig, PrintConfig
└── WorldComponents.cs  # Atmosphere, Scene, Gui, Physics, Actor, ...
```

## Building

```bash
dotnet build
```

## Mapping from C++ to C#

| C++ (libsdformat) | C# (SdFormat-Sharp) |
|---|---|
| `sdf::Root` | `SdFormat.Root` |
| `sdf::World` | `SdFormat.World` |
| `sdf::Model` | `SdFormat.Model` |
| `sdf::Link` | `SdFormat.Link` |
| `sdf::Joint` | `SdFormat.Joint` |
| `sdf::Visual` | `SdFormat.Visual` |
| `sdf::Collision` | `SdFormat.Collision` |
| `sdf::Sensor` | `SdFormat.Sensor` |
| `sdf::Light` | `SdFormat.Light` |
| `sdf::Geometry` | `SdFormat.Geometry` |
| `sdf::Element` | `SdFormat.Element` |
| `sdf::Param` | `SdFormat.Param` |
| `sdf::Error` | `SdFormat.SdfError` |
| `sdf::SDF` | `SdFormat.SdfDocument` |
| `std::vector<sdf::Error>` | `List<SdfError>` |
| `sdf::ElementPtr` | `Element?` |
| `gz::math::Vector3d` | `SdFormat.Math.Vector3d` |
| `gz::math::Pose3d` | `SdFormat.Math.Pose3d` |
| `gz::math::Quaterniond` | `SdFormat.Math.Quaterniond` |
| `gz::math::Color` | `SdFormat.Math.Color` |

## License

Apache-2.0 — Same as the original libsdformat. See [LICENSE](LICENSE) and [NOTICE](NOTICE).
