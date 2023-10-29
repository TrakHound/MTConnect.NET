// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonConditions
    {
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

        [JsonPropertyName("Fault")]
        public IEnumerable<JsonCondition> Fault { get; set; }

        [JsonPropertyName("Warning")]
        public IEnumerable<JsonCondition> Warning { get; set; }

        [JsonPropertyName("Normal")]
        public IEnumerable<JsonCondition> Normal { get; set; }

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