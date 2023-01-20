// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism that transforms electric energy from a source to a secondary circuit.
    /// </summary>
    public class TransformerModel : CompositionModel, ITransformerModel
    {
        public TransformerModel() 
        {
            Type = TransformerComposition.TypeId;
        }

        public TransformerModel(string compositionId)
        {
            Id = compositionId;
            Type = TransformerComposition.TypeId;
        }
    }
}