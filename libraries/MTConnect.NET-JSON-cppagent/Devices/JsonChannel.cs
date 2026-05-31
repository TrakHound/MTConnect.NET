// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a sensor <c>Channel</c> in the
    /// cppagent-compatible shape. Identifies a single signal channel
    /// inside a <c>SensorConfiguration</c>, including its calibration
    /// history. Converts to and from the strongly-typed
    /// <see cref="Channel"/> model.
    /// </summary>
    public class JsonChannel
    {
        /// <summary>
        /// The numeric identifier of the channel within its sensor.
        /// </summary>
        [JsonPropertyName("number")]
        public string Number { get; set; }

        /// <summary>
        /// The descriptive name of the channel.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Date of the last calibration of the channel.
        /// </summary>
        [JsonPropertyName("CalibrationDate")]
        public DateTime? CalibrationDate { get; set; }

        /// <summary>
        /// Date of the next scheduled calibration of the channel.
        /// </summary>
        [JsonPropertyName("NextCalibrationDate")]
        public DateTime? NextCalibrationDate { get; set; }

        /// <summary>
        /// Initials of the technician who performed the most recent
        /// calibration.
        /// </summary>
        [JsonPropertyName("CalibrationInitials")]
        public string CalibrationInitials { get; set; }

        /// <summary>
        /// Free-form description of the channel.
        /// </summary>
        [JsonPropertyName("Description")]
        public string Description { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonChannel() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IChannel"/>.
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

                if (channel.Description != null)
                {
                    Description = channel.Description;
                    //Description = channel.Description.Value;
                }
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IChannel"/>.
        /// </summary>
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