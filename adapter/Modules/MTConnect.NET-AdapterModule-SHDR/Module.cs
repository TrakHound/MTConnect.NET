// Copyright(c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Adapters;
using MTConnect.Configurations;
using MTConnect.Input;
using MTConnect.Logging;
using MTConnect.Shdr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MTConnect
{
    /// <summary>
    /// Adapter module that listens for SHDR-protocol TCP clients (i.e.
    /// downstream MTConnect agents) and streams observations / assets /
    /// device models as SHDR text lines. The module owns an
    /// <see cref="AgentClientConnectionListener"/> and routes outbound
    /// data to every connected agent client.
    /// </summary>
    public class Module : MTConnectAdapterModule
    {
        /// <summary>
        /// Token used in <c>adapter.config.yaml</c> to bind this
        /// module (<c>type: shdr</c>).
        /// </summary>
        public const string ConfigurationTypeId = "shdr";
        private const string ModuleId = "SDHR";

        private readonly object _lock = new object();
        private readonly ModuleConfiguration _configuration;
        private readonly TimeZoneInfo _timeZoneInfo;
        private readonly AgentClientConnectionListener _connectionListener;
        private readonly Dictionary<string, AgentClient> _clients = new Dictionary<string, AgentClient>();
        private CancellationTokenSource _stop;


        /// <summary>
        /// Initialises the module, binds the supplied configuration
        /// payload to <see cref="ModuleConfiguration"/>, and wires the
        /// <see cref="AgentClientConnectionListener"/> connect /
        /// disconnect / ping / pong events.
        /// </summary>
        /// <param name="id">Adapter-side identifier the host passes
        /// through.</param>
        /// <param name="moduleConfiguration">Raw configuration payload
        /// bound to <see cref="ModuleConfiguration"/>.</param>
        public Module(string id, object moduleConfiguration) : base(id)
        {
            Id = ModuleId;

            _configuration = AdapterApplicationConfiguration.GetConfiguration<ModuleConfiguration>(moduleConfiguration);
            if (_configuration == null) _configuration = new ModuleConfiguration();

            _timeZoneInfo = GetTimeZone(_configuration);

            _connectionListener = new AgentClientConnectionListener(_configuration.Port, _configuration.Heartbeat);
            _connectionListener.ClientConnected += AgentClientConnected;
            _connectionListener.ClientDisconnected += AgentClientDisconnected;
            _connectionListener.ClientPingReceived += AgentClientPingReceived;
            _connectionListener.ClientPongSent += AgentClientPongSent;
        }


        /// <summary>
        /// Module lifecycle hook: starts the SHDR-protocol connection
        /// listener so downstream agents can connect.
        /// </summary>
        protected override void OnStart()
        {
            _stop = new CancellationTokenSource();

            // Start Agent Connection Listener
            _connectionListener.Start(_stop.Token);
        }

        /// <summary>
        /// Module lifecycle hook: cancels the connection-listener
        /// token and stops the listener. Existing clients drain via
        /// their own disconnect handlers.
        /// </summary>
        protected override void OnStop()
        {
            if (_stop != null) _stop.Cancel();
            _connectionListener.Stop();
        }


        #region "Entities"

        /// <summary>
        /// Translates each observation into one or more SHDR text
        /// lines (data items, messages, conditions, time-series,
        /// data-sets, tables) and writes them to every connected
        /// agent client.
        /// </summary>
        /// <param name="observations">Observations to transmit.</param>
        /// <returns>Always <c>true</c>; transport-level errors per
        /// client are caught inside <c>WriteLine</c>.</returns>
        public override bool AddObservations(IEnumerable<IObservationInput> observations)
        {
            // DataItems
            var dataItems = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.DataItem);
            if (!dataItems.IsNullOrEmpty())
            {
                var shdrDataItems = new List<ShdrDataItem>();
                foreach (var x in dataItems)
                {
                    shdrDataItems.Add(new ShdrDataItem(x));
                }

                var shdrLine = ShdrDataItem.ToString(shdrDataItems, timeZoneInfo: _timeZoneInfo);
                WriteLine(shdrLine);
            }

            // Messages
            var messages = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.Message);
            if (!messages.IsNullOrEmpty())
            {
                foreach (var x in messages)
                {
                    var shdrModel = new ShdrMessage(x);
                    shdrModel.TimeZoneInfo = _timeZoneInfo;

                    var shdrLine = shdrModel.ToString();
                    WriteLine(shdrLine);
                }
            }

            // Conditions
            var conditions = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.Condition);
            if (!conditions.IsNullOrEmpty())
            {
                foreach (var x in conditions)
                {
                    var shdrModel = new ShdrFaultState(new ConditionFaultStateObservationInput(x));
                    shdrModel.TimeZoneInfo = _timeZoneInfo;

                    var shdrLine = shdrModel.ToString();
                    WriteLine(shdrLine);
                }
            }

            // DataSets
            var dataSets = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.DataSet);
            if (!dataSets.IsNullOrEmpty())
            {
                foreach (var x in dataSets)
                {
                    var shdrModel = new ShdrDataSet(new DataSetObservationInput(x));
                    shdrModel.TimeZoneInfo = _timeZoneInfo;

                    var shdrLine = shdrModel.ToString();
                    WriteLine(shdrLine);
                }
            }

            // Tables
            var tables = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.Table);
            if (!tables.IsNullOrEmpty())
            {
                foreach (var x in tables)
                {
                    var shdrModel = new ShdrTable(new TableObservationInput(x));
                    shdrModel.TimeZoneInfo = _timeZoneInfo;

                    var shdrLine = shdrModel.ToString();
                    WriteLine(shdrLine);
                }
            }

            // TimeSeries
            var timeSeries = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.TimeSeries);
            if (!timeSeries.IsNullOrEmpty())
            {
                foreach (var x in timeSeries)
                {
                    var shdrModel = new ShdrTimeSeries(new TimeSeriesObservationInput(x));
                    shdrModel.TimeZoneInfo = _timeZoneInfo;

                    var shdrLine = shdrModel.ToString();
                    WriteLine(shdrLine);
                }
            }

            return true;
        }

        /// <summary>
        /// Serialises each asset into an SHDR asset block and writes
        /// it to every connected agent client.
        /// </summary>
        /// <param name="assets">Assets to transmit.</param>
        /// <returns>Always <c>true</c>; transport-level errors per
        /// client are caught inside <c>WriteLine</c>.</returns>
        public override bool AddAssets(IEnumerable<IAssetInput> assets)
        {
            foreach (var asset in assets)
            {
                var shdrModel = new ShdrAsset(asset.Asset);
                var shdrLine = shdrModel.ToString();
                WriteLine(shdrLine);
            }

            return true;
        }

        /// <summary>
        /// Serialises each device into an SHDR device block and writes
        /// it to every connected agent client.
        /// </summary>
        /// <param name="devices">Devices to transmit.</param>
        /// <returns>Always <c>true</c>; transport-level errors per
        /// client are caught inside <c>WriteLine</c>.</returns>
        public override bool AddDevices(IEnumerable<IDeviceInput> devices)
        {
            foreach (var device in devices)
            {
                var shdrModel = new ShdrDevice(device.Device);
                var shdrLine = shdrModel.ToString();
                WriteLine(shdrLine);
            }

            return true;
        }

        #endregion

        #region "Event Handlers"

        private void AgentClientConnected(string clientId, TcpClient client)
        {
            AddAgentClient(clientId, client);
            Log(MTConnectLogLevel.Information, "Agent Connected");
            Adapter.SendLast(UnixDateTime.Now);
        }

        private void AgentClientDisconnected(string clientId)
        {
            RemoveAgentClient(clientId);
            Log(MTConnectLogLevel.Information, "Agent Disconnected");
        }

        private void AgentClientPingReceived(string clientId)
        {
            Log(MTConnectLogLevel.Information, "PING Received");
        }

        private void AgentClientPongSent(string clientId)
        {
            Log(MTConnectLogLevel.Information, "PONG Sent");
        }

        #endregion

        #region "Clients"

        private void AddAgentClient(string clientId, TcpClient tcpClient)
        {
            if (!string.IsNullOrEmpty(clientId) && tcpClient != null)
            {
                lock (_lock)
                {
                    _clients.Remove(clientId);
                    _clients.Add(clientId, new AgentClient(clientId, tcpClient));
                }
            }
        }

        private AgentClient GetAgentClient(string clientId)
        {
            if (!string.IsNullOrEmpty(clientId))
            {
                lock (_lock)
                {
                    if (_clients.TryGetValue(clientId, out AgentClient agentClient))
                    {
                        return agentClient;
                    }
                }
            }

            return null;
        }

        private IEnumerable<AgentClient> GetAgentClients()
        {
            lock (_lock)
            {
                return _clients.Values;
            }
        }

        private void RemoveAgentClient(string clientId)
        {
            if (!string.IsNullOrEmpty(clientId))
            {
                lock (_lock)
                {
                    _clients.Remove(clientId);
                }
            }
        }

        #endregion

        #region "Write"

        /// <summary>
        /// Writes the supplied SHDR text line to every connected agent
        /// client. Per-client transport failures are caught and the
        /// affected client is silently dropped from the broadcast set.
        /// </summary>
        /// <param name="line">SHDR-encoded text line.</param>
        /// <returns><c>true</c> when at least one client was written
        /// to; <c>false</c> otherwise.</returns>
        protected bool WriteLine(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                try
                {
                    // Write Line to each client in stored client list
                    var clients = GetAgentClients();
                    if (!clients.IsNullOrEmpty())
                    {
                        var success = true;

                        foreach (var client in clients)
                        {
                            if (!WriteLineToClient(client, line)) success = false;
                        }

                        return success;
                    }
                }
                catch { }
            }

            return false;
        }

        private bool WriteLineToClient(AgentClient client, string line)
        {
            if (client != null && !string.IsNullOrEmpty(line))
            {
                var lines = SplitLines(line);
                if (!lines.IsNullOrEmpty())
                {
                    foreach (var singleLine in lines)
                    {
                        try
                        {
                            // Convert string to ASCII bytes and add line terminator
                            var bytes = Encoding.ASCII.GetBytes(singleLine + "\n");

                            // Get the TcpClient Stream
                            var stream = client.TcpClient.GetStream();
                            stream.ReadTimeout = _configuration.ConnectionTimeout;
                            stream.WriteTimeout = _configuration.ConnectionTimeout;

                            // Write the line (in bytes) to the Stream
                            stream.Write(bytes, 0, bytes.Length);

                            //Log(MTConnectLogLevel.Debug, $"SHDR line written to stream : Client ID = {client.Id} : {bytes.Length} bytes");
                            Log(MTConnectLogLevel.Debug, $"SHDR line written to stream : Client ID = {client.Id} : {singleLine}");
                        }
                        catch (Exception ex)
                        {
                            Log(MTConnectLogLevel.Error, $"SHDR Write ERROR : {ex.Message}");
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        // Split Lines by \r\n
        // Can't use string.Split(string, StringSplitOptions.TrimEntries since
        // it isn't fully compatible with all of the target runtimes
        private static IEnumerable<string> SplitLines(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                var lines = new List<string>();
                char cr = '\r';
                char lf = '\n';
                char prev = '$';
                var s = 0;
                var e = 0;
                string l;

                while (e < line.Length - 1)
                {
                    // Look for \r\n
                    if (line[e] == lf && prev == cr)
                    {
                        // Add trimmed line to return list
                        l = line.Substring(s, (e - s) + 1).Trim('\r').Trim('\n');
                        if (!string.IsNullOrEmpty(l))
                        {
                            if (l.Length > 1 || (l.Length == 1 && l[0] != cr))
                            {
                                lines.Add(l);
                            }
                        }
                        s = e;
                    }

                    prev = line[e];
                    e++;
                }

                // Get Last Line
                l = line.Substring(s, (e - s) + 1).Trim('\n');
                if (!string.IsNullOrEmpty(l))
                {
                    if (l.Length > 1 || (l.Length == 1 && l[0] != cr))
                    {
                        lines.Add(l);
                    }
                }

                return lines;
            }

            return null;
        }

        #endregion


        private static TimeZoneInfo GetTimeZone(ModuleConfiguration configuration)
        {
            if (configuration != null)
            {
                if (!string.IsNullOrEmpty(configuration.TimeZoneOutput))
                {
                    var timeZoneDefinition = MTConnectTimeZone.Get(configuration.TimeZoneOutput);
                    if (timeZoneDefinition != null)
                    {
                        var timeZoneInfo = timeZoneDefinition.ToTimeZoneInfo();
                        if (timeZoneInfo != null)
                        {
                            return timeZoneInfo;
                        }
                    }
                }
            }

            return TimeZoneInfo.Utc;
        }

    }
}