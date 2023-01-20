// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// An indication of whether the end of a piece of bar stock being feed by a bar feeder has been reached.
    /// </summary>
    public class EndOfBarDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "END_OF_BAR";
        public const string NameId = "eob";
        public new const string DescriptionText = "An indication of whether the end of a piece of bar stock being feed by a bar feeder has been reached.";

        public override string TypeDescription => DescriptionText;

        public override Version MinimumVersion => MTConnectVersions.Version13;

        public enum SubTypes
        {
            /// <summary>
            /// Specific applications MAY reference one or more locations on a piece of bar stock as the indication for the END_OF_BAR.
            /// The main or most important location MUST be designated as the PRIMARY indication for the END_OF_BAR.
            /// </summary>
            PRIMARY,

            /// <summary>
            /// When multiple locations on a piece of bar stock are referenced as the indication for the END_OF_BAR, the additional location(s) MUST be designated as AUXILIARY indication(s) for the END_OF_BAR.
            /// </summary>
            AUXILIARY
        }


        public EndOfBarDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public EndOfBarDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        protected override ValidationResult OnValidation(Version mtconnectVersion, IObservationInput observation)
        {
            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                // Get the Result Value for the Observation
                var result = observation.GetValue(ValueKeys.Result);
                if (result != null)
                {
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(Observations.Events.Values.EndOfBar));
                    foreach (var validValue in validValues)
                    {
                        if (result == validValue.ToString())
                        {
                            return new ValidationResult(true);
                        }
                    }

                    return new ValidationResult(false, "'" + result + "' is not a valid value");
                }
                else
                {
                    return new ValidationResult(false, "No Result is specified for the Observation");
                }
            }

            return new ValidationResult(false, "No Observation is Specified");
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.PRIMARY: return "Specific applications MAY reference one or more locations on a piece of bar stock as the indication for the END_OF_BAR. The main or most important location MUST be designated as the PRIMARY indication for the END_OF_BAR.";
                case SubTypes.AUXILIARY: return "When multiple locations on a piece of bar stock are referenced as the indication for the END_OF_BAR, the additional location(s) MUST be designated as AUXILIARY indication(s) for the END_OF_BAR.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.PRIMARY: return "primary";
                case SubTypes.AUXILIARY: return "aux";
            }

            return null;
        }
    }
}
