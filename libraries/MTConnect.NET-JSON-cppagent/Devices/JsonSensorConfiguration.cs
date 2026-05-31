// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a
    /// <c>SensorConfiguration</c> in the cppagent-compatible shape.
    /// Carries the sensor firmware revision and calibration history plus
    /// a typed container of channel definitions. Converts to and from
    /// the strongly-typed <see cref="SensorConfiguration"/> model.
    /// </summary>
    public class JsonSensorConfiguration
    {
        /// <summary>
        /// The firmware version of the sensor.
        /// </summary>
        [JsonPropertyName("FirmwareVersion")]
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// Date of the last sensor-level calibration.
        /// </summary>
        [JsonPropertyName("CalibrationDate")]
        public DateTime? CalibrationDate { get; set; }

        /// <summary>
        /// Date of the next scheduled sensor-level calibration.
        /// </summary>
        [JsonPropertyName("NextCalibrationDate")]
        public DateTime? NextCalibrationDate { get; set; }

        /// <summary>
        /// Initials of the technician who performed the most recent
        /// sensor-level calibration.
        /// </summary>
        [JsonPropertyName("CalibrationInitials")]
        public string CalibrationInitials { get; set; }

        /// <summary>
        /// Per-channel calibration data carried inside a typed
        /// channel container.
        /// </summary>
        [JsonPropertyName("Channels")]
        public JsonChannels Channels { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSensorConfiguration() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ISensorConfiguration"/>, suppressing the channel
        /// container when the source has no channels.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISensorConfiguration"/>, flattening the typed
        /// channel container back into a uniform channel list.
        /// </summary>
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