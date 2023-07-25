// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
	/// <summary>
	/// UUID of new device added to an MTConnect Agent.
	/// </summary>
	public class DeviceAddedValueObservation : EventValueObservation
    {
		/// <summary>
		/// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
		/// </summary>
		public string Hash
		{
			get => GetValue(ValueKeys.Hash);
			set => AddValue(new ObservationValue(ValueKeys.Hash, value));
		}
	}
}