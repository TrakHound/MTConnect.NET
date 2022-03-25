// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// An indicator of the state of the axis lockout function when power has been removed and the axis is allowed to move freely.
    /// </summary>
    public class AxisInterlockDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "AXIS_INTERLOCK";
        public const string NameId = "interlock";
        public new const string DescriptionText = "An indicator of the state of the axis lockout function when power has been removed and the axis is allowed to move freely.";

        public override string TypeDescription => DescriptionText;


        public AxisInterlockDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public AxisInterlockDataItem(string parentId)
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
        protected override DataItemValidationResult OnValidation(Version mtconnectVersion, IObservationInput observation)
        {
            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                // Get the CDATA Value for the Observation
                var cdata = observation.GetValue(ValueKeys.CDATA);
                if (cdata != null)
                {
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(Observations.Events.Values.AxisInterlock));
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
