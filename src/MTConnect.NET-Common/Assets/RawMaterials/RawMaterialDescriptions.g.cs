// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.RawMaterials
{
    public static class RawMaterialDescriptions
    {
        /// <summary>
        /// Type of container holding the raw material. Examples: `Pallet`, `Canister`, `Cartridge`, `Tank`, `Bin`, `Roll`, and `Spool`.
        /// </summary>
        public const string ContainerType = "Type of container holding the raw material. Examples: `Pallet`, `Canister`, `Cartridge`, `Tank`, `Bin`, `Roll`, and `Spool`.";
        
        /// <summary>
        /// Dimension of material currently in raw material.
        /// </summary>
        public const string CurrentDimensions = "Dimension of material currently in raw material.";
        
        /// <summary>
        /// Quantity of material currently in raw material.
        /// </summary>
        public const string CurrentQuantity = "Quantity of material currently in raw material.";
        
        /// <summary>
        /// Amount of material currently in raw material.
        /// </summary>
        public const string CurrentVolume = "Amount of material currently in raw material.";
        
        /// <summary>
        /// Date raw material was first used.
        /// </summary>
        public const string FirstUseDate = "Date raw material was first used.";
        
        /// <summary>
        /// Form of the raw material.
        /// </summary>
        public const string Form = "Form of the raw material.";
        
        /// <summary>
        /// Material has existing usable volume.
        /// </summary>
        public const string HasMaterial = "Material has existing usable volume.";
        
        /// <summary>
        /// Dimension of material initially placed in raw material when manufactured.
        /// </summary>
        public const string InitialDimensions = "Dimension of material initially placed in raw material when manufactured.";
        
        /// <summary>
        /// Quantity of material initially placed in raw material when manufactured.
        /// </summary>
        public const string InitialQuantity = "Quantity of material initially placed in raw material when manufactured.";
        
        /// <summary>
        /// Amount of material initially placed in raw material when manufactured.
        /// </summary>
        public const string InitialVolume = "Amount of material initially placed in raw material when manufactured.";
        
        /// <summary>
        /// Date raw material was last used.
        /// </summary>
        public const string LastUseDate = "Date raw material was last used.";
        
        /// <summary>
        /// Date the raw material was created.
        /// </summary>
        public const string ManufacturingDate = "Date the raw material was created.";
        
        /// <summary>
        /// Material used as the RawMaterial.
        /// </summary>
        public const string Material = "Material used as the RawMaterial.";
        
        /// <summary>
        /// Name of the raw material.Examples: `Container1` and `AcrylicContainer`.
        /// </summary>
        public const string Name = "Name of the raw material.Examples: `Container1` and `AcrylicContainer`.";
        
        /// <summary>
        /// ISO process type supported by this raw material. Examples include: `VAT_POLYMERIZATION`, `BINDER_JETTING`, `MATERIAL_EXTRUSION`, `MATERIAL_JETTING`, `SHEET_LAMINATION`, `POWDER_BED_FUSION` and `DIRECTED_ENERGY_DEPOSITION`.
        /// </summary>
        public const string ProcessKind = "ISO process type supported by this raw material. Examples include: `VAT_POLYMERIZATION`, `BINDER_JETTING`, `MATERIAL_EXTRUSION`, `MATERIAL_JETTING`, `SHEET_LAMINATION`, `POWDER_BED_FUSION` and `DIRECTED_ENERGY_DEPOSITION`.";
        
        /// <summary>
        /// Serial number of the raw material.
        /// </summary>
        public const string SerialNumber = "Serial number of the raw material.";
    }
}