// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// The primary container element of each device. 
    /// Device is contained within the top level Devices container. 
    /// There MAY be multiple Device elements in an XML document.
    /// </summary>
    [XmlRoot("Device")]
    public class XmlDevice
    {
        private const string XPATH_DATAITEM = "DataItem[@id=\"{0}\"]";
        private const string XPATH_COMPONENT = "{0}[@id=\"{1}\"]";


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
        public Description Description { get; set; }

        /// <summary>
        /// An XML element that contains technical information about a piece of equipment describing its physical layout or functional characteristics.
        /// </summary>
        [XmlElement("Configuration")]
        public Configuration Configuration { get; set; }

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
        [XmlArrayItem("ComponentRef", typeof(ComponentReference))]
        [XmlArrayItem("DataItemRef", typeof(DataItemReference))]
        public List<Reference> References { get; set; }


        public XmlDevice() 
        {
            DataItems = new List<XmlDataItem>();
            Compositions = new List<XmlComposition>();
        }

        public XmlDevice(Device device)
        {
            DataItems = new List<XmlDataItem>();
            Compositions = new List<XmlComposition>();

            if (device != null)
            {
                Id = device.Id;
                Name = device.Name;
                NativeName = device.NativeName;
                Uuid = device.Uuid;
                Description = device.Description;
                SampleRate = device.SampleRate;
                SampleInterval = device.SampleInterval;
                Iso841Class = device.Iso841Class;
                CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                if (device.MTConnectVersion != null) MTConnectVersion = device.MTConnectVersion.ToString();
                Configuration = device.Configuration;
                References = device.References;
                Description = device.Description;


                // DataItems
                if (!device.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in device.DataItems)
                    {
                        DataItems.Add(new XmlDataItem(dataItem));
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
                    ComponentCollection = new XmlComponentCollection { Components = device.Components };
                }
            }
        }

        public virtual Device ToDevice()
        {
            var device = new Device();

            device.Id = Id;
            device.Name = Name;
            device.NativeName = NativeName;
            device.Uuid = Uuid;
            device.Description = Description;
            device.SampleRate = SampleRate;
            device.SampleInterval = SampleInterval;
            device.Iso841Class = Iso841Class;
            device.CoordinateSystemIdRef = CoordinateSystemIdRef;
            if (Version.TryParse(MTConnectVersion, out var mtconnectVersion))
            {
                device.MTConnectVersion = mtconnectVersion;
            }
            device.Configuration = Configuration;
            device.References = References;
            device.Description = Description;

            // DataItems
            if (!DataItems.IsNullOrEmpty())
            {
                device.DataItems = new List<DataItem>();
                foreach (var dataItem in DataItems)
                {
                    device.DataItems.Add(dataItem.ToDataItem());
                }
            }

            // Compositions
            if (!Compositions.IsNullOrEmpty())
            {
                device.Compositions = new List<Composition>();
                foreach (var composition in Compositions)
                {
                    device.Compositions.Add(composition.ToComposition());
                }
            }

            // Components
            if (ComponentCollection != null && !ComponentCollection.Components.IsNullOrEmpty())
            {
                device.Components = ComponentCollection.Components;
            }

            return device;
        }


        //#region "XPath"

        //public void AssignXPaths()
        //{
        //    // Set Root DataItems
        //    foreach (var dataItem in DataItems)
        //    {
        //        //dataItem.XPath = "//" + string.Format(XPATH_DATAITEM, dataItem.Id);
        //    }

        //    // Set Root Components
        //    foreach (var component in Components)
        //    {
        //        var xpath = string.Format(XPATH_COMPONENT, component.Type, component.Id);
        //        AssignXPaths("//" + xpath, component);
        //    }

        //    //// Set Root Components
        //    //foreach (var component in Components.Components)
        //    //{
        //    //    var xpath = string.Format(XPATH_COMPONENT, component.Type, component.Id);
        //    //    AssignXPaths("//" + xpath, component);
        //    //}
        //}

        //private static void AssignXPaths(string parentPath, Component component)
        //{
        //    // Set Component XPath
        //    //component.XPath = parentPath;

        //    // Set Root DataItems
        //    foreach (var dataItem in component.DataItems)
        //    {
        //        //dataItem.XPath = parentPath + "/DataItems/" + string.Format(XPATH_DATAITEM, dataItem.Id);
        //    }

        //    // Set Root Components
        //    foreach (var subcomponent in component.Components)
        //    {
        //        var xpath = string.Format(XPATH_COMPONENT, subcomponent.Type, subcomponent.Id);
        //        AssignXPaths(parentPath + "/Components/" + xpath, subcomponent);
        //    }

        //    //// Set Root Components
        //    //foreach (var subcomponent in component.SubComponents.Components)
        //    //{
        //    //    var xpath = string.Format(XPATH_COMPONENT, subcomponent.Type, subcomponent.Id);
        //    //    AssignXPaths(parentPath + "/Components/" + xpath, subcomponent);
        //    //}
        //}

        //#endregion
    }
}
