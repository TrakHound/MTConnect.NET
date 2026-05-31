// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for the <c>AlarmLimits</c> of a
    /// Specification, carrying the upper/lower alarm and warning bounds.
    /// </summary>
    public class JsonAlarmLimits
    {
        /// <summary>
        /// The upper alarm bound.
        /// </summary>
        [JsonPropertyName("upperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper warning bound.
        /// </summary>
        [JsonPropertyName("upperWarning")]
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The lower alarm bound.
        /// </summary>
        [JsonPropertyName("lowerLimit")]
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower warning bound.
        /// </summary>
        [JsonPropertyName("lowerWarning")]
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
        /// Converts this surrogate to a strongly-typed <see cref="AlarmLimits"/>.
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