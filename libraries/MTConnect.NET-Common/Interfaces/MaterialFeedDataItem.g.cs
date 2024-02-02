// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Operating state of the service to advance material or feed product to a piece of equipment from a continuous or bulk source.
    /// </summary>
    public class MaterialFeedDataItem : InterfaceDataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MATERIAL_FEED";
        public const string NameId = "materialFeed";
             
        public new const string DescriptionText = "Operating state of the service to advance material or feed product to a piece of equipment from a continuous or bulk source.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public MaterialFeedDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public MaterialFeedDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}