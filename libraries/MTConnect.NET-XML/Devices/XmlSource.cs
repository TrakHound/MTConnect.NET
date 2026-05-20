// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a data item's <c>Source</c>, which
    /// identifies the component, composition, or data item the value
    /// originates from. Mirrors the on-the-wire element and converts to and
    /// from the strongly-typed <see cref="Source"/> model.
    /// </summary>
    public class XmlSource
    {
        /// <summary>
        /// The <c>id</c> of the component the value originates from.
        /// </summary>
        [XmlAttribute("componentId")]
        public string ComponentId { get; set; }

        /// <summary>
        /// The <c>id</c> of the data item the value originates from.
        /// </summary>
        [XmlAttribute("dataItemId")]
        public string DataItemId { get; set; }

        /// <summary>
        /// The <c>id</c> of the composition the value originates from.
        /// </summary>
        [XmlAttribute("compositionId")]
        public string CompositionId { get; set; }

        /// <summary>
        /// The native source identifier, carried as the element text.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="Source"/>.
        /// </summary>
        public ISource ToSource()
        {
            var source = new Source();
            source.ComponentId = ComponentId;
            source.DataItemId = DataItemId;
            source.CompositionId = CompositionId;
            source.Value = Value;
            return source;
        }

        /// <summary>
        /// Writes the given <see cref="ISource"/> to <paramref name="writer"/>
        /// as a <c>Source</c> element, skipping the element entirely when every
        /// field is empty.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ISource source)
        {
            if (source != null && (!string.IsNullOrEmpty(source.ComponentId) || !string.IsNullOrEmpty(source.CompositionId) || !string.IsNullOrEmpty(source.DataItemId) || !string.IsNullOrEmpty(source.Value)))
            {
                writer.WriteStartElement("Source");
                if (!string.IsNullOrEmpty(source.ComponentId)) writer.WriteAttributeString("componentId", source.ComponentId);
                if (!string.IsNullOrEmpty(source.CompositionId)) writer.WriteAttributeString("compositionId", source.CompositionId);
                if (!string.IsNullOrEmpty(source.DataItemId)) writer.WriteAttributeString("dataItemId", source.DataItemId);
                if (!string.IsNullOrEmpty(source.Value)) writer.WriteString(source.Value);
                writer.WriteEndElement();
            }
        }
    }
}