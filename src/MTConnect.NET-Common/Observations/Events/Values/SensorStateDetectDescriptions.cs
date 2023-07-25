// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
	/// <summary>
	/// Detection result of a sensor where the state is Detect.
	/// </summary>
	public static class SensorStateDetectDescriptions
    {
		/// <summary>
		/// Sensor is active and the threshold has been met.
		/// </summary>
		public const string DETECTED = "Sensor is active and the threshold has been met.";

        /// <summary>
        /// Power has been removed and the spindle cannot be operated.
        /// </summary>
        public const string NOT_DETECTED = "Sensor is active and ready but the threshold has not been met.";

		/// <summary>
		/// Sensor is active, but the state cannot be determined. Note: unknown covers situations where the sensor reading is unstable.
		/// </summary>
		public const string UNKNOWN = "Sensor is active, but the state cannot be determined. Note: unknown covers situations where the sensor reading is unstable.";


		public static string Get(SensorStateDetect value)
        {
            switch (value)
            {
                case SensorStateDetect.DETECTED: return DETECTED;
                case SensorStateDetect.NOT_DETECTED: return NOT_DETECTED;
                case SensorStateDetect.UNKNOWN: return UNKNOWN;
            }

            return null;
        }
    }
}