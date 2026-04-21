// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - World.hh

#nullable enable

using System.Collections.Generic;
using System.Linq;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>
    /// A world is a container for models, actors, lights, and other objects.
    /// It also holds environment settings (gravity, atmosphere, scene, etc.).
    /// </summary>
    public class World : SdfElement
    {
        /// <summary>Name of the world.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Audio device URI.</summary>
        public string AudioDevice { get; set; } = "default";

        /// <summary>Wind linear velocity.</summary>
        public Vector3d WindLinearVelocity { get; set; } = Vector3d.Zero;

        /// <summary>Gravity vector (m/s^2).</summary>
        public Vector3d Gravity { get; set; } = new(0, 0, -9.8);

        /// <summary>Magnetic field vector (Tesla).</summary>
        public Vector3d MagneticField { get; set; } = new(5.5645e-6, 22.8758e-6, -42.3884e-6);

        /// <summary>Spherical coordinates.</summary>
        public SphericalCoordinates? SphericalCoordinatesInfo { get; set; }

        /// <summary>Atmosphere settings.</summary>
        public Atmosphere? AtmosphereInfo { get; set; }

        /// <summary>Scene settings.</summary>
        public Scene? SceneInfo { get; set; }

        /// <summary>GUI settings.</summary>
        public Gui? GuiInfo { get; set; }

        /// <summary>Models in this world.</summary>
        public List<Model> Models { get; } = new();

        /// <summary>Actors in this world.</summary>
        public List<Actor> Actors { get; } = new();

        /// <summary>Lights in this world.</summary>
        public List<Light> Lights { get; } = new();

        /// <summary>Frames in this world.</summary>
        public List<Frame> Frames { get; } = new();

        /// <summary>Joints in this world.</summary>
        public List<Joint> Joints { get; } = new();

        /// <summary>Physics profiles.</summary>
        public List<Physics> PhysicsProfiles { get; } = new();

        /// <summary>Plugins.</summary>
        public List<Plugin> Plugins { get; } = new();

        /// <summary>Include elements (before resolution).</summary>
        public List<Include> Includes { get; } = new();

        /// <summary>Road elements.</summary>
        public List<Road> Roads { get; } = new();

        /// <summary>Population elements.</summary>
        public List<Population> Populations { get; } = new();

        /// <summary>State snapshots.</summary>
        public List<State> States { get; } = new();

        // ---- Model accessors ----
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

        // ---- Actor accessors ----
        public int ActorCount => Actors.Count;
        public Actor? ActorByIndex(int index) =>
            index >= 0 && index < Actors.Count ? Actors[index] : null;
        public bool ActorNameExists(string name) =>
            Actors.Any(a => a.Name == name);

        // ---- Light accessors ----
        public int LightCount => Lights.Count;
        public Light? LightByIndex(int index) =>
            index >= 0 && index < Lights.Count ? Lights[index] : null;
        public bool LightNameExists(string name) =>
            Lights.Any(l => l.Name == name);

        // ---- Frame accessors ----
        public int FrameCount => Frames.Count;
        public Frame? FrameByIndex(int index) =>
            index >= 0 && index < Frames.Count ? Frames[index] : null;
        public bool FrameNameExists(string name) =>
            Frames.Any(f => f.Name == name);

        // ---- Joint accessors ----
        public int JointCount => Joints.Count;
        public Joint? JointByIndex(int index) =>
            index >= 0 && index < Joints.Count ? Joints[index] : null;
        public bool JointNameExists(string name) =>
            Joints.Any(j => j.Name == name);

        // ---- Physics accessors ----
        public int PhysicsCount => PhysicsProfiles.Count;
        public Physics? PhysicsByIndex(int index) =>
            index >= 0 && index < PhysicsProfiles.Count ? PhysicsProfiles[index] : null;
        public Physics? PhysicsDefault =>
            PhysicsProfiles.FirstOrDefault(p => p.IsDefault);
        public bool PhysicsNameExists(string name) =>
            PhysicsProfiles.Any(p => p.Name == name);

        // ---- Plugin accessors ----
        public void ClearPlugins() => Plugins.Clear();
        public void AddPlugin(Plugin plugin) => Plugins.Add(plugin);

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf) => Load(sdf, null);

        /// <summary>Load from an SDF element with parser configuration for include resolution.</summary>
        public List<SdfError> Load(Element sdf, ParserConfig? config)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            // Gravity
            var gravity = sdf.FindElement("gravity");
            if (gravity?.Value != null) Gravity = gravity.Value.Vector3dValue;

            // Magnetic field
            var magField = sdf.FindElement("magnetic_field");
            if (magField?.Value != null) MagneticField = magField.Value.Vector3dValue;

            // Wind
            if (sdf.HasElement("wind"))
            {
                var wind = sdf.FindElement("wind")!;
                var linearVel = wind.FindElement("linear_velocity");
                if (linearVel?.Value != null) WindLinearVelocity = linearVel.Value.Vector3dValue;
            }

            // Audio device
            var audioDevice = sdf.FindElement("audio");
            if (audioDevice != null)
            {
                var device = audioDevice.FindElement("device");
                if (device?.Value != null) AudioDevice = device.Value.GetAsString();
            }

            // Atmosphere
            if (sdf.HasElement("atmosphere"))
            {
                AtmosphereInfo = new Atmosphere();
                errors.AddRange(AtmosphereInfo.Load(sdf.FindElement("atmosphere")!));
            }

            // Scene
            if (sdf.HasElement("scene"))
            {
                SceneInfo = new Scene();
                errors.AddRange(SceneInfo.Load(sdf.FindElement("scene")!));
            }

            // GUI
            if (sdf.HasElement("gui"))
            {
                GuiInfo = new Gui();
                errors.AddRange(GuiInfo.Load(sdf.FindElement("gui")!));
            }

            // Spherical coordinates
            if (sdf.HasElement("spherical_coordinates"))
            {
                var scElem = sdf.FindElement("spherical_coordinates")!;
                SphericalCoordinatesInfo = new SphericalCoordinates();
                var surfModel = scElem.FindElement("surface_model");
                if (surfModel?.Value != null)
                {
                    SphericalCoordinatesInfo.Surface = surfModel.Value.GetAsString().ToUpperInvariant() switch
                    {
                        "EARTH_WGS84" => SphericalCoordinates.SurfaceType.EarthWgs84,
                        _ => SphericalCoordinates.SurfaceType.EarthWgs84,
                    };
                }
                var worldFrame = scElem.FindElement("world_frame_orientation");
                if (worldFrame?.Value != null) SphericalCoordinatesInfo.WorldFrameOrientation = worldFrame.Value.GetAsString();
                var lat = scElem.FindElement("latitude_deg");
                if (lat?.Value != null) SphericalCoordinatesInfo.LatitudeDeg = lat.Value.DoubleValue;
                var lon = scElem.FindElement("longitude_deg");
                if (lon?.Value != null) SphericalCoordinatesInfo.LongitudeDeg = lon.Value.DoubleValue;
                var elev = scElem.FindElement("elevation");
                if (elev?.Value != null) SphericalCoordinatesInfo.ElevationM = elev.Value.DoubleValue;
                var heading = scElem.FindElement("heading_deg");
                if (heading?.Value != null) SphericalCoordinatesInfo.HeadingDeg = heading.Value.DoubleValue;
            }

            // Physics profiles
            var physicsElem = sdf.FindElement("physics");
            while (physicsElem != null)
            {
                var physics = new Physics();
                errors.AddRange(physics.Load(physicsElem));
                PhysicsProfiles.Add(physics);
                physicsElem = physicsElem.GetNextElement("physics");
            }

            // Models
            var modelElem = sdf.FindElement("model");
            while (modelElem != null)
            {
                var model = new Model();
                errors.AddRange(model.Load(modelElem));
                Models.Add(model);
                modelElem = modelElem.GetNextElement("model");
            }

            // Actors
            var actorElem = sdf.FindElement("actor");
            while (actorElem != null)
            {
                var actor = new Actor();
                errors.AddRange(actor.Load(actorElem));
                Actors.Add(actor);
                actorElem = actorElem.GetNextElement("actor");
            }

            // Lights
            var lightElem = sdf.FindElement("light");
            while (lightElem != null)
            {
                var light = new Light();
                errors.AddRange(light.Load(lightElem));
                Lights.Add(light);
                lightElem = lightElem.GetNextElement("light");
            }

            // Frames
            var frameElem = sdf.FindElement("frame");
            while (frameElem != null)
            {
                var frame = new Frame();
                errors.AddRange(frame.Load(frameElem));
                Frames.Add(frame);
                frameElem = frameElem.GetNextElement("frame");
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

            // Roads
            var roadElem = sdf.FindElement("road");
            while (roadElem != null)
            {
                var road = new Road();
                errors.AddRange(road.Load(roadElem));
                Roads.Add(road);
                roadElem = roadElem.GetNextElement("road");
            }

            // Populations
            var popElem = sdf.FindElement("population");
            while (popElem != null)
            {
                var pop = new Population();
                errors.AddRange(pop.Load(popElem));
                Populations.Add(pop);
                popElem = popElem.GetNextElement("population");
            }

            // States
            var stateElem = sdf.FindElement("state");
            while (stateElem != null)
            {
                var state = new State();
                errors.AddRange(state.Load(stateElem));
                States.Add(state);
                stateElem = stateElem.GetNextElement("state");
            }

            // Includes
            var includeElem = sdf.FindElement("include");
            while (includeElem != null)
            {
                var (inc, incErrors) = Include.Load(includeElem);
                errors.AddRange(incErrors);
                Includes.Add(inc);

                if (config != null)
                    errors.AddRange(inc.ResolveAndApply(this, config));

                includeElem = includeElem.GetNextElement("include");
            }

            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "world" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);

            var gravChild = new Element { Name = "gravity" };
            gravChild.AddValue("vector3", "0 0 -9.8", false);
            gravChild.Set(Gravity.ToString());
            elem.InsertElement(gravChild);

            if (AtmosphereInfo != null)
                elem.InsertElement(AtmosphereInfo.ToElement());
            if (SceneInfo != null)
                elem.InsertElement(SceneInfo.ToElement());
            if (GuiInfo != null)
                elem.InsertElement(GuiInfo.ToElement());

            foreach (var physics in PhysicsProfiles)
                elem.InsertElement(physics.ToElement());
            foreach (var model in Models)
                elem.InsertElement(model.ToElement());
            foreach (var actor in Actors)
                elem.InsertElement(actor.ToElement());
            foreach (var light in Lights)
                elem.InsertElement(light.ToElement());
            foreach (var frame in Frames)
                elem.InsertElement(frame.ToElement());
            foreach (var joint in Joints)
                elem.InsertElement(joint.ToElement());
            foreach (var plugin in Plugins)
                elem.InsertElement(plugin.ToElement());

            return elem;
        }
    }
}
