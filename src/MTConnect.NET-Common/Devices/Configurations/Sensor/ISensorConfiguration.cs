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
    public interface ISensorConfiguration
    {
        /// <summary>
        /// Version number for the sensor unit as specified by the manufacturer.
        /// </summary>
        string FirmwareVersion { get; }

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

        /// <summary>
        /// When Sensor represents multiple sensing elements, each sensing element is represented by a Channel for the Sensor.
        /// </summary>
        List<Channel> Channels { get; }
    }
}
