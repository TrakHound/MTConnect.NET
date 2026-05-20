// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.NET_JSON_cppagent.Streams;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a SAMPLE observation carrying a
    /// DATA_SET representation in the cppagent-compatible Streams shape.
    /// The keyed entries are emitted as a JSON object on the
    /// <c>value</c> property (via <see cref="JsonDataSetEntries"/>) and
    /// the entry count is carried alongside.
    /// </summary>
    public class JsonSampleDataSet : JsonObservation
    {
        /// <summary>
        /// The keyed entries of the data set, emitted as a JSON object
        /// or as the <c>UNAVAILABLE</c> string sentinel.
        /// </summary>
        [JsonPropertyName("value")]
        public JsonDataSetEntries Entries { get; set; }

        /// <summary>
        /// The number of entries in the data set.
        /// </summary>
        [JsonPropertyName("count")]
        public long? Count { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSampleDataSet() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed data-set
        /// sample <see cref="IObservation"/>, optionally surfacing
        /// category and instance-id. Emits an unavailable marker when
        /// the source has no entries.
        /// </summary>
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

        /// <summary>
        /// Initializes the surrogate from a streaming
        /// <see cref="IObservationOutput"/>, rehydrating the data-set
        /// entries from the observation's value bag.
        /// </summary>
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

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISampleDataSetObservation"/>, restoring the
        /// data-item type from the supplied dictionary key, or emitting
        /// the <c>UNAVAILABLE</c> result when the entries are marked
        /// unavailable.
        /// </summary>
        public ISampleDataSetObservation ToObservation(string type)
        {
            // Route construction through the typed factory so the runtime
            // type discriminator survives the envelope read path.
            var observation = SampleObservation.Create(type, DataItemRepresentation.DATA_SET) as SampleDataSetObservation;
            if (observation == null) observation = new SampleDataSetObservation();
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