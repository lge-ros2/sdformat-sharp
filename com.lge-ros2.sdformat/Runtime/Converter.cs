// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Converter.hh / Converter.cc
//
// Upgrades an SDF <sdf version="..."> Element tree to the latest supported
// spec version before any DOM class (Root, Model, Joint, ...) parses it.
// Mirrors libsdformat's declarative ".convert" rule files (sdf/<N>/<M>.convert)
// applied one version-step at a time. Supported input range: 1.4 - 1.12
// (1.0 - 1.3 is not supported; those steps are legacy and out of scope).

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace SDFormat
{
    /// <summary>
    /// Converts an SDF Element tree from an older declared version up to the
    /// latest supported version, using the same declarative rule set as
    /// upstream libsdformat's Converter class.
    /// </summary>
    public static class Converter
    {
        // ---- Rule text for each version step (verbatim ports of upstream
        // sdf/<N>/<M>.convert files, with attribute quoting changed from " to '
        // so they can be embedded as plain C# verbatim strings). Steps with no
        // rule text are no-ops (upstream's corresponding .convert file is empty). ----

        private const string Rules_1_4_to_1_5 = @"
<convert name='sdf'>

  <convert name='world'>
    <convert name='model'>
      <convert name='joint'>
        <convert name='axis'>
          <add element='use_parent_model_frame' value='true'/>
        </convert>
        <convert name='axis2'>
          <add element='use_parent_model_frame' value='true'/>
        </convert>
      </convert>
    </convert>

    <convert name='actor'>
      <rename>
        <from attribute='static'/>
        <to element='static'/>
      </rename>
    </convert>
  </convert>

  <convert name='model'>
    <convert name='joint'>
      <convert name='axis'>
        <add element='use_parent_model_frame' value='true'/>
      </convert>
      <convert name='axis2'>
        <add element='use_parent_model_frame' value='true'/>
      </convert>
    </convert>

    <convert name='actor'>
      <rename>
        <from attribute='static'/>
        <to element='static'/>
      </rename>
    </convert>
  </convert>

</convert>
";

        private const string Rules_1_5_to_1_6 = @"
<convert name='sdf'>

  <convert name='world'>
    <move>
      <from element='physics::gravity'/>
      <to element='gravity'/>
    </move>
    <move>
      <from element='physics::magnetic_field'/>
      <to element='magnetic_field'/>
    </move>
  </convert>

  <convert name='world'>
    <convert name='model'>
      <convert name='link'>
        <convert name='sensor'>
          <convert name='imu'>
            <add element='angular_velocity'/>
            <add element='linear_acceleration'/>

            <copy>
              <from element='noise::type'/>
              <to element='angular_velocity::x::noise' attribute='type'/>
            </copy>
            <copy>
              <from element='noise::type'/>
              <to element='angular_velocity::y::noise' attribute='type'/>
            </copy>
            <copy>
              <from element='noise::type'/>
              <to element='angular_velocity::z::noise' attribute='type'/>
            </copy>

            <copy>
              <from element='noise::type'/>
              <to element='linear_acceleration::x::noise' attribute='type'/>
            </copy>
            <copy>
              <from element='noise::type'/>
              <to element='linear_acceleration::y::noise' attribute='type'/>
            </copy>
            <move>
              <from element='noise::type'/>
              <to element='linear_acceleration::z::noise' attribute='type'/>
            </move>

            <copy>
              <from element='noise::rate::mean'/>
              <to element='angular_velocity::x::noise::mean'/>
            </copy>
            <copy>
              <from element='noise::rate::mean'/>
              <to element='angular_velocity::y::noise::mean'/>
            </copy>
            <move>
              <from element='noise::rate::mean'/>
              <to element='angular_velocity::z::noise::mean'/>
            </move>

            <copy>
              <from element='noise::rate::stddev'/>
              <to element='angular_velocity::x::noise::stddev'/>
            </copy>
            <copy>
              <from element='noise::rate::stddev'/>
              <to element='angular_velocity::y::noise::stddev'/>
            </copy>
            <move>
              <from element='noise::rate::stddev'/>
              <to element='angular_velocity::z::noise::stddev'/>
            </move>

            <copy>
              <from element='noise::rate::bias_mean'/>
              <to element='angular_velocity::x::noise::bias_mean'/>
            </copy>
            <copy>
              <from element='noise::rate::bias_mean'/>
              <to element='angular_velocity::y::noise::bias_mean'/>
            </copy>
            <move>
              <from element='noise::rate::bias_mean'/>
              <to element='angular_velocity::z::noise::bias_mean'/>
            </move>

            <copy>
              <from element='noise::rate::bias_stddev'/>
              <to element='angular_velocity::x::noise::bias_stddev'/>
            </copy>
            <copy>
              <from element='noise::rate::bias_stddev'/>
              <to element='angular_velocity::y::noise::bias_stddev'/>
            </copy>
            <move>
              <from element='noise::rate::bias_stddev'/>
              <to element='angular_velocity::z::noise::bias_stddev'/>
            </move>

            <copy>
              <from element='noise::accel::mean'/>
              <to element='linear_acceleration::x::noise::mean'/>
            </copy>
            <copy>
              <from element='noise::accel::mean'/>
              <to element='linear_acceleration::y::noise::mean'/>
            </copy>
            <move>
              <from element='noise::accel::mean'/>
              <to element='linear_acceleration::z::noise::mean'/>
            </move>

            <copy>
              <from element='noise::accel::stddev'/>
              <to element='linear_acceleration::x::noise::stddev'/>
            </copy>
            <copy>
              <from element='noise::accel::stddev'/>
              <to element='linear_acceleration::y::noise::stddev'/>
            </copy>
            <move>
              <from element='noise::accel::stddev'/>
              <to element='linear_acceleration::z::noise::stddev'/>
            </move>

            <copy>
              <from element='noise::accel::bias_mean'/>
              <to element='linear_acceleration::x::noise::bias_mean'/>
            </copy>
            <copy>
              <from element='noise::accel::bias_mean'/>
              <to element='linear_acceleration::y::noise::bias_mean'/>
            </copy>
            <move>
              <from element='noise::accel::bias_mean'/>
              <to element='linear_acceleration::z::noise::bias_mean'/>
            </move>

            <copy>
              <from element='noise::accel::bias_stddev'/>
              <to element='linear_acceleration::x::noise::bias_stddev'/>
            </copy>
            <copy>
              <from element='noise::accel::bias_stddev'/>
              <to element='linear_acceleration::y::noise::bias_stddev'/>
            </copy>
            <move>
              <from element='noise::accel::bias_stddev'/>
              <to element='linear_acceleration::z::noise::bias_stddev'/>
            </move>

            <remove element='noise'/>
          </convert>
        </convert>
      </convert>
    </convert>
  </convert>

  <convert name='model'>
    <convert name='link'>
      <convert name='sensor'>
        <convert name='imu'>
          <add element='angular_velocity'/>
          <add element='linear_acceleration'/>

          <copy>
            <from element='noise::type'/>
            <to element='angular_velocity::x::noise' attribute='type'/>
          </copy>
          <copy>
            <from element='noise::type'/>
            <to element='angular_velocity::y::noise' attribute='type'/>
          </copy>
          <copy>
            <from element='noise::type'/>
            <to element='angular_velocity::z::noise' attribute='type'/>
          </copy>

          <copy>
            <from element='noise::type'/>
            <to element='linear_acceleration::x::noise' attribute='type'/>
          </copy>
          <copy>
            <from element='noise::type'/>
            <to element='linear_acceleration::y::noise' attribute='type'/>
          </copy>
          <move>
            <from element='noise::type'/>
            <to element='linear_acceleration::z::noise' attribute='type'/>
          </move>

          <copy>
            <from element='noise::rate::mean'/>
            <to element='angular_velocity::x::noise::mean'/>
          </copy>
          <copy>
            <from element='noise::rate::mean'/>
            <to element='angular_velocity::y::noise::mean'/>
          </copy>
          <move>
            <from element='noise::rate::mean'/>
            <to element='angular_velocity::z::noise::mean'/>
          </move>

          <copy>
            <from element='noise::rate::stddev'/>
            <to element='angular_velocity::x::noise::stddev'/>
          </copy>
          <copy>
            <from element='noise::rate::stddev'/>
            <to element='angular_velocity::y::noise::stddev'/>
          </copy>
          <move>
            <from element='noise::rate::stddev'/>
            <to element='angular_velocity::z::noise::stddev'/>
          </move>

          <copy>
            <from element='noise::rate::bias_mean'/>
            <to element='angular_velocity::x::noise::bias_mean'/>
          </copy>
          <copy>
            <from element='noise::rate::bias_mean'/>
            <to element='angular_velocity::y::noise::bias_mean'/>
          </copy>
          <move>
            <from element='noise::rate::bias_mean'/>
            <to element='angular_velocity::z::noise::bias_mean'/>
          </move>

          <copy>
            <from element='noise::rate::bias_stddev'/>
            <to element='angular_velocity::x::noise::bias_stddev'/>
          </copy>
          <copy>
            <from element='noise::rate::bias_stddev'/>
            <to element='angular_velocity::y::noise::bias_stddev'/>
          </copy>
          <move>
            <from element='noise::rate::bias_stddev'/>
            <to element='angular_velocity::z::noise::bias_stddev'/>
          </move>

          <copy>
            <from element='noise::accel::mean'/>
            <to element='linear_acceleration::x::noise::mean'/>
          </copy>
          <copy>
            <from element='noise::accel::mean'/>
            <to element='linear_acceleration::y::noise::mean'/>
          </copy>
          <move>
            <from element='noise::accel::mean'/>
            <to element='linear_acceleration::z::noise::mean'/>
          </move>

          <copy>
            <from element='noise::accel::stddev'/>
            <to element='linear_acceleration::x::noise::stddev'/>
          </copy>
          <copy>
            <from element='noise::accel::stddev'/>
            <to element='linear_acceleration::y::noise::stddev'/>
          </copy>
          <move>
            <from element='noise::accel::stddev'/>
            <to element='linear_acceleration::z::noise::stddev'/>
          </move>

          <copy>
            <from element='noise::accel::bias_mean'/>
            <to element='linear_acceleration::x::noise::bias_mean'/>
          </copy>
          <copy>
            <from element='noise::accel::bias_mean'/>
            <to element='linear_acceleration::y::noise::bias_mean'/>
          </copy>
          <move>
            <from element='noise::accel::bias_mean'/>
            <to element='linear_acceleration::z::noise::bias_mean'/>
          </move>

          <copy>
            <from element='noise::accel::bias_stddev'/>
            <to element='linear_acceleration::x::noise::bias_stddev'/>
          </copy>
          <copy>
            <from element='noise::accel::bias_stddev'/>
            <to element='linear_acceleration::y::noise::bias_stddev'/>
          </copy>
          <move>
            <from element='noise::accel::bias_stddev'/>
            <to element='linear_acceleration::z::noise::bias_stddev'/>
          </move>

          <remove element='noise'/>
        </convert>
      </convert>
    </convert>
  </convert>
</convert>
";

        private const string Rules_1_6_to_1_7 = @"
<convert name='sdf'>

  <convert descendant_name='pose'>
    <move>
      <from attribute='frame'/>
      <to attribute='relative_to'/>
    </move>
  </convert>

  <convert descendant_name='joint'>
    <convert name='axis'>
      <map>
        <from name='use_parent_model_frame'>
          <value>true</value>
          <value>True</value>
          <value>TRUE</value>
          <value>1</value>
        </from>
        <to name='xyz/@expressed_in'>
          <value>__model__</value>
        </to>
      </map>
      <remove element='use_parent_model_frame'/>
    </convert>
    <convert name='axis2'>
      <map>
        <from name='use_parent_model_frame'>
          <value>true</value>
          <value>True</value>
          <value>TRUE</value>
          <value>1</value>
        </from>
        <to name='xyz/@expressed_in'>
          <value>__model__</value>
        </to>
      </map>
      <remove element='use_parent_model_frame'/>
    </convert>
  </convert>

</convert>
";

        private const string Rules_1_7_to_1_8 = @"
<convert name='sdf'>

  <convert name='world'>
    <convert name='model'>
      <unflatten/>
    </convert>
  </convert>

  <convert descendant_name='inertial'>
    <convert name='pose'>
      <remove_empty attribute='relative_to' />
    </convert>
  </convert>

</convert>
";

        // Fixed, ordered version chain. Steps with a null rule set are no-ops
        // (upstream's own .convert file for that step is empty).
        private static readonly (string From, string To, string? RuleXml)[] VersionChain =
        {
            ("1.4", "1.5", Rules_1_4_to_1_5),
            ("1.5", "1.6", Rules_1_5_to_1_6),
            ("1.6", "1.7", Rules_1_6_to_1_7),
            ("1.7", "1.8", Rules_1_7_to_1_8),
            ("1.8", "1.9", null),
            ("1.9", "1.10", null),
            ("1.10", "1.11", null),
            ("1.11", "1.12", null),
        };

        private static readonly HashSet<string> FlattenableTags =
            new() { "frame", "joint", "link", "model", "gripper" };

        /// <summary>Convert an `&lt;sdf&gt;` Element tree in-place up to the latest supported version.</summary>
        public static List<SdfError> ConvertToLatest(Element sdfRoot) =>
            Convert(sdfRoot, SdfDocument.DefaultVersion);

        /// <summary>Convert an `&lt;sdf&gt;` Element tree in-place up to the given target version.</summary>
        public static List<SdfError> Convert(Element sdfRoot, string toVersion)
        {
            var errors = new List<SdfError>();

            if (sdfRoot.Name != "sdf") return errors;

            var versionAttr = sdfRoot.GetAttribute("version");
            if (versionAttr == null) return errors;

            string origVersion = versionAttr.GetAsString();
            if (origVersion == toVersion) return errors;

            // Stamp the target version immediately, mirroring upstream behavior.
            versionAttr.SetFromString(toVersion);

            string cur = origVersion;
            bool started = false;
            foreach (var (from, to, ruleXml) in VersionChain)
            {
                if (!started)
                {
                    if (from != cur) continue;
                    started = true;
                }

                if (ruleXml != null)
                {
                    var (ruleRoot, parseErrors) = new SdfParser().Parse(ruleXml);
                    errors.AddRange(parseErrors);
                    if (ruleRoot != null)
                        ConvertImpl(sdfRoot, ruleRoot, errors);
                }

                cur = to;
                if (cur == toVersion) break;
            }

            if (cur != toVersion)
            {
                errors.Add(new SdfError(ErrorCode.ConversionError,
                    $"Unable to convert from SDF version {origVersion} to {toVersion}"));
            }

            return errors;
        }

        // ---- Core recursive engine ----

        private static void ConvertImpl(Element elem, Element convertElem, List<SdfError> errors)
        {
            CheckDeprecation(elem, convertElem, errors);

            // Pass 1: recurse into nested <convert> blocks first.
            var nested = convertElem.FindElement("convert");
            while (nested != null)
            {
                var nameAttr = nested.GetAttribute("name");
                if (nameAttr != null)
                {
                    string name = nameAttr.GetAsString();
                    var e = elem.FindElement(name);
                    while (e != null)
                    {
                        ConvertImpl(e, nested, errors);
                        e = e.GetNextElement(name);
                    }
                }

                var descAttr = nested.GetAttribute("descendant_name");
                if (descAttr != null)
                    ConvertDescendantsImpl(elem, nested, errors);

                nested = nested.GetNextElement("convert");
            }

            // Pass 2: apply this level's own operations to `elem`.
            foreach (var op in convertElem.Children.ToList())
            {
                switch (op.Name)
                {
                    case "convert":
                    case "deprecated":
                        break; // handled above / by CheckDeprecation
                    case "rename": Rename(elem, op, errors); break;
                    case "copy": Move(elem, op, copy: true, errors); break;
                    case "move": Move(elem, op, copy: false, errors); break;
                    case "map": Map(elem, op, errors); break;
                    case "add": Add(elem, op, errors); break;
                    case "remove": Remove(elem, op, onlyEmpty: false, errors); break;
                    case "remove_empty": Remove(elem, op, onlyEmpty: true, errors); break;
                    case "unflatten": Unflatten(elem, errors); break;
                    default:
                        errors.Add(new SdfError(ErrorCode.ConversionError,
                            $"Unknown convert element[{op.Name}]"));
                        break;
                }
            }
        }

        private static void ConvertDescendantsImpl(Element elem, Element convertElem, List<SdfError> errors)
        {
            var descAttr = convertElem.GetAttribute("descendant_name");
            if (descAttr == null) return;
            if (elem.Name == "plugin") return;
            if (elem.Name.Contains(':')) return;

            string descendantName = descAttr.GetAsString();
            foreach (var child in elem.Children.ToList())
            {
                if (child.Name == descendantName)
                    ConvertImpl(child, convertElem, errors);
                ConvertDescendantsImpl(child, convertElem, errors);
            }
        }

        private static void CheckDeprecation(Element elem, Element convertElem, List<SdfError> errors)
        {
            var deprecated = convertElem.FindElement("deprecated");
            while (deprecated != null)
            {
                string? path = deprecated.Value?.GetAsString();
                if (!string.IsNullOrEmpty(path))
                {
                    errors.Add(new SdfError(ErrorCode.VersionDeprecated,
                        $"'{path}' is deprecated in this element."));
                }
                deprecated = deprecated.GetNextElement("deprecated");
            }
        }

        // ---- Value resolution helper (mirrors upstream Converter::GetValue) ----

        private static string? GetValue(string? elementName, string? attributeName, Element elem)
        {
            if (!string.IsNullOrEmpty(elementName))
            {
                var child = elem.FindElement(elementName);
                if (child == null) return null;
                if (!string.IsNullOrEmpty(attributeName))
                    return child.GetAttribute(attributeName)?.GetAsString();
                return child.Value?.GetAsString();
            }

            if (!string.IsNullOrEmpty(attributeName))
                return elem.GetAttribute(attributeName)?.GetAsString();

            return null;
        }

        // ---- <rename> ----

        private static void Rename(Element elem, Element renameElem, List<SdfError> errors)
        {
            var fromElem = renameElem.FindElement("from");
            var toElem = renameElem.FindElement("to");
            if (fromElem == null)
            {
                errors.Add(new SdfError(ErrorCode.ConversionError, "No 'from' element name specified"));
                return;
            }

            string? toElemName = toElem?.GetAttribute("element")?.GetAsString();
            if (string.IsNullOrEmpty(toElemName))
            {
                errors.Add(new SdfError(ErrorCode.ConversionError, "No 'to' element name specified"));
                return;
            }

            string? fromElemName = fromElem.GetAttribute("element")?.GetAsString();
            string? fromAttrName = fromElem.GetAttribute("attribute")?.GetAsString();
            string? toAttrName = toElem!.GetAttribute("attribute")?.GetAsString();

            string? value = GetValue(fromElemName, fromAttrName, elem);
            if (value == null) return; // source doesn't exist in this document — silent no-op

            var replacement = new Element { Name = toElemName };
            if (!string.IsNullOrEmpty(toAttrName))
            {
                replacement.AddAttribute(toAttrName, "string", "", false);
                replacement.GetAttribute(toAttrName)!.SetFromString(value);
            }
            else
            {
                replacement.AddValue("string", "", false);
                replacement.Set(value);
            }

            if (!string.IsNullOrEmpty(fromElemName))
            {
                var oldChild = elem.FindElement(fromElemName);
                if (oldChild != null)
                    elem.RemoveChild(oldChild);
                elem.InsertElement(replacement);
            }
            else if (!string.IsNullOrEmpty(fromAttrName))
            {
                elem.RemoveAttribute(fromAttrName);
                elem.InsertElement(replacement);
            }
        }

        // ---- <add> ----

        private static void Add(Element elem, Element addElem, List<SdfError> errors)
        {
            string? elementName = addElem.GetAttribute("element")?.GetAsString();
            string? attributeName = addElem.GetAttribute("attribute")?.GetAsString();
            string? value = addElem.GetAttribute("value")?.GetAsString();

            bool hasElement = !string.IsNullOrEmpty(elementName);
            bool hasAttribute = !string.IsNullOrEmpty(attributeName);
            if (hasElement == hasAttribute)
            {
                errors.Add(new SdfError(ErrorCode.ConversionError,
                    "Exactly one 'element' or 'attribute' must be specified in <add>"));
                return;
            }

            if (hasAttribute)
            {
                if (value == null)
                {
                    errors.Add(new SdfError(ErrorCode.ConversionError, "No 'value' specified in <add>"));
                    return;
                }
                elem.AddAttribute(attributeName!, "string", "", false);
                elem.GetAttribute(attributeName!)!.SetFromString(value);
            }
            else
            {
                var newElem = new Element { Name = elementName! };
                if (value != null)
                {
                    newElem.AddValue("string", "", false);
                    newElem.Set(value);
                }
                elem.InsertElement(newElem);
            }
        }

        // ---- <remove> / <remove_empty> ----

        private static void Remove(Element elem, Element removeElem, bool onlyEmpty, List<SdfError> errors)
        {
            string? elementName = removeElem.GetAttribute("element")?.GetAsString();
            string? attributeName = removeElem.GetAttribute("attribute")?.GetAsString();

            bool hasElement = !string.IsNullOrEmpty(elementName);
            bool hasAttribute = !string.IsNullOrEmpty(attributeName);
            if (hasElement == hasAttribute)
            {
                errors.Add(new SdfError(ErrorCode.ConversionError,
                    $"Exactly one 'element' or 'attribute' must be specified in <{removeElem.Name}>"));
                return;
            }

            if (hasAttribute)
            {
                var attr = elem.GetAttribute(attributeName!);
                if (attr != null && (!onlyEmpty || string.IsNullOrEmpty(attr.GetAsString())))
                    elem.RemoveAttribute(attributeName!);
            }
            else
            {
                var child = elem.FindElement(elementName!);
                while (child != null)
                {
                    var next = child.GetNextElement(elementName!);
                    bool isEmpty = child.AttributeCount == 0 && child.Children.Count == 0 &&
                                   string.IsNullOrEmpty(child.Value?.GetAsString());
                    if (!onlyEmpty || isEmpty)
                        elem.RemoveChild(child);
                    child = next;
                }
            }
        }

        // ---- <map> ----

        private static void Map(Element elem, Element mapElem, List<SdfError> errors)
        {
            var fromElem = mapElem.FindElement("from");
            var toElem = mapElem.FindElement("to");
            string? fromName = fromElem?.GetAttribute("name")?.GetAsString();
            string? toName = toElem?.GetAttribute("name")?.GetAsString();

            if (fromElem == null || string.IsNullOrEmpty(fromName))
            {
                errors.Add(new SdfError(ErrorCode.ConversionError, "Map: <from> has invalid name attribute"));
                return;
            }
            if (toElem == null || string.IsNullOrEmpty(toName))
            {
                errors.Add(new SdfError(ErrorCode.ConversionError, "Map: <to> has invalid name attribute"));
                return;
            }

            var fromValues = CollectValues(fromElem);
            var toValues = CollectValues(toElem);
            if (fromValues.Count == 0 || toValues.Count == 0)
            {
                errors.Add(new SdfError(ErrorCode.ConversionError,
                    "Map: <from>/<to> must each have at least one <value>"));
                return;
            }

            string? current = GetMapValue(elem, fromName!);
            if (current == null) return;

            for (int i = 0; i < fromValues.Count; i++)
            {
                if (current == fromValues[i])
                {
                    string toValue = toValues[System.Math.Min(i, toValues.Count - 1)];
                    SetMapValue(elem, toName!, toValue);
                    return;
                }
            }
        }

        private static List<string> CollectValues(Element container)
        {
            var values = new List<string>();
            var v = container.FindElement("value");
            while (v != null)
            {
                if (v.Value != null) values.Add(v.Value.GetAsString());
                v = v.GetNextElement("value");
            }
            return values;
        }

        // "/"-separated path, optional "@attrName" leaf.
        private static (Element? Parent, string Leaf) ResolveMapPath(Element elem, string path, bool createMissing)
        {
            var tokens = path.Split('/');
            Element current = elem;
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                var next = current.FindElement(tokens[i]);
                if (next == null)
                {
                    if (!createMissing) return (null, tokens[^1]);
                    next = new Element { Name = tokens[i] };
                    current.InsertElement(next);
                }
                current = next;
            }
            return (current, tokens[^1]);
        }

        private static string? GetMapValue(Element elem, string namePath)
        {
            var (parent, leaf) = ResolveMapPath(elem, namePath, createMissing: false);
            if (parent == null) return null;
            if (leaf.StartsWith('@'))
                return parent.GetAttribute(leaf.Substring(1))?.GetAsString();
            return parent.FindElement(leaf)?.Value?.GetAsString();
        }

        private static void SetMapValue(Element elem, string namePath, string value)
        {
            var (parent, leaf) = ResolveMapPath(elem, namePath, createMissing: true);
            if (parent == null) return;

            if (leaf.StartsWith('@'))
            {
                string attrName = leaf.Substring(1);
                if (!parent.HasAttribute(attrName))
                    parent.AddAttribute(attrName, "string", "", false);
                parent.GetAttribute(attrName)!.SetFromString(value);
            }
            else
            {
                var child = parent.FindElement(leaf);
                if (child == null)
                {
                    child = new Element { Name = leaf };
                    child.AddValue("string", "", false);
                    parent.InsertElement(child);
                }
                else if (child.Value == null)
                {
                    child.AddValue("string", "", false);
                }
                child.Set(value);
            }
        }

        // ---- <move> / <copy> ----

        private static void Move(Element elem, Element opElem, bool copy, List<SdfError> errors)
        {
            var fromElem = opElem.FindElement("from");
            var toElem = opElem.FindElement("to");
            if (fromElem == null)
            {
                errors.Add(new SdfError(ErrorCode.ConversionError, "No 'from' element specified"));
                return;
            }
            if (toElem == null)
            {
                errors.Add(new SdfError(ErrorCode.ConversionError, "No 'to' element specified"));
                return;
            }

            string? fromElementPath = fromElem.GetAttribute("element")?.GetAsString();
            string? fromAttrName = fromElem.GetAttribute("attribute")?.GetAsString();
            string? toElementPath = toElem.GetAttribute("element")?.GetAsString();
            string? toAttrName = toElem.GetAttribute("attribute")?.GetAsString();

            if (!string.IsNullOrEmpty(fromElementPath))
            {
                var fromTokens = fromElementPath.Split("::");
                Element? fromParent = elem;
                for (int i = 0; i < fromTokens.Length - 1; i++)
                {
                    fromParent = fromParent!.FindElement(fromTokens[i]);
                    if (fromParent == null) return; // path doesn't exist — silent no-op
                }
                string fromLeafName = fromTokens[^1];
                var fromLeafElem = fromParent!.FindElement(fromLeafName);
                if (fromLeafElem == null) return;

                if (!string.IsNullOrEmpty(toElementPath))
                {
                    var toTokens = toElementPath.Split("::");
                    int offset = string.IsNullOrEmpty(toAttrName) ? 1 : 0;
                    Element toParent = elem;
                    for (int i = 0; i < toTokens.Length - offset; i++)
                    {
                        var next = toParent.FindElement(toTokens[i]);
                        if (next == null)
                        {
                            next = new Element { Name = toTokens[i] };
                            toParent.InsertElement(next);
                        }
                        toParent = next;
                    }

                    if (!string.IsNullOrEmpty(toAttrName))
                    {
                        string? value = fromLeafElem.Value?.GetAsString();
                        if (value != null)
                        {
                            if (!toParent.HasAttribute(toAttrName))
                                toParent.AddAttribute(toAttrName, "string", "", false);
                            toParent.GetAttribute(toAttrName)!.SetFromString(value);
                        }
                    }
                    else
                    {
                        var clone = fromLeafElem.Clone();
                        clone.Name = toTokens[^1];
                        toParent.InsertElement(clone);
                    }
                }
                else if (!string.IsNullOrEmpty(toAttrName))
                {
                    string? value = fromLeafElem.Value?.GetAsString();
                    if (value != null)
                    {
                        if (!elem.HasAttribute(toAttrName))
                            elem.AddAttribute(toAttrName, "string", "", false);
                        elem.GetAttribute(toAttrName)!.SetFromString(value);
                    }
                }

                if (!copy)
                    fromParent!.RemoveChild(fromLeafElem);
            }
            else if (!string.IsNullOrEmpty(fromAttrName))
            {
                var attr = elem.GetAttribute(fromAttrName);
                if (attr == null) return;
                string value = attr.GetAsString();

                if (!string.IsNullOrEmpty(toElementPath))
                {
                    var newElem = new Element { Name = toElementPath };
                    newElem.AddValue("string", "", false);
                    newElem.Set(value);
                    elem.InsertElement(newElem);
                }
                else if (!string.IsNullOrEmpty(toAttrName))
                {
                    if (!elem.HasAttribute(toAttrName))
                        elem.AddAttribute(toAttrName, "string", "", false);
                    elem.GetAttribute(toAttrName)!.SetFromString(value);
                }

                if (!copy)
                    elem.RemoveAttribute(fromAttrName);
            }
        }

        // ---- <unflatten> ----
        // Reconstructs "::"-flattened submodel names (e.g. "A::B::link") back
        // into real nested <model> elements. The one genuinely non-declarative
        // operation in this DSL; ported function-for-function from upstream
        // Converter::Unflatten / FindNewModelElements / UpdatePose. Deviates
        // from upstream in two ways: SDF_ASSERT invariant violations become
        // recoverable SdfErrors instead of hard aborts, and exact sibling
        // ordering of synthesized elements is not preserved (irrelevant here,
        // since this codebase always looks up children by name, not position).

        private static bool IsNotFlattenedElement(string name) => !FlattenableTags.Contains(name);

        private static bool StartsWithPrefix(string value, string prefix) =>
            value.Length >= prefix.Length && value.Substring(0, prefix.Length) == prefix;

        private static void UpdatePose(Element elem, int childNameIdx, string modelName, List<SdfError> errors)
        {
            var pose = elem.FindElement("pose");
            var relTo = pose?.GetAttribute("relative_to");
            if (relTo != null)
            {
                string poseRelTo = relTo.GetAsString();
                if (StartsWithPrefix(poseRelTo, modelName))
                    relTo.SetFromString(poseRelTo.Substring(childNameIdx));
                else
                    errors.Add(new SdfError(ErrorCode.ConversionError,
                        $"Pose attribute 'relative_to' does not start with {modelName}"));
            }

            var camera = elem.FindElement("camera");
            if (camera != null)
                UpdatePose(camera, childNameIdx, modelName, errors);
        }

        private static void Unflatten(Element elem, List<SdfError> errors)
        {
            Element? firstUnflatModel = null;
            var child = elem.GetFirstElement();
            while (child != null)
            {
                if (ReferenceEquals(child, firstUnflatModel)) break;

                var next = child.GetNextElement();
                var nameAttr = child.GetAttribute("name");

                if (IsNotFlattenedElement(child.Name) || nameAttr == null)
                {
                    child = next;
                    continue;
                }

                string attrName = nameAttr.GetAsString();
                int found = attrName.IndexOf("::", StringComparison.Ordinal);
                if (found < 0)
                {
                    if (child.Name == "model")
                        Unflatten(child, errors);
                    break;
                }

                string newModelName = attrName.Substring(0, found);
                var newModel = new Element { Name = "model" };
                newModel.AddAttribute("name", "string", "", true);
                newModel.GetAttribute("name")!.SetFromString(newModelName);

                if (FindNewModelElements(elem, newModel, found + 2, errors))
                {
                    Unflatten(newModel, errors);
                    elem.InsertElement(newModel);
                    next = elem.GetFirstElement();
                    firstUnflatModel ??= newModel;
                }

                child = next;
            }
        }

        private static bool FindNewModelElements(Element elem, Element newModel, int childNameIdx, List<SdfError> errors)
        {
            bool unflattenedNewModel = false;
            string newModelName = newModel.GetAttribute("name")!.GetAsString();

            var child = elem.GetFirstElement();
            while (child != null)
            {
                var next = child.GetNextElement();
                string elemName = child.Name;
                string elemAttrName = child.GetAttribute("name")?.GetAsString() ?? "";
                bool prefixMatch = StartsWithPrefix(elemAttrName, newModelName);

                if ((elemAttrName.Length == 0 || !prefixMatch || IsNotFlattenedElement(elemName))
                    && elemName != "gripper")
                {
                    child = next;
                    continue;
                }

                string childAttrName = "";
                if (prefixMatch)
                {
                    childAttrName = elemAttrName.Substring(childNameIdx);
                    child.GetAttribute("name")!.SetFromString(childAttrName);
                }

                var poseElem = child.FindElement("pose");
                var poseRelToAttr = poseElem?.GetAttribute("relative_to");
                if (poseRelToAttr != null && StartsWithPrefix(poseRelToAttr.GetAsString(), newModelName))
                    poseRelToAttr.SetFromString(poseRelToAttr.GetAsString().Substring(childNameIdx));

                bool alreadyMoved = false;

                if (elemName == "frame")
                {
                    var attachedToAttr = child.GetAttribute("attached_to");
                    if (attachedToAttr != null)
                    {
                        string attachedTo = attachedToAttr.GetAsString();
                        if (StartsWithPrefix(attachedTo, newModelName))
                        {
                            attachedTo = attachedTo.Substring(childNameIdx);
                            attachedToAttr.SetFromString(attachedTo);

                            if (childAttrName == "__model__")
                            {
                                newModel.AddAttribute("canonical_link", "string", "", false);
                                newModel.GetAttribute("canonical_link")!.SetFromString(attachedTo);

                                elem.RemoveChild(child);
                                if (poseElem != null)
                                {
                                    poseElem.RemoveFromParent();
                                    newModel.InsertElement(poseElem);
                                }
                                alreadyMoved = true;
                            }
                        }
                        else
                        {
                            errors.Add(new SdfError(ErrorCode.ConversionError,
                                $"Frame attribute 'attached_to' does not start with {newModelName}"));
                        }
                    }
                }
                else if (elemName == "link")
                {
                    foreach (var e in child.Children.ToList())
                        UpdatePose(e, childNameIdx, newModelName, errors);
                }
                else if (elemName == "joint")
                {
                    var parentElem = child.FindElement("parent");
                    if (parentElem?.Value != null)
                    {
                        string text = parentElem.Value.GetAsString();
                        if (StartsWithPrefix(text, newModelName))
                            parentElem.Set(text.Substring(childNameIdx));
                        else
                            errors.Add(new SdfError(ErrorCode.ConversionError,
                                $"Joint's <parent> value does not start with {newModelName}"));
                    }

                    var childElem = child.FindElement("child");
                    if (childElem?.Value != null)
                    {
                        string text = childElem.Value.GetAsString();
                        if (StartsWithPrefix(text, newModelName))
                            childElem.Set(text.Substring(childNameIdx));
                        else
                            errors.Add(new SdfError(ErrorCode.ConversionError,
                                $"Joint's <child> value does not start with {newModelName}"));
                    }

                    foreach (var axisName in new[] { "axis", "axis2" })
                    {
                        var expressedIn = child.FindElement(axisName)?.FindElement("xyz")?.GetAttribute("expressed_in");
                        if (expressedIn != null)
                        {
                            string ei = expressedIn.GetAsString();
                            if (StartsWithPrefix(ei, newModelName))
                                expressedIn.SetFromString(ei.Substring(childNameIdx));
                            else
                                errors.Add(new SdfError(ErrorCode.ConversionError,
                                    $"<xyz>'s attribute 'expressed_in' does not start with {newModelName}"));
                        }
                    }

                    var sensor = child.FindElement("sensor");
                    while (sensor != null)
                    {
                        UpdatePose(sensor, childNameIdx, newModelName, errors);
                        sensor = sensor.GetNextElement("sensor");
                    }
                }
                else if (elemName == "gripper")
                {
                    bool hasPrefix = true;
                    var gLink = child.FindElement("gripper_link");
                    while (gLink != null)
                    {
                        if (gLink.Value != null)
                        {
                            string text = gLink.Value.GetAsString();
                            if (!StartsWithPrefix(text, newModelName))
                            {
                                hasPrefix = false;
                                break;
                            }
                            gLink.Set(text.Substring(childNameIdx));
                        }
                        gLink = gLink.GetNextElement("gripper_link");
                    }

                    if (!hasPrefix)
                    {
                        child = next;
                        continue;
                    }

                    var palmLink = child.FindElement("palm_link");
                    if (palmLink?.Value != null)
                    {
                        string text = palmLink.Value.GetAsString();
                        if (StartsWithPrefix(text, newModelName))
                            palmLink.Set(text.Substring(childNameIdx));
                        else
                            errors.Add(new SdfError(ErrorCode.ConversionError,
                                $"Gripper's <palm_link> value does not start with {newModelName}"));
                    }
                }

                unflattenedNewModel = true;
                if (!alreadyMoved)
                {
                    elem.RemoveChild(child);
                    newModel.InsertElement(child);
                }

                child = next;
            }

            return unflattenedNewModel;
        }
    }
}
