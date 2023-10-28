// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1618831557881_852871_420

namespace MTConnect.Assets.RawMaterials
{
    /// <summary>
    /// Material used as the RawMaterial.
    /// </summary>
    public class Material : IMaterial
    {
        public const string DescriptionText = "Material used as the RawMaterial.";


        /// <summary>
        /// Unique identifier for the material.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Manufacturer's lot code of the material.
        /// </summary>
        public string Lot { get; set; }
        
        /// <summary>
        /// Name of the material manufacturer.
        /// </summary>
        public string Manufacturer { get; set; }
        
        /// <summary>
        /// Lot code of the raw feed stock for the material, from the feed stock manufacturer.
        /// </summary>
        public string ManufacturingCode { get; set; }
        
        /// <summary>
        /// Manufacturing date of the material from the material manufacturer.
        /// </summary>
        public System.DateTime ManufacturingDate { get; set; }
        
        /// <summary>
        /// ASTM standard code that the material complies with.
        /// </summary>
        public string MaterialCode { get; set; }
        
        /// <summary>
        /// Name of the material. Examples: `ULTM9085`, `ABS`, `4140`.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Type of material. Examples: `Metal`, `Polymer`, `Wood`, `4140`, `Recycled`, `Prestine` and `Used`.
        /// </summary>
        public string Type { get; set; }
    }
}