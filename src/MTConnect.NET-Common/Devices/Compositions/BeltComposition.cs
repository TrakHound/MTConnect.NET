// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// An endless flexible band used to transmit motion for a piece of equipment or to convey materials and objects.
    /// </summary>
    public class BeltComposition : Composition 
    {
        public const string TypeId = "BELT";
        public const string NameId = "belt";
        public new const string DescriptionText = "An endless flexible band used to transmit motion for a piece of equipment or to convey materials and objects.";

        public override string TypeDescription => DescriptionText;


        public BeltComposition()  { Type = TypeId; }
    }
}
