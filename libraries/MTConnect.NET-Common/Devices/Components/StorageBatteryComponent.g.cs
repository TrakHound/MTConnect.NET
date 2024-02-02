// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106475_664974_44456

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of one or more cells in which chemical energy is converted into electricity and used as a source of power.
    /// </summary>
    public class StorageBatteryComponent : Component
    {
        public const string TypeId = "StorageBattery";
        public const string NameId = "storageBattery";
        public new const string DescriptionText = "Leaf Component composed of one or more cells in which chemical energy is converted into electricity and used as a source of power.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public StorageBatteryComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}