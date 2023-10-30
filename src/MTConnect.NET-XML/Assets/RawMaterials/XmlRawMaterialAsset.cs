// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.RawMaterials
{
    [XmlRoot("RawMaterial")]
    public class XmlRawMaterialAsset : XmlAsset
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("containerType")]
        public string ContainerType { get; set; }

        [XmlAttribute("processKind")]
        public string ProcessKind { get; set; }

        [XmlAttribute("serialNumber")]
        public string SerialNumber { get; set; }

        [XmlElement("Form")]
        public string Form { get; set; }

        [XmlElement("HasMaterial")]
        public string HasMaterial { get; set; }

        [XmlElement("ManufacturingDate")]
        public string ManufacturingDate { get; set; }

        [XmlElement("FirstUseDate")]
        public string FirstUseDate { get; set; }

        [XmlElement("LastUseDate")]
        public string LastUseDate { get; set; }

        [XmlElement("InitialVolume")]
        public string InitialVolume { get; set; }

        [XmlElement("InitialDimension")]
        public string InitialDimension { get; set; }

        [XmlElement("InitialQuantity")]
        public string InitialQuantity { get; set; }

        [XmlElement("CurrentVolume")]
        public string CurrentVolume { get; set; }

        [XmlElement("CurrentDimension")]
        public string CurrentDimension { get; set; }

        [XmlElement("CurrentQuantity")]
        public string CurrentQuantity { get; set; }

        [XmlElement("Material")]
        public XmlMaterial Material { get; set; }


        public override IRawMaterialAsset ToAsset()
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

        public static void WriteXml(XmlWriter writer, IAsset asset)
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