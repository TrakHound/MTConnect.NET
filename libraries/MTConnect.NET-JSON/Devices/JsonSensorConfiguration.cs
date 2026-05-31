// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Component <c>SensorConfiguration</c>,
    /// recording sensor firmware and calibration metadata along with the
    /// per-channel configuration.
    /// </summary>
    public class JsonSensorConfiguration
    {
        /// <summary>
        /// The firmware version of the sensor.
        /// </summary>
        [JsonPropertyName("firmwareVersion")]
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// The date the sensor was last calibrated.
        /// </summary>
        [JsonPropertyName("calibrationDate")]
        public DateTime? CalibrationDate { get; set; }

        /// <summary>
        /// The date the sensor is next due for calibration.
        /// </summary>
        [JsonPropertyName("nextCalibrationDate")]
        public DateTime? NextCalibrationDate { get; set; }

        /// <summary>
        /// The initials of the person who performed the calibration.
        /// </summary>
        [JsonPropertyName("calibrationInitials")]
        public string CalibrationInitials { get; set; }

        /// <summary>
        /// The configuration of each sensing channel.
        /// </summary>
        [JsonPropertyName("channels")]
        public List<JsonChannel> Channels { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSensorConfiguration() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ISensorConfiguration"/>, converting each channel.
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
                    var channels = new List<JsonChannel>();
                    foreach (var coordinateSystem in configuration.Channels)
                    {
                        channels.Add(new JsonChannel(coordinateSystem));
                    }
                    Channels = channels;
                }
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="SensorConfiguration"/>, converting each channel.
        /// </summary>
        public ISensorConfiguration ToSensorConfiguration()
        {
            var sensorConfiguration = new SensorConfiguration();
            sensorConfiguration.FirmwareVersion = FirmwareVersion;
            sensorConfiguration.CalibrationDate = CalibrationDate;
            sensorConfiguration.NextCalibrationDate = NextCalibrationDate;
            sensorConfiguration.CalibrationInitials = CalibrationInitials;

            // Channels
            if (!Channels.IsNullOrEmpty())
            {
                var channels = new List<IChannel>();
                foreach (var channel in Channels)
                {
                    channels.Add(channel.ToChannel());
                }
                sensorConfiguration.Channels = channels;
            }

            return sensorConfiguration;
        }
    }
}