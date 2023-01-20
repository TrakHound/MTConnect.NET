// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.References;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Composition")]
    public class XmlComposition
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("nativeName")]
        public string NativeName { get; set; }

        [XmlAttribute("sampleInterval")]
        public double SampleInterval { get; set; }

        [XmlAttribute("sampleRate")]
        public double SampleRate { get; set; }

        [XmlAttribute("uuid")]
        public string Uuid { get; set; }

        [XmlElement("Description")]
        public XmlDescription Description { get; set; }

        [XmlElement("Configuration")]
        public XmlConfiguration Configuration { get; set; }

        [XmlArray("References")]
        [XmlArrayItem("ComponentRef", typeof(XmlComponentReference))]
        [XmlArrayItem("DataItemRef", typeof(XmlDataItemReference))]
        public List<XmlReference> References { get; set; }


        public Composition ToComposition(IDevice device = null)
        {
            var composition = Composition.Create(Type);
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
