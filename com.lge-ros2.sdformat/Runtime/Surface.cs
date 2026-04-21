// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Surface.hh

#nullable enable

using System.Collections.Generic;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>Contact properties of a surface.</summary>
    public class Contact : SdfElement
    {
        /// <summary>The collide bitmask for filtering collision pairs.</summary>
        public ushort CollideBitmask { get; set; } = 0xFF;

        /// <summary>Category bitmask for collision groups.</summary>
        public ushort CategoryBitmask { get; set; } = 0xFFFF;

        /// <summary>If true, contact force generation is disabled.</summary>
        public bool CollideWithoutContact { get; set; }

        /// <summary>Bitmask for collide_without_contact filtering.</summary>
        public uint CollideWithoutContactBitmask { get; set; } = 1;

        /// <summary>Poisson's ratio.</summary>
        public double PoissonsRatio { get; set; } = 0.3;

        /// <summary>Young's modulus (Pa). Negative disables elastic contact.</summary>
        public double ElasticModulus { get; set; } = -1;

        /// <summary>ODE-specific contact parameters.</summary>
        public OdeContact? OdeParams { get; set; }

        /// <summary>Bullet-specific contact parameters.</summary>
        public BulletContact? BulletParams { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var bitmask = sdf.FindElement("collide_bitmask");
            if (bitmask?.Value != null)
            {
                var str = bitmask.Value.GetAsString().Trim();
                if (str.StartsWith("0x", System.StringComparison.OrdinalIgnoreCase))
                    CollideBitmask = System.Convert.ToUInt16(str, 16);
                else if (ushort.TryParse(str, out var val))
                    CollideBitmask = val;
            }

            var catBitmask = sdf.FindElement("category_bitmask");
            if (catBitmask?.Value != null)
            {
                var str = catBitmask.Value.GetAsString().Trim();
                if (str.StartsWith("0x", System.StringComparison.OrdinalIgnoreCase))
                    CategoryBitmask = System.Convert.ToUInt16(str, 16);
                else if (ushort.TryParse(str, out var val))
                    CategoryBitmask = val;
            }

            var cwc = sdf.FindElement("collide_without_contact");
            if (cwc?.Value != null) CollideWithoutContact = cwc.Value.BoolValue;
            var cwcBitmask = sdf.FindElement("collide_without_contact_bitmask");
            if (cwcBitmask?.Value != null) CollideWithoutContactBitmask = (uint)cwcBitmask.Value.IntValue;
            var poisson = sdf.FindElement("poissons_ratio");
            if (poisson?.Value != null) PoissonsRatio = poisson.Value.DoubleValue;
            var elastic = sdf.FindElement("elastic_modulus");
            if (elastic?.Value != null) ElasticModulus = elastic.Value.DoubleValue;

            // ODE contact params
            if (sdf.HasElement("ode"))
            {
                OdeParams = new OdeContact();
                errors.AddRange(OdeParams.Load(sdf.FindElement("ode")!));
            }

            // Bullet contact params
            if (sdf.HasElement("bullet"))
            {
                BulletParams = new BulletContact();
                errors.AddRange(BulletParams.Load(sdf.FindElement("bullet")!));
            }

            return errors;
        }
    }

    /// <summary>ODE friction parameters.</summary>
    public class OdeFriction : SdfElement
    {
        public double Mu { get; set; } = 1.0;
        public double Mu2 { get; set; } = 1.0;
        public Vector3d Fdir1 { get; set; } = Vector3d.Zero;
        public double Slip1 { get; set; }
        public double Slip2 { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var mu = sdf.FindElement("mu");
            if (mu?.Value != null) Mu = mu.Value.DoubleValue;
            var mu2 = sdf.FindElement("mu2");
            if (mu2?.Value != null) Mu2 = mu2.Value.DoubleValue;
            var fdir1 = sdf.FindElement("fdir1");
            if (fdir1?.Value != null) Fdir1 = fdir1.Value.Vector3dValue;
            var slip1 = sdf.FindElement("slip1");
            if (slip1?.Value != null) Slip1 = slip1.Value.DoubleValue;
            var slip2 = sdf.FindElement("slip2");
            if (slip2?.Value != null) Slip2 = slip2.Value.DoubleValue;
            return errors;
        }
    }

    /// <summary>Bullet friction parameters.</summary>
    public class BulletFriction : SdfElement
    {
        public double Friction { get; set; } = 1.0;
        public double Friction2 { get; set; } = 1.0;
        public Vector3d Fdir1 { get; set; } = Vector3d.Zero;
        public double RollingFriction { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var friction = sdf.FindElement("friction");
            if (friction?.Value != null) Friction = friction.Value.DoubleValue;
            var friction2 = sdf.FindElement("friction2");
            if (friction2?.Value != null) Friction2 = friction2.Value.DoubleValue;
            var fdir1 = sdf.FindElement("fdir1");
            if (fdir1?.Value != null) Fdir1 = fdir1.Value.Vector3dValue;
            var rolling = sdf.FindElement("rolling_friction");
            if (rolling?.Value != null) RollingFriction = rolling.Value.DoubleValue;
            return errors;
        }
    }

    /// <summary>Torsional friction parameters.</summary>
    public class Torsional : SdfElement
    {
        public double Coefficient { get; set; } = 1.0;
        public bool UsePatchRadius { get; set; } = true;
        public double PatchRadius { get; set; }
        public double SurfaceRadius { get; set; }
        public double OdeSlip { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var coeff = sdf.FindElement("coefficient");
            if (coeff?.Value != null) Coefficient = coeff.Value.DoubleValue;
            var usePatch = sdf.FindElement("use_patch_radius");
            if (usePatch?.Value != null) UsePatchRadius = usePatch.Value.BoolValue;
            var patch = sdf.FindElement("patch_radius");
            if (patch?.Value != null) PatchRadius = patch.Value.DoubleValue;
            var surfR = sdf.FindElement("surface_radius");
            if (surfR?.Value != null) SurfaceRadius = surfR.Value.DoubleValue;
            var odeElem = sdf.FindElement("ode");
            if (odeElem != null)
            {
                var slip = odeElem.FindElement("slip");
                if (slip?.Value != null) OdeSlip = slip.Value.DoubleValue;
            }
            return errors;
        }
    }

    /// <summary>Friction properties of a surface.</summary>
    public class Friction : SdfElement
    {
        public OdeFriction? Ode { get; set; }
        public BulletFriction? Bullet { get; set; }
        public Torsional? TorsionalFriction { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            if (sdf.HasElement("ode"))
            {
                Ode = new OdeFriction();
                errors.AddRange(Ode.Load(sdf.FindElement("ode")!));
            }
            if (sdf.HasElement("bullet"))
            {
                Bullet = new BulletFriction();
                errors.AddRange(Bullet.Load(sdf.FindElement("bullet")!));
            }
            if (sdf.HasElement("torsional"))
            {
                TorsionalFriction = new Torsional();
                errors.AddRange(TorsionalFriction.Load(sdf.FindElement("torsional")!));
            }

            return errors;
        }
    }

    /// <summary>Bounce properties of a surface.</summary>
    public class Bounce : SdfElement
    {
        /// <summary>Coefficient of restitution [0..1].</summary>
        public double RestitutionCoefficient { get; set; }

        /// <summary>Bounce capture velocity threshold.</summary>
        public double Threshold { get; set; } = 100000;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var restitution = sdf.FindElement("restitution_coefficient");
            if (restitution?.Value != null) RestitutionCoefficient = restitution.Value.DoubleValue;
            var threshold = sdf.FindElement("threshold");
            if (threshold?.Value != null) Threshold = threshold.Value.DoubleValue;
            return errors;
        }
    }

    /// <summary>Surface properties combining contact and friction.</summary>
    public class Surface : SdfElement
    {
        public Contact? ContactInfo { get; set; }
        public Friction? FrictionInfo { get; set; }
        public Bounce? BounceInfo { get; set; }
        public DartSoftContact? SoftContact { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            if (sdf.HasElement("contact"))
            {
                ContactInfo = new Contact();
                errors.AddRange(ContactInfo.Load(sdf.FindElement("contact")!));
            }
            if (sdf.HasElement("friction"))
            {
                FrictionInfo = new Friction();
                errors.AddRange(FrictionInfo.Load(sdf.FindElement("friction")!));
            }
            if (sdf.HasElement("bounce"))
            {
                BounceInfo = new Bounce();
                errors.AddRange(BounceInfo.Load(sdf.FindElement("bounce")!));
            }
            if (sdf.HasElement("soft_contact"))
            {
                var softElem = sdf.FindElement("soft_contact")!;
                if (softElem.HasElement("dart"))
                {
                    SoftContact = new DartSoftContact();
                    errors.AddRange(SoftContact.Load(softElem.FindElement("dart")!));
                }
            }

            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "surface" };
            return elem;
        }
    }

    /// <summary>ODE-specific contact parameters.</summary>
    public class OdeContact : SdfElement
    {
        public double SoftCfm { get; set; }
        public double SoftErp { get; set; } = 0.2;
        public double Kp { get; set; } = 1e12;
        public double Kd { get; set; } = 1.0;
        public double MaxVel { get; set; } = 0.01;
        public double MinDepth { get; set; }

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var softCfm = sdf.FindElement("soft_cfm");
            if (softCfm?.Value != null) SoftCfm = softCfm.Value.DoubleValue;
            var softErp = sdf.FindElement("soft_erp");
            if (softErp?.Value != null) SoftErp = softErp.Value.DoubleValue;
            var kp = sdf.FindElement("kp");
            if (kp?.Value != null) Kp = kp.Value.DoubleValue;
            var kd = sdf.FindElement("kd");
            if (kd?.Value != null) Kd = kd.Value.DoubleValue;
            var maxVel = sdf.FindElement("max_vel");
            if (maxVel?.Value != null) MaxVel = maxVel.Value.DoubleValue;
            var minDepth = sdf.FindElement("min_depth");
            if (minDepth?.Value != null) MinDepth = minDepth.Value.DoubleValue;
            return errors;
        }
    }

    /// <summary>Bullet-specific contact parameters.</summary>
    public class BulletContact : SdfElement
    {
        public double SoftCfm { get; set; }
        public double SoftErp { get; set; } = 0.2;
        public double Kp { get; set; } = 1e12;
        public double Kd { get; set; } = 1.0;
        public bool SplitImpulse { get; set; } = true;
        public double SplitImpulsePenetrationThreshold { get; set; } = -0.01;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var softCfm = sdf.FindElement("soft_cfm");
            if (softCfm?.Value != null) SoftCfm = softCfm.Value.DoubleValue;
            var softErp = sdf.FindElement("soft_erp");
            if (softErp?.Value != null) SoftErp = softErp.Value.DoubleValue;
            var kp = sdf.FindElement("kp");
            if (kp?.Value != null) Kp = kp.Value.DoubleValue;
            var kd = sdf.FindElement("kd");
            if (kd?.Value != null) Kd = kd.Value.DoubleValue;
            var splitImpulse = sdf.FindElement("split_impulse");
            if (splitImpulse?.Value != null) SplitImpulse = splitImpulse.Value.BoolValue;
            var splitThresh = sdf.FindElement("split_impulse_penetration_threshold");
            if (splitThresh?.Value != null) SplitImpulsePenetrationThreshold = splitThresh.Value.DoubleValue;
            return errors;
        }
    }

    /// <summary>DART soft-contact parameters.</summary>
    public class DartSoftContact : SdfElement
    {
        public double BoneAttachment { get; set; } = 100.0;
        public double Stiffness { get; set; } = 100.0;
        public double Damping { get; set; } = 10.0;
        public double FleshMassFraction { get; set; } = 0.05;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var boneAttachment = sdf.FindElement("bone_attachment");
            if (boneAttachment?.Value != null) BoneAttachment = boneAttachment.Value.DoubleValue;
            var stiffness = sdf.FindElement("stiffness");
            if (stiffness?.Value != null) Stiffness = stiffness.Value.DoubleValue;
            var damping = sdf.FindElement("damping");
            if (damping?.Value != null) Damping = damping.Value.DoubleValue;
            var fleshMass = sdf.FindElement("flesh_mass_fraction");
            if (fleshMass?.Value != null) FleshMassFraction = fleshMass.Value.DoubleValue;
            return errors;
        }
    }
}
