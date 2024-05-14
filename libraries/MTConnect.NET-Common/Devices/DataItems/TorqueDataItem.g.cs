// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218461_778117_2235

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Turning force exerted on an object or by an object.
    /// </summary>
    public class TorqueDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "TORQUE";
        public const string NameId = "torque";
             
        public const string DefaultUnits = Devices.Units.NEWTON_METER;     
        public new const string DescriptionText = "Turning force exerted on an object or by an object.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public TorqueDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public TorqueDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}