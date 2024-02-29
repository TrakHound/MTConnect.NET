// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Buffers
{
    public struct BufferObservation
    {
        internal int _key;
        internal ObservationValue[] _values;
        internal ulong _sequence;
        internal long _timestamp;
        internal byte _category;
        internal byte _representation;

        public int Key
        {
            get => _key;
            set => _key = value;
        }

        public int DeviceIndex => GetDeviceIndexFromBufferKey(_key);

        public int DataItemIndex => GetDataItemIndexFromBufferKey(_key);

        public ObservationValue[] Values
        {
            get => _values;
            set => _values = value;
        }

        public ulong Sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        public long Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
        }

        public DataItemCategory Category
        {
            get
            {
                switch (_category)
                {
                    case 1: return DataItemCategory.EVENT;
                    case 2: return DataItemCategory.SAMPLE;
                    default: return DataItemCategory.CONDITION;
                }
            }
            set
            {
                _category = (byte)value;
            }
        }

        public DataItemRepresentation Representation
        {
            get
            {
                switch (_representation)
                {
                    case 1: return DataItemRepresentation.DATA_SET;
                    case 2: return DataItemRepresentation.DISCRETE;
                    case 3: return DataItemRepresentation.TIME_SERIES;
                    case 4: return DataItemRepresentation.TABLE;
                    default: return DataItemRepresentation.VALUE;
                }
            }
            set
            {
                _representation = (byte)value;
            }
        }


        public bool IsValid =>
            Key >= 0 &&
            Values != null && Values.Length > 0 &&
            Sequence > 0 && 
            Timestamp > 0;



        public BufferObservation(int bufferKey, IObservation observation)
        {
            _key = bufferKey;
            _sequence = 0;
            _timestamp = observation.Timestamp.ToUnixTime();
            _category = (byte)observation.Category;
            _representation = (byte)observation.Representation;

            // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
            if (observation.Values != null)
            {
                _values = observation.Values.OrderBy(o => o._key).ToArray();
            }
            else _values = null;
        }

        public BufferObservation(int bufferKey, ulong sequence, IObservation observation)
        {
            _key = bufferKey;
            _sequence = sequence;
            _timestamp = observation.Timestamp.ToUnixTime();
            _category = (byte)observation.Category;
            _representation = (byte)observation.Representation;

            // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
            if (observation.Values != null)
            {
                _values = observation.Values.OrderBy(o => o._key).ToArray();
            }
            else _values = null;
        }

        public BufferObservation(
            int bufferKey,
            DataItemCategory category,
            DataItemRepresentation representation, 
            IEnumerable<ObservationValue> values,
            ulong sequence,
            long timestamp
            )
        {
            _key = bufferKey;
            _category = (byte)category;
            _representation = (byte)representation;
            _sequence = sequence;
            _timestamp = timestamp;

            // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
            if (values != null)
            {
                _values = values.OrderBy(o => o._key).ToArray();
            }
            else _values = null;
        }


        public ulong CreateHash() => CreateHash(Key, Sequence);

        public static ulong CreateHash(int key, ulong sequence)
        {
            // [DeviceIndex] [DataItemIndex] [Sequence]
            // {3} {4} {12}
            // 
            // 922 3372 036854775807
            // 000 0000 000000000000
            // 001 0001 000000000001
            // 
            // 030 0161 000000000000

            // Max DeviceIndex can be 999
            // Max DataItemIndex can be 9999
            // Max Sequence can be 999,999,999,999

            return (ulong)(key * 1000000000000) + sequence;
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


        public static int GetDeviceIndexFromBufferKey(int bufferKey)
        {
            return bufferKey / 10000;
        }

        public static int GetDataItemIndexFromBufferKey(int bufferKey)
        {
            return bufferKey % 10000;
        }
    }
}