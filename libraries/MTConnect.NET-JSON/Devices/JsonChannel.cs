// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a single sensing <c>Channel</c> within
    /// a SensorConfiguration, carrying its identification and per-channel
    /// calibration metadata.
    /// </summary>
    public class JsonChannel
    {
        /// <summary>
        /// The channel number that identifies it within the sensor.
        /// </summary>
        [JsonPropertyName("number")]
        public string Number { get; set; }

        /// <summary>
        /// The optional human-readable channel name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The date the channel was last calibrated.
        /// </summary>
        [JsonPropertyName("calibrationDate")]
        public DateTime? CalibrationDate { get; set; }

        /// <summary>
        /// The date the channel is next due for calibration.
        /// </summary>
        [JsonPropertyName("nextCalibrationDate")]
        public DateTime? NextCalibrationDate { get; set; }

        /// <summary>
        /// The initials of the person who performed the calibration.
        /// </summary>
        [JsonPropertyName("calibrationInitials")]
        public string CalibrationInitials { get; set; }

        /// <summary>
        /// The free-text channel description (carried as a string from v2.5).
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonChannel() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed <see cref="IChannel"/>.
        /// </summary>
        public JsonChannel(IChannel channel)
        {
            if (channel != null)
            {
                Number = channel.Number;
                Name = channel.Name;
                CalibrationDate = channel.CalibrationDate;
                NextCalibrationDate = channel.NextCalibrationDate;
                CalibrationInitials = channel.CalibrationInitials;

                if (channel.Description != null) Description = channel.Description; // v2.5
                //if (channel.Description != null) Description = new JsonDescription(channel.Description);
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Channel"/>.
        /// </summary>
        public IChannel ToChannel()
        {
            var channel = new Channel();
            channel.Number = Number;
            channel.Name = Name;
            channel.CalibrationDate = CalibrationDate;
            channel.NextCalibrationDate = NextCalibrationDate;
            channel.CalibrationInitials = CalibrationInitials;
            if (Description != null) channel.Description = Description; // v2.5
            //if (Description != null) channel.Description = Description.ToDescription();
            return channel;
        }
    }
}