// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// An interconnected series of objects that band together and are used to transmit motion for a piece of equipment or to convey materials and objects.
    /// </summary>
    public class ChainComposition : Composition 
    {
        public const string TypeId = "CHAIN";
        public const string NameId = "chain";
        public new const string DescriptionText = "An interconnected series of objects that band together and are used to transmit motion for a piece of equipment or to convey materials and objects.";

        public override string TypeDescription => DescriptionText;


        public ChainComposition()  { Type = TypeId; }
    }
}
