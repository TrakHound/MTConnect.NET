// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for a <c>CuttingToolArchetypeReference</c>,
    /// linking a CuttingTool instance to the archetype that defines its
    /// reusable, instance-independent characteristics.
    /// </summary>
    public class XmlCuttingToolArchetypeReference
    {
        /// <summary>
        /// An optional URI locating the archetype definition, carried by the
        /// <c>source</c> attribute.
        /// </summary>
        [XmlAttribute("source")]
        public string Source { get; set; }

        /// <summary>
        /// The <c>assetId</c> of the referenced cutting tool archetype, carried
        /// as the element's text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="CuttingToolArchetypeReference"/>, copying the source URI
        /// and referenced archetype id.
        /// </summary>
        public ICuttingToolArchetypeReference ToCuttingToolArchetypeReference()
        {
            var location = new CuttingToolArchetypeReference();
            location.Source = Source;
            location.Value = Value;
            return location;
        }

        /// <summary>
        /// Writes the <c>CuttingToolArchetypeReference</c> element, emitting the
        /// optional <c>source</c> attribute and the archetype id as element
        /// content.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ICuttingToolArchetypeReference cuttingToolArchetypeReference)
        {
            if (cuttingToolArchetypeReference != null)
            {
                writer.WriteStartElement("CuttingToolArchetypeReference");
                if (!string.IsNullOrEmpty(cuttingToolArchetypeReference.Source)) writer.WriteAttributeString("source", cuttingToolArchetypeReference.Source);
                writer.WriteString(cuttingToolArchetypeReference.Value);
                writer.WriteEndElement();
            }
        }
    }
}