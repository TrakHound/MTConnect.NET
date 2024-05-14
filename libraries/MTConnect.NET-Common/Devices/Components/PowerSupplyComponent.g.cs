// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106472_81456_44438

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that provides power to electric mechanisms.
    /// </summary>
    public class PowerSupplyComponent : Component
    {
        public const string TypeId = "PowerSupply";
        public const string NameId = "powerSupply";
        public new const string DescriptionText = "Leaf Component that provides power to electric mechanisms.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public PowerSupplyComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}