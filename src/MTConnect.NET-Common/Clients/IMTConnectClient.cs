// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using System;
using System.Threading;

namespace MTConnect.Clients
{
    public interface IMTConnectClient
    {
        /// <summary>
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </summary>
        string Device { get; }

        /// <summary>
        /// Gets or Sets the Document Format to use
        /// </summary>
        string DocumentFormat { get; }

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

        /// <summary>
        /// Gets or Sets whether the stream requests a Current (true) or a Sample (false)
        /// </summary>
        bool CurrentOnly { get; set; }


        #region "Events"

        /// <summary>
        /// Raised when an MTConnectDevices Document is received
        /// </summary>
        EventHandler<IDevicesResponseDocument> OnProbeReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from a Current Request
        /// </summary>
        EventHandler<IStreamsResponseDocument> OnCurrentReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from the Samples Stream
        /// </summary>
        EventHandler<IStreamsResponseDocument> OnSampleReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets Document is received
        /// </summary>
        EventHandler<IAssetsResponseDocument> OnAssetsReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        EventHandler<IErrorResponseDocument> OnMTConnectError { get; set; }

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
        void Start(string path = null);

        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        void Start(CancellationToken cancellationToken, string path = null);

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        void StartFromSequence(long instanceId, long sequence, string path = null);

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        void StartFromSequence(long instanceId, long sequence, CancellationToken cancellationToken, string path = null);

        /// <summary>
        /// Starts the MTConnectClient from the beginning of the MTConnect Agent's Buffer
        /// </summary>
        void StartFromBuffer(string path = null);

        /// <summary>
        /// Starts the MTConnectClient from the beginning of the MTConnect Agent's Buffer
        /// </summary>
        void StartFromBuffer(CancellationToken cancellationToken, string path = null);

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
