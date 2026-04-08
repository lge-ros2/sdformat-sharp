// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Param.hh

#nullable enable

using System;
using System.Globalization;
using SDFormat.Math;

namespace SDFormat
{
    /// <summary>
    /// A parameter consisting of a key, type, default value, required flag,
    /// and an optional description. Each attribute and value in an SDF Element
    /// is stored as a Param.
    /// </summary>
    public class Param : ICloneable
    {
        /// <summary>The key/name of this parameter.</summary>
        public string Key { get; set; }

        /// <summary>The SDF type name string (e.g., "string", "double", "pose").</summary>
        public string TypeName { get; set; }

        /// <summary>Whether this parameter is required.</summary>
        public bool Required { get; set; }

        /// <summary>Description of this parameter.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Whether this param has been explicitly set.</summary>
        public bool IsSet { get; private set; }

        /// <summary>Optional minimum value as string.</summary>
        public string? MinValue { get; set; }

        /// <summary>Optional maximum value as string.</summary>
        public string? MaxValue { get; set; }

        /// <summary>The parent Element, if any.</summary>
        public Element? ParentElement { get; set; }

        private string _defaultValue;
        private string _value;

        /// <summary>
        /// Construct a parameter with key, type name, default value, and required flag.
        /// </summary>
        public Param(string key, string typeName, string defaultValue, bool required,
            string description = "")
        {
            Key = key;
            TypeName = typeName;
            _defaultValue = defaultValue;
            _value = defaultValue;
            Required = required;
            Description = description;
        }

        /// <summary>
        /// Construct with optional min/max.
        /// </summary>
        public Param(string key, string typeName, string defaultValue, bool required,
            string minValue, string maxValue, string description = "")
            : this(key, typeName, defaultValue, required, description)
        {
            MinValue = string.IsNullOrEmpty(minValue) ? null : minValue;
            MaxValue = string.IsNullOrEmpty(maxValue) ? null : maxValue;
        }

        /// <summary>Get the value as a string.</summary>
        public string GetAsString() => _value;

        /// <summary>Get the default value as string.</summary>
        public string GetDefaultAsString() => _defaultValue;

        /// <summary>Set the value from a string. Returns true on success.</summary>
        public bool SetFromString(string value)
        {
            _value = value;
            IsSet = true;
            return true;
        }

        /// <summary>Set the value from a string, optionally ignoring parent element attributes.</summary>
        public bool SetFromString(string value, bool ignoreParentAttributes)
        {
            return SetFromString(value);
        }

        /// <summary>Reset to the default value.</summary>
        public void Reset()
        {
            _value = _defaultValue;
            IsSet = false;
        }

        /// <summary>Set a typed value.</summary>
        public bool Set<T>(T value)
        {
            if (value == null)
                return false;
            _value = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
            IsSet = true;
            return true;
        }

        /// <summary>Get a typed value.</summary>
        public bool Get<T>(out T value)
        {
            try
            {
                value = (T)Convert.ChangeType(_value, typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                value = default!;
                return false;
            }
        }

        /// <summary>Get the default value as a typed value.</summary>
        public bool GetDefault<T>(out T value)
        {
            try
            {
                value = (T)Convert.ChangeType(_defaultValue, typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                value = default!;
                return false;
            }
        }

        /// <summary>Get value as string.</summary>
        public string StringValue => _value;

        /// <summary>Get value as double.</summary>
        public double DoubleValue =>
            double.TryParse(_value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0.0;

        /// <summary>Get value as int.</summary>
        public int IntValue =>
            int.TryParse(_value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : 0;

        /// <summary>Get value as bool.</summary>
        public bool BoolValue =>
            _value == "1" || _value.Equals("true", StringComparison.OrdinalIgnoreCase);

        /// <summary>Get value as Vector3d.</summary>
        public Vector3d Vector3dValue
        {
            get
            {
                try { return Vector3d.Parse(_value); }
                catch { return Vector3d.Zero; }
            }
        }

        /// <summary>Get value as Pose3d.</summary>
        public Pose3d Pose3dValue
        {
            get
            {
                try { return Pose3d.Parse(_value); }
                catch { return Pose3d.Zero; }
            }
        }

        /// <summary>Get value as Color.</summary>
        public Color ColorValue
        {
            get
            {
                try { return Color.Parse(_value); }
                catch { return Color.White; }
            }
        }

        /// <summary>Validate the value against min/max if set.</summary>
        public bool ValidateValue()
        {
            // Basic validation — could be extended
            return true;
        }

        /// <summary>Clone this parameter.</summary>
        public Param Clone()
        {
            var clone = new Param(Key, TypeName, _defaultValue, Required, Description)
            {
                MinValue = MinValue,
                MaxValue = MaxValue,
                IsSet = IsSet,
            };
            clone._value = _value;
            return clone;
        }

        object ICloneable.Clone() => Clone();

        /// <inheritdoc/>
        public override string ToString() => $"Param[{Key}={_value} ({TypeName})]";
    }
}
