// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738874_596493_44716

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a chamber or bin that stores materials temporarily, typically being filled through the top and dispensed through the bottom.
    /// </summary>
    public class HopperComposition : Composition 
    {
        public const string TypeId = "HOPPER";
        public const string NameId = "hopperComposition";
        public new const string DescriptionText = "Composition composed of a chamber or bin that stores materials temporarily, typically being filled through the top and dispensed through the bottom.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public HopperComposition()  { Type = TypeId; }
    }
}