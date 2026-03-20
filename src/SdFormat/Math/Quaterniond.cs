// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0

using System;

namespace SdFormat.Math;

/// <summary>
/// A quaternion for 3D rotations. Replaces gz::math::Quaterniond.
/// Stored as (W, X, Y, Z) where W is the scalar component.
/// </summary>
public struct Quaterniond : IEquatable<Quaterniond>
{
    public double W { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public static readonly Quaterniond Identity = new(1, 0, 0, 0);

    public Quaterniond(double w, double x, double y, double z)
    {
        W = w; X = x; Y = y; Z = z;
    }

    /// <summary>
    /// Construct from Euler angles (roll, pitch, yaw) in radians.
    /// </summary>
    public static Quaterniond FromEuler(double roll, double pitch, double yaw)
    {
        double cr = System.Math.Cos(roll * 0.5);
        double sr = System.Math.Sin(roll * 0.5);
        double cp = System.Math.Cos(pitch * 0.5);
        double sp = System.Math.Sin(pitch * 0.5);
        double cy = System.Math.Cos(yaw * 0.5);
        double sy = System.Math.Sin(yaw * 0.5);

        return new Quaterniond(
            cr * cp * cy + sr * sp * sy,
            sr * cp * cy - cr * sp * sy,
            cr * sp * cy + sr * cp * sy,
            cr * cp * sy - sr * sp * cy);
    }

    /// <summary>
    /// Construct from Euler angles vector (roll, pitch, yaw) in radians.
    /// </summary>
    public static Quaterniond FromEuler(Vector3d euler) =>
        FromEuler(euler.X, euler.Y, euler.Z);

    /// <summary>
    /// Get the Euler angles (roll, pitch, yaw) in radians.
    /// </summary>
    public Vector3d ToEuler()
    {
        // Roll (x-axis rotation)
        double sinr = 2.0 * (W * X + Y * Z);
        double cosr = 1.0 - 2.0 * (X * X + Y * Y);
        double roll = System.Math.Atan2(sinr, cosr);

        // Pitch (y-axis rotation)
        double sinp = 2.0 * (W * Y - Z * X);
        double pitch;
        if (System.Math.Abs(sinp) >= 1)
            pitch = System.Math.CopySign(System.Math.PI / 2, sinp);
        else
            pitch = System.Math.Asin(sinp);

        // Yaw (z-axis rotation)
        double siny = 2.0 * (W * Z + X * Y);
        double cosy = 1.0 - 2.0 * (Y * Y + Z * Z);
        double yaw = System.Math.Atan2(siny, cosy);

        return new Vector3d(roll, pitch, yaw);
    }

    public double Length() => System.Math.Sqrt(W * W + X * X + Y * Y + Z * Z);

    public Quaterniond Normalized()
    {
        double len = Length();
        return len > 0
            ? new Quaterniond(W / len, X / len, Y / len, Z / len)
            : Identity;
    }

    public Quaterniond Inverse()
    {
        double len2 = W * W + X * X + Y * Y + Z * Z;
        return len2 > 0
            ? new Quaterniond(W / len2, -X / len2, -Y / len2, -Z / len2)
            : Identity;
    }

    /// <summary>
    /// Rotate a vector by this quaternion.
    /// </summary>
    public Vector3d RotateVector(Vector3d v)
    {
        var q = this;
        var qVec = new Vector3d(q.X, q.Y, q.Z);
        var uv = qVec.Cross(v);
        var uuv = qVec.Cross(uv);
        return v + 2.0 * (q.W * uv + uuv);
    }

    public static Quaterniond operator *(Quaterniond a, Quaterniond b)
    {
        return new Quaterniond(
            a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z,
            a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
            a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
            a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W);
    }

    public static bool operator ==(Quaterniond a, Quaterniond b) => a.Equals(b);
    public static bool operator !=(Quaterniond a, Quaterniond b) => !a.Equals(b);

    public bool Equals(Quaterniond other) =>
        W == other.W && X == other.X && Y == other.Y && Z == other.Z;
    public override bool Equals(object? obj) => obj is Quaterniond q && Equals(q);
    public override int GetHashCode() => HashCode.Combine(W, X, Y, Z);
    public override string ToString() => $"{W} {X} {Y} {Z}";

    public static Quaterniond Parse(string s)
    {
        var parts = s.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 4)
            throw new FormatException($"Cannot parse Quaterniond from '{s}'");
        return new Quaterniond(
            double.Parse(parts[0]),
            double.Parse(parts[1]),
            double.Parse(parts[2]),
            double.Parse(parts[3]));
    }
}
