// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// An indication that a piece of equipment, or a sub-part of a piece of equipment, is performing specific types of activities.
    /// </summary>
    public class EquipmentModeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "EQUIPMENT_MODE";
        public const string NameId = "equipMode";
        public new const string DescriptionText = "An indication that a piece of equipment, or a sub-part of a piece of equipment, is performing specific types of activities.";

        public override string TypeDescription => DescriptionText;

        public override Version MinimumVersion => MTConnectVersions.Version14;

        public enum SubTypes
        {
            /// <summary>
            /// Measurement of the time that the sub-parts of a piece of equipment are under load. 
            /// </summary>
            LOADED,

            /// <summary>
            /// Measurement of the time that a piece of equipment is performing any activity the equipment is active and performing a function under load or not.
            /// </summary>
            WORKING,

            /// <summary>
            /// Measurement of the time that the major sub-parts of a piece of equipment are powered or performing any activity whether producing a part or product or not.
            /// </summary>
            OPERATING,

            /// <summary>
            /// The measurement of time that primary power is applied to the piece of equipment and, as a minimum, 
            /// the controller or logic portion of the piece of equipment is powered and functioning or components that are required to remain on are powered.
            /// </summary>
            POWERED,

            /// <summary>
            /// The elapsed time of a temporary halt of action.
            /// </summary>
            DELAY
        }


        public EquipmentModeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public EquipmentModeDataItem(
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
                    var validValues = Enum.GetValues(typeof(Observations.Events.Values.ControllerModeOverrideValue));
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
                case SubTypes.LOADED: return "Measurement of the time that the sub-parts of a piece of equipment are under load.";
                case SubTypes.WORKING: return "Measurement of the time that a piece of equipment is performing any activity the equipment is active and performing a function under load or not.";
                case SubTypes.OPERATING: return "Measurement of the time that the major sub-parts of a piece of equipment are powered or performing any activity whether producing a part or product or not.";
                case SubTypes.POWERED: return "The measurement of time that primary power is applied to the piece of equipment and, as a minimum, the controller or logic portion of the piece of equipment is powered and functioning or components that are required to remain on are powered.";
                case SubTypes.DELAY: return "The elapsed time of a temporary halt of action.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.LOADED: return "loaded";
                case SubTypes.WORKING: return "working";
                case SubTypes.OPERATING: return "operating";
                case SubTypes.POWERED: return "powered";
                case SubTypes.DELAY: return "delay";
            }

            return null;
        }
    }
}
