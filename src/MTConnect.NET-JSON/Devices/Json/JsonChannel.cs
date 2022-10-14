// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Sensor;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonChannel
    {
        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("calibrationDate")]
        public DateTime CalibrationDate { get; set; }

        [JsonPropertyName("nextCalibrationDate")]
        public DateTime NextCalibrationDate { get; set; }

        [JsonPropertyName("calibrationInitials")]
        public string CalibrationInitials { get; set; }


        public JsonChannel() { }

        public JsonChannel(IChannel channel)
        {
            if (channel != null)
            {
                Number = channel.Number;
                Name = channel.Name;
                Description = channel.Description;
                CalibrationDate = channel.CalibrationDate;
                NextCalibrationDate = channel.NextCalibrationDate;
                CalibrationInitials = channel.CalibrationInitials;
            }
        }


        public IChannel ToChannel()
        {
            var channel = new Channel();
            channel.Number = Number;
            channel.Name = Name;
            channel.Description = Description;
            channel.CalibrationDate = CalibrationDate;
            channel.NextCalibrationDate = NextCalibrationDate;
            channel.CalibrationInitials = CalibrationInitials;
            return channel;
        }
    }
}
