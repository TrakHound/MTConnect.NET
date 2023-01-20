// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public interface IPathFormatter
    {
        string Id { get; }


        IEnumerable<string> GetDataItemIds(IDevicesResponseDocument devicesDocument, string path);
    }
}
