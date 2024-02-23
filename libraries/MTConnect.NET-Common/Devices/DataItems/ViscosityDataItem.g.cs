// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218468_92553_2256

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Fluid's resistance to flow.
    /// </summary>
    public class ViscosityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VISCOSITY";
        public const string NameId = "viscosity";
             
        public const string DefaultUnits = Devices.Units.PASCAL_SECOND;     
        public new const string DescriptionText = "Fluid's resistance to flow.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public ViscosityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public ViscosityDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}