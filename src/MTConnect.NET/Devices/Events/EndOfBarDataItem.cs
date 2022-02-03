// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System;

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// An indication of whether the end of a piece of bar stock being feed by a bar feeder has been reached.
    /// </summary>
    public class EndOfBarDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "END_OF_BAR";
        public const string NameId = "eob";

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
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public EndOfBarDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            DataItemCategory = CategoryId;
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
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(Streams.Events.EndOfBar));
                    foreach (var validValue in validValues)
                    {
                        if (cdata == validValue.ToString())
                        {
                            return new DataItemValidationResult(true);
                        }
                    }

                    return new DataItemValidationResult(false, "'" + cdata + "' is not a valid value");
                }
                else
                {
                    return new DataItemValidationResult(false, "No CDATA is specified for the Observation");
                }
            }

            return new DataItemValidationResult(false, "No Observation is Specified");
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
