// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Text.Json.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// Channel represents each sensing element connected to a sensor unit.
    /// </summary>
    public class Channel : IChannel
    {
        /// <summary>
        /// A unique identifier that will only refer to a specific sensing element.      
        /// </summary>
        [JsonPropertyName("number")]
        public string Number { get; set; }

        /// <summary>
        /// The name of the sensing element.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description MAY include any additional descriptive information the implementer chooses to include regarding a sensor element.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Date upon which the sensor unit was last calibrated.
        /// </summary>
        [JsonPropertyName("calibrationDate")]
        public DateTime CalibrationDate { get; set; }

        /// <summary>
        /// Date upon which the sensor unit is next scheduled to be calibrated.
        /// </summary>
        [JsonPropertyName("nextCalibrationDate")]
        public DateTime NextCalibrationDate { get; set; }

        /// <summary>
        /// The initials of the person verifying the validity of the calibration data
        /// </summary>
        [JsonPropertyName("calibrationInitials")]
        public string CalibrationInitials { get; set; }
    }
}
