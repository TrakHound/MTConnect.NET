// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Sensor
{
    public static class ChannelAttributeDescriptions
    {
        /// <summary>
        /// A unique identifier that will only refer to a specific sensing element.      
        /// </summary>
        public const string Number = "A unique identifier that will only refer to a specific sensing element.    ";

        /// <summary>
        /// The name of the sensing element.
        /// </summary>
        public const string Name = "The name of the sensing element.";

        /// <summary>
        /// Description MAY include any additional descriptive information the implementer chooses to include regarding a sensor element.
        /// </summary>
        public const string Description = "Description MAY include any additional descriptive information the implementer chooses to include regarding a sensor element.";

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
