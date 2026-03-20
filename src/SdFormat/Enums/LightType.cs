// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Light.hh

namespace SdFormat;

/// <summary>
/// The set of light types.
/// </summary>
public enum LightType
{
    /// <summary>An invalid light. This should not be used.</summary>
    Invalid = 0,
    /// <summary>A point light source.</summary>
    Point = 1,
    /// <summary>A spot light source.</summary>
    Spot = 2,
    /// <summary>A directional light source.</summary>
    Directional = 3,
}
