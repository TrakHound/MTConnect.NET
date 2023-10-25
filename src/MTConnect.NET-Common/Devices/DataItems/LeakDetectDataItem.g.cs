// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication designating whether a leak has been detected.
    /// </summary>
    public class LeakDetectDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "LEAK_DETECT";
        public const string NameId = "";
             
        public new const string DescriptionText = "Indication designating whether a leak has been detected.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version21;       


        public LeakDetectDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public LeakDetectDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}