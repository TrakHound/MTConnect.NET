// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

using System.Xml;


namespace MTConnect.MTConnectDevices
{
    /// <summary>
    /// The primary container element of each device. 
    /// Device is contained within the top level Devices container. 
    /// There MAY be multiple Device elements in an XML document.
    /// </summary>
    public class Device : IDisposable
    {
        public Device()
        {
            Init();
        }

        public Device(XmlNode node)
        {
            Init();
            Process(node);
        }

        private void Init()
        {
            Components = new List<Component>();
            DataItems = new List<DataItem>();
        }

        #region "Required"

        /// <summary>
        /// The unique identifier for this Device in the document.
        /// An id MUST be unique across all the id attributes in the document.
        /// An XML ID-type.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the Device.
        /// THis name should be unique within the XML document to allow for easier data integration.
        /// An NMTOKEN XML type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique identifier that will only refer ot this Device.
        /// For example, this may be the manufacturer's code and the serial number.
        /// The uuid shoudl be alphanumeric and not exceeding 255 characters.
        /// An NMTOKEN XML type.
        /// </summary>
        public string Uuid { get; set; }

        #endregion

        #region "Optional"

        /// <summary>
        /// DEPRECATED IN REL. 1.1
        /// </summary>
        public string Iso841Class { get; set; }

        /// <summary>
        /// The name the device manufacturer assigned to this Device.
        /// If the native name is not provided, it MUST be the name.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a device until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures.
        /// If the sample interval is smaller than one millisecond, the number can be represented as a floating point number.
        /// For example, an interval of 100 microseconds would be 0.1.
        /// </summary>
        public string SampleInterval { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)
        /// </summary>
        public string SampleRate { get; set; }

        #endregion

        #region "Sub-Elements"

        /// <summary>
        /// An XML element that can contain any descriptive content.
        /// This can contain configuration information and manufacturer specific details.
        /// </summary>
        public Description Description { get; set; }

        /// <summary>
        /// A container for Component XML Elements associated with this Device.
        /// </summary>
        public List<Component> Components { get; set; }

        /// <summary>
        /// A container for the Data XML Elements provided by this Device.
        /// The data items define the measured values to be reported by this Device.
        /// </summary>
        public List<DataItem> DataItems { get; set; }

        #endregion

        public List<DataItem> GetAllDataItems()
        {
            var result = new List<DataItem>();

            // Add DataItems in device root
            result.AddRange(DataItems);

            // Loop through each Component and Add DataItems
            foreach (var component in Components) result.AddRange(component.GetAllDataItems());

            return result;
        }


        private void Process(XmlNode node)
        {
            MTConnect.Tools.XML.AssignProperties(this, node);

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    switch (child.Name.ToLower())
                    {
                        case "components":

                            Components.AddRange(ProcessComponents(child));

                            break;

                        case "dataitems":

                            DataItems = ProcessDataItems(child);

                            break;

                        case "description":

                            Description = new Description(child);

                            break;
                    }
                }
            }
        }

        private List<Component> ProcessComponents(XmlNode node)
        {
            var result = new List<Component>();

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    result.Add(new Component(child));
                }
            }

            return result;
        }

        private List<DataItem> ProcessDataItems(XmlNode node)
        {
            var result = new List<DataItem>();

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    result.Add(new DataItem(child));
                }
            }

            return result;
        }


        public void Dispose()
        {
            Components.Clear();
            Components = null;

            DataItems.Clear();
            DataItems = null;
        }
    }
}
