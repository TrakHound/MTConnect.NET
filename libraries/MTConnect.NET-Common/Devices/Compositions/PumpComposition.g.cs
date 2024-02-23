// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738877_834141_44728

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of an apparatus that raises, drives, exhausts, or compresses fluids or gases by means of a piston, plunger, or set of rotating vanes.
    /// </summary>
    public class PumpComposition : Composition 
    {
        public const string TypeId = "PUMP";
        public const string NameId = "pumpComposition";
        public new const string DescriptionText = "Composition composed of an apparatus that raises, drives, exhausts, or compresses fluids or gases by means of a piston, plunger, or set of rotating vanes.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public PumpComposition()  { Type = TypeId; }
    }
}