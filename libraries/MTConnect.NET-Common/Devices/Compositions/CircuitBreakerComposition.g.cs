// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738866_292316_44692

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that interrupts an electric circuit.
    /// </summary>
    public class CircuitBreakerComposition : Composition 
    {
        public const string TypeId = "CIRCUIT_BREAKER";
        public const string NameId = "circuitBreakerComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that interrupts an electric circuit.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public CircuitBreakerComposition()  { Type = TypeId; }
    }
}