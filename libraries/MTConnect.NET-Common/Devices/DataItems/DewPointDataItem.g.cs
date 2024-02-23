// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1659032383380_200012_108

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Temperature at which moisture begins to condense, corresponding to saturation for a given absolute humidity.
    /// </summary>
    public class DewPointDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "DEW_POINT";
        public const string NameId = "dewPoint";
             
        public const string DefaultUnits = Devices.Units.CELSIUS;     
        public new const string DescriptionText = "Temperature at which moisture begins to condense, corresponding to saturation for a given absolute humidity.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version21;       


        public DewPointDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public DewPointDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}