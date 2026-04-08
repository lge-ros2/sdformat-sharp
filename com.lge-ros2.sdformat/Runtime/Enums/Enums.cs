// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Material.hh, Noise.hh, Atmosphere.hh, Pbr.hh, ParserConfig.hh

#nullable enable

namespace SDFormat
{
    /// <summary>Shader type for materials.</summary>
    public enum ShaderType
    {
        Pixel = 0,
        Vertex = 1,
        NormalMapObjectSpace = 2,
        NormalMapTangentSpace = 3,
    }

    /// <summary>Noise model type.</summary>
    public enum NoiseType
    {
        None = 0,
        Gaussian = 1,
        GaussianQuantized = 2,
    }

    /// <summary>Type of atmosphere model.</summary>
    public enum AtmosphereType
    {
        Adiabatic = 0,
    }

    /// <summary>PBR workflow type.</summary>
    public enum PbrWorkflowType
    {
        None = 0,
        Metal = 1,
        Specular = 2,
    }

    /// <summary>Normal map space.</summary>
    public enum NormalMapSpace
    {
        Tangent = 0,
        Object = 1,
    }

    /// <summary>Policy for handling warnings, unrecognized elements, etc.</summary>
    public enum EnforcementPolicy
    {
        /// <summary>Treat as error.</summary>
        Err,
        /// <summary>Treat as warning.</summary>
        Warn,
        /// <summary>Log only.</summary>
        Log,
    }

    /// <summary>Configuration for auto-inertial calculation during load.</summary>
    public enum ConfigureResolveAutoInertials
    {
        SkipCalculationInLoad,
        SaveCalculation,
        SaveCalculationInElement,
    }

    /// <summary>Policy type for auto-inertial calculation failure.</summary>
    public enum CalculateInertialFailurePolicyType
    {
        Err,
        WarnAndUseDefaultInertial,
    }
}
