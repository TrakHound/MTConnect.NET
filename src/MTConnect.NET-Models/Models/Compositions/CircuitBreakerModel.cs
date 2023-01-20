// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism for interrupting an electric circuit.
    /// </summary>
    public class CircuitBreakerModel : CompositionModel, ICircuitBreakerModel
    {
        public CircuitBreakerModel() 
        {
            Type = CircuitBreakerComposition.TypeId;
        }

        public CircuitBreakerModel(string compositionId)
        {
            Id = compositionId;
            Type = CircuitBreakerComposition.TypeId;
        }
    }
}
