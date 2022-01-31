// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The direction of motion.
    /// </summary>
    public class DirectionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DIRECTION";
        public const string NameId = "direction";

        public enum SubTypes
        {
            /// <summary>
            /// The direction of rotary motion using the right-hand rule convention.
            /// </summary>
            ROTARY,

            /// <summary>
            /// The direction of linear motion.
            /// </summary>
            LINEAR
        }


        public DirectionDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public DirectionDataItem(
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

                if (SubType == SubTypes.LINEAR.ToString())
                {
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(Streams.Events.LinearDirection));
                    foreach (var validValue in validValues)
                    {
                        if (value.ToString() == validValue.ToString()) return true;
                    }
                }
                else if (SubType == SubTypes.ROTARY.ToString())
                {
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(Streams.Events.RotaryDirection));
                    foreach (var validValue in validValues)
                    {
                        if (value.ToString() == validValue.ToString()) return true;
                    }
                }
            }

            return false;
        }


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ROTARY: return "rotary";
                case SubTypes.LINEAR: return "linear";
            }

            return null;
        }
    }
}
