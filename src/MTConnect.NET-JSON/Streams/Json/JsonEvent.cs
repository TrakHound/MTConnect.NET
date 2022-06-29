// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Observations;
using MTConnect.Streams;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Sample XML elements representing SAMPLE category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Sample XML Elements in a Samples container.
    /// </summary>
    public class JsonEvent : JsonDataItem
    {
        public JsonEvent() { }

        public JsonEvent(EventObservation e)
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
                if (e.ResetTriggered != Observations.ResetTriggered.NOT_SPECIFIED) ResetTriggered = e.ResetTriggered.ToString();
                Result = e.GetValue(ValueKeys.CDATA);
                //Entries = e.Entries;
                //if (e.Count > 0) Count = e.Count;
            }
        }

        public JsonEvent(Observation e)
        {
            if (e != null)
            {
                DataItemId = e.DataItemId;
                Timestamp = e.Timestamp;
                Name = e.Name;
                Sequence = e.Sequence;
                Type = e.Type.ToPascalCase();
                SubType = e.SubType.ToPascalCase();
                CompositionId = e.CompositionId;
                //if (e.ResetTriggered != Streams.ResetTriggered.NOT_SPECIFIED) ResetTriggered = e.ResetTriggered.ToString();
                Result = e.GetValue(ValueKeys.CDATA);
                //Entries = e.Entries;
                //if (e.Count > 0) Count = e.Count;
            }
        }

        public EventObservation ToEvent()
        {
            var e = new EventObservation();
            //e.DataItemId = DataItemId;
            //e.Timestamp = Timestamp;
            //e.Name = Name;
            //e.Sequence = Sequence;
            //e.Category = Devices.DataItemCategory.EVENT;
            //e.Type = Type;
            //e.SubType = SubType;
            //e.CompositionId = CompositionId;
            //e.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            ////e.CDATA = CDATA;
            ////e.Entries = Entries;
            //e.Count = Count.HasValue ? Count.Value : 0; ;
            return e;
        }

        public Observations.Observation ToObservation()
        {
            var e = new Observations.Observation();
            //e.DataItemId = DataItemId;
            //e.Timestamp = Timestamp;
            //e.Name = Name;
            //e.Sequence = Sequence;
            //e.Category = Devices.DataItemCategory.EVENT;
            //e.Type = Type;
            //e.SubType = SubType;
            //e.CompositionId = CompositionId;
            //e.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            //e.CDATA = CDATA;
            //e.Entries = Entries;
            //e.Count = Count.HasValue ? Count.Value : 0; ;
            return e;
        }
    }
}
