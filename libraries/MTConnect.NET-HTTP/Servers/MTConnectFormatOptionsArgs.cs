// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Servers
{
    /// <summary>
    /// Context passed to format-option providers (registered through the formatter extension
    /// points) so they can decide which serialisation knobs apply to a particular MTConnect
    /// response. The HTTP server populates this once per outgoing response.
    /// </summary>
    public struct MTConnectFormatOptionsArgs
    {
        /// <summary>The MTConnect request type being formatted (see <see cref="MTConnect.Http.MTConnectRequestType"/>).</summary>
        public string RequestType { get; set; }

        /// <summary>The document format key (e.g. <c>xml</c>, <c>json</c>) that will be produced.</summary>
        public string DocumentFormat { get; set; }

        /// <summary>The MTConnect schema version that should be honoured when serialising the document.</summary>
        public Version MTConnectVersion { get; set; }

        /// <summary>
        /// The numeric mapping of <see cref="MTConnect.Http.OutputValidationLevel"/> that tells
        /// the formatter how strictly to validate the produced document.
        /// </summary>
        public int ValidationLevel { get; set; }
    }
}
