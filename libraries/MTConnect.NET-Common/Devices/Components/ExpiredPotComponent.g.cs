// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605552258379_348093_2712

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that is a Pot for a tool that is no longer usable for removal from a ToolMagazine or Turret.
    /// </summary>
    public class ExpiredPotComponent : Component
    {
        public const string TypeId = "ExpiredPot";
        public const string NameId = "expiredPotComponent";
        public new const string DescriptionText = "Leaf Component that is a Pot for a tool that is no longer usable for removal from a ToolMagazine or Turret.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ExpiredPotComponent() { Type = TypeId; }
    }
}