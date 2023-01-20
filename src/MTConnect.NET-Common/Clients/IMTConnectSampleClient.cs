// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Errors;
using MTConnect.Streams;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that is used to perform a Sample request from an MTConnect Agent
    /// </summary>
    public interface IMTConnectSampleClient
    {
        /// <summary>
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </summary>
        string Device { get; set; }

        /// <summary> 
        /// (Optional) The sequence to retrieve the sample data from
        /// </summary>
        long From { get; set; }

        /// <summary> 
        /// (Optional) The sequence to retrieve the sample data to
        /// </summary>
        long To { get; set; }

        /// <summary> 
        /// (Optional) The number of sequences to include in the sample data
        /// </summary>
        long Count { get; set; }

        /// <summary>
        /// (Optional) The XPath expression specifying the components and/or data items to include
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        EventHandler<IErrorResponseDocument> OnMTConnectError { get; set; }


        /// <summary>
        /// Execute the Samples Request
        /// </summary>
        IStreamsResponseDocument Get();

        /// <summary>
        /// Asyncronously execute the Samples Request
        /// </summary>
        Task<IStreamsResponseDocument> GetAsync(CancellationToken cancel);
    }
}
