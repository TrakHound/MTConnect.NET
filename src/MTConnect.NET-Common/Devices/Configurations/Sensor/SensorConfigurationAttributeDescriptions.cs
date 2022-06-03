// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Sensor
{
    public static class SensorConfigurationAttributeDescriptions
    {
        /// <summary>
        /// Version number for the sensor unit as specified by the manufacturer.
        /// </summary>
        public const string FirmwareVersion = "Version number for the sensor unit as specified by the manufacturer.";

        /// <summary>
        /// Date upon which the sensor unit was last calibrated.
        /// </summary>
        public const string CalibrationDate = "Date upon which the sensor unit was last calibrated.";

        /// <summary>
        /// Date upon which the sensor unit is next scheduled to be calibrated.
        /// </summary>
        public const string NextCalibrationDate = "Date upon which the sensor unit is next scheduled to be calibrated.";

        /// <summary>
        /// The initials of the person verifying the validity of the calibration data
        /// </summary>
        public const string CalibrationInitials = "The initials of the person verifying the validity of the calibration data";
    }
}
