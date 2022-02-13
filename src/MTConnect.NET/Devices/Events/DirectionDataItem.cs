// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System;

namespace MTConnect.Devices.Events
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


        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        protected override DataItemValidationResult OnValidation(Version mtconnectVersion, IObservation observation)
        {
            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                // Get the CDATA Value for the Observation
                var cdata = observation.GetValue(ValueTypes.CDATA);
                if (cdata != null)
                {
                    // Check if Unavailable
                    if (cdata == Streams.DataItem.Unavailable) return new DataItemValidationResult(true);

                    if (SubType == SubTypes.LINEAR.ToString())
                    {
                        // Check Valid values in Enum
                        var validValues = Enum.GetValues(typeof(Streams.Events.LinearDirection));
                        foreach (var validValue in validValues)
                        {
                            if (cdata == validValue.ToString())
                            {
                                return new DataItemValidationResult(true);
                            }
                        }

                        return new DataItemValidationResult(false, "'" + cdata + "' is not a valid value");
                    }
                    else if (SubType == SubTypes.ROTARY.ToString())
                    {
                        // Check Valid values in Enum
                        var validValues = Enum.GetValues(typeof(Streams.Events.RotaryDirection));
                        foreach (var validValue in validValues)
                        {
                            if (cdata == validValue.ToString())
                            {
                                return new DataItemValidationResult(true);
                            }
                        }

                        return new DataItemValidationResult(false, "'" + cdata + "' is not a valid value");
                    }
                }
                else
                {
                    return new DataItemValidationResult(false, "No CDATA is specified for the Observation");
                }
            }

            return new DataItemValidationResult(false, "No Observation is Specified");
        }

        public override string GetSubTypeDescription() => GetSubTypeDescription(SubType);

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
