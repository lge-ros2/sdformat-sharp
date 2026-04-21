// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Include support for SDF 1.12

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>
    /// Represents an &lt;include&gt; element in an SDF document.
    /// An include references an external model/light/actor by URI
    /// and optionally merges its contents into the parent scope.
    /// </summary>
    public class Include : SdfElement
    {
        /// <summary>URI to the resource (required).</summary>
        public string Uri { get; set; } = string.Empty;

        /// <summary>Optional name override for the included entity.</summary>
        public string? Name { get; set; }

        /// <summary>Whether to merge the included model into the parent scope.</summary>
        public bool Merge { get; set; }

        /// <summary>Override the static value of the included entity.</summary>
        public bool? Static { get; set; }

        /// <summary>Placement frame name.</summary>
        public string? PlacementFrame { get; set; }

        /// <summary>Pose of the included entity.</summary>
        public Pose3d RawPose { get; set; } = Pose3d.Zero;

        /// <summary>Frame this pose is relative to.</summary>
        public string PoseRelativeTo { get; set; } = string.Empty;

        /// <summary>Plugins attached to the include.</summary>
        public List<Plugin> Plugins { get; } = new();

        /// <summary>Parse an Include from an &lt;include&gt; Element.</summary>
        public static (Include include, List<SdfError> errors) Load(Element sdf)
        {
            var inc = new Include();
            var errors = new List<SdfError>();
            inc.Element = sdf;

            var uriElem = sdf.FindElement("uri");
            if (uriElem?.Value != null)
                inc.Uri = uriElem.Value.GetAsString();
            else
                errors.Add(new SdfError(ErrorCode.ElementMissing,
                    "<include> is missing required <uri> element."));

            var nameElem = sdf.FindElement("name");
            if (nameElem?.Value != null)
                inc.Name = nameElem.Value.GetAsString();

            var mergeElem = sdf.FindElement("merge");
            if (mergeElem?.Value != null)
                inc.Merge = mergeElem.Value.BoolValue;

            var staticElem = sdf.FindElement("static");
            if (staticElem?.Value != null)
                inc.Static = staticElem.Value.BoolValue;

            var placementElem = sdf.FindElement("placement_frame");
            if (placementElem?.Value != null)
                inc.PlacementFrame = placementElem.Value.GetAsString();

            var poseElem = sdf.FindElement("pose");
            if (poseElem?.Value != null)
            {
                inc.RawPose = poseElem.Value.Pose3dValue;
                var relTo = poseElem.GetAttribute("relative_to");
                if (relTo != null) inc.PoseRelativeTo = relTo.GetAsString();
            }

            var pluginElem = sdf.FindElement("plugin");
            while (pluginElem != null)
            {
                var plugin = new Plugin();
                errors.AddRange(plugin.Load(pluginElem));
                inc.Plugins.Add(plugin);
                pluginElem = pluginElem.GetNextElement("plugin");
            }

            return (inc, errors);
        }

        /// <summary>Convert back to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "include" };

            var uriChild = new Element { Name = "uri" };
            uriChild.AddValue("string", "", true);
            uriChild.Set(Uri);
            elem.InsertElement(uriChild);

            if (!string.IsNullOrEmpty(Name))
            {
                var nameChild = new Element { Name = "name" };
                nameChild.AddValue("string", "", false);
                nameChild.Set(Name);
                elem.InsertElement(nameChild);
            }

            if (Merge)
            {
                var mergeChild = new Element { Name = "merge" };
                mergeChild.AddValue("bool", "false", false);
                mergeChild.Set("true");
                elem.InsertElement(mergeChild);
            }

            if (Static.HasValue)
            {
                var staticChild = new Element { Name = "static" };
                staticChild.AddValue("bool", "false", false);
                staticChild.Set(Static.Value ? "true" : "false");
                elem.InsertElement(staticChild);
            }

            if (!string.IsNullOrEmpty(PlacementFrame))
            {
                var pfChild = new Element { Name = "placement_frame" };
                pfChild.AddValue("string", "", false);
                pfChild.Set(PlacementFrame);
                elem.InsertElement(pfChild);
            }

            if (RawPose != Pose3d.Zero || !string.IsNullOrEmpty(PoseRelativeTo))
            {
                var poseChild = new Element { Name = "pose" };
                poseChild.AddValue("pose", "0 0 0 0 0 0", false);
                poseChild.Set(RawPose.ToString());
                if (!string.IsNullOrEmpty(PoseRelativeTo))
                {
                    poseChild.AddAttribute("relative_to", "string", "", false);
                    poseChild.GetAttribute("relative_to")!.SetFromString(PoseRelativeTo);
                }
                elem.InsertElement(poseChild);
            }

            foreach (var plugin in Plugins)
                elem.InsertElement(plugin.ToElement());

            return elem;
        }

        /// <summary>
        /// Resolve a URI to a filesystem path using the parser configuration.
        /// Looks for model.sdf or model.config in the resolved directory.
        /// </summary>
        public static string? ResolveUri(string uri, ParserConfig config)
        {
            // Try FindFileCallback first
            if (config.FindFileCallback != null)
            {
                var resolved = config.FindFileCallback(uri);
                if (!string.IsNullOrEmpty(resolved))
                    return resolved;
            }

            // Try UriPathMap
            // URI may be "model://model_name" or just "model_name"
            string scheme = "";
            string modelName = uri;

            int schemeEnd = uri.IndexOf("://", StringComparison.Ordinal);
            if (schemeEnd >= 0)
            {
                scheme = uri.Substring(0, schemeEnd);
                modelName = uri.Substring(schemeEnd + 3);
            }

            // Search in UriPathMap
            var keysToSearch = new List<string>();
            if (!string.IsNullOrEmpty(scheme))
                keysToSearch.Add(scheme);
            keysToSearch.Add(""); // empty-key paths act as global search paths

            foreach (var key in keysToSearch)
            {
                if (!config.UriPathMap.TryGetValue(key, out var paths))
                    continue;

                foreach (var basePath in paths)
                {
                    var candidate = Path.Combine(basePath, modelName);

                    // If candidate is a file, return it directly
                    if (File.Exists(candidate))
                        return candidate;

                    // If candidate is a directory, look for model.sdf or model.config
                    if (Directory.Exists(candidate))
                    {
                        var modelSdf = Path.Combine(candidate, "model.sdf");
                        if (File.Exists(modelSdf))
                            return modelSdf;

                        var modelConfig = Path.Combine(candidate, "model.config");
                        if (File.Exists(modelConfig))
                            return modelConfig;
                    }
                }
            }

            // Try as an absolute/relative path directly
            if (File.Exists(uri))
                return uri;

            if (Directory.Exists(uri))
            {
                var modelSdf = Path.Combine(uri, "model.sdf");
                if (File.Exists(modelSdf))
                    return modelSdf;
            }

            return null;
        }

        /// <summary>
        /// Resolve this include: load the referenced SDF file and return the
        /// parsed Root. Returns null if the URI cannot be resolved.
        /// </summary>
        public (Root? root, List<SdfError> errors) Resolve(ParserConfig config)
        {
            var errors = new List<SdfError>();

            var resolvedPath = ResolveUri(Uri, config);
            if (resolvedPath == null)
            {
                errors.Add(new SdfError(ErrorCode.UriLookup,
                    $"Could not resolve include URI: '{Uri}'"));
                return (null, errors);
            }

            var root = new Root();
            errors.AddRange(root.Load(resolvedPath, config));
            return (root, errors);
        }

        /// <summary>
        /// Resolve this include and apply it to a World: either merge contents
        /// into the world or add as a nested model/light/actor.
        /// </summary>
        public List<SdfError> ResolveAndApply(World world, ParserConfig config)
        {
            var errors = new List<SdfError>();
            var (root, resolveErrors) = Resolve(config);
            errors.AddRange(resolveErrors);

            if (root == null)
                return errors;

            if (Merge)
            {
                // Merge: only standalone model contents are merged into the world
                if (root.StandaloneModel != null)
                {
                    var model = root.StandaloneModel;

                    // Merge model's children into world
                    foreach (var m in model.Models)
                        world.Models.Add(m);
                    foreach (var frame in model.Frames)
                        world.Frames.Add(frame);
                    foreach (var joint in model.Joints)
                        world.Joints.Add(joint);
                    foreach (var plugin in model.Plugins)
                        world.Plugins.Add(plugin);
                }
                else if (root.Worlds.Count > 0)
                {
                    // Merge world contents
                    var srcWorld = root.Worlds[0];
                    foreach (var m in srcWorld.Models)
                        world.Models.Add(m);
                    foreach (var light in srcWorld.Lights)
                        world.Lights.Add(light);
                    foreach (var frame in srcWorld.Frames)
                        world.Frames.Add(frame);
                    foreach (var joint in srcWorld.Joints)
                        world.Joints.Add(joint);
                    foreach (var actor in srcWorld.Actors)
                        world.Actors.Add(actor);
                    foreach (var plugin in srcWorld.Plugins)
                        world.Plugins.Add(plugin);
                    foreach (var physics in srcWorld.PhysicsProfiles)
                        world.PhysicsProfiles.Add(physics);
                }
                else
                {
                    errors.Add(new SdfError(ErrorCode.MergeIncludeUnsupported,
                        $"Merge include from '{Uri}' did not contain a model or world to merge."));
                }
            }
            else
            {
                // Non-merge: add the included entity as-is
                ApplyOverrides(root);

                if (root.StandaloneModel != null)
                    world.Models.Add(root.StandaloneModel);
                else if (root.StandaloneLight != null)
                    world.Lights.Add(root.StandaloneLight);
                else if (root.StandaloneActor != null)
                    world.Actors.Add(root.StandaloneActor);
                else if (root.Worlds.Count > 0)
                {
                    errors.Add(new SdfError(ErrorCode.ElementInvalid,
                        $"Include URI '{Uri}' resolved to a world, but only model/light/actor can be included in a world without merge."));
                }
            }

            return errors;
        }

        /// <summary>
        /// Resolve this include and apply it to a Model: either merge contents
        /// into the model or add as a nested model.
        /// </summary>
        public List<SdfError> ResolveAndApply(Model model, ParserConfig config)
        {
            var errors = new List<SdfError>();
            var (root, resolveErrors) = Resolve(config);
            errors.AddRange(resolveErrors);

            if (root == null)
                return errors;

            if (Merge)
            {
                if (root.StandaloneModel != null)
                {
                    var src = root.StandaloneModel;

                    // Merge model contents into parent model
                    foreach (var link in src.Links)
                        model.Links.Add(link);
                    foreach (var joint in src.Joints)
                        model.Joints.Add(joint);
                    foreach (var frame in src.Frames)
                        model.Frames.Add(frame);
                    foreach (var nested in src.Models)
                        model.Models.Add(nested);
                    foreach (var plugin in src.Plugins)
                        model.Plugins.Add(plugin);
                }
                else
                {
                    errors.Add(new SdfError(ErrorCode.MergeIncludeUnsupported,
                        $"Merge include from '{Uri}' did not contain a standalone model to merge."));
                }
            }
            else
            {
                // Non-merge: add as nested model
                ApplyOverrides(root);

                if (root.StandaloneModel != null)
                    model.Models.Add(root.StandaloneModel);
                else
                {
                    errors.Add(new SdfError(ErrorCode.ElementInvalid,
                        $"Include URI '{Uri}' did not resolve to a model for nesting."));
                }
            }

            return errors;
        }

        /// <summary>
        /// Apply name/static/pose overrides from this Include onto the resolved Root's entity.
        /// </summary>
        private void ApplyOverrides(Root root)
        {
            if (root.StandaloneModel != null)
            {
                var m = root.StandaloneModel;
                if (!string.IsNullOrEmpty(Name))
                    m.Name = Name;
                if (Static.HasValue)
                    m.Static = Static.Value;
                if (RawPose != Pose3d.Zero || !string.IsNullOrEmpty(PoseRelativeTo))
                {
                    m.RawPose = RawPose;
                    m.PoseRelativeTo = PoseRelativeTo;
                }
                if (!string.IsNullOrEmpty(PlacementFrame))
                    m.PlacementFrameName = PlacementFrame;
                m.Uri = Uri;
                foreach (var plugin in Plugins)
                    m.Plugins.Add(plugin);
            }
            else if (root.StandaloneLight != null)
            {
                var l = root.StandaloneLight;
                if (!string.IsNullOrEmpty(Name))
                    l.Name = Name;
                if (RawPose != Pose3d.Zero || !string.IsNullOrEmpty(PoseRelativeTo))
                {
                    l.RawPose = RawPose;
                    l.PoseRelativeTo = PoseRelativeTo;
                }
            }
            else if (root.StandaloneActor != null)
            {
                var a = root.StandaloneActor;
                if (!string.IsNullOrEmpty(Name))
                    a.Name = Name;
                if (RawPose != Pose3d.Zero || !string.IsNullOrEmpty(PoseRelativeTo))
                {
                    a.RawPose = RawPose;
                    a.PoseRelativeTo = PoseRelativeTo;
                }
            }
        }
    }
}
