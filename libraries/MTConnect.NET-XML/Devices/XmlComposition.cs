// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>Composition</c>, a
    /// lower-level functional building block of a component. Mirrors the
    /// on-the-wire element and converts to and from the strongly-typed
    /// <see cref="MTConnect.Devices.Composition"/> model.
    /// </summary>
    [XmlRoot("Composition")]
    public class XmlComposition
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlComposition));


        /// <summary>
        /// The unique identifier of the composition within the device.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The MTConnect composition type, such as <c>MOTOR</c> or
        /// <c>SENSING_ELEMENT</c>.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The optional human-readable name of the composition.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The name the composition is known by on its native control.
        /// </summary>
        [XmlAttribute("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The interval, in milliseconds, between samples the composition
        /// reports.
        /// </summary>
        [XmlAttribute("sampleInterval")]
        public double SampleInterval { get; set; }

        /// <summary>
        /// The deprecated sample rate, in samples per second; superseded by
        /// <see cref="SampleInterval"/>.
        /// </summary>
        [XmlAttribute("sampleRate")]
        public double SampleRate { get; set; }

        /// <summary>
        /// The optional globally unique identifier of the composition.
        /// </summary>
        [XmlAttribute("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// The free-form description of the composition.
        /// </summary>
        [XmlElement("Description")]
        public XmlDescription Description { get; set; }

        /// <summary>
        /// The configuration metadata that applies to the composition.
        /// </summary>
        [XmlElement("Configuration")]
        public XmlConfiguration Configuration { get; set; }

        /// <summary>
        /// The references to components and data items related to this
        /// composition.
        /// </summary>
        [XmlArray("References")]
        [XmlArrayItem("ComponentRef", typeof(XmlComponentReference))]
        [XmlArrayItem("DataItemRef", typeof(XmlDataItemReference))]
        public List<XmlReference> References { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="MTConnect.Devices.Composition"/>, instantiating the
        /// concrete composition class that matches <see cref="Type"/> when one
        /// exists.
        /// </summary>
        /// <param name="device">The owning device, supplied for context where
        /// the conversion needs it.</param>
        public Composition ToComposition(IDevice device = null)
        {
            var composition = Composition.Create(Type.ToPascalCase());
            if (composition == null) composition = new Composition();

            composition.Id = Id;
            composition.Uuid = Uuid;
            composition.Name = Name;
            composition.NativeName = NativeName;
            composition.Type = Type;
            composition.SampleRate = SampleRate;
            composition.SampleInterval = SampleInterval;

            if (Description != null) composition.Description = Description.ToDescription();
            if (Configuration != null) composition.Configuration = Configuration.ToConfiguration();

            // References
            if (!References.IsNullOrEmpty())
            {
                var references = new List<IReference>();
                foreach (var reference in References)
                {
                    references.Add(reference.ToReference());
                }
                composition.References = references;
            }

            return composition;
        }

        /// <summary>
        /// Deserializes a standalone <c>Composition</c> XML document into a
        /// strongly-typed <see cref="IComposition"/>, returning <c>null</c> if
        /// the input is empty or cannot be parsed.
        /// </summary>
        public static IComposition FromXml(byte[] xmlBytes)
        {
            if (xmlBytes != null && xmlBytes.Length > 0)
            {
                try
                {
                    using (var textReader = new MemoryStream(xmlBytes))
                    {
                        using (var xmlReader = XmlReader.Create(textReader))
                        {
                            var xmlObj = (XmlComposition)_serializer.Deserialize(xmlReader);
                            if (xmlObj != null)
                            {
                                return xmlObj.ToComposition();
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Writes the given <see cref="IComposition"/> to <paramref name="writer"/>
        /// as a <c>Composition</c> element, optionally preceding it with a
        /// comment describing the composition type.
        /// </summary>
        /// <param name="writer">The XML writer to emit to.</param>
        /// <param name="composition">The composition to serialize.</param>
        /// <param name="outputComments">When <c>true</c>, emits the human-readable
        /// type description as an XML comment.</param>
        public static void WriteXml(XmlWriter writer, IComposition composition, bool outputComments = false)
        {
            if (composition != null)
            {
                // Add Comments
                if (outputComments && composition != null)
                {
                    // Write Composition Type Description as Comment
                    if (!string.IsNullOrEmpty(composition.TypeDescription))
                    {
                        writer.WriteComment($"Type = {composition.Type} : {composition.TypeDescription}");
                    }
                }


                writer.WriteStartElement("Composition");

                // Write Properties
                writer.WriteAttributeString("id", composition.Id);
                writer.WriteAttributeString("type", composition.Type);
                if (!string.IsNullOrEmpty(composition.Name)) writer.WriteAttributeString("name", composition.Name);
                if (!string.IsNullOrEmpty(composition.Uuid)) writer.WriteAttributeString("uuid", composition.Uuid);
                if (!string.IsNullOrEmpty(composition.NativeName)) writer.WriteAttributeString("nativeName", composition.NativeName);
                if (composition.SampleInterval > 0) writer.WriteAttributeString("sampleInterval", composition.SampleInterval.ToString());
                if (composition.SampleRate > 0) writer.WriteAttributeString("sampleRate", composition.SampleRate.ToString());
                if (!string.IsNullOrEmpty(composition.CoordinateSystemIdRef)) writer.WriteAttributeString("coordinateSystemIdRef", composition.CoordinateSystemIdRef);

                // Write Description
                XmlDescription.WriteXml(writer, composition.Description);

                // Write Configuration
                XmlConfiguration.WriteXml(writer, composition.Configuration, outputComments);

                // Write References
                if (!composition.References.IsNullOrEmpty())
                {
                    writer.WriteStartElement("References");
                    foreach (var reference in composition.References)
                    {
                        XmlReference.WriteXml(writer, reference);
                    }
                    writer.WriteEndElement();
                }

                // Write DataItems
                if (!composition.DataItems.IsNullOrEmpty())
                {
                    writer.WriteStartElement("DataItems");
                    foreach (var dataItem in composition.DataItems)
                    {
                        XmlDataItem.WriteXml(writer, dataItem, outputComments);
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}