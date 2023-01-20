// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using System.Collections.Generic;

namespace MTConnect.Servers.Http
{
    public static class PathProcessor
    {
        public static IEnumerable<string> GetDataItemIds(IMTConnectAgentBroker mtconnectAgent, string path, string documentFormat)
        {
            if (mtconnectAgent != null && !string.IsNullOrEmpty(path))
            {
                var deviceDocument = mtconnectAgent.GetDevicesResponseDocument();
                if (deviceDocument != null)
                {
                    return Formatters.PathFormatter.GetDataItemIds(documentFormat, deviceDocument, path);
                }
            }

            return null;
        }
    }
}