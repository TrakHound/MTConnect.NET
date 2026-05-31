// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a CONDITION-category observation,
    /// extending <see cref="JsonObservation"/> with the condition-specific
    /// properties. Converts to and from the strongly-typed
    /// <see cref="ConditionObservation"/> model.
    /// </summary>
    public class JsonCondition : JsonObservation
    {
        /// <summary>
        /// The condition level (NORMAL, WARNING, FAULT, or UNAVAILABLE).
        /// </summary>
        [JsonPropertyName("level")]
        public string Level { get; set; }

        /// <summary>
        /// The severity of the condition as reported by the native data
        /// source.
        /// </summary>
        [JsonPropertyName("nativeSeverity")]
        public string NativeSeverity { get; set; }

        /// <summary>
        /// The qualifier (for example HIGH or LOW) further classifying the
        /// condition.
        /// </summary>
        [JsonPropertyName("qualifier")]
        public string Qualifier { get; set; }

        /// <summary>
        /// The statistical operation associated with the condition, when
        /// reported.
        /// </summary>
        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCondition() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed condition
        /// <see cref="IObservation"/>, mapping the condition message to
        /// <see cref="JsonObservation.Result"/> and optionally emitting the
        /// category and instance id.
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
                if (!string.IsNullOrEmpty(condition.Message)) Result = condition.Message;

                Level = condition.Level.ToString();
                NativeCode = condition.NativeCode;
                NativeSeverity = condition.NativeSeverity;
                if (condition.Qualifier != ConditionQualifier.NOT_SPECIFIED) Qualifier = condition.Qualifier.ToString();
            }
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IObservationOutput"/>, reading the condition message,
        /// level, native code, native severity, and qualifier from the
        /// observation's values.
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IConditionObservation"/>, parsing the category, level,
        /// and qualifier enumerations.
        /// </summary>
        public IConditionObservation ToCondition()
        {
            var condition = new ConditionObservation();
            condition.DataItemId = DataItemId;
            condition.Timestamp = Timestamp;
            condition.Name = Name;
            condition.InstanceId = InstanceId;
            condition.Sequence = Sequence;
            condition.Category = Category.ConvertEnum<DataItemCategory>();
            condition.Type = Type;
            condition.SubType = SubType;
            condition.CompositionId = CompositionId;
            condition.Level = Level.ConvertEnum<ConditionLevel>();
            condition.NativeCode = NativeCode;
            condition.NativeSeverity = NativeSeverity;
            condition.Qualifier = Qualifier.ConvertEnum<ConditionQualifier>();
            return condition;
        }
    }
}