// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonEventValue : JsonObservation
    {
        [JsonPropertyName("value")]
        public object Value { get; set; }


        public JsonEventValue() { }

        public JsonEventValue(IObservation observation, bool categoryOutput = false, bool instanceIdOutput = false)
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
                Value = observation.GetValue(ValueKeys.Result);
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                NativeCode = observation.GetValue(ValueKeys.NativeCode);
                AssetType = observation.GetValue(ValueKeys.AssetType);
            }
        }

        public JsonEventValue(IObservationOutput observation)
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
                Value = observation.GetValue(ValueKeys.Result);
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                NativeCode = observation.GetValue(ValueKeys.NativeCode);
                AssetType = observation.GetValue(ValueKeys.AssetType);
            }
        }

        public IEventValueObservation ToObservation(string type)
        {
            var e = new EventValueObservation();
            e.DataItemId = DataItemId;
            e.Timestamp = Timestamp;
            e.Name = Name;
            e.InstanceId = InstanceId;
            e.Sequence = Sequence;
            //e.Category = Category.ConvertEnum<DataItemCategory>();
            e.Type = type;
            e.SubType = SubType;
            e.CompositionId = CompositionId;
            e.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            if (Value != null) e.Result = Value.ToString();
            return e;
        }
    }
}