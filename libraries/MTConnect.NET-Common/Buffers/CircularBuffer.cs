// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Fixed-capacity ring of <see cref="BufferObservation"/> entries that overwrites the oldest entry once full.
    /// Logical index 0 is always the oldest stored observation and <c>Size - 1</c> the newest.
    /// </summary>
    /// <remarks>
    /// Not thread-safe. <see cref="MTConnectObservationBuffer"/> serializes every access under its own lock,
    /// which lets a request scan the whole archive without taking a lock per element.
    /// </remarks>
    sealed class CircularBuffer
    {
        private readonly BufferObservation[] _buffer;
        private readonly int[] _keys; // BufferKey of each physical slot (lets readers copy keys without the observations)
        private long _evicted; // Number of observations overwritten so far
        private int _start; // Physical slot of the oldest observation
        private int _size;


        /// <summary>
        /// The maximum number of observations the buffer can hold.
        /// </summary>
        public int Capacity => _buffer.Length;

        /// <summary>
        /// The number of observations currently stored.
        /// </summary>
        public int Size => _size;


        /// <summary>
        /// Initializes an empty buffer.
        /// </summary>
        /// <param name="capacity">The maximum number of observations to hold; must be between 1 and <see cref="int.MaxValue"/>.</param>
        public CircularBuffer(uint capacity)
        {
            if (capacity == 0 || capacity > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Observation buffer capacity must be between 1 and " + int.MaxValue);
            }

            _buffer = new BufferObservation[capacity];
            _keys = new int[capacity];
        }


        /// <summary>
        /// Returns a read-only reference to the observation at the given logical index (0 = oldest) without copying it.
        /// </summary>
        public ref readonly BufferObservation this[int index]
        {
            get
            {
                // The unsigned compare also rejects negative indexes
                if ((uint)index >= (uint)_size)
                {
                    if (_size == 0) throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, _size));
                }

                var actualIndex = _start + index;
                if (actualIndex >= _buffer.Length) actualIndex -= _buffer.Length;
                return ref _buffer[actualIndex];
            }
        }

        /// <summary>
        /// Appends an observation, overwriting the oldest one when the buffer is full.
        /// </summary>
        /// <summary>
        /// Number of observations overwritten so far. The logical index of an entry decreases by one for each eviction.
        /// </summary>
        public long Evicted => _evicted;

        /// <summary>
        /// Copies the BufferKeys of <paramref name="count"/> entries starting at logical <paramref name="index"/>.
        /// </summary>
        public void CopyKeys(int index, int[] destination, int count)
        {
            if (index < 0 || count < 0 || index + count > _size) throw new ArgumentOutOfRangeException(nameof(index));

            var physical = _start + index;
            if (physical >= _buffer.Length) physical -= _buffer.Length;

            var first = Math.Min(count, _buffer.Length - physical);
            Array.Copy(_keys, physical, destination, 0, first);
            if (count > first) Array.Copy(_keys, 0, destination, first, count - first);
        }

        public void Add(ref BufferObservation observation)
        {
            var end = _start + _size;
            if (end >= _buffer.Length) end -= _buffer.Length;

            _buffer[end] = observation;
            _keys[end] = observation._key;

            if (_size == _buffer.Length)
            {
                _evicted++;

                // Full: the slot just written held the oldest entry, so the oldest moves forward
                if (++_start == _buffer.Length) _start = 0;
            }
            else
            {
                _size++;
            }
        }
    }
}
