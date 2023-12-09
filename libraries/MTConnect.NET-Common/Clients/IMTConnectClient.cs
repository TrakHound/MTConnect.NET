// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using System;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that implements the full MTConnect Api Protocol (Probe, Current, Sample Stream, and Assets)
    /// </summary>
    public interface IMTConnectClient
    {
        /// <summary>
        /// Gets the Unix Timestamp (in Milliseconds) since the last response from the MTConnect Agent
        /// </summary>
        long LastResponse { get; }


        /// <summary>
        /// Raised when an MTConnectDevices Document is received
        /// </summary>
        event EventHandler<IDevicesResponseDocument> ProbeReceived;

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from a Current Request
        /// </summary>
        event EventHandler<IStreamsResponseDocument> CurrentReceived;

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from the Samples Stream
        /// </summary>
        event EventHandler<IStreamsResponseDocument> SampleReceived;

        /// <summary>
        /// Raised when an MTConnectAssets Document is received
        /// </summary>
        event EventHandler<IAssetsResponseDocument> AssetsReceived;

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        event EventHandler<IErrorResponseDocument> MTConnectError;

        /// <summary>
        /// Raised when any Response from the Client is received
        /// </summary>
        event EventHandler ResponseReceived;

        /// <summary>
        /// Raised when the Client is Starting
        /// </summary>
        event EventHandler ClientStarting;

        /// <summary>
        /// Raised when the Client is Started
        /// </summary>
        event EventHandler ClientStarted;

        /// <summary>
        /// Raised when the Client is Stopping
        /// </summary>
        event EventHandler ClientStopping;

        /// <summary>
        /// Raised when the Client is Stopeed
        /// </summary>
        event EventHandler ClientStopped;


        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the MTConnectClient
        /// </summary>
        void Stop();
    }
}