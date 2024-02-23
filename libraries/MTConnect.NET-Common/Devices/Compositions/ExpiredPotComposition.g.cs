// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605552406488_998382_3133

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Pot for a tool that is no longer usable for removal from a ToolMagazine or Turret.
    /// </summary>
    public class ExpiredPotComposition : Composition 
    {
        public const string TypeId = "EXPIRED_POT";
        public const string NameId = "expiredPotComposition";
        public new const string DescriptionText = "Pot for a tool that is no longer usable for removal from a ToolMagazine or Turret.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ExpiredPotComposition()  { Type = TypeId; }
    }
}