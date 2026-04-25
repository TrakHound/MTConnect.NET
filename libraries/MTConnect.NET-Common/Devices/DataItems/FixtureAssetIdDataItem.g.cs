// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1760963612607_418361_879

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// AssetId of the Fixture that is associated with a Component
    /// </summary>
    public class FixtureAssetIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "FIXTURE_ASSET_ID";
        public const string NameId = "fixtureAssetId";
             
             
        public new const string DescriptionText = "AssetId of the Fixture that is associated with a Component";
        
        public override string TypeDescription => DescriptionText;
        
               


        public FixtureAssetIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public FixtureAssetIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}