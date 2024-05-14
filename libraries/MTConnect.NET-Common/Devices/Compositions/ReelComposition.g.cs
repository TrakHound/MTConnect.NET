// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738878_98050_44730

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a rotary storage unit for material.
    /// </summary>
    public class ReelComposition : Composition 
    {
        public const string TypeId = "REEL";
        public const string NameId = "reelComposition";
        public new const string DescriptionText = "Composition composed of a rotary storage unit for material.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public ReelComposition()  { Type = TypeId; }
    }
}