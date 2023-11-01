// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.NET_JSON_cppagent.Streams;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonEventTable : JsonObservation
    {
        [JsonPropertyName("value")]
        public JsonTableEntries Entries { get; set; }

        [JsonPropertyName("count")]
        public long? Count { get; set; }


        public JsonEventTable() { }

        public JsonEventTable(IObservation observation, bool categoryOutput = false, bool instanceIdOutput = false)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                if (categoryOutput) Category = observation.Category.ToString();
                if (instanceIdOutput) InstanceId = observation.InstanceId;
                Timestamp = observation.Timestamp;
                Name = observation.Name;
                Sequence = observation.Sequence;
                SubType = observation.SubType;
                CompositionId = observation.CompositionId;
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                NativeCode = observation.GetValue(ValueKeys.NativeCode);
                AssetType = observation.GetValue(ValueKeys.AssetType);

                // Table Entries
                if (observation is EventTableObservation)
                {
                    var tableObservation = (EventTableObservation)observation;

                    if (!tableObservation.Entries.IsNullOrEmpty())
                    {
                        Entries = CreateTableEntries(tableObservation.Entries);
                        Count = Entries != null ? Entries.Count : 0;
                    }
                    else
                    {
                        Entries = new JsonTableEntries(true);
                        Count = 0;
                    }
                }
            }
        }

        public JsonEventTable(IObservationOutput observation)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                Timestamp = observation.Timestamp;
                Name = observation.Name;
                InstanceId = observation.InstanceId;
                Sequence = observation.Sequence;
                SubType = observation.SubType;
                CompositionId = observation.CompositionId;
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                NativeCode = observation.GetValue(ValueKeys.NativeCode);
                AssetType = observation.GetValue(ValueKeys.AssetType);

                // Table Entries
                if (observation.Representation == DataItemRepresentation.TABLE)
                {
                    var tableObservation = new EventTableObservation();
                    tableObservation.AddValues(observation.Values);

                    if (!tableObservation.Entries.IsNullOrEmpty())
                    {
                        Entries = CreateTableEntries(tableObservation.Entries);
                        Count = Entries != null ? Entries.Count : 0;
                    }
                    else
                    {
                        Entries = new JsonTableEntries(true);
                        Count = 0;
                    }
                }
            }
        }

        public IEventTableObservation ToObservation(string type)
        {
            var observation = new EventTableObservation();
            observation.DataItemId = DataItemId;
            observation.Timestamp = Timestamp;
            observation.Name = Name;
            observation.InstanceId = InstanceId;
            observation.Sequence = Sequence;
            observation.Category = Category.ConvertEnum<DataItemCategory>();
            observation.Type = type;
            observation.SubType = SubType;
            observation.CompositionId = CompositionId;
            observation.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();

            if (Entries != null && !Entries.IsUnavailable)
            {
                observation.Entries = CreateTableEntries(Entries.Entries);
            }
            else
            {
                observation.AddValue(ValueKeys.Result, Observation.Unavailable);
            }

            return observation;
        }
    }
}