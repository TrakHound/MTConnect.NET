// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A string like piece or filament of relatively rigid or flexible material provided in a variety of diameters.
    /// </summary>
    public class WireComposition : Composition 
    {
        public const string TypeId = "WIRE";
        public const string NameId = "wire";
        public new const string DescriptionText = "A string like piece or filament of relatively rigid or flexible material provided in a variety of diameters.";

        public override string TypeDescription => DescriptionText;


        public WireComposition()  { Type = TypeId; }
    }
}
