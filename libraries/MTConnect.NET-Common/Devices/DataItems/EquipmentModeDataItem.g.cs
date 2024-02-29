// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218291_180049_1785

using System;
using MTConnect.Observations;
using MTConnect.Input;
using MTConnect.Observations.Events;

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication that a piece of equipment, or a sub-part of a piece of equipment, is performing specific types of activities.
    /// </summary>
    public class EquipmentModeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "EQUIPMENT_MODE";
        public const string NameId = "equipmentMode";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "Indication that a piece of equipment, or a sub-part of a piece of equipment, is performing specific types of activities.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public enum SubTypes
        {
            /// <summary>
            /// Indication that the sub-parts of a piece of equipment are under load.Example: For traditional machine tools, this is an indication that the cutting tool is assumed to be engaged with the part.
            /// </summary>
            LOADED,
            
            /// <summary>
            /// Indication that a piece of equipment is performing any activity, the equipment is active and performing a function under load or not.Example: For traditional machine tools, this includes when the piece of equipment is `LOADED`, making rapid moves, executing a tool change, etc.
            /// </summary>
            WORKING,
            
            /// <summary>
            /// Indication that the major sub-parts of a piece of equipment are powered or performing any activity whether producing a part or product or not.Example: For traditional machine tools, this includes when the piece of equipment is `WORKING` or it is idle.
            /// </summary>
            OPERATING,
            
            /// <summary>
            /// Indication that primary power is applied to the piece of equipment and, as a minimum, the controller or logic portion of the piece of equipment is powered and functioning or components that are required to remain on arepowered.Example: Heaters for an extrusion machine that required to be powered even when the equipment is turned off.
            /// </summary>
            POWERED,
            
            /// <summary>
            /// Elapsed time of a temporary halt of action.
            /// </summary>
            DELAY
        }


        public EquipmentModeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
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
            Representation = DefaultRepresentation; 
            
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.LOADED: return "Indication that the sub-parts of a piece of equipment are under load.Example: For traditional machine tools, this is an indication that the cutting tool is assumed to be engaged with the part.";
                case SubTypes.WORKING: return "Indication that a piece of equipment is performing any activity, the equipment is active and performing a function under load or not.Example: For traditional machine tools, this includes when the piece of equipment is `LOADED`, making rapid moves, executing a tool change, etc.";
                case SubTypes.OPERATING: return "Indication that the major sub-parts of a piece of equipment are powered or performing any activity whether producing a part or product or not.Example: For traditional machine tools, this includes when the piece of equipment is `WORKING` or it is idle.";
                case SubTypes.POWERED: return "Indication that primary power is applied to the piece of equipment and, as a minimum, the controller or logic portion of the piece of equipment is powered and functioning or components that are required to remain on arepowered.Example: Heaters for an extrusion machine that required to be powered even when the equipment is turned off.";
                case SubTypes.DELAY: return "Elapsed time of a temporary halt of action.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.LOADED: return "LOADED";
                case SubTypes.WORKING: return "WORKING";
                case SubTypes.OPERATING: return "OPERATING";
                case SubTypes.POWERED: return "POWERED";
                case SubTypes.DELAY: return "DELAY";
            }

            return null;
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
                    var validValues = Enum.GetValues(typeof(EquipmentMode));
                    foreach (var validValue in validValues)
                    {
                        if (result == validValue.ToString())
                        {
                            return new ValidationResult(true);
                        }
                    }

                    return new ValidationResult(false, "'" + result + "' is not a valid value for EQUIPMENT_MODE");
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
                    var validValues = Enum.GetValues(typeof(EquipmentMode));
                    foreach (var validValue in validValues)
                    {
                        if (result == validValue.ToString())
                        {
                            return new ValidationResult(true);
                        }
                    }

                    return new ValidationResult(false, "'" + result + "' is not a valid value for EQUIPMENT_MODE");
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