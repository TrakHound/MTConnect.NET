// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218451_953222_2205

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Degree of hotness or coldness measured on a definite scale.
    /// </summary>
    public class TemperatureDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "TEMPERATURE";
        public const string NameId = "temp";
             
        public const string DefaultUnits = Devices.Units.CELSIUS;     
        public new const string DescriptionText = "Degree of hotness or coldness measured on a definite scale.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public TemperatureDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public TemperatureDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}