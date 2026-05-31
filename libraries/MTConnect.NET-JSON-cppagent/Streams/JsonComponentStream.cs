// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>ComponentStream</c> in the
    /// cppagent-compatible Streams shape. Observations belonging to the
    /// component are partitioned into the three sibling containers
    /// <c>Samples</c>, <c>Events</c>, and <c>Condition</c> by data-item
    /// category and emitted as keyed dictionaries rather than the XML
    /// element-array form. Converts to and from the strongly-typed
    /// <see cref="ComponentStream"/> model.
    /// </summary>
    public class JsonComponentStream
    {
        /// <summary>
        /// The element name of the component (for example
        /// <c>Axes</c>, <c>Controller</c>).
        /// </summary>
        [JsonPropertyName("component")]
        public string Component { get; set; }

        /// <summary>
        /// The unique <c>id</c> of the component this stream is reporting
        /// observations for.
        /// </summary>
        [JsonPropertyName("componentId")]
        public string ComponentId { get; set; }

        /// <summary>
        /// The descriptive name of the component.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The native name of the component as used on the equipment.
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The UUID of the component.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Aggregated view of every observation across the three category
        /// containers; not serialized.
        /// </summary>
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

        /// <summary>
        /// SAMPLE-category observations keyed by data-item identifier.
        /// </summary>
        [JsonPropertyName("Samples")]
        public JsonSamples Samples { get; set; }

        /// <summary>
        /// EVENT-category observations keyed by data-item identifier.
        /// </summary>
        [JsonPropertyName("Events")]
        public JsonEvents Events { get; set; }

        /// <summary>
        /// CONDITION-category observations keyed by data-item identifier
        /// (note the singular JSON key <c>Condition</c>).
        /// </summary>
        [JsonPropertyName("Condition")]
        public JsonConditions Conditions { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonComponentStream() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IComponentStreamOutput"/>, partitioning the
        /// observations into SAMPLE, EVENT, and CONDITION containers and
        /// suppressing any container that has no observations.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ComponentStream"/>, flattening the three category
        /// containers back into a single observations list.
        /// </summary>
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