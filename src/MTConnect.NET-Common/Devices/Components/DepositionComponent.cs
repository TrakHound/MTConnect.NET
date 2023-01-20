// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
        public new const string DescriptionText = "Deposition is an Auxiliary that represents the information for a system that manages the addition of material or state change of material being performed in an additive manufacturing process. For example, this could describe the portion of a piece of equipment that manages a material extrusion process or a vat polymerization process.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        public DepositionComponent()  { Type = TypeId; }
    }
}