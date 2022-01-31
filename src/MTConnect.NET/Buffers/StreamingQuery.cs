// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Buffers
{
    class StreamingQuery
    {
        public string DeviceName { get; set; }

        public IEnumerable<string> DataItemIds { get; set; }


        public StreamingQuery(string deviceName, IEnumerable<string> dataItemIds)
        {
            DeviceName = deviceName;
            DataItemIds = dataItemIds;
        }
    }
}
