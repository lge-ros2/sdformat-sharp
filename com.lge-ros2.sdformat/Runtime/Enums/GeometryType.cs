// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Geometry.hh

#nullable enable

namespace SDFormat
{
    /// <summary>
    /// The set of geometry types.
    /// </summary>
    public enum GeometryType
    {
        /// <summary>Empty geometry. This is the default value.</summary>
        Empty = 0,
        /// <summary>A box geometry.</summary>
        Box = 1,
        /// <summary>A cylinder geometry.</summary>
        Cylinder = 2,
        /// <summary>A plane geometry.</summary>
        Plane = 3,
        /// <summary>A sphere geometry.</summary>
        Sphere = 4,
        /// <summary>A mesh geometry.</summary>
        Mesh = 5,
        /// <summary>A heightmap geometry.</summary>
        Heightmap = 6,
        /// <summary>A capsule geometry.</summary>
        Capsule = 7,
        /// <summary>An ellipsoid geometry.</summary>
        Ellipsoid = 8,
        /// <summary>A polyline geometry.</summary>
        Polyline = 9,
        /// <summary>A cone geometry.</summary>
        Cone = 10,
    }
}
