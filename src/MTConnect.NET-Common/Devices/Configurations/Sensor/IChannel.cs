// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Devices.Configurations.Sensor
{
    /// <summary>
    /// Channel represents each sensing element connected to a sensor unit.
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// A unique identifier that will only refer to a specific sensing element.      
        /// </summary>
        string Number { get; }

        /// <summary>
        /// The name of the sensing element.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description MAY include any additional descriptive information the implementer chooses to include regarding a sensor element.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Date upon which the sensor unit was last calibrated.
        /// </summary>
        DateTime CalibrationDate { get; }

        /// <summary>
        /// Date upon which the sensor unit is next scheduled to be calibrated.
        /// </summary>
        DateTime NextCalibrationDate { get; }

        /// <summary>
        /// The initials of the person verifying the validity of the calibration data
        /// </summary>
        string CalibrationInitials { get; }
    }
}