// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218234_78662_1653

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Ability of a material to conduct electricity.
    /// </summary>
    public class ConductivityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "CONDUCTIVITY";
        public const string NameId = "conductivity";
             
        public const string DefaultUnits = Devices.Units.SIEMENS_PER_METER;     
        public new const string DescriptionText = "Ability of a material to conduct electricity.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public ConductivityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public ConductivityDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}