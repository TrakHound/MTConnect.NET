// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218335_32762_1887

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Force applied to a mass in one direction only.
    /// </summary>
    public class LinearForceDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "LINEAR_FORCE";
        public const string NameId = "linearForce";
             
        public const string DefaultUnits = Devices.Units.NEWTON;     
        public new const string DescriptionText = "Force applied to a mass in one direction only.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public LinearForceDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public LinearForceDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}