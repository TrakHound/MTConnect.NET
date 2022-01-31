// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A fluid.
    /// </summary>
    public class WaterComposition : Composition 
    {
        public const string TypeId = "WATER";
        public const string NameId = "water";

        public WaterComposition()  { Type = TypeId; }
    }
}
