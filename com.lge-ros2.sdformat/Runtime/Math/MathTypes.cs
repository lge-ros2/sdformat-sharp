// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0

#nullable enable

using System;

namespace SDFormat.Math
{
    /// <summary>
    /// An RGBA color with float components [0, 1]. Replaces gz::math::Color.
    /// </summary>
    public struct Color : IEquatable<Color>
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public static readonly Color White = new(1f, 1f, 1f, 1f);
        public static readonly Color Black = new(0f, 0f, 0f, 1f);

        public Color(float r, float g, float b, float a = 1f)
        {
            R = r; G = g; B = b; A = a;
        }

        public static bool operator ==(Color a, Color b) => a.Equals(b);
        public static bool operator !=(Color a, Color b) => !a.Equals(b);

        public bool Equals(Color other) =>
            R == other.R && G == other.G && B == other.B && A == other.A;
        public override bool Equals(object? obj) => obj is Color c && Equals(c);
        public override int GetHashCode() => HashCode.Combine(R, G, B, A);
        public override string ToString() => $"{R} {G} {B} {A}";

        public static Color Parse(string s)
        {
            var parts = s.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3)
                throw new FormatException($"Cannot parse Color from '{s}'");
            float a = parts.Length >= 4 ? float.Parse(parts[3]) : 1f;
            return new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), a);
        }
    }

    /// <summary>
    /// Represents an angle in radians. Replaces gz::math::Angle.
    /// </summary>
    public struct Angle : IEquatable<Angle>, IComparable<Angle>
    {
        /// <summary>The angle in radians.</summary>
        public double Radians { get; set; }

        public static readonly Angle Zero = new(0);

        public Angle(double radians) => Radians = radians;

        /// <summary>Create from degrees.</summary>
        public static Angle FromDegrees(double degrees) =>
            new(degrees * System.Math.PI / 180.0);

        /// <summary>Get value in degrees.</summary>
        public double Degrees => Radians * 180.0 / System.Math.PI;

        public static bool operator ==(Angle a, Angle b) => a.Radians == b.Radians;
        public static bool operator !=(Angle a, Angle b) => a.Radians != b.Radians;
        public static bool operator <(Angle a, Angle b) => a.Radians < b.Radians;
        public static bool operator >(Angle a, Angle b) => a.Radians > b.Radians;
        public static Angle operator +(Angle a, Angle b) => new(a.Radians + b.Radians);
        public static Angle operator -(Angle a, Angle b) => new(a.Radians - b.Radians);

        public bool Equals(Angle other) => Radians == other.Radians;
        public override bool Equals(object? obj) => obj is Angle a && Equals(a);
        public override int GetHashCode() => Radians.GetHashCode();
        public int CompareTo(Angle other) => Radians.CompareTo(other.Radians);
        public override string ToString() => $"{Radians}";
    }

    /// <summary>
    /// Represents a temperature value in Kelvin. Replaces gz::math::Temperature.
    /// </summary>
    public struct Temperature : IEquatable<Temperature>
    {
        /// <summary>Temperature in Kelvin.</summary>
        public double Kelvin { get; set; }

        public Temperature(double kelvin) => Kelvin = kelvin;

        public double Celsius => Kelvin - 273.15;
        public static Temperature FromCelsius(double c) => new(c + 273.15);
        public double Fahrenheit => Celsius * 9.0 / 5.0 + 32;
        public static Temperature FromFahrenheit(double f) => new((f - 32) * 5.0 / 9.0 + 273.15);

        public static bool operator ==(Temperature a, Temperature b) => a.Kelvin == b.Kelvin;
        public static bool operator !=(Temperature a, Temperature b) => a.Kelvin != b.Kelvin;

        public bool Equals(Temperature other) => Kelvin == other.Kelvin;
        public override bool Equals(object? obj) => obj is Temperature t && Equals(t);
        public override int GetHashCode() => Kelvin.GetHashCode();
        public override string ToString() => $"{Kelvin}";
    }

    /// <summary>
    /// An axis-aligned bounding box. Replaces gz::math::AxisAlignedBox.
    /// </summary>
    public struct AxisAlignedBox
    {
        public Vector3d Min { get; set; }
        public Vector3d Max { get; set; }

        public AxisAlignedBox(Vector3d min, Vector3d max)
        {
            Min = min;
            Max = max;
        }

        public Vector3d Size => Max - Min;
        public Vector3d Center => (Min + Max) * 0.5;
    }

    /// <summary>
    /// Inertial properties (mass, pose, moment of inertia). Replaces gz::math::Inertiald.
    /// </summary>
    public class Inertial
    {
        /// <summary>Mass in kg.</summary>
        public double Mass { get; set; }

        /// <summary>Pose of the center of mass.</summary>
        public Pose3d Pose { get; set; } = Pose3d.Zero;

        /// <summary>Moment of inertia Ixx.</summary>
        public double Ixx { get; set; }
        /// <summary>Moment of inertia Iyy.</summary>
        public double Iyy { get; set; }
        /// <summary>Moment of inertia Izz.</summary>
        public double Izz { get; set; }
        /// <summary>Product of inertia Ixy.</summary>
        public double Ixy { get; set; }
        /// <summary>Product of inertia Ixz.</summary>
        public double Ixz { get; set; }
        /// <summary>Product of inertia Iyz.</summary>
        public double Iyz { get; set; }
    }

    /// <summary>
    /// Spherical coordinates. Replaces gz::math::SphericalCoordinates.
    /// </summary>
    public class SphericalCoordinates
    {
        /// <summary>Surface type.</summary>
        public enum SurfaceType
        {
            EarthWgs84
        }

        public SurfaceType Surface { get; set; } = SurfaceType.EarthWgs84;
        public string WorldFrameOrientation { get; set; } = "ENU";
        public double LatitudeDeg { get; set; }
        public double LongitudeDeg { get; set; }
        public double ElevationM { get; set; }
        public double HeadingDeg { get; set; }
    }

    /// <summary>
    /// SDF Time representation.
    /// </summary>
    public struct SdfTime : IEquatable<SdfTime>
    {
        public int Seconds { get; set; }
        public int Nanoseconds { get; set; }

        public SdfTime(int sec, int nsec) { Seconds = sec; Nanoseconds = nsec; }

        public double ToDouble() => Seconds + Nanoseconds / 1_000_000_000.0;

        public bool Equals(SdfTime other) =>
            Seconds == other.Seconds && Nanoseconds == other.Nanoseconds;
        public override bool Equals(object? obj) => obj is SdfTime t && Equals(t);
        public override int GetHashCode() => HashCode.Combine(Seconds, Nanoseconds);
        public override string ToString() => $"{Seconds} {Nanoseconds}";

        public static bool operator ==(SdfTime a, SdfTime b) => a.Equals(b);
        public static bool operator !=(SdfTime a, SdfTime b) => !a.Equals(b);
    }
}
