// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using System;
using System.Threading;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that implements the full MTConnect Api Protocol (Probe, Current, Sample Stream, and Assets)
    /// </summary>
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
        int MaximumSampleCount { get; set; }

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
        /// Raised when the Stream is Starting
        /// </summary>
        event EventHandler<string> StreamStarting;

        /// <summary>
        /// Raised when the Stream is Started
        /// </summary>
        event EventHandler<string> StreamStarted;

        /// <summary>
        /// Raised when the Stream is Stopped
        /// </summary>
        event EventHandler<string> StreamStopping;

        /// <summary>
        /// Raised when the Stream is Stopped
        /// </summary>
        event EventHandler<string> StreamStopped;

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