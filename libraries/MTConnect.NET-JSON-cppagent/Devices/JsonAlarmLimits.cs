// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for the <c>AlarmLimits</c>
    /// sub-element of a <c>ProcessSpecification</c> in the
    /// cppagent-compatible shape. Carries the engineering bounds whose
    /// breach triggers an alarm condition, plus the early-warning
    /// thresholds. Converts to and from the strongly-typed
    /// <see cref="AlarmLimits"/> model.
    /// </summary>
    public class JsonAlarmLimits
    {
        /// <summary>
        /// The upper alarm limit; values above this should raise an
        /// alarm.
        /// </summary>
        [JsonPropertyName("UpperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper warning threshold before the upper alarm limit.
        /// </summary>
        [JsonPropertyName("UpperWarning")]
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The lower alarm limit; values below this should raise an
        /// alarm.
        /// </summary>
        [JsonPropertyName("LowerLimit")]
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower warning threshold before the lower alarm limit.
        /// </summary>
        [JsonPropertyName("LowerWarning")]
        public double? LowerWarning { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonAlarmLimits() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IAlarmLimits"/>.
        /// </summary>
        public JsonAlarmLimits(IAlarmLimits alarmLimits)
        {
            if (alarmLimits != null)
            {
                UpperLimit = alarmLimits.UpperLimit;
                UpperWarning = alarmLimits.UpperWarning;
                LowerLimit = alarmLimits.LowerLimit;
                LowerWarning = alarmLimits.LowerWarning;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IAlarmLimits"/>.
        /// </summary>
        public IAlarmLimits ToAlarmLimits()
        {
            var alarmLimits = new AlarmLimits();
            alarmLimits.UpperLimit = UpperLimit;
            alarmLimits.UpperWarning = UpperWarning;
            alarmLimits.LowerLimit = LowerLimit;
            alarmLimits.LowerWarning = LowerWarning;
            return alarmLimits;
        }
    }
}