// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A POT for a tool removed from Spindle or Turret and awaiting for return to a ToolMagazine.
    /// </summary>
    public class ReturnPotComposition : Composition 
    {
        public const string TypeId = "RETURN_POT";
        public const string NameId = "retpot";
        public new const string DescriptionText = "A POT for a tool removed from Spindle or Turret and awaiting for return to a ToolMagazine.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public ReturnPotComposition()  { Type = TypeId; }
    }
}