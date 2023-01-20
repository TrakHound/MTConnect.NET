// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Servers.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MTConnect.Applications.Agents
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

        private readonly IHttpAgentApplicationConfiguration _configuration;


        public MTConnectHttpAgentServer(IHttpAgentApplicationConfiguration configuration, IMTConnectAgentBroker mtconnectAgent, IEnumerable<string> prefixes = null, int port = 0) : base(configuration, mtconnectAgent, prefixes, port)
        {
            _configuration = configuration;
        }


        protected override byte[] OnProcessStatic(HttpListenerRequest httpRequest, string absolutePath, string relativePath, Version version = null)
        {
            if (_configuration.DevicesStyle != null && (relativePath == _configuration.DevicesStyle.Location || relativePath.Replace('\\', '/') == _configuration.StreamsStyle.Location))
            {
                return ReadDevicesStylesheet(absolutePath, version);
            }
            else if (_configuration.StreamsStyle != null && (relativePath == _configuration.StreamsStyle.Location || relativePath.Replace('\\', '/') == _configuration.StreamsStyle.Location))
            {
                return ReadStreamsStylesheet(absolutePath, version);
            }

            return null;
        }

        protected override List<KeyValuePair<string, string>> OnCreateFormatOptions(string requestType, string documentFormat, Version mtconnectVersion, int validationLevel = 0)
        {
            var x = new List<KeyValuePair<string, string>>();

            switch (documentFormat)
            {
                case DocumentFormat.XML:

                    if (_configuration != null)
                    {
                        if (validationLevel > 0)
                        {
                            // Add XSD Schema (xlink)
                            x.Add(new KeyValuePair<string, string>("schema", ReadCommonSchema(mtconnectVersion)));

                            // Add XSD Schema
                            switch (requestType)
                            {
                                case MTConnectRequestType.Probe: x.Add(new KeyValuePair<string, string>("schema", ReadDevicesSchema(mtconnectVersion))); break;
                                case MTConnectRequestType.Current: x.Add(new KeyValuePair<string, string>("schema", ReadStreamsSchema(mtconnectVersion))); break;
                                case MTConnectRequestType.Sample: x.Add(new KeyValuePair<string, string>("schema", ReadStreamsSchema(mtconnectVersion))); break;
                                case MTConnectRequestType.Asset: x.Add(new KeyValuePair<string, string>("schema", ReadAssetsSchema(mtconnectVersion))); break;
                                case MTConnectRequestType.Assets: x.Add(new KeyValuePair<string, string>("schema", ReadAssetsSchema(mtconnectVersion))); break;
                            }
                        }

                        if (!_configuration.StreamsNamespaces.IsNullOrEmpty())
                        {
                            foreach (var streamsNamespace in _configuration.StreamsNamespaces)
                            {
                                x.Add(new KeyValuePair<string, string>("namespace", JsonSerializer.Serialize(streamsNamespace)));
                            }
                        }

                        // Add Devices Stylesheet
                        if (_configuration.DevicesStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("devicesStyle.location", _configuration.DevicesStyle.Location));
                            x.Add(new KeyValuePair<string, string>("devicesStyle.path", _configuration.DevicesStyle.Path));
                        }

                        // Add Streams Stylesheet
                        if (_configuration.StreamsStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("streamsStyle.location", _configuration.StreamsStyle.Location));
                            x.Add(new KeyValuePair<string, string>("streamsStyle.path", _configuration.StreamsStyle.Path));
                        }

                        // Add Assets Stylesheet
                        if (_configuration.AssetsStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("assetsStyle.location", _configuration.AssetsStyle.Location));
                            x.Add(new KeyValuePair<string, string>("assetsStyle.path", _configuration.AssetsStyle.Path));
                        }

                        // Add Error Stylesheet
                        if (_configuration.ErrorStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("errorStyle.location", _configuration.ErrorStyle.Location));
                            x.Add(new KeyValuePair<string, string>("errorStyle.path", _configuration.ErrorStyle.Path));
                        }
                    }

                    break;
            }

            return x;
        }


        #region "Stylesheets"

        private byte[] ReadDevicesStylesheet(string filePath, Version mtconnectVersion)
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

                        return Encoding.UTF8.GetBytes(s);
                    }
                }
                catch { }
            }

            return null;
        }

        private byte[] ReadStreamsStylesheet(string filePath, Version mtconnectVersion)
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

                        return Encoding.UTF8.GetBytes(fileContents);
                    }
                }
                catch { }
            }

            return null;
        }

        #endregion

        #region "Schemas"

        private string ReadDevicesSchema(Version mtconnectVersion)
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

        private string ReadStreamsSchema(Version mtconnectVersion)
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

        private string ReadAssetsSchema(Version mtconnectVersion)
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

        private string ReadCommonSchema(Version mtconnectVersion)
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
