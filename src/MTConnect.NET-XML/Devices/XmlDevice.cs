// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.References;
using MTConnect.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// The primary container element of each device. 
    /// Device is contained within the top level Devices container. 
    /// There MAY be multiple Device elements in an XML document.
    /// </summary>
    [XmlRoot("Device")]
    public class XmlDevice
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlDevice));


        /// <summary>
        /// The unique identifier for this Device in the document.
        /// An id MUST be unique across all the id attributes in the document.
        /// An XML ID-type.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the Device.
        /// THis name should be unique within the XML document to allow for easier data integration.
        /// An NMTOKEN XML type.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// A unique identifier that will only refer ot this Device.
        /// For example, this may be the manufacturer's code and the serial number.
        /// The uuid shoudl be alphanumeric and not exceeding 255 characters.
        /// An NMTOKEN XML type.
        /// </summary>
        [XmlAttribute("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.1
        /// </summary>
        [XmlAttribute("iso841Class")]
        public string Iso841Class { get; set; }

        /// <summary>
        /// The name the device manufacturer assigned to this Device.
        /// If the native name is not provided, it MUST be the name.
        /// </summary>
        [XmlAttribute("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a device until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures.
        /// If the sample interval is smaller than one millisecond, the number can be represented as a floating point number.
        /// For example, an interval of 100 microseconds would be 0.1.
        /// </summary>
        [XmlAttribute("sampleInterval")]
        public double SampleInterval { get; set; }

        [XmlIgnore]
        public bool SampleIntervalSpecified => SampleInterval > 0;

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)
        /// </summary>
        [XmlAttribute("sampleRate")]
        public double SampleRate { get; set; }

        [XmlIgnore]
        public bool SampleRateSpecified => SampleRate > 0;

        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The MTConnect version of the Devices Information Model used to configure
        /// the information to be published for a piece of equipment in an MTConnect Response Document.
        /// </summary>
        [XmlAttribute("mtconnectVersion")]
        public string MTConnectVersion { get; set; }

        /// <summary>
        /// An element that can contain any descriptive content. 
        /// This can contain information about the Component and manufacturer specific details.
        /// </summary>
        [XmlElement("Description")]
        public XmlDescription Description { get; set; }

        /// <summary>
        /// An XML element that contains technical information about a piece of equipment describing its physical layout or functional characteristics.
        /// </summary>
        [XmlElement("Configuration")]
        public XmlConfiguration Configuration { get; set; }

        /// <summary>
        /// A container for the Data Entities associated with this Component element.
        /// </summary>
        [XmlElement("DataItems")]
        public XmlDataItemCollection DataItemCollection { get; set; }

        [XmlIgnore]
        public bool DataItemCollectionSpecified => DataItemCollection != null && !DataItemCollection.DataItems.IsNullOrEmpty();

        /// <summary>
        /// A container for Lower Level Component XML elements associated with this parent Component.
        /// </summary>
        [XmlElement("Components")]
        public XmlComponentCollection ComponentCollection { get; set; }

        [XmlIgnore]
        public bool ComponentCollectionSpecified => ComponentCollection != null && !ComponentCollection.Components.IsNullOrEmpty();

        /// <summary>
        /// A container for the Composition elements associated with this Component element.
        /// </summary>
        [XmlArray("Compositions")]
        [XmlArrayItem("Composition")]
        public List<XmlComposition> Compositions { get; set; }

        [XmlIgnore]
        public bool CompositionsSpecified => !Compositions.IsNullOrEmpty();

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        [XmlArray("References")]
        [XmlArrayItem("ComponentRef", typeof(XmlComponentReference))]
        [XmlArrayItem("DataItemRef", typeof(XmlDataItemReference))]
        public List<XmlReference> References { get; set; }

        [XmlIgnore]
        public bool ReferencesSpecified => !References.IsNullOrEmpty();


        public XmlDevice() 
        {
            DataItemCollection = new XmlDataItemCollection();
            Compositions = new List<XmlComposition>();
        }

        public XmlDevice(IDevice device)
        {
            DataItemCollection = new XmlDataItemCollection();
            Compositions = new List<XmlComposition>();

            if (device != null)
            {
                Id = device.Id;
                Name = device.Name;
                NativeName = device.NativeName;
                Uuid = device.Uuid;
                SampleRate = device.SampleRate;
                SampleInterval = device.SampleInterval;
                Iso841Class = device.Iso841Class;
                CoordinateSystemIdRef = device.CoordinateSystemIdRef;

                if (device.MTConnectVersion != null) MTConnectVersion = device.MTConnectVersion.ToString();
                if (device.Description != null) Description = new XmlDescription(device.Description);
                if (device.Configuration != null) Configuration = new XmlConfiguration(device.Configuration);

                // References
                if (!device.References.IsNullOrEmpty())
                {
                    var references = new List<XmlReference>();
                    foreach (var reference in device.References)
                    {
                        if (reference.GetType() == typeof(ComponentReference))
                        {
                            references.Add(new XmlComponentReference((ComponentReference)reference));
                        }

                        if (reference.GetType() == typeof(DataItemReference))
                        {
                            references.Add(new XmlDataItemReference((DataItemReference)reference));
                        }
                    }
                    References = references;
                }


                // DataItems
                if (!device.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in device.DataItems)
                    {
                        DataItemCollection.DataItems.Add(new DataItem(dataItem));
                    }
                }

                // Compositions
                if (!device.Compositions.IsNullOrEmpty())
                {
                    foreach (var composition in device.Compositions)
                    {
                        Compositions.Add(new XmlComposition(composition));
                    }
                }

                // Components
                if (!device.Components.IsNullOrEmpty())
                {
                    var componentCollection = new XmlComponentCollection();
                    foreach (var component in device.Components)
                    {
                        componentCollection.Components.Add(new XmlComponent(component));
                    }
                    ComponentCollection = componentCollection;
                }
            }
        }


        public override string ToString() => ToXml(ToDevice(), true);


        public virtual Device ToDevice()
        {
            var device = new Device();

            device.Id = Id;
            device.Name = Name;
            device.NativeName = NativeName;
            device.Uuid = Uuid;
            device.SampleRate = SampleRate;
            device.SampleInterval = SampleInterval;
            device.Iso841Class = Iso841Class;
            device.CoordinateSystemIdRef = CoordinateSystemIdRef;
            if (Version.TryParse(MTConnectVersion, out var mtconnectVersion))
            {
                device.MTConnectVersion = mtconnectVersion;
            }

            if (Description != null) device.Description = Description.ToDescription();
            if (Configuration != null) device.Configuration = Configuration.ToConfiguration();

            // References
            if (!References.IsNullOrEmpty())
            {
                var references = new List<Reference>();
                foreach (var reference in References)
                {
                    references.Add(reference.ToReference());
                }
                device.References = references;
            }

            // DataItems
            if (DataItemCollection != null && !DataItemCollection.DataItems.IsNullOrEmpty())
            {
                var dataItems = new List<IDataItem>();
                foreach (var dataItem in DataItemCollection.DataItems)
                {
                    dataItem.Container = device;
                    dataItem.Device = device;
                    dataItems.Add(dataItem);
                }
                device.DataItems = dataItems;
            }

            // Compositions
            if (!Compositions.IsNullOrEmpty())
            {
                var compositions = new List<Composition>();
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
                var components = new List<Component>();
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


        public static IDevice FromXml(string xml, string deviceUuid = null)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                try
                {
                    xml = xml.Trim();

                    using (var textReader = new StringReader(Namespaces.Clear(xml)))
                    {
                        using (var xmlReader = XmlReader.Create(textReader))
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

        public static string ToXml(IDevice device, bool indent = false)
        {
            if (device != null)
            {
                try
                {
                    using (var writer = new Utf8Writer())
                    {
                        _serializer.Serialize(writer, new XmlDevice(device));
                        var xml = writer.ToString();

                        // Remove the XSD namespace
                        string regex = @"\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""";
                        xml = Regex.Replace(xml, regex, "");
                        regex = @"\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""";
                        xml = Regex.Replace(xml, regex, "");

                        return XmlFunctions.FormatXml(xml, indent, false, true);
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
