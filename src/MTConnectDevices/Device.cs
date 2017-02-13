// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.MTConnectDevices
{
    /// <summary>
    /// The primary container element of each device. 
    /// Device is contained within the top level Devices container. 
    /// There MAY be multiple Device elements in an XML document.
    /// </summary>
    [XmlRoot("Device")]
    public class Device
    {

        #region "Required"

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

        #endregion

        #region "Optional"

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

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)
        /// </summary>
        [XmlAttribute("sampleRate")]
        public double SampleRate { get; set; }

        #endregion

        #region "Sub-Elements"

        /// <summary>
        /// An XML element that can contain any descriptive content.
        /// This can contain configuration information and manufacturer specific details.
        /// </summary>
        [XmlElement("Description")]
        public Description Description { get; set; }

        /// <summary>
        /// A container for SubComponent XML Elements.
        /// </summary>
        [XmlElement("Components")]
        public ComponentCollection Components { get; set; }

        /// <summary>
        /// A container for the Data XML Elements provided by this Device.
        /// The data items define the measured values to be reported by this Device.
        /// </summary>
        [XmlArray("DataItems")]
        public List<DataItem> DataItems { get; set; }

        #endregion

        /// <summary>
        /// Return a list of All DataItems
        /// </summary>
        public List<DataItem> GetDataItems()
        {
            var l = new List<DataItem>();

            // Add Root DataItems
            if (DataItems != null) l.AddRange(DataItems);

            foreach (var component in Components.Components) l.AddRange(GetDataItems(component));

            return l;
        }

        private List<DataItem> GetDataItems(Component component)
        {
            var l = new List<DataItem>();

            // Add Root DataItems
            if (component.DataItems != null) l.AddRange(component.DataItems);

            // Get SubComponent DataItems
            foreach (var subComponent in component.SubComponents.Components) l.AddRange(GetDataItems(subComponent));

            return l;
        }

        /// <summary>
        /// Return a list of All Components
        /// </summary>
        public List<Component> GetComponents()
        {
            var l = new List<Component>();

            foreach (var subComponent in Components.Components)
            {
                l.AddRange(GetComponents(subComponent));
            }

            return l;
        }

        private List<Component> GetComponents(Component component)
        {
            var l = new List<Component>();

            l.Add(component);

            foreach (var subComponent in component.SubComponents.Components) l.AddRange(GetComponents(subComponent));

            return l;
        }

        private const string XPATH_DATAITEM = "DataItem[@id=\"{0}\"]";
        private const string XPATH_COMPONENT = "{0}[@id=\"{1}\"]";

        public void AssignXPaths()
        {
            // Set Root DataItems
            foreach (var dataItem in DataItems)
            {
                dataItem.XPath = "//" + string.Format(XPATH_DATAITEM, dataItem.Id);
            }

            // Set Root Components
            foreach (var component in Components.Components)
            {
                var xpath = string.Format(XPATH_COMPONENT, component.Type, component.Id);
                AssignXPaths("//" + xpath, component);
            }
        }

        private static void AssignXPaths(string parentPath, Component component)
        {
            // Set Component XPath
            component.XPath = parentPath;

            // Set Root DataItems
            foreach (var dataItem in component.DataItems)
            {
                dataItem.XPath = parentPath + "/DataItems/" + string.Format(XPATH_DATAITEM, dataItem.Id);
            }

            // Set Root Components
            foreach (var subcomponent in component.SubComponents.Components)
            {
                var xpath = string.Format(XPATH_COMPONENT, subcomponent.Type, subcomponent.Id);
                AssignXPaths(parentPath + "/Components/" + xpath, subcomponent);
            }
        }


        public void AssignTypePaths()
        {
            // Set Root DataItems
            foreach (var dataItem in DataItems)
            {
                dataItem.TypePath = dataItem.Type;
            }

            // Set Root Components
            foreach (var component in Components.Components)
            {
                AssignTypePaths(component.Type, component);
            }
        }

        private static void AssignTypePaths(string parentPath, Component component)
        {
            component.TypePath = parentPath;

            // Set Root DataItems
            foreach (var dataItem in component.DataItems)
            {
                dataItem.TypePath = parentPath + "/" + dataItem.Type;
            }

            // Set Root Components
            foreach (var subcomponent in component.SubComponents.Components)
            {
                AssignTypePaths(parentPath + "/" + subcomponent.Type, subcomponent);
            }
        }
    }
}
