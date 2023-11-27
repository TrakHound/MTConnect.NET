// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Deposition is an Auxiliary that represents the information for a system that manages the addition of material
    /// or state change of material being performed in an additive manufacturing process. 
    /// For example, this could describe the portion of a piece of equipment that manages a material extrusion process or a vat polymerization process.
    /// </summary>
    public class DepositionModel : AuxiliaryModel
    {
        public DepositionModel()
        {
            Type = DepositionComponent.TypeId;
        }

        public DepositionModel(string componentId)
        {
            Id = componentId;
            Type = DepositionComponent.TypeId;
        }
    }
}