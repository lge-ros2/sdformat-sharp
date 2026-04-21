# SDFormat-Sharp Examples & Tests

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

## Build

```bash
dotnet build examples/Examples.csproj
```

## Run

### Interactive mode

```bash
dotnet run --project examples/Examples.csproj
```

This displays a menu where you can pick an example by number.

### Run a specific example

```bash
dotnet run --project examples/Examples.csproj -- <number>
```

### Run all examples

```bash
dotnet run --project examples/Examples.csproj -- 0
```

## Examples

| # | Name | Description |
|---|------|-------------|
| 1 | **Parse World** | Load and print `world.sdf` |
| 2 | **Build Model** | Create a model programmatically |
| 3 | **Inspect/Modify** | Load, edit, and re-serialize an SDF |
| 4 | **Sensors** | Create a sensor-rich model |
| 5 | **Full Parse** | Comprehensive SDF element verification (381 checks) |
| 6 | **Gazebosim** | Parse real Gazebosim SDF files |
| 7 | **New Elements** | Verify SDF 1.12 additions (191 checks) |
| 8 | **Nested Elems** | Verify nested structure parsing (185 checks) |

## Test Coverage

Examples 5, 7, and 8 serve as verification tests with pass/fail assertions.

### Example 5 — Full Parse (381 checks)

Verifies all core SDF elements: world, model, link, joint, visual, collision, geometry (box, sphere, cylinder, capsule, ellipsoid, mesh, plane, heightmap, polyline), material, sensor types, plugin, frame, and more.

```bash
dotnet run --project examples/Examples.csproj -- 5
```

### Example 7 — New SDF 1.12 Elements (191 checks)

Verifies newly implemented SDF 1.12 features:

- Physics engine params (ODE, Bullet, Simbody, DART)
- Road, Population, State
- Gripper
- Link audio (audio\_sink, audio\_source), fluid\_added\_mass
- Surface contact params (ODE, Bullet, DART soft\_contact)
- Geometry types (empty, image, mesh convex\_decomposition)
- Sensor sub-types (contact, wireless transceiver)
- Joint (gearbox params, JointPhysics with ODE/Simbody)

```bash
dotnet run --project examples/Examples.csproj -- 7
```

### Example 8 — Nested Elements (185 checks)

Verifies deeply nested structures are parsed correctly:

- 3-level nested models (top\_robot > end\_effector > sensor\_module)
- Link > Visual > Geometry > Material chain
- Link > Collision > Surface > Friction/Contact chain
- Link > Sensor (camera, lidar, IMU, force\_torque)
- Joints referencing nested model links (`::` scoped names)
- Frames with `attached_to` and `pose relative_to`
- Grippers referencing nested model links
- Nested state (3-level model states with link/joint states)
- World-level lights and frames

```bash
dotnet run --project examples/Examples.csproj -- 8
```

## Test Data

| File | Used by |
|------|---------|
| `data/world.sdf` | Example 1 |
| `data/complex_world.sdf` | Example 5 |
| `data/test_new_elements.sdf` | Example 7 |
| `data/test_nested_elements.sdf` | Example 8 |
| `data/gazebosim/` | Example 6 |
