// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml;

namespace MTConnect.Application.Streams
{
    /// <summary>
    /// An XML container type element that may be provided in a MTConnectStreams XML document returned from a MTConnect Agent in response to a Current or Sample request that organizes data for a specific Structural Element of a device.
    /// </summary>
    public class ComponentStream : IDisposable
    {
        public ComponentStream()
        {
            Init();
        }

        public ComponentStream(XmlNode node)
        {
            Init();
            Process(node);
        }

        private void Init()
        {
            DataItems = new List<DataItem>();
        }

        #region "Required"

        /// <summary>
        /// Id attribute of a device's Structural Element (Device, Component type or Subcomponent type) for which data is provided.
        /// </summary>
        public string ComponentId { get; set; }

        /// <summary>
        /// The type of Structural Element (Device, Component type or Subcomponent type) for which data is provided.
        /// </summary>
        public string Component { get; set; }

        #endregion

        #region "Optional"

        /// <summary>
        /// Name attribute of the Structural Element (Device, Component type or Subcomponent type) for which data is provided.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// NativeName attribute of the Structural Element (Device, Component type or Subcomponent type) for which data is provided.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// Uuid attribute (unique identifier) of the Structural Element (Device, Component type or Subcomponent type) for which data is provided.
        /// </summary>
        public string Uuid { get; set; }

        #endregion


        public List<DataItem> DataItems { get; set; }

        public string FullAddress { get; set; }



        private void Process(XmlNode node)
        {
            Tools.XML.AssignProperties(this, node);
            FullAddress = Tools.Address.GetStreams(node);

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    switch (child.Name.ToLower())
                    {
                        case "condition":

                            DataItems.AddRange(ProcessConditions(child));

                            break;

                        case "events":

                            DataItems.AddRange(ProcessEvents(child));

                            break;

                        case "samples":

                            DataItems.AddRange(ProcessSamples(child));

                            break;
                    }
                }
            }
        }

        private List<Condition> ProcessConditions(XmlNode node)
        {
            var result = new List<Condition>();

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    result.Add(new Condition(child));
                }
            }

            return result;
        }

        private List<Event> ProcessEvents(XmlNode node)
        {
            var result = new List<Event>();

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    result.Add(new Event(child));
                }
            }

            return result;
        }

        private List<Sample> ProcessSamples(XmlNode node)
        {
            var result = new List<Sample>();

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    result.Add(new Sample(child));
                }
            }

            return result;
        }


        public void Dispose()
        {
            DataItems.Clear();
            DataItems = null;
        }
    }
}
