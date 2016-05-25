// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MTConnect.Streams
{
    /// <summary>
    /// Object class to return all data associated with Current command results
    /// </summary>
    public class ReturnData : IDisposable
    {
        // Device object with heirarchy of values and xml structure
        public List<DeviceStream> DeviceStreams { get; set; }

        // Header Information
        public Headers.Streams Header { get; set; }

        public void Dispose()
        {
            DeviceStreams.Clear();
            DeviceStreams = null;
        }
    }
}
