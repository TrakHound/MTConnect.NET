// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
