// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// An abstract XML Element.
    /// Replaced in the XML document by types of Component elements representing physical and logical parts of the Device.
    /// There can be multiple types of Component XML Elements in the document.
    /// </summary>
    [XmlRoot("Component")]
    public class XmlComponent
    {
        /// <summary>
        /// The XPath address of the Component
        /// </summary>
        [XmlIgnore]
        public string XPath { get; set; }

        /// <summary>
        /// The path of the Component by Type
        /// </summary>
        [XmlIgnore]
        public string TypePath { get; set; }


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
        public Description Description { get; set; }

        /// <summary>
        /// An XML element that contains technical information about a piece of equipment describing its physical layout or functional characteristics.
        /// </summary>
        [XmlElement("Configuration")]
        public XmlConfiguration Configuration { get; set; }

        /// <summary>
        /// A container for the Data Entities associated with this Component element.
        /// </summary>
        [XmlArray("DataItems")]
        [XmlArrayItem("DataItem")]
        public List<XmlDataItem> DataItems { get; set; }

        [XmlIgnore]
        public bool DataItemsSpecified => !DataItems.IsNullOrEmpty();

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


        public XmlComponent() 
        {
            DataItems = new List<XmlDataItem>();
            Compositions = new List<XmlComposition>();
        }

        public XmlComponent(Component component)
        {
            DataItems = new List<XmlDataItem>();
            Compositions = new List<XmlComposition>();

            if (component != null)
            {
                Id = component.Id;
                Uuid = component.Uuid;
                Name = component.Name;
                NativeName = component.NativeName;
                Type = component.Type;
                Description = component.Description;
                SampleRate = component.SampleRate;
                SampleInterval = component.SampleInterval;
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
                        DataItems.Add(new XmlDataItem(dataItem));
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

        public Component ToComponent()
        {
            var component = Component.Create(Type);
            if (component == null) component = new Component();

            component.Id = Id;
            component.Uuid = Uuid;
            component.Name = Name;
            component.NativeName = NativeName;
            component.Type = Type;
            component.Description = Description;
            component.SampleRate = SampleRate;
            component.SampleInterval = SampleInterval;
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
            if (!DataItems.IsNullOrEmpty())
            {
                component.DataItems = new List<DataItem>();
                foreach (var dataItem in DataItems)
                {
                    component.DataItems.Add(dataItem.ToDataItem());
                }
            }

            // Compositions
            if (!Compositions.IsNullOrEmpty())
            {
                component.Compositions = new List<Composition>();
                foreach (var composition in Compositions)
                {
                    component.Compositions.Add(composition.ToComposition());
                }
            }

            // Components
            if (ComponentCollection != null && !ComponentCollection.Components.IsNullOrEmpty())
            {
                var components = new List<Component>();
                foreach (var subcomponent in ComponentCollection.Components)
                {
                    components.Add(subcomponent.ToComponent());
                }
                component.Components = components;
            }

            return component;
        }
    }
}
