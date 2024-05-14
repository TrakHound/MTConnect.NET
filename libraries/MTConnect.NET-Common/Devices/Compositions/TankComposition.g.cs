// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738882_135940_44742

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a receptacle or container that holds material.
    /// </summary>
    public class TankComposition : Composition 
    {
        public const string TypeId = "TANK";
        public const string NameId = "tankComposition";
        public new const string DescriptionText = "Composition composed of a receptacle or container that holds material.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public TankComposition()  { Type = TypeId; }
    }
}