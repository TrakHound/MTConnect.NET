// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.RawMaterials
{
    /// <summary>
    /// Material used as the RawMaterial.
    /// </summary>
    public interface IMaterial
    {
        /// <summary>
        /// Unique identifier for the material.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Manufacturer's lot code of the material.
        /// </summary>
        string Lot { get; }
        
        /// <summary>
        /// Name of the material manufacturer.
        /// </summary>
        string Manufacturer { get; }
        
        /// <summary>
        /// Lot code of the raw feed stock for the material, from the feed stock manufacturer.
        /// </summary>
        string ManufacturingCode { get; }
        
        /// <summary>
        /// Manufacturing date of the material from the material manufacturer.
        /// </summary>
        System.DateTime ManufacturingDate { get; }
        
        /// <summary>
        /// Astm standard code that the material complies with.
        /// </summary>
        string MaterialCode { get; }
        
        /// <summary>
        /// Name of the material. Examples: `ULTM9085`, `ABS`, `4140`.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Type of material. Examples: `Metal`, `Polymer`, `Wood`, `4140`, `Recycled`, `Prestine` and `Used`.
        /// </summary>
        string Type { get; }
    }
}