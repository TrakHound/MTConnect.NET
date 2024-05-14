// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1660317516642_837590_88

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Force relative to earth's gravity.
    /// </summary>
    public class GravitationalForceDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "GRAVITATIONAL_FORCE";
        public const string NameId = "gravitationalForce";
             
        public const string DefaultUnits = Devices.NativeUnits.GRAVITATIONAL_FORCE;     
        public new const string DescriptionText = "Force relative to earth's gravity.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version21;       


        public GravitationalForceDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public GravitationalForceDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}