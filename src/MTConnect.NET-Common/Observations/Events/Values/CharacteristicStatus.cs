// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    public enum CharacteristicStatus
    {
		/// <summary>
		/// nominal provided without tolerance limits. QIF 3:2018 5.10.2.6
		/// </summary>
		BASIC_OR_THEORETIC_EXACT_DIMENSION,

		/// <summary>
		/// measurement is not within acceptable tolerances.
		/// </summary>
		FAIL,

		/// <summary>
		/// measurement cannot be determined.
		/// </summary>
		INDETERMINATE,

		/// <summary>
		/// measurement cannot be evaluated.
		/// </summary>
		NOT_ANALYZED,

		/// <summary>
		/// measurement is within acceptable tolerances.
		/// </summary>
		PASS,

		/// <summary>
		/// failed, but acceptable constraints achievable by utilizing additional manufacturing processes.
		/// </summary>
		REWORK,

		/// <summary>
		/// measurement is indeterminate due to an equipment failure.
		/// </summary>
		SYSTEM_ERROR,

		/// <summary>
		/// status of measurement cannot be determined.
		/// </summary>
		UNDEFINED
	}
}