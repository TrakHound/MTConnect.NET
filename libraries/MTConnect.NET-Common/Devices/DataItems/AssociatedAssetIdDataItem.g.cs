// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1744720952328_73710_24751

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// AssetId of the Assets associated with a Component.
    /// </summary>
    public class AssociatedAssetIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ASSOCIATED_ASSET_ID";
        public const string NameId = "associatedAssetId";
             
             
        public new const string DescriptionText = "AssetId of the Assets associated with a Component.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version26;       


        public AssociatedAssetIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public AssociatedAssetIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}