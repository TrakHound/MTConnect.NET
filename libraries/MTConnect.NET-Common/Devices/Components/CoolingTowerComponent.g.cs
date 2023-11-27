// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605117477013_561048_2109

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a heat exchange system that uses a fluid to transfer heat to the atmosphere.
    /// </summary>
    public class CoolingTowerComponent : Component
    {
        public const string TypeId = "CoolingTower";
        public const string NameId = "coolingTowerComponent";
        public new const string DescriptionText = "Leaf Component composed of a heat exchange system that uses a fluid to transfer heat to the atmosphere.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public CoolingTowerComponent() { Type = TypeId; }
    }
}