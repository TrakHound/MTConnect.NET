// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Http;
using MTConnect.Tls;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace MTConnect.Configurations
{
    public class HttpServerConfiguration : IHttpServerConfiguration
    {
        /// <summary>
        /// The port number the agent binds to for requests.
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// The server Hostname to bind to. Change this to the server's IP Address or hostname
        /// </summary>
        [JsonPropertyName("server")]
        public string Server { get; set; }

        /// <summary>
        /// Gets or Sets the TLS settings
        /// </summary>
        [JsonPropertyName("tls")]
        public TlsConfiguration Tls { get; set; }

        /// <summary>
        /// Maps HTTP Accept Headers to the corresponding Document Format ID
        /// </summary>
        [JsonPropertyName("accept")]
        public Dictionary<string, string> Accept { get; set; }

        /// <summary>
        /// Gets or Sets the List of Encodings (ex. gzip, br, deflate) to pass to the Accept-Encoding HTTP Header
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        public IEnumerable<HttpResponseCompression> ResponseCompression
        {
            get
            {
                if (!ResponseCompressionString.IsNullOrEmpty())
                {
                    var responseCompression = new List<HttpResponseCompression>();
                    foreach (var str in ResponseCompressionString)
                    {
                        var s = str;
                        if (!string.IsNullOrEmpty(s)) s = s.ToTitleCase();
                        var rc = s.ConvertEnum<HttpResponseCompression>();
                        if (rc != HttpResponseCompression.None) responseCompression.Add(rc);
                    }
                    return responseCompression;
                }
                return null;
            }
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    var strValues = new List<string>();
                    foreach (var val in value)
                    {
                        strValues.Add(val.ToString());
                    }
                    ResponseCompressionString = strValues;
                }
                else
                {
                    ResponseCompressionString = null;
                }
            }
        }

        [JsonPropertyName("responseCompression")]
        [YamlMember(Alias = "responseCompression")]
        public IEnumerable<string> ResponseCompressionString { get; set; }

        /// <summary>
        /// Allow HTTP PUT or POST of data item values or assets.
        /// </summary>
        [JsonPropertyName("allowPut")]
        public bool AllowPut { get; set; }

        /// <summary>
        /// Allow HTTP PUT or POST from a specific host or list of hosts. 
        /// </summary>
        [JsonPropertyName("allowPutFrom")]
        public IEnumerable<string> AllowPutFrom { get; set; }


        /// <summary>
        /// Gets or Sets the default MTConnect Version to respond to requests with
        /// </summary>
        [JsonPropertyName("defaultVersion")]
        public string DefaultVersion { get; set; }

        /// <summary>
        /// Gets or Sets the default DocumentFormat (ex. XML, JSON)
        /// </summary>
        [JsonPropertyName("documentFormat")]
        public string DocumentFormat { get; set; }

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
        public OutputValidationLevel OutputValidationLevel { get; set; }


        /// <summary>
        /// Gets or Sets the configuration for Static Files that can be served from the Http Server
        /// </summary>
        [JsonPropertyName("files")]
        public IEnumerable<FileConfiguration> Files { get; set; }


        public HttpServerConfiguration()
        {
            Server = null;
            Port = 5000;

            DocumentFormat = MTConnect.DocumentFormat.XML;

            Accept = new Dictionary<string, string>();
            Accept["text/xml"] = "XML";
            Accept["application/xml"] = "XML";
            Accept["application/json"] = "JSON";

            AllowPut = false;
            AllowPutFrom = null;
            IndentOutput = true;
            OutputComments = false;
            OutputValidationLevel = OutputValidationLevel.Ignore;
        }
    }
}
