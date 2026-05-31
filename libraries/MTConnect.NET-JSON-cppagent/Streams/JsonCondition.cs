// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a CONDITION observation in the
    /// cppagent-compatible Streams shape. Each entry in the parent
    /// <see cref="JsonConditions"/> map carries the condition's type,
    /// native severity/code, qualifier, and message. The condition level
    /// (NORMAL/WARNING/FAULT/UNAVAILABLE) is represented by the parent
    /// dictionary key rather than a field on this surrogate, so it is
    /// supplied to <see cref="ToCondition"/>.
    /// </summary>
    public class JsonCondition : JsonObservation
    {
        /// <summary>
        /// The MTConnect type of the underlying data item (for example
        /// <c>ACTUATOR</c>, <c>SYSTEM</c>).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        //[JsonPropertyName("level")]
        //public string Level { get; set; }

        /// <summary>
        /// Optional native severity reported by the equipment alongside
        /// the native code.
        /// </summary>
        [JsonPropertyName("nativeSeverity")]
        public string NativeSeverity { get; set; }

        /// <summary>
        /// Optional qualifier refining the condition (HIGH, LOW) when
        /// not <c>NOT_SPECIFIED</c>.
        /// </summary>
        [JsonPropertyName("qualifier")]
        public string Qualifier { get; set; }

        /// <summary>
        /// Optional statistic applied to the underlying data, mirroring
        /// the SAMPLE statistic attribute.
        /// </summary>
        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }

        /// <summary>
        /// The free-form condition message text reported by the
        /// equipment.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCondition() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed condition
        /// <see cref="IObservation"/>, optionally surfacing the category
        /// and instance-id fields. The qualifier is suppressed when it
        /// equals the implicit <c>NOT_SPECIFIED</c> default.
        /// </summary>
        public JsonCondition(IObservation observation, bool categoryOutput = false, bool instanceIdOutput = false)
        {
            var condition = observation as ConditionObservation;
            if (condition != null)
            {
                DataItemId = condition.DataItemId;
                if (categoryOutput) Category = observation.Category.ToString();
                if (instanceIdOutput) InstanceId = observation.InstanceId;
                Timestamp = condition.Timestamp;
                Name = condition.Name;
                Sequence = condition.Sequence;
                Type = condition.Type;
                SubType = condition.SubType;
                CompositionId = condition.CompositionId;
                if (!string.IsNullOrEmpty(condition.Message)) Value = condition.Message;

                //Level = condition.Level.ToString();
                NativeCode = condition.NativeCode;
                NativeSeverity = condition.NativeSeverity;
                if (condition.Qualifier != ConditionQualifier.NOT_SPECIFIED) Qualifier = condition.Qualifier.ToString();
            }
        }

        /// <summary>
        /// Initializes the surrogate from a streaming
        /// <see cref="IObservationOutput"/>, pulling message, native
        /// code, native severity, and qualifier from the observation's
        /// value bag by key.
        /// </summary>
        public JsonCondition(IObservationOutput condition)
        {
            if (condition != null)
            {
                DataItemId = condition.DataItemId;
                Timestamp = condition.Timestamp;
                Name = condition.Name;
                InstanceId = condition.InstanceId;
                Sequence = condition.Sequence;
                Type = condition.Type;
                SubType = condition.SubType;
                CompositionId = condition.CompositionId;

                // Message
                var message = condition.GetValue(ValueKeys.Message);
                if (!string.IsNullOrEmpty(message)) Value = message;

                //// Level
                //var level = condition.GetValue(ValueKeys.Level);
                //if (!string.IsNullOrEmpty(level)) Level = level;

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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IConditionObservation"/>, restoring the condition
        /// level from the parent dictionary key (passed in by the caller)
        /// and parsing the qualifier enumeration from its serialized form.
        /// </summary>
        public IConditionObservation ToCondition(ConditionLevel conditionLevel)
        {
            // Route construction through the typed factory so the runtime
            // type discriminator survives the envelope read path. A naked
            // `new ConditionObservation()` collapses any typed condition
            // subclass back to the abstract carrier.
            var condition = ConditionObservation.Create(Type, DataItemRepresentation.VALUE);
            if (condition == null) condition = new ConditionObservation();
            condition.DataItemId = DataItemId;
            condition.Timestamp = Timestamp;
            condition.Name = Name;
            condition.InstanceId = InstanceId;
            condition.Sequence = Sequence;
            //condition.Category = Category.ConvertEnum<DataItemCategory>();
            condition.Type = Type;
            condition.SubType = SubType;
            condition.CompositionId = CompositionId;
            condition.Level = conditionLevel;
            //condition.Level = Level.ConvertEnum<ConditionLevel>();
            condition.NativeCode = NativeCode;
            condition.NativeSeverity = NativeSeverity;
            condition.Qualifier = Qualifier.ConvertEnum<ConditionQualifier>();
            return condition;
        }
    }
}