// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Devices.Configurations.Sensor
{
    /// <summary>
    /// Channel represents each sensing element connected to a sensor unit.
    /// </summary>
    public class Channel : IChannel
    {
        /// <summary>
        /// A unique identifier that will only refer to a specific sensing element.      
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// The name of the sensing element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description MAY include any additional descriptive information the implementer chooses to include regarding a sensor element.
        /// </summary>
        public string Description { get; set; }

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
    }
}