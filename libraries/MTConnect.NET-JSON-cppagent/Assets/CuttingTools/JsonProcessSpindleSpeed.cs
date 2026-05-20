// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a CuttingTool
    /// <c>ProcessSpindleSpeed</c> in the cppagent-compatible shape.
    /// Carries the spindle-speed envelope (minimum, maximum, nominal)
    /// the tool should be operated within, plus an optional current
    /// value. Converts to and from the strongly-typed
    /// <see cref="ProcessSpindleSpeed"/> model.
    /// </summary>
    public class JsonProcessSpindleSpeed
    {
        /// <summary>
        /// Upper bound of the recommended spindle-speed envelope.
        /// </summary>
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// Lower bound of the recommended spindle-speed envelope.
        /// </summary>
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// Nominal (target) spindle speed.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The current operating spindle speed, when reported.
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