// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A POT for a tool awaiting transfer from a ToolMagazine to Spindle or Turret.
    /// </summary>
    public class TransferPotComposition : Composition 
    {
        public const string TypeId = "TRANSFER_POT";
        public const string NameId = "transpot";
        public new const string DescriptionText = "A POT for a tool awaiting transfer from a ToolMagazine to Spindle or Turret.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public TransferPotComposition()  { Type = TypeId; }
    }
}