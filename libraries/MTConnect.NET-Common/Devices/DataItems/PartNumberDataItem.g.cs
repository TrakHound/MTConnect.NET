// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier of a part or product moving through the manufacturing process.**DEPRECATED** in *Version 1.7*. `PART_NUMBER` is now a `subType` of `PART_KIND_ID`.
    /// </summary>
    public class PartNumberDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_NUMBER";
        public const string NameId = "partNumber";
             
        public new const string DescriptionText = "Identifier of a part or product moving through the manufacturing process.**DEPRECATED** in *Version 1.7*. `PART_NUMBER` is now a `subType` of `PART_KIND_ID`.";
        
        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version17;
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public PartNumberDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public PartNumberDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}