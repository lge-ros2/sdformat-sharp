// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace SDFormat.Editor
{
    /// <summary>
    /// ScriptedImporter that recognizes .sdf files in the Unity project
    /// and produces a TextAsset plus an <see cref="SdfAsset"/> wrapper so
    /// users can double-click or reference SDF files from inspectors.
    /// </summary>
    [ScriptedImporter(1, "sdf")]
    public class SdfImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var text = File.ReadAllText(ctx.assetPath);

            // Main asset: a TextAsset (viewable in Inspector)
            var textAsset = new TextAsset(text);
            ctx.AddObjectToAsset("sdfText", textAsset);

            // Secondary asset: parsed metadata
            var sdfAsset = ScriptableObject.CreateInstance<SdfAsset>();
            sdfAsset.sdfXml = text;
            sdfAsset.fileName = Path.GetFileNameWithoutExtension(ctx.assetPath);

            // Quick parse to extract summary info
            var root = new Root();
            var errors = root.LoadSdfString(text);
            sdfAsset.version = root.Version;
            sdfAsset.worldCount = root.WorldCount;
            sdfAsset.hasErrors = errors.Count > 0;
            sdfAsset.errorSummary = errors.Count > 0
                ? string.Join("\n", errors.ConvertAll(e => e.ToString()))
                : string.Empty;

            if (root.StandaloneModel != null)
                sdfAsset.standaloneModelName = root.StandaloneModel.Name;

            ctx.AddObjectToAsset("sdfAsset", sdfAsset);
            ctx.SetMainObject(textAsset);
        }
    }
}
