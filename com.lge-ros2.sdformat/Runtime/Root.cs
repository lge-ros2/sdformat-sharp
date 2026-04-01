// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Root.hh

#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SdFormat
{
    /// <summary>
    /// Root of an SDF document. This is the top-level entry point that contains
    /// worlds and optionally a standalone model, light, or actor.
    /// </summary>
    public class Root
    {
        /// <summary>SDF version string (e.g. "1.12").</summary>
        public string Version { get; set; } = SdfDocument.DefaultVersion;

        /// <summary>Worlds contained in this SDF document.</summary>
        public List<World> Worlds { get; } = new();

        /// <summary>Standalone model (mutually exclusive with worlds).</summary>
        public Model? StandaloneModel { get; set; }

        /// <summary>Standalone light.</summary>
        public Light? StandaloneLight { get; set; }

        /// <summary>Standalone actor.</summary>
        public Actor? StandaloneActor { get; set; }

        /// <summary>The underlying SDF element tree.</summary>
        public Element? Element { get; set; }

        // ---- World accessors ----

        public int WorldCount => Worlds.Count;

        public World? WorldByIndex(int index) =>
            index >= 0 && index < Worlds.Count ? Worlds[index] : null;

        public World? WorldByName(string name) =>
            Worlds.FirstOrDefault(w => w.Name == name);

        public bool WorldNameExists(string name) =>
            Worlds.Any(w => w.Name == name);

        public List<SdfError> AddWorld(World world)
        {
            var errors = new List<SdfError>();
            if (WorldNameExists(world.Name))
            {
                errors.Add(new SdfError(ErrorCode.DuplicateName,
                    $"World with name '{world.Name}' already exists."));
                return errors;
            }
            Worlds.Add(world);
            return errors;
        }

        public void ClearWorlds() => Worlds.Clear();

        /// <summary>
        /// Clear standalone model, light, and actor.
        /// </summary>
        public void ClearActorLightModel()
        {
            StandaloneModel = null;
            StandaloneLight = null;
            StandaloneActor = null;
        }

        /// <summary>
        /// Load an SDF file by path.
        /// </summary>
        public List<SdfError> Load(string filename)
        {
            return Load(filename, new ParserConfig());
        }

        /// <summary>
        /// Load an SDF file by path with parser configuration.
        /// </summary>
        public List<SdfError> Load(string filename, ParserConfig config)
        {
            var errors = new List<SdfError>();

            if (!File.Exists(filename))
            {
                errors.Add(new SdfError(ErrorCode.FileRead,
                    $"File not found: {filename}"));
                return errors;
            }

            string content;
            try { content = File.ReadAllText(filename); }
            catch (IOException ex)
            {
                errors.Add(new SdfError(ErrorCode.FileRead,
                    $"Error reading file '{filename}': {ex.Message}"));
                return errors;
            }

            errors.AddRange(LoadSdfString(content, config));
            return errors;
        }

        /// <summary>
        /// Load from an SDF string.
        /// </summary>
        public List<SdfError> LoadSdfString(string sdfString)
        {
            return LoadSdfString(sdfString, new ParserConfig());
        }

        /// <summary>
        /// Load from an SDF string with parser configuration.
        /// </summary>
        public List<SdfError> LoadSdfString(string sdfString, ParserConfig config)
        {
            var errors = new List<SdfError>();

            var parser = new SdfParser();
            var (element, parseErrors) = parser.Parse(sdfString);
            errors.AddRange(parseErrors);

            if (element == null)
            {
                errors.Add(new SdfError(ErrorCode.ParsingError,
                    "Failed to parse SDF string."));
                return errors;
            }

            errors.AddRange(LoadFromElement(element));
            return errors;
        }

        /// <summary>
        /// Load from a pre-parsed Element tree.
        /// </summary>
        public List<SdfError> LoadFromElement(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            // Get SDF version
            var versionAttr = sdf.GetAttribute("version");
            if (versionAttr != null) Version = versionAttr.GetAsString();

            // Load worlds
            var worldElem = sdf.FindElement("world");
            while (worldElem != null)
            {
                var world = new World();
                errors.AddRange(world.Load(worldElem));
                Worlds.Add(world);
                worldElem = worldElem.GetNextElement("world");
            }

            // Load standalone model
            if (sdf.HasElement("model"))
            {
                StandaloneModel = new Model();
                errors.AddRange(StandaloneModel.Load(sdf.FindElement("model")!));
            }

            // Load standalone light
            if (sdf.HasElement("light"))
            {
                StandaloneLight = new Light();
                errors.AddRange(StandaloneLight.Load(sdf.FindElement("light")!));
            }

            // Load standalone actor
            if (sdf.HasElement("actor"))
            {
                StandaloneActor = new Actor();
                errors.AddRange(StandaloneActor.Load(sdf.FindElement("actor")!));
            }

            return errors;
        }

        /// <summary>
        /// Get world names from a file without fully loading.
        /// </summary>
        public static List<SdfError> WorldNamesFromFile(string filename, out List<string> worldNames)
        {
            worldNames = new List<string>();
            var root = new Root();
            var errors = root.Load(filename);

            foreach (var world in root.Worlds)
                worldNames.Add(world.Name);

            return errors;
        }

        /// <summary>
        /// Convert back to an SDF element tree.
        /// </summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "sdf" };
            elem.AddAttribute("version", "string", SdfDocument.DefaultVersion, true);
            elem.GetAttribute("version")!.SetFromString(Version);

            foreach (var world in Worlds)
                elem.InsertElement(world.ToElement());

            if (StandaloneModel != null)
                elem.InsertElement(StandaloneModel.ToElement());
            if (StandaloneLight != null)
                elem.InsertElement(StandaloneLight.ToElement());
            if (StandaloneActor != null)
                elem.InsertElement(StandaloneActor.ToElement());

            return elem;
        }

        /// <summary>
        /// Clone this root and all its contents.
        /// </summary>
        public Root Clone()
        {
            var clone = new Root { Version = Version };
            // Re-load from our own element for deep copy
            if (Element != null)
                clone.LoadFromElement(Element.Clone());
            return clone;
        }
    }
}
