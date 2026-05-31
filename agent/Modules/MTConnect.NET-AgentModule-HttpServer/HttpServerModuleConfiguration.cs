// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration shape for the HTTP-server agent module. Inherits
    /// the base <see cref="HttpServerConfiguration"/> (host, port,
    /// allowed paths, …) and adds per-document namespace and stylesheet
    /// settings so the emitted XML / JSON responses can carry custom
    /// XSD namespaces or XSL stylesheets.
    /// </summary>
    public class HttpServerModuleConfiguration : HttpServerConfiguration
    {
        /// <summary>
        /// Extra XML namespaces injected into <c>MTConnectDevices</c>
        /// responses.
        /// </summary>
        public IEnumerable<NamespaceConfiguration> DevicesNamespaces { get; set; }

        /// <summary>
        /// Extra XML namespaces injected into <c>MTConnectStreams</c>
        /// (current / sample) responses.
        /// </summary>
        public IEnumerable<NamespaceConfiguration> StreamsNamespaces { get; set; }

        /// <summary>
        /// Extra XML namespaces injected into <c>MTConnectAssets</c>
        /// responses.
        /// </summary>
        public IEnumerable<NamespaceConfiguration> AssetsNamespaces { get; set; }

        /// <summary>
        /// Extra XML namespaces injected into <c>MTConnectError</c>
        /// responses.
        /// </summary>
        public IEnumerable<NamespaceConfiguration> ErrorNamespaces { get; set; }


        /// <summary>
        /// XSL stylesheet pinned to every <c>MTConnectDevices</c>
        /// response.
        /// </summary>
        public StyleConfiguration DevicesStyle { get; set; }

        /// <summary>
        /// XSL stylesheet pinned to every <c>MTConnectStreams</c>
        /// response.
        /// </summary>
        public StyleConfiguration StreamsStyle { get; set; }

        /// <summary>
        /// XSL stylesheet pinned to every <c>MTConnectAssets</c>
        /// response.
        /// </summary>
        public StyleConfiguration AssetsStyle { get; set; }

        /// <summary>
        /// XSL stylesheet pinned to every <c>MTConnectError</c>
        /// response.
        /// </summary>
        public StyleConfiguration ErrorStyle { get; set; }
    }
}