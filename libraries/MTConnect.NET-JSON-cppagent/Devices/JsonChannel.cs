// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
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

        [JsonPropertyName("CalibrationDate")]
        public DateTime? CalibrationDate { get; set; }

        [JsonPropertyName("NextCalibrationDate")]
        public DateTime? NextCalibrationDate { get; set; }

        [JsonPropertyName("CalibrationInitials")]
        public string CalibrationInitials { get; set; }

        [JsonPropertyName("Description")]
        public string Description { get; set; }


        public JsonChannel() { }

        public JsonChannel(IChannel channel)
        {
            if (channel != null)
            {
                Number = channel.Number;
                Name = channel.Name;
                CalibrationDate = channel.CalibrationDate;
                NextCalibrationDate = channel.NextCalibrationDate;
                CalibrationInitials = channel.CalibrationInitials;

                if (channel.Description != null)
                {
                    Description = channel.Description;
                    //Description = channel.Description.Value;
                }
            }
        }


        public IChannel ToChannel()
        {
            var channel = new Channel();
            channel.Number = Number;
            channel.Name = Name;
            channel.CalibrationDate = CalibrationDate;
            channel.NextCalibrationDate = NextCalibrationDate;
            channel.CalibrationInitials = CalibrationInitials;

            if (Description != null)
            {
                //var description = new Description();
                //description.Value = Description;
                //channel.Description = description;
                channel.Description = Description;
            }

            return channel;
        }
    }
}