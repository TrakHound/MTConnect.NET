// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using System.Collections.Generic;

namespace MTConnect.Formatters
{
    /// <summary>
    /// Resolves a path expression (such as an XPath) against a Devices document to the set of matching DataItem IDs, for a particular document format.
    /// </summary>
    public interface IPathFormatter
    {
        /// <summary>
        /// The unique identifier of the document format this path formatter applies to.
        /// </summary>
        string Id { get; }


        /// <summary>
        /// Evaluates the given path expression against the supplied Devices document and returns the IDs of the DataItems it selects.
        /// </summary>
        /// <param name="devicesDocument">The Devices document to evaluate the path against.</param>
        /// <param name="path">The format-specific path expression.</param>
        IEnumerable<string> GetDataItemIds(IDevicesResponseDocument devicesDocument, string path);
    }
}