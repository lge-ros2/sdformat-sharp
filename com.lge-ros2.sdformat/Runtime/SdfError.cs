// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Error.hh

#nullable enable

using System.Text;

namespace SdFormat
{
    /// <summary>
    /// Defines a single SDF error with an error code, message, and optional
    /// file path / line number context.
    /// </summary>
    public class SdfError
    {
        /// <summary>Gets or sets the error code.</summary>
        public ErrorCode Code { get; set; }

        /// <summary>Gets or sets the error message.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional file path where the error occurred.</summary>
        public string? FilePath { get; set; }

        /// <summary>Gets or sets the optional line number where the error occurred.</summary>
        public int? LineNumber { get; set; }

        /// <summary>Gets or sets the optional XML path of the error.</summary>
        public string? XmlPath { get; set; }

        /// <summary>Default constructor (no error).</summary>
        public SdfError()
        {
            Code = ErrorCode.None;
        }

        /// <summary>Construct with an error code and message.</summary>
        public SdfError(ErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>Construct with an error code, message, and file path.</summary>
        public SdfError(ErrorCode code, string message, string filePath)
        {
            Code = code;
            Message = message;
            FilePath = filePath;
        }

        /// <summary>Construct with an error code, message, file path, and line number.</summary>
        public SdfError(ErrorCode code, string message, string filePath, int lineNumber)
        {
            Code = code;
            Message = message;
            FilePath = filePath;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Returns true if this error represents an actual error condition
        /// (i.e. Code is not None).
        /// </summary>
        public bool HasError => Code != ErrorCode.None;

        /// <summary>
        /// Implicit bool conversion. Returns true if error code is not None.
        /// </summary>
        public static implicit operator bool(SdfError error) => error.HasError;

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Error Code {(int)Code}: ");

            if (!string.IsNullOrEmpty(Message))
                sb.Append($"Msg: {Message}");

            if (FilePath != null)
                sb.Append($" [File: {FilePath}");

            if (LineNumber.HasValue)
                sb.Append($", Line: {LineNumber.Value}");

            if (FilePath != null)
                sb.Append(']');

            if (XmlPath != null)
                sb.Append($" [XmlPath: {XmlPath}]");

            return sb.ToString();
        }
    }
}
