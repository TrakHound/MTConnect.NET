// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
	/// <summary>
	/// Pass/fail result of the measurement.
	/// </summary>
	public static class CharacteristicStatusDescriptions
	{
        public const string BASIC_OR_THEORETIC_EXACT_DIMENSION = "nominal provided without tolerance limits. QIF 3:2018 5.10.2.6";

        public const string FAIL = "measurement is not within acceptable tolerances.";

		public const string INDETERMINATE = "measurement cannot be determined.";

		public const string NOT_ANALYZED = "measurement cannot be evaluated.";

		public const string PASS = "measurement is within acceptable tolerances.";

		public const string REWORK = "failed, but acceptable constraints achievable by utilizing additional manufacturing processes.";

		public const string SYSTEM_ERROR = "measurement is indeterminate due to an equipment failure.";

		public const string UNDEFINED = "status of measurement cannot be determined.";


		public static string Get(CharacteristicStatus value)
        {
            switch (value)
            {
                case CharacteristicStatus.BASIC_OR_THEORETIC_EXACT_DIMENSION: return BASIC_OR_THEORETIC_EXACT_DIMENSION;
                case CharacteristicStatus.FAIL: return FAIL;
                case CharacteristicStatus.INDETERMINATE: return INDETERMINATE;
                case CharacteristicStatus.NOT_ANALYZED: return NOT_ANALYZED;
                case CharacteristicStatus.PASS: return PASS;
                case CharacteristicStatus.REWORK: return REWORK;
                case CharacteristicStatus.SYSTEM_ERROR: return SYSTEM_ERROR;
                case CharacteristicStatus.UNDEFINED: return UNDEFINED;
            }

            return null;
        }
    }
}