// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218209_919961_1605

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Maximum amount of fluid that can be held by a container.
    /// </summary>
    public class CapacityFluidDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "CAPACITY_FLUID";
        public const string NameId = "capacityFluid";
             
        public const string DefaultUnits = Devices.Units.MILLILITER;     
        public new const string DescriptionText = "Maximum amount of fluid that can be held by a container.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public CapacityFluidDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public CapacityFluidDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}