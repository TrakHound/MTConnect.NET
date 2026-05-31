// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>CuttingToolArchetype</c>
    /// asset, the template a physical cutting tool is instantiated from.
    /// Mirrors the on-the-wire element and converts to and from the
    /// strongly-typed <see cref="CuttingToolArchetypeAsset"/> model.
    /// </summary>
    [XmlRoot("CuttingToolArchetype")]
    public class XmlCuttingToolArchetypeAsset : XmlAsset
    {
        /// <summary>
        /// The serial number of the archetype.
        /// </summary>
        [XmlAttribute("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The identifier the tool is referenced by in programs.
        /// </summary>
        [XmlAttribute("toolId")]
        public string ToolId { get; set; }

        /// <summary>
        /// The manufacturers of the archetype, as a comma-separated list.
        /// </summary>
        [XmlAttribute("manufacturers")]
        public string Manufacturers { get; set; }

        /// <summary>
        /// The vendor-specific definition document describing the tool.
        /// </summary>
        [XmlAttribute("CuttingToolDefinition")]
        public XmlCuttingToolDefinition CuttingToolDefinition { get; set; }

        /// <summary>
        /// The default life-cycle state the archetype prescribes.
        /// </summary>
        [XmlElement("CuttingToolLifeCycle")]
        public XmlCuttingToolLifeCycle CuttingToolLifeCycle { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="CuttingToolArchetypeAsset"/>, splitting the
        /// comma-separated manufacturers and projecting the nested elements.
        /// </summary>
        public override IAsset ToAsset()
        {
            var asset = new CuttingToolArchetypeAsset();
            asset.AssetId = AssetId;
            asset.Type = CuttingToolAsset.TypeId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            asset.SerialNumber = SerialNumber;
            asset.ToolId = ToolId;

            if (!string.IsNullOrEmpty(Manufacturers))
            {
                asset.Manufacturers = Manufacturers.Split(',');
            }

            if (CuttingToolLifeCycle != null) asset.CuttingToolLifeCycle = CuttingToolLifeCycle.ToCuttingToolLifeCycle();
            if (CuttingToolDefinition != null) asset.CuttingToolDefinition = CuttingToolDefinition.ToCuttingToolDefinition();
            return asset;
        }

        /// <summary>
        /// Writes the given <see cref="ICuttingToolArchetypeAsset"/> to
        /// <paramref name="writer"/> as a <c>CuttingToolArchetype</c> element,
        /// joining the manufacturers list and omitting optional elements that
        /// are not set.
        /// </summary>
        public static new void WriteXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null)
            {
                var cuttingTool = (ICuttingToolArchetypeAsset)asset;

                writer.WriteStartElement("CuttingToolArchetype");

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