// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738884_732376_44748

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that halts or controls the flow of a liquid, gas, or other material through a passage, pipe, inlet, or outlet.
    /// </summary>
    public class ValveComposition : Composition 
    {
        public const string TypeId = "VALVE";
        public const string NameId = "valveComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that halts or controls the flow of a liquid, gas, or other material through a passage, pipe, inlet, or outlet.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ValveComposition()  { Type = TypeId; }
    }
}