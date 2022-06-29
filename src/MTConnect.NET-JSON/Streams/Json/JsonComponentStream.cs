// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using MTConnect.Streams;
using MTConnect.Observations;

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

        public JsonComponentStream(IComponentStream componentStream)
        {
            if (componentStream != null)
            {
                //Component = componentStream.Component;
                ComponentId = componentStream.ComponentId;
                Name = componentStream.Name;
                NativeName = componentStream.NativeName;
                Uuid = componentStream.Uuid;

                // Add Samples
                if (!componentStream.Samples.IsNullOrEmpty())
                {
                    var samples = new List<JsonSample>();
                    foreach (var sample in componentStream.Samples)
                    {
                        samples.Add(new JsonSample(sample));
                    }
                    Samples = samples;
                }

                // Add Events
                if (!componentStream.Events.IsNullOrEmpty())
                {
                    var events = new List<JsonEvent>();
                    foreach (var e in componentStream.Events)
                    {
                        events.Add(new JsonEvent(e));
                    }
                    Events = events;
                }

                // Add Conditions
                if (!componentStream.Conditions.IsNullOrEmpty())
                {
                    var conditions = new List<JsonCondition>();
                    foreach (var condition in componentStream.Conditions)
                    {
                        conditions.Add(new JsonCondition(condition));
                    }
                    Conditions = conditions;
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

            // Add Samples
            if (!Samples.IsNullOrEmpty())
            {
                var samples = new List<SampleObservation>();
                foreach (var sample in Samples)
                {
                    samples.Add(sample.ToSample());
                }
                componentStream.Samples = samples;
            }

            // Add Events
            if (!Events.IsNullOrEmpty())
            {
                var events = new List<EventObservation>();
                foreach (var e in Events)
                {
                    events.Add(e.ToEvent());
                }
                componentStream.Events = events;
            }

            // Add Conditions
            if (!Conditions.IsNullOrEmpty())
            {
                var conditions = new List<ConditionObservation>();
                foreach (var sample in Conditions)
                {
                    conditions.Add(sample.ToCondition());
                }
                componentStream.Conditions = conditions;
            }

            return componentStream;
        }
    }  
}
