// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Joint.hh

namespace SdFormat
{
    /// <summary>
    /// The set of joint types. Used by the Joint class to define the type of joint.
    /// </summary>
    public enum JointType
    {
        /// <summary>An invalid joint. This should not be used.</summary>
        Invalid = 0,
        /// <summary>A ball and socket joint.</summary>
        Ball = 1,
        /// <summary>A hinge joint that rotates on a single axis with a continuous range of motion.</summary>
        Continuous = 2,
        /// <summary>A joint with zero degrees of freedom that rigidly connects two links.</summary>
        Fixed = 3,
        /// <summary>A gearbox joint.</summary>
        Gearbox = 4,
        /// <summary>A sliding joint that slides along an axis with a limited range.</summary>
        Prismatic = 5,
        /// <summary>A hinge joint that rotates on a single axis with a fixed range of motion.</summary>
        Revolute = 6,
        /// <summary>Same as two revolute joints connected in series.</summary>
        Revolute2 = 7,
        /// <summary>A single DOF joint with coupled sliding and rotational motion.</summary>
        Screw = 8,
        /// <summary>Like a ball joint but constrains one degree of freedom.</summary>
        Universal = 9,
    }
}
