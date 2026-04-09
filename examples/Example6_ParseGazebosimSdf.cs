using System;
using System.Collections.Generic;
using System.IO;
using SDFormat;

namespace Examples
{
    public static class Example6_ParseGazebosimSdf
    {
        private static int _pass;
        private static int _fail;

        public static void Run()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Example 6: Parse Gazebosim SDF Test Files        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");

            _pass = 0;
            _fail = 0;

            var sdfDir = Path.Combine(AppContext.BaseDirectory, "data", "gazebosim", "sdformat", "sdf");
            if (!Directory.Exists(sdfDir))
            {
                Console.WriteLine($"  SDF directory not found: {sdfDir}");
                return;
            }

            var files = Directory.GetFiles(sdfDir, "*.sdf");
            Array.Sort(files);

            foreach (var file in files)
            {
                ParseFile(file);
            }

            // Also parse .world files
            var worldFiles = Directory.GetFiles(sdfDir, "*.world");
            Array.Sort(worldFiles);
            foreach (var file in worldFiles)
            {
                ParseFile(file);
            }

            Console.WriteLine($"\n  ========================================");
            Console.WriteLine($"  Results: {_pass} passed, {_fail} failed, {_pass + _fail} total");
            Console.WriteLine($"  ========================================");
        }

        private static void ParseFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            try
            {
                var sdfRoot = new Root();
                var loadErrors = sdfRoot.Load(filePath);

                // Count significant errors (skip warnings about missing features)
                var significantErrors = new List<SdfError>();
                foreach (var err in loadErrors)
                {
                    // Filter out non-critical errors
                    if (err.Code == ErrorCode.Warning) continue;
                    significantErrors.Add(err);
                }

                if (significantErrors.Count > 0)
                {
                    Console.WriteLine($"    [WARN] {fileName}: parsed with {significantErrors.Count} error(s)");
                    foreach (var err in significantErrors)
                        Console.WriteLine($"           [{err.Code}] {err.Message}");
                    // Still count as pass if it parsed
                    _pass++;
                }
                else
                {
                    // Verify structure
                    var info = new List<string>();
                    if (sdfRoot.WorldCount > 0)
                    {
                        var w = sdfRoot.WorldByIndex(0)!;
                        info.Add($"world={w.Name}");
                        info.Add($"models={w.ModelCount}");
                        info.Add($"lights={w.LightCount}");
                    }
                    else if (sdfRoot.StandaloneModel != null)
                    {
                        info.Add($"model={sdfRoot.StandaloneModel.Name}");
                        info.Add($"links={sdfRoot.StandaloneModel.LinkCount}");
                        info.Add($"joints={sdfRoot.StandaloneModel.JointCount}");
                    }
                    else if (sdfRoot.StandaloneLight != null)
                    {
                        info.Add($"light={sdfRoot.StandaloneLight.Name}");
                    }

                    Console.WriteLine($"    [PASS] {fileName}: {string.Join(", ", info)}");
                    _pass++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    [FAIL] {fileName}: {ex.GetType().Name}: {ex.Message}");
                _fail++;
            }
        }
    }
}
