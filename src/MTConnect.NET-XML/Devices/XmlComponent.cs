// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.References;
using MTConnect.Writers;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// An abstract XML Element.
    /// Replaced in the XML document by types of Component elements representing physical and logical parts of the Device.
    /// There can be multiple types of Component XML Elements in the document.
    /// </summary>
    [XmlRoot("Component")]
    public class XmlComponent
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlComponent));


        /// <summary>
        /// The unique identifier for this Component in the document.
        /// An id MUST be unique across all the id attributes in the document.
        /// An XML ID-type.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The type of component
        /// </summary>
        [XmlIgnore]
        public string Type { get; set; }

        /// <summary>
        /// The name of the Component.
        /// Name is an optional attribute.
        /// If provided, Name MUST be unique within a type of Component or subComponent.
        /// It is recommended that duplicate names SHOULD NOT occur within a Device.
        /// An NMTOKEN XML type.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The name the device manufacturer assigned to the Component.
        /// If the native name is not provided it MUST be the Name.
        /// </summary>
        [XmlAttribute("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a component until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures. 
        /// If the sample interval is smaller than one millisecond, the number can be represented as a floating point number.
        /// For example, an interval of 100 microseconds would be 0.1.
        /// </summary>
        [XmlAttribute("sampleInterval")]
        public double SampleInterval { get; set; }

        [XmlIgnore]
        public bool SampleIntervalSpecified => SampleInterval > 0;

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY sampleInterval)
        /// </summary>
        [XmlAttribute("sampleRate")]
        public double SampleRate { get; set; }

        [XmlIgnore]
        public bool SampleRateSpecified => SampleRate > 0;

        /// <summary>
        /// A unique identifier that will only refer to this Component.
        /// For example, this can be the manufacturer's code or the serial number.
        /// The uuid should be alphanumeric and not exceeding 255 characters.
        /// An NMTOKEN XML type.
        /// </summary>
        [XmlAttribute("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

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

        [XmlIgnore]
        public string TypeDescription { get; set; }



        public XmlComponent() 
        {
            DataItemCollection = new XmlDataItemCollection();
            Compositions = new List<XmlComposition>();
        }

        public XmlComponent(IComponent component)
        {
            DataItemCollection = new XmlDataItemCollection();
            Compositions = new List<XmlComposition>();

            if (component != null)
            {
                Id = component.Id;
                Uuid = component.Uuid;
                Name = component.Name;
                NativeName = component.NativeName;
                Type = component.Type;
                SampleRate = component.SampleRate;
                SampleInterval = component.SampleInterval;
                TypeDescription = component.TypeDescription;

                if (component.Description != null) Description = new XmlDescription(component.Description);
                if (component.Configuration != null) Configuration = new XmlConfiguration(component.Configuration);

                // References
                if (!component.References.IsNullOrEmpty())
                {
                    var references = new List<XmlReference>();
                    foreach (var reference in component.References)
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
                if (!component.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in component.DataItems)
                    {
                        DataItemCollection.DataItems.Add(DataItem.Create(dataItem));
                    }
                }

                // Compositions
                if (!component.Compositions.IsNullOrEmpty())
                {
                    foreach (var composition in component.Compositions)
                    {
                        Compositions.Add(new XmlComposition(composition));
                    }
                }

                // Components
                if (!component.Components.IsNullOrEmpty())
                {
                    var componentCollection = new XmlComponentCollection();
                    foreach (var subcomponent in component.Components)
                    {
                        componentCollection.Components.Add(new XmlComponent(subcomponent));
                    }
                    ComponentCollection = componentCollection;
                }
            }
        }


        public override string ToString() => ToXml(ToComponent(), true);

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
                var references = new List<Reference>();
                foreach (var reference in References)
                {
                    references.Add(reference.ToReference());
                }
                component.References = references;
            }

            // DataItems
            if (DataItemCollection != null && !DataItemCollection.DataItems.IsNullOrEmpty())
            {
                var dataItems = new List<IDataItem>();
                foreach (var dataItem in DataItemCollection.DataItems)
                {
                    dataItem.Container = component;
                    dataItem.Device = device;
                    dataItems.Add(dataItem);
                }
                component.DataItems = dataItems;
            }

            // Compositions
            if (!Compositions.IsNullOrEmpty())
            {
                var compositions = new List<Composition>();
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
                var components = new List<Component>();
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


        public static IComponent FromXml(string xml)
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
                            var xmlObj = (XmlComponent)_serializer.Deserialize(xmlReader);
                            if (xmlObj != null)
                            {
                                return xmlObj.ToComponent();
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static string ToXml(IComponent component, bool indent = false)
        {
            if (component != null)
            {
                try
                {
                    using (var writer = new Utf8Writer())
                    {
                        _serializer.Serialize(writer, new XmlComponent(component));
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
