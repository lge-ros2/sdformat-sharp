// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0

using System;

namespace SdFormat.Math;

/// <summary>
/// A 3D vector with double components. Replaces gz::math::Vector3d.
/// </summary>
public struct Vector3d : IEquatable<Vector3d>
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public static readonly Vector3d Zero = new(0, 0, 0);
    public static readonly Vector3d One = new(1, 1, 1);
    public static readonly Vector3d UnitX = new(1, 0, 0);
    public static readonly Vector3d UnitY = new(0, 1, 0);
    public static readonly Vector3d UnitZ = new(0, 0, 1);

    public Vector3d(double x, double y, double z) { X = x; Y = y; Z = z; }

    public double Length() => System.Math.Sqrt(X * X + Y * Y + Z * Z);
    public double SquaredLength() => X * X + Y * Y + Z * Z;

    public Vector3d Normalized()
    {
        double len = Length();
        return len > 0 ? new Vector3d(X / len, Y / len, Z / len) : Zero;
    }

    public double Dot(Vector3d other) => X * other.X + Y * other.Y + Z * other.Z;

    public Vector3d Cross(Vector3d other) => new(
        Y * other.Z - Z * other.Y,
        Z * other.X - X * other.Z,
        X * other.Y - Y * other.X);

    public static Vector3d operator +(Vector3d a, Vector3d b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3d operator -(Vector3d a, Vector3d b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3d operator *(Vector3d v, double s) => new(v.X * s, v.Y * s, v.Z * s);
    public static Vector3d operator *(double s, Vector3d v) => new(v.X * s, v.Y * s, v.Z * s);
    public static Vector3d operator -(Vector3d v) => new(-v.X, -v.Y, -v.Z);
    public static bool operator ==(Vector3d a, Vector3d b) => a.Equals(b);
    public static bool operator !=(Vector3d a, Vector3d b) => !a.Equals(b);

    public bool Equals(Vector3d other) => X == other.X && Y == other.Y && Z == other.Z;
    public override bool Equals(object? obj) => obj is Vector3d v && Equals(v);
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    public override string ToString() => $"{X} {Y} {Z}";

    public static Vector3d Parse(string s)
    {
        var parts = s.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
            throw new FormatException($"Cannot parse Vector3d from '{s}'");
        return new Vector3d(
            double.Parse(parts[0]),
            double.Parse(parts[1]),
            double.Parse(parts[2]));
    }
}
