// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Atmosphere.hh, Scene.hh, Gui.hh, Physics.hh, Actor.hh

#nullable enable

using System.Collections.Generic;
using System.Linq;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>Atmosphere model settings.</summary>
    public class Atmosphere : SdfElement
    {
        public AtmosphereType Type { get; set; } = AtmosphereType.Adiabatic;
        public Temperature Temperature { get; set; } = new(288.15);
        public double TemperatureGradient { get; set; } = -0.0065;
        public double Pressure { get; set; } = 101325.0;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var typeAttr = sdf.GetAttribute("type");
            if (typeAttr != null)
            {
                Type = typeAttr.GetAsString().ToLowerInvariant() switch
                {
                    "adiabatic" => AtmosphereType.Adiabatic,
                    _ => AtmosphereType.Adiabatic,
                };
            }

            var temp = sdf.FindElement("temperature");
            if (temp?.Value != null)
                Temperature = new Temperature(temp.Value.DoubleValue);
            var gradient = sdf.FindElement("temperature_gradient");
            if (gradient?.Value != null)
                TemperatureGradient = gradient.Value.DoubleValue;
            var pressure = sdf.FindElement("pressure");
            if (pressure?.Value != null)
                Pressure = pressure.Value.DoubleValue;
            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "atmosphere" };
            elem.AddAttribute("type", "string", "adiabatic", true);
            return elem;
        }
    }

    /// <summary>Sky settings for a scene (sun direction, clouds, etc.).</summary>
    public class Sky : SdfElement
    {
        public double Time { get; set; } = 10.0;
        public double Sunrise { get; set; } = 6.0;
        public double Sunset { get; set; } = 20.0;
        public double CloudSpeed { get; set; } = 0.6;
        public Angle CloudDirection { get; set; } = Angle.Zero;
        public double CloudHumidity { get; set; } = 0.5;
        public double CloudMeanSize { get; set; } = 0.5;
        public Color CloudAmbient { get; set; } = new(0.8f, 0.8f, 0.8f, 1f);

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var time = sdf.FindElement("time");
            if (time?.Value != null) Time = time.Value.DoubleValue;
            var sunrise = sdf.FindElement("sunrise");
            if (sunrise?.Value != null) Sunrise = sunrise.Value.DoubleValue;
            var sunset = sdf.FindElement("sunset");
            if (sunset?.Value != null) Sunset = sunset.Value.DoubleValue;

            var clouds = sdf.FindElement("clouds");
            if (clouds != null)
            {
                var speed = clouds.FindElement("speed");
                if (speed?.Value != null) CloudSpeed = speed.Value.DoubleValue;
                var dir = clouds.FindElement("direction");
                if (dir?.Value != null) CloudDirection = new Angle(dir.Value.DoubleValue);
                var humidity = clouds.FindElement("humidity");
                if (humidity?.Value != null) CloudHumidity = humidity.Value.DoubleValue;
                var meanSize = clouds.FindElement("mean_size");
                if (meanSize?.Value != null) CloudMeanSize = meanSize.Value.DoubleValue;
                var ambient = clouds.FindElement("ambient");
                if (ambient?.Value != null) CloudAmbient = ambient.Value.ColorValue;
            }

            return errors;
        }
    }

    /// <summary>Scene properties (ambient light, background color, etc.).</summary>
    public class Scene : SdfElement
    {
        public Color Ambient { get; set; } = new(0.4f, 0.4f, 0.4f, 1f);
        public Color Background { get; set; } = new(0.7f, 0.7f, 0.7f, 1f);
        public bool Grid { get; set; } = true;
        public bool OriginVisual { get; set; } = true;
        public bool Shadows { get; set; } = true;
        public Sky? SkySettings { get; set; }
        public Fog? FogSettings { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var ambient = sdf.FindElement("ambient");
            if (ambient?.Value != null) Ambient = ambient.Value.ColorValue;
            var background = sdf.FindElement("background");
            if (background?.Value != null) Background = background.Value.ColorValue;
            var grid = sdf.FindElement("grid");
            if (grid?.Value != null) Grid = grid.Value.BoolValue;
            var shadows = sdf.FindElement("shadows");
            if (shadows?.Value != null) Shadows = shadows.Value.BoolValue;
            var originVisual = sdf.FindElement("origin_visual");
            if (originVisual?.Value != null) OriginVisual = originVisual.Value.BoolValue;

            if (sdf.HasElement("sky"))
            {
                SkySettings = new Sky();
                errors.AddRange(SkySettings.Load(sdf.FindElement("sky")!));
            }

            if (sdf.HasElement("fog"))
            {
                FogSettings = new Fog();
                errors.AddRange(FogSettings.Load(sdf.FindElement("fog")!));
            }

            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "scene" };
            return elem;
        }
    }

    /// <summary>Fog properties.</summary>
    public class Fog : SdfElement
    {
        public Color FogColor { get; set; } = new(1f, 1f, 1f, 1f);
        public string FogType { get; set; } = "none";
        public double Start { get; set; } = 1.0;
        public double End { get; set; } = 100.0;
        public double Density { get; set; } = 1.0;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var color = sdf.FindElement("color");
            if (color?.Value != null) FogColor = color.Value.ColorValue;
            var type = sdf.FindElement("type");
            if (type?.Value != null) FogType = type.Value.GetAsString();
            var start = sdf.FindElement("start");
            if (start?.Value != null) Start = start.Value.DoubleValue;
            var end = sdf.FindElement("end");
            if (end?.Value != null) End = end.Value.DoubleValue;
            var density = sdf.FindElement("density");
            if (density?.Value != null) Density = density.Value.DoubleValue;

            return errors;
        }
    }

    /// <summary>GUI settings (fullscreen, plugins).</summary>
    public class Gui : SdfElement
    {
        public bool Fullscreen { get; set; }
        public List<Plugin> Plugins { get; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var fullscreenAttr = sdf.GetAttribute("fullscreen");
            if (fullscreenAttr != null) Fullscreen = fullscreenAttr.BoolValue;

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

        public int PluginCount => Plugins.Count;
        public Plugin? PluginByIndex(int index) =>
            index >= 0 && index < Plugins.Count ? Plugins[index] : null;
        public void ClearPlugins() => Plugins.Clear();
        public void AddPlugin(Plugin plugin) => Plugins.Add(plugin);

        public Element ToElement()
        {
            var elem = new Element { Name = "gui" };
            return elem;
        }
    }

    /// <summary>Physics profile settings.</summary>
    public class Physics : SdfElement
    {
        public string Name { get; set; } = "default_physics";
        public bool IsDefault { get; set; } = true;
        public string EngineType { get; set; } = "ode";
        public double MaxStepSize { get; set; } = 0.001;
        public double RealTimeFactor { get; set; } = 1.0;
        public int MaxContacts { get; set; } = 20;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var defaultAttr = sdf.GetAttribute("default");
            if (defaultAttr != null) IsDefault = defaultAttr.BoolValue;

            var typeAttr = sdf.GetAttribute("type");
            if (typeAttr != null) EngineType = typeAttr.GetAsString();

            var maxStep = sdf.FindElement("max_step_size");
            if (maxStep?.Value != null) MaxStepSize = maxStep.Value.DoubleValue;

            var rtf = sdf.FindElement("real_time_factor");
            if (rtf?.Value != null) RealTimeFactor = rtf.Value.DoubleValue;

            var maxContacts = sdf.FindElement("max_contacts");
            if (maxContacts?.Value != null) MaxContacts = maxContacts.Value.IntValue;

            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "physics" };
            elem.AddAttribute("name", "string", "default_physics", false);
            elem.GetAttribute("name")!.SetFromString(Name);
            return elem;
        }
    }

    // ---- Actor classes ----

    /// <summary>An animation for an actor.</summary>
    public class Animation : SdfElement
    {
        public string Name { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public double Scale { get; set; } = 1.0;
        public bool InterpolateX { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();
            var filename = sdf.FindElement("filename");
            if (filename?.Value != null) Filename = filename.Value.GetAsString();
            var scale = sdf.FindElement("scale");
            if (scale?.Value != null) Scale = scale.Value.DoubleValue;
            var interp = sdf.FindElement("interpolate_x");
            if (interp?.Value != null) InterpolateX = interp.Value.BoolValue;
            return errors;
        }
    }

    /// <summary>A waypoint for an actor trajectory.</summary>
    public class Waypoint : SdfElement
    {
        public double Time { get; set; }
        public Pose3d Pose { get; set; } = Pose3d.Zero;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var time = sdf.FindElement("time");
            if (time?.Value != null) Time = time.Value.DoubleValue;
            var pose = sdf.FindElement("pose");
            if (pose?.Value != null) Pose = pose.Value.Pose3dValue;
            return errors;
        }
    }

    /// <summary>A trajectory for an actor's animation.</summary>
    public class Trajectory : SdfElement
    {
        public ulong Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public double Tension { get; set; }
        public List<Waypoint> Waypoints { get; } = new();

        public int WaypointCount => Waypoints.Count;
        public Waypoint? WaypointByIndex(int index) =>
            index >= 0 && index < Waypoints.Count ? Waypoints[index] : null;
        public void AddWaypoint(Waypoint wp) => Waypoints.Add(wp);

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var idAttr = sdf.GetAttribute("id");
            if (idAttr != null && ulong.TryParse(idAttr.GetAsString(), out var id))
                Id = id;

            var typeAttr = sdf.GetAttribute("type");
            if (typeAttr != null) Type = typeAttr.GetAsString();

            var tensionElem = sdf.FindElement("tension");
            if (tensionElem?.Value != null) Tension = tensionElem.Value.DoubleValue;

            var wpElem = sdf.FindElement("waypoint");
            while (wpElem != null)
            {
                var wp = new Waypoint();
                errors.AddRange(wp.Load(wpElem));
                Waypoints.Add(wp);
                wpElem = wpElem.GetNextElement("waypoint");
            }

            return errors;
        }
    }

    /// <summary>An animated actor in the world.</summary>
    public class Actor : SdfNamedPosedElement
    {
        public string FilePath { get; set; } = string.Empty;
        public string SkinFilename { get; set; } = string.Empty;
        public double SkinScale { get; set; } = 1.0;
        public List<Animation> Animations { get; } = new();
        public bool ScriptLoop { get; set; } = true;
        public double ScriptDelayStart { get; set; }
        public bool ScriptAutoStart { get; set; } = true;
        public List<Trajectory> Trajectories { get; } = new();
        public List<Link> Links { get; } = new();
        public List<Joint> Joints { get; } = new();
        public List<Plugin> Plugins { get; } = new();

        public int AnimationCount => Animations.Count;
        public Animation? AnimationByIndex(int index) =>
            index >= 0 && index < Animations.Count ? Animations[index] : null;
        public bool AnimationNameExists(string name) =>
            Animations.Any(a => a.Name == name);
        public void AddAnimation(Animation anim) => Animations.Add(anim);

        public int TrajectoryCount => Trajectories.Count;
        public Trajectory? TrajectoryByIndex(int index) =>
            index >= 0 && index < Trajectories.Count ? Trajectories[index] : null;
        public bool TrajectoryIdExists(ulong id) =>
            Trajectories.Any(t => t.Id == id);
        public void AddTrajectory(Trajectory traj) => Trajectories.Add(traj);

        public int LinkCount => Links.Count;
        public Link? LinkByIndex(int index) =>
            index >= 0 && index < Links.Count ? Links[index] : null;
        public bool LinkNameExists(string name) =>
            Links.Any(l => l.Name == name);
        public bool AddLink(Link link) { Links.Add(link); return true; }
        public void ClearLinks() => Links.Clear();

        public int JointCount => Joints.Count;
        public Joint? JointByIndex(int index) =>
            index >= 0 && index < Joints.Count ? Joints[index] : null;
        public bool JointNameExists(string name) =>
            Joints.Any(j => j.Name == name);
        public bool AddJoint(Joint joint) { Joints.Add(joint); return true; }
        public void ClearJoints() => Joints.Clear();

        public void ClearPlugins() => Plugins.Clear();
        public void AddPlugin(Plugin plugin) => Plugins.Add(plugin);

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var poseElem = sdf.FindElement("pose");
            if (poseElem?.Value != null)
            {
                RawPose = poseElem.Value.Pose3dValue;
                var relTo = poseElem.GetAttribute("relative_to");
                if (relTo != null) PoseRelativeTo = relTo.GetAsString();
            }

            if (sdf.HasElement("skin"))
            {
                var skin = sdf.FindElement("skin")!;
                var filename = skin.FindElement("filename");
                if (filename?.Value != null) SkinFilename = filename.Value.GetAsString();
                var scale = skin.FindElement("scale");
                if (scale?.Value != null) SkinScale = scale.Value.DoubleValue;
            }

            // Animations
            var animElem = sdf.FindElement("animation");
            while (animElem != null)
            {
                var anim = new Animation();
                errors.AddRange(anim.Load(animElem));
                Animations.Add(anim);
                animElem = animElem.GetNextElement("animation");
            }

            // Script
            if (sdf.HasElement("script"))
            {
                var script = sdf.FindElement("script")!;
                var loop = script.FindElement("loop");
                if (loop?.Value != null) ScriptLoop = loop.Value.BoolValue;
                var delay = script.FindElement("delay_start");
                if (delay?.Value != null) ScriptDelayStart = delay.Value.DoubleValue;
                var auto = script.FindElement("auto_start");
                if (auto?.Value != null) ScriptAutoStart = auto.Value.BoolValue;

                var trajElem = script.FindElement("trajectory");
                while (trajElem != null)
                {
                    var traj = new Trajectory();
                    errors.AddRange(traj.Load(trajElem));
                    Trajectories.Add(traj);
                    trajElem = trajElem.GetNextElement("trajectory");
                }
            }

            // Links
            var linkElem = sdf.FindElement("link");
            while (linkElem != null)
            {
                var link = new Link();
                errors.AddRange(link.Load(linkElem));
                Links.Add(link);
                linkElem = linkElem.GetNextElement("link");
            }

            // Joints
            var jointElem = sdf.FindElement("joint");
            while (jointElem != null)
            {
                var joint = new Joint();
                errors.AddRange(joint.Load(jointElem));
                Joints.Add(joint);
                jointElem = jointElem.GetNextElement("joint");
            }

            // Plugins
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

        public Element ToElement()
        {
            var elem = new Element { Name = "actor" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);
            return elem;
        }
    }
}
