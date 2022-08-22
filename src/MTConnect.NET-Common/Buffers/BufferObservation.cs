// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using System.Linq;

namespace MTConnect.Buffers
{
    public struct BufferObservation
    {
        internal int _key;
        internal ObservationValue[] _values;
        internal long _sequence;
        internal long _timestamp;
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

        public long Sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        public long Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
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
            !Values.IsNullOrEmpty() &&
            Sequence > 0 &&
            Timestamp > 0;



        public BufferObservation(int bufferKey, long sequence, IObservation observation)
        {
            _key = bufferKey;
            _sequence = sequence;
            _timestamp = observation.Timestamp.ToUnixTime();
            _representation = (byte)observation.Representation;

            // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
            if (observation.Values != null)
            {
                _values = observation.Values.OrderBy(o => o._key).ToArray();
            }
            else _values = null;
        }


        public long CreateHash() => CreateHash(Key, Sequence);

        public static long CreateHash(int key, long sequence)
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

            return (key * 1000000000000) + sequence;
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
