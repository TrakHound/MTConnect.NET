// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;

namespace MTConnect.Input
{
    /// <summary>
    /// An Information Model that describes Condition Streaming Data reported by a piece of equipment.
    /// </summary>
    public class ConditionFaultStateObservationInput : ObservationInput, IConditionFaultStateObservationInput
    {
        /// <summary>
        /// Level of the Condition (Normal, Warning, Fault, or Unavailable)
        /// </summary>
        public ConditionLevel Level
        {
            get => GetValue(ValueKeys.Level).ConvertEnum<ConditionLevel>();
            set => AddValue(new ObservationValue(ValueKeys.Level, value));
        }

        /// <summary>
        /// Identifier of an individual condition activation provided by a piece of equipment.
        /// </summary>
        public string ConditionId
        {
            get => GetValue(ValueKeys.ConditionId);
            set
            {
                var val = !string.IsNullOrEmpty(value) ? value : null;
                AddValue(new ObservationValue(ValueKeys.ConditionId, val));
            }
        }

        /// <summary>
        /// The native code (usually an alpha-numeric value) generated by the controller of a piece of equipment providing a reference identifier for a condition state or alarm.
        /// This is the same information an operator or maintenance personnel would see as a reference code designating a specific type of Condition when viewed at the piece of equipment.Usually this reference code is used to point to a more detailed description of the Condition.
        /// </summary>
        public string NativeCode
        {
            get => GetValue(ValueKeys.NativeCode);
            set
            {
                var val = !string.IsNullOrEmpty(value) ? value : null;
                AddValue(new ObservationValue(ValueKeys.NativeCode, val));
            }
        }

        /// <summary>
        /// If the data source assigns a severity level to a Condition, nativeSeverity is used to report that severity information to a client software application.
        /// </summary>
        public string NativeSeverity
        {
            get => GetValue(ValueKeys.NativeSeverity);
            set
            {
                var val = !string.IsNullOrEmpty(value) ? value : null;
                AddValue(new ObservationValue(ValueKeys.NativeSeverity, val));
            }
        }

        /// <summary>
        /// Qualifies the Condition and adds context or additional clarification.
        /// This optional attribute can be used to convey information such as HIGH or LOW type Warning and Fault condition to indicate differing types of condition states
        /// </summary>
        public ConditionQualifier Qualifier
        {
            get => GetValue(ValueKeys.Qualifier).ConvertEnum<ConditionQualifier>();
            set
            {
                if (value != ConditionQualifier.NOT_SPECIFIED)
                {
                    AddValue(new ObservationValue(ValueKeys.Qualifier, value));
                }
            }
        }

        /// <summary>
        /// The Message of the Condition Observation
        /// </summary>
        public string Message
        {
            get => GetValue(ValueKeys.Message);
            set
            {
                var val = !string.IsNullOrEmpty(value) ? value : null;
                AddValue(new ObservationValue(ValueKeys.Message, val));
            }
        }


        public ConditionFaultStateObservationInput() { }

        public ConditionFaultStateObservationInput(string dataItemKey)
        {
            DataItemKey = dataItemKey;
        }

        public ConditionFaultStateObservationInput(string dataItemKey, ConditionLevel level)
        {
            DataItemKey = dataItemKey;
            Level = level;
        }

        public ConditionFaultStateObservationInput(string dataItemKey, ConditionLevel level, long timestamp)
        {
            DataItemKey = dataItemKey;
            Level = level;
            Timestamp = timestamp;
        }

        public ConditionFaultStateObservationInput(string dataItemKey, ConditionLevel level, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Level = level;
            Timestamp = timestamp.ToUnixTime();
        }

        public ConditionFaultStateObservationInput(IObservationInput observation)
        {
            if (observation != null)
            {
                DeviceKey = observation.DeviceKey;
                DataItemKey = observation.DataItemKey;
                Timestamp = observation.Timestamp;
                Values = observation.Values;
            }
        }
    }
}