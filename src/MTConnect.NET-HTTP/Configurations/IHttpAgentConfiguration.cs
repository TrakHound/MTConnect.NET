// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using System.Collections.Generic;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Http Agent
    /// </summary>
    public interface IHttpAgentConfiguration : IAgentConfiguration
    {
        /// <summary>
        /// The port number the agent binds to for requests.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// The server IP Address to bind to. Can be used to select the interface in IPV4 or IPV6.
        /// </summary>
        string ServerIp { get; }


        /// <summary>
        /// Gets or Sets the List of Encodings (ex. gzip, br, deflate) to pass to the Accept-Encoding HTTP Header
        /// </summary>
        IEnumerable<Http.HttpResponseCompression> ResponseCompression { get; }

        /// <summary>
        /// Allow HTTP PUT or POST of data item values or assets.
        /// </summary>
        bool AllowPut { get; }

        /// <summary>
        /// Allow HTTP PUT or POST from a specific host or list of hosts. 
        /// Lists are comma (,) separated and the host names will be validated by translating them into IP addresses.
        /// </summary>
        IEnumerable<string> AllowPutFrom { get; }

        /// <summary>
        /// The maximum number of Threads to use for the Http Requests
        /// </summary>
        int MaxListenerThreads { get; }


        /// <summary>
        /// Gets or Sets the default response document indendation
        /// </summary>
        bool IndentOutput { get; }

        /// <summary>
        /// Gets or Sets the default response document comments output. Comments contain descriptions from the MTConnect standard
        /// </summary>
        bool OutputComments { get; }

        /// <summary>
        /// Gets or Sets the default response document validation level. 0 = Ignore, 1 = Warning, 2 = Strict
        /// </summary>
        ValidationLevel OutputValidationLevel { get; }
    }
}
