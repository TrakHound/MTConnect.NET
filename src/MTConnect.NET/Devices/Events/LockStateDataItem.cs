// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The state or operating mode of a Lock.
    /// </summary>
    public class LockStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "LOCK_STATE";
        public const string NameId = "lockState";


        public LockStateDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public LockStateDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
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
                var validValues = Enum.GetValues(typeof(Streams.Events.LockState));
                foreach (var validValue in validValues)
                {
                    if (value.ToString() == validValue.ToString()) return true;
                }
            }

            return false;
        }
    }
}
