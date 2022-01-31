// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// BarFeeder is a Loader involved in delivering bar stock to a piece of equipment.
    /// </summary>
    public class BarFeederComponent : Component 
    {
        public const string TypeId = "BarFeeder";
        public const string NameId = "bfeeder";

        public BarFeederComponent()  { Type = TypeId; }
    }
}
