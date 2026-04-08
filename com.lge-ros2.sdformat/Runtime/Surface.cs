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

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;
            var bitmask = sdf.FindElement("collide_bitmask");
            if (bitmask?.Value != null)
                CollideBitmask = (ushort)bitmask.Value.IntValue;
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

    /// <summary>Surface properties combining contact and friction.</summary>
    public class Surface : SdfElement
    {
        public Contact? ContactInfo { get; set; }
        public Friction? FrictionInfo { get; set; }

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

            return errors;
        }

        public Element ToElement()
        {
            var elem = new Element { Name = "surface" };
            return elem;
        }
    }
}
