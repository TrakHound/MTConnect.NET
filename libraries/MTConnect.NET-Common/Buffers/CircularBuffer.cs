// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Buffers
{
    class CircularBuffer
    {
        private readonly object _lock = new object();
        private readonly BufferObservation[] _buffer;
        private int _start;
        private int _end;
        private int _size;
        private bool _full;


        public uint Capacity
        {
            get 
            {
                lock (_lock) return (uint)_buffer.Length;
            }
        }

        public int Size
        {
            get
            {
                lock (_lock) return _size;
            }
        }


        public CircularBuffer(uint capacity)
        {
            _buffer = new BufferObservation[capacity];
            _start = 0;
            _end = 0;
        }


        public BufferObservation this[int index]
        {
            get
            {
                lock (_lock)
                {
                    if (_size == 0)
                    {
                        throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
                    }
                    if (index >= _size)
                    {
                        throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, _size));
                    }
                    int actualIndex = _start + (index < (_buffer.Length - _start) ? index : index - _buffer.Length);
                    return _buffer[actualIndex];
                }
            }
            set
            {
                lock (_lock)
                {
                    if (_size == 0)
                    {
                        throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
                    }
                    if (index >= _size)
                    {
                        throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, _size));
                    }
                    int actualIndex = _start + (index < (_buffer.Length - _start) ? index : index - _buffer.Length);
                    _buffer[actualIndex] = value;
                }
            }
        }

        public void Add(ref BufferObservation observation)
        {
            lock (_lock)
            {
                if (_full)
                {
                    _buffer[_end] = observation;
                    if (++_end == _buffer.Length) _end = 0;
                    _start = _end;
                }
                else
                {
                    _buffer[_end] = observation;
                    if (++_end == _buffer.Length) _end = 0;
                    ++_size;
                    _full = _size == _buffer.Length;
                }
            }     
        }
    }
}