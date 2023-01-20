// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Errors;
using MTConnect.Streams;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that is used to perform a Current request from an MTConnect Agent
    /// </summary>
    public interface IMTConnectCurrentClient
    {
        /// <summary>
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </summary>
        string Device { get; }

        /// <summary> 
        /// Gets or Sets the sequence number to retrieve the current data for
        /// </summary>
        long At { get; set; }

        /// <summary>
        /// An XPath that defines specific information or a set of information to be included in an MTConnectStreams Response Document.       
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        EventHandler<IErrorResponseDocument> OnMTConnectError { get; set; }


        /// <summary>
        /// Execute the Current Request and return an MTConnectStreams Response document
        /// </summary>
        /// <returns>An IStreamsDocument Response document</returns>
        IStreamsResponseDocument Get();

        /// <summary>
        /// Asyncronously execute the Current Request and return an MTConnectStreams Response document
        /// </summary>
        /// <returns>An IStreamsDocument Response document</returns>
        Task<IStreamsResponseDocument> GetAsync(CancellationToken cancel);
    }
}