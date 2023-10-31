// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_82C852E8_47AD_4b8c_804D_F38FCA663918

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Sensing element of a Sensor.
    /// </summary>
    public class Channel : IChannel
    {
        public const string DescriptionText = "Sensing element of a Sensor.";


        /// <summary>
        /// Date upon which the sensor unit was last calibrated to the sensor element.
        /// </summary>
        public System.DateTime? CalibrationDate { get; set; }
        
        /// <summary>
        /// The initials of the person verifying the validity of the calibration data.
        /// </summary>
        public string CalibrationInitials { get; set; }
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        public MTConnect.Devices.IDescription Description { get; set; }
        
        /// <summary>
        /// Name of the specific sensing element.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Date upon which the sensor element is next scheduled to be calibrated with the sensor unit.
        /// </summary>
        public System.DateTime? NextCalibrationDate { get; set; }
        
        /// <summary>
        /// Unique identifier that will only refer to a specific sensing element.
        /// </summary>
        public string Number { get; set; }
    }
}