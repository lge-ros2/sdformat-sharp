// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Plugin.hh

#nullable enable

using System.Collections.Generic;
using System.IO;

namespace SDFormat
{
    /// <summary>
    /// An SDF plugin element, identified by name and filename.
    /// Plugins can contain arbitrary child elements as content.
    /// </summary>
    public class Plugin : SdfElement
    {
        /// <summary>The name of the plugin.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>The filename/path of the plugin shared library.</summary>
        public string Filename { get; set; } = string.Empty;

        /// <summary>Arbitrary child elements (content).</summary>
        public List<Element> Contents { get; } = new();

        public Plugin() { }

        public Plugin(string filename, string name, string xmlContent = "")
        {
            Filename = filename;
            Name = name;
            if (!string.IsNullOrEmpty(xmlContent))
            {
                // Parse xmlContent into elements — simplified for now
            }
        }

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var fileAttr = sdf.GetAttribute("filename");
            if (fileAttr != null) Filename = fileAttr.GetAsString();

            // Load all child elements as content
            var child = sdf.GetFirstElement();
            while (child != null)
            {
                Contents.Add(child);
                child = child.GetNextElement();
            }

            return errors;
        }

        /// <summary>Clear all content elements.</summary>
        public void ClearContents() => Contents.Clear();

        /// <summary>Insert a content element.</summary>
        public void InsertContent(Element elem) => Contents.Add(elem);

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "plugin" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);
            elem.AddAttribute("filename", "string", "", true);
            elem.GetAttribute("filename")!.SetFromString(Filename);

            foreach (var content in Contents)
                elem.InsertElement(content.Clone());

            return elem;
        }

        public override string ToString() => $"Plugin[{Name}, {Filename}]";
    }
}
