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
    /// JSON serialization surrogate for an EVENT observation carrying a
    /// TABLE representation in the cppagent-compatible Streams shape.
    /// Mirrors the SAMPLE table surrogate but emits under the EVENTS
    /// branch of the parent <c>ComponentStream</c>.
    /// </summary>
    public class JsonEventTable : JsonObservation
    {
        /// <summary>
        /// The keyed table rows, emitted as a nested JSON object or as
        /// the <c>UNAVAILABLE</c> string sentinel.
        /// </summary>
        [JsonPropertyName("value")]
        public JsonTableEntries Entries { get; set; }

        /// <summary>
        /// The number of rows in the table.
        /// </summary>
        [JsonPropertyName("count")]
        public long? Count { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonEventTable() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed table event
        /// <see cref="IObservation"/>, optionally surfacing category
        /// and instance-id. Emits an unavailable marker when the source
        /// has no rows.
        /// </summary>
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

        /// <summary>
        /// Initializes the surrogate from a streaming
        /// <see cref="IObservationOutput"/>, rehydrating the table rows
        /// from the observation's value bag.
        /// </summary>
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

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IEventTableObservation"/>, restoring the
        /// data-item type from the supplied dictionary key, or emitting
        /// the <c>UNAVAILABLE</c> result when the rows are marked
        /// unavailable.
        /// </summary>
        public IEventTableObservation ToObservation(string type)
        {
            // Route construction through the typed factory so the runtime
            // type discriminator survives the envelope read path.
            var observation = EventObservation.Create(type, DataItemRepresentation.TABLE) as EventTableObservation;
            if (observation == null) observation = new EventTableObservation();
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