// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_1DD02014_D949_43cc_A79F_FF2C0AF0DFBE

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Configuration for a Sensor.
    /// </summary>
    public class SensorConfiguration : ISensorConfiguration
    {
        public const string DescriptionText = "Configuration for a Sensor.";


        /// <summary>
        /// Date upon which the sensor unit was last calibrated.
        /// </summary>
        public System.DateTime? CalibrationDate { get; set; }
        
        /// <summary>
        /// The initials of the person verifying the validity of the calibration data.
        /// </summary>
        public string CalibrationInitials { get; set; }
        
        /// <summary>
        /// Sensing element of a Sensor.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.Configurations.IChannel> Channels { get; set; }
        
        /// <summary>
        /// Version number for the sensor unit as specified by the manufacturer.
        /// </summary>
        public string FirmwareVersion { get; set; }
        
        /// <summary>
        /// Date upon which the sensor unit is next scheduled to be calibrated.
        /// </summary>
        public System.DateTime? NextCalibrationDate { get; set; }
    }
}