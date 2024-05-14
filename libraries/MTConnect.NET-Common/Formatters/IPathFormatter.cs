// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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