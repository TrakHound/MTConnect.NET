// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The measurement of the amount of time a piece of equipment or a sub-part of a piece of equipment has performed specific activities.
    /// </summary>
    public class EquipmentTimerDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "EQUIPMENT_TIMER";
        public const string NameId = "equipTimer";
        public const string DefaultUnits = Devices.Units.SECOND;
        public new const string DescriptionText = "The measurement of the amount of time a piece of equipment or a sub-part of a piece of equipment has performed specific activities.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;

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


        public EquipmentTimerDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
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