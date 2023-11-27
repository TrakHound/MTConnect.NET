// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.NET_JSON_cppagent.Streams;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonSampleDataSet : JsonObservation
    {
        [JsonPropertyName("value")]
        public JsonDataSetEntries Entries { get; set; }

        [JsonPropertyName("count")]
        public long? Count { get; set; }


        public JsonSampleDataSet() { }

        public JsonSampleDataSet(IObservation observation, bool categoryOutput = false, bool instanceIdOutput = false)
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

                // DataSet Entries
                if (observation is SampleDataSetObservation)
                {
                    var dataSetObservation = (SampleDataSetObservation)observation;

                    if (!dataSetObservation.Entries.IsNullOrEmpty())
                    {
                        Entries = CreateDataSetEntries(dataSetObservation.Entries);
                        Count = Entries != null ? Entries.Count : 0;
                    }
                    else
                    {
                        Entries = new JsonDataSetEntries(true);
                        Count = 0;
                    }
                }
            }
        }

        public JsonSampleDataSet(IObservationOutput observation)
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
                if (observation.Representation == DataItemRepresentation.DATA_SET)
                {
                    var dataSetObservation = new SampleDataSetObservation();
                    dataSetObservation.AddValues(observation.Values);

                    if (!dataSetObservation.Entries.IsNullOrEmpty())
                    {
                        Entries = CreateDataSetEntries(dataSetObservation.Entries);
                        Count = Entries != null ? Entries.Count : 0;
                    }
                    else
                    {
                        Entries = new JsonDataSetEntries(true);
                        Count = 0;
                    }
                }
            }
        }

        public ISampleDataSetObservation ToObservation(string type)
        {
            var observation = new SampleDataSetObservation();
            observation.DataItemId = DataItemId;
            observation.Timestamp = Timestamp;
            observation.Name = Name;
            observation.InstanceId = InstanceId;
            observation.Sequence = Sequence;
            //observation.Category = Category.ConvertEnum<DataItemCategory>();
            observation.Type = type;
            observation.SubType = SubType;
            observation.CompositionId = CompositionId;
            observation.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();

            if (Entries != null && !Entries.IsUnavailable)
            {
                observation.Entries = CreateDataSetEntries(Entries.Entries);
            }
            else
            {
                observation.AddValue(ValueKeys.Result, Observation.Unavailable);
            }

            return observation;
        }
    }
}