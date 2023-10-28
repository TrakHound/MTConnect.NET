// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.RawMaterials
{
    /// <summary>
    /// Asset that represents raw material.
    /// </summary>
    public partial interface IRawMaterial : IAsset
    {
        /// <summary>
        /// Type of container holding the raw material. Examples: `Pallet`, `Canister`, `Cartridge`, `Tank`, `Bin`, `Roll`, and `Spool`.
        /// </summary>
        string ContainerType { get; }
        
        /// <summary>
        /// Dimension of material currently in raw material.
        /// </summary>
        System.Collections.Generic.IEnumerable<string> CurrentDimensions { get; }
        
        /// <summary>
        /// Quantity of material currently in raw material.
        /// </summary>
        int CurrentQuantity { get; }
        
        /// <summary>
        /// Amount of material currently in raw material.
        /// </summary>
        string CurrentVolume { get; }
        
        /// <summary>
        /// Date raw material was first used.
        /// </summary>
        System.DateTime FirstUseDate { get; }
        
        /// <summary>
        /// Form of the raw material.
        /// </summary>
        MTConnect.Assets.RawMaterials.Form Form { get; }
        
        /// <summary>
        /// Material has existing usable volume.
        /// </summary>
        bool HasMaterial { get; }
        
        /// <summary>
        /// Dimension of material initially placed in raw material when manufactured.
        /// </summary>
        System.Collections.Generic.IEnumerable<string> InitialDimensions { get; }
        
        /// <summary>
        /// Quantity of material initially placed in raw material when manufactured.
        /// </summary>
        int InitialQuantity { get; }
        
        /// <summary>
        /// Amount of material initially placed in raw material when manufactured.
        /// </summary>
        string InitialVolume { get; }
        
        /// <summary>
        /// Date raw material was last used.
        /// </summary>
        System.DateTime LastUseDate { get; }
        
        /// <summary>
        /// Date the raw material was created.
        /// </summary>
        System.DateTime ManufacturingDate { get; }
        
        /// <summary>
        /// Material used as the RawMaterial.
        /// </summary>
        MTConnect.Assets.RawMaterials.IMaterial Material { get; }
        
        /// <summary>
        /// Name of the raw material.Examples: `Container1` and `AcrylicContainer`.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// ISO process type supported by this raw material. Examples include: `VAT_POLYMERIZATION`, `BINDER_JETTING`, `MATERIAL_EXTRUSION`, `MATERIAL_JETTING`, `SHEET_LAMINATION`, `POWDER_BED_FUSION` and `DIRECTED_ENERGY_DEPOSITION`.
        /// </summary>
        string ProcessKind { get; }
        
        /// <summary>
        /// Serial number of the raw material.
        /// </summary>
        string SerialNumber { get; }
    }
}