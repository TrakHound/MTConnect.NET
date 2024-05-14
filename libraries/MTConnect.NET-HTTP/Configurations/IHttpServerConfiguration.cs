// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Http;
using MTConnect.Tls;
using System.Collections.Generic;

namespace MTConnect.Configurations
{
    public interface IHttpServerConfiguration
    {
        /// <summary>
        /// The port number the agent binds to for requests.
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// The server Hostname to bind to. Change this to the server's IP Address or hostname
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// Gets or Sets the TLS settings
        /// </summary>
        TlsConfiguration Tls { get; set; }

        /// <summary>
        /// Maps HTTP Accept Headers to the corresponding Document Format ID
        /// </summary>
        Dictionary<string, string> Accept { get; set; }

        /// <summary>
        /// Gets or Sets the List of Encodings (ex. gzip, br, deflate) to pass to the Accept-Encoding HTTP Header
        /// </summary>
        IEnumerable<HttpResponseCompression> ResponseCompression { get; set; }

        /// <summary>
        /// Allow HTTP PUT or POST of data item values or assets.
        /// </summary>
        bool AllowPut { get; set; }

        /// <summary>
        /// Allow HTTP PUT or POST from a specific host or list of hosts. 
        /// </summary>
        IEnumerable<string> AllowPutFrom { get; set; }


        /// <summary>
        /// 
        /// </summary>
        string DefaultVersion { get; set; }

        /// <summary>
        /// Gets or Sets the default DocumentFormat (ex. XML, JSON)
        /// </summary>
        string DocumentFormat { get; set; }

        /// <summary>
        /// Gets or Sets the default response document indendation
        /// </summary>
        bool IndentOutput { get; set; }

        /// <summary>
        /// Gets or Sets the default response document comments output. Comments contain descriptions from the MTConnect standard
        /// </summary>
        bool OutputComments { get; set; }


        /// <summary>
        /// Gets or Sets the default response document validation level. 0 = Ignore, 1 = Warning, 2 = Strict
        /// </summary>
        OutputValidationLevel OutputValidationLevel { get; set; }


        /// <summary>
        /// Gets or Sets the configuration for Static Files that can be served from the Http Server
        /// </summary>
        IEnumerable<FileConfiguration> Files { get; set; }
    }
}
