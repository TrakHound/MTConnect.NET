// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A unit that provides power to electric mechanisms.
    /// </summary>
    public class PowerSupplyComposition : Composition 
    {
        public const string TypeId = "POWER_SUPPLY";
        public const string NameId = "pwrsupply";
        public new const string DescriptionText = "A unit that provides power to electric mechanisms.";

        public override string TypeDescription => DescriptionText;


        public PowerSupplyComposition()  { Type = TypeId; }
    }
}
