// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MTConnect.Modules.WebUI
{
    /// <summary>
    /// Minimal <see cref="HttpListener"/>-based web server that serves the
    /// embedded UI page and the JSON API it calls.
    /// </summary>
    /// <remarks>
    /// Routes:
    /// <list type="bullet">
    /// <item><c>GET /</c> – the UI page</item>
    /// <item><c>GET /fonts/{name}.woff2</c> – the embedded Roboto / Roboto Mono fonts</item>
    /// <item><c>GET /api/agent</c> – Agent information and buffer state</item>
    /// <item><c>GET /api/devices</c> – Device / Component / DataItem tree</item>
    /// <item><c>GET /api/current?device=</c> – current Observations for a Device</item>
    /// <item><c>GET /api/sample?device=&amp;dataItemId=&amp;count=</c> – most recent Observations</item>
    /// <item><c>GET /api/assets?device=</c> – Asset list</item>
    /// <item><c>GET /api/asset?id=</c> – a single Asset as an MTConnectAssets XML document</item>
    /// <item><c>GET /api/config</c> / <c>PUT /api/config</c> – read / save the Agent configuration file</item>
    /// </list>
    /// </remarks>
    public class WebUiServer
    {
        private const string IndexResourceName = "MTConnect.WebUI.index.html";
        private const string FontResourcePrefix = "MTConnect.WebUI.fonts.";
        private const int MaxSampleCount = 1000;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly WebUiModuleConfiguration _configuration;
        private readonly IMTConnectAgentBroker _agent;
        private readonly ConcurrentDictionary<string, long> _observationCounts = new ConcurrentDictionary<string, long>();
        private HttpListener _listener;
        private CancellationTokenSource _stop;


        /// <summary>
        /// Raised with an informational message (for example the listening address).
        /// </summary>
        public event EventHandler<string> LogReceived;

        /// <summary>
        /// Raised with a message when the listener or a request fails.
        /// </summary>
        public event EventHandler<string> ErrorReceived;


        /// <summary>
        /// Initialises a new server for the given configuration and Agent.
        /// </summary>
        /// <param name="configuration">Listen address and edit policy.</param>
        /// <param name="agent">Agent broker the API reads from.</param>
        public WebUiServer(WebUiModuleConfiguration configuration, IMTConnectAgentBroker agent)
        {
            _configuration = configuration ?? new WebUiModuleConfiguration();
            _agent = agent;

            // Count Observations per Device from the moment the server is created
            if (_agent != null) _agent.ObservationAdded += ObservationAdded;
        }


        /// <summary>
        /// Starts listening on a background task. Returns immediately.
        /// </summary>
        public void Start()
        {
            var host = _configuration.Server;
            if (string.IsNullOrEmpty(host) || host == "*" || host == "0.0.0.0") host = "+";
            var prefix = $"http://{host}:{_configuration.Port}/";

            try
            {
                _stop = new CancellationTokenSource();
                _listener = new HttpListener();
                _listener.Prefixes.Add(prefix);
                _listener.Start();

                LogReceived?.Invoke(this, $"Web UI listening at {prefix.Replace("+", "localhost")}");

                _ = Task.Run(() => Listen(_stop.Token));
            }
            catch (Exception ex)
            {
                ErrorReceived?.Invoke(this, $"Web UI failed to start at {prefix} : {ex.Message}");
            }
        }

        /// <summary>
        /// Stops the listener and releases the port.
        /// </summary>
        public void Stop()
        {
            if (_agent != null) _agent.ObservationAdded -= ObservationAdded;

            try
            {
                if (_stop != null) _stop.Cancel();
                if (_listener != null) _listener.Close();
            }
            catch { }
        }


        private void ObservationAdded(object sender, IObservation observation)
        {
            if (observation != null && observation.DeviceUuid != null)
            {
                _observationCounts.AddOrUpdate(observation.DeviceUuid, 1, (key, count) => count + 1);
            }
        }

        private async Task Listen(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _listener.IsListening)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync();
                }
                catch
                {
                    // Listener was closed
                    break;
                }

                _ = Task.Run(() => HandleRequest(context));
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                var path = request.Url.AbsolutePath.TrimEnd('/').ToLowerInvariant();
                var method = request.HttpMethod;

                if (method == "GET" && (path == "" || path == "/index.html"))
                {
                    WriteIndex(response);
                }
                else if (method == "GET" && path.StartsWith("/fonts/")) WriteFont(response, path.Substring("/fonts/".Length));
                else if (method == "GET" && path == "/api/agent") WriteJson(response, GetAgentInfo());
                else if (method == "GET" && path == "/api/devices") WriteJson(response, GetDevices());
                else if (method == "GET" && path == "/api/current") WriteJson(response, GetCurrent(request.QueryString["device"]));
                else if (method == "GET" && path == "/api/sample") WriteJson(response, GetSample(request.QueryString["device"], request.QueryString["dataItemId"], request.QueryString["count"]));
                else if (method == "GET" && path == "/api/assets") WriteJson(response, GetAssets(request.QueryString["device"]));
                else if (method == "GET" && path == "/api/asset") WriteAsset(response, request.QueryString["id"]);
                else if (method == "GET" && path == "/api/config") WriteJson(response, GetConfiguration());
                else if (method == "PUT" && path == "/api/config") SaveConfiguration(request, response);
                else WriteJson(response, new { error = "Not Found" }, 404);
            }
            catch (Exception ex)
            {
                ErrorReceived?.Invoke(this, $"Web UI request error ({request.HttpMethod} {request.Url.AbsolutePath}) : {ex.Message}");
                try { WriteJson(response, new { error = ex.Message }, 500); } catch { }
            }
            finally
            {
                try { response.Close(); } catch { }
            }
        }


        #region "API"

        private object GetAgentInfo()
        {
            var configuration = _agent.Configuration;
            var applicationConfiguration = configuration as IAgentApplicationConfiguration;

            return new
            {
                uuid = _agent.Uuid,
                instanceId = _agent.InstanceId,
                sender = _agent.Sender,
                version = _agent.Version?.ToString(),
                mtconnectVersion = _agent.MTConnectVersion?.ToString(),
                deviceModelChangeTime = _agent.DeviceModelChangeTime,
                bufferSize = _agent.BufferSize,
                firstSequence = _agent.FirstSequence,
                lastSequence = _agent.LastSequence,
                nextSequence = _agent.NextSequence,
                assetBufferSize = _agent.AssetBufferSize,
                assetCount = _agent.AssetCount,
                configurationPath = configuration?.Path,
                monitorConfigurationFiles = applicationConfiguration?.MonitorConfigurationFiles ?? false,
                allowConfigurationEdit = _configuration.AllowConfigurationEdit,
                observationCounts = _observationCounts.ToDictionary(o => o.Key, o => o.Value),
                mtconnectServer = GetMTConnectServer(applicationConfiguration)
            };
        }

        /// <summary>
        /// Minimal view of an <c>http-server</c> module entry, used to link to the
        /// Agent's MTConnect REST API without referencing the HTTP server assembly.
        /// </summary>
        private class HttpServerModuleEntry
        {
            public int Port { get; set; } = 5000;
            public string Server { get; set; }
            public object Tls { get; set; }
        }

        private static object GetMTConnectServer(IAgentApplicationConfiguration configuration)
        {
            // Link to the first configured HTTP server module (the Agent's MTConnect REST API)
            var server = configuration?.GetModules<HttpServerModuleEntry>("http-server")?.FirstOrDefault();
            if (server == null) return null;

            return new
            {
                port = server.Port,
                server = server.Server,
                tls = server.Tls != null
            };
        }

        private object GetDevices()
        {
            var devices = _agent.GetDevices();
            if (devices == null) return new object[0];

            return devices.Select(device => new
            {
                id = device.Id,
                uuid = device.Uuid,
                name = device.Name,
                type = device.Type,
                dataItems = ToDataItems(device.DataItems),
                components = ToComponents(device.Components)
            }).ToList();
        }

        private static IEnumerable<object> ToComponents(IEnumerable<IComponent> components)
        {
            if (components == null) return new object[0];

            return components.Select(component => new
            {
                id = component.Id,
                name = component.Name,
                type = component.Type,
                dataItems = ToDataItems(component.DataItems),
                components = ToComponents(component.Components)
            }).ToList();
        }

        private static IEnumerable<object> ToDataItems(IEnumerable<IDataItem> dataItems)
        {
            if (dataItems == null) return new object[0];

            return dataItems.Select(dataItem => new
            {
                id = dataItem.Id,
                name = dataItem.Name,
                type = dataItem.Type,
                subType = dataItem.SubType,
                category = dataItem.Category.ToString(),
                units = dataItem.Units
            }).ToList();
        }

        private object GetCurrent(string deviceKey)
        {
            var observations = string.IsNullOrEmpty(deviceKey)
                ? _agent.GetCurrentObservations()
                : _agent.GetCurrentObservations(deviceKey);

            return ToObservations(observations);
        }

        private object GetSample(string deviceKey, string dataItemId, string countText)
        {
            if (string.IsNullOrEmpty(deviceKey)) return new object[0];

            var count = 100;
            if (int.TryParse(countText, out var parsedCount)) count = Math.Max(1, Math.Min(MaxSampleCount, parsedCount));

            var dataItemIds = string.IsNullOrEmpty(dataItemId) ? null : new[] { dataItemId };
            var first = _agent.FirstSequence;
            var last = _agent.LastSequence;
            if (last < first) return new object[0];

            // Walk backwards from the end of the buffer with a growing window
            // until enough Observations are found (or the whole buffer is read)
            var results = new List<IObservationOutput>();
            ulong window = (ulong)count;
            while (true)
            {
                var from = last >= first + window ? last - window + 1 : first;

                var document = dataItemIds != null
                    ? _agent.GetDeviceStreamsResponseDocument(deviceKey, dataItemIds, from, last, (uint)Math.Min(window, uint.MaxValue))
                    : _agent.GetDeviceStreamsResponseDocument(deviceKey, from, last, (uint)Math.Min(window, uint.MaxValue));

                results.Clear();
                if (document != null && document.Streams != null)
                {
                    foreach (var stream in document.Streams)
                    {
                        if (stream.Observations != null) results.AddRange(stream.Observations);
                    }
                }

                if (results.Count >= count || from <= first) break;
                window *= 10;
            }

            return ToObservations(results.OrderByDescending(o => o.Sequence).Take(count));
        }

        private static IEnumerable<object> ToObservations(IEnumerable<IObservationOutput> observations)
        {
            if (observations == null) return new object[0];

            return observations.Select(observation => new
            {
                deviceUuid = observation.DeviceUuid,
                dataItemId = observation.DataItemId,
                name = observation.Name,
                type = observation.Type,
                subType = observation.SubType,
                category = observation.Category.ToString(),
                representation = observation.Representation.ToString(),
                sequence = observation.Sequence,
                timestamp = observation.Timestamp,
                values = observation.Values != null
                    ? observation.Values.Where(v => v.Key != null).GroupBy(v => v.Key).ToDictionary(g => g.Key, g => g.First().Value)
                    : new Dictionary<string, string>()
            }).ToList();
        }

        private object GetAssets(string deviceKey)
        {
            var assets = string.IsNullOrEmpty(deviceKey) ? _agent.GetAssets() : _agent.GetAssets(deviceKey);
            if (assets == null) return new object[0];

            return assets.Select(asset => new
            {
                assetId = asset.AssetId,
                type = asset.Type,
                deviceUuid = asset.DeviceUuid,
                timestamp = asset.Timestamp,
                removed = asset.Removed
            }).ToList();
        }

        private void WriteAsset(HttpListenerResponse response, string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                WriteJson(response, new { error = "Missing 'id' parameter" }, 400);
                return;
            }

            var document = _agent.GetAssetsResponseDocument(new[] { assetId });
            var options = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("indentOutput", "true") };
            var result = ResponseDocumentFormatter.Format(DocumentFormat.XML, document, options);
            if (!result.Success || result.Content == null)
            {
                WriteJson(response, new { error = "Unable to format Asset" }, 500);
                return;
            }

            using (var reader = new StreamReader(result.Content))
            {
                WriteText(response, reader.ReadToEnd(), "application/xml; charset=utf-8");
            }
        }

        private object GetConfiguration()
        {
            var path = _agent.Configuration?.Path;
            string content = null;
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) content = File.ReadAllText(path);

            return new
            {
                path,
                content,
                editable = _configuration.AllowConfigurationEdit && !string.IsNullOrEmpty(path)
            };
        }

        private void SaveConfiguration(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (!_configuration.AllowConfigurationEdit)
            {
                WriteJson(response, new { error = "Configuration editing is disabled (allowConfigurationEdit: false)" }, 403);
                return;
            }

            var path = _agent.Configuration?.Path;
            if (string.IsNullOrEmpty(path))
            {
                WriteJson(response, new { error = "The Agent was not loaded from a configuration file" }, 400);
                return;
            }

            if (!path.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
            {
                WriteJson(response, new { error = "Only YAML configuration files can be edited from the UI" }, 400);
                return;
            }

            string content;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8))
            {
                content = reader.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                WriteJson(response, new { error = "Configuration is empty" }, 400);
                return;
            }

            // Validate the YAML with the same settings the Agent reads it with
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();

                deserializer.Deserialize<AgentApplicationConfiguration>(content);
            }
            catch (YamlException ex)
            {
                WriteJson(response, new { error = $"Invalid configuration (line {ex.Start.Line}, column {ex.Start.Column}) : {ex.InnerException?.Message ?? ex.Message}" }, 400);
                return;
            }

            // Keep a copy of the previous file before overwriting it
            if (File.Exists(path)) File.Copy(path, path + ".bak", true);
            File.WriteAllText(path, content, new UTF8Encoding(false));

            LogReceived?.Invoke(this, $"Agent configuration saved from Web UI ({path})");

            var applicationConfiguration = _agent.Configuration as IAgentApplicationConfiguration;
            WriteJson(response, new
            {
                saved = true,
                restartPending = applicationConfiguration?.MonitorConfigurationFiles ?? false
            });
        }

        #endregion

        #region "Response Writers"

        private static void WriteIndex(HttpListenerResponse response)
        {
            using (var stream = typeof(WebUiServer).GetTypeInfo().Assembly.GetManifestResourceStream(IndexResourceName))
            {
                if (stream == null)
                {
                    WriteJson(response, new { error = "UI resource not found" }, 500);
                    return;
                }

                using (var reader = new StreamReader(stream))
                {
                    WriteText(response, reader.ReadToEnd(), "text/html; charset=utf-8");
                }
            }
        }

        private static void WriteFont(HttpListenerResponse response, string fileName)
        {
            // Only names of embedded .woff2 resources resolve; anything else is a 404
            Stream stream = null;
            if (fileName.EndsWith(".woff2") && fileName.IndexOf('/') < 0)
            {
                stream = typeof(WebUiServer).GetTypeInfo().Assembly.GetManifestResourceStream(FontResourcePrefix + fileName);
            }

            if (stream == null)
            {
                WriteJson(response, new { error = "Not Found" }, 404);
                return;
            }

            using (stream)
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();

                response.StatusCode = 200;
                response.ContentType = "font/woff2";
                response.Headers["Cache-Control"] = "public, max-age=86400";
                response.ContentLength64 = bytes.Length;
                response.OutputStream.Write(bytes, 0, bytes.Length);
            }
        }

        private static void WriteJson(HttpListenerResponse response, object value, int statusCode = 200)
        {
            WriteText(response, JsonSerializer.Serialize(value, _jsonOptions), "application/json; charset=utf-8", statusCode);
        }

        private static void WriteText(HttpListenerResponse response, string text, string contentType, int statusCode = 200)
        {
            var bytes = Encoding.UTF8.GetBytes(text ?? "");
            response.StatusCode = statusCode;
            response.ContentType = contentType;
            response.Headers["Cache-Control"] = "no-store";
            response.ContentLength64 = bytes.Length;
            response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        #endregion
    }
}
