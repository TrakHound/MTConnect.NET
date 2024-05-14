// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605117515979_120166_2159

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a heat exchange system that uses a fluid to transfer heat to the atmosphere.
    /// </summary>
    public class CoolingTowerComposition : Composition 
    {
        public const string TypeId = "COOLING_TOWER";
        public const string NameId = "coolingTowerComposition";
        public new const string DescriptionText = "Composition composed of a heat exchange system that uses a fluid to transfer heat to the atmosphere.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public CoolingTowerComposition()  { Type = TypeId; }
    }
}