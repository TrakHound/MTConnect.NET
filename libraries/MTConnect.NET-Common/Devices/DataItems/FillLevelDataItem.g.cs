// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218308_747718_1824

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Amount of a substance remaining compared to the planned maximum amount of that substance.
    /// </summary>
    public class FillLevelDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "FILL_LEVEL";
        public const string NameId = "fillLevel";
             
        public const string DefaultUnits = Devices.Units.PERCENT;     
        public new const string DescriptionText = "Amount of a substance remaining compared to the planned maximum amount of that substance.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public FillLevelDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public FillLevelDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}