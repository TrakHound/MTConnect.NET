// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.RawMaterials
{
    /// <summary>
    /// Raw material represents the source of material for immediate use and sources of material that may or may not be used during the manufacturing process.
    /// </summary>
    [XmlRoot("RawMaterial")]
    public class RawMaterialAsset : Asset
    {
        public const string TypeId = "RawMaterial";


        /// <summary>
        /// The raw material name. 
        /// Examples: Container1 and AcrylicContainer
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of container holding the raw material. 
        /// Examples: Pallet, Canister, Cartridge, Tank, Bin, Roll, and Spool.
        /// </summary>
        [XmlAttribute("containerType")]
        [JsonPropertyName("containerType")]
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
        [JsonPropertyName("processKind")]
        public string ProcessKind { get; set; }

        /// <summary>
        /// The serial number of the raw material.
        /// </summary>
        [XmlAttribute("serialNumber")]
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The form of the raw material.
        /// The value MUST be BAR, SHEET, BLOCK,
        /// CASTING, POWDER, LIQUID, GEL,
        /// FILAMENT, or GAS.
        /// </summary>
        [XmlElement("Form")]
        [JsonPropertyName("form")]
        public string Form { get; set; }

        /// <summary>
        /// Material has existing usable volume.
        /// </summary>
        [XmlElement("HasMaterial")]
        [JsonPropertyName("hasMaterial")]
        public bool HasMaterial { get; set; }

        /// <summary>
        /// The date the raw material was created.
        /// </summary>
        [XmlElement("ManufacturingDate")]
        [JsonPropertyName("manufacturingDate")]
        public DateTime ManufacturingDate { get; set; }

        /// <summary>
        /// The date raw material was first used.
        /// </summary>
        [XmlElement("FirstUseDate")]
        [JsonPropertyName("firstUseDate")]
        public DateTime FirstUseDate { get; set; }

        /// <summary>
        /// The date raw material was last used.
        /// </summary>
        [XmlElement("LastUseDate")]
        [JsonPropertyName("lastUseDate")]
        public DateTime LastUseDate { get; set; }

        /// <summary>
        /// The amount of material initially placed in raw material when manufactured.
        /// </summary>
        [XmlElement("InitialVolume")]
        [JsonPropertyName("initialVolume")]
        public double InitialVolume { get; set; }

        /// <summary>
        /// The dimension of material initially placed in raw material when manufactured.
        /// </summary>
        [XmlElement("InitialDimension")]
        [JsonPropertyName("initialDimension")]
        public string InitialDimension { get; set; }

        /// <summary>
        /// The quantity of material initially placed in raw material when manufactured.
        /// </summary>
        [XmlElement("InitialQuantity")]
        [JsonPropertyName("initialQuantity")]
        public int InitialQuantity { get; set; }

        /// <summary>
        /// The amount of material currently in raw material.
        /// </summary>
        [XmlElement("CurrentVolume")]
        [JsonPropertyName("currentVolume")]
        public double CurrentVolume { get; set; }

        /// <summary>
        /// The dimension of material currently in raw material.
        /// </summary>
        [XmlElement("CurrentDimension")]
        [JsonPropertyName("currentDimension")]
        public string CurrentDimension { get; set; }

        /// <summary>
        /// The quantity of material currently in raw material.
        /// </summary>
        [XmlElement("CurrentQuantity")]
        [JsonPropertyName("currentQuantity")]
        public int CurrentQuantity { get; set; }

        /// <summary>
        /// Material used as the raw material.
        /// </summary>
        [XmlElement("Material")]
        [JsonPropertyName("material")]
        public Material Material { get; set; }


        public RawMaterialAsset()
        {
            Type = TypeId;
        }


        protected override IAsset OnProcess(Version mtconnectVersion)
        {
            if (mtconnectVersion != null && mtconnectVersion >= MTConnectVersions.Version18)
            {
                return this;
            }

            return null;
        }

        public override AssetValidationResult IsValid(Version mtconnectVersion)
        {
            var message = "";
            var result = true;

            if (string.IsNullOrEmpty(Form))
            {
                message = "Form property is Required";
                result = false;
            }
            else
            {
                if (Material != null && string.IsNullOrEmpty(Material.Type))
                {
                    message = "Material Type property is Required";
                    result = false;
                }
            }

            return new AssetValidationResult(result, message);
        }
    }
}