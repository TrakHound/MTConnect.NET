// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonEventTable : JsonObservation
    {
        [JsonPropertyName("value")]
        public Dictionary<string, Dictionary<string, object>> Entries { get; set; }

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
                    Entries = CreateTableEntries(((EventTableObservation)observation).Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
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

                // DataSet Entries
                if (observation.Representation == DataItemRepresentation.TABLE)
                {
                    var dataSetObservation = new EventTableObservation();
                    dataSetObservation.AddValues(observation.Values);
                    Entries = CreateTableEntries(dataSetObservation.Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }
            }
        }

        public IEventTableObservation ToObservation(string type)
        {
            var e = new EventTableObservation();
            e.DataItemId = DataItemId;
            e.Timestamp = Timestamp;
            e.Name = Name;
            e.InstanceId = InstanceId;
            e.Sequence = Sequence;
            e.Category = Category.ConvertEnum<DataItemCategory>();
            e.Type = type;
            e.SubType = SubType;
            e.CompositionId = CompositionId;
            e.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            e.Entries = CreateTableEntries(Entries);
            return e;
        }
    }
}