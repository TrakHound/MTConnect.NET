// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Component")]
    public class XmlComponent
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlComponent));


        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlIgnore]
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

        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        [XmlElement("Description")]
        public XmlDescription Description { get; set; }

        [XmlElement("Configuration")]
        public XmlConfiguration Configuration { get; set; }

        [XmlArray("DataItems")]
        [XmlArrayItem("DataItem")]
        public List<XmlDataItem> DataItems { get; set; }

        [XmlElement("Components")]
        public XmlComponentCollection ComponentCollection { get; set; }

        [XmlArray("Compositions")]
        [XmlArrayItem("Composition")]
        public List<XmlComposition> Compositions { get; set; }

        [XmlArray("References")]
        [XmlArrayItem("ComponentRef", typeof(XmlComponentReference))]
        [XmlArrayItem("DataItemRef", typeof(XmlDataItemReference))]
        public List<XmlReference> References { get; set; }

        [XmlIgnore]
        public string TypeDescription { get; set; }


        public Component ToComponent(IDevice device = null)
        {
            var component = Component.Create(Type);
            if (component == null) component = new Component();

            component.Id = Id;
            component.Uuid = Uuid;
            component.Name = Name;
            component.NativeName = NativeName;
            component.Type = Type;
            component.SampleRate = SampleRate;
            component.SampleInterval = SampleInterval;

            if (Description != null) component.Description = Description.ToDescription();
            if (Configuration != null) component.Configuration = Configuration.ToConfiguration();

            // References
            if (!References.IsNullOrEmpty())
            {
                var references = new List<IReference>();
                foreach (var reference in References)
                {
                    references.Add(reference.ToReference());
                }
                component.References = references;
            }

            // DataItems
            if (!DataItems.IsNullOrEmpty())
            {
                var dataItems = new List<IDataItem>();
                foreach (var xmlDataItem in DataItems)
                {
                    var dataItem = xmlDataItem.ToDataItem();
                    dataItem.Container = component;
                    dataItem.Device = device;
                    dataItems.Add(dataItem);
                }
                component.DataItems = dataItems;
            }

            // Compositions
            if (!Compositions.IsNullOrEmpty())
            {
                var compositions = new List<IComposition>();
                foreach (var xmlComposition in Compositions)
                {
                    var composition = xmlComposition.ToComposition(device);
                    composition.Parent = component;
                    compositions.Add(composition);
                }
                component.Compositions = compositions;
            }

            // Components
            if (ComponentCollection != null && !ComponentCollection.Components.IsNullOrEmpty())
            {
                var components = new List<IComponent>();
                foreach (var xmlSubcomponent in ComponentCollection.Components)
                {
                    var subcomponent = xmlSubcomponent.ToComponent(device);
                    subcomponent.Parent = component;
                    components.Add(subcomponent);
                }
                component.Components = components;
            }

            return component;
        }

        public static IComponent FromXml(byte[] xmlBytes)
        {
            if (xmlBytes != null && xmlBytes.Length > 0)
            {
                try
                {
                    // Clean whitespace and Encoding Marks (BOM)
                    var bytes = XmlFunctions.SanitizeBytes(xmlBytes);

                    var xml = System.Text.Encoding.UTF8.GetString(bytes);
                    xml = XmlFunctions.FormatXml(xml, false, false, true);

                    var typeRegex = "^<(\\S*).*<\\/(.*)>$";
                    var type = new Regex(typeRegex).Match(xml).Groups[1]?.ToString();

                    if (type != null)
                    {
                        xml = xml.Replace($"<{type}", "<Component");
                        xml = xml.Replace($"</{type}>", "</Component>");

                        bytes = System.Text.Encoding.UTF8.GetBytes(xml);

                        using (var memoryReader = new MemoryStream(bytes))
                        {
                            //var doc = XDocument.Load(memoryReader);
                            //if (doc != null)
                            //{
                            //    doc.Save(memoryReader);
                            //}

                            var readerSettings = new XmlReaderSettings();
                            readerSettings.ConformanceLevel = ConformanceLevel.Fragment;
                            readerSettings.IgnoreComments = true;
                            readerSettings.IgnoreProcessingInstructions = true;
                            readerSettings.IgnoreWhitespace = true;

                            using (var xmlReader = XmlReader.Create(memoryReader, readerSettings))
                            {
                                var xmlObj = (XmlComponent)_serializer.Deserialize(xmlReader);
                                if (xmlObj != null)
                                {
                                    xmlObj.Type = type;

                                    return xmlObj.ToComponent();
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        //public static IComponent FromXml(byte[] xmlBytes)
        //{
        //    if (xmlBytes != null && xmlBytes.Length > 0)
        //    {
        //        try
        //        {
        //            using (var textReader = new MemoryStream(xmlBytes))
        //            {
        //                var settings = new XmlReaderSettings();
        //                settings.ConformanceLevel = ConformanceLevel.Fragment;

        //                using (var xmlReader = XmlReader.Create(textReader, settings))
        //                {
        //                    var xmlObj = (XmlComponent)_serializer.Deserialize(xmlReader);
        //                    if (xmlObj != null)
        //                    {
        //                        return xmlObj.ToComponent();
        //                    }
        //                }
        //            }
        //        }
        //        catch { }
        //    }

        //    return null;
        //}

        public static void WriteXml(XmlWriter writer, IComponent component, bool outputComments = false)
        {
            if (component != null)
            {
                // Add Comments
                if (outputComments && component != null)
                {
                    // Write Component Type Description as Comment
                    if (!string.IsNullOrEmpty(component.TypeDescription))
                    {
                        writer.WriteComment($"Type = {component.Type} : {component.TypeDescription}");
                    }
                }


                writer.WriteStartElement(component.Type);

                // Write Properties
                writer.WriteAttributeString("id", component.Id);
                if (!string.IsNullOrEmpty(component.Name)) writer.WriteAttributeString("name", component.Name);
                if (!string.IsNullOrEmpty(component.Uuid)) writer.WriteAttributeString("uuid", component.Uuid);
                if (!string.IsNullOrEmpty(component.NativeName)) writer.WriteAttributeString("nativeName", component.NativeName);
                if (component.SampleInterval > 0) writer.WriteAttributeString("sampleInterval", component.SampleInterval.ToString());
                if (component.SampleRate > 0) writer.WriteAttributeString("sampleRate", component.SampleRate.ToString());
                if (!string.IsNullOrEmpty(component.CoordinateSystemIdRef)) writer.WriteAttributeString("coordinateSystemIdRef", component.CoordinateSystemIdRef);

                // Write Description
                XmlDescription.WriteXml(writer, component.Description);

                // Write Configuration
                XmlConfiguration.WriteXml(writer, component.Configuration, outputComments);

                // Write References
                if (!component.References.IsNullOrEmpty())
                {
                    writer.WriteStartElement("References");
                    foreach (var reference in component.References)
                    {
                        XmlReference.WriteXml(writer, reference);
                    }
                    writer.WriteEndElement();
                }

                // Write DataItems
                if (!component.DataItems.IsNullOrEmpty())
                {
                    writer.WriteStartElement("DataItems");
                    foreach (var dataItem in component.DataItems)
                    {
                        XmlDataItem.WriteXml(writer, dataItem, outputComments);
                    }
                    writer.WriteEndElement();
                }

                // Write Compositions
                if (!component.Compositions.IsNullOrEmpty())
                {
                    writer.WriteStartElement("Compositions");
                    foreach (var composition in component.Compositions)
                    {
                        XmlComposition.WriteXml(writer, composition, outputComments);
                    }
                    writer.WriteEndElement();
                }

                // Write Components
                if (!component.Components.IsNullOrEmpty())
                {
                    writer.WriteStartElement("Components");
                    foreach (var subComponent in component.Components)
                    {
                        WriteXml(writer, subComponent, outputComments);
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}