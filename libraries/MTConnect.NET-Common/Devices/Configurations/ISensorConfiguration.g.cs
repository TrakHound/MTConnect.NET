// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Configuration for a Sensor.
    /// </summary>
    public interface ISensorConfiguration
    {
        /// <summary>
        /// Date upon which the sensor unit was last calibrated.
        /// </summary>
        System.DateTime? CalibrationDate { get; }
        
        /// <summary>
        /// The initials of the person verifying the validity of the calibration data.
        /// </summary>
        string CalibrationInitials { get; }
        
        /// <summary>
        /// Sensing element of a Sensor.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.Configurations.IChannel> Channels { get; }
        
        /// <summary>
        /// Version number for the sensor unit as specified by the manufacturer.
        /// </summary>
        string FirmwareVersion { get; }
        
        /// <summary>
        /// Date upon which the sensor unit is next scheduled to be calibrated.
        /// </summary>
        System.DateTime? NextCalibrationDate { get; }
    }
}