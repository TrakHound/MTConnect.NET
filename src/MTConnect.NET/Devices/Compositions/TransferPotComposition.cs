// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A POT for a tool awaiting transfer from a ToolMagazine to Spindle or Turret.
    /// </summary>
    public class TransferPotComposition : Composition 
    {
        public const string TypeId = "TRANSFER_POT";
        public const string NameId = "transpot";

        public TransferPotComposition()  { Type = TypeId; }
    }
}
