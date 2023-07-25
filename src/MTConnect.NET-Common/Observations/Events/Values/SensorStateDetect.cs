// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
	/// <summary>
	/// Detection result of a sensor where the state is Detect.
	/// </summary>
	public enum SensorStateDetect
    {
		/// <summary>
		/// Sensor is active and the threshold has been met.
		/// </summary>
		DETECTED,

		/// <summary>
		/// Sensor is active and ready but the threshold has not been met.
		/// </summary>
		NOT_DETECTED,

		/// <summary>
		/// Sensor is active, but the state cannot be determined. Note: unknown covers situations where the sensor reading is unstable.
		/// </summary>
		UNKNOWN
	}
}