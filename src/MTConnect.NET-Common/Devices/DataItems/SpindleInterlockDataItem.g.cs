// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication of the status of the spindle for a piece of equipment when power has been removed and it is free to rotate.
    /// </summary>
    public class SpindleInterlockDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SPINDLE_INTERLOCK";
        public const string NameId = "";
             
        public new const string DescriptionText = "Indication of the status of the spindle for a piece of equipment when power has been removed and it is free to rotate.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public SpindleInterlockDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public SpindleInterlockDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}