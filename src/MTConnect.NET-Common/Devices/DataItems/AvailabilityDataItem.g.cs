// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Agent's ability to communicate with the data source.
    /// </summary>
    public class AvailabilityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "AVAILABILITY";
        public const string NameId = "";
             
        public new const string DescriptionText = "Agent's ability to communicate with the data source.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public AvailabilityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public AvailabilityDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}