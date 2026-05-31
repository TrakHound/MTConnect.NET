// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using System.Collections.Generic;

namespace MTConnect.Servers.Http
{
    /// <summary>
    /// Helper that resolves an MTConnect XPath/JSONPath request <c>path</c> argument against the
    /// currently configured device model and returns the set of DataItem identifiers that match.
    /// The MTConnect HTTP server uses this when building <c>sample</c>, <c>current</c>, or
    /// <c>asset</c> responses to narrow the observations to those addressed by the request path.
    /// </summary>
    public static class PathProcessor
    {
        /// <summary>
        /// Resolves <paramref name="path"/> against the device model exposed by
        /// <paramref name="mtconnectAgent"/> and returns the matching DataItem identifiers, using
        /// the document-format-specific path syntax (XPath for XML, JSONPath for JSON) selected by
        /// <paramref name="documentFormat"/>.
        /// </summary>
        /// <param name="mtconnectAgent">The agent broker whose <c>MTConnectDevices</c> document is queried; null returns null.</param>
        /// <param name="path">The path expression supplied by the client. Null or empty returns null.</param>
        /// <param name="documentFormat">The document-format key (e.g. <c>xml</c>, <c>json</c>) that selects the matching path formatter.</param>
        /// <returns>The collection of matching DataItem ids, or <c>null</c> if the agent could not produce a devices document.</returns>
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
