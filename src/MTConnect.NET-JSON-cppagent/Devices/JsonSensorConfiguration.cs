// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonSensorConfiguration
    {
        [JsonPropertyName("FirmwareVersion")]
        public string FirmwareVersion { get; set; }

        [JsonPropertyName("CalibrationDate")]
        public DateTime? CalibrationDate { get; set; }

        [JsonPropertyName("NextCalibrationDate")]
        public DateTime? NextCalibrationDate { get; set; }

        [JsonPropertyName("CalibrationInitials")]
        public string CalibrationInitials { get; set; }

        [JsonPropertyName("Channels")]
        public JsonChannels Channels { get; set; }


        public JsonSensorConfiguration() { }

        public JsonSensorConfiguration(ISensorConfiguration configuration)
        {
            if (configuration != null)
            {
                FirmwareVersion = configuration.FirmwareVersion;
                CalibrationDate = configuration.CalibrationDate;
                NextCalibrationDate = configuration.NextCalibrationDate;
                CalibrationInitials = configuration.CalibrationInitials;

                // Channels
                if (!configuration.Channels.IsNullOrEmpty())
                {
                    Channels = new JsonChannels(configuration.Channels);
                }
            }
        }


        public ISensorConfiguration ToSensorConfiguration()
        {
            var sensorConfiguration = new SensorConfiguration();
            sensorConfiguration.FirmwareVersion = FirmwareVersion;
            sensorConfiguration.CalibrationDate = CalibrationDate;
            sensorConfiguration.NextCalibrationDate = NextCalibrationDate;
            sensorConfiguration.CalibrationInitials = CalibrationInitials;

            if (Channels != null && !Channels.Channels.IsNullOrEmpty())
            {
                var channels = new List<IChannel>();
                foreach (var channel in Channels.Channels)
                {
                    channels.Add(channel.ToChannel());
                }
                sensorConfiguration.Channels = channels;
            }

            //// Channels
            //if (!Channels.IsNullOrEmpty())
            //{
            //    var channels = new List<IChannel>();
            //    foreach (var channel in Channels)
            //    {
            //        channels.Add(channel.ToChannel());
            //    }
            //    sensorConfiguration.Channels = channels;
            //}

            return sensorConfiguration;
        }
    }
}