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
        public double RealTimeUpdateRate { get; set; } = 1000.0;
        public int MaxContacts { get; set; } = 20;

        /// <summary>ODE engine parameters.</summary>
        public PhysicsOde? Ode { get; set; }

        /// <summary>Bullet engine parameters.</summary>
        public PhysicsBullet? BulletEngine { get; set; }

        /// <summary>Simbody engine parameters.</summary>
        public PhysicsSimbody? SimbodyEngine { get; set; }

        /// <summary>DART engine parameters.</summary>
        public PhysicsDart? DartEngine { get; set; }

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

            var rtUpdateRate = sdf.FindElement("real_time_update_rate");
            if (rtUpdateRate?.Value != null) RealTimeUpdateRate = rtUpdateRate.Value.DoubleValue;

            var maxContacts = sdf.FindElement("max_contacts");
            if (maxContacts?.Value != null) MaxContacts = maxContacts.Value.IntValue;

            // Engine-specific params
            if (sdf.HasElement("ode"))
            {
                Ode = new PhysicsOde();
                errors.AddRange(Ode.Load(sdf.FindElement("ode")!));
            }
            if (sdf.HasElement("bullet"))
            {
                BulletEngine = new PhysicsBullet();
                errors.AddRange(BulletEngine.Load(sdf.FindElement("bullet")!));
            }
            if (sdf.HasElement("simbody"))
            {
                SimbodyEngine = new PhysicsSimbody();
                errors.AddRange(SimbodyEngine.Load(sdf.FindElement("simbody")!));
            }
            if (sdf.HasElement("dart"))
            {
                DartEngine = new PhysicsDart();
                errors.AddRange(DartEngine.Load(sdf.FindElement("dart")!));
            }

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

    // ---- Road ----

    /// <summary>A road in the world defined by a width and a series of points.</summary>
    public class Road : SdfElement
    {
        /// <summary>Name of the road.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Width of the road in meters.</summary>
        public double Width { get; set; } = 1.0;

        /// <summary>Ordered points defining the road center.</summary>
        public List<Vector3d> Points { get; } = new();

        /// <summary>Material of the road surface.</summary>
        public Material? RoadMaterial { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var widthElem = sdf.FindElement("width");
            if (widthElem?.Value != null) Width = widthElem.Value.DoubleValue;

            var pointElem = sdf.FindElement("point");
            while (pointElem != null)
            {
                if (pointElem.Value != null)
                    Points.Add(pointElem.Value.Vector3dValue);
                pointElem = pointElem.GetNextElement("point");
            }

            if (sdf.HasElement("material"))
            {
                RoadMaterial = new Material();
                errors.AddRange(RoadMaterial.Load(sdf.FindElement("material")!));
            }

            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "road" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);
            return elem;
        }
    }

    // ---- Population ----

    /// <summary>A population element that distributes multiple copies of a model.</summary>
    public class Population : SdfElement
    {
        /// <summary>Name of the population.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Number of models to distribute.</summary>
        public int ModelCount { get; set; } = 1;

        /// <summary>Distribution type (random, uniform, grid, linear-x, linear-y, linear-z).</summary>
        public string Distribution { get; set; } = "random";

        /// <summary>URI of the model to populate.</summary>
        public string ModelUri { get; set; } = string.Empty;

        /// <summary>Model name.</summary>
        public string ModelName { get; set; } = string.Empty;

        /// <summary>Pose of the population frame.</summary>
        public Pose3d RawPose { get; set; } = Pose3d.Zero;

        /// <summary>Relative-to frame for the pose.</summary>
        public string PoseRelativeTo { get; set; } = string.Empty;

        /// <summary>Box region size for distribution.</summary>
        public Vector3d BoxSize { get; set; } = Vector3d.One;

        /// <summary>Cylinder region radius for distribution.</summary>
        public double CylinderRadius { get; set; } = 1.0;

        /// <summary>Cylinder region length for distribution.</summary>
        public double CylinderLength { get; set; } = 1.0;

        /// <summary>Grid row count (for grid distribution).</summary>
        public int GridRows { get; set; } = 1;

        /// <summary>Grid column count (for grid distribution).</summary>
        public int GridCols { get; set; } = 1;

        /// <summary>Grid step size.</summary>
        public Vector3d GridStep { get; set; } = Vector3d.One;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var modelCount = sdf.FindElement("model_count");
            if (modelCount?.Value != null) ModelCount = modelCount.Value.IntValue;

            var distribution = sdf.FindElement("distribution");
            if (distribution != null)
            {
                var type = distribution.FindElement("type");
                if (type?.Value != null) Distribution = type.Value.GetAsString();
            }

            var poseElem = sdf.FindElement("pose");
            if (poseElem?.Value != null)
            {
                RawPose = poseElem.Value.Pose3dValue;
                var relTo = poseElem.GetAttribute("relative_to");
                if (relTo != null) PoseRelativeTo = relTo.GetAsString();
            }

            // Model reference
            if (sdf.HasElement("model"))
            {
                var modelElem = sdf.FindElement("model")!;
                var modelNameAttr = modelElem.GetAttribute("name");
                if (modelNameAttr != null) ModelName = modelNameAttr.GetAsString();
            }

            // Box region
            var box = sdf.FindElement("box");
            if (box != null)
            {
                var size = box.FindElement("size");
                if (size?.Value != null) BoxSize = size.Value.Vector3dValue;
            }

            // Cylinder region
            var cyl = sdf.FindElement("cylinder");
            if (cyl != null)
            {
                var radius = cyl.FindElement("radius");
                if (radius?.Value != null) CylinderRadius = radius.Value.DoubleValue;
                var length = cyl.FindElement("length");
                if (length?.Value != null) CylinderLength = length.Value.DoubleValue;
            }

            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "population" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);
            return elem;
        }
    }

    // ---- Gripper ----

    /// <summary>A gripper element within a model.</summary>
    public class Gripper : SdfElement
    {
        /// <summary>Name of the gripper.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Minimum contact count for successful grasp.</summary>
        public int GraspCheckMinContactCount { get; set; } = 2;

        /// <summary>Number of steps to wait before checking grasp.</summary>
        public int GraspCheckAttachSteps { get; set; } = 20;

        /// <summary>Gripper link names.</summary>
        public List<string> GripperLinks { get; } = new();

        /// <summary>Palm link name.</summary>
        public string PalmLink { get; set; } = string.Empty;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            // Grasp check
            var graspCheck = sdf.FindElement("grasp_check");
            if (graspCheck != null)
            {
                var minContacts = graspCheck.FindElement("min_contact_count");
                if (minContacts?.Value != null) GraspCheckMinContactCount = minContacts.Value.IntValue;

                var attachSteps = graspCheck.FindElement("attach_steps");
                if (attachSteps?.Value != null) GraspCheckAttachSteps = attachSteps.Value.IntValue;
            }

            // Gripper links
            var gripperLinkElem = sdf.FindElement("gripper_link");
            while (gripperLinkElem != null)
            {
                if (gripperLinkElem.Value != null)
                    GripperLinks.Add(gripperLinkElem.Value.GetAsString());
                gripperLinkElem = gripperLinkElem.GetNextElement("gripper_link");
            }

            // Palm link
            var palmLink = sdf.FindElement("palm_link");
            if (palmLink?.Value != null) PalmLink = palmLink.Value.GetAsString();

            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "gripper" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);
            return elem;
        }
    }

    // ---- Physics engine-specific parameters ----

    /// <summary>ODE solver parameters for a physics profile.</summary>
    public class PhysicsOde : SdfElement
    {
        // Solver
        public string SolverType { get; set; } = "quick";
        public int SolverIters { get; set; } = 50;
        public double SolverSor { get; set; } = 1.3;
        public double SolverPreconIters { get; set; }
        public double SolverFrictionModel { get; set; }
        public int SolverIslandThreads { get; set; }

        // Constraints
        public double ConstraintsCfm { get; set; }
        public double ConstraintsErp { get; set; } = 0.2;
        public double ConstraintsContactMaxCorrectingVel { get; set; } = 100.0;
        public double ConstraintsContactSurfaceLayer { get; set; } = 0.001;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var solver = sdf.FindElement("solver");
            if (solver != null)
            {
                var type = solver.FindElement("type");
                if (type?.Value != null) SolverType = type.Value.GetAsString();
                var iters = solver.FindElement("iters");
                if (iters?.Value != null) SolverIters = iters.Value.IntValue;
                var sor = solver.FindElement("sor");
                if (sor?.Value != null) SolverSor = sor.Value.DoubleValue;
                var preconIters = solver.FindElement("precon_iters");
                if (preconIters?.Value != null) SolverPreconIters = preconIters.Value.DoubleValue;
            }

            var constraints = sdf.FindElement("constraints");
            if (constraints != null)
            {
                var cfm = constraints.FindElement("cfm");
                if (cfm?.Value != null) ConstraintsCfm = cfm.Value.DoubleValue;
                var erp = constraints.FindElement("erp");
                if (erp?.Value != null) ConstraintsErp = erp.Value.DoubleValue;
                var maxVel = constraints.FindElement("contact_max_correcting_vel");
                if (maxVel?.Value != null) ConstraintsContactMaxCorrectingVel = maxVel.Value.DoubleValue;
                var surfLayer = constraints.FindElement("contact_surface_layer");
                if (surfLayer?.Value != null) ConstraintsContactSurfaceLayer = surfLayer.Value.DoubleValue;
            }

            return errors;
        }
    }

    /// <summary>Bullet solver parameters for a physics profile.</summary>
    public class PhysicsBullet : SdfElement
    {
        // Solver
        public string SolverType { get; set; } = "sequential_impulse";
        public int SolverIters { get; set; } = 50;
        public double SolverSor { get; set; } = 1.3;
        public double SolverMinStepSize { get; set; } = 0.0001;

        // Constraints
        public double ConstraintsCfm { get; set; }
        public double ConstraintsErp { get; set; } = 0.2;
        public double ConstraintsContactSurfaceLayer { get; set; } = 0.001;
        public bool ConstraintsSplitImpulse { get; set; } = true;
        public double ConstraintsSplitImpulsePenetrationThreshold { get; set; } = -0.01;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var solver = sdf.FindElement("solver");
            if (solver != null)
            {
                var type = solver.FindElement("type");
                if (type?.Value != null) SolverType = type.Value.GetAsString();
                var iters = solver.FindElement("iters");
                if (iters?.Value != null) SolverIters = iters.Value.IntValue;
                var sor = solver.FindElement("sor");
                if (sor?.Value != null) SolverSor = sor.Value.DoubleValue;
                var minStep = solver.FindElement("min_step_size");
                if (minStep?.Value != null) SolverMinStepSize = minStep.Value.DoubleValue;
            }

            var constraints = sdf.FindElement("constraints");
            if (constraints != null)
            {
                var cfm = constraints.FindElement("cfm");
                if (cfm?.Value != null) ConstraintsCfm = cfm.Value.DoubleValue;
                var erp = constraints.FindElement("erp");
                if (erp?.Value != null) ConstraintsErp = erp.Value.DoubleValue;
                var surfLayer = constraints.FindElement("contact_surface_layer");
                if (surfLayer?.Value != null) ConstraintsContactSurfaceLayer = surfLayer.Value.DoubleValue;
                var splitImpulse = constraints.FindElement("split_impulse");
                if (splitImpulse?.Value != null) ConstraintsSplitImpulse = splitImpulse.Value.BoolValue;
                var splitThresh = constraints.FindElement("split_impulse_penetration_threshold");
                if (splitThresh?.Value != null) ConstraintsSplitImpulsePenetrationThreshold = splitThresh.Value.DoubleValue;
            }

            return errors;
        }
    }

    /// <summary>Simbody engine parameters for a physics profile.</summary>
    public class PhysicsSimbody : SdfElement
    {
        public double Accuracy { get; set; } = 0.001;
        public double MaxTransientVelocity { get; set; } = 0.01;

        // Contact parameters
        public double ContactStiffness { get; set; } = 1e8;
        public double ContactDissipation { get; set; } = 100;
        public double ContactPlasticCoefRestitution { get; set; } = 0.5;
        public double ContactPlasticImpactVelocity { get; set; } = 0.5;
        public double ContactStaticFriction { get; set; } = 0.9;
        public double ContactDynamicFriction { get; set; } = 0.9;
        public double ContactViscousFriction { get; set; }
        public double ContactOverrideImpactCaptureVelocity { get; set; } = 0.001;
        public double ContactOverrideStictionTransitionVelocity { get; set; } = 0.001;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var accuracy = sdf.FindElement("accuracy");
            if (accuracy?.Value != null) Accuracy = accuracy.Value.DoubleValue;
            var maxTransient = sdf.FindElement("max_transient_velocity");
            if (maxTransient?.Value != null) MaxTransientVelocity = maxTransient.Value.DoubleValue;

            var contact = sdf.FindElement("contact");
            if (contact != null)
            {
                var stiffness = contact.FindElement("stiffness");
                if (stiffness?.Value != null) ContactStiffness = stiffness.Value.DoubleValue;
                var dissipation = contact.FindElement("dissipation");
                if (dissipation?.Value != null) ContactDissipation = dissipation.Value.DoubleValue;
                var plasticCoef = contact.FindElement("plastic_coef_restitution");
                if (plasticCoef?.Value != null) ContactPlasticCoefRestitution = plasticCoef.Value.DoubleValue;
                var plasticVel = contact.FindElement("plastic_impact_velocity");
                if (plasticVel?.Value != null) ContactPlasticImpactVelocity = plasticVel.Value.DoubleValue;
                var staticFriction = contact.FindElement("static_friction");
                if (staticFriction?.Value != null) ContactStaticFriction = staticFriction.Value.DoubleValue;
                var dynamicFriction = contact.FindElement("dynamic_friction");
                if (dynamicFriction?.Value != null) ContactDynamicFriction = dynamicFriction.Value.DoubleValue;
                var viscousFriction = contact.FindElement("viscous_friction");
                if (viscousFriction?.Value != null) ContactViscousFriction = viscousFriction.Value.DoubleValue;
                var overrideCapture = contact.FindElement("override_impact_capture_velocity");
                if (overrideCapture?.Value != null) ContactOverrideImpactCaptureVelocity = overrideCapture.Value.DoubleValue;
                var overrideStiction = contact.FindElement("override_stiction_transition_velocity");
                if (overrideStiction?.Value != null) ContactOverrideStictionTransitionVelocity = overrideStiction.Value.DoubleValue;
            }

            return errors;
        }
    }

    /// <summary>DART solver parameters for a physics profile.</summary>
    public class PhysicsDart : SdfElement
    {
        public string Solver { get; set; } = "dantzig";
        public string CollisionDetector { get; set; } = "fcl";

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var solver = sdf.FindElement("solver");
            if (solver != null)
            {
                var type = solver.FindElement("solver_type");
                if (type?.Value != null) Solver = type.Value.GetAsString();
            }

            var collision = sdf.FindElement("collision_detector");
            if (collision?.Value != null) CollisionDetector = collision.Value.GetAsString();

            return errors;
        }
    }
}
