// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// State or condition of a part.
    /// </summary>
    public class PartStatusDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_STATUS";
        public const string NameId = "partStatus";
             
        public new const string DescriptionText = "State or condition of a part.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public PartStatusDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public PartStatusDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}