// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218434_188173_2142

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Degree to which a substance opposes the passage of an electric current.
    /// </summary>
    public class ResistanceDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "RESISTANCE";
        public const string NameId = "resistance";
             
        public const string DefaultUnits = Devices.Units.OHM;     
        public new const string DescriptionText = "Degree to which a substance opposes the passage of an electric current.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public ResistanceDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public ResistanceDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}