// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams
{
    /// <summary>
    /// ComponentStream is a XML container that organizes the data associated with each Structural Element defined for that piece of equipment in the associated MTConnectDevices XML document
    /// </summary>
    public class ComponentStream : IComponentStream
    {
        /// <summary>
        /// Component identifies the Structural Element associated with the ComponentStream element.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IComponent Component { get; set; }

        [XmlAttribute("component")]
        [JsonPropertyName("component")]
        public string ComponentType { get; set; }

        /// <summary>
        /// The identifier of the Structural Element as defined by the id attribute of the corresponding Structural Element in the MTConnectDevices XML document.
        /// </summary>
        [XmlAttribute("componentId")]
        [JsonPropertyName("componentId")]
        public string ComponentId { get; set; }

        /// <summary>
        /// The name of the ComponentStream element.
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// NativeName identifies the common name normally associated with the ComponentStream element.
        /// </summary>
        [XmlAttribute("nativeName")]
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// Uuid of the ComponentStream element.
        /// </summary>
        [XmlAttribute("uuid")]
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Returns All Observations for the ComponentStream
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<Observation> Observations
        {
            get
            {
                var l = new List<Observation>();

                if (!Samples.IsNullOrEmpty()) l.AddRange(Samples);
                if (!Events.IsNullOrEmpty()) l.AddRange(Events);
                if (!Conditions.IsNullOrEmpty()) l.AddRange(Conditions);

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE
        /// </summary>
        [XmlIgnore]
        [JsonPropertyName("samples")]
        public IEnumerable<SampleObservation> Samples { get; set; }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of VALUE
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<SampleValueObservation> SampleValues => GetObservations<SampleValueObservation>(Samples);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TIME_SERIES
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<SampleTimeSeriesObservation> SampleTimeSeries => GetObservations<SampleTimeSeriesObservation>(Samples);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of DATA_SET
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<SampleDataSetObservation> SampleDataSets => GetObservations<SampleDataSetObservation>(Samples);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TABLE
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<SampleTableObservation> SampleTables => GetObservations<SampleTableObservation>(Samples);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT
        /// </summary>
        [XmlIgnore]
        [JsonPropertyName("events")]
        public IEnumerable<EventObservation> Events { get; set; }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of VALUE
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<EventValueObservation> EventValues => GetObservations<EventValueObservation>(Events);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of DATA_SET
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<EventDataSetObservation> EventDataSets => GetObservations<EventDataSetObservation>(Events);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of TABLE
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<EventTableObservation> EventTables => GetObservations<EventTableObservation>(Events);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of CONDITION
        /// </summary>
        [XmlIgnore]
        [JsonPropertyName("condition")]
        public IEnumerable<ConditionObservation> Conditions { get; set; }


        public ComponentStream()
        {
            Samples = Enumerable.Empty<SampleObservation>();
            Events = Enumerable.Empty<EventObservation>();
            Conditions = Enumerable.Empty<ConditionObservation>();
        }


        private IEnumerable<T> GetObservations<T>(IEnumerable<Observation> observations) where T : Observation
        {
            var l = new List<T>();
            if (!observations.IsNullOrEmpty())
            {
                var x = observations.Where(o => o.GetType().IsAssignableFrom(typeof(T)));
                if (!x.IsNullOrEmpty())
                {
                    foreach (var y in x) l.Add((T)y);
                }
            }
            return l;
        }
    }  
}
