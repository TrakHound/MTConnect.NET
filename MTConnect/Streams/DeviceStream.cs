// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml;

namespace MTConnect.Streams
{
    /// <summary>
    /// An XML container element provided in each MTConnectStreams XML document provided by a MTConnect Agent in response to a Sample or Current request.
    /// </summary>
    public class DeviceStream : IDisposable
    {
        public DeviceStream()
        {
            Init();
        }

        public DeviceStream(XmlNode node)
        {
            Init();
            Process(node);
        }

        private void Init()
        {
            ComponentStreams = new List<ComponentStream>();
            DataItems = new List<DataItem>();
        }

        #region "Required"

        /// <summary>
        /// Name attribute of the Devices defined in the Device Information Model for which data is provided.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Uuid attribute of the Device defined in the Device Information Model for which data is provided
        /// </summary>
        public string Uuid { get; set; }

        #endregion

        #region "Sub-Elements"

        /// <summary>
        /// An XML container type element that may be provided in a MTConnectStreams XML document returned from a MTConnect Agent in response to a Current or Sample request that organizes data for a specific Structural Element of a device.
        /// </summary>
        public List<ComponentStream> ComponentStreams { get; set; }

        public List<DataItem> DataItems { get; set; }

        #endregion


        public List<DataItem> GetAllDataItems()
        {
            var result = new List<DataItem>();

            // Add DataItems in DeviceStream root
            result.AddRange(DataItems);

            // Loop through each Component and Add DataItems
            foreach (var componentStream in ComponentStreams) result.AddRange(componentStream.DataItems);

            return result;
        }


        private void Process(XmlNode node)
        {
            Tools.XML.AssignProperties(this, node);

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    switch (child.Name.ToLower())
                    {
                        case "componentstream":

                            ComponentStreams.Add(new ComponentStream(child));

                            break;

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
            ComponentStreams.Clear();
            ComponentStreams = null;

            DataItems.Clear();
            DataItems = null;
        }
    }
}
