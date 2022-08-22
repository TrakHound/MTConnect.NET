// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MTConnect.Devices.Configurations.Sensor
{
    /// <summary>
    /// An element that can contain descriptive content defining the configuration information for Sensor.
    /// </summary>
    public class SensorConfiguration : ISensorConfiguration
    {
        /// <summary>
        /// Version number for the sensor unit as specified by the manufacturer.
        /// </summary>
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// Date upon which the sensor unit was last calibrated.
        /// </summary>
        public DateTime CalibrationDate { get; set; }

        /// <summary>
        /// Date upon which the sensor unit is next scheduled to be calibrated.
        /// </summary>
        public DateTime NextCalibrationDate { get; set; }

        /// <summary>
        /// The initials of the person verifying the validity of the calibration data
        /// </summary>
        public string CalibrationInitials { get; set; }

        /// <summary>
        /// When Sensor represents multiple sensing elements, each sensing element is represented by a Channel for the Sensor.
        /// </summary>
        public IEnumerable<IChannel> Channels { get; set; }
    }
}
