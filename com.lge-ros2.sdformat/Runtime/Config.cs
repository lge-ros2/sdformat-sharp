// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - ParserConfig.hh, OutputConfig.hh, PrintConfig.hh

#nullable enable

using System;
using System.Collections.Generic;

namespace SDFormat
{
    /// <summary>
    /// Configuration options for the SDF parser.
    /// </summary>
    public class ParserConfig
    {
        private static readonly ParserConfig _globalConfig = new();

        /// <summary>Get the global parser config (modifiable).</summary>
        public static ParserConfig GlobalConfig => _globalConfig;

        /// <summary>Map of URI scheme to filesystem paths for resolution.</summary>
        public Dictionary<string, List<string>> UriPathMap { get; } = new();

        /// <summary>Callback function to locate files by URI.</summary>
        public Func<string, string>? FindFileCallback { get; set; }

        /// <summary>Policy for warnings.</summary>
        public EnforcementPolicy WarningsPolicy { get; set; } = EnforcementPolicy.Warn;

        /// <summary>Policy for unrecognized elements.</summary>
        public EnforcementPolicy UnrecognizedElementsPolicy { get; set; } = EnforcementPolicy.Warn;

        /// <summary>Policy for deprecated elements.</summary>
        public EnforcementPolicy DeprecatedElementsPolicy { get; set; } = EnforcementPolicy.Warn;

        /// <summary>Auto-inertial calculation configuration.</summary>
        public ConfigureResolveAutoInertials CalculateInertialConfiguration { get; set; } =
            ConfigureResolveAutoInertials.SkipCalculationInLoad;

        /// <summary>Auto-inertial calculation failure policy.</summary>
        public CalculateInertialFailurePolicyType CalculateInertialFailurePolicy { get; set; } =
            CalculateInertialFailurePolicyType.Err;

        /// <summary>Whether to preserve fixed joints in URDF conversion.</summary>
        public bool UrdfPreserveFixedJoint { get; set; }

        /// <summary>Whether to store resolved URIs.</summary>
        public bool StoreResolvedUris { get; set; }

        /// <summary>Add a URI path mapping.</summary>
        public void AddUriPath(string uri, string path)
        {
            if (!UriPathMap.ContainsKey(uri))
                UriPathMap[uri] = new List<string>();
            UriPathMap[uri].Add(path);
        }
    }

    /// <summary>
    /// Configuration options for outputting SDF elements.
    /// </summary>
    public class OutputConfig
    {
        private static readonly OutputConfig _globalConfig = new();

        /// <summary>Get the global output config (modifiable).</summary>
        public static OutputConfig GlobalConfig => _globalConfig;

        /// <summary>Whether to use include tags when converting to element tree.</summary>
        public bool ToElementUseIncludeTag { get; set; } = true;
    }

    /// <summary>
    /// Configuration options for printing SDF elements as strings.
    /// </summary>
    public class PrintConfig
    {
        /// <summary>Whether to print rotations in degrees instead of radians.</summary>
        public bool RotationInDegrees { get; set; }

        /// <summary>Snap rotation values to the nearest interval in degrees.</summary>
        public int? RotationSnapToDegrees { get; set; }

        /// <summary>Tolerance for rotation snapping.</summary>
        public double? RotationSnapTolerance { get; set; }

        /// <summary>Whether to preserve include tags.</summary>
        public bool PreserveIncludes { get; set; }

        /// <summary>Output precision for floating point numbers.</summary>
        public int OutPrecision { get; set; } = -1;

        /// <summary>Set rotation snap to degrees with tolerance.</summary>
        public bool SetRotationSnapToDegrees(int interval, double tolerance)
        {
            if (interval <= 0 || tolerance < 0)
                return false;
            RotationSnapToDegrees = interval;
            RotationSnapTolerance = tolerance;
            return true;
        }
    }
}
