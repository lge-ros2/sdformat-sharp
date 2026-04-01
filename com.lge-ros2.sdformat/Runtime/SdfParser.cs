// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// SDF XML parser — converts XML text to Element tree.
// Uses System.Xml.Linq (built into .NET) instead of tinyxml2.

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace SdFormat
{
    /// <summary>
    /// Parses SDF XML text or files into an Element tree.
    /// </summary>
    public class SdfParser
    {
        /// <summary>
        /// Parse an SDF XML string into an Element tree.
        /// Returns a tuple of (root element, errors).
        /// </summary>
        public (Element? Root, List<SdfError> Errors) Parse(string xmlString)
        {
            var errors = new List<SdfError>();

            XDocument doc;
            try
            {
                doc = XDocument.Parse(xmlString, LoadOptions.SetLineInfo);
            }
            catch (XmlException ex)
            {
                errors.Add(new SdfError(ErrorCode.XmlError,
                    $"XML parse error: {ex.Message}", "", ex.LineNumber));
                return (null, errors);
            }

            if (doc.Root == null)
            {
                errors.Add(new SdfError(ErrorCode.ParsingError,
                    "XML document has no root element."));
                return (null, errors);
            }

            var rootElement = ConvertXElement(doc.Root, errors);
            return (rootElement, errors);
        }

        /// <summary>
        /// Parse an SDF file into an Element tree.
        /// </summary>
        public (Element? Root, List<SdfError> Errors) ParseFile(string filePath)
        {
            var errors = new List<SdfError>();

            if (!File.Exists(filePath))
            {
                errors.Add(new SdfError(ErrorCode.FileRead,
                    $"File not found: {filePath}", filePath));
                return (null, errors);
            }

            string content;
            try
            {
                content = File.ReadAllText(filePath);
            }
            catch (IOException ex)
            {
                errors.Add(new SdfError(ErrorCode.FileRead,
                    $"Error reading file: {ex.Message}", filePath));
                return (null, errors);
            }

            var (root, parseErrors) = Parse(content);
            errors.AddRange(parseErrors);

            // Annotate element tree with file path
            if (root != null)
                AnnotateFilePath(root, filePath);

            return (root, errors);
        }

        /// <summary>
        /// Convert an XElement (System.Xml.Linq) into an SdFormat.Element recursively.
        /// </summary>
        private Element ConvertXElement(XElement xElem, List<SdfError> errors)
        {
            var element = new Element();
            element.Name = xElem.Name.LocalName;

            // Line number
            if (xElem is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
                element.LineNumber = lineInfo.LineNumber;

            // Attributes → Param attributes
            foreach (var attr in xElem.Attributes())
            {
                // Skip xmlns attributes
                if (attr.IsNamespaceDeclaration)
                    continue;

                element.AddAttribute(attr.Name.LocalName, "string", "", false);
                var param = element.GetAttribute(attr.Name.LocalName);
                param?.SetFromString(attr.Value);
            }

            // Text content (only direct text, not from children)
            string textContent = string.Empty;
            foreach (var node in xElem.Nodes())
            {
                if (node is XText textNode)
                {
                    textContent += textNode.Value;
                }
            }

            textContent = textContent.Trim();

            if (!string.IsNullOrEmpty(textContent))
            {
                // Infer type from content
                string typeName = InferType(textContent);
                element.AddValue(typeName, "", false);
                element.Value!.SetFromString(textContent);
            }

            // Child elements
            foreach (var child in xElem.Elements())
            {
                var childElem = ConvertXElement(child, errors);
                element.InsertElement(childElem);
            }

            return element;
        }

        /// <summary>
        /// Infer the SDF type name from a string value.
        /// </summary>
        private string InferType(string value)
        {
            if (value == "true" || value == "false" || value == "0" || value == "1")
                return "string"; // Could be bool or int, keep as string

            if (double.TryParse(value, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out _))
            {
                return "string"; // numeric but keep as string
            }

            // Could be vector, pose, color — keep as string and let specific
            // loaders parse as needed.
            return "string";
        }

        /// <summary>
        /// Set the FilePath on all elements in the tree.
        /// </summary>
        private void AnnotateFilePath(Element element, string filePath)
        {
            element.FilePath = filePath;
            foreach (var child in element.Children)
                AnnotateFilePath(child, filePath);
        }
    }
}
