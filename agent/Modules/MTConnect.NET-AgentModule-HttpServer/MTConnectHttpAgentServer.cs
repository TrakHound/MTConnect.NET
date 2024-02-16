// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen.Httpd;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Servers;
using MTConnect.Servers.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MTConnect.Modules.Http
{
    /// <summary>
    /// An Http Web Server for processing MTConnect REST Api Requests including functionality for serving XML related files
    /// </summary>
    public class MTConnectHttpAgentServer : MTConnectHttpServer
    {
        private static readonly Dictionary<string, string> _devicesSchemas = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _streamsSchemas = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _assetsSchemas = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _commonSchemas = new Dictionary<string, string>();
        private static readonly object _lock = new object();

        private readonly HttpServerModuleConfiguration _moduleConfiguration;


        public MTConnectHttpAgentServer(HttpServerModuleConfiguration configuration, IMTConnectAgentBroker mtconnectAgent) : base(configuration, mtconnectAgent)
        {
            _moduleConfiguration = configuration;
        }


        protected override void OnConfigureServer(ServerConfig serverConfig)
        {
            base.OnConfigureServer(serverConfig);
        }

        protected override Stream OnProcessStatic(MTConnectStaticFileRequest request)
        {
            if (request.LocalPath != null)
            {
                if (_moduleConfiguration.DevicesStyle != null && _moduleConfiguration.DevicesStyle.Location != null)
                {
                    var requestPath = !request.LocalPath.StartsWith("/") ? "/" + request.LocalPath : request.LocalPath;

                    // Check to see if the request is for the Devices Stylesheet set in the Agent Configuration
                    if (requestPath == _moduleConfiguration.DevicesStyle.Location || requestPath.Replace('\\', '/') == _moduleConfiguration.DevicesStyle.Location)
                    {
                        return ReadDevicesStylesheet(request.FilePath, request.Version);
                    }
                }
                else if (_moduleConfiguration.StreamsStyle != null && _moduleConfiguration.StreamsStyle.Location != null)
                {
                    var requestPath = !request.LocalPath.StartsWith("/") ? "/" + request.LocalPath : request.LocalPath;

                    // Check to see if the request is for the Streams Stylesheet set in the Agent Configuration
                    if (requestPath == _moduleConfiguration.StreamsStyle.Location || requestPath.Replace('\\', '/') == _moduleConfiguration.StreamsStyle.Location)
                    {
                        return ReadStreamsStylesheet(request.FilePath, request.Version);
                    }
                }
            }

            return null;
        }

        protected override List<KeyValuePair<string, string>> OnCreateFormatOptions(MTConnectFormatOptionsArgs args)
        {
            var x = new List<KeyValuePair<string, string>>();

            switch (args.DocumentFormat)
            {
                case DocumentFormat.XML:

                    if (_configuration != null)
                    {
                        if (args.ValidationLevel > 0)
                        {
                            // Add XSD Schema (xlink)
                            x.Add(new KeyValuePair<string, string>("schema", ReadCommonSchema(args.MTConnectVersion)));

                            // Add XSD Schema
                            switch (args.RequestType)
                            {
                                case MTConnectRequestType.Probe: x.Add(new KeyValuePair<string, string>("schema", ReadDevicesSchema(args.MTConnectVersion))); break;
                                case MTConnectRequestType.Current: x.Add(new KeyValuePair<string, string>("schema", ReadStreamsSchema(args.MTConnectVersion))); break;
                                case MTConnectRequestType.Sample: x.Add(new KeyValuePair<string, string>("schema", ReadStreamsSchema(args.MTConnectVersion))); break;
                                case MTConnectRequestType.Asset: x.Add(new KeyValuePair<string, string>("schema", ReadAssetsSchema(args.MTConnectVersion))); break;
                                case MTConnectRequestType.Assets: x.Add(new KeyValuePair<string, string>("schema", ReadAssetsSchema(args.MTConnectVersion))); break;
                            }
                        }

                        if (!_moduleConfiguration.StreamsNamespaces.IsNullOrEmpty())
                        {
                            foreach (var streamsNamespace in _moduleConfiguration.StreamsNamespaces)
                            {
                                x.Add(new KeyValuePair<string, string>("namespace", JsonSerializer.Serialize(streamsNamespace)));
                            }
                        }

                        // Add Devices Stylesheet
                        if (_moduleConfiguration.DevicesStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("devicesStyle.location", _moduleConfiguration.DevicesStyle.Location));
                            x.Add(new KeyValuePair<string, string>("devicesStyle.path", _moduleConfiguration.DevicesStyle.Path));
                        }

                        // Add Streams Stylesheet
                        if (_moduleConfiguration.StreamsStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("streamsStyle.location", _moduleConfiguration.StreamsStyle.Location));
                            x.Add(new KeyValuePair<string, string>("streamsStyle.path", _moduleConfiguration.StreamsStyle.Path));
                        }

                        // Add Assets Stylesheet
                        if (_moduleConfiguration.AssetsStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("assetsStyle.location", _moduleConfiguration.AssetsStyle.Location));
                            x.Add(new KeyValuePair<string, string>("assetsStyle.path", _moduleConfiguration.AssetsStyle.Path));
                        }

                        // Add Error Stylesheet
                        if (_moduleConfiguration.ErrorStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("errorStyle.location", _moduleConfiguration.ErrorStyle.Location));
                            x.Add(new KeyValuePair<string, string>("errorStyle.path", _moduleConfiguration.ErrorStyle.Path));
                        }
                    }

                    break;
            }

            return x;
        }


        #region "Stylesheets"

        private static Stream ReadDevicesStylesheet(string filePath, Version mtconnectVersion)
        {
            if (filePath != null)
            {
                try
                {
                    var fileContents = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(fileContents))
                    {
                        string s = fileContents;
                        string pattern = null;
                        string replace = null;

                        try
                        {
                            // Replace Devices Namespace
                            pattern = @"urn:mtconnect\.org:MTConnectDevices:(\d\.\d)";
                            replace = $@"urn:mtconnect.org:MTConnectDevices:{mtconnectVersion}";
                            s = Regex.Replace(fileContents, pattern, replace);
                        }
                        catch { }

                        try
                        {
                            // Replace Streams Namespace
                            pattern = @"urn:mtconnect\.org:MTConnectStreams:(\d\.\d)";
                            replace = $@"urn:mtconnect.org:MTConnectStreams:{mtconnectVersion}";
                            s = Regex.Replace(s, pattern, replace);
                        }
                        catch { }

                        return new MemoryStream(Encoding.UTF8.GetBytes(s));
                    }
                }
                catch { }
            }

            return null;
        }

        private static Stream ReadStreamsStylesheet(string filePath, Version mtconnectVersion)
        {
            if (filePath != null)
            {
                try
                {
                    var fileContents = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(fileContents))
                    {
                        if (mtconnectVersion != null)
                        {
                            // Replace Streams Namespace
                            var pattern = @"urn:mtconnect\.org:MTConnectStreams:(\d\.\d)";
                            var replace = $@"urn:mtconnect.org:MTConnectStreams:{mtconnectVersion.Major}.{mtconnectVersion.Minor}";
                            fileContents = Regex.Replace(fileContents, pattern, replace);
                        }

                        return new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
                    }
                }
                catch { }
            }

            return null;
        }

        #endregion

        #region "Schemas"

        private static string ReadDevicesSchema(Version mtconnectVersion)
        {
            if (mtconnectVersion != null)
            {
                var versionKey = mtconnectVersion.ToString();
                string schema = null;
                lock (_lock) if (_devicesSchemas.TryGetValue(versionKey, out var x)) schema = x;

                if (string.IsNullOrEmpty(schema))
                {
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
                    var filename = $"MTConnectDevices_{mtconnectVersion.Major}.{mtconnectVersion.Minor}.xsd";
                    var path = Path.Combine(dir, filename);

                    if (File.Exists(path))
                    {
                        try
                        {
                            schema = File.ReadAllText(path);
                            if (!string.IsNullOrEmpty(schema))
                            {
                                lock (_lock) _devicesSchemas.Add(versionKey, schema);
                            }
                        }
                        catch { }
                    }
                }

                return schema;
            }

            return null;
        }

        private static string ReadStreamsSchema(Version mtconnectVersion)
        {
            if (mtconnectVersion != null)
            {
                var versionKey = mtconnectVersion.ToString();
                string schema = null;
                lock (_lock) if (_streamsSchemas.TryGetValue(versionKey, out var x)) schema = x;

                if (string.IsNullOrEmpty(schema))
                {
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
                    var filename = $"MTConnectStreams_{mtconnectVersion.Major}.{mtconnectVersion.Minor}.xsd";
                    var path = Path.Combine(dir, filename);

                    if (File.Exists(path))
                    {
                        try
                        {
                            schema = File.ReadAllText(path);
                            if (!string.IsNullOrEmpty(schema))
                            {
                                lock (_lock) _streamsSchemas.Add(versionKey, schema);
                            }
                        }
                        catch { }
                    }
                }

                return schema;
            }

            return null;
        }

        private static string ReadAssetsSchema(Version mtconnectVersion)
        {
            if (mtconnectVersion != null)
            {
                var versionKey = mtconnectVersion.ToString();
                string schema = null;
                lock (_lock) if (_assetsSchemas.TryGetValue(versionKey, out var x)) schema = x;

                if (string.IsNullOrEmpty(schema))
                {
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
                    var filename = $"MTConnectAssets_{mtconnectVersion.Major}.{mtconnectVersion.Minor}.xsd";
                    var path = Path.Combine(dir, filename);

                    if (File.Exists(path))
                    {
                        try
                        {
                            schema = File.ReadAllText(path);
                            if (!string.IsNullOrEmpty(schema))
                            {
                                lock (_lock) _assetsSchemas.Add(versionKey, schema);
                            }
                        }
                        catch { }
                    }
                }

                return schema;
            }

            return null;
        }

        private static string ReadCommonSchema(Version mtconnectVersion)
        {
            if (mtconnectVersion != null)
            {
                var key = "xlink";
                string schema = null;
                lock (_lock) if (_commonSchemas.TryGetValue(key, out var x)) schema = x;

                if (string.IsNullOrEmpty(schema))
                {
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
                    var filename = "xlink.xsd";
                    var path = Path.Combine(dir, filename);

                    if (File.Exists(path))
                    {
                        try
                        {
                            schema = File.ReadAllText(path);
                            if (!string.IsNullOrEmpty(schema))
                            {
                                lock (_lock) _commonSchemas.Add(key, schema);
                            }
                        }
                        catch { }
                    }
                }

                return schema;
            }

            return null;
        }

        #endregion

    }
}