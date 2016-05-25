// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml;

namespace MTConnect.Components
{
    /// <summary>
    /// An abstract XML Element.
    /// Replaced in the XML document by types of Component elements representing physical and logical parts of the Device.
    /// There can be multiple types of Component XML Elements in the document.
    /// </summary>
    public class Component : IDisposable
    {
        public Component()
        {
            Init();
        }

        public Component(XmlNode node)
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
        /// The unique identifier for this Component in the document.
        /// An id MUST be unique across all the id attributes in the document.
        /// An XML ID-type.
        /// </summary>
        public string Id { get; set; }

        #endregion

        #region "Optional"

        /// <summary>
        /// The name of the Component.
        /// Name is an optional attribute.
        /// If provided, Name MUST be unique within a type of Component or subComponent.
        /// It is recommended that duplicate names SHOULD NOT occur within a Device.
        /// An NMTOKEN XML type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name the device manufacturer assigned to the Component.
        /// If the native name is not provided it MUST be the Name.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a component until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures. 
        /// If the sample interval is smaller than one millisecond, the number can be represented as a floating point number.
        /// For example, an interval of 100 microseconds would be 0.1.
        /// </summary>
        public string SampleInterval { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY sampleInterval)
        /// </summary>
        public string SampleRate { get; set; }

        /// <summary>
        /// A unique identifier that will only refer to this Component.
        /// For example, this can be the manufacturer's code or the serial number.
        /// The uuid should be alphanumeric and not exceeding 255 characters.
        /// An NMTOKEN XML type.
        /// </summary>
        public string Uuid { get; set; }

        #endregion

        #region "Sub-Elements"

        /// <summary>
        /// An element that can contain any descriptive content. 
        /// This can contain information about the Component and manufacturer specific details.
        /// </summary>
        public Description Description { get; set; }

        /// <summary>
        /// An element that can contain descriptive content defining the configuration information for a Component.
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// A container for lower level Component XML Elements associated with this parent Component.
        /// These lower level elements in this container are defined as Subcomponent elements.
        /// </summary>
        public List<Component> Components;

        /// <summary>
        /// A container for the Data XML Elements provided that are directly related to this Compoenent.
        /// The data items define the measured values to be reported that are related to this Component.
        /// </summary>
        public List<DataItem> DataItems { get; set; }

        #endregion

        /// <summary>
        /// Full XML XPath address of the Component.
        /// (Added for TrakHound)
        /// </summary>
        public string FullAddress { get; set; }

        
        public List<DataItem> GetAllDataItems()
        {
            var result = new List<DataItem>();

            // Add DataItems in root
            result.AddRange(DataItems);

            // Loop through each Subcomponent and Add DataItems
            foreach (var component in Components) result.AddRange(component.GetAllDataItems());

            return result;
        }


        private void Process(XmlNode node)
        {
            Tools.XML.AssignProperties(this, node);
            FullAddress = Tools.Address.GetComponents(node);

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
