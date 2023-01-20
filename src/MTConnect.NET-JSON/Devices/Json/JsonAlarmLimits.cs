// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Specifications;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonAlarmLimits
    {
        [JsonPropertyName("upperLimit")]
        public double? UpperLimit { get; set; }

        [JsonPropertyName("upperWarning")]
        public double? UpperWarning { get; set; }

        [JsonPropertyName("lowerLimit")]
        public double? LowerLimit { get; set; }

        [JsonPropertyName("lowerWarning")]
        public double? LowerWarning { get; set; }


        public JsonAlarmLimits() { }

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
