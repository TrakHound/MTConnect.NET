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
        ///
        /// </summary>
        [JsonPropertyName("interval")]
        public int Interval { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("heartbeat")]
        public int Heartbeat { get; set; }

        /// <summary>
        ///
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


        public HttpClientConfiguration()
        {
            Port = 5000;
            Interval = 500;
            Heartbeat = 1000;
            OutputConnectionInformation = true;
            CurrentOnly = false;
            UseStreaming = true;
        }


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