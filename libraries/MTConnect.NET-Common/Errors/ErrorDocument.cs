// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using System;
using System.Collections.Generic;

namespace MTConnect.Errors
{
    /// <summary>
    /// The Error Information Model establishes the rules and terminology that describes the Response Document
    /// returned by an Agent when it encounters an error while interpreting a Request for information from a client
    /// software application or when an Agent experiences an error while publishing the Response to a Request for information.
    /// </summary>
    public class ErrorResponseDocument : IErrorResponseDocument
    {
        /// <summary>
        /// The Header element for an MTConnectErrors Response Document defines information regarding
        /// the creation of the document and the data storage capability of the Agent that generated the document.
        /// </summary>
        public IMTConnectErrorHeader Header { get; set; }

        /// <summary>
        /// An XML container element in an MTConnectErrors Response Document provided by an Agent when an error
        /// is encountered associated with a Request for information from a client software application.
        /// </summary>
        public IEnumerable<IError> Errors { get; set; }

        /// <summary>
        /// The MTConnect Version of the Response document
        /// </summary>
        public Version Version { get; set; }
    }
}