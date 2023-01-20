// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonComponentStream
    {
        [JsonPropertyName("component")]
        public string Component { get; set; }

        [JsonPropertyName("componentId")]
        public string ComponentId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

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
                Component = componentStream.ComponentType;
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
            }

            return componentStream;
        }
    }  
}