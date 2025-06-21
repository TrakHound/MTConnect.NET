// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using System;
using System.Linq;

namespace MTConnect.Observations.Output
{
    /// <summary>
    /// An Information Model Input that describes Streaming Data reported by a piece of equipment.
    /// </summary>
    public struct ObservationOutput : IObservationOutput
    {
        internal string _deviceUuid;
        public string DeviceUuid
        {
            get => _deviceUuid;
            set => _deviceUuid = value;
        }

        internal IDataItem _dataItem;
        public IDataItem DataItem
        {
            get => _dataItem;
            set => _dataItem = value;
        }

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

        internal DateTimeOffset _timeZoneTimestamp;
        public DateTimeOffset TimeZoneTimestamp
        {
            get => _timeZoneTimestamp;
            set => _timeZoneTimestamp = value;
        }

        /// <summary>
        /// The name of the DataItem.
        /// The name MUST match the name of the data item defined in the Device Information Model that this DataItem represents.
        /// </summary>
        internal string _name;
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// The Agent Instance ID that produced the Observation
        /// </summary>
        internal ulong _instanceId;
        public ulong InstanceId
        {
            get => _instanceId;
            set => _instanceId = value;
        }

        /// <summary>
        /// A number representing the sequential position of an occurence of the DataItem in the data buffer of the Agent.
        /// The value MUST be represented as an unsigned 64 bit with valid values from 1 to 2^64-1.
        /// </summary>
        internal ulong _sequence;
        public ulong Sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        /// <summary>
        /// Category of DataItem (Condition, Event, or Sample)
        /// </summary>
        internal DataItemCategory _category;
        public DataItemCategory Category
        {
            get => _category;
            set => _category = value;
        }

        /// <summary>
        /// Type associated with the DataItem
        /// </summary>
        internal string _type;
        public string Type
        {
            get => _type;
            set => _type = value;
        }

        /// <summary>
        /// The subtype of the DataItem defined in the Device Information Model that this DataItem element represents
        /// </summary>
        internal string _subType;
        public string SubType
        {
            get => _subType;
            set => _subType = value;
        }

        /// <summary>
        /// The identifier of the Composition element defined in the MTConnectDevices document associated with the data reported for the DataItem.
        /// </summary>
        internal string _compositionId;
        public string CompositionId
        {
            get => _compositionId;
            set => _compositionId = value;
        }

        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        internal DataItemRepresentation _representation;
        public DataItemRepresentation Representation
        {
            get => _representation;
            set => _representation = value;
        }

        /// <summary>
        /// Indicates if the Observation is verifiable and is in accordance with the normative definitions within the MTConnect Standard.
        /// </summary>
        internal Quality _quality = Quality.UNVERIFIABLE;
        public Quality Quality
        {
            get => _quality;
            set => _quality = value;
        }

        /// <summary>
        /// Indicates if the Observation has any property or controlled vocabulary that has been deprecated in the MTConnect Standard.
        /// </summary>
        internal bool _deprecated;
        public bool Deprecated
        {
            get => _deprecated;
            set => _deprecated = value;
        }

        /// <summary>
        /// Indicates if the Observation has any property or controlled vocabulary that has been extended and cannot be validated.
        /// </summary>
        internal bool _extended;
        public bool Extended
        {
            get => _extended;
            set => _extended = value;
        }

        /// <summary>
        /// Gets the Values associated with this Observation. These values represent data recorded during an Observation.
        /// </summary>
        internal ObservationValue[] _values;
        public ObservationValue[] Values
        {
            get => _values;
            set => _values = value;
        }


        public ObservationOutput(IObservation observation)
        {
            _deviceUuid = null;
            _dataItem = null;
            _dataItemId = null;
            _timestamp = DateTime.MinValue;
            _timeZoneTimestamp = DateTimeOffset.MinValue;
            _name = null;
            _instanceId = 0;
            _sequence = 0;
            _category = DataItemCategory.CONDITION;
            _type = null;
            _subType = null;
            _compositionId = null;
            _representation = DataItemRepresentation.VALUE;
            _quality = Quality.UNVERIFIABLE;
            _deprecated = false;
            _extended = false;
            _values = null;

            if (observation != null)
            {
                _deviceUuid = observation.DeviceUuid;
                _dataItem = observation.DataItem;
                _dataItemId = observation.DataItemId;
                _timestamp = observation.Timestamp;
                _timeZoneTimestamp = observation.Timestamp;
                _name = observation.Name;
                _instanceId = observation.InstanceId;
                _sequence = observation.Sequence;
                _category = observation.Category;
                _type = observation.Type;
                _subType = observation.SubType;
                _compositionId = observation.CompositionId;
                _representation = observation.Representation;
                _quality = observation.Quality;
                _deprecated = observation.Deprecated;
                _extended = observation.Extended;

                if (observation.Values != null)
                {
                    _values = observation.Values.ToArray();
                }
            }
        }


        public string GetValue(string valueKey)
        {
            if (valueKey != null && _values != null && _values.Length > 0)
            {
                for (var i = 0; i < _values.Length; i++)
                {
                    if (_values[i]._key == valueKey)
                    {
                        return _values[i]._value;
                    }
                }
            }

            return null;
        }
    }
}