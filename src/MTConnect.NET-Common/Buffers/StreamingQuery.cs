// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Buffers
{
    struct StreamingQuery
    {
        public string DeviceUuid { get; set; }

        public IEnumerable<string> DataItemIds { get; set; }


        public StreamingQuery(string deviceUuid, IEnumerable<string> dataItemIds)
        {
            DeviceUuid = deviceUuid;
            DataItemIds = dataItemIds;
        }
    }
}
