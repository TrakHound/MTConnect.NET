// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model Input that describes Streaming Data reported by a piece of equipment.
    /// </summary>
    public class Observation : IObservation
    {
        /// <summary>
        /// If an Agent cannot determine a Valid Data Value for a DataItem, the value returned for the Result for the Data Entity MUST be reported as UNAVAILABLE.
        /// </summary>
        public const string Unavailable = "UNAVAILABLE";
        public const string UnavailableDescription = "If an Agent cannot determine a Valid Data Value for a DataItem, the value returned for the Result for the Data Entity MUST be reported as UNAVAILABLE.";
        private const string UppercaseValuePattern = "^[a-zA-Z_]*$";

        private static readonly Regex _uppercaseValueRegex = new Regex(UppercaseValuePattern);
        protected static readonly Dictionary<string, string> _typeIds = new Dictionary<string, string>();

        protected static Dictionary<string, Type> _types;

        private readonly object _lock = new object();
        protected readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        protected readonly Dictionary<string, ObservationValue> _values = new Dictionary<string, ObservationValue>();


        public string DeviceUuid => GetProperty<string>(nameof(DeviceUuid));

        public IDataItem DataItem { get; set; }


        /// <summary>
        /// The unique identifier for the DataItem. 
        /// The DataItemID MUST match the id attribute of the data item defined in the Device Information Model that this DataItem element represents.
        /// </summary>
        public string DataItemId => GetProperty<string>(nameof(DataItemId));

        /// <summary>
        /// The time the data for the DataItem was reported or the statistics for the DataItem was computed.
        /// The timestamp MUST always represent the end of the collection interval when a duration or a TIME_SERIES is provided.
        /// The most accurate time available to the device MUST be used for the timestamp.
        /// </summary>
        public DateTime Timestamp => GetProperty<DateTime>(nameof(Timestamp));

        /// <summary>
        /// The name of the DataItem.
        /// The name MUST match the name of the data item defined in the Device Information Model that this DataItem represents.
        /// </summary>
        public string Name => GetProperty<string>(nameof(Name));

        /// <summary>
        /// A number representing the sequential position of an occurence of the DataItem in the data buffer of the Agent.
        /// The value MUST be represented as an unsigned 64 bit with valid values from 1 to 2^64-1.
        /// </summary>
        public long Sequence => GetProperty<long>(nameof(Sequence));

        /// <summary>
        /// Category of DataItem (Condition, Event, or Sample)
        /// </summary>
        public DataItemCategory Category => GetProperty<DataItemCategory>(nameof(Category));

        /// <summary>
        /// Type associated with the DataItem
        /// </summary>
        public virtual string Type => GetProperty<string>(nameof(Type));

        /// <summary>
        /// The subtype of the DataItem defined in the Device Information Model that this DataItem element represents
        /// </summary>
        public string SubType => GetProperty<string>(nameof(SubType));

        /// <summary>
        /// The identifier of the Composition element defined in the MTConnectDevices document associated with the data reported for the DataItem.
        /// </summary>
        public string CompositionId => GetProperty<string>(nameof(CompositionId));

        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        public DataItemRepresentation Representation => GetProperty<DataItemRepresentation>(nameof(Representation));

        /// <summary>
        /// Gets the Properties associated with this Observation
        /// </summary>
        public Dictionary<string, object> Properties => _properties;

        /// <summary>
        /// Gets the Values associated with this Observation. These values represent data recorded during an Observation.
        /// </summary>
        public IEnumerable<ObservationValue> Values => _values.Values;

        /// <summary>
        /// Returns whether the Observation is Unavailable meaning a valid value cannot be determined
        /// </summary>
        public bool IsUnavailable => GetValue(ValueKeys.Result) == Unavailable;


        public static Observation Create(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                switch (dataItem.Category)
                {
                    case DataItemCategory.SAMPLE: return SampleObservation.Create(dataItem);
                    case DataItemCategory.EVENT: return EventObservation.Create(dataItem);
                    case DataItemCategory.CONDITION: return ConditionObservation.Create(dataItem);
                }
            }

            return new Observation();
        }


        public T GetProperty<T>(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                object value = null;
                lock (_lock) _properties.TryGetValue(propertyName, out value);
                if (value != null)
                {
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch { }
                }
            }

            return default;
        }

        public void SetProperty(string propertyName, object value)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                try
                {
                    lock (_lock)
                    {
                        _properties.Remove(propertyName);
                        _properties[propertyName] = value;
                    }
                }
                catch { }
            }
        }


        public string GetValue(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey) && !_values.IsNullOrEmpty())
            {
                try
                {
                    ObservationValue value;
                    lock (_lock) _values.TryGetValue(valueKey, out value);
                    return value.Value;
                }
                catch { }
            }

            return null;
        }

        public void AddValue(string valueKey, object value)
        {
            if (!string.IsNullOrEmpty(valueKey) && value != null)
            {
                AddValue(new ObservationValue(valueKey, value));
            }
        }

        public void AddValue(ObservationValue observationValue)
        {
            if (!string.IsNullOrEmpty(observationValue.Key))
            {
                try
                {
                    lock (_lock)
                    {
                        _values.Remove(observationValue.Key);
                        _values.Add(observationValue.Key, observationValue);
                    }
                }
                catch { }
            }
        }

        public void AddValues(IEnumerable<ObservationValue> observationValues)
        {
            if (!observationValues.IsNullOrEmpty())
            {
                foreach (var observationValue in observationValues)
                {
                    AddValue(observationValue);
                }
            }
        }


        public static string GetDescriptionText(DataItemCategory category, string type, string subType, string value)
        {
            switch (category)
            {
                case DataItemCategory.EVENT: return Events.Values.EventValue.GetDescriptionText(type, subType, value);
            }

            return null;
        }


        public static IEnumerable<ObservationValue> ProcessValues(IEnumerable<ObservationValue> values)
        {
            var valuesList = new List<ObservationValue>();

            if (!values.IsNullOrEmpty())
            {
                foreach (var value in values)
                {
                    valuesList.Add(value);
                }
            }

            return valuesList;
        }

        public static IEnumerable<ObservationValue> UppercaseValues(IEnumerable<ObservationValue> values)
        {
            if (!values.IsNullOrEmpty())
            {
                var x = new List<ObservationValue>();

                foreach (var value in values)
                {
                    if (value.Value != null)
                    {
                        if (_uppercaseValueRegex.IsMatch(value.Value))
                        {
                            var v = value.Value.ToUpper();
                            x.Add(new ObservationValue(value.Key, v));
                        }
                        else
                        {
                            x.Add(value);
                        }
                    }
                }

                return x;
            }

            return null;
        }


        protected static Dictionary<string, Type> GetAllTypes()
        {
            var assemblies = Assemblies.Get();
            if (!assemblies.IsNullOrEmpty())
            {
                var types = assemblies
                    .SelectMany(
                        x => x.GetMatchingTypesInAssembly(
                            t => typeof(IObservation).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));

                if (!types.IsNullOrEmpty())
                {
                    var objs = new Dictionary<string, Type>();
                    var regex = new Regex("(.*)Observation");

                    foreach (var type in types)
                    {
                        var match = regex.Match(type.Name);
                        if (match.Success && match.Groups.Count > 1)
                        {
                            string key = null;

                            if (match.Groups[1].Success) key = match.Groups[1].Value;
                            else if (match.Groups[2].Success) key = match.Groups[2].Value;

                            if (!string.IsNullOrEmpty(key))
                            {
                                if (!objs.ContainsKey(key)) objs.Add(key, type);
                            }
                        }
                    }

                    return objs;
                }
            }

            return new Dictionary<string, Type>();
        }
    }
}
