// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a single <c>CutterStatus</c>
    /// enumeration value (NEW, USED, RECONDITIONED, EXPIRED, etc.),
    /// wrapped as an object so the cppagent shape can carry the value
    /// under a <c>value</c> property.
    /// </summary>
    public class JsonCutterStatus
    {
        /// <summary>
        /// The serialized cutter status enumeration name.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCutterStatus() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="CutterStatusType"/> value.
        /// </summary>
        public JsonCutterStatus(CutterStatusType cutterStatus)
        {
            Value = cutterStatus.ToString();
        }


        /// <summary>
        /// Parses the serialized status back into a
        /// <see cref="CutterStatusType"/>.
        /// </summary>
        public CutterStatusType ToCutterStatus()
        {
            return Value.ConvertEnum<CutterStatusType>();
        }
    }
}