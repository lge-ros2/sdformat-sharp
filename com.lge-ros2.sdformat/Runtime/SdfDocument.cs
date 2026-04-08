// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - SDFImpl.hh

#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SDFormat
{
    /// <summary>
    /// The SDF (Simulation Description Format) document class.
    /// This holds the root element tree and provides methods for
    /// reading, writing, and manipulating SDF documents.
    /// </summary>
    public class SdfDocument
    {
        /// <summary>Default SDF version used when creating new documents.</summary>
        public const string DefaultVersion = "1.12";

        private static string _version = DefaultVersion;

        /// <summary>The root element of the SDF document tree.</summary>
        public Element? RootElement { get; set; }

        /// <summary>The file path from which this document was loaded.</summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>The original SDF version from the loaded file.</summary>
        public string OriginalVersion { get; set; } = DefaultVersion;

        /// <summary>Get or set the global SDF version.</summary>
        public static string Version
        {
            get => _version;
            set => _version = value;
        }

        /// <summary>Set the root element and update from an SDF string.</summary>
        public void SetFromString(string sdfData)
        {
            var parser = new SdfParser();
            var (root, _) = parser.Parse(sdfData);
            RootElement = root;
        }

        /// <summary>Clear the document.</summary>
        public void Clear()
        {
            RootElement = null;
            FilePath = string.Empty;
        }

        /// <summary>
        /// Write the SDF document to a file.
        /// </summary>
        public void Write(string filename)
        {
            if (RootElement == null) return;
            string xml = ToXmlString();
            File.WriteAllText(filename, xml);
        }

        /// <summary>
        /// Convert the document to an XML string.
        /// </summary>
        public string ToXmlString()
        {
            if (RootElement == null)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version='1.0' ?>");
            sb.Append(RootElement.ToString(""));
            return sb.ToString();
        }

        /// <summary>
        /// Get the document content as a string.
        /// </summary>
        public override string ToString() => ToXmlString();

        /// <summary>
        /// Wrap an element inside a root SDF element with version.
        /// </summary>
        public static Element WrapInRoot(Element element)
        {
            var root = new Element { Name = "sdf" };
            root.AddAttribute("version", "string", DefaultVersion, true);
            root.GetAttribute("version")!.SetFromString(Version);
            root.InsertElement(element.Clone());
            return root;
        }
    }
}
