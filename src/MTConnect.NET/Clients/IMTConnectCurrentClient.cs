// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    public interface IMTConnectCurrentClient
    {
        /// <summary>
        /// (Optional) The name of the requested device
        /// </summary>
        string DeviceName { get; set; }

        /// <summary> 
        /// (Optional) The sequence number to retrieve the current data for
        /// </summary>
        long At { get; set; }

        /// <summary>
        /// (Optional) The XPath expression specifying the components and/or data items to include
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// User settable object sent with request and returned in Document on response
        /// </summary>
        object UserObject { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        EventHandler<Errors.ErrorDocument> OnMTConnectError { get; set; }


        /// <summary>
        /// Execute the Current Request
        /// </summary>
        Streams.StreamsDocument Get();

        /// <summary>
        /// Asyncronously execute the Current Request
        /// </summary>
        Task<Streams.StreamsDocument> GetAsync(CancellationToken cancel);
    }
}
