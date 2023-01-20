// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism for interrupting an electric circuit.
    /// </summary>
    public class CircuitBreakerComposition : Composition 
    {
        public const string TypeId = "CIRCUIT_BREAKER";
        public const string NameId = "cbreaker";
        public new const string DescriptionText = "A mechanism for interrupting an electric circuit.";

        public override string TypeDescription => DescriptionText;


        public CircuitBreakerComposition()  { Type = TypeId; }
    }
}