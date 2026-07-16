// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Noise.hh

#nullable enable

using System.Collections.Generic;

namespace SDFormat
{
    /// <summary>
    /// A noise model, used by sensors to add noise to readings.
    /// </summary>
    public class Noise : SdfElement
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

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var typeAttr = sdf.GetAttribute("type");
            if (typeAttr != null)
            {
                Type = typeAttr.GetAsString() switch
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
            var dynStdDev = sdf.FindElement("dynamic_bias_stddev");
            if (dynStdDev?.Value != null) DynamicBiasStdDev = dynStdDev.Value.DoubleValue;
            var dynCorr = sdf.FindElement("dynamic_bias_correlation_time");
            if (dynCorr?.Value != null) DynamicBiasCorrelationTime = dynCorr.Value.DoubleValue;

            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "noise" };
            string typeStr = Type switch
            {
                NoiseType.Gaussian => "gaussian",
                NoiseType.GaussianQuantized => "gaussian_quantized",
                _ => "none"
            };
            elem.AddAttribute("type", "string", "none", true);
            elem.GetAttribute("type")!.SetFromString(typeStr);
            return elem;
        }
    }
}
