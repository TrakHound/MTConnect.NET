// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System;

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The current intended production status of the device or component.
    /// </summary>
    public class FunctionalModeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "FUNCTIONAL_MODE";
        public const string NameId = "funcMode";
        public new const string DescriptionText = "The current intended production status of the device or component.";

        public override string TypeDescription => DescriptionText;


        public FunctionalModeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public FunctionalModeDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
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
                    var validValues = Enum.GetValues(typeof(Streams.Events.FunctionalMode));
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
    }
}
