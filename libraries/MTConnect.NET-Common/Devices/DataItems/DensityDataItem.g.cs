// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218257_564545_1704

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Volumetric mass of a material per unit volume of that material.
    /// </summary>
    public class DensityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "DENSITY";
        public const string NameId = "density";
             
        public const string DefaultUnits = Devices.Units.MILLIGRAM_PER_CUBIC_MILLIMETER;     
        public new const string DescriptionText = "Volumetric mass of a material per unit volume of that material.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public DensityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public DensityDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}