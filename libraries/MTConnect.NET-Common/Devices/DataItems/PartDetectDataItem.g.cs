// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication designating whether a part or work piece has been detected or is present.
    /// </summary>
    public class PartDetectDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_DETECT";
        public const string NameId = "partDetect";
             
        public new const string DescriptionText = "Indication designating whether a part or work piece has been detected or is present.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public PartDetectDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public PartDetectDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}