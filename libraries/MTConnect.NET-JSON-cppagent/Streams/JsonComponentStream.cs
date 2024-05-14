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

                if (Samples != null)
                {
                    foreach (var observation in Samples.Observations) l.Add(observation);
                }

                if (Events != null)
                {
                    foreach (var observation in Events.Observations) l.Add(observation);
                }

                if (Conditions != null)
                {
                    foreach (var observation in Conditions.Observations) l.Add(observation);
                }

                return l;
            }
        }

        [JsonPropertyName("Samples")]
        public JsonSamples Samples { get; set; }

        [JsonPropertyName("Events")]
        public JsonEvents Events { get; set; }

        [JsonPropertyName("Condition")]
        public JsonConditions Conditions { get; set; }


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
                    var sampleObservations = componentStream.Observations.Where(o => o.Category == Devices.DataItemCategory.SAMPLE);
                    if (!sampleObservations.IsNullOrEmpty())
                    {
                        Samples = new JsonSamples(sampleObservations);
                    }

                    // Add Events
                    var eventObservations = componentStream.Observations.Where(o => o.Category == Devices.DataItemCategory.EVENT);
                    if (!eventObservations.IsNullOrEmpty())
                    {
                        Events = new JsonEvents(eventObservations);
                    }

                    // Add Conditions
                    var conditionObservations = componentStream.Observations.Where(o => o.Category == Devices.DataItemCategory.CONDITION);
                    if (!conditionObservations.IsNullOrEmpty())
                    {
                        Conditions = new JsonConditions(conditionObservations);
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
            if (Samples != null)
            {
                observations.AddRange(Samples.Observations);
            }

            // Add Events
            if (Events != null)
            {
                observations.AddRange(Events.Observations);
            }

            // Add Conditions
            if (Conditions != null)
            {
                observations.AddRange(Conditions.Observations);
            }

            componentStream.Observations = observations;

            return componentStream;
        }
    }  
}