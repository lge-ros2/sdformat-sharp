// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0

using UnityEngine;

namespace SdFormat.Editor
{
    /// <summary>
    /// ScriptableObject that stores metadata about an imported .sdf file.
    /// Created automatically by <see cref="SdfImporter"/>.
    /// </summary>
    public class SdfAsset : ScriptableObject
    {
        [TextArea(3, 30)]
        public string sdfXml;

        public string fileName;
        public string version;
        public int worldCount;
        public string standaloneModelName;
        public bool hasErrors;

        [TextArea(1, 10)]
        public string errorSummary;

        /// <summary>
        /// Parse this asset into a fully populated <see cref="Root"/> DOM.
        /// </summary>
        public Root Parse()
        {
            var root = new Root();
            root.LoadSdfString(sdfXml);
            return root;
        }
    }
}
