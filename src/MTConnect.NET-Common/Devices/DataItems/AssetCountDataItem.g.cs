// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Data set of the number of Asset of a given type for a Device.
    /// </summary>
    public class AssetCountDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ASSET_COUNT";
        public const string NameId = "assetCount";
             
        public new const string DescriptionText = "Data set of the number of Asset of a given type for a Device.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version20;       


        public AssetCountDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public AssetCountDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}