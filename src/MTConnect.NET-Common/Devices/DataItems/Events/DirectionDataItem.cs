// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The direction of motion.
    /// </summary>
    public class DirectionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DIRECTION";
        public const string NameId = "direction";
        public new const string DescriptionText = "The direction of motion.";

        public override string TypeDescription => DescriptionText;

        public enum SubTypes
        {
            /// <summary>
            /// The direction of rotary motion using the right-hand rule convention.
            /// </summary>
            ROTARY,

            /// <summary>
            /// The direction of linear motion.
            /// </summary>
            LINEAR
        }


        public DirectionDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public DirectionDataItem(
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


        protected override IDataItem OnProcess(IDataItem dataItem, Version mtconnectVersion)
        {
            if (!string.IsNullOrEmpty(SubType) && mtconnectVersion < MTConnectVersions.Version12) return null;

            return dataItem;
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
                    // Check if Unavailable
                    if (result == Observation.Unavailable) return new ValidationResult(true);

                    if (mtconnectVersion >= MTConnectVersions.Version12)
                    {
                        if (SubType == SubTypes.LINEAR.ToString())
                        {
                            // Check Valid values in Enum
                            var validValues = Enum.GetValues(typeof(Observations.Events.Values.LinearDirection));
                            foreach (var validValue in validValues)
                            {
                                if (result == validValue.ToString())
                                {
                                    return new ValidationResult(true);
                                }
                            }

                            return new ValidationResult(false, "'" + result + "' is not a valid value");
                        }
                        else if (SubType == SubTypes.ROTARY.ToString())
                        {
                            // Check Valid values in Enum
                            var validValues = Enum.GetValues(typeof(Observations.Events.Values.RotaryDirection));
                            foreach (var validValue in validValues)
                            {
                                if (result == validValue.ToString())
                                {
                                    return new ValidationResult(true);
                                }
                            }

                            return new ValidationResult(false, "'" + result + "' is not a valid value");
                        }
                    }
                    else
                    {
                        return new ValidationResult(true);
                    }
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
                case SubTypes.ROTARY: return "The direction of rotary motion using the right-hand rule convention.";
                case SubTypes.LINEAR: return "The direction of linear motion.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ROTARY: return "rotary";
                case SubTypes.LINEAR: return "linear";
            }

            return null;
        }
    }
}