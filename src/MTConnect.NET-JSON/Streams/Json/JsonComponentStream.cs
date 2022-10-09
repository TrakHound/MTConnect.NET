// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MTConnect.Streams.Output;
using System.Linq;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// ComponentStream is a XML container that organizes the data associated with each Structural Element defined for that piece of equipment in the associated MTConnectDevices XML document
    /// </summary>
    public class JsonComponentStream
    {
        /// <summary>
        /// Component identifies the Structural Element associated with the ComponentStream element.
        /// </summary>
        [JsonPropertyName("component")]
        public string Component { get; set; }

        /// <summary>
        /// The identifier of the Structural Element as defined by the id attribute of the corresponding Structural Element in the MTConnectDevices XML document.
        /// </summary>
        [JsonPropertyName("componentId")]
        public string ComponentId { get; set; }

        /// <summary>
        /// The name of the ComponentStream element.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// NativeName identifies the common name normally associated with the ComponentStream element.
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// Uuid of the ComponentStream element.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonIgnore]
        public List<IObservation> Observations
        {
            get
            {
                var l = new List<IObservation>();

                if (!Samples.IsNullOrEmpty())
                {
                    foreach (var sample in Samples) l.Add(sample.ToSample());
                }

                if (!Events.IsNullOrEmpty())
                {
                    foreach (var e in Events) l.Add(e.ToEvent());
                }

                if (!Conditions.IsNullOrEmpty())
                {
                    foreach (var condition in Conditions) l.Add(condition.ToCondition());
                }

                return l;
            }
        }

        [JsonPropertyName("samples")]
        public IEnumerable<JsonSample> Samples { get; set; }

        [JsonPropertyName("events")]
        public IEnumerable<JsonEvent> Events { get; set; }

        [JsonPropertyName("condition")]
        public IEnumerable<JsonCondition> Conditions { get; set; }


        public JsonComponentStream() { }

        public JsonComponentStream(IComponentStreamOutput componentStream)
        {
            if (componentStream != null)
            {
                //Component = componentStream.Component;
                ComponentId = componentStream.ComponentId;
                Name = componentStream.Name;
                NativeName = componentStream.NativeName;
                Uuid = componentStream.Uuid;

                if (!componentStream.Observations.IsNullOrEmpty())
                {
                    // Add Samples
                    var sampleObservations = componentStream.Observations.Where(o => o.Category == Devices.DataItems.DataItemCategory.SAMPLE);
                    if (!sampleObservations.IsNullOrEmpty())
                    {
                        var samples = new List<JsonSample>();
                        foreach (var observation in sampleObservations)
                        {
                            samples.Add(new JsonSample(observation));
                        }
                        Samples = samples;
                    }

                    // Add Events
                    var eventObservations = componentStream.Observations.Where(o => o.Category == Devices.DataItems.DataItemCategory.EVENT);
                    if (!eventObservations.IsNullOrEmpty())
                    {
                        var events = new List<JsonEvent>();
                        foreach (var observation in eventObservations)
                        {
                            events.Add(new JsonEvent(observation));
                        }
                        Events = events;
                    }

                    // Add Conditions
                    var confiditionObservations = componentStream.Observations.Where(o => o.Category == Devices.DataItems.DataItemCategory.CONDITION);
                    if (!confiditionObservations.IsNullOrEmpty())
                    {
                        var conditions = new List<JsonCondition>();
                        foreach (var observation in confiditionObservations)
                        {
                            conditions.Add(new JsonCondition(observation));
                        }
                        Conditions = conditions;
                    }
                }
            }
        }


        public ComponentStream ToComponentStream()
        {
            var componentStream = new ComponentStream();

            componentStream.ComponentId = ComponentId;
            componentStream.Name = Name;
            componentStream.NativeName = NativeName;
            componentStream.Uuid = Uuid;

            var observations = new List<IObservation>();

            // Add Samples
            if (!Samples.IsNullOrEmpty())
            {
                var samples = new List<SampleObservation>();
                foreach (var sample in Samples)
                {
                    samples.Add(sample.ToSample());
                }
                observations.AddRange(samples);
                //componentStream.Samples = samples;
            }

            // Add Events
            if (!Events.IsNullOrEmpty())
            {
                var events = new List<EventObservation>();
                foreach (var e in Events)
                {
                    events.Add(e.ToEvent());
                }
                observations.AddRange(events);
                //componentStream.Events = events;
            }

            // Add Conditions
            if (!Conditions.IsNullOrEmpty())
            {
                var conditions = new List<ConditionObservation>();
                foreach (var sample in Conditions)
                {
                    conditions.Add(sample.ToCondition());
                }
                observations.AddRange(conditions);
                //componentStream.Conditions = conditions;
            }

            return componentStream;
        }
    }  
}
