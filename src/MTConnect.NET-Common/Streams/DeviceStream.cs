// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MTConnect.Streams
{
    /// <summary>
    /// DeviceStream is a XML container that organizes data reported from a single piece of equipment.A DeviceStream element MUST be provided for each piece of equipment reporting data in an MTConnectStreams document.
    /// </summary>
    [XmlRoot("DeviceStream")]
    public class DeviceStream : IDeviceStream
    {
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

        /// <summary>
        /// An XML container type element that organizes data returned from an Agent in response to a current or sample HTTP request.
        /// </summary>
        [XmlElement("ComponentStream")]
        [JsonPropertyName("componentStreams")]
        public IEnumerable<IComponentStream> ComponentStreams { get; set; }

        /// <summary>
        /// Gets All Observations (Samples, Events, & Conditions)
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<Observation> Observations
        {
            get
            {
                var l = new List<Observation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.Observations.IsNullOrEmpty())
                        {
                            l.AddRange(componentStream.Observations);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<SampleObservation> Samples
        {
            get
            {
                var l = new List<SampleObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
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

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of VALUE
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<SampleValueObservation> SampleValues
        {
            get
            {
                var l = new List<SampleValueObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.SampleValues.IsNullOrEmpty())
                        {
                            foreach (var dataItem in componentStream.SampleValues) l.Add(dataItem);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TIME_SERIES
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<SampleTimeSeriesObservation> SampleTimeSeries
        {
            get
            {
                var l = new List<SampleTimeSeriesObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.SampleTimeSeries.IsNullOrEmpty())
                        {
                            foreach (var dataItem in componentStream.SampleTimeSeries) l.Add(dataItem);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of DATA_SET
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<SampleDataSetObservation> SampleDataSets
        {
            get
            {
                var l = new List<SampleDataSetObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.SampleDataSets.IsNullOrEmpty())
                        {
                            foreach (var dataItem in componentStream.SampleDataSets) l.Add(dataItem);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TABLE
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<SampleTableObservation> SampleTables
        {
            get
            {
                var l = new List<SampleTableObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.SampleTables.IsNullOrEmpty())
                        {
                            foreach (var dataItem in componentStream.SampleTables) l.Add(dataItem);
                        }
                    }
                }

                return l;
            }
        }


        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<EventObservation> Events
        {
            get
            {
                var l = new List<EventObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
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

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of VALUE
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<EventValueObservation> EventValues
        {
            get
            {
                var l = new List<EventValueObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.EventValues.IsNullOrEmpty())
                        {
                            foreach (var dataItem in componentStream.EventValues) l.Add(dataItem);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of DATA_SET
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<EventDataSetObservation> EventDataSets
        {
            get
            {
                var l = new List<EventDataSetObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.EventDataSets.IsNullOrEmpty())
                        {
                            foreach (var dataItem in componentStream.EventDataSets) l.Add(dataItem);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of TABLE
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<EventTableObservation> EventTables
        {
            get
            {
                var l = new List<EventTableObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.EventTables.IsNullOrEmpty())
                        {
                            foreach (var dataItem in componentStream.EventTables) l.Add(dataItem);
                        }
                    }
                }

                return l;
            }
        }


        /// <summary>
        /// Condition organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of CONDITION in the MTConnectDevices document.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<ConditionObservation> Conditions
        {
            get
            {
                var l = new List<ConditionObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
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
    }
}
