// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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