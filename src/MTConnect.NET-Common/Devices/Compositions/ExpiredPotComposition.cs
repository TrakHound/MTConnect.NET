// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A POT for a tool that is no longer useable for removal from a ToolMagazine or Turret.
    /// </summary>
    public class ExpiredPotComposition : Composition 
    {
        public const string TypeId = "EXPIRED_POT";
        public const string NameId = "exppot";
        public new const string DescriptionText = "A POT for a tool that is no longer useable for removal from a ToolMagazine or Turret.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public ExpiredPotComposition()  { Type = TypeId; }
    }
}