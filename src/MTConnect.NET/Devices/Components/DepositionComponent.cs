// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Deposition is an Auxiliary that represents the information for a system that manages the addition of material
    /// or state change of material being performed in an additive manufacturing process. 
    /// For example, this could describe the portion of a piece of equipment that manages a material extrusion process or a vat polymerization process.
    /// </summary>
    public class DepositionComponent : Component 
    {
        public const string TypeId = "Deposition";
        public const string NameId = "dep";

        public DepositionComponent()  { Type = TypeId; }
    }
}
