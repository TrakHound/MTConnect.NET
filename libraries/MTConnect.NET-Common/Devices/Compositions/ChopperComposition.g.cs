// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738863_858561_44686

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that breaks material into smaller pieces.
    /// </summary>
    public class ChopperComposition : Composition 
    {
        public const string TypeId = "CHOPPER";
        public const string NameId = "chopperComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that breaks material into smaller pieces.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ChopperComposition()  { Type = TypeId; }
    }
}