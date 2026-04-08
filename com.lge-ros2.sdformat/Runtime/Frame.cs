// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Frame.hh

#nullable enable

using System.Collections.Generic;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>
    /// An explicit frame defined in a model or world.
    /// </summary>
    public class Frame : SdfNamedPosedElement
    {
        /// <summary>
        /// Name of the body (link/model/frame) to which this frame is attached.
        /// </summary>
        public string AttachedTo { get; set; } = string.Empty;

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var attachedTo = sdf.GetAttribute("attached_to");
            if (attachedTo != null) AttachedTo = attachedTo.GetAsString();

            var poseElem = sdf.FindElement("pose");
            if (poseElem?.Value != null)
            {
                RawPose = poseElem.Value.Pose3dValue;
                var relTo = poseElem.GetAttribute("relative_to");
                if (relTo != null) PoseRelativeTo = relTo.GetAsString();
            }

            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "frame" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);

            if (!string.IsNullOrEmpty(AttachedTo))
            {
                elem.AddAttribute("attached_to", "string", "", false);
                elem.GetAttribute("attached_to")!.SetFromString(AttachedTo);
            }

            if (RawPose != Pose3d.Zero || !string.IsNullOrEmpty(PoseRelativeTo))
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

            return elem;
        }
    }
}
