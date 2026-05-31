// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.RawMaterials
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>RawMaterial</c> asset.
    /// Mirrors the on-the-wire element so the XML serializer can read and write
    /// it, then converts to and from the strongly-typed
    /// <see cref="RawMaterialAsset"/> model.
    /// </summary>
    [XmlRoot("RawMaterial")]
    public class XmlRawMaterialAsset : XmlAsset
    {
        /// <summary>
        /// The optional human-readable name of the raw material.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of container holding the raw material, such as a bottle or
        /// cartridge.
        /// </summary>
        [XmlAttribute("containerType")]
        public string ContainerType { get; set; }

        /// <summary>
        /// The manufacturing process the raw material is intended for.
        /// </summary>
        [XmlAttribute("processKind")]
        public string ProcessKind { get; set; }

        /// <summary>
        /// The serial number that uniquely identifies this raw material.
        /// </summary>
        [XmlAttribute("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The physical form of the material (for example <c>BAR</c>,
        /// <c>BLOCK</c>, or <c>POWDER</c>) as the raw element text.
        /// </summary>
        [XmlElement("Form")]
        public string Form { get; set; }

        /// <summary>
        /// Whether the container currently holds material, as the raw boolean
        /// element text.
        /// </summary>
        [XmlElement("HasMaterial")]
        public string HasMaterial { get; set; }

        /// <summary>
        /// The date the raw material was manufactured, as the raw element text.
        /// </summary>
        [XmlElement("ManufacturingDate")]
        public string ManufacturingDate { get; set; }

        /// <summary>
        /// The date the raw material was first used, as the raw element text.
        /// </summary>
        [XmlElement("FirstUseDate")]
        public string FirstUseDate { get; set; }

        /// <summary>
        /// The date the raw material was last used, as the raw element text.
        /// </summary>
        [XmlElement("LastUseDate")]
        public string LastUseDate { get; set; }

        /// <summary>
        /// The volume of unused material when first received, as the raw
        /// element text.
        /// </summary>
        [XmlElement("InitialVolume")]
        public string InitialVolume { get; set; }

        /// <summary>
        /// The dimension of unused material when first received, as the raw
        /// element text.
        /// </summary>
        [XmlElement("InitialDimension")]
        public string InitialDimension { get; set; }

        /// <summary>
        /// The number of individual pieces of material when first received, as
        /// the raw element text.
        /// </summary>
        [XmlElement("InitialQuantity")]
        public string InitialQuantity { get; set; }

        /// <summary>
        /// The current volume of unused material, as the raw element text.
        /// </summary>
        [XmlElement("CurrentVolume")]
        public string CurrentVolume { get; set; }

        /// <summary>
        /// The current dimension of unused material, as the raw element text.
        /// </summary>
        [XmlElement("CurrentDimension")]
        public string CurrentDimension { get; set; }

        /// <summary>
        /// The current number of individual pieces of material, as the raw
        /// element text.
        /// </summary>
        [XmlElement("CurrentQuantity")]
        public string CurrentQuantity { get; set; }

        /// <summary>
        /// The material the raw material is made of.
        /// </summary>
        [XmlElement("Material")]
        public XmlMaterial Material { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="RawMaterialAsset"/>, parsing each raw element value into
        /// its model representation.
        /// </summary>
        public override IAsset ToAsset()
        {
            var asset = new RawMaterialAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            //if (Description != null) asset.Description = Description.ToDescription();

            asset.Name = Name;
            asset.ContainerType = ContainerType;
            asset.ProcessKind = ProcessKind;
            asset.SerialNumber = SerialNumber;
            asset.Form = Form.ConvertEnum<Form>();
            if (!string.IsNullOrEmpty(HasMaterial)) asset.HasMaterial = HasMaterial.ToBoolean();
            if (!string.IsNullOrEmpty(ManufacturingDate)) asset.ManufacturingDate = ManufacturingDate.ToDateTime();
            if (!string.IsNullOrEmpty(FirstUseDate)) asset.FirstUseDate = FirstUseDate.ToDateTime();
            if (!string.IsNullOrEmpty(LastUseDate)) asset.LastUseDate = LastUseDate.ToDateTime();

            if (!string.IsNullOrEmpty(CurrentVolume)) asset.CurrentVolume = CurrentVolume.ToDouble();
            if (!string.IsNullOrEmpty(CurrentDimension)) asset.CurrentDimension = Millimeter3D.FromString(CurrentDimension);
            if (!string.IsNullOrEmpty(CurrentQuantity)) asset.CurrentQuantity = CurrentQuantity.ToInt();

            if (!string.IsNullOrEmpty(InitialVolume)) asset.InitialVolume = InitialVolume.ToDouble();
            if (!string.IsNullOrEmpty(InitialDimension)) asset.InitialDimension = Millimeter3D.FromString(InitialDimension);
            if (!string.IsNullOrEmpty(InitialQuantity)) asset.InitialQuantity = InitialQuantity.ToInt();

            if (Material != null)
            {
                asset.Material = Material.ToMaterial();
            }

            return asset;
        }

        /// <summary>
        /// Writes the given <see cref="IRawMaterialAsset"/> to <paramref name="writer"/>
        /// as a <c>RawMaterial</c> element, omitting optional values that are
        /// not set.
        /// </summary>
        public static new void WriteXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null)
            {
                var rawMaterial = (IRawMaterialAsset)asset;

                writer.WriteStartElement("RawMaterial");

                WriteCommonXml(writer, asset);

                // Write Properties
                if (!string.IsNullOrEmpty(rawMaterial.Name)) writer.WriteAttributeString("name", rawMaterial.Name);
                if (!string.IsNullOrEmpty(rawMaterial.ContainerType)) writer.WriteAttributeString("containerType", rawMaterial.ContainerType);
                if (!string.IsNullOrEmpty(rawMaterial.ProcessKind)) writer.WriteAttributeString("processKind", rawMaterial.ProcessKind);
                if (!string.IsNullOrEmpty(rawMaterial.SerialNumber)) writer.WriteAttributeString("serialNumber", rawMaterial.SerialNumber);

                // Form
                writer.WriteStartElement("Form");
                writer.WriteString(rawMaterial.Form.ToString());
                writer.WriteEndElement();

                // HasMaterial
                if (rawMaterial.HasMaterial != null)
                {
                    writer.WriteStartElement("HasMaterial");
                    writer.WriteString(rawMaterial.HasMaterial.ToString());
                    writer.WriteEndElement();
                }

                // ManufacturingDate
                if (rawMaterial.ManufacturingDate != null)
                {
                    writer.WriteStartElement("ManufacturingDate");
                    writer.WriteString(rawMaterial.ManufacturingDate?.ToString("o"));
                    writer.WriteEndElement();
                }

                // FirstUseDate
                if (rawMaterial.FirstUseDate != null)
                {
                    writer.WriteStartElement("FirstUseDate");
                    writer.WriteString(rawMaterial.FirstUseDate?.ToString("o"));
                    writer.WriteEndElement();
                }

                // LastUseDate
                if (rawMaterial.LastUseDate != null)
                {
                    writer.WriteStartElement("LastUseDate");
                    writer.WriteString(rawMaterial.LastUseDate?.ToString("o"));
                    writer.WriteEndElement();
                }

                // InitialVolume
                if (rawMaterial.InitialVolume != null)
                {
                    writer.WriteStartElement("InitialVolume");
                    writer.WriteString(rawMaterial.InitialVolume.ToString());
                    writer.WriteEndElement();
                }

                // InitialDimension
                if (rawMaterial.InitialDimension != null)
                {
                    writer.WriteStartElement("InitialDimension");
                    writer.WriteString(rawMaterial.InitialDimension.ToString());
                    writer.WriteEndElement();
                }

                // InitialQuantity
                if (rawMaterial.InitialQuantity != null)
                {
                    writer.WriteStartElement("InitialQuantity");
                    writer.WriteString(rawMaterial.InitialQuantity.ToString());
                    writer.WriteEndElement();
                }

                // CurrentVolume
                if (rawMaterial.CurrentVolume != null)
                {
                    writer.WriteStartElement("CurrentVolume");
                    writer.WriteString(rawMaterial.CurrentVolume.ToString());
                    writer.WriteEndElement();
                }

                // CurrentDimension
                if (rawMaterial.CurrentDimension != null)
                {
                    writer.WriteStartElement("CurrentDimension");
                    writer.WriteString(rawMaterial.CurrentDimension.ToString());
                    writer.WriteEndElement();
                }

                // CurrentQuantity
                if (rawMaterial.CurrentQuantity != null)
                {
                    writer.WriteStartElement("CurrentQuantity");
                    writer.WriteString(rawMaterial.CurrentQuantity.ToString());
                    writer.WriteEndElement();
                }


                // Write Material
                if (rawMaterial.Material != null)
                {
                    XmlMaterial.WriteXml(writer, rawMaterial.Material);
                }

                writer.WriteEndElement();
            }
        }
    }
}