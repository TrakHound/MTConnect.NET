// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Extensions;
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
        protected static readonly object _typeLock = new object();

        protected readonly object _valueLock = new object();
        protected readonly Dictionary<string, ObservationValue> _values = new Dictionary<string, ObservationValue>();


        internal string _deviceUuid;
        /// <summary>
        /// The UUID of the Device that this Observation is associated with
        /// </summary>
        public string DeviceUuid
        {
            get => _deviceUuid;
            set => _deviceUuid = value;
        }

        internal IDataItem _dataItem;
        /// <summary>
        /// The DataItem that this Observation is associated with
        /// </summary>
        public IDataItem DataItem
        {
            get => _dataItem;
            set => _dataItem = value;
        }

        internal string _uuid;
        /// <summary>
        /// The UUID of the MTConnect Entity
        /// </summary>
        public string Uuid
        {
            get
            {
                _uuid = $"{DeviceUuid}.{DataItemId}.{Timestamp.ToUnixTime()}";
                return _uuid;
            }
            set => _uuid = value;
        }

        public MTConnectEntityType EntityType => MTConnectEntityType.Observation;

        /// <summary>
        /// The unique identifier for the DataItem. 
        /// The DataItemID MUST match the id attribute of the data item defined in the Device Information Model that this DataItem element represents.
        /// </summary>
        internal string _dataItemId;
        public string DataItemId
        {
            get => _dataItemId;
            set => _dataItemId = value;
        }

        /// <summary>
        /// The time the data for the DataItem was reported or the statistics for the DataItem was computed.
        /// The timestamp MUST always represent the end of the collection interval when a duration or a TIME_SERIES is provided.
        /// The most accurate time available to the device MUST be used for the timestamp.
        /// </summary>
        internal DateTime _timestamp;
        public DateTime Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
        }

        internal string _name;
        /// <summary>
        /// The name of the DataItem.
        /// The name MUST match the name of the data item defined in the Device Information Model that this DataItem represents.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        internal ulong _instanceId;
        /// <summary>
        /// The Agent Instance ID that produced the Observation
        /// </summary>
        public ulong InstanceId
        {
            get => _instanceId;
            set => _instanceId = value;
        }

        internal ulong _sequence;
        /// <summary>
        /// A number representing the sequential position of an occurence of the DataItem in the data buffer of the Agent.
        /// The value MUST be represented as an unsigned 64 bit with valid values from 1 to 2^64-1.
        /// </summary>
        public ulong Sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        internal DataItemCategory _category;
        /// <summary>
        /// Category of DataItem (Condition, Event, or Sample)
        /// </summary>
        public DataItemCategory Category
        {
            get => _category;
            set => _category = value;
        }

        internal string _type;
        /// <summary>
        /// Type associated with the DataItem
        /// </summary>
        public virtual string Type
        {
            get => _type;
            set => _type = value;
        }

        internal string _subType;
        /// <summary>
        /// The subtype of the DataItem defined in the Device Information Model that this DataItem element represents
        /// </summary>
        public string SubType
        {
            get => _subType;
            set => _subType = value;
        }

        internal string _compositionId;
        /// <summary>
        /// The identifier of the Composition element defined in the MTConnectDevices document associated with the data reported for the DataItem.
        /// </summary>
        public string CompositionId
        {
            get => _compositionId;
            set => _compositionId = value;
        }

        internal DataItemRepresentation _representation;
        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        public DataItemRepresentation Representation
        {
            get => _representation;
            set => _representation = value;
        }

        /// <summary>
        /// Gets the Values associated with this Observation. These values represent data recorded during an Observation.
        /// </summary>
        public IEnumerable<ObservationValue> Values => _values.Values;

        /// <summary>
        /// Returns whether the Observation is Unavailable meaning a valid value cannot be determined
        /// </summary>
        public virtual bool IsUnavailable => GetValue(ValueKeys.Result) == Unavailable;


        public ValidationResult Validate()
        {
            var result = new ValidationResult(false);

            if (DataItem != null && DataItem.Device != null)
            {
                return DataItem.Validate(DataItem.Device.MTConnectVersion, this);
            }

            return result;
        }


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


        public string GetValue(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey) && !_values.IsNullOrEmpty())
            {
                try
                {
                    ObservationValue value;
                    lock (_valueLock) _values.TryGetValue(valueKey, out value);
                    return value.Value;
                }
                catch { }
            }

            return null;
        }

        public TValue GetValue<TValue>(string valueKey)
        {
            var value = GetValue(valueKey);
            if (value != null)
            {
                return (TValue)ChangeType(value, typeof(TValue));
            }

            return default;
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
                    lock (_valueLock)
                    {
                        _values.Remove(observationValue.Key);
                        _values.Add(observationValue.Key, observationValue);
                    }

                    OnValueAdded(observationValue);
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


        protected virtual void OnValueAdded(ObservationValue observationValue) { }


        public static string GetDescriptionText(DataItemCategory category, string type, string subType, string value)
        {
            switch (category)
            {
                case DataItemCategory.EVENT: return Observations.Events.EventValue.GetDescriptionText(type, subType, value);
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
                        if (!value.Value.IsNumeric() && _uppercaseValueRegex.IsMatch(value.Value))
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

        private static object ChangeType(object obj, Type type)
        {
            if (obj != null)
            {
                try
                {
                    if (type == typeof(DateTime))
                    {
                        return obj.ToString().ToDateTime();
                    }
                    else if (typeof(Enum).IsAssignableFrom(type) && obj.GetType() == typeof(string))
                    {
                        return Enum.Parse(type, (string)obj);
                    }
                    else
                    {
                        return Convert.ChangeType(obj, type);
                    }
                }
                catch { }
            }

            return default;
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