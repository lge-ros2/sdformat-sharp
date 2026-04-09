# Changelog

## [16.0.1] — 2026-04-09

### Added

#### Core
- Initial Unity package release — pure C#, no native dependencies.
- Full SDF 1.12 DOM: Root, World, Model, Link, Joint, Visual, Collision, Sensor, Light, Frame, Actor, Plugin.
- `SdfParser` — XML to Element tree via `System.Xml.Linq` with line/file/path tracking.
- `SdfDocument` — high-level document wrapper with SDF version preservation.
- Round-trip support: Load → Modify → `ToElement()` → XML serialization.
- Programmatic model building — construct Root/Model/Link/Joint/Sensor hierarchies entirely in code.
- Structured error reporting via `SdfError` with error code, message, file path, and line number.
- `ParserConfig` — URI path mapping, enforcement policies, auto-inertia configuration.

#### Geometry (10 shapes)
- Box, Sphere, Cylinder, Capsule, Cone, Ellipsoid, Plane, Mesh, Heightmap, Polyline.

#### Sensors (26 types)
- Camera family: Camera, DepthCamera, RgbdCamera, ThermalCamera, SegmentationCamera, BoundingBoxCamera, WideAngleCamera, Multicamera, LogicalCamera.
- Motion: IMU (6-DOF with noise), Lidar, GpuLidar.
- Navigation: GPS/NavSat, Altimeter, Magnetometer, AirPressure, AirSpeed.
- Interaction: ForceTorque, Contact, Sonar.
- Wireless: WirelessReceiver, WirelessTransmitter, RFID, RfidTag.
- Per-axis Gaussian/quantized noise and bias models.
- Camera distortion (K1–K3, P1–P2), lens intrinsics, and triggered-camera support.

#### Joints (9 types)
- Revolute, Revolute2, Prismatic, Fixed, Ball, Continuous, Screw, Universal, Gearbox.
- Dual-axis support with limits, damping, friction, spring stiffness/dissipation.
- Mimic constraints (multiplier, offset, reference joint).

#### Materials & Rendering
- Blinn-Phong: ambient, diffuse, specular, emissive colors, shininess, double-sided, lighting toggle.
- PBR metallic and specular workflows (albedo, normal, roughness, metalness, emission, AO, lightmap).
- Render-order control and material script/URI references.

#### Physics & Surface
- `Surface` — ODE, Bullet, and Torsional friction models; contact bitmasks; Poisson's ratio; elastic modulus.
- Per-link gravity, wind, velocity decay; per-model self-collision and auto-disable.
- Joint stop stiffness/dissipation, spring-loaded joints, effort/velocity limits.

#### World Environment
- `Atmosphere`, `Scene`, `Sky`, `Fog`, `Gui`, `SphericalCoordinates`, `Physics` engine config.

#### Math Utilities
- `Vector2d`, `Vector3d`, `Quaterniond`, `Pose3d`, `Color`, `Angle`, `Temperature` — all double-precision with parsing helpers.

#### Unity Integration
- `SdfUnityBridge` — coordinate conversion (SDF right-hand Z-up ↔ Unity left-hand Y-up), `ToUnity()`/`ToSdf()` extensions, `ApplyPose()`.
- `SdfSpawner` — spawn entire SDF hierarchy as GameObjects with MeshRenderers, BoxCollider/SphereCollider/CapsuleCollider, Rigidbodies, Lights.
- `SdfImporter` — ScriptedImporter for `.sdf` files; produces `SdfAsset` with quick metadata extraction.
- Two importable samples: Parse World, Build Model.

#### Access Patterns
- Index-based (`LinkByIndex`), name-based (`ModelByName`), existence checks (`LinkNameExists`), and bulk clear operations on all collections.
