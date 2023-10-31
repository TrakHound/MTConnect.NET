// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonSampleTable : JsonObservation
    {
        [JsonPropertyName("value")]
        [JsonConverter(typeof(JsonSampleTableValueConverter))]
        public Dictionary<string, Dictionary<string, object>> Entries { get; set; }

        [JsonPropertyName("count")]
        public long? Count { get; set; }


        public JsonSampleTable() { }

        public JsonSampleTable(IObservation observation, bool categoryOutput = false, bool instanceIdOutput = false)
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
                if (observation is SampleTableObservation)
                {
                    Entries = CreateTableEntries(((SampleTableObservation)observation).Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }
            }
        }

        public JsonSampleTable(IObservationOutput observation)
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

                // DataSet Entries
                if (observation.Representation == DataItemRepresentation.TABLE)
                {
                    var dataSetObservation = new SampleTableObservation();
                    dataSetObservation.AddValues(observation.Values);
                    Entries = CreateTableEntries(dataSetObservation.Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }
            }
        }

        public ISampleTableObservation ToObservation(string type)
        {
            var observation = new SampleTableObservation();
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

            if (Entries != null)
            {
                observation.Entries = CreateTableEntries(Entries);
            }
            else
            {
                observation.AddValue(ValueKeys.Result, Observation.Unavailable);
            }

            return observation;
        }

        public class JsonSampleTableValueConverter : JsonConverter<Dictionary<string, Dictionary<string, object>>>
        {
            public override Dictionary<string, Dictionary<string, object>> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var obj = JsonObject.Parse(ref reader);
                    return obj.Deserialize<Dictionary<string, Dictionary<string, object>>>();
                }
                else
                {
                    reader.Skip(); // Unavailable
                }

                return null;
            }

            public override void Write(Utf8JsonWriter writer, Dictionary<string, Dictionary<string, object>> value, JsonSerializerOptions options)
            {
                //writer.WriteStringValue(dateTimeValue.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            }
        }
    }
}