// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonCondition : JsonObservation
    {
        [JsonPropertyName("level")]
        public string Level { get; set; }

        [JsonPropertyName("nativeSeverity")]
        public string NativeSeverity { get; set; }

        [JsonPropertyName("qualifier")]
        public string Qualifier { get; set; }

        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }


        public JsonCondition() { }

        public JsonCondition(IObservation observation)
        {
            var condition = observation as ConditionObservation;
            if (condition != null)
            {
                DataItemId = condition.DataItemId;
                Timestamp = condition.Timestamp;
                Name = condition.Name;
                Sequence = condition.Sequence;
                Type = condition.Type;
                SubType = condition.SubType;
                CompositionId = condition.CompositionId;
                if (!string.IsNullOrEmpty(condition.Message)) Result = condition.Message;

                Level = condition.Level.ToString();
                NativeCode = condition.NativeCode;
                NativeSeverity = condition.NativeSeverity;
                if (condition.Qualifier != ConditionQualifier.NOT_SPECIFIED) Qualifier = condition.Qualifier.ToString();
            }
        }

        public JsonCondition(IObservationOutput condition)
        {
            if (condition != null)
            {
                DataItemId = condition.DataItemId;
                Timestamp = condition.Timestamp;
                Name = condition.Name;
                Sequence = condition.Sequence;
                Type = condition.Type;
                SubType = condition.SubType;
                CompositionId = condition.CompositionId;

                // Message
                var message = condition.GetValue(ValueKeys.Message);
                if (!string.IsNullOrEmpty(message)) Result = message;

                // Level
                var level = condition.GetValue(ValueKeys.Level);
                if (!string.IsNullOrEmpty(level)) Level = level;

                // NativeCode
                var nativeCode = condition.GetValue(ValueKeys.NativeCode);
                if (!string.IsNullOrEmpty(nativeCode)) NativeCode = nativeCode;

                // NativeSeverity
                var nativeSeverity = condition.GetValue(ValueKeys.NativeSeverity);
                if (!string.IsNullOrEmpty(nativeSeverity)) NativeSeverity = nativeSeverity;

                // Qualifier
                var qualifier = condition.GetValue(ValueKeys.Qualifier);
                if (!string.IsNullOrEmpty(qualifier)) Qualifier = qualifier;
            }
        }

        public ConditionObservation ToCondition()
        {
            var condition = new ConditionObservation();
            //condition.DataItemId = DataItemId;
            //condition.Timestamp = Timestamp;
            //condition.Name = Name;
            //condition.Sequence = Sequence;
            //condition.Category = Devices.DataItemCategory.CONDITION;
            //condition.Type = Type;
            //condition.SubType = SubType;
            //condition.CompositionId = CompositionId;
            //condition.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            //condition.CDATA = CDATA;
            //condition.Entries = Entries;
            //condition.Count = Count.HasValue ? Count.Value : 0; ;

            //condition.Level = Level.ConvertEnum<ConditionLevel>();
            //condition.NativeCode = NativeCode;
            //condition.NativeSeverity = NativeSeverity;
            //condition.Qualifier = Qualifier.ConvertEnum<ConditionQualifier>();

            return condition;
        }
    }
}
