// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1618829902716_470188_214

namespace MTConnect.Assets.RawMaterials
{
    /// <summary>
    /// Asset that represents raw material.
    /// </summary>
    public partial class RawMaterial : Asset, IRawMaterial
    {
        public new const string DescriptionText = "Asset that represents raw material.";


        /// <summary>
        /// Type of container holding the raw material. Examples: `Pallet`, `Canister`, `Cartridge`, `Tank`, `Bin`, `Roll`, and `Spool`.
        /// </summary>
        public string ContainerType { get; set; }
        
        /// <summary>
        /// Dimension of material currently in raw material.
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> CurrentDimensions { get; set; }
        
        /// <summary>
        /// Quantity of material currently in raw material.
        /// </summary>
        public int CurrentQuantity { get; set; }
        
        /// <summary>
        /// Amount of material currently in raw material.
        /// </summary>
        public string CurrentVolume { get; set; }
        
        /// <summary>
        /// Date raw material was first used.
        /// </summary>
        public System.DateTime FirstUseDate { get; set; }
        
        /// <summary>
        /// Form of the raw material.
        /// </summary>
        public MTConnect.Assets.RawMaterials.Form Form { get; set; }
        
        /// <summary>
        /// Material has existing usable volume.
        /// </summary>
        public bool HasMaterial { get; set; }
        
        /// <summary>
        /// Dimension of material initially placed in raw material when manufactured.
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> InitialDimensions { get; set; }
        
        /// <summary>
        /// Quantity of material initially placed in raw material when manufactured.
        /// </summary>
        public int InitialQuantity { get; set; }
        
        /// <summary>
        /// Amount of material initially placed in raw material when manufactured.
        /// </summary>
        public string InitialVolume { get; set; }
        
        /// <summary>
        /// Date raw material was last used.
        /// </summary>
        public System.DateTime LastUseDate { get; set; }
        
        /// <summary>
        /// Date the raw material was created.
        /// </summary>
        public System.DateTime ManufacturingDate { get; set; }
        
        /// <summary>
        /// Material used as the RawMaterial.
        /// </summary>
        public MTConnect.Assets.RawMaterials.IMaterial Material { get; set; }
        
        /// <summary>
        /// Name of the raw material.Examples: `Container1` and `AcrylicContainer`.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// ISO process type supported by this raw material. Examples include: `VAT_POLYMERIZATION`, `BINDER_JETTING`, `MATERIAL_EXTRUSION`, `MATERIAL_JETTING`, `SHEET_LAMINATION`, `POWDER_BED_FUSION` and `DIRECTED_ENERGY_DEPOSITION`.
        /// </summary>
        public string ProcessKind { get; set; }
        
        /// <summary>
        /// Serial number of the raw material.
        /// </summary>
        public string SerialNumber { get; set; }
    }
}