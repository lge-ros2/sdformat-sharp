# Changelog

## [15.0.0] — 2025-01-01

### Added
- Initial Unity package release.
- Full SDF 1.12 DOM (Root, World, Model, Link, Joint, Visual, Collision, Sensor, Light, …).
- `SdfParser` — XML to Element tree, no native dependencies.
- `SdfUnityBridge` — coordinate conversion (SDF Z-up → Unity Y-up).
- `SdfSpawner` — spawn GameObjects with primitives, colliders, rigidbodies, lights.
- `SdfImporter` — ScriptedImporter for `.sdf` files in the Editor.
- Two importable samples: Parse World, Build Model.
