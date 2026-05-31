// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Http;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for a MTConnect Http Client
    /// </summary>
    public class HttpClientConfiguration : IHttpClientConfiguration
    {
        /// <summary>
        /// The unique identifier for the Adapter
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        public string Id
        {
            get
            {
                var id = $"{Address}:{Port}/{DeviceKey}";
                id = id.ToMD5Hash().Substring(0, 10);
                return $"adapter_http_{id}";
            }
        }

        /// <summary>
        /// The Name or UUID of the MTConnect Device
        /// </summary>
        [JsonPropertyName("deviceKey")]
        public string DeviceKey { get; set; }

        /// <summary>
        /// The URL address the client MTConnect Agent is located at.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// The port to connect to the client MTConnect Agent.
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// The polling/streaming interval in milliseconds. For polling clients this is the wait
        /// between successive <c>current</c>/<c>sample</c> requests; for streaming clients it is
        /// the <c>interval</c> query parameter sent to the agent's <c>sample</c> long-poll.
        /// </summary>
        [JsonPropertyName("interval")]
        public int Interval { get; set; }

        /// <summary>
        /// The streaming heartbeat in milliseconds passed to the agent as the <c>heartbeat</c>
        /// query parameter on <c>sample</c> long-poll requests. The agent emits an empty document
        /// each heartbeat to keep the HTTP connection alive when no observations are produced.
        /// </summary>
        [JsonPropertyName("heartbeat")]
        public int Heartbeat { get; set; }

        /// <summary>
        /// When true, the client connects to the agent over <c>https://</c>; when false, over
        /// <c>http://</c>. Any explicit scheme already present in <see cref="Address"/> is
        /// rewritten to match this flag.
        /// </summary>
        [JsonPropertyName("useSSL")]
        public bool UseSSL { get; set; }

        /// <summary>
        /// Gets or Sets whether the Connection Information (Host / Port) is output to the Agent to be collected by a client
        /// </summary>
        [JsonPropertyName("outputConnectionInformation")]
        public bool OutputConnectionInformation { get; set; }

        /// <summary>
        /// Gets or Sets whether the stream requests a Current (true) or a Sample (false)
        /// </summary>
        [JsonPropertyName("currentOnly")]
        public bool CurrentOnly { get; set; }

        /// <summary>
        /// Gets or Sets whether the client should use Streaming (true) or Polling (false)
        /// </summary>
        [JsonPropertyName("useStreaming")]
        public bool UseStreaming { get; set; }


        /// <summary>
        /// Initialises a new client configuration with the MTConnect-default port (5000), a 500 ms
        /// interval, a 1000 ms heartbeat, connection-information output enabled, streaming sampling
        /// enabled, and current-only mode disabled.
        /// </summary>
        public HttpClientConfiguration()
        {
            Port = 5000;
            Interval = 500;
            Heartbeat = 1000;
            OutputConnectionInformation = true;
            CurrentOnly = false;
            UseStreaming = true;
        }


        /// <summary>
        /// Builds the agent base URI (scheme + host + port, no trailing slash, no path) from a
        /// configuration instance. Any pre-existing <c>http://</c> or <c>https://</c> prefix in
        /// <see cref="Address"/> is stripped and replaced according to <see cref="UseSSL"/>.
        /// Returns <c>null</c> if <paramref name="configuration"/> is <c>null</c>.
        /// </summary>
        public static string CreateBaseUri(HttpClientConfiguration configuration)
        {
            if (configuration != null)
            {
                string baseUrl = null;
                var clientAddress = configuration.Address;
                var clientPort = configuration.Port;

                if (configuration.UseSSL) clientAddress = clientAddress.Replace("https://", "");
                else clientAddress = clientAddress.Replace("http://", "");

                // Create the MTConnect Agent Base URL
                if (configuration.UseSSL) baseUrl = string.Format("https://{0}", Url.AddPort(clientAddress, clientPort));
                else baseUrl = string.Format("http://{0}", Url.AddPort(clientAddress, clientPort));

                return baseUrl;
            }

            return null;
        }
    }
}