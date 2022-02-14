// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// An element that can contain descriptive content defining the configuration information for Sensor.
    /// </summary>
    public class SensorConfiguration
    {
        /// <summary>
        /// Version number for the sensor unit as specified by the manufacturer.
        /// </summary>
        [JsonPropertyName("firmwareVersion")]
        public string FirmwareVersion { get; set; }

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

        /// <summary>
        /// When Sensor represents multiple sensing elements, each sensing element is represented by a Channel for the Sensor.
        /// </summary>
        [JsonPropertyName("channels")]
        public List<Channel> Channels { get; set; }
    }
}
