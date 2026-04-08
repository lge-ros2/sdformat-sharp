// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0

#nullable enable

using UnityEngine;
using SDFormat.Math;

namespace SDFormat.Unity
{
    /// <summary>
    /// Extension methods that convert SDFormat math types to Unity math types
    /// and vice versa, handling coordinate system differences.
    ///
    /// SDF uses a right-handed Z-up coordinate system.
    /// Unity uses a left-handed Y-up coordinate system.
    ///
    /// Conversion: SDF (X, Y, Z) → Unity (X, Z, Y) with Z negated for handedness.
    /// </summary>
    public static class SdfUnityBridge
    {
        // ──────────────────────── Vectors ────────────────────────

        /// <summary>Convert SDF Vector3d (right-hand Z-up) → Unity Vector3 (left-hand Y-up).</summary>
        public static Vector3 ToUnity(this Vector3d v)
            => new Vector3((float)v.X, (float)v.Z, (float)v.Y);

        /// <summary>Convert Unity Vector3 → SDF Vector3d.</summary>
        public static Vector3d ToSdf(this Vector3 v)
            => new Vector3d(v.x, v.z, v.y);

        /// <summary>Convert SDF Vector3d to Unity Vector3 without coordinate system change (raw).</summary>
        public static Vector3 ToUnityRaw(this Vector3d v)
            => new Vector3((float)v.X, (float)v.Y, (float)v.Z);

        // ──────────────────────── Quaternions ────────────────────────

        /// <summary>Convert SDF Quaterniond (right-hand Z-up) → Unity Quaternion (left-hand Y-up).</summary>
        public static Quaternion ToUnity(this Quaterniond q)
        {
            // SDF: (W, X, Y, Z) right-hand
            // Swap Y↔Z and negate to switch handedness
            return new Quaternion(
                -(float)q.X,
                -(float)q.Z,
                -(float)q.Y,
                (float)q.W);
        }

        /// <summary>Convert Unity Quaternion → SDF Quaterniond.</summary>
        public static Quaterniond ToSdf(this Quaternion q)
        {
            return new Quaterniond(q.w, -q.x, -q.z, -q.y);
        }

        // ──────────────────────── Poses ────────────────────────

        /// <summary>Convert SDF Pose3d → Unity position + rotation.</summary>
        public static (Vector3 position, Quaternion rotation) ToUnity(this Pose3d pose)
        {
            return (pose.Position.ToUnity(), pose.Rotation.ToUnity());
        }

        /// <summary>Apply an SDF Pose3d to a Unity Transform.</summary>
        public static void ApplyPose(this Transform transform, Pose3d pose)
        {
            var (pos, rot) = pose.ToUnity();
            transform.localPosition = pos;
            transform.localRotation = rot;
        }

        /// <summary>Read a Unity Transform into an SDF Pose3d.</summary>
        public static Pose3d ToPose3d(this Transform transform)
        {
            return new Pose3d(
                transform.localPosition.ToSdf(),
                transform.localRotation.ToSdf());
        }

        // ──────────────────────── Colors ────────────────────────

        /// <summary>Convert SDF Color → Unity Color.</summary>
        public static UnityEngine.Color ToUnity(this SDFormat.Math.Color c)
            => new UnityEngine.Color(c.R, c.G, c.B, c.A);

        /// <summary>Convert Unity Color → SDF Color.</summary>
        public static SDFormat.Math.Color ToSdf(this UnityEngine.Color c)
            => new SDFormat.Math.Color(c.r, c.g, c.b, c.a);

        // ──────────────────────── Vector2 ────────────────────────

        /// <summary>Convert SDF Vector2d → Unity Vector2.</summary>
        public static Vector2 ToUnity(this SDFormat.Math.Vector2d v)
            => new Vector2((float)v.X, (float)v.Y);

        /// <summary>Convert Unity Vector2 → SDF Vector2d.</summary>
        public static SDFormat.Math.Vector2d ToSdf(this Vector2 v)
            => new SDFormat.Math.Vector2d(v.x, v.y);
    }
}
