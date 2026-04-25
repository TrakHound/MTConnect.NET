// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1760962647094_276885_585

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Concentration of calcium carbonate (CaCO3) in water
    /// </summary>
    public class WaterHardnessDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "WATER_HARDNESS";
        public const string NameId = "waterHardness";
             
        public const string DefaultUnits = Devices.Units.MILLIGRAM_PER_LITER;     
        public new const string DescriptionText = "Concentration of calcium carbonate (CaCO3) in water";
        
        public override string TypeDescription => DescriptionText;
        
               


        public WaterHardnessDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public WaterHardnessDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}