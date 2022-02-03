// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System;

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The indication of the status of the source of energy for a Structural Element to allow it to perform
    /// its intended function or the state of an enabling signal providing permission for the Structural Element to perform its functions.
    /// </summary>
    public class PowerStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "POWER_STATE";
        public const string NameId = "powerState";

        public enum SubTypes
        {
            /// <summary>
            /// The state of the power source for the Structural Element.
            /// </summary>
            LINE,

            /// <summary>
            /// The state of the enabling signal or control logic that enables or disables the function or operation of the Structural Element.
            /// </summary>
            CONTROL
        }


        public PowerStateDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public PowerStateDataItem(
            string parentId,
            SubTypes subType = SubTypes.LINE
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
        public override DataItemValidationResult IsValid(Version mtconnectVersion, IObservation observation)
        {
            var messageSuffix = "DataItem(" + Id + ") of '" + Type + "' and Representation of '" + Representation.ToString() + "'";

            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                // Get the CDATA Value for the Observation
                var cdata = observation.GetValue(ValueTypes.CDATA);
                if (cdata != null)
                {
                    // Check if Unavailable
                    if (cdata == Streams.DataItem.Unavailable) return new DataItemValidationResult(true);

                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(Streams.Events.PowerState));
                    foreach (var validValue in validValues)
                    {
                        if (cdata == validValue.ToString())
                        {
                            return new DataItemValidationResult(true);
                        }
                    }

                    return new DataItemValidationResult(false, "'" + cdata + "' is not a valid value for " + messageSuffix);
                }
                else
                {
                    return new DataItemValidationResult(false, "No CDATA is specified for the Observation for " + messageSuffix);
                }
            }

            return new DataItemValidationResult(false, "No Observation is Specified for " + messageSuffix);
        }


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.LINE: return "line";
                case SubTypes.CONTROL: return "control";
            }

            return null;
        }
    }
}
