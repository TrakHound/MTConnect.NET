// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    public interface IMTConnectProbeClient
    {
        /// <summary>
        /// (Optional) The name of the requested device
        /// </summary>
        string DeviceName { get; set; }

        /// <summary>
        /// (Optional) User settable object sent with request and returned in Document on response
        /// </summary>
        object UserObject { get; set; }

        /// <summary>
        /// Gets of Sets the connection timeout for the request
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        EventHandler<Errors.ErrorDocument> OnMTConnectError { get; set; }


        /// <summary>
        /// Execute the Probe Request
        /// </summary>
        Devices.DevicesDocument Get();

        /// <summary>
        /// Asyncronously execute the Probe Request
        /// </summary>
        Task<Devices.DevicesDocument> GetAsync(CancellationToken cancel);
    }
}
