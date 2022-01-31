// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Threading;

namespace MTConnect.Clients
{
    public interface IMTConnectClient
    {
        /// <summary>
        /// (Optional) The name of the requested device
        /// </summary>
        string DeviceName { get; set; }

        /// <summary>
        /// Gets or Sets the Interval in Milliseconds for the Sample Stream
        /// </summary>
        int Interval { get; set; }

        /// <summary>
        /// Gets or Sets the MTConnect Agent Heartbeat for the request
        /// </summary>
        int Heartbeat { get; set; }

        /// <summary>
        /// Gets or Sets the Interval in Milliseconds that the Client will attempt to reconnect if the connection fails
        /// </summary>
        int RetryInterval { get; set; }

        /// <summary>
        /// Gets or Sets the Maximum Number of Samples returned per interval from the Sample Stream
        /// </summary>
        long MaximumSampleCount { get; set; }

        /// <summary>
        /// Gets the Last Instance ID read from the MTConnect Agent
        /// </summary>
        long LastInstanceId { get; }

        /// <summary>
        /// Gets the Last Sequence read from the MTConnect Agent
        /// </summary>
        long LastSequence { get; }

        /// <summary>
        /// Gets the Unix Timestamp (in Milliseconds) since the last response from the MTConnect Agent
        /// </summary>
        long LastResponse { get; }


        bool CurrentOnly { get; set; }


        #region "Events"

        /// <summary>
        /// Raised when an MTConnectDevices Document is received
        /// </summary>
        EventHandler<Devices.DevicesDocument> OnProbeReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from a Current Request
        /// </summary>
        EventHandler<Streams.StreamsDocument> OnCurrentReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from the Samples Stream
        /// </summary>
        EventHandler<Streams.StreamsDocument> OnSampleReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets Document is received
        /// </summary>
        EventHandler<Assets.AssetsDocument> OnAssetsReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        EventHandler<Errors.ErrorDocument> OnMTConnectError { get; set; }

        /// <summary>
        /// Raised when any Response from the Client is received
        /// </summary>
        EventHandler OnResponseReceived { get; set; }

        /// <summary>
        /// Raised when the Client is Starting
        /// </summary>
        EventHandler OnClientStarting { get; set; }

        /// <summary>
        /// Raised when the Client is Started
        /// </summary>
        EventHandler OnClientStarted { get; set; }

        /// <summary>
        /// Raised when the Client is Stopping
        /// </summary>
        EventHandler OnClientStopping { get; set; }

        /// <summary>
        /// Raised when the Client is Stopeed
        /// </summary>
        EventHandler OnClientStopped { get; set; }

        /// <summary>
        /// Raised when the Stream is Starting
        /// </summary>
        EventHandler<string> OnStreamStarting { get; set; }

        /// <summary>
        /// Raised when the Stream is Started
        /// </summary>
        EventHandler<string> OnStreamStarted { get; set; }

        /// <summary>
        /// Raised when the Stream is Stopped
        /// </summary>
        EventHandler<string> OnStreamStopping { get; set; }

        /// <summary>
        /// Raised when the Stream is Stopped
        /// </summary>
        EventHandler<string> OnStreamStopped { get; set; }

        #endregion


        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        void Start();

        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        void Start(CancellationToken cancellationToken);

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        void StartFromSequence(long sequence);

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        void StartFromSequence(long sequence, CancellationToken cancellationToken);

        /// <summary>
        /// Starts the MTConnectClient from the beginning of the MTConnect Agent's Buffer
        /// </summary>
        void StartFromBuffer();

        /// <summary>
        /// Starts the MTConnectClient from the beginning of the MTConnect Agent's Buffer
        /// </summary>
        void StartFromBuffer(CancellationToken cancellationToken);

        /// <summary>
        /// Stops the MTConnectClient
        /// </summary>
        void Stop();

        /// <summary>
        /// Resets the MTConnect Client to read from the beginning of the Agent Buffer
        /// </summary>
        void Reset();
    }
}
