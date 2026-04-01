// SdFormat Unity Sample — Parse an SDF world file and spawn it in the scene.
//
// Usage:
//   1. Attach this MonoBehaviour to any GameObject.
//   2. Drag an .sdf TextAsset into the "sdfFile" field in the Inspector.
//   3. Press Play — the SDF world will appear as GameObjects.

using UnityEngine;
using SdFormat;
using SdFormat.Unity;

namespace SdFormat.Samples
{
    public class ParseWorldSample : MonoBehaviour
    {
        [Tooltip("Drag an .sdf file (TextAsset) here.")]
        public TextAsset sdfFile;

        [Tooltip("If sdfFile is null, paste raw SDF XML here.")]
        [TextArea(5, 20)]
        public string sdfXml = "";

        void Start()
        {
            string xml = sdfFile != null ? sdfFile.text : sdfXml;

            if (string.IsNullOrWhiteSpace(xml))
            {
                Debug.LogWarning("[ParseWorldSample] No SDF data provided.");
                return;
            }

            // Parse
            var root = new Root();
            var errors = root.LoadSdfString(xml);

            foreach (var err in errors)
                Debug.LogWarning($"[SDF Parse] {err}");

            Debug.Log($"[SDF] Version {root.Version}, {root.WorldCount} world(s)");

            for (int w = 0; w < root.WorldCount; w++)
            {
                var world = root.WorldByIndex(w);
                Debug.Log($"  World '{world.Name}': {world.ModelCount} models, " +
                          $"{world.LightCount} lights");

                for (int m = 0; m < world.ModelCount; m++)
                {
                    var model = world.ModelByIndex(m);
                    Debug.Log($"    Model '{model.Name}': {model.LinkCount} links, " +
                              $"{model.JointCount} joints");
                }
            }

            // Spawn into scene
            SdfSpawner.Spawn(root, transform);
        }
    }
}
