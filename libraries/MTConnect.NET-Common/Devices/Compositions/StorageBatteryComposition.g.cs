// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738879_189590_44736

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of one or more cells that converts chemical energy to electricity and serves as a source of power.
    /// </summary>
    public class StorageBatteryComposition : Composition 
    {
        public const string TypeId = "STORAGE_BATTERY";
        public const string NameId = "storageBatteryComposition";
        public new const string DescriptionText = "Composition composed of one or more cells that converts chemical energy to electricity and serves as a source of power.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public StorageBatteryComposition()  { Type = TypeId; }
    }
}