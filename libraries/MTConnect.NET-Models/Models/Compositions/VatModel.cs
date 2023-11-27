// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A container for liquid or powdered materials.
    /// </summary>
    public class VatModel : CompositionModel, IVatModel
    {
        public VatModel() 
        {
            Type = VatComposition.TypeId;
        }

        public VatModel(string compositionId)
        {
            Id = compositionId;
            Type = VatComposition.TypeId;
        }
    }
}