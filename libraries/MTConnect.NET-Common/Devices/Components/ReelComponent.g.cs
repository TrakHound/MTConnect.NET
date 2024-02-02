// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106474_29298_44447

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a rotary storage unit for material.
    /// </summary>
    public class ReelComponent : Component
    {
        public const string TypeId = "Reel";
        public const string NameId = "reel";
        public new const string DescriptionText = "Leaf Component composed of a rotary storage unit for material.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public ReelComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}