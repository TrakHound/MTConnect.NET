// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.RawMaterials
{
    /// <summary>
    /// Raw material represents the source of material for immediate use and sources of material that may or may not be used during the manufacturing process.
    /// </summary>
    [XmlRoot("RawMaterial")]
    public class RawMaterialAsset : Asset<RawMaterialAsset>
    {
        public const string TypeId = "RawMaterial";


        /// <summary>
        /// The raw material name. 
        /// Examples: Container1 and AcrylicContainer
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of container holding the raw material. 
        /// Examples: Pallet, Canister, Cartridge, Tank, Bin, Roll, and Spool.
        /// </summary>
        [XmlAttribute("containerType")]
        public string ContainerType { get; set; }

        /// <summary>
        /// The ISO process type supported by this raw material.
        /// Examples include: VAT_POLYMERIZATION,
        /// BINDER_JETTING,
        /// MATERIAL_EXTRUSION,
        /// MATERIAL_JETTING,
        /// SHEET_LAMINATION,
        /// POWDER_BED_FUSION, or
        /// DIRECTED_ENERGY_DEPOSITION.
        /// </summary>
        [XmlAttribute("processKind")]
        public string ProcessKind { get; set; }

        /// <summary>
        /// The serial number of the raw material.
        /// </summary>
        [XmlAttribute("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The form of the raw material.
        /// The value MUST be BAR, SHEET, BLOCK,
        /// CASTING, POWDER, LIQUID, GEL,
        /// FILAMENT, or GAS.
        /// </summary>
        [XmlElement("Form")]
        public string Form { get; set; }

        /// <summary>
        /// Material has existing usable volume.
        /// </summary>
        [XmlElement("HasMaterial")]
        public bool HasMaterial { get; set; }

        /// <summary>
        /// The date the raw material was created.
        /// </summary>
        [XmlElement("ManufacturingDate")]
        public DateTime ManufacturingDate { get; set; }

        /// <summary>
        /// The date raw material was first used.
        /// </summary>
        [XmlElement("FirstUseDate")]
        public DateTime FirstUseDate { get; set; }

        /// <summary>
        /// The date raw material was last used.
        /// </summary>
        [XmlElement("LastUseDate")]
        public DateTime LastUseDate { get; set; }

        /// <summary>
        /// The amount of material initially placed in raw material when manufactured.
        /// </summary>
        [XmlElement("InitialVolume")]
        public double InitialVolume { get; set; }

        /// <summary>
        /// The dimension of material initially placed in raw material when manufactured.
        /// </summary>
        [XmlElement("InitialDimension")]
        public string InitialDimension { get; set; }

        /// <summary>
        /// The quantity of material initially placed in raw material when manufactured.
        /// </summary>
        [XmlElement("InitialQuantity")]
        public int InitialQuantity { get; set; }

        /// <summary>
        /// The amount of material currently in raw material.
        /// </summary>
        [XmlElement("CurrentVolume")]
        public double CurrentVolume { get; set; }

        /// <summary>
        /// The dimension of material currently in raw material.
        /// </summary>
        [XmlElement("CurrentDimension")]
        public string CurrentDimension { get; set; }

        /// <summary>
        /// The quantity of material currently in raw material.
        /// </summary>
        [XmlElement("CurrentQuantity")]
        public int CurrentQuantity { get; set; }

        /// <summary>
        /// Material used as the raw material.
        /// </summary>
        [XmlElement("Material")]
        public Material Material { get; set; }


        public RawMaterialAsset()
        {
            Type = TypeId;
        }
    }
}
