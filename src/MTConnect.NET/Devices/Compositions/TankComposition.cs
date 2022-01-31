// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A receptacle or container for holding material.
    /// </summary>
    public class TankComposition : Composition 
    {
        public const string TypeId = "TANK";
        public const string NameId = "tank";

        public TankComposition()  { Type = TypeId; }
    }
}
