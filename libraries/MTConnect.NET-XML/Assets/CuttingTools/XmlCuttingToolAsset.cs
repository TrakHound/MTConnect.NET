// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>CuttingTool</c> asset.
    /// Mirrors the on-the-wire element and converts to and from the
    /// strongly-typed <see cref="CuttingToolAsset"/> model.
    /// </summary>
    [XmlRoot("CuttingTool")]
    public class XmlCuttingToolAsset : XmlAsset
    {
        /// <summary>
        /// The serial number that uniquely identifies the physical tool.
        /// </summary>
        [XmlAttribute("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The identifier the tool is referenced by in programs.
        /// </summary>
        [XmlAttribute("toolId")]
        public string ToolId { get; set; }

        /// <summary>
        /// The manufacturers of the tool, as a comma-separated list.
        /// </summary>
        [XmlAttribute("manufacturers")]
        public string Manufacturers { get; set; }

        /// <summary>
        /// The measured state of the tool, including its life counters and
        /// cutting items.
        /// </summary>
        [XmlElement("CuttingToolLifeCycle")]
        public XmlCuttingToolLifeCycle CuttingToolLifeCycle { get; set; }

        /// <summary>
        /// The reference to the archetype this tool is an instance of.
        /// </summary>
        [XmlElement("cuttingToolArchetypeReference")]
        public XmlCuttingToolArchetypeReference CuttingToolArchetypeReference { get; set; }

        /// <summary>
        /// The vendor-specific definition document describing the tool.
        /// </summary>
        [XmlElement("CuttingToolDefinition")]
        public XmlCuttingToolDefinition CuttingToolDefinition { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="CuttingToolAsset"/>, splitting the comma-separated
        /// manufacturers and projecting the nested elements.
        /// </summary>
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

        /// <summary>
        /// Writes the given <see cref="ICuttingToolAsset"/> to
        /// <paramref name="writer"/> as a <c>CuttingTool</c> element, joining
        /// the manufacturers list and omitting optional elements that are not
        /// set.
        /// </summary>
        public static new void WriteXml(XmlWriter writer, IAsset asset)
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