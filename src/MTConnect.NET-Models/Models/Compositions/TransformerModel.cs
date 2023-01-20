// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
