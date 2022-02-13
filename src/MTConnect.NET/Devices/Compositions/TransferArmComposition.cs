// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism for physically moving a tool from one location to another.
    /// </summary>
    public class TransferArmComposition : Composition 
    {
        public const string TypeId = "TRANSFER_ARM";
        public const string NameId = "transarm";
        public new const string DescriptionText = "A mechanism for physically moving a tool from one location to another.";

        public override string TypeDescription => DescriptionText;


        public TransferArmComposition()  { Type = TypeId; }
    }
}
