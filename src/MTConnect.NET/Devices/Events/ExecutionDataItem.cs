// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System;

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The execution status of a component.
    /// </summary>
    public class ExecutionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "EXECUTION";
        public const string NameId = "execution";


        public ExecutionDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public ExecutionDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
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
                    var validValues = Enum.GetValues(typeof(Streams.Events.Execution));
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
    }
}
