// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218211_523723_1608

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Geometric capacity of an object or container.
    /// </summary>
    public class CapacitySpatialDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "CAPACITY_SPATIAL";
        public const string NameId = "capacitySpatial";
             
        public const string DefaultUnits = Devices.Units.CUBIC_MILLIMETER;     
        public new const string DescriptionText = "Geometric capacity of an object or container.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public CapacitySpatialDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public CapacitySpatialDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}