// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Buffers
{
    /// <summary>
    /// A compact value-type representation of an observation as stored inside an MTConnect buffer, packing the device/data-item identity, values, sequence, timestamp, and metadata into byte-coded fields for memory efficiency.
    /// </summary>
    public struct BufferObservation
    {
        internal int _key;
        internal ObservationValue[] _values;
        internal ulong _sequence;
        internal long _timestamp;
        internal byte _category;
        internal byte _representation;
        internal byte _quality;
        internal bool _deprecated;
        internal bool _extended;

        /// <summary>
        /// The packed buffer key encoding both the device and data-item slot indices for this observation.
        /// </summary>
        public int Key
        {
            get => _key;
            set => _key = value;
        }

        /// <summary>
        /// The device slot index decoded from <see cref="Key"/>.
        /// </summary>
        public int DeviceIndex => GetDeviceIndexFromBufferKey(_key);

        /// <summary>
        /// The data-item slot index decoded from <see cref="Key"/>.
        /// </summary>
        public int DataItemIndex => GetDataItemIndexFromBufferKey(_key);

        /// <summary>
        /// The observation's value components, kept sorted by value key.
        /// </summary>
        public ObservationValue[] Values
        {
            get => _values;
            set => _values = value;
        }

        /// <summary>
        /// The buffer sequence number assigned to this observation.
        /// </summary>
        public ulong Sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        /// <summary>
        /// The observation timestamp expressed as Unix time.
        /// </summary>
        public long Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
        }

        /// <summary>
        /// The data-item category, decoded from and stored as a single byte.
        /// </summary>
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

        /// <summary>
        /// The data-item representation, decoded from and stored as a single byte.
        /// </summary>
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

        /// <summary>
        /// The observation quality, decoded from and stored as a single byte.
        /// </summary>
        public Quality Quality
        {
            get
            {
                switch (_quality)
                {
                    case 0: return Quality.INVALID;
                    case 2: return Quality.VALID;
                    default: return Quality.UNVERIFIABLE;
                }
            }
            set
            {
                _quality = (byte)value;
            }
        }

        /// <summary>
        /// Indicates whether the originating data item has been deprecated.
        /// </summary>
        public bool Deprecated
        {
            get => _deprecated;
            set => _deprecated = value;
        }

        /// <summary>
        /// Indicates whether the observation carries extended (vendor or non-standard) data.
        /// </summary>
        public bool Extended
        {
            get => _extended;
            set => _extended = value;
        }


        /// <summary>
        /// Indicates whether the entry is well-formed: a non-negative key, at least one value, and positive sequence and timestamp.
        /// </summary>
        public bool IsValid =>
            Key >= 0 &&
            Values != null && Values.Length > 0 &&
            Sequence > 0 &&
            Timestamp > 0;



        /// <summary>
        /// Captures an observation under the given buffer key with no sequence yet assigned; values are copied and sorted by value key.
        /// </summary>
        /// <param name="bufferKey">The packed device/data-item buffer key.</param>
        /// <param name="observation">The source observation to capture.</param>
        public BufferObservation(int bufferKey, IObservation observation)
        {
            _key = bufferKey;
            _sequence = 0;
            _timestamp = observation.Timestamp.ToUnixTime();
            _category = (byte)observation.Category;
            _representation = (byte)observation.Representation;
            _quality = (byte)observation.Quality;
            _deprecated = observation.Deprecated;
            _extended = observation.Extended;

            // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
            if (observation.Values != null)
            {
                _values = observation.Values.OrderBy(o => o._key).ToArray();
            }
            else _values = null;
        }

        /// <summary>
        /// Captures an observation under the given buffer key with an explicit sequence number; values are copied and sorted by value key.
        /// </summary>
        /// <param name="bufferKey">The packed device/data-item buffer key.</param>
        /// <param name="sequence">The buffer sequence number to assign.</param>
        /// <param name="observation">The source observation to capture.</param>
        public BufferObservation(int bufferKey, ulong sequence, IObservation observation)
        {
            _key = bufferKey;
            _sequence = sequence;
            _timestamp = observation.Timestamp.ToUnixTime();
            _category = (byte)observation.Category;
            _representation = (byte)observation.Representation;
            _quality = (byte)observation.Quality;
            _deprecated = observation.Deprecated;
            _extended = observation.Extended;

            // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
            if (observation.Values != null)
            {
                _values = observation.Values.OrderBy(o => o._key).ToArray();
            }
            else _values = null;
        }

        /// <summary>
        /// Constructs a buffer observation directly from its constituent fields, without an originating <see cref="IObservation"/>; values are copied and sorted by value key.
        /// </summary>
        /// <param name="bufferKey">The packed device/data-item buffer key.</param>
        /// <param name="category">The data-item category.</param>
        /// <param name="representation">The data-item representation.</param>
        /// <param name="values">The observation value components.</param>
        /// <param name="sequence">The buffer sequence number.</param>
        /// <param name="timestamp">The observation timestamp as Unix time.</param>
        /// <param name="quality">The observation quality.</param>
        /// <param name="deprecated">Whether the originating data item is deprecated.</param>
        /// <param name="extended">Whether the observation carries extended data.</param>
        public BufferObservation(
            int bufferKey,
            DataItemCategory category,
            DataItemRepresentation representation,
            IEnumerable<ObservationValue> values,
            ulong sequence,
            long timestamp,
            Quality quality,
            bool deprecated,
            bool extended
            )
        {
            _key = bufferKey;
            _category = (byte)category;
            _representation = (byte)representation;
            _sequence = sequence;
            _timestamp = timestamp;
            _quality = (byte)quality;
            _deprecated = deprecated;
            _extended = extended;

            // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
            if (values != null)
            {
                _values = values.OrderBy(o => o._key).ToArray();
            }
            else _values = null;
        }


        /// <summary>
        /// Computes a stable hash combining this observation's key and sequence, used to deduplicate and index entries.
        /// </summary>
        public ulong CreateHash() => CreateHash(Key, Sequence);

        /// <summary>
        /// Computes the combined key/sequence hash for an arbitrary key and sequence by interleaving the buffer key with the 12-digit sequence space.
        /// </summary>
        /// <param name="key">The packed buffer key.</param>
        /// <param name="sequence">The buffer sequence number.</param>
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


        /// <summary>
        /// Returns the string value of the value component matching the given value key, or null when no such component exists.
        /// </summary>
        /// <param name="valueKey">The value-component key to look up.</param>
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


        /// <summary>
        /// Extracts the device slot index from a packed buffer key.
        /// </summary>
        /// <param name="bufferKey">The packed buffer key.</param>
        public static int GetDeviceIndexFromBufferKey(int bufferKey)
        {
            return bufferKey / 10000;
        }

        /// <summary>
        /// Extracts the data-item slot index from a packed buffer key.
        /// </summary>
        /// <param name="bufferKey">The packed buffer key.</param>
        public static int GetDataItemIndexFromBufferKey(int bufferKey)
        {
            return bufferKey % 10000;
        }
    }
}