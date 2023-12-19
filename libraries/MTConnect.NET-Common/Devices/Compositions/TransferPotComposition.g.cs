// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Pot for a tool awaiting transfer from a ToolMagazine to spindle or Turret.
    /// </summary>
    public class TransferPotComposition : Composition 
    {
        public const string TypeId = "TRANSFER_POT";
        public const string NameId = "transferPotComposition";
        public new const string DescriptionText = "Pot for a tool awaiting transfer from a ToolMagazine to spindle or Turret.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public TransferPotComposition()  { Type = TypeId; }
    }
}