// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0

#nullable enable

using SDFormat.Math;

namespace SDFormat
{
    /// <summary>
    /// Base class for all SDFormat domain objects.
    /// Provides a reference to the underlying SDF <see cref="Element"/> tree node.
    /// </summary>
    public abstract class SdfElement
    {
        /// <summary>The SDF element from which this object was loaded.</summary>
        public Element? Element { get; set; }
    }

    /// <summary>
    /// Base class for SDFormat entities that have a name and a pose
    /// (e.g. Model, Link, Joint, Light, Visual, Collision, Frame, Sensor, Actor).
    /// </summary>
    public abstract class SdfNamedPosedElement : SdfElement
    {
        /// <summary>Name of the element.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>The raw pose.</summary>
        public Pose3d RawPose { get; set; } = Pose3d.Zero;

        /// <summary>Name of the frame this pose is relative to.</summary>
        public string PoseRelativeTo { get; set; } = string.Empty;
    }
}
