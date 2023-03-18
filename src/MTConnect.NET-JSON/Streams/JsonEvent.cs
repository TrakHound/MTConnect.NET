// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Linq;

namespace MTConnect.Streams.Json
{
    public class JsonEvent : JsonObservation
    {
        public JsonEvent() { }

        public JsonEvent(IObservation observation, bool categoryOutput = false)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                if (categoryOutput) Category = observation.Category.ToString();
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

        public JsonEvent(IObservationOutput observation)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
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

        public IEventValueObservation ToEventValue()
        {
            var e = new EventValueObservation();
            e.DataItemId = DataItemId;
            e.Timestamp = Timestamp;
            e.Name = Name;
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