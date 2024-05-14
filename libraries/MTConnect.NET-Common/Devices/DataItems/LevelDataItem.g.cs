// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218331_809018_1875

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Level of a resource.**DEPRECATED** in *Version 1.2*.  See `FILL_LEVEL`.
    /// </summary>
    public class LevelDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "LEVEL";
        public const string NameId = "level";
             
             
        public new const string DescriptionText = "Level of a resource.**DEPRECATED** in *Version 1.2*.  See `FILL_LEVEL`.";
        
        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version12;
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public LevelDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public LevelDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}