// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a cutting tool
    /// <c>ProcessSpindleSpeed</c>, the spindle speed limits the tool is
    /// intended to operate within. Mirrors the on-the-wire shape so the JSON
    /// serializer can read and write it, then converts to and from the
    /// strongly-typed <see cref="ProcessSpindleSpeed"/> model.
    /// </summary>
    public class JsonProcessSpindleSpeed
    {
        /// <summary>
        /// The maximum spindle speed the tool may be operated at.
        /// </summary>
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// The minimum spindle speed the tool may be operated at.
        /// </summary>
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// The nominal (recommended) spindle speed for the tool.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The spindle speed value.
        /// </summary>
        [JsonPropertyName("value")]
        public double? Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonProcessSpindleSpeed() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IProcessSpindleSpeed"/>.
        /// </summary>
        public JsonProcessSpindleSpeed(IProcessSpindleSpeed processSpindleSpeed)
        {
            if (processSpindleSpeed != null)
            {
                Maximum = processSpindleSpeed.Maximum;
                Minimum = processSpindleSpeed.Minimum;
                Nominal = processSpindleSpeed.Nominal;
                Value = processSpindleSpeed.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IProcessSpindleSpeed"/>.
        /// </summary>
        public IProcessSpindleSpeed ToProcessSpindleSpeed()
        {
            var processSpindleSpeed = new ProcessSpindleSpeed();
            processSpindleSpeed.Maximum = Maximum;
            processSpindleSpeed.Minimum = Minimum;
            processSpindleSpeed.Nominal = Nominal;
            processSpindleSpeed.Value = Value;
            return processSpindleSpeed;
        }
    }
}