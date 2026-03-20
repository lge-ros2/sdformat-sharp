# SdFormat — Unity Package

A pure C# **SDF (Simulation Description Format)** parser and DOM library for Unity.  
Parse, inspect, modify, and build SDF robot / world descriptions — no native plugins required.

## Features

- **Parse** `.sdf` files or XML strings into a strongly-typed DOM (Root → World → Model → Link → Visual / Collision / Sensor / Joint …)
- **Build** models entirely from code using the DOM classes
- **Serialize** back to SDF XML via `ToElement().ToString()`
- **Unity bridge** — convert SDF coordinate system (right-hand Z-up) to Unity (left-hand Y-up)
- **Auto-spawn** — `SdfSpawner.Spawn(root)` creates a full GameObject hierarchy with primitives, colliders, rigidbodies, and lights
- **Editor importer** — `.sdf` files dropped in Assets are recognized and can be assigned to `TextAsset` fields

## Requirements

| Requirement | Minimum |
|---|---|
| Unity | **2022.3 LTS** or newer |
| .NET | .NET Standard 2.1 / .NET Framework (Mono) — both work |
| Dependencies | **None** (pure C#, uses only `System.Xml.Linq`) |

## Installation

### Option A — Local folder (recommended for development)

1. Copy or symlink the `com.gazebosim.sdformat` folder into your project's `Packages/` directory:

```
YourUnityProject/
  Packages/
    com.gazebosim.sdformat/   ← this folder
```

2. Unity will detect it automatically.

### Option B — Git URL

In Unity, go to **Window → Package Manager → + → Add package from git URL…** and enter:

```
https://github.com/lge-ros2/sdformat-sharp.git?path=com.gazebosim.sdformat
```

### Option C — Tarball

```bash
cd com.gazebosim.sdformat
npm pack          # produces com.gazebosim.sdformat-15.0.0.tgz
```

Then **Window → Package Manager → + → Add package from tarball…**

## Quick Start

### Parse an SDF file

```csharp
using SdFormat;

var root = new Root();
var errors = root.LoadSdfString(mySdfXml);

for (int w = 0; w < root.WorldCount; w++)
{
    var world = root.WorldByIndex(w);
    Debug.Log($"World '{world.Name}' has {world.ModelCount} models");
}
```

### Spawn into Unity scene

```csharp
using SdFormat;
using SdFormat.Unity;

var root = new Root();
root.LoadSdfString(sdfXml);
SdfSpawner.Spawn(root);   // creates GameObjects with primitives & colliders
```

### Build a model from code

```csharp
using SdFormat;
using SdFormat.Math;

var model = new Model { Name = "my_box" };
var link = new Link { Name = "body" };
var vis = new Visual { Name = "vis" };
vis.Geom.Type = GeometryType.Box;
vis.Geom.BoxShape = new Box { Size = new Vector3d(1, 1, 1) };
link.AddVisual(vis);
model.AddLink(link);

string xml = model.ToElement().ToString("");
```

### Coordinate conversion

```csharp
using SdFormat.Unity;
using SdFormat.Math;

Vector3d sdfPos = new Vector3d(1, 2, 3);
UnityEngine.Vector3 unityPos = sdfPos.ToUnity(); // (1, 3, 2)

transform.ApplyPose(somePose3d);
Pose3d back = transform.ToPose3d();
```

## Assembly Definitions

| Assembly | Description |
|---|---|
| `Gazebosim.SdFormat` | Core parser & DOM — no Unity dependency, `noEngineReferences: true` |
| `Gazebosim.SdFormat.Unity` | Bridge, spawner — references `UnityEngine` |
| `Gazebosim.SdFormat.Editor` | `.sdf` importer — Editor-only |

## Samples

Import via **Package Manager → SdFormat → Samples**:

- **Parse World** — Load `world.sdf`, log structure, spawn GameObjects
- **Build Model** — Construct a diff-drive robot from code, serialize & spawn

## License

Apache-2.0 — see [LICENSE](LICENSE).
