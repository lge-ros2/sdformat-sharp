// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Link.hh

#nullable enable

using System.Collections.Generic;
using System.Linq;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>
    /// A link contains the physical properties of a body within a model,
    /// including collision geometry, visual appearance, and sensor attachments.
    /// </summary>
    public class Link : SdfNamedPosedElement
    {
        /// <summary>Optional density (kg/m^3).</summary>
        public double? Density { get; set; }

        /// <summary>Inertial properties.</summary>
        public Inertial Inertial { get; set; } = new();

        /// <summary>Whether auto-inertia is enabled.</summary>
        public bool AutoInertia { get; set; }

        /// <summary>Whether auto-inertia result has been saved.</summary>
        public bool AutoInertiaSaved { get; set; }

        /// <summary>Auto-inertia parameters element.</summary>
        public Element? AutoInertiaParams { get; set; }

        /// <summary>Whether wind affects this link.</summary>
        public bool EnableWind { get; set; }

        /// <summary>Whether gravity affects this link.</summary>
        public bool EnableGravity { get; set; } = true;

        /// <summary>Whether the link is kinematic only.</summary>
        public bool Kinematic { get; set; }

        /// <summary>Whether the link can self-collide.</summary>
        public bool SelfCollide { get; set; }

        /// <summary>Whether the link must be a base link (6DOF).</summary>
        public bool MustBeBaseLink { get; set; }

        /// <summary>Linear velocity decay damping.</summary>
        public double VelocityDecayLinear { get; set; }

        /// <summary>Angular velocity decay damping.</summary>
        public double VelocityDecayAngular { get; set; }

        /// <summary>Visual elements.</summary>
        public List<Visual> Visuals { get; } = new();

        /// <summary>Collision elements.</summary>
        public List<Collision> Collisions { get; } = new();

        /// <summary>Sensors.</summary>
        public List<Sensor> Sensors { get; } = new();

        /// <summary>Lights.</summary>
        public List<Light> Lights { get; } = new();

        /// <summary>Particle emitters.</summary>
        public List<Element> ParticleEmitters { get; } = new();

        /// <summary>Projectors.</summary>
        public List<Element> Projectors { get; } = new();

        /// <summary>Battery name.</summary>
        public string BatteryName { get; set; } = string.Empty;
        /// <summary>Battery voltage.</summary>
        public double BatteryVoltage { get; set; }

        /// <summary>Whether an audio sink is present on this link.</summary>
        public bool HasAudioSink { get; set; }

        /// <summary>Audio sources attached to this link.</summary>
        public List<AudioSource> AudioSources { get; } = new();

        /// <summary>Fluid added mass matrix (6x6, stored as 21 unique upper-triangular values).</summary>
        public double[]? FluidAddedMass { get; set; }

        // ---- Visual accessors ----
        public int VisualCount => Visuals.Count;
        public Visual? VisualByIndex(int index) =>
            index >= 0 && index < Visuals.Count ? Visuals[index] : null;
        public Visual? VisualByName(string name) =>
            Visuals.FirstOrDefault(v => v.Name == name);
        public bool VisualNameExists(string name) =>
            Visuals.Any(v => v.Name == name);
        public bool AddVisual(Visual visual)
        {
            if (VisualNameExists(visual.Name)) return false;
            Visuals.Add(visual);
            return true;
        }
        public void ClearVisuals() => Visuals.Clear();

        // ---- Collision accessors ----
        public int CollisionCount => Collisions.Count;
        public Collision? CollisionByIndex(int index) =>
            index >= 0 && index < Collisions.Count ? Collisions[index] : null;
        public Collision? CollisionByName(string name) =>
            Collisions.FirstOrDefault(c => c.Name == name);
        public bool CollisionNameExists(string name) =>
            Collisions.Any(c => c.Name == name);
        public bool AddCollision(Collision collision)
        {
            if (CollisionNameExists(collision.Name)) return false;
            Collisions.Add(collision);
            return true;
        }
        public void ClearCollisions() => Collisions.Clear();

        // ---- Sensor accessors ----
        public int SensorCount => Sensors.Count;
        public Sensor? SensorByIndex(int index) =>
            index >= 0 && index < Sensors.Count ? Sensors[index] : null;
        public Sensor? SensorByName(string name) =>
            Sensors.FirstOrDefault(s => s.Name == name);
        public bool SensorNameExists(string name) =>
            Sensors.Any(s => s.Name == name);
        public bool AddSensor(Sensor sensor)
        {
            if (SensorNameExists(sensor.Name)) return false;
            Sensors.Add(sensor);
            return true;
        }
        public void ClearSensors() => Sensors.Clear();

        // ---- Light accessors ----
        public int LightCount => Lights.Count;
        public Light? LightByIndex(int index) =>
            index >= 0 && index < Lights.Count ? Lights[index] : null;
        public bool LightNameExists(string name) =>
            Lights.Any(l => l.Name == name);
        public bool AddLight(Light light)
        {
            if (LightNameExists(light.Name)) return false;
            Lights.Add(light);
            return true;
        }
        public void ClearLights() => Lights.Clear();

        /// <summary>Load from an SDF element.</summary>
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

            // Gravity
            var gravity = sdf.FindElement("gravity");
            if (gravity?.Value != null) EnableGravity = gravity.Value.BoolValue;

            // Wind
            var wind = sdf.FindElement("enable_wind");
            if (wind?.Value != null) EnableWind = wind.Value.BoolValue;

            // Kinematic
            var kinematic = sdf.FindElement("kinematic");
            if (kinematic?.Value != null) Kinematic = kinematic.Value.BoolValue;

            // Self-collide
            var selfCollide = sdf.FindElement("self_collide");
            if (selfCollide?.Value != null) SelfCollide = selfCollide.Value.BoolValue;

            // Must be base link
            var mustBeBase = sdf.FindElement("must_be_base_link");
            if (mustBeBase?.Value != null) MustBeBaseLink = mustBeBase.Value.BoolValue;

            // Velocity decay
            var velDecay = sdf.FindElement("velocity_decay");
            if (velDecay != null)
            {
                var linear = velDecay.FindElement("linear");
                if (linear?.Value != null) VelocityDecayLinear = linear.Value.DoubleValue;
                var angular = velDecay.FindElement("angular");
                if (angular?.Value != null) VelocityDecayAngular = angular.Value.DoubleValue;
            }

            // Inertial
            if (sdf.HasElement("inertial"))
            {
                var inertialElem = sdf.FindElement("inertial")!;

                // Auto inertia
                var autoAttr = inertialElem.GetAttribute("auto");
                if (autoAttr != null) AutoInertia = autoAttr.BoolValue;

                var densityElem = inertialElem.FindElement("density");
                if (densityElem?.Value != null) Density = densityElem.Value.DoubleValue;

                var autoParamsElem = inertialElem.FindElement("auto_inertia_params");
                if (autoParamsElem != null) AutoInertiaParams = autoParamsElem;

                var mass = inertialElem.FindElement("mass");
                if (mass?.Value != null) Inertial.Mass = mass.Value.DoubleValue;

                var inertia = inertialElem.FindElement("inertia");
                if (inertia != null)
                {
                    var ixx = inertia.FindElement("ixx");
                    if (ixx?.Value != null) Inertial.Ixx = ixx.Value.DoubleValue;
                    var iyy = inertia.FindElement("iyy");
                    if (iyy?.Value != null) Inertial.Iyy = iyy.Value.DoubleValue;
                    var izz = inertia.FindElement("izz");
                    if (izz?.Value != null) Inertial.Izz = izz.Value.DoubleValue;
                    var ixy = inertia.FindElement("ixy");
                    if (ixy?.Value != null) Inertial.Ixy = ixy.Value.DoubleValue;
                    var ixz = inertia.FindElement("ixz");
                    if (ixz?.Value != null) Inertial.Ixz = ixz.Value.DoubleValue;
                    var iyz = inertia.FindElement("iyz");
                    if (iyz?.Value != null) Inertial.Iyz = iyz.Value.DoubleValue;
                }

                var inertialPose = inertialElem.FindElement("pose");
                if (inertialPose?.Value != null)
                    Inertial.Pose = inertialPose.Value.Pose3dValue;

                // Fluid added mass (21 unique elements of 6x6 symmetric matrix)
                var fluidAddedMass = inertialElem.FindElement("fluid_added_mass");
                if (fluidAddedMass != null)
                {
                    FluidAddedMass = new double[21];
                    string[] names = {
                        "xx", "xy", "xz", "xp", "xq", "xr",
                        "yy", "yz", "yp", "yq", "yr",
                        "zz", "zp", "zq", "zr",
                        "pp", "pq", "pr",
                        "qq", "qr",
                        "rr"
                    };
                    for (int i = 0; i < names.Length; i++)
                    {
                        var fam = fluidAddedMass.FindElement(names[i]);
                        if (fam?.Value != null) FluidAddedMass[i] = fam.Value.DoubleValue;
                    }
                }
            }

            // Visuals
            var visualElem = sdf.FindElement("visual");
            while (visualElem != null)
            {
                var visual = new Visual();
                errors.AddRange(visual.Load(visualElem));
                Visuals.Add(visual);
                visualElem = visualElem.GetNextElement("visual");
            }

            // Collisions
            var collisionElem = sdf.FindElement("collision");
            while (collisionElem != null)
            {
                var collision = new Collision();
                errors.AddRange(collision.Load(collisionElem));
                Collisions.Add(collision);
                collisionElem = collisionElem.GetNextElement("collision");
            }

            // Sensors
            var sensorElem = sdf.FindElement("sensor");
            while (sensorElem != null)
            {
                var sensor = new Sensor();
                errors.AddRange(sensor.Load(sensorElem));
                Sensors.Add(sensor);
                sensorElem = sensorElem.GetNextElement("sensor");
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

            // Particle emitters (stored as raw elements)
            var peElem = sdf.FindElement("particle_emitter");
            while (peElem != null)
            {
                ParticleEmitters.Add(peElem);
                peElem = peElem.GetNextElement("particle_emitter");
            }

            // Projectors (stored as raw elements)
            var projElem = sdf.FindElement("projector");
            while (projElem != null)
            {
                Projectors.Add(projElem);
                projElem = projElem.GetNextElement("projector");
            }

            // Battery
            var batteryElem = sdf.FindElement("battery");
            if (batteryElem != null)
            {
                var battNameAttr = batteryElem.GetAttribute("name");
                if (battNameAttr != null) BatteryName = battNameAttr.GetAsString();
                var voltageElem = batteryElem.FindElement("voltage");
                if (voltageElem?.Value != null) BatteryVoltage = voltageElem.Value.DoubleValue;
            }

            // Audio sink
            if (sdf.HasElement("audio_sink"))
                HasAudioSink = true;

            // Audio sources
            var audioSrcElem = sdf.FindElement("audio_source");
            while (audioSrcElem != null)
            {
                var src = new AudioSource();
                errors.AddRange(src.Load(audioSrcElem));
                AudioSources.Add(src);
                audioSrcElem = audioSrcElem.GetNextElement("audio_source");
            }

            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "link" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);

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

            foreach (var visual in Visuals)
                elem.InsertElement(visual.ToElement());
            foreach (var collision in Collisions)
                elem.InsertElement(collision.ToElement());
            foreach (var sensor in Sensors)
                elem.InsertElement(sensor.ToElement());
            foreach (var light in Lights)
                elem.InsertElement(light.ToElement());

            return elem;
        }
    }

    /// <summary>An audio source attached to a link.</summary>
    public class AudioSource : SdfElement
    {
        /// <summary>URI of the audio file.</summary>
        public string Uri { get; set; } = string.Empty;

        /// <summary>Pitch of the audio source.</summary>
        public double Pitch { get; set; } = 1.0;

        /// <summary>Gain of the audio source.</summary>
        public double Gain { get; set; } = 1.0;

        /// <summary>Whether the audio loops.</summary>
        public bool Loop { get; set; }

        /// <summary>Inner angle of the audio cone (radians).</summary>
        public double InnerAngle { get; set; }

        /// <summary>Outer angle of the audio cone (radians).</summary>
        public double OuterAngle { get; set; }

        /// <summary>Falloff factor for distance-based attenuation.</summary>
        public double Falloff { get; set; }

        /// <summary>Contact surface names that trigger audio playback.</summary>
        public List<string> ContactSurfaces { get; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var uri = sdf.FindElement("uri");
            if (uri?.Value != null) Uri = uri.Value.GetAsString();

            var pitch = sdf.FindElement("pitch");
            if (pitch?.Value != null) Pitch = pitch.Value.DoubleValue;

            var gain = sdf.FindElement("gain");
            if (gain?.Value != null) Gain = gain.Value.DoubleValue;

            var loop = sdf.FindElement("loop");
            if (loop?.Value != null) Loop = loop.Value.BoolValue;

            var innerAngle = sdf.FindElement("inner_angle");
            if (innerAngle?.Value != null) InnerAngle = innerAngle.Value.DoubleValue;

            var outerAngle = sdf.FindElement("outer_angle");
            if (outerAngle?.Value != null) OuterAngle = outerAngle.Value.DoubleValue;

            var falloff = sdf.FindElement("falloff");
            if (falloff?.Value != null) Falloff = falloff.Value.DoubleValue;

            var contact = sdf.FindElement("contact");
            if (contact != null)
            {
                var collisionElem = contact.FindElement("collision");
                while (collisionElem != null)
                {
                    if (collisionElem.Value != null)
                        ContactSurfaces.Add(collisionElem.Value.GetAsString());
                    collisionElem = collisionElem.GetNextElement("collision");
                }
            }

            return errors;
        }
    }
}
