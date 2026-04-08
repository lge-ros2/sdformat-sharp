// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Error.hh

#nullable enable

namespace SDFormat
{
    /// <summary>
    /// Set of error codes. Each code is associated with a human-readable error message.
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>No error.</summary>
        None = 0,
        /// <summary>Unable to read a file.</summary>
        FileRead,
        /// <summary>A duplicate name was found.</summary>
        DuplicateName,
        /// <summary>A reserved name was used.</summary>
        ReservedName,
        /// <summary>An attribute is missing.</summary>
        AttributeMissing,
        /// <summary>An attribute has an invalid value.</summary>
        AttributeInvalid,
        /// <summary>An attribute is deprecated.</summary>
        AttributeDeprecated,
        /// <summary>An attribute has an incorrect type.</summary>
        AttributeIncorrectType,
        /// <summary>A required element is missing.</summary>
        ElementMissing,
        /// <summary>An element has an invalid value.</summary>
        ElementInvalid,
        /// <summary>An element has been deprecated.</summary>
        ElementDeprecated,
        /// <summary>An element has an incorrect type.</summary>
        ElementIncorrectType,
        /// <summary>Generic element error.</summary>
        ElementError,
        /// <summary>A URI is invalid.</summary>
        UriInvalid,
        /// <summary>A URI lookup failed.</summary>
        UriLookup,
        /// <summary>A directory does not exist.</summary>
        DirectoryNonexistent,
        /// <summary>Model's canonical link is invalid.</summary>
        ModelCanonicalLinkInvalid,
        /// <summary>A model doesn't have any links.</summary>
        ModelWithoutLink,
        /// <summary>Nested models are unsupported.</summary>
        NestedModelsUnsupported,
        /// <summary>A link's inertia is invalid.</summary>
        LinkInertiaInvalid,
        /// <summary>A joint's child link is invalid.</summary>
        JointChildLinkInvalid,
        /// <summary>A joint's parent link is invalid.</summary>
        JointParentLinkInvalid,
        /// <summary>A joint's parent and child are the same.</summary>
        JointParentSameAsChild,
        /// <summary>Frame attached-to value is invalid.</summary>
        FrameAttachedToInvalid,
        /// <summary>Frame attached-to graph has a cycle.</summary>
        FrameAttachedToCycle,
        /// <summary>Frame attached-to graph error.</summary>
        FrameAttachedToGraphError,
        /// <summary>Pose relative-to value is invalid.</summary>
        PoseRelativeToInvalid,
        /// <summary>Pose relative-to graph has a cycle.</summary>
        PoseRelativeToCycle,
        /// <summary>Pose relative-to graph error.</summary>
        PoseRelativeToGraphError,
        /// <summary>Rotation snap configuration error.</summary>
        RotationSnapConfigError,
        /// <summary>Error reading a string value.</summary>
        StringRead,
        /// <summary>Model placement frame is invalid.</summary>
        ModelPlacementFrameInvalid,
        /// <summary>Deprecated SDF version.</summary>
        VersionDeprecated,
        /// <summary>Merge include is unsupported.</summary>
        MergeIncludeUnsupported,
        /// <summary>Parameter error.</summary>
        ParameterError,
        /// <summary>Unknown parameter type.</summary>
        UnknownParameterType,
        /// <summary>Fatal error.</summary>
        FatalError,
        /// <summary>Warning (non-fatal).</summary>
        Warning,
        /// <summary>Joint axis expressed-in value is invalid.</summary>
        JointAxisExpressedInInvalid,
        /// <summary>Conversion error.</summary>
        ConversionError,
        /// <summary>Parsing error.</summary>
        ParsingError,
        /// <summary>Joint axis mimic constraint is invalid.</summary>
        JointAxisMimicInvalid,
        /// <summary>XML error.</summary>
        XmlError,
    }
}
