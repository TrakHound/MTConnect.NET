// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Input;
using System;

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Represents the Agent’s ability to communicate with the data source.
    /// </summary>
    public class AvailabilityObservation : EventObservation
    {
        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        protected override ObservationValidationResult OnValidation(Version mtconnectVersion, IObservationInput observation)
        {
            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                // Get the CDATA Value for the Observation
                var cdata = observation.GetValue(ValueTypes.CDATA);
                if (cdata != null)
                {
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(Values.Availability));
                    foreach (var validValue in validValues)
                    {
                        if (cdata == validValue.ToString())
                        {
                            return new ObservationValidationResult(true);
                        }
                    }

                    return new ObservationValidationResult(false, "'" + cdata + "' is not a valid value");
                }
                else
                {
                    return new ObservationValidationResult(false, "No CDATA is specified for the Observation");
                }
            }

            return new ObservationValidationResult(false, "No Observation is Specified");
        }
    }
}
