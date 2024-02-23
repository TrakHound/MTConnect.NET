// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218298_41713_1803

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Amount of time a piece of equipment or a sub-part of a piece of equipment has performed specific activities.
    /// </summary>
    public class EquipmentTimerDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "EQUIPMENT_TIMER";
        public const string NameId = "equipmentTimer";
             
        public const string DefaultUnits = Devices.Units.SECOND;     
        public new const string DescriptionText = "Amount of time a piece of equipment or a sub-part of a piece of equipment has performed specific activities.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public enum SubTypes
        {
            /// <summary>
            /// Time that the sub-parts of a piece of equipment are under load.Example: For traditional machine tools, this is a measurement of the time that the cutting tool is assumed to be engaged with the part.
            /// </summary>
            LOADED,
            
            /// <summary>
            /// Time that a piece of equipment is performing any activity the equipment is active and performing a function under load or not.Example: For traditional machine tools, this includes `LOADED`, plus rapid moves, tool changes, etc.
            /// </summary>
            WORKING,
            
            /// <summary>
            /// Time that the major sub-parts of a piece of equipment are powered or performing any activity whether producing a part or product or not.Example: For traditional machine tools, this includes `WORKING`, plus idle time.
            /// </summary>
            OPERATING,
            
            /// <summary>
            /// Time that primary power is applied to the piece of equipment and, as a minimum, the controller or logic portion of the piece of equipment is powered and functioning or components that are required to remain on are powered.Example: Heaters for an extrusion machine that are required to be powered even when the equipment is turned off.
            /// </summary>
            POWERED,
            
            /// <summary>
            /// Elapsed time of a temporary halt of action.
            /// </summary>
            DELAY
        }


        public EquipmentTimerDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public EquipmentTimerDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
             
            Units = DefaultUnits;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.LOADED: return "Time that the sub-parts of a piece of equipment are under load.Example: For traditional machine tools, this is a measurement of the time that the cutting tool is assumed to be engaged with the part.";
                case SubTypes.WORKING: return "Time that a piece of equipment is performing any activity the equipment is active and performing a function under load or not.Example: For traditional machine tools, this includes `LOADED`, plus rapid moves, tool changes, etc.";
                case SubTypes.OPERATING: return "Time that the major sub-parts of a piece of equipment are powered or performing any activity whether producing a part or product or not.Example: For traditional machine tools, this includes `WORKING`, plus idle time.";
                case SubTypes.POWERED: return "Time that primary power is applied to the piece of equipment and, as a minimum, the controller or logic portion of the piece of equipment is powered and functioning or components that are required to remain on are powered.Example: Heaters for an extrusion machine that are required to be powered even when the equipment is turned off.";
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

    }
}