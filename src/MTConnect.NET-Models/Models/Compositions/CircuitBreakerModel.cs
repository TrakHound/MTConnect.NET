// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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