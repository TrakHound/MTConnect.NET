// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using System;
using System.Collections.Generic;

namespace MTConnect.Assets
{
    /// <summary>
    /// The Asset Information Model associates each electronic MTConnectAssets document with a unique
    /// identifier and allows for some predefined mechanisms to find, create, request, updated, and delete these
    /// electronic documents in a way that provides for consistency across multiple pieces of equipment.
    /// </summary>
    public interface IAssetsResponseDocument
    {
        /// <summary>
        /// Contains the Header information in an MTConnect Assets XML document
        /// </summary>
        IMTConnectAssetsHeader Header { get; }

        /// <summary>
        /// An XML container that consists of one or more types of Asset XML elements.
        /// </summary>
        IEnumerable<IAsset> Assets { get; }

        /// <summary>
        /// The MTConnect Version of the Response document
        /// </summary>
        Version Version { get; }
    }
}
