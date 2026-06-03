// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Shdr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Adapters
{
    /// <summary>
    /// An Adapter class for communicating with an MTConnect Agent using the SHDR protocol.
    /// Supports multiple concurrent Agent connections.
    /// Uses a queue to collect changes to Observations and Assets and sends the most recent changes at the specified interval.
    /// </summary>
    public class ShdrAdapter
    {
        private readonly IMTConnectAdapter _adapter;
        private readonly AgentClientConnectionListener _connectionListener;
        private readonly Dictionary<string, AgentClient> _clients = new Dictionary<string, AgentClient>();
        private readonly object _lock = new object();


        private CancellationTokenSource _stop;
        /// <summary>Exposes the internal cancellation source to subclasses so they can hook custom shutdown work onto the same token used by the adapter listener and worker threads.</summary>
        protected CancellationTokenSource StopToken => _stop;


        /// <summary>The underlying <see cref="IMTConnectAdapter"/> that buffers SHDR observations and serialises them onto each connected agent's socket.</summary>
        protected IMTConnectAdapter Adapter => _adapter;


        /// <summary>
        /// Get a unique identifier for the Adapter
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The Name or UUID of the Device to create a connection for
        /// </summary>
        public string DeviceKey { get; }

        /// <summary>
        /// The TCP Port used for communication
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// The heartbeat used to maintain a connection between the Adapter and the Agent
        /// </summary>
        public int Heartbeat { get; }

        /// <summary>
        /// The amount of time (in milliseconds) to allow for a connection attempt to the Agent
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Use multiline Assets
        /// </summary>
        public bool MultilineAssets { get; set; }

        /// <summary>
        /// Use multiline Devices
        /// </summary>
        public bool MultilineDevices { get; set; }

        /// <summary>
        /// Determines whether to filter out duplicate data
        /// </summary>
        public bool FilterDuplicates { get; set; }

        /// <summary>
        /// Determines whether to output Timestamps for each SHDR line
        /// </summary>
        public bool OutputTimestamps { get; set; }


        /// <summary>
        /// Raised when a new Agent connection is established. Includes the AgentClient ID as an argument.
        /// </summary>
        public event EventHandler<string> AgentConnected;

        /// <summary>
        /// Raised when an existing Agent connection is disconnected. Includes the AgentClient ID as an argument.
        /// </summary>
        public event EventHandler<string> AgentDisconnected;

        /// <summary>
        /// Raised when an error occurs during an existing Agent connection. Includes the AgentClient ID as an argument.
        /// </summary>
        #pragma warning disable CS0067 // event is part of the public API surface, raised by subclasses
        public event EventHandler<string> AgentConnectionError;
        #pragma warning restore CS0067


        /// <summary>
        /// Raised when a Ping request message is received from an Agent. Includes the AgentClient ID as an argument.
        /// </summary>
        public event EventHandler<string> PingReceived;

        /// <summary>
        /// Raised when a Pong response message is sent to an Agent. Includes the AgentClient ID as an argument.
        /// </summary>
        public event EventHandler<string> PongSent;

        /// <summary>
        /// Raised when a new line is sent to the Agent. Includes the AgentClient ID and the Line sent as an argument.
        /// </summary>
        public event EventHandler<AdapterEventArgs<string>> LineSent;

        /// <summary>
        /// Raised when new data is sent to the Agent. Includes the AgentClient ID and the Line sent as an argument.
        /// </summary>
        #pragma warning disable CS0067 // event is part of the public API surface, raised by subclasses
        public event EventHandler<AdapterEventArgs<string>> DataSent;
        #pragma warning restore CS0067

        /// <summary>
        /// Raised when an error occurs when sending a new line to the Agent. Includes the AgentClient ID and the Error message as an argument.
        /// </summary>
        public event EventHandler<AdapterEventArgs<string>> SendError;

        /// <summary>
        /// Raised when an error a TCP connection error occurs
        /// </summary>
        public event EventHandler<AdapterEventArgs<Exception>> ConnectionError;


        /// <summary>Creates an SHDR adapter that listens on <paramref name="port"/> with the supplied <paramref name="heartbeat"/> interval (milliseconds), no device-key scope, duplicate filtering enabled, and timestamps emitted on every line.</summary>
        public ShdrAdapter(int port = 7878, int heartbeat = 10000)
        {
            FilterDuplicates = true;
            OutputTimestamps = true;
            Port = port;
            Heartbeat = heartbeat;
            Timeout = 5000;

            var adapter = new MTConnectAdapter(null, false);
            adapter.OutputTimestamps = OutputTimestamps;
            adapter.WriteObservationsFunction = WriteObservations;
            adapter.WriteAssetsFunction = WriteAssets;
            adapter.WriteDevicesFunction = WriteDevices;
            _adapter = adapter;

            _connectionListener = new AgentClientConnectionListener(Port, heartbeat);
            _connectionListener.ClientConnected += ClientConnected;
            _connectionListener.ClientDisconnected += ClientDisconnected;
            _connectionListener.ClientPingReceived += ClientPingReceived;
            _connectionListener.ClientPongSent += ClientPongSent;
            _connectionListener.ConnectionErrorReceived += ClientConnectionError;
        }

        /// <summary>Creates an SHDR adapter scoped to <paramref name="deviceKey"/>, listening on <paramref name="port"/> with the supplied <paramref name="heartbeat"/> interval; lines targeting other devices are skipped.</summary>
        public ShdrAdapter(string deviceKey, int port = 7878, int heartbeat = 10000)
        {
            FilterDuplicates = true;
            OutputTimestamps = true;
            DeviceKey = deviceKey;
            Port = port;
            Heartbeat = heartbeat;
            Timeout = 5000;

            var adapter = new MTConnectAdapter(null, false);
            adapter.OutputTimestamps = OutputTimestamps;
            adapter.WriteObservationsFunction = WriteObservations;
            adapter.WriteAssetsFunction = WriteAssets;
            adapter.WriteDevicesFunction = WriteDevices;
            _adapter = adapter;

            _connectionListener = new AgentClientConnectionListener(Port, heartbeat);
            _connectionListener.ClientConnected += ClientConnected;
            _connectionListener.ClientDisconnected += ClientDisconnected;
            _connectionListener.ClientPingReceived += ClientPingReceived;
            _connectionListener.ClientPongSent += ClientPongSent;
            _connectionListener.ConnectionErrorReceived += ClientConnectionError;
        }

        /// <summary>Creates an SHDR adapter from <paramref name="configuration"/>, pulling device key, port, heartbeat, and the <c>IgnoreTimestamps</c> flag from the configuration object.</summary>
        public ShdrAdapter(ShdrAdapterClientConfiguration configuration)
        {
            FilterDuplicates = true;
            OutputTimestamps = true;

            if (configuration != null)
            {
                DeviceKey = configuration.DeviceKey;
                Port = configuration.Port;
                Heartbeat = configuration.Heartbeat;
                Timeout = 5000;

                var adapter = new MTConnectAdapter(null, false);
                adapter.IgnoreTimestamps = configuration.IgnoreTimestamps;
                adapter.OutputTimestamps = OutputTimestamps;
                adapter.WriteObservationsFunction = WriteObservations;
                adapter.WriteAssetsFunction = WriteAssets;
                adapter.WriteDevicesFunction = WriteDevices;
                _adapter = adapter;

                _connectionListener = new AgentClientConnectionListener(Port, Heartbeat);
                _connectionListener.ClientConnected += ClientConnected;
                _connectionListener.ClientDisconnected += ClientDisconnected;
                _connectionListener.ClientPingReceived += ClientPingReceived;
                _connectionListener.ClientPongSent += ClientPongSent;
                _connectionListener.ConnectionErrorReceived += ClientConnectionError;
            }
        }

        /// <summary>Protected constructor used by interval and queue subclasses to wire the optional flush <paramref name="interval"/> (milliseconds) and to opt in to the queue-buffer mode via <paramref name="bufferEnabled"/>.</summary>
        protected ShdrAdapter(int port = 7878, int heartbeat = 10000, int? interval = null, bool bufferEnabled = false)
        {
            FilterDuplicates = true;
            OutputTimestamps = true;
            Port = port;
            Heartbeat = heartbeat;
            Timeout = 5000;

            var adapter = new MTConnectAdapter(interval, bufferEnabled);
            adapter.OutputTimestamps = OutputTimestamps;
            adapter.WriteObservationsFunction = WriteObservations;
            adapter.WriteAssetsFunction = WriteAssets;
            adapter.WriteDevicesFunction = WriteDevices;
            _adapter = adapter;

            _connectionListener = new AgentClientConnectionListener(Port, heartbeat);
            _connectionListener.ClientConnected += ClientConnected;
            _connectionListener.ClientDisconnected += ClientDisconnected;
            _connectionListener.ClientPingReceived += ClientPingReceived;
            _connectionListener.ClientPongSent += ClientPongSent;
            _connectionListener.ConnectionErrorReceived += ClientConnectionError;
        }

        /// <summary>Protected device-keyed variant of the interval/queue constructor; see <see cref="ShdrAdapter(int, int, int?, bool)"/> for the <paramref name="interval"/> and <paramref name="bufferEnabled"/> semantics.</summary>
        protected ShdrAdapter(string deviceKey, int port = 7878, int heartbeat = 10000, int? interval = null, bool bufferEnabled = false)
        {
            FilterDuplicates = true;
            OutputTimestamps = true;
            DeviceKey = deviceKey;
            Port = port;
            Heartbeat = heartbeat;
            Timeout = 5000;

            var adapter = new MTConnectAdapter(interval, bufferEnabled);
            adapter.OutputTimestamps = OutputTimestamps;
            adapter.WriteObservationsFunction = WriteObservations;
            adapter.WriteAssetsFunction = WriteAssets;
            adapter.WriteDevicesFunction = WriteDevices;
            _adapter = adapter;

            _connectionListener = new AgentClientConnectionListener(Port, heartbeat);
            _connectionListener.ClientConnected += ClientConnected;
            _connectionListener.ClientDisconnected += ClientDisconnected;
            _connectionListener.ClientPingReceived += ClientPingReceived;
            _connectionListener.ClientPongSent += ClientPongSent;
            _connectionListener.ConnectionErrorReceived += ClientConnectionError;
        }

        /// <summary>Protected configuration-driven variant of the interval/queue constructor; see <see cref="ShdrAdapter(int, int, int?, bool)"/> for the <paramref name="interval"/> and <paramref name="bufferEnabled"/> semantics.</summary>
        protected ShdrAdapter(ShdrAdapterClientConfiguration configuration, int? interval = null, bool bufferEnabled = false)
        {
            FilterDuplicates = true;
            OutputTimestamps = true;

            if (configuration != null)
            {
                DeviceKey = configuration.DeviceKey;
                Port = configuration.Port;
                Heartbeat = configuration.Heartbeat;
                Timeout = 5000;

                var adapter = new MTConnectAdapter(interval, bufferEnabled);
                adapter.IgnoreTimestamps = configuration.IgnoreTimestamps;
                adapter.WriteObservationsFunction = WriteObservations;
                adapter.WriteAssetsFunction = WriteAssets;
                adapter.WriteDevicesFunction = WriteDevices;
                _adapter = adapter;

                _connectionListener = new AgentClientConnectionListener(Port, Heartbeat);
                _connectionListener.ClientConnected += ClientConnected;
                _connectionListener.ClientDisconnected += ClientDisconnected;
                _connectionListener.ClientPingReceived += ClientPingReceived;
                _connectionListener.ClientPongSent += ClientPongSent;
                _connectionListener.ConnectionErrorReceived += ClientConnectionError;
            }
        }


        /// <summary>
        /// Starts the Adapter to begins listening for Agent connections as well as starts the Queue for collecting and sending data to the Agent(s).
        /// </summary>
        public void Start()
        {
            _stop = new CancellationTokenSource();

            _adapter.Start();

            // Start Agent Connection Listener
            _connectionListener.Start(_stop.Token);

            // Call Overridable Method
            OnStart();
        }

        /// <summary>
        /// Stops the adapter which also stops listening for Agent connections, disconnects any existing Agent connections, and stops the Queue for sending data to the Agent(s).
        /// </summary>
        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
            _connectionListener.Stop();
            _adapter.Stop();

            // Call Overridable Method
            OnStop();
        }


        /// <summary>Override-point invoked at the tail end of <see cref="Start"/>, after the listener and underlying adapter have been started.</summary>
        protected virtual void OnStart() { }

        /// <summary>Override-point invoked at the tail end of <see cref="Stop"/>, after the listener and underlying adapter have been signalled to stop.</summary>
        protected virtual void OnStop() { }


        /// <summary>
        /// Set all items to Unavailable
        /// </summary>
        public void SetUnavailable(long timestamp = 0)
        {
            _adapter.SetUnavailable(timestamp);
        }


        #region "Event Handlers"

        private void ClientConnected(string clientId, TcpClient client)
        {
            AddAgentClient(clientId, client);
            MulticastIsolation.Raise(AgentConnected, this, clientId, null);

            SendLast(UnixDateTime.Now);
        }

        private void ClientDisconnected(string clientId)
        {
            RemoveAgentClient(clientId);
            MulticastIsolation.Raise(AgentDisconnected, this, clientId, null);
        }

        private void ClientPingReceived(string clientId)
        {
            MulticastIsolation.Raise(PingReceived, this, clientId, null);
        }

        private void ClientPongSent(string clientId)
        {
            MulticastIsolation.Raise(PongSent, this, clientId, null);
        }

        private void ClientConnectionError(string clientId, Exception exception)
        {
            MulticastIsolation.Raise(ConnectionError, this, new AdapterEventArgs<Exception>(clientId, exception), null);
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

        #region "Send"

        /// <summary>
        /// Sends all Items that have changed since last sent to the Agent
        /// </summary>
        public bool SendChanged()
        {
            return _adapter.SendChanged();
        }

        /// <summary>
        /// Sends all of the last sent Items, Assets, and Devices to the Agent. This can be used upon reconnection to the Agent
        /// </summary>
        public bool SendLast(long timestamp = 0)
        {
            return _adapter.SendLast(timestamp);
        }


        /// <summary>Override-point invoked after <see cref="SendChanged"/> completes; subclasses can use it to record diagnostics or refresh derived state.</summary>
        protected virtual void OnChangedSent() { }

        /// <summary>Override-point invoked after <see cref="SendLast"/> completes (typically once per reconnect); subclasses can use it to clear sent flags or trigger follow-up publishes.</summary>
        protected virtual void OnLastSent() { }

        #endregion

        #region "Write"

        private bool WriteObservations(IEnumerable<IObservationInput> observations)
        {
            // DataItems
            var dataItems = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.DataItem);
            if (!dataItems.IsNullOrEmpty())
            {
                var shdrDataItems = new List<ShdrDataItem>();
                foreach (var x in dataItems) shdrDataItems.Add(new ShdrDataItem(x));
                var shdrLine = ShdrDataItem.ToString(shdrDataItems, !OutputTimestamps);
                if (!WriteLine(shdrLine)) return false;
            }

            // Messages
            var messages = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.Message);
            if (!messages.IsNullOrEmpty())
            {
                foreach (var x in messages)
                {
                    var shdrModel = new ShdrMessage(x);
                    var shdrLine = shdrModel.ToString();
                    if (!WriteLine(shdrLine)) return false;
                }
            }

            // Conditions
            var conditions = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.Condition);
            if (!conditions.IsNullOrEmpty())
            {
                foreach (var x in conditions)
                {
                    var shdrModel = new ShdrFaultState(new ConditionFaultStateObservationInput(x));
                    var shdrLine = shdrModel.ToString();
                    if (!WriteLine(shdrLine)) return false;
                }
            }

            // DataSets
            var dataSets = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.DataSet);
            if (!dataSets.IsNullOrEmpty())
            {
                foreach (var x in dataSets)
                {
                    var shdrModel = new ShdrDataSet(new DataSetObservationInput(x));
                    var shdrLine = shdrModel.ToString();
                    if (!WriteLine(shdrLine)) return false;
                }
            }

            // Tables
            var tables = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.Table);
            if (!tables.IsNullOrEmpty())
            {
                foreach (var x in tables)
                {
                    var shdrModel = new ShdrTable(new TableObservationInput(x));
                    var shdrLine = shdrModel.ToString();
                    if (!WriteLine(shdrLine)) return false;
                }
            }

            // TimeSeries
            var timeSeries = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.TimeSeries);
            if (!timeSeries.IsNullOrEmpty())
            {
                foreach (var x in timeSeries)
                {
                    var shdrModel = new ShdrTimeSeries(new TimeSeriesObservationInput(x));
                    var shdrLine = shdrModel.ToString();
                    if (!WriteLine(shdrLine)) return false;
                }
            }

            return true;
        }

        private bool WriteAssets(IEnumerable<IAssetInput> assets)
        {
            foreach (var asset in assets)
            {
                var shdrModel = new ShdrAsset(asset.Asset);
                var shdrLine = shdrModel.ToString(MultilineAssets);
                if (!WriteLine(shdrLine)) return false;
            }

            return true;
        }

        private bool WriteDevices(IEnumerable<IDeviceInput> devices)
        {
            foreach (var device in devices)
            {
                var shdrModel = new ShdrDevice(device.Device);
                var shdrLine = shdrModel.ToString(MultilineDevices);
                if (!WriteLine(shdrLine)) return false;
            }

            return true;
        }


        /// <summary>Writes a fully-formatted SHDR <paramref name="line"/> (terminator included) to every connected agent and returns <c>true</c> when at least one write succeeded.</summary>
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
                            stream.ReadTimeout = Timeout;
                            stream.WriteTimeout = Timeout;

                            // Write the line (in bytes) to the Stream
                            stream.Write(bytes, 0, bytes.Length);

                            MulticastIsolation.Raise(LineSent, this, new AdapterEventArgs<string>(client.Id, singleLine), null);
                        }
                        catch (Exception ex)
                        {
                            MulticastIsolation.Raise(SendError, this, new AdapterEventArgs<string>(client.Id, ex.Message), null);
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private async Task<bool> WriteLineToClientAsync(AgentClient client, string line)
        {
            if (client != null)
            {
                try
                {
                    // Convert string to ASCII bytes and add line terminator
                    var bytes = Encoding.ASCII.GetBytes(line + "\n");

                    // Get the TcpClient Stream
                    var stream = client.TcpClient.GetStream();
                    stream.ReadTimeout = Timeout;
                    stream.WriteTimeout = Timeout;

                    // Write the line (in bytes) to the Stream
                    await stream.WriteAsync(bytes, 0, bytes.Length);

                    MulticastIsolation.Raise(LineSent, this, new AdapterEventArgs<string>(client.Id, line), null);

                    return true;
                }
                catch (Exception ex)
                {
                    MulticastIsolation.Raise(SendError, this, new AdapterEventArgs<string>(client.Id, ex.Message), null);
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


        #region "DataItems"

        /// <summary>Override-point invoked when an SHDR data-item observation is added to the buffered queue; subclasses can use it to tap, log, or rewrite observations before they are flushed.</summary>
        protected virtual void OnDataItemAdd(ShdrDataItem dataItem) { }


        /// <summary>Adds a scalar Sample/Event observation for <paramref name="dataItemKey"/> with <paramref name="value"/> and the current Unix time as the timestamp.</summary>
        public void AddDataItem(string dataItemKey, object value)
        {
            AddDataItem(dataItemKey, value, UnixDateTime.Now);
        }

        /// <summary>Adds a scalar Sample/Event observation with an explicit <paramref name="timestamp"/> (converted to Unix UTC time).</summary>
        public void AddDataItem(string dataItemKey, object value, DateTime timestamp)
        {
            AddDataItem(dataItemKey, value, timestamp.ToUnixUtcTime());
        }

        /// <summary>Adds a scalar Sample/Event observation with an explicit Unix-time <paramref name="timestamp"/> (milliseconds since epoch).</summary>
        public void AddDataItem(string dataItemKey, object value, long timestamp)
        {
            AddDataItem(new ShdrDataItem(dataItemKey, value, timestamp));
        }

        /// <summary>Adds a pre-built <see cref="ShdrDataItem"/> to the buffered queue.</summary>
        public void AddDataItem(ShdrDataItem dataItem)
        {
            AddDataItem((IObservationInput)dataItem);
        }

        /// <summary>Adds a batch of <see cref="ShdrDataItem"/> objects to the buffered queue.</summary>
        public void AddDataItems(IEnumerable<ShdrDataItem> dataItems)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                foreach (var dataItem in dataItems)
                {
                    AddDataItem(dataItem);
                }
            }
        }

        /// <summary>Adds an arbitrary <see cref="IObservationInput"/> to the buffered queue (used by SHDR data set/table/time-series observations that all flow through the same buffer).</summary>
        public void AddDataItem(IObservationInput observation)
        {
            _adapter.AddObservation(observation);
        }


        /// <summary>Immediately writes a scalar Sample/Event observation to every connected agent and returns <c>true</c> when all writes succeeded.</summary>
        public bool SendDataItem(string dataItemKey, object value)
        {
            return SendDataItem(dataItemKey, value, UnixDateTime.Now);
        }

        /// <summary>Sends a scalar Sample/Event observation with an explicit <paramref name="timestamp"/> (converted to Unix UTC time).</summary>
        public bool SendDataItem(string dataItemKey, object value, DateTime timestamp)
        {
            return SendDataItem(dataItemKey, value, timestamp.ToUnixUtcTime());
        }

        /// <summary>Sends a scalar Sample/Event observation with an explicit Unix-time <paramref name="timestamp"/>.</summary>
        public bool SendDataItem(string dataItemKey, object value, long timestamp)
        {
            return SendDataItem(new ShdrDataItem(dataItemKey, value, timestamp));
        }

        /// <summary>Sends a pre-built <see cref="ShdrDataItem"/> immediately.</summary>
        public bool SendDataItem(ShdrDataItem dataItem)
        {
            return SendDataItem((IObservationInput)dataItem);
        }

        /// <summary>Sends a batch of <see cref="ShdrDataItem"/> objects immediately; returns <c>true</c> only when every observation was written successfully.</summary>
        public bool SendDataItems(IEnumerable<ShdrDataItem> dataItems)
        {
            var success = true;

            if (!dataItems.IsNullOrEmpty())
            {
                foreach (var dataItem in dataItems)
                {
                    if (!SendDataItem(dataItem)) success = false;
                }

                return success;
            }

            return false;
        }

        /// <summary>Sends an arbitrary <see cref="IObservationInput"/> immediately; the common entry point used by the data-set/table/time-series send overloads.</summary>
        public bool SendDataItem(IObservationInput observation)
        {
            return _adapter.SendObservation(observation);
        }

        #endregion

        #region "Messages"

        /// <summary>Override-point invoked when an SHDR Message event is added to the buffered queue.</summary>
        protected virtual void OnMessageAdd(ShdrMessage message) { }


        /// <summary>Adds a Message event for <paramref name="messageId"/> with the supplied text <paramref name="value"/> using the current Unix time.</summary>
        public void AddMessage(string messageId, string value)
        {
            AddMessage(messageId, value, UnixDateTime.Now);
        }

        /// <summary>Adds a Message event with an explicit <paramref name="timestamp"/> (converted to Unix UTC time).</summary>
        public void AddMessage(string messageId, string value, DateTime timestamp)
        {
            AddMessage(messageId, value, timestamp.ToUnixUtcTime());
        }

        /// <summary>Adds a Message event with an explicit Unix-time <paramref name="timestamp"/>.</summary>
        public void AddMessage(string messageId, string value, long timestamp)
        {
            AddMessage(new ShdrMessage(messageId, value, timestamp));
        }

        /// <summary>Adds a Message event with a controller-supplied <paramref name="nativeCode"/> using the current Unix time.</summary>
        public void AddMessage(string messageId, string value, string nativeCode)
        {
            AddMessage(messageId, value, nativeCode, UnixDateTime.Now);
        }

        /// <summary>Adds a Message event with native code and explicit <paramref name="timestamp"/> (converted to Unix UTC time).</summary>
        public void AddMessage(string messageId, string value, string nativeCode, DateTime timestamp)
        {
            AddMessage(messageId, value, nativeCode, timestamp.ToUnixUtcTime());
        }

        /// <summary>Adds a Message event with native code and an explicit Unix-time <paramref name="timestamp"/>.</summary>
        public void AddMessage(string messageId, string value, string nativeCode, long timestamp)
        {
            AddMessage(new ShdrMessage(messageId, value, nativeCode, timestamp));
        }

        /// <summary>Adds a pre-built <see cref="ShdrMessage"/> to the buffered queue.</summary>
        public void AddMessage(ShdrMessage message)
        {
            _adapter.AddObservation(message);
        }

        /// <summary>Adds a batch of Message events to the buffered queue.</summary>
        public void AddMessages(IEnumerable<ShdrMessage> messages)
        {
            if (!messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    AddMessage(message);
                }
            }
        }


        /// <summary>Immediately writes a Message event using the current Unix time.</summary>
        public bool SendMessage(string dataItemId, string value)
        {
            return SendMessage(dataItemId, value, UnixDateTime.Now);
        }

        /// <summary>Sends a Message event with an explicit <paramref name="timestamp"/>.</summary>
        public bool SendMessage(string dataItemId, string value, DateTime timestamp)
        {
            return SendMessage(dataItemId, value, timestamp.ToUnixUtcTime());
        }

        /// <summary>Sends a Message event with an explicit Unix-time <paramref name="timestamp"/>.</summary>
        public bool SendMessage(string dataItemId, string value, long timestamp)
        {
            return SendMessage(new ShdrMessage(dataItemId, value, timestamp));
        }

        /// <summary>Sends a Message event with a controller-supplied <paramref name="nativeCode"/> using the current Unix time.</summary>
        public bool SendMessage(string dataItemId, string value, string nativeCode)
        {
            return SendMessage(dataItemId, value, nativeCode, UnixDateTime.Now);
        }

        /// <summary>Sends a Message event with native code and explicit <paramref name="timestamp"/>.</summary>
        public bool SendMessage(string dataItemId, string value, string nativeCode, DateTime timestamp)
        {
            return SendMessage(dataItemId, value, nativeCode, timestamp.ToUnixUtcTime());
        }

        /// <summary>Sends a Message event with native code and an explicit Unix-time <paramref name="timestamp"/>.</summary>
        public bool SendMessage(string dataItemId, string value, string nativeCode, long timestamp)
        {
            return SendMessage(new ShdrMessage(dataItemId, value, nativeCode, timestamp));
        }

        /// <summary>Sends a pre-built <see cref="ShdrMessage"/> immediately.</summary>
        public bool SendMessage(ShdrMessage message)
        {
            return _adapter.SendObservation(message);
        }

        /// <summary>Sends a batch of Message events immediately; returns <c>true</c> when every message wrote successfully.</summary>
        public bool SendMessages(IEnumerable<ShdrMessage> messages)
        {
            var success = true;

            if (!messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    if (!SendDataItem(message)) success = false;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "Conditions"

        /// <summary>Override-point invoked when an SHDR Condition is added to the buffered queue.</summary>
        protected virtual void OnConditionAdd(ShdrCondition condition) { }


        /// <summary>Adds every fault state of <paramref name="condition"/> to the buffered queue; the SHDR adapter emits one line per fault state so multi-state conditions remain individually addressable.</summary>
        public void AddCondition(ShdrCondition condition)
        {
            if (condition != null && !condition.FaultStates.IsNullOrEmpty())
            {
                foreach (var faultState in condition.FaultStates)
                {
                    _adapter.AddObservation(faultState);
                }
            }
        }

        /// <summary>Adds a batch of <see cref="ShdrCondition"/> objects, flattening each into its fault states.</summary>
        public void AddConditions(IEnumerable<ShdrCondition> conditions)
        {
            if (!conditions.IsNullOrEmpty())
            {
                foreach (var condition in conditions)
                {
                    AddCondition(condition);
                }
            }
        }


        /// <summary>Immediately writes the first fault state of <paramref name="condition"/> to every connected agent; the early-return behaviour exists to match the original send-on-first-state contract.</summary>
        public bool SendCondition(ShdrCondition condition)
        {
            if (condition != null && !condition.FaultStates.IsNullOrEmpty())
            {
                foreach (var faultState in condition.FaultStates)
                {
                    return _adapter.SendObservation(faultState);
                }
            }

            return false;
        }

        /// <summary>Sends a batch of <see cref="ShdrCondition"/> objects immediately; returns <c>true</c> when every send succeeded.</summary>
        public bool SendConditions(IEnumerable<ShdrCondition> conditions)
        {
            var success = true;

            if (!conditions.IsNullOrEmpty())
            {
                foreach (var condition in conditions)
                {
                    if (!SendCondition(condition)) success = false;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "TimeSeries"

        /// <summary>Override-point invoked when an SHDR TimeSeries observation is added to the buffered queue.</summary>
        protected virtual void OnTimeSeriesAdd(ShdrTimeSeries timeSeries) { }


        /// <summary>Adds a TimeSeries observation to the buffered queue.</summary>
        public void AddTimeSeries(ShdrTimeSeries timeSeries)
        {
            _adapter.AddObservation(timeSeries);
        }

        /// <summary>Adds a batch of TimeSeries observations to the buffered queue.</summary>
        public void AddTimeSeries(IEnumerable<ShdrTimeSeries> timeSeries)
        {
            if (!timeSeries.IsNullOrEmpty())
            {
                foreach (var item in timeSeries)
                {
                    AddTimeSeries(item);
                }
            }
        }


        /// <summary>Immediately writes a TimeSeries observation to every connected agent.</summary>
        public bool SendTimeSeries(ShdrTimeSeries timeSeries)
        {
            return _adapter.SendObservation(timeSeries);
        }

        /// <summary>Sends a batch of TimeSeries observations immediately; returns <c>true</c> only when every observation was written successfully.</summary>
        public bool SendTimeSeries(IEnumerable<ShdrTimeSeries> timeSeries)
        {
            var success = true;

            if (!timeSeries.IsNullOrEmpty())
            {
                foreach (var item in timeSeries)
                {
                    if (!SendTimeSeries(item)) success = false;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "DataSet"

        /// <summary>Override-point invoked when an SHDR DataSet observation is added to the buffered queue.</summary>
        protected virtual void OnDataSetAdd(ShdrDataSet dataSet) { }


        /// <summary>Adds a DataSet observation to the buffered queue.</summary>
        public void AddDataSet(ShdrDataSet dataSet)
        {
            _adapter.AddObservation(dataSet);
        }

        /// <summary>Adds a batch of DataSet observations to the buffered queue.</summary>
        public void AddDataSets(IEnumerable<ShdrDataSet> dataSets)
        {
            if (!dataSets.IsNullOrEmpty())
            {
                foreach (var item in dataSets)
                {
                    AddDataSet(item);
                }
            }
        }


        /// <summary>Immediately writes a DataSet observation to every connected agent.</summary>
        public bool SendDataSet(ShdrDataSet dataSet)
        {
            return _adapter.SendObservation(dataSet);
        }

        /// <summary>Sends a batch of DataSet observations immediately; returns <c>true</c> only when every observation was written successfully.</summary>
        public bool SendDataSets(IEnumerable<ShdrDataSet> dataSets)
        {
            var success = true;

            if (!dataSets.IsNullOrEmpty())
            {
                foreach (var item in dataSets)
                {
                    if (!SendDataSet(item)) success = false;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "Table"

        /// <summary>Override-point invoked when an SHDR Table observation is added to the buffered queue.</summary>
        protected virtual void OnTableAdd(ShdrTable table) { }


        /// <summary>Adds a Table observation to the buffered queue.</summary>
        public void AddTable(ShdrTable table)
        {
            _adapter.AddObservation(table);
        }

        /// <summary>Adds a batch of Table observations to the buffered queue.</summary>
        public void AddTables(IEnumerable<ShdrTable> tables)
        {
            if (!tables.IsNullOrEmpty())
            {
                foreach (var item in tables)
                {
                    AddTable(item);
                }
            }
        }


        /// <summary>Immediately writes a Table observation to every connected agent.</summary>
        public bool SendTable(ShdrTable table)
        {
            return _adapter.SendObservation(table);
        }

        /// <summary>Sends a batch of Table observations immediately; returns <c>true</c> only when every observation was written successfully.</summary>
        public bool SendTables(IEnumerable<ShdrTable> tables)
        {
            var success = true;

            if (!tables.IsNullOrEmpty())
            {
                foreach (var item in tables)
                {
                    if (!SendTable(item)) success = false;
                }

                return success;
            }

            return false;
        }

        #endregion


        #region "Assets"

        /// <summary>Override-point invoked when an SHDR Asset is staged for publish; subclasses can rewrite or annotate the asset before it is serialised onto the wire.</summary>
        protected virtual void OnAssetAdd(ShdrAsset asset) { }


        /// <summary>
        /// Add the specified MTConnect Asset
        /// </summary>
        /// <param name="asset">The Asset to add</param>
        public void AddAsset(IAsset asset)
        {
            if (asset != null)
            {
                _adapter.AddAsset(new AssetInput(asset));
            }
        }

        /// <summary>
        /// Add the specified MTConnect Assets
        /// </summary>
        /// <param name="assets">The Assets to add</param>
        public void AddAssets(IEnumerable<IAsset> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                foreach (var item in assets)
                {
                    AddAsset(item);
                }
            }
        }


        /// <summary>
        /// Add the specified MTConnect Asset and sends it to the Agent
        /// </summary>
        /// <param name="asset">The Asset to send</param>
        public void SendAsset(IAsset asset)
        {
            if (asset != null)
            {
                _adapter.AddAsset(new AssetInput(asset));
            }
        }

        /// <summary>
        /// Add the specified MTConnect Asset and sends it to the Agent
        /// </summary>
        /// <param name="asset">The Asset to send</param>
        private void SendAsset(ShdrAsset asset)
        {
            if (asset != null && asset.Asset != null)
            {
                SendAsset(asset.Asset);
            }
        }

        /// <summary>
        /// Add the specified MTConnect Assets and sends them to the Agent
        /// </summary>
        /// <param name="assets">The Assets to send</param>
        public void SendAssets(IEnumerable<IAsset> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                foreach (var item in assets)
                {
                    SendAsset(item);
                }
            }
        }


        /// <summary>
        /// Remove the specified Asset using the SHDR command @REMOVE_ASSET@
        /// </summary>
        /// <param name="assetId">The AssetId of the Asset to remove</param>
        /// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        public void RemoveAsset(string assetId, long timestamp = 0)
        {
            // Create SHDR string to send
            var shdrLine = ShdrAsset.Remove(assetId, timestamp);

            // Write line to stream
            WriteLine(shdrLine);
        }

        /// <summary>
        /// Remove all Assets of the specified Type using the SHDR command @REMOVE_ALL_ASSETS@
        /// </summary>
        /// <param name="assetType">The Type of the Assets to remove</param>
        /// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        public void RemoveAllAssets(string assetType, long timestamp = 0)
        {
            // Create SHDR string to send
            var shdrLine = ShdrAsset.RemoveAll(assetType, timestamp);

            // Write line to stream
            WriteLine(shdrLine);
        }

        #endregion

        #region "Devices"

        /// <summary>
        /// Add the specified MTConnect Device to the queue to be written to the adapter stream
        /// </summary>
        /// <param name="device">The Device to add</param>
        public void AddDevice(IDevice device)
        {
            if (device != null)
            {
                _adapter.AddDevice(new DeviceInput(device));
            }
        }

        /// <summary>
        /// Add the specified MTConnect Devices to the queue to be written to the adapter stream
        /// </summary>
        /// <param name="devices">The Devices to add</param>
        public void AddDevices(IEnumerable<IDevice> devices)
        {
            if (!devices.IsNullOrEmpty())
            {
                foreach (var item in devices)
                {
                    AddDevice(item);
                }
            }
        }


        /// <summary>
        /// Remove the specified Device using the SHDR command @REMOVE_ASSET@
        /// </summary>
        /// <param name="deviceId">The DeviceId of the Device to remove</param>
        /// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        public void RemoveDevice(string deviceId, long timestamp = 0)
        {
            // Create SHDR string to send
            var shdrLine = ShdrDevice.Remove(deviceId, timestamp);

            // Write line to stream
            WriteLine(shdrLine);
        }

        /// <summary>
        /// Remove all Devices of the specified Type using the SHDR command @REMOVE_ALL_ASSETS@
        /// </summary>
        /// <param name="deviceType">The Type of the Devices to remove</param>
        /// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        public void RemoveAllDevices(string deviceType, long timestamp = 0)
        {
            // Create SHDR string to send
            var shdrLine = ShdrDevice.RemoveAll(deviceType, timestamp);

            // Write line to stream
            WriteLine(shdrLine);
        }

        #endregion

    }
}