// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218284_523548_1767

using System;
using MTConnect.Observations;
using MTConnect.Input;
using MTConnect.Observations.Events;

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Operational state of a Door component or composition element.
    /// </summary>
    public class DoorStateDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "DOOR_STATE";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "doorState";

        /// <summary>
        /// The default <c>representation</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Operational state of a Door component or composition element.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public DoorStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        /// <summary>
        /// Initializes a new instance scoped to the given device.
        /// </summary>
        /// <param name="deviceId">The Id of the device this DataItem belongs to.</param>
        public DoorStateDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;
            
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
                    var validValues = Enum.GetValues(typeof(DoorState));
                    foreach (var validValue in validValues)
                    {
                        if (result == validValue.ToString())
                        {
                            return new ValidationResult(true);
                        }
                    }

                    return new ValidationResult(false, "'" + result + "' is not a valid value for DOOR_STATE");
                }
                else
                {
                    return new ValidationResult(false, "No Result is specified for the Observation");
                }
            }

            return new ValidationResult(false, "No Observation is Specified");
        }

        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        protected override ValidationResult OnValidation(Version mtconnectVersion, IObservation observation)
        {
            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                // Get the Result Value for the Observation
                var result = observation.GetValue(ValueKeys.Result);
                if (result != null)
                {
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(DoorState));
                    foreach (var validValue in validValues)
                    {
                        if (result == validValue.ToString())
                        {
                            return new ValidationResult(true);
                        }
                    }

                    return new ValidationResult(false, "'" + result + "' is not a valid value for DOOR_STATE");
                }
                else
                {
                    return new ValidationResult(false, "No Result is specified for the Observation");
                }
            }

            return new ValidationResult(false, "No Observation is Specified");
        }
    }
}