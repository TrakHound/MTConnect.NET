// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Linq;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for an EVENT-category observation,
    /// extending <see cref="JsonObservation"/> with event-specific projection
    /// of data-set and table payloads. Converts to and from the strongly-typed
    /// <see cref="EventValueObservation"/> model.
    /// </summary>
    public class JsonEvent : JsonObservation
    {
        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonEvent() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IObservation"/>, optionally emitting the category and
        /// instance id, and projecting data-set or table payloads into the
        /// <see cref="JsonObservation.Entries"/> property.
        /// </summary>
        public JsonEvent(IObservation observation, bool categoryOutput = false, bool instanceIdOutput = false)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                if (categoryOutput) Category = observation.Category.ToString();
                if (instanceIdOutput) InstanceId = observation.InstanceId;
                Timestamp = observation.Timestamp;
                Name = observation.Name;
                Sequence = observation.Sequence;
                Type = observation.Type;
                SubType = observation.SubType;
                CompositionId = observation.CompositionId;
                Result = observation.GetValue(ValueKeys.Result);
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                NativeCode = observation.GetValue(ValueKeys.NativeCode);
                AssetType = observation.GetValue(ValueKeys.AssetType);


                // DataSet Entries
                if (observation is EventDataSetObservation)
                {
                    Entries = CreateEntries(((EventDataSetObservation)observation).Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }

                // Table Entries
                if (observation is EventTableObservation)
                {
                    Entries = CreateEntries(((EventTableObservation)observation).Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }
            }
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IObservationOutput"/>, reconstructing the data-set or
        /// table payload from the observation's raw values.
        /// </summary>
        public JsonEvent(IObservationOutput observation)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                Timestamp = observation.Timestamp;
                Name = observation.Name;
                InstanceId = observation.InstanceId;
                Sequence = observation.Sequence;
                Type = observation.Type;
                SubType = observation.SubType;
                CompositionId = observation.CompositionId;
                Result = observation.GetValue(ValueKeys.Result);
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                NativeCode = observation.GetValue(ValueKeys.NativeCode);
                AssetType = observation.GetValue(ValueKeys.AssetType);

                // DataSet Entries
                if (observation.Representation == DataItemRepresentation.DATA_SET)
                {
                    var dataSetObservation = new EventDataSetObservation();
                    dataSetObservation.AddValues(observation.Values);
                    Entries = CreateEntries(dataSetObservation.Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }

                // Table Entries
                if (observation.Representation == DataItemRepresentation.TABLE)
                {
                    var tableObservation = new EventTableObservation();
                    tableObservation.AddValues(observation.Values);
                    Entries = CreateEntries(tableObservation.Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IEventObservation"/>; currently delegates to
        /// <see cref="ToEventValue"/> for all representations.
        /// </summary>
        public IEventObservation ToEvent()
        {
            if (Representation == DataItemRepresentation.DATA_SET.ToString())
            {

            }
            else if (Representation == DataItemRepresentation.TABLE.ToString())
            {

            }
            else if (Representation == DataItemRepresentation.TIME_SERIES.ToString())
            {

            }

            return ToEventValue();
        }

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IEventValueObservation"/>, parsing the category and reset
        /// trigger enumerations.
        /// </summary>
        public IEventValueObservation ToEventValue()
        {
            var e = new EventValueObservation();
            e.DataItemId = DataItemId;
            e.Timestamp = Timestamp;
            e.Name = Name;
            e.InstanceId = InstanceId;
            e.Sequence = Sequence;
            e.Category = Category.ConvertEnum<DataItemCategory>();
            e.Type = Type;
            e.SubType = SubType;
            e.CompositionId = CompositionId;
            e.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            e.Result = Result;
            return e;
        }
    }
}