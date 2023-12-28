// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
	[XmlRoot("Device")]
    public class XmlDevice
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlDevice));


        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("uuid")]
        public string Uuid { get; set; }

        [XmlAttribute("iso841Class")]
        public string Iso841Class { get; set; }

        [XmlAttribute("nativeName")]
        public string NativeName { get; set; }

        [XmlAttribute("sampleInterval")]
        public double SampleInterval { get; set; }

        [XmlAttribute("sampleRate")]
        public double SampleRate { get; set; }

        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        [XmlAttribute("mtconnectVersion")]
        public string MTConnectVersion { get; set; }

		[XmlAttribute("hash")]
		public string Hash { get; set; }

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


        public virtual IDevice ToDevice()
        {
            var device = new Device();
            return ToDevice(device);
        }

        protected IDevice ToDevice(Device device)
        {
            device.Id = Id;
            device.Name = Name;
            device.NativeName = NativeName;
            device.Uuid = Uuid;
            device.SampleRate = SampleRate;
            device.SampleInterval = SampleInterval;
            device.Iso841Class = Iso841Class;
            device.CoordinateSystemIdRef = CoordinateSystemIdRef;
            device.Hash = Hash;
            if (Version.TryParse(MTConnectVersion, out var mtconnectVersion))
            {
                device.MTConnectVersion = mtconnectVersion;
            }

            if (Description != null) device.Description = Description.ToDescription();
            if (Configuration != null) device.Configuration = Configuration.ToConfiguration();

            // References
            if (!References.IsNullOrEmpty())
            {
                var references = new List<IReference>();
                foreach (var reference in References)
                {
                    references.Add(reference.ToReference());
                }
                device.References = references;
            }

            // DataItems
            if (!DataItems.IsNullOrEmpty())
            {
                var dataItems = new List<IDataItem>();
                foreach (var xmlDataItem in DataItems)
                {
                    var dataItem = xmlDataItem.ToDataItem();
                    dataItem.Container = device;
                    dataItem.Device = device;
                    dataItems.Add(dataItem);
                }
                device.DataItems = dataItems;
            }

            // Compositions
            if (!Compositions.IsNullOrEmpty())
            {
                var compositions = new List<IComposition>();
                foreach (var xmlComposition in Compositions)
                {
                    var composition = xmlComposition.ToComposition(device);
                    composition.Parent = device;
                    compositions.Add(composition);
                }
                device.Compositions = compositions;
            }

            // Components
            if (ComponentCollection != null && !ComponentCollection.Components.IsNullOrEmpty())
            {
                var components = new List<IComponent>();
                foreach (var xmlComponent in ComponentCollection.Components)
                {
                    var component = xmlComponent.ToComponent(device);
                    component.Parent = device;
                    components.Add(component);
                }
                device.Components = components;
            }

            return device;
        }

        public static IDevice FromXml(byte[] xmlBytes, string deviceUuid = null)
        {
            if (xmlBytes != null && xmlBytes.Length > 0)
            {
                try
                {
                    // Clean whitespace and Encoding Marks (BOM)
                    var bytes = XmlFunctions.SanitizeBytes(xmlBytes);

                    using (var memoryReader = new MemoryStream(bytes))
                    {
                        var readerSettings = new XmlReaderSettings();
                        readerSettings.ConformanceLevel = ConformanceLevel.Fragment;
                        readerSettings.IgnoreComments = true;
                        readerSettings.IgnoreProcessingInstructions = true;
                        readerSettings.IgnoreWhitespace = true;

                        using (var xmlReader = XmlReader.Create(memoryReader, readerSettings))
                        {
                            var xmlObj = (XmlDevice)_serializer.Deserialize(xmlReader);
                            if (xmlObj != null)
                            {
                                if (!string.IsNullOrEmpty(deviceUuid)) xmlObj.Uuid = deviceUuid;

                                return xmlObj.ToDevice();
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static byte[] ToXml(IDevice device, bool indent = true)
        {
            if (device != null)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        // Set the XmlWriterSettings to use
                        var xmlWriterSettings = indent ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                        // Use XmlWriter to write XML to stream
                        using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                        {
                            WriteXml(xmlWriter, device);
                            xmlWriter.Flush();
                            return stream.ToArray();
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static void WriteXml(XmlWriter writer, IDevice device, bool outputComments = false)
        {
            if (device != null)
            {
                // Add Comments
                if (outputComments && device != null)
                {
                    // Write Device Type Description as Comment
                    if (!string.IsNullOrEmpty(device.TypeDescription))
                    {
                        writer.WriteComment($"Type = {device.Type} : {device.TypeDescription}");
                    }
                }


                writer.WriteStartElement(device.Type);

                // Write Properties
                writer.WriteAttributeString("id", device.Id);
                writer.WriteAttributeString("name", device.Name);
                writer.WriteAttributeString("uuid", device.Uuid);
                if (!string.IsNullOrEmpty(device.Iso841Class)) writer.WriteAttributeString("iso841Class", device.Iso841Class);
                if (!string.IsNullOrEmpty(device.NativeName)) writer.WriteAttributeString("nativeName", device.NativeName);
                if (device.SampleInterval > 0) writer.WriteAttributeString("sampleInterval", device.SampleInterval.ToString());
                if (device.SampleRate > 0) writer.WriteAttributeString("sampleRate", device.SampleRate.ToString());
                if (!string.IsNullOrEmpty(device.CoordinateSystemIdRef)) writer.WriteAttributeString("coordinateSystemIdRef", device.CoordinateSystemIdRef);
                if (device.MTConnectVersion != null) writer.WriteAttributeString("mtconnectVersion", device.MTConnectVersion.ToString());
                if (!string.IsNullOrEmpty(device.Hash)) writer.WriteAttributeString("hash", device.Hash);

				// Write Description
				XmlDescription.WriteXml(writer, device.Description);

                // Write Configuration
                XmlConfiguration.WriteXml(writer, device.Configuration, outputComments);

                // Write References
                if (!device.References.IsNullOrEmpty())
                {
                    writer.WriteStartElement("References");
                    foreach (var reference in device.References)
                    {
                        XmlReference.WriteXml(writer, reference);
                    }
                    writer.WriteEndElement();
                }

                // Write DataItems
                if (!device.DataItems.IsNullOrEmpty())
                {
                    writer.WriteStartElement("DataItems");
                    foreach (var dataItem in device.DataItems)
                    {
                        XmlDataItem.WriteXml(writer, dataItem, outputComments);
                    }
                    writer.WriteEndElement();
                }

                // Write Compositions
                if (!device.Compositions.IsNullOrEmpty())
                {
                    writer.WriteStartElement("Compositions");
                    foreach (var composition in device.Compositions)
                    {
                        XmlComposition.WriteXml(writer, composition, outputComments);
                    }
                    writer.WriteEndElement();
                }

                // Write Components
                if (!device.Components.IsNullOrEmpty())
                {
                    writer.WriteStartElement("Components");
                    foreach (var component in device.Components)
                    {
                        XmlComponent.WriteXml(writer, component, outputComments);
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}