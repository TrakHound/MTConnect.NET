// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    [XmlRoot("CuttingTool")]
    public class XmlCuttingToolAsset : XmlAsset
    {
        [XmlAttribute("serialNumber")]
        public string SerialNumber { get; set; }

        [XmlAttribute("toolId")]
        public string ToolId { get; set; }

        [XmlAttribute("manufacturers")]
        public string Manufacturers { get; set; }

        [XmlElement("CuttingToolLifeCycle")]
        public XmlCuttingToolLifeCycle CuttingToolLifeCycle { get; set; }

        [XmlElement("cuttingToolArchetypeReference")]
        public XmlCuttingToolArchetypeReference CuttingToolArchetypeReference { get; set; }

        [XmlElement("CuttingToolDefinition")]
        public XmlCuttingToolDefinition CuttingToolDefinition { get; set; }


        public override IAsset ToAsset()
        {
            var asset = new CuttingToolAsset();
            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            //if (Description != null) asset.Description = Description.ToDescription();

            asset.SerialNumber = SerialNumber;
            asset.ToolId = ToolId;

            if (!string.IsNullOrEmpty(Manufacturers))
            {
                asset.Manufacturers = Manufacturers.Split(',');
            }

            if (CuttingToolLifeCycle != null) asset.CuttingToolLifeCycle = CuttingToolLifeCycle.ToCuttingToolLifeCycle();
            if (CuttingToolArchetypeReference != null) asset.CuttingToolArchetypeReference = CuttingToolArchetypeReference.ToCuttingToolArchetypeReference();
            if (CuttingToolDefinition != null) asset.CuttingToolDefinition = CuttingToolDefinition.ToCuttingToolDefinition();
            return asset;
        }

        public static void WriteXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null)
            {
                var cuttingTool = (ICuttingToolAsset)asset;

                writer.WriteStartElement("CuttingTool");

                WriteCommonXml(writer, asset);

                // Write Properties
                if (!string.IsNullOrEmpty(cuttingTool.SerialNumber)) writer.WriteAttributeString("serialNumber", cuttingTool.SerialNumber);
                if (!string.IsNullOrEmpty(cuttingTool.ToolId)) writer.WriteAttributeString("toolId", cuttingTool.ToolId);

                // Write Manufacturers
                if (!cuttingTool.Manufacturers.IsNullOrEmpty())
                {
                    var manufacturers = string.Join(",", cuttingTool.Manufacturers);
                    if (!string.IsNullOrEmpty(manufacturers))
                    {
                        writer.WriteAttributeString("manufacturers", manufacturers);
                    }
                }

                // Write CuttingToolLifeCycle
                if (cuttingTool.CuttingToolLifeCycle != null)
                {
                    XmlCuttingToolLifeCycle.WriteXml(writer, cuttingTool.CuttingToolLifeCycle);
                }

                // Write CuttingToolArchetypeReference
                if (cuttingTool.CuttingToolArchetypeReference != null)
                {
                    XmlCuttingToolArchetypeReference.WriteXml(writer, cuttingTool.CuttingToolArchetypeReference);
                }

                // Write CuttingToolDefinition
                if (cuttingTool.CuttingToolDefinition != null)
                {
                    XmlCuttingToolDefinition.WriteXml(writer, cuttingTool.CuttingToolDefinition);
                }

                writer.WriteEndElement();
            }
        }
    }
}