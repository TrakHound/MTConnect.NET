// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Http;
using System;

namespace MTConnect.Servers
{
    /// <summary>
    /// Describes a request the MTConnect HTTP server has resolved to a static file (for example a
    /// schema, styling document, or stylesheet referenced from an MTConnect response). The
    /// resolved record bundles the originating HTTP request together with both the URL-relative
    /// and the on-disk file paths so handlers can apply conditional headers and version pinning.
    /// </summary>
    public struct MTConnectStaticFileRequest
    {
        /// <summary>The originating HTTP request, including the headers used for cache validation and content negotiation.</summary>
        public IHttpRequest HttpRequest { get; set; }

        /// <summary>The relative file path requested by the client (the URL portion below the static-files mount).</summary>
        public string FilePath { get; set; }

        /// <summary>The fully resolved on-disk path that the relative <see cref="FilePath"/> maps to.</summary>
        public string LocalPath { get; set; }

        /// <summary>The MTConnect schema version associated with the static asset (used to select the correct stylesheet or schema variant).</summary>
        public Version Version { get; set; }
    }
}
