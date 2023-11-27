// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// State or operating mode of a Lock.
    /// </summary>
    public class LockStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "LOCK_STATE";
        public const string NameId = "lockState";
             
        public new const string DescriptionText = "State or operating mode of a Lock.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version18;       


        public LockStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public LockStateDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}