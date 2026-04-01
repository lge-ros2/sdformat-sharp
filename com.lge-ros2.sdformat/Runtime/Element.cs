// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Element.hh

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SdFormat
{
    /// <summary>
    /// SDF Element class. Every SDF document is represented as a tree of Elements.
    /// Each Element has a name, optional attributes (Params), an optional value (Param),
    /// and child Elements. This is the low-level representation of an SDF document.
    /// </summary>
    public class Element
    {
        private string _name = string.Empty;
        private string _required = string.Empty;
        private string _description = string.Empty;
        private bool _explicitlySetInFile;
        private bool _copyChildren;
        private string _referenceSdf = string.Empty;
        private string _filePath = string.Empty;
        private string _xmlPath = string.Empty;
        private string _originalVersion = string.Empty;
        private int? _lineNumber;
        private Param? _value;
        private readonly List<Param> _attributes = new();
        private readonly List<Element> _children = new();
        private readonly List<Element> _elementDescriptions = new();
        private Element? _parent;
        private Element? _includeElement;

        /// <summary>Gets or sets the element name.</summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>Gets or sets the 'required' attribute string (e.g., "1", "0", "+", "*").</summary>
        public string Required
        {
            get => _required;
            set => _required = value;
        }

        /// <summary>Gets or sets whether this element was explicitly set in the loaded file.</summary>
        public bool ExplicitlySetInFile
        {
            get => _explicitlySetInFile;
            set => _explicitlySetInFile = value;
        }

        /// <summary>Gets or sets whether to copy children verbatim.</summary>
        public bool CopyChildren
        {
            get => _copyChildren;
            set => _copyChildren = value;
        }

        /// <summary>Gets or sets the reference SDF element name.</summary>
        public string ReferenceSdf
        {
            get => _referenceSdf;
            set => _referenceSdf = value;
        }

        /// <summary>Gets or sets the file path from which this element was loaded.</summary>
        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        /// <summary>Gets or sets the XML path within the document.</summary>
        public string XmlPath
        {
            get => _xmlPath;
            set => _xmlPath = value;
        }

        /// <summary>Gets or sets the original SDF version string.</summary>
        public string OriginalVersion
        {
            get => _originalVersion;
            set => _originalVersion = value;
        }

        /// <summary>Gets or sets the optional line number.</summary>
        public int? LineNumber
        {
            get => _lineNumber;
            set => _lineNumber = value;
        }

        /// <summary>Gets or sets the description.</summary>
        public string Description
        {
            get => _description;
            set => _description = value;
        }

        /// <summary>Gets or sets the parent element.</summary>
        public Element? Parent
        {
            get => _parent;
            set => _parent = value;
        }

        /// <summary>Gets or sets the include element.</summary>
        public Element? IncludeElement
        {
            get => _includeElement;
            set => _includeElement = value;
        }

        /// <summary>Gets the value parameter (body text of the element).</summary>
        public Param? Value => _value;

        /// <summary>Gets the list of child elements.</summary>
        public IReadOnlyList<Element> Children => _children;

        /// <summary>Gets the list of attributes.</summary>
        public IReadOnlyList<Param> Attributes => _attributes;

        /// <summary>Gets the list of element descriptions (schema).</summary>
        public IReadOnlyList<Element> ElementDescriptions => _elementDescriptions;

        // ---- Clone / Copy ----

        /// <summary>Clone this element and its children deeply.</summary>
        public Element Clone()
        {
            var clone = new Element
            {
                _name = _name,
                _required = _required,
                _description = _description,
                _explicitlySetInFile = _explicitlySetInFile,
                _copyChildren = _copyChildren,
                _referenceSdf = _referenceSdf,
                _filePath = _filePath,
                _xmlPath = _xmlPath,
                _originalVersion = _originalVersion,
                _lineNumber = _lineNumber,
                _value = _value?.Clone(),
            };

            foreach (var attr in _attributes)
                clone._attributes.Add(attr.Clone());

            foreach (var child in _children)
            {
                var childClone = child.Clone();
                childClone._parent = clone;
                clone._children.Add(childClone);
            }

            foreach (var desc in _elementDescriptions)
                clone._elementDescriptions.Add(desc.Clone());

            return clone;
        }

        /// <summary>Copy values from another element.</summary>
        public void Copy(Element other)
        {
            _name = other._name;
            _required = other._required;
            _description = other._description;
            _value = other._value?.Clone();

            _attributes.Clear();
            foreach (var attr in other._attributes)
                _attributes.Add(attr.Clone());

            _children.Clear();
            foreach (var child in other._children)
            {
                var childClone = child.Clone();
                childClone._parent = this;
                _children.Add(childClone);
            }
        }

        // ---- Attributes ----

        /// <summary>Add an attribute specification.</summary>
        public void AddAttribute(string key, string type, string defaultValue, bool required,
            string description = "")
        {
            var param = new Param(key, type, defaultValue, required, description);
            _attributes.Add(param);
        }

        /// <summary>Get an attribute by key.</summary>
        public Param? GetAttribute(string key) =>
            _attributes.FirstOrDefault(a => a.Key == key);

        /// <summary>Get an attribute by index.</summary>
        public Param? GetAttribute(int index) =>
            index >= 0 && index < _attributes.Count ? _attributes[index] : null;

        /// <summary>Returns the number of attributes.</summary>
        public int AttributeCount => _attributes.Count;

        /// <summary>Check if an attribute with the given key exists.</summary>
        public bool HasAttribute(string key) => _attributes.Any(a => a.Key == key);

        /// <summary>Check if an attribute has been explicitly set.</summary>
        public bool GetAttributeSet(string key)
        {
            var attr = GetAttribute(key);
            return attr?.IsSet ?? false;
        }

        /// <summary>Remove an attribute by key.</summary>
        public void RemoveAttribute(string key) =>
            _attributes.RemoveAll(a => a.Key == key);

        /// <summary>Remove all attributes.</summary>
        public void RemoveAllAttributes() => _attributes.Clear();

        // ---- Value ----

        /// <summary>Add a value specification to this element.</summary>
        public void AddValue(string type, string defaultValue, bool required,
            string description = "")
        {
            _value = new Param("", type, defaultValue, required, description);
        }

        /// <summary>Get the value parameter.</summary>
        public Param? GetValue() => _value;

        /// <summary>
        /// Get a value by key. If key is empty, returns the element value.
        /// Otherwise returns the attribute value.
        /// </summary>
        public T Get<T>(string key = "")
        {
            Param? param;
            if (string.IsNullOrEmpty(key))
                param = _value;
            else
                param = GetAttribute(key);

            if (param == null)
                return default!;

            if (param.Get<T>(out var val))
                return val;
            return default!;
        }

        /// <summary>Get a value with a default fallback.</summary>
        public (T value, bool found) Get<T>(string key, T defaultValue)
        {
            Param? param;
            if (string.IsNullOrEmpty(key))
                param = _value;
            else
                param = GetAttribute(key);

            if (param != null && param.Get<T>(out var val))
                return (val, true);
            return (defaultValue, false);
        }

        /// <summary>Set the element value.</summary>
        public bool Set<T>(T value)
        {
            if (_value == null)
                return false;
            return _value.Set(value);
        }

        // ---- Children ----

        /// <summary>Check if a child element with the given name exists.</summary>
        public bool HasElement(string name) => _children.Any(c => c._name == name);

        /// <summary>Get the first child element.</summary>
        public Element? GetFirstElement() => _children.Count > 0 ? _children[0] : null;

        /// <summary>
        /// Get the next sibling element. If name is specified, only considers
        /// elements with that name.
        /// </summary>
        public Element? GetNextElement(string name = "")
        {
            if (_parent == null) return null;
            int idx = _parent._children.IndexOf(this);
            if (idx < 0) return null;

            for (int i = idx + 1; i < _parent._children.Count; i++)
            {
                if (string.IsNullOrEmpty(name) || _parent._children[i]._name == name)
                    return _parent._children[i];
            }
            return null;
        }

        /// <summary>Get the set of distinct child element type names.</summary>
        public HashSet<string> GetElementTypeNames() =>
            new(_children.Select(c => c._name));

        /// <summary>
        /// Get or create a child element of the given name.
        /// If a matching description exists, a new child is created from it.
        /// Otherwise existing child is returned if found.
        /// </summary>
        public Element GetElement(string name)
        {
            var existing = _children.FirstOrDefault(c => c._name == name);
            if (existing != null) return existing;

            var desc = _elementDescriptions.FirstOrDefault(d => d._name == name);
            if (desc != null)
            {
                var newChild = desc.Clone();
                newChild._parent = this;
                _children.Add(newChild);
                return newChild;
            }

            var elem = new Element { _name = name, _parent = this };
            _children.Add(elem);
            return elem;
        }

        /// <summary>Find the first child element with the given name.</summary>
        public Element? FindElement(string name) =>
            _children.FirstOrDefault(c => c._name == name);

        /// <summary>Add a new child element with the given name.</summary>
        public Element AddElement(string name)
        {
            var desc = _elementDescriptions.FirstOrDefault(d => d._name == name);
            Element elem;
            if (desc != null)
            {
                elem = desc.Clone();
                elem._parent = this;
            }
            else
            {
                elem = new Element { _name = name, _parent = this };
            }
            _children.Add(elem);
            return elem;
        }

        /// <summary>Insert an existing element as a child.</summary>
        public void InsertElement(Element elem)
        {
            elem._parent = this;
            _children.Add(elem);
        }

        /// <summary>Remove this element from its parent.</summary>
        public void RemoveFromParent()
        {
            _parent?._children.Remove(this);
            _parent = null;
        }

        /// <summary>Remove a specific child element.</summary>
        public void RemoveChild(Element child)
        {
            _children.Remove(child);
            if (child._parent == this)
                child._parent = null;
        }

        /// <summary>Remove all child elements.</summary>
        public void ClearElements() => _children.Clear();

        /// <summary>Clear all data (attributes, children, value).</summary>
        public void Clear()
        {
            _attributes.Clear();
            _children.Clear();
            _elementDescriptions.Clear();
            _value = null;
        }

        /// <summary>Reset all attributes and children to defaults.</summary>
        public void Reset()
        {
            foreach (var attr in _attributes)
                attr.Reset();
            _value?.Reset();

            foreach (var child in _children)
                child.Reset();
        }

        // ---- Element Descriptions (Schema) ----

        /// <summary>Add an element description (defining schema for child elements).</summary>
        public void AddElementDescription(Element elem)
        {
            _elementDescriptions.Add(elem);
        }

        /// <summary>Get an element description by index.</summary>
        public Element? GetElementDescription(int index) =>
            index >= 0 && index < _elementDescriptions.Count ? _elementDescriptions[index] : null;

        /// <summary>Get an element description by name.</summary>
        public Element? GetElementDescription(string key) =>
            _elementDescriptions.FirstOrDefault(d => d._name == key);

        /// <summary>Check if an element description with the given name exists.</summary>
        public bool HasElementDescription(string name) =>
            _elementDescriptions.Any(d => d._name == name);

        // ---- Name Uniqueness ----

        /// <summary>Check if all children of a given type have unique names.</summary>
        public bool HasUniqueChildNames(string type = "")
        {
            var names = CountNamedElements(type);
            return names.Values.All(count => count <= 1);
        }

        /// <summary>Count named children, optionally filtered by type.</summary>
        public Dictionary<string, int> CountNamedElements(string type = "")
        {
            var counts = new Dictionary<string, int>();
            foreach (var child in _children)
            {
                if (!string.IsNullOrEmpty(type) && child._name != type)
                    continue;

                var nameAttr = child.GetAttribute("name");
                if (nameAttr != null)
                {
                    var name = nameAttr.GetAsString();
                    if (counts.ContainsKey(name))
                        counts[name]++;
                    else
                        counts[name] = 1;
                }
            }
            return counts;
        }

        // ---- String / Print ----

        /// <summary>
        /// Convert this element and its children to an SDF XML string.
        /// </summary>
        public string ToString(string prefix)
        {
            var sb = new StringBuilder();
            sb.Append($"{prefix}<{_name}");

            foreach (var attr in _attributes)
            {
                if (attr.IsSet)
                    sb.Append($" {attr.Key}='{attr.GetAsString()}'");
            }

            bool hasContent = _value != null || _children.Count > 0;
            if (!hasContent)
            {
                sb.AppendLine("/>");
                return sb.ToString();
            }

            sb.Append('>');

            if (_value != null && _value.IsSet)
            {
                if (_children.Count == 0)
                {
                    sb.Append(_value.GetAsString());
                    sb.AppendLine($"</{_name}>");
                    return sb.ToString();
                }
                sb.AppendLine(_value.GetAsString());
            }
            else
            {
                sb.AppendLine();
            }

            foreach (var child in _children)
                sb.Append(child.ToString(prefix + "  "));

            sb.AppendLine($"{prefix}</{_name}>");
            return sb.ToString();
        }

        /// <summary>Convert to string with no prefix.</summary>
        public override string ToString() => ToString("");
    }
}
