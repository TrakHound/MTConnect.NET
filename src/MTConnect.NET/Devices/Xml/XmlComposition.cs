// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Composition XML elements are used to describe the lowest level physical building blocks of a piece of equipment contained within a Component.
    /// </summary>
    [XmlRoot("Composition")]
    public class XmlComposition
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
        [XmlAttribute("type")]
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
        /// An element that can contain any descriptive content. 
        /// This can contain information about the Component and manufacturer specific details.
        /// </summary>
        [XmlElement("Description")]
        public Description Description { get; set; }

        /// <summary>
        /// An element that can contain descriptive content defining the configuration information for a Component.
        /// </summary>
        [XmlElement("Configuration")]
        public Configuration Configuration { get; set; }


        [XmlElement("DataItems")]
        public XmlDataItemCollection DataItemCollection { get; set; }

        [XmlIgnore]
        public bool DataItemCollectionSpecified => DataItemCollection != null && !DataItemCollection.DataItems.IsNullOrEmpty();

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        [XmlArray("References")]
        [XmlArrayItem("ComponentReference", typeof(ComponentReference))]
        [XmlArrayItem("DataItemReference", typeof(DataItemReference))]
        public List<Reference> References { get; set; }

        [XmlIgnore]
        public bool ReferencesSpecified => !References.IsNullOrEmpty();


        public XmlComposition() { }

        public XmlComposition(Composition composition)
        {
            if (composition != null)
            {
                Id = composition.Id;
                Uuid = composition.Uuid;
                Name = composition.Name;
                NativeName = composition.NativeName;
                Type = composition.Type;
                Description = composition.Description;
                SampleRate = composition.SampleRate;
                SampleInterval = composition.SampleInterval;
                References = composition.References;
                Configuration = composition.Configuration;

                // DataItems
                if (!composition.DataItems.IsNullOrEmpty())
                {
                    DataItemCollection = new XmlDataItemCollection { DataItems = composition.DataItems };
                }
            }
        }

        public Composition ToComposition()
        {
            var composition = Composition.Create(Type);
            if (composition == null) composition = new Composition();

            composition.Id = Id;
            composition.Uuid = Uuid;
            composition.Name = Name;
            composition.NativeName = NativeName;
            composition.Type = Type;
            composition.Description = Description;
            composition.SampleRate = SampleRate;
            composition.SampleInterval = SampleInterval;
            composition.References = References;
            composition.Configuration = Configuration;

            // DataItems
            if (DataItemCollection != null && !DataItemCollection.DataItems.IsNullOrEmpty())
            {
                composition.DataItems = DataItemCollection.DataItems;
            }

            return composition;
        }

        //public Composition ToComposition()
        //{
        //    var composition = new Composition();


        //    composition.Id = Id;
        //    composition.Uuid = Uuid;
        //    composition.Name = Name;
        //    composition.NativeName = NativeName;
        //    composition.Type = Type;
        //    composition.Description = Description;
        //    composition.SampleRate = SampleRate;
        //    composition.SampleInterval = SampleInterval;
        //    composition.References = References;
        //    composition.Configuration = Configuration;

        //    // DataItems
        //    if (DataItemCollection != null && !DataItemCollection.DataItems.IsNullOrEmpty())
        //    {
        //        composition.DataItems = DataItemCollection.DataItems;
        //    }

        //    return composition;
        //}
    }
}
