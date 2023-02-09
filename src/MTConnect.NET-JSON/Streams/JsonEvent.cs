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

        public JsonEvent(IObservation e)
        {
            if (e != null)
            {
                DataItemId = e.DataItemId;
                Timestamp = e.Timestamp;
                Name = e.Name;
                Sequence = e.Sequence;
                Type = e.Type;
                SubType = e.SubType;
                CompositionId = e.CompositionId;
                Result = e.GetValue(ValueKeys.Result);
                ResetTriggered = e.GetValue(ValueKeys.ResetTriggered);
                NativeCode = e.GetValue(ValueKeys.NativeCode);
                AssetType = e.GetValue(ValueKeys.AssetType);


                // DataSet Entries
                if (e is EventDataSetObservation)
                {
                    Entries = CreateEntries(((EventDataSetObservation)e).Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }

                // Table Entries
                if (e is EventTableObservation)
                {
                    Entries = CreateEntries(((EventTableObservation)e).Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }
            }
        }

        public JsonEvent(IObservationOutput e)
        {
            if (e != null)
            {
                DataItemId = e.DataItemId;
                Timestamp = e.Timestamp;
                Name = e.Name;
                Sequence = e.Sequence;
                Type = e.Type;
                SubType = e.SubType;
                CompositionId = e.CompositionId;
                Result = e.GetValue(ValueKeys.Result);
                ResetTriggered = e.GetValue(ValueKeys.ResetTriggered);
                NativeCode = e.GetValue(ValueKeys.NativeCode);
                AssetType = e.GetValue(ValueKeys.AssetType);

                // DataSet Entries
                if (e.Representation == DataItemRepresentation.DATA_SET)
                {
                    var dataSetObservation = new EventDataSetObservation();
                    dataSetObservation.AddValues(e.Values);
                    Entries = CreateEntries(dataSetObservation.Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }

                // Table Entries
                if (e.Representation == DataItemRepresentation.TABLE)
                {
                    var tableObservation = new EventTableObservation();
                    tableObservation.AddValues(e.Values);
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