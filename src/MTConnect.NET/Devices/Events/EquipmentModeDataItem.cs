// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// An indication that a piece of equipment, or a sub-part of a piece of equipment, is performing specific types of activities.
    /// </summary>
    public class EquipmentModeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "EQUIPMENT_MODE";
        public const string NameId = "equipMode";

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
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public EquipmentModeDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            DataItemCategory = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        /// <summary>
        /// Determine if the DataItem with the specified Value is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="value">The value of the DataItem</param>
        /// <returns>(true) if the value is valid. (false) if the value is invalid.</returns>
        public override bool IsValid(Version mtconnectVersion, object value)
        {
            if (value != null)
            {
                // Check if Unavailable
                if (value.ToString() == Streams.DataItem.Unavailable) return true;

                // Check Valid values in Enum
                var validValues = Enum.GetValues(typeof(Streams.Events.ControllerModeOverrideValue));
                foreach (var validValue in validValues)
                {
                    if (value.ToString() == validValue.ToString()) return true;
                }
            }

            return false;
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
