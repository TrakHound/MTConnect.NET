// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that is used to perform a Probe request from an MTConnect Agent
    /// </summary>
    public interface IMTConnectProbeClient
    {
        /// <summary>
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </summary>
        string Device { get; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        EventHandler<IErrorResponseDocument> OnMTConnectError { get; set; }


        /// <summary>
        /// Execute the Probe Request and return a MTConnectDevice Response document
        /// </summary>
        IDevicesResponseDocument Get();

        /// <summary>
        /// Asyncronously execute the Probe Request and return a MTConnectDevice Response document
        /// </summary>
        Task<IDevicesResponseDocument> GetAsync(CancellationToken cancellationToken);
    }
}