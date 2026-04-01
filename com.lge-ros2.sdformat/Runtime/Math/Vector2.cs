// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Lightweight math types to replace gz::math dependencies

using System;

namespace SdFormat.Math
{
    /// <summary>
    /// A 2D vector with double components.
    /// </summary>
    public struct Vector2d : IEquatable<Vector2d>
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static readonly Vector2d Zero = new(0, 0);
        public static readonly Vector2d One = new(1, 1);

        public Vector2d(double x, double y) { X = x; Y = y; }

        public double Length() => System.Math.Sqrt(X * X + Y * Y);
        public double SquaredLength() => X * X + Y * Y;

        public Vector2d Normalized()
        {
            double len = Length();
            return len > 0 ? new Vector2d(X / len, Y / len) : Zero;
        }

        public static Vector2d operator +(Vector2d a, Vector2d b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2d operator -(Vector2d a, Vector2d b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector2d operator *(Vector2d v, double s) => new(v.X * s, v.Y * s);
        public static Vector2d operator *(double s, Vector2d v) => new(v.X * s, v.Y * s);
        public static Vector2d operator -(Vector2d v) => new(-v.X, -v.Y);
        public static bool operator ==(Vector2d a, Vector2d b) => a.Equals(b);
        public static bool operator !=(Vector2d a, Vector2d b) => !a.Equals(b);

        public bool Equals(Vector2d other) => X == other.X && Y == other.Y;
        public override bool Equals(object? obj) => obj is Vector2d v && Equals(v);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"{X} {Y}";

        public static Vector2d Parse(string s)
        {
            var parts = s.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                throw new FormatException($"Cannot parse Vector2d from '{s}'");
            return new Vector2d(double.Parse(parts[0]), double.Parse(parts[1]));
        }
    }

    /// <summary>
    /// A 2D vector with integer components.
    /// </summary>
    public struct Vector2i : IEquatable<Vector2i>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static readonly Vector2i Zero = new(0, 0);

        public Vector2i(int x, int y) { X = x; Y = y; }

        public static Vector2i operator +(Vector2i a, Vector2i b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2i operator -(Vector2i a, Vector2i b) => new(a.X - b.X, a.Y - b.Y);
        public static bool operator ==(Vector2i a, Vector2i b) => a.Equals(b);
        public static bool operator !=(Vector2i a, Vector2i b) => !a.Equals(b);

        public bool Equals(Vector2i other) => X == other.X && Y == other.Y;
        public override bool Equals(object? obj) => obj is Vector2i v && Equals(v);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"{X} {Y}";
    }
}
