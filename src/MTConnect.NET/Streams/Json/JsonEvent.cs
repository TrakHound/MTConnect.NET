// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Sample XML elements representing SAMPLE category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Sample XML Elements in a Samples container.
    /// </summary>
    public class JsonEvent : JsonDataItem
    {
        public JsonEvent() { }

        public JsonEvent(Event e)
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
                if (e.ResetTriggered != Devices.DataItemResetTrigger.NONE) ResetTriggered = e.ResetTriggered.ToString();
                CDATA = e.CDATA;
                Entries = e.Entries;
                if (e.Count > 0) Count = e.Count;
            }
        }

        public Event ToEvent()
        {
            var e = new Event();
            e.DataItemId = DataItemId;
            e.Timestamp = Timestamp;
            e.Name = Name;
            e.Sequence = Sequence;
            e.Category = Devices.DataItemCategory.EVENT;
            e.Type = Type;
            e.SubType = SubType;
            e.CompositionId = CompositionId;
            e.ResetTriggered = ResetTriggered.ConvertEnum<Devices.DataItemResetTrigger>();
            e.CDATA = CDATA;
            e.Entries = Entries;
            e.Count = Count.HasValue ? Count.Value : 0; ;
            return e;
        }
    }
}
