// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for a CuttingTool <c>CuttingToolDefinition</c>,
    /// the vendor-specific tool definition payload whose encoding is described
    /// by its format.
    /// </summary>
    public class XmlCuttingToolDefinition
    {
        /// <summary>
        /// The encoding of the definition payload, carried by the
        /// <c>format</c> attribute (for example <c>XML</c> or <c>TEXT</c>).
        /// </summary>
        [XmlAttribute("format")]
        public string Format { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="CuttingToolDefinition"/>, mapping the format string to
        /// its enumeration.
        /// </summary>
        public ICuttingToolDefinition ToCuttingToolDefinition()
        {
            var cuttingToolDefinition = new CuttingToolDefinition();
            cuttingToolDefinition.Format = Format.ConvertEnum<FormatType>();
            return cuttingToolDefinition;
        }

        /// <summary>
        /// Writes the <c>CuttingToolDefinition</c> element, emitting the
        /// <c>format</c> attribute.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ICuttingToolDefinition cuttingToolDefinition)
        {
            if (cuttingToolDefinition != null)
            {
                writer.WriteStartElement("CuttingToolDefinition");
                writer.WriteAttributeString("format", cuttingToolDefinition.Format.ToString());
                writer.WriteEndElement();
            }
        }
    }
}