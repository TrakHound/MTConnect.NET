// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Pot for a tool that is no longer usable for removal from a ToolMagazine or Turret.
    /// </summary>
    public class ExpiredPotCompositionComposition : Composition 
    {
        public const string TypeId = "EXPIRED_POT";
        public const string NameId = "expiredPotComposition";
        public new const string DescriptionText = "Pot for a tool that is no longer usable for removal from a ToolMagazine or Turret.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ExpiredPotCompositionComposition()  { Type = TypeId; }
    }
}