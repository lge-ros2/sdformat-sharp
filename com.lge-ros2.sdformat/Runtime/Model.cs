// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Model.hh

#nullable enable

using System.Collections.Generic;
using System.Linq;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>
    /// A model describes a complete robot or object, consisting of links
    /// connected by joints, with optional nested models.
    /// </summary>
    public class Model
    {
        /// <summary>Name of the model.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Whether the model is static (does not move).</summary>
        public bool Static { get; set; }

        /// <summary>Whether self-collision is enabled.</summary>
        public bool SelfCollide { get; set; }

        /// <summary>Whether auto-disable is allowed.</summary>
        public bool AllowAutoDisable { get; set; } = true;

        /// <summary>Whether wind is enabled for this model.</summary>
        public bool EnableWind { get; set; }

        /// <summary>The raw pose.</summary>
        public Pose3d RawPose { get; set; } = Pose3d.Zero;

        /// <summary>Name of the frame this pose is relative to.</summary>
        public string PoseRelativeTo { get; set; } = string.Empty;

        /// <summary>Name of the canonical link.</summary>
        public string CanonicalLinkName { get; set; } = string.Empty;

        /// <summary>Placement frame name.</summary>
        public string PlacementFrameName { get; set; } = string.Empty;

        /// <summary>URI of the included model.</summary>
        public string Uri { get; set; } = string.Empty;

        /// <summary>Links in this model.</summary>
        public List<Link> Links { get; } = new();

        /// <summary>Joints in this model.</summary>
        public List<Joint> Joints { get; } = new();

        /// <summary>Explicit frames in this model.</summary>
        public List<Frame> Frames { get; } = new();

        /// <summary>Nested models.</summary>
        public List<Model> Models { get; } = new();

        /// <summary>Plugins.</summary>
        public List<Plugin> Plugins { get; } = new();

        /// <summary>The SDF element.</summary>
        public Element? Element { get; set; }

        // ---- Link accessors ----
        public int LinkCount => Links.Count;
        public Link? LinkByIndex(int index) =>
            index >= 0 && index < Links.Count ? Links[index] : null;
        public Link? LinkByName(string name) =>
            Links.FirstOrDefault(l => l.Name == name);
        public bool LinkNameExists(string name) =>
            Links.Any(l => l.Name == name);
        public bool AddLink(Link link)
        {
            if (LinkNameExists(link.Name)) return false;
            Links.Add(link);
            return true;
        }
        public void ClearLinks() => Links.Clear();

        // ---- Joint accessors ----
        public int JointCount => Joints.Count;
        public Joint? JointByIndex(int index) =>
            index >= 0 && index < Joints.Count ? Joints[index] : null;
        public Joint? JointByName(string name) =>
            Joints.FirstOrDefault(j => j.Name == name);
        public bool JointNameExists(string name) =>
            Joints.Any(j => j.Name == name);
        public bool AddJoint(Joint joint)
        {
            if (JointNameExists(joint.Name)) return false;
            Joints.Add(joint);
            return true;
        }
        public void ClearJoints() => Joints.Clear();

        // ---- Frame accessors ----
        public int FrameCount => Frames.Count;
        public Frame? FrameByIndex(int index) =>
            index >= 0 && index < Frames.Count ? Frames[index] : null;
        public Frame? FrameByName(string name) =>
            Frames.FirstOrDefault(f => f.Name == name);
        public bool FrameNameExists(string name) =>
            Frames.Any(f => f.Name == name);
        public bool AddFrame(Frame frame)
        {
            if (FrameNameExists(frame.Name)) return false;
            Frames.Add(frame);
            return true;
        }
        public void ClearFrames() => Frames.Clear();

        // ---- Model accessors (nested) ----
        public int ModelCount => Models.Count;
        public Model? ModelByIndex(int index) =>
            index >= 0 && index < Models.Count ? Models[index] : null;
        public Model? ModelByName(string name) =>
            Models.FirstOrDefault(m => m.Name == name);
        public bool ModelNameExists(string name) =>
            Models.Any(m => m.Name == name);
        public bool AddModel(Model model)
        {
            if (ModelNameExists(model.Name)) return false;
            Models.Add(model);
            return true;
        }
        public void ClearModels() => Models.Clear();

        // ---- Plugin accessors ----
        public void ClearPlugins() => Plugins.Clear();
        public void AddPlugin(Plugin plugin) => Plugins.Add(plugin);

        /// <summary>Get the canonical link (first link if not specified).</summary>
        public Link? CanonicalLink
        {
            get
            {
                if (!string.IsNullOrEmpty(CanonicalLinkName))
                    return LinkByName(CanonicalLinkName);
                return Links.Count > 0 ? Links[0] : null;
            }
        }

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var canonicalLink = sdf.GetAttribute("canonical_link");
            if (canonicalLink != null) CanonicalLinkName = canonicalLink.GetAsString();

            var placementFrame = sdf.GetAttribute("placement_frame");
            if (placementFrame != null) PlacementFrameName = placementFrame.GetAsString();

            var poseElem = sdf.FindElement("pose");
            if (poseElem?.Value != null)
            {
                RawPose = poseElem.Value.Pose3dValue;
                var relTo = poseElem.GetAttribute("relative_to");
                if (relTo != null) PoseRelativeTo = relTo.GetAsString();
            }

            var staticElem = sdf.FindElement("static");
            if (staticElem?.Value != null) Static = staticElem.Value.BoolValue;

            var selfCollide = sdf.FindElement("self_collide");
            if (selfCollide?.Value != null) SelfCollide = selfCollide.Value.BoolValue;

            var allowAutoDisable = sdf.FindElement("allow_auto_disable");
            if (allowAutoDisable?.Value != null) AllowAutoDisable = allowAutoDisable.Value.BoolValue;

            var enableWind = sdf.FindElement("enable_wind");
            if (enableWind?.Value != null) EnableWind = enableWind.Value.BoolValue;

            // Load links
            var linkElem = sdf.FindElement("link");
            while (linkElem != null)
            {
                var link = new Link();
                errors.AddRange(link.Load(linkElem));
                Links.Add(link);
                linkElem = linkElem.GetNextElement("link");
            }

            // Load joints
            var jointElem = sdf.FindElement("joint");
            while (jointElem != null)
            {
                var joint = new Joint();
                errors.AddRange(joint.Load(jointElem));
                Joints.Add(joint);
                jointElem = jointElem.GetNextElement("joint");
            }

            // Load frames
            var frameElem = sdf.FindElement("frame");
            while (frameElem != null)
            {
                var frame = new Frame();
                errors.AddRange(frame.Load(frameElem));
                Frames.Add(frame);
                frameElem = frameElem.GetNextElement("frame");
            }

            // Load nested models
            var modelElem = sdf.FindElement("model");
            while (modelElem != null)
            {
                var model = new Model();
                errors.AddRange(model.Load(modelElem));
                Models.Add(model);
                modelElem = modelElem.GetNextElement("model");
            }

            // Load plugins
            var pluginElem = sdf.FindElement("plugin");
            while (pluginElem != null)
            {
                var plugin = new Plugin();
                errors.AddRange(plugin.Load(pluginElem));
                Plugins.Add(plugin);
                pluginElem = pluginElem.GetNextElement("plugin");
            }

            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "model" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);

            if (!string.IsNullOrEmpty(CanonicalLinkName))
            {
                elem.AddAttribute("canonical_link", "string", "", false);
                elem.GetAttribute("canonical_link")!.SetFromString(CanonicalLinkName);
            }

            if (RawPose != Pose3d.Zero)
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

            if (Static)
            {
                var staticChild = new Element { Name = "static" };
                staticChild.AddValue("bool", "false", false);
                staticChild.Set("true");
                elem.InsertElement(staticChild);
            }

            foreach (var link in Links)
                elem.InsertElement(link.ToElement());
            foreach (var joint in Joints)
                elem.InsertElement(joint.ToElement());
            foreach (var frame in Frames)
                elem.InsertElement(frame.ToElement());
            foreach (var model in Models)
                elem.InsertElement(model.ToElement());
            foreach (var plugin in Plugins)
                elem.InsertElement(plugin.ToElement());

            return elem;
        }
    }
}
