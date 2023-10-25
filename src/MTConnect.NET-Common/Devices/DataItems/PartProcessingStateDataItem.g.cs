// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Particular condition of the part occurrence at a specific time.
    /// </summary>
    public class PartProcessingStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_PROCESSING_STATE";
        public const string NameId = "";
             
        public new const string DescriptionText = "Particular condition of the part occurrence at a specific time.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version18;       


        public PartProcessingStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public PartProcessingStateDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}