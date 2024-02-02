// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106480_27284_44483

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a string like piece or filament of relatively rigid or flexible material provided in a variety of diameters.
    /// </summary>
    public class WireComponent : Component
    {
        public const string TypeId = "Wire";
        public const string NameId = "wire";
        public new const string DescriptionText = "Leaf Component composed of a string like piece or filament of relatively rigid or flexible material provided in a variety of diameters.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public WireComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}