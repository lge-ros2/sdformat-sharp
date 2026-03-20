// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Noise.hh

using System.Collections.Generic;

namespace SdFormat;

/// <summary>
/// A noise model, used by sensors to add noise to readings.
/// </summary>
public class Noise
{
    /// <summary>Type of noise model.</summary>
    public NoiseType Type { get; set; } = NoiseType.None;

    /// <summary>Mean of the Gaussian noise distribution.</summary>
    public double Mean { get; set; }

    /// <summary>Standard deviation of the Gaussian noise distribution.</summary>
    public double StdDev { get; set; }

    /// <summary>Mean of the noise bias.</summary>
    public double BiasMean { get; set; }

    /// <summary>Standard deviation of the noise bias.</summary>
    public double BiasStdDev { get; set; }

    /// <summary>Precision of quantized noise output.</summary>
    public double Precision { get; set; }

    /// <summary>Standard deviation of the dynamic bias.</summary>
    public double DynamicBiasStdDev { get; set; }

    /// <summary>Correlation time of the dynamic bias in seconds.</summary>
    public double DynamicBiasCorrelationTime { get; set; }

    /// <summary>The SDF element from which this was loaded.</summary>
    public Element? Element { get; set; }

    /// <summary>Load from an SDF element.</summary>
    public List<SdfError> Load(Element sdf)
    {
        var errors = new List<SdfError>();
        Element = sdf;

        var typeElem = sdf.FindElement("type");
        if (typeElem?.Value != null)
        {
            var t = typeElem.Value.GetAsString();
            Type = t switch
            {
                "gaussian" => NoiseType.Gaussian,
                "gaussian_quantized" => NoiseType.GaussianQuantized,
                _ => NoiseType.None,
            };
        }

        var mean = sdf.FindElement("mean");
        if (mean?.Value != null) Mean = mean.Value.DoubleValue;
        var stddev = sdf.FindElement("stddev");
        if (stddev?.Value != null) StdDev = stddev.Value.DoubleValue;
        var bm = sdf.FindElement("bias_mean");
        if (bm?.Value != null) BiasMean = bm.Value.DoubleValue;
        var bs = sdf.FindElement("bias_stddev");
        if (bs?.Value != null) BiasStdDev = bs.Value.DoubleValue;
        var prec = sdf.FindElement("precision");
        if (prec?.Value != null) Precision = prec.Value.DoubleValue;

        return errors;
    }

    /// <summary>Convert to an SDF element.</summary>
    public Element ToElement()
    {
        var elem = new Element { Name = "noise" };
        var typeChild = new Element { Name = "type" };
        typeChild.AddValue("string", "none", true);
        string typeStr = Type switch
        {
            NoiseType.Gaussian => "gaussian",
            NoiseType.GaussianQuantized => "gaussian_quantized",
            _ => "none"
        };
        typeChild.Set(typeStr);
        elem.InsertElement(typeChild);
        return elem;
    }
}
