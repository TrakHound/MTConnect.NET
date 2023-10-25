// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106464_188160_44390

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that interrupts an electric circuit.
    /// </summary>
    public class CircuitBreakerComponent : Component
    {
        public const string TypeId = "CircuitBreaker";
        public const string NameId = "circuitBreakerComponent";
        public new const string DescriptionText = "Leaf Component that interrupts an electric circuit.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public CircuitBreakerComponent() { Type = TypeId; }
    }
}