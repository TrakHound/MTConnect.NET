// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// Typed representation of a Condition list on a Component stream,
    /// bucketed by Condition level (Fault, Warning, Normal, Unavailable).
    /// </summary>
    /// <remarks>
    /// Serialized via <see cref="JsonConditionsConverter"/> in the
    /// cppagent JSON v2 wire shape: an array of single-key wrapper
    /// objects, one per Condition entry
    /// (e.g. <c>[{"Normal": {...}}, {"Warning": {...}}]</c>).
    /// The level order on the wire is fixed at Fault, Warning, Normal,
    /// Unavailable; mixed-level interleaving is not round-trip preserved
    /// through this typed model. The legacy MTConnect JSON v1
    /// object-keyed shape (<c>{"Fault": [...], "Warning": [...], ...}</c>)
    /// is still accepted on the read path for back-compat.
    /// </remarks>
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonConditionsConverter))]
    public class JsonConditions
    {
        /// <summary>
        /// Materializes every level bucket into a flat list of
        /// <see cref="IObservation"/> instances, tagged with the
        /// corresponding <see cref="ConditionLevel"/>. Enumeration order
        /// matches the wire-emission order: Fault, then Warning, then
        /// Normal, then Unavailable.
        /// </summary>
        [JsonIgnore]
        public List<IObservation> Observations
        {
            get
            {
                var l = new List<IObservation>();

                if (!Fault.IsNullOrEmpty())
                {
                    foreach (var x in Fault) l.Add(x.ToCondition(ConditionLevel.FAULT));
                }

                if (!Warning.IsNullOrEmpty())
                {
                    foreach (var x in Warning) l.Add(x.ToCondition(ConditionLevel.WARNING));
                }

                if (!Normal.IsNullOrEmpty())
                {
                    foreach (var x in Normal) l.Add(x.ToCondition(ConditionLevel.NORMAL));
                }

                if (!Unavailable.IsNullOrEmpty())
                {
                    foreach (var x in Unavailable) l.Add(x.ToCondition(ConditionLevel.UNAVAILABLE));
                }

                return l;
            }
        }

        /// <summary>
        /// Condition entries at <c>FAULT</c> level. Source order is
        /// preserved within the bucket; entries are emitted on the wire
        /// as <c>{"Fault": {...}}</c> wrapper objects, ahead of every
        /// other level.
        /// </summary>
        [JsonPropertyName("Fault")]
        public IEnumerable<JsonCondition> Fault { get; set; }

        /// <summary>
        /// Condition entries at <c>WARNING</c> level. Source order is
        /// preserved within the bucket; entries are emitted on the wire
        /// as <c>{"Warning": {...}}</c> wrapper objects, after Fault
        /// and before Normal.
        /// </summary>
        [JsonPropertyName("Warning")]
        public IEnumerable<JsonCondition> Warning { get; set; }

        /// <summary>
        /// Condition entries at <c>NORMAL</c> level. Source order is
        /// preserved within the bucket; entries are emitted on the wire
        /// as <c>{"Normal": {...}}</c> wrapper objects, after Warning
        /// and before Unavailable.
        /// </summary>
        [JsonPropertyName("Normal")]
        public IEnumerable<JsonCondition> Normal { get; set; }

        /// <summary>
        /// Condition entries at <c>UNAVAILABLE</c> level. Source order
        /// is preserved within the bucket; entries are emitted on the
        /// wire as <c>{"Unavailable": {...}}</c> wrapper objects, after
        /// every other level.
        /// </summary>
        [JsonPropertyName("Unavailable")]
        public IEnumerable<JsonCondition> Unavailable { get; set; }


        public JsonConditions() { }

        public JsonConditions(IEnumerable<IObservationOutput> observations)
        {
            if (observations != null)
            {
                if (!observations.IsNullOrEmpty())
                {
                    // Add Fault
                    var levelObservations = observations.Where(o => o.GetValue(ValueKeys.Level) == ConditionLevel.FAULT.ToString());
                    if (!levelObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonCondition>();
                        foreach (var observation in levelObservations)
                        {
                            jsonObservations.Add(new JsonCondition(observation));
                        }
                        Fault = jsonObservations;
                    }

                    // Add Warning
                    levelObservations = observations.Where(o => o.GetValue(ValueKeys.Level) == ConditionLevel.WARNING.ToString());
                    if (!levelObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonCondition>();
                        foreach (var observation in levelObservations)
                        {
                            jsonObservations.Add(new JsonCondition(observation));
                        }
                        Warning = jsonObservations;
                    }

                    // Add Normal
                    levelObservations = observations.Where(o => o.GetValue(ValueKeys.Level) == ConditionLevel.NORMAL.ToString());
                    if (!levelObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonCondition>();
                        foreach (var observation in levelObservations)
                        {
                            jsonObservations.Add(new JsonCondition(observation));
                        }
                        Normal = jsonObservations;
                    }

                    // Add Unavailable
                    levelObservations = observations.Where(o => o.GetValue(ValueKeys.Level) == ConditionLevel.UNAVAILABLE.ToString());
                    if (!levelObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonCondition>();
                        foreach (var observation in levelObservations)
                        {
                            jsonObservations.Add(new JsonCondition(observation));
                        }
                        Unavailable = jsonObservations;
                    }
                }
            }
        }
    }
}
