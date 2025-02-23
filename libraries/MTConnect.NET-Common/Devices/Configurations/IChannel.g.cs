// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Sensing element of a Sensor.
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// Date upon which the sensor unit was last calibrated to the sensor element.
        /// </summary>
        System.DateTime? CalibrationDate { get; }
        
        /// <summary>
        /// The initials of the person verifying the validity of the calibration data.
        /// </summary>
        string CalibrationInitials { get; }
        
        /// <summary>
        /// Textual description for Channel.
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Name of the specific sensing element.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Date upon which the sensor element is next scheduled to be calibrated with the sensor unit.
        /// </summary>
        System.DateTime? NextCalibrationDate { get; }
        
        /// <summary>
        /// Unique identifier that will only refer to a specific sensing element.
        /// </summary>
        string Number { get; }
    }
}