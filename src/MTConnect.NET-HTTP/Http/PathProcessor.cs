// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using System.Collections.Generic;

namespace MTConnect.Http
{
    internal class PathProcessor
    {
        public static IEnumerable<string> GetDataItemIds(IMTConnectAgent mtconnectAgent, string path, string documentFormat)
        {
            if (mtconnectAgent != null && !string.IsNullOrEmpty(path))
            {
                var deviceDocument = mtconnectAgent.GetDevices();
                if (deviceDocument != null)
                {
                    return Formatters.PathFormatter.GetDataItemIds(documentFormat, deviceDocument, path);
                }
            }

            return null;
        }
    }
}
