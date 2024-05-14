// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1640605430258_307416_529

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier for the current workholding or part clamp in use by a piece of equipment.
    /// </summary>
    public class FixtureIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "FIXTURE_ID";
        public const string NameId = "fixtureId";
             
             
        public new const string DescriptionText = "Identifier for the current workholding or part clamp in use by a piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version20;       


        public FixtureIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public FixtureIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}