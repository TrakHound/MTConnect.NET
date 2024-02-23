// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738876_256677_44724

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a unit that provides power to electric mechanisms.
    /// </summary>
    public class PowerSupplyComposition : Composition 
    {
        public const string TypeId = "POWER_SUPPLY";
        public const string NameId = "powerSupplyComposition";
        public new const string DescriptionText = "Composition composed of a unit that provides power to electric mechanisms.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public PowerSupplyComposition()  { Type = TypeId; }
    }
}