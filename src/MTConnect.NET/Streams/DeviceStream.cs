// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Streams
{
    /// <summary>
    /// DeviceStream is a XML container that organizes data reported from a single piece of equipment.A DeviceStream element MUST be provided for each piece of equipment reporting data in an MTConnectStreams document.
    /// </summary>
    [XmlRoot("DeviceStream")]
    public class DeviceStream
    {

        #region "Required"

        /// <summary>
        /// The name of an element or a piece of equipment. The name associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The uuid associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        [XmlAttribute("uuid")]
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        #endregion

        #region "Sub-Elements"

        /// <summary>
        /// An XML container type element that organizes data returned from an Agent in response to a current or sample HTTP request.
        /// </summary>
        [XmlElement("ComponentStream", IsNullable = false)]
        [JsonPropertyName("componentStreams")]
        public List<ComponentStream> ComponentStreams { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<IDataItem> DataItems
        {
            get
            {
                var l = new List<IDataItem>();

                if (ComponentStreams != null)
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        l.AddRange(componentStream.DataItems);
                    }
                }

                return l;
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public List<Condition> Conditions
        {
            get
            {
                var l = new List<Condition>();

                if (ComponentStreams != null)
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.Conditions.IsNullOrEmpty())
                        {
                            foreach (var dataItem in componentStream.Conditions) l.Add(dataItem);
                        }
                    }
                }

                return l;
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public List<Event> Events
        {
            get
            {
                var l = new List<Event>();

                if (ComponentStreams != null)
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.Events.IsNullOrEmpty())
                        {
                            foreach (var dataItem in componentStream.Events) l.Add(dataItem);
                        }
                    }
                }

                return l;
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public List<Sample> Samples
        {
            get
            {
                var l = new List<Sample>();

                if (ComponentStreams != null)
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.Samples.IsNullOrEmpty())
                        {
                            foreach (var dataItem in componentStream.Samples) l.Add(dataItem);
                        }
                    }
                }

                return l;
            }
        }

        #endregion

    }
}
