// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A heat exchange system that uses a fluid to transfer heat to the atmosphere.
    /// </summary>
    public class CoolingTowerComposition : Composition 
    {
        public const string TypeId = "COOLING_TOWER";
        public const string NameId = "cooltower";

        public CoolingTowerComposition()  { Type = TypeId; }
    }
}
