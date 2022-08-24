// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Http Agent
    /// </summary>
    public class HttpAgentConfiguration : AgentConfiguration, IHttpAgentConfiguration
    {
        /// <summary>
        /// The port number the agent binds to for requests.
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// The server IP Address to bind to. Can be used to select the interface in IPV4 or IPV6.
        /// </summary>
        [JsonPropertyName("serverIp")]
        public string ServerIp { get; set; }

        /// <summary>
        /// Gets or Sets the List of Encodings (ex. gzip, br, deflate) to pass to the Accept-Encoding HTTP Header
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Http.HttpResponseCompression> ResponseCompression
        {
            get
            {
                if (!ResponseCompressionString.IsNullOrEmpty())
                {
                    var responseCompression = new List<Http.HttpResponseCompression>();
                    foreach (var str in ResponseCompressionString)
                    {
                        var s = str;
                        if (!string.IsNullOrEmpty(s)) s = s.ToTitleCase();
                        var rc = s.ConvertEnum<Http.HttpResponseCompression>();
                        if (rc != Http.HttpResponseCompression.None) responseCompression.Add(rc);
                    }
                    return responseCompression;
                }
                return null;
            }
        }

        [JsonPropertyName("responseCompression")]
        public IEnumerable<string> ResponseCompressionString { get; set; }

        /// <summary>
        /// Allow HTTP PUT or POST of data item values or assets.
        /// </summary>
        [JsonPropertyName("allowPut")]
        public bool AllowPut { get; set; }

        /// <summary>
        /// Allow HTTP PUT or POST from a specific host or list of hosts. 
        /// Lists are comma (,) separated and the host names will be validated by translating them into IP addresses.
        /// </summary>
        [JsonPropertyName("allowPutFrom")]
        public IEnumerable<string> AllowPutFrom { get; set; }

        /// <summary>
        /// The maximum number of Threads to use for the Http Requests
        /// </summary>
        [JsonPropertyName("maxListenerThreads")]
        public int MaxListenerThreads { get; set; }


        /// <summary>
        /// Gets or Sets the default response document indendation
        /// </summary>
        [JsonPropertyName("indentOutput")]
        public bool IndentOutput { get; set; }

        /// <summary>
        /// Gets or Sets the default response document comments output. Comments contain descriptions from the MTConnect standard
        /// </summary>
        [JsonPropertyName("outputComments")]
        public bool OutputComments { get; set; }


        /// <summary>
        /// Gets or Sets the default response document validation level. 0 = Ignore, 1 = Warning, 2 = Strict
        /// </summary>
        [JsonPropertyName("outputValidationLevel")]
        public ValidationLevel OutputValidationLevel { get; set; }


        public HttpAgentConfiguration()
        {
            ServerIp = "127.0.0.1";
            Port = 5000;
            AllowPut = false;
            AllowPutFrom = null;
            MaxListenerThreads = 5;
            IndentOutput = true;
            OutputComments = false;
            OutputValidationLevel = ValidationLevel.Ignore;
        }
    }
}
