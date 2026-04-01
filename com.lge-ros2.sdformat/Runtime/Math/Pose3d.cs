// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0

using System;

namespace SdFormat.Math
{
    /// <summary>
    /// A 3D pose composed of a position (Vector3d) and rotation (Quaterniond).
    /// Replaces gz::math::Pose3d.
    /// </summary>
    public struct Pose3d : IEquatable<Pose3d>
    {
        /// <summary>The position component.</summary>
        public Vector3d Position { get; set; }

        /// <summary>The rotation component.</summary>
        public Quaterniond Rotation { get; set; }

        public static readonly Pose3d Zero = new(Vector3d.Zero, Quaterniond.Identity);

        public Pose3d(Vector3d position, Quaterniond rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Pose3d(double x, double y, double z, double roll, double pitch, double yaw)
        {
            Position = new Vector3d(x, y, z);
            Rotation = Quaterniond.FromEuler(roll, pitch, yaw);
        }

        public Pose3d(double x, double y, double z, double qw, double qx, double qy, double qz)
        {
            Position = new Vector3d(x, y, z);
            Rotation = new Quaterniond(qw, qx, qy, qz);
        }

        /// <summary>
        /// Return the inverse of this pose.
        /// </summary>
        public Pose3d Inverse()
        {
            var invRot = Rotation.Inverse();
            return new Pose3d(invRot.RotateVector(-Position), invRot);
        }

        /// <summary>
        /// Compose two poses: result = this * other
        /// </summary>
        public static Pose3d operator *(Pose3d a, Pose3d b)
        {
            return new Pose3d(
                a.Position + a.Rotation.RotateVector(b.Position),
                a.Rotation * b.Rotation);
        }

        public static bool operator ==(Pose3d a, Pose3d b) => a.Equals(b);
        public static bool operator !=(Pose3d a, Pose3d b) => !a.Equals(b);

        public bool Equals(Pose3d other) =>
            Position.Equals(other.Position) && Rotation.Equals(other.Rotation);
        public override bool Equals(object? obj) => obj is Pose3d p && Equals(p);
        public override int GetHashCode() => HashCode.Combine(Position, Rotation);

        /// <summary>
        /// Returns a string representation as "x y z roll pitch yaw" (radians).
        /// </summary>
        public override string ToString()
        {
            var euler = Rotation.ToEuler();
            return $"{Position.X} {Position.Y} {Position.Z} {euler.X} {euler.Y} {euler.Z}";
        }

        /// <summary>
        /// Parse from "x y z roll pitch yaw" format.
        /// </summary>
        public static Pose3d Parse(string s)
        {
            var parts = s.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 6)
                throw new FormatException($"Cannot parse Pose3d from '{s}'");
            return new Pose3d(
                double.Parse(parts[0]),
                double.Parse(parts[1]),
                double.Parse(parts[2]),
                double.Parse(parts[3]),
                double.Parse(parts[4]),
                double.Parse(parts[5]));
        }
    }
}
