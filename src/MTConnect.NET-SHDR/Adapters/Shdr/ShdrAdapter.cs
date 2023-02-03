// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Observations.Input;
using MTConnect.Shdr;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// An Adapter class for communicating with an MTConnect Agent using the SHDR protocol.
    /// Supports multiple concurrent Agent connections.
    /// Uses a queue to collect changes to Observations and sends the most recent changes at the specified interval.
    /// </summary>
    public class ShdrAdapter
    {
        private readonly object _lock = new object();
        private readonly AgentClientConnectionListener _connectionListener;
        private readonly Dictionary<string, AgentClient> _clients = new Dictionary<string, AgentClient>();

        private readonly Dictionary<string, ShdrDataItem> _currentDataItems = new Dictionary<string, ShdrDataItem>();
        private readonly Dictionary<string, ShdrDataItem> _lastDataItems = new Dictionary<string, ShdrDataItem>();

        private readonly Dictionary<string, ShdrMessage> _currentMessages = new Dictionary<string, ShdrMessage>();
        private readonly Dictionary<string, ShdrMessage> _lastMessages = new Dictionary<string, ShdrMessage>();

        private readonly Dictionary<string, ShdrCondition> _currentConditions = new Dictionary<string, ShdrCondition>();
        private readonly Dictionary<string, ShdrCondition> _lastConditions = new Dictionary<string, ShdrCondition>();

        private readonly Dictionary<string, ShdrTimeSeries> _currentTimeSeries = new Dictionary<string, ShdrTimeSeries>();
        private readonly Dictionary<string, ShdrTimeSeries> _lastTimeSeries = new Dictionary<string, ShdrTimeSeries>();

        private readonly Dictionary<string, ShdrDataSet> _currentDataSets = new Dictionary<string, ShdrDataSet>();
        private readonly Dictionary<string, ShdrDataSet> _lastDataSets = new Dictionary<string, ShdrDataSet>();

        private readonly Dictionary<string, ShdrTable> _currentTables = new Dictionary<string, ShdrTable>();
        private readonly Dictionary<string, ShdrTable> _lastTables = new Dictionary<string, ShdrTable>();

        private readonly Dictionary<string, ShdrAsset> _currentAssets = new Dictionary<string, ShdrAsset>();
        private readonly Dictionary<string, ShdrAsset> _lastAssets = new Dictionary<string, ShdrAsset>();

        private readonly Dictionary<string, ShdrDevice> _devices = new Dictionary<string, ShdrDevice>();


        private CancellationTokenSource _stop;
        protected CancellationTokenSource StopToken => _stop;


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
        public EventHandler<string> AgentConnected { get; set; }

        /// <summary>
        /// Raised when an existing Agent connection is disconnected. Includes the AgentClient ID as an argument.
        /// </summary>
        public EventHandler<string> AgentDisconnected { get; set; }

        /// <summary>
        /// Raised when an error occurs during an existing Agent connection. Includes the AgentClient ID as an argument.
        /// </summary>
        public EventHandler<string> AgentConnectionError { get; set; }


        /// <summary>
        /// Raised when a Ping request message is received from an Agent. Includes the AgentClient ID as an argument.
        /// </summary>
        public EventHandler<string> PingReceived { get; set; }

        /// <summary>
        /// Raised when a Pong response message is sent to an Agent. Includes the AgentClient ID as an argument.
        /// </summary>
        public EventHandler<string> PongSent { get; set; }

        /// <summary>
        /// Raised when a new line is sent to the Agent. Includes the AgentClient ID and the Line sent as an argument.
        /// </summary>
        public EventHandler<AdapterEventArgs> LineSent { get; set; }

        /// <summary>
        /// Raised when an error occurs when sending a new line to the Agent. Includes the AgentClient ID and the Error message as an argument.
        /// </summary>
        public EventHandler<AdapterEventArgs> SendError { get; set; }


        public ShdrAdapter(int port = 7878, int heartbeat = 10000)
        {
            FilterDuplicates = true;
            OutputTimestamps = true;
            Port = port;
            Heartbeat = heartbeat;
            Timeout = 5000;

            _connectionListener = new AgentClientConnectionListener(Port, heartbeat);
            _connectionListener.ClientConnected += ClientConnected;
            _connectionListener.ClientDisconnected += ClientDisconnected;
            _connectionListener.ClientPingReceived += ClientPingReceived;
            _connectionListener.ClientPongSent += ClientPongSent;
        }

        public ShdrAdapter(string deviceKey, int port = 7878, int heartbeat = 10000)
        {
            FilterDuplicates = true;
            OutputTimestamps = true;
            DeviceKey = deviceKey;
            Port = port;
            Heartbeat = heartbeat;
            Timeout = 5000;

            _connectionListener = new AgentClientConnectionListener(Port, heartbeat);
            _connectionListener.ClientConnected += ClientConnected;
            _connectionListener.ClientDisconnected += ClientDisconnected;
            _connectionListener.ClientPingReceived += ClientPingReceived;
            _connectionListener.ClientPongSent += ClientPongSent;
        }

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

                _connectionListener = new AgentClientConnectionListener(Port, Heartbeat);
                _connectionListener.ClientConnected += ClientConnected;
                _connectionListener.ClientDisconnected += ClientDisconnected;
                _connectionListener.ClientPingReceived += ClientPingReceived;
                _connectionListener.ClientPongSent += ClientPongSent;
            }
        }


        /// <summary>
        /// Starts the Adapter to begins listening for Agent connections as well as starts the Queue for collecting and sending data to the Agent(s).
        /// </summary>
        public void Start()
        {
            _stop = new CancellationTokenSource();

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

            // Call Overridable Method
            OnStop();
        }


        protected virtual void OnStart() { }

        protected virtual void OnStop() { }


        /// <summary>
        /// Set all items to Unavailable
        /// </summary>
        public void SetUnavailable(long timestamp = 0)
        {
            SetDataItemsUnavailable(timestamp);
            SetMessagesUnavailable(timestamp);
            SetConditionsUnavailable(timestamp);
            SetTimeSeriesUnavailable(timestamp);
            SetDataSetsUnavailable(timestamp);
            SetTablesUnavailable(timestamp);
        }


        #region "Event Handlers"

        private void ClientConnected(string clientId, TcpClient client)
        {
            AddAgentClient(clientId, client);
            AgentConnected?.Invoke(this, clientId);

            SendLast(UnixDateTime.Now);
        }

        private void ClientDisconnected(string clientId)
        {
            RemoveAgentClient(clientId);
            AgentDisconnected?.Invoke(this, clientId);
        }

        private void ClientPingReceived(string clientId)
        {
            PingReceived?.Invoke(this, clientId);
        }

        private void ClientPongSent(string clientId)
        {
            PongSent?.Invoke(this, clientId);
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
        public void SendChanged()
        {
            WriteChangedDataItems();
            WriteChangedMessages();
            WriteChangedConditions();
            WriteChangedTimeSeries();
            WriteChangedDataSets();
            WriteChangedTables();
            WriteChangedAssets();

            // Call Overridable Method
            OnChangedSent();
        }

        /// <summary>
        /// Sends all of the last sent Items, Assets, and Devices to the Agent. This can be used upon reconnection to the Agent
        /// </summary>
        public void SendLast(long timestamp = 0)
        {
            WriteLastDataItems(timestamp);
            WriteLastMessages(timestamp);
            WriteLastConditions(timestamp);
            WriteLastTimeSeries(timestamp);
            WriteLastDataSets(timestamp);
            WriteLastTables(timestamp);
            WriteAllAssets();
            WriteAllDevices();

            // Call Overridable Method
            OnLastSent();
        }


        protected virtual void OnChangedSent() { }

        protected virtual void OnLastSent() { }

        #endregion

        #region "Write"

        protected bool WriteLine(string line)
        {
            if (!string.IsNullOrEmpty(line))
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

            return false;
        }

        private bool WriteLine(string clientId, string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                var client = GetAgentClient(clientId);
                if (client != null)
                {
                    return WriteLineToClient(client, line);
                }
            }

            return false;
        }

        private async Task<bool> WriteLineAsync(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                // Write Line to each client in stored client list
                var clients = GetAgentClients();
                if (!clients.IsNullOrEmpty())
                {
                    foreach (var client in clients)
                    {
                        await WriteLineToClientAsync(client, line);
                    }

                    return true;
                }
            }

            return false;
        }

        private async Task<bool> WriteLineAsync(string clientId, string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                var client = GetAgentClient(clientId);
                if (client != null)
                {
                    return await WriteLineToClientAsync(client, line);
                }
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

                            LineSent?.Invoke(this, new AdapterEventArgs(client.Id, singleLine));
                        }
                        catch (Exception ex)
                        {
                            SendError?.Invoke(this, new AdapterEventArgs(client.Id, ex.Message));
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

                    LineSent?.Invoke(this, new AdapterEventArgs(client.Id, line));

                    return true;
                }
                catch (Exception ex)
                {
                    SendError?.Invoke(this, new AdapterEventArgs(client.Id, ex.Message));
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

        protected virtual void OnDataItemAdd(ShdrDataItem dataItem) { }


        public void AddDataItem(string dataItemId, object value)
        {
            AddDataItem(dataItemId, value, UnixDateTime.Now);
        }

        public void AddDataItem(string dataItemId, object value, DateTime timestamp)
        {
            AddDataItem(dataItemId, value, timestamp.ToUnixTime());
        }

        public void AddDataItem(string dataItemId, object value, long timestamp)
        {
            AddDataItem(new ShdrDataItem(dataItemId, value, timestamp));
        }

        public void AddDataItem(ObservationInput observation)
        {
            AddDataItem(new ShdrDataItem(observation));
        }

        public void AddDataItem(ShdrDataItem dataItem)
        {
            if (dataItem != null)
            {
                var newDataItem = new ShdrDataItem(dataItem);

                // Set the DeviceKey
                newDataItem.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (newDataItem.Timestamp <= 0) newDataItem.Timestamp = UnixDateTime.Now;

                // Get the Current Observation (if exists)
                ShdrDataItem currentDataItem;
                lock (_lock) _currentDataItems.TryGetValue(newDataItem.DataItemKey, out currentDataItem);

                // Check to see if new Observation is the same as the Current
                var add = true;
                if (currentDataItem != null && FilterDuplicates)
                {
                    add = !ObjectExtensions.ByteArraysEqual(newDataItem.ChangeId, currentDataItem.ChangeId);
                }

                if (add)
                {
                    // Add to Current
                    lock (_lock)
                    {
                        _currentDataItems.Remove(newDataItem.DataItemKey);
                        _currentDataItems.Add(newDataItem.DataItemKey, newDataItem);
                    }

                    // Call Overridable Method
                    OnDataItemAdd(newDataItem);
                }
            }
        }

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


        public bool SendDataItem(string dataItemId, object value)
        {
            return SendDataItem(dataItemId, value, UnixDateTime.Now);
        }

        public bool SendDataItem(string dataItemId, object value, DateTime timestamp)
        {
            return SendDataItem(dataItemId, value, timestamp.ToUnixTime());
        }

        public bool SendDataItem(string dataItemId, object value, long timestamp)
        {
            return SendDataItem(new ShdrDataItem(dataItemId, value, timestamp));
        }

        public bool SendDataItem(ObservationInput observation)
        {
            return SendDataItem(new ShdrDataItem(observation));
        }

        public bool SendDataItem(ShdrDataItem dataItem)
        {
            if (dataItem != null)
            {
                var newDataItem = new ShdrDataItem(dataItem);

                // Set the DeviceKey
                newDataItem.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (newDataItem.Timestamp <= 0) newDataItem.Timestamp = UnixDateTime.Now;
                //if (!OutputTimestamps) newDataItem.Timestamp = 0;
                //else /*if (newDataItem.Timestamp <= 0) newDataItem.Timestamp = UnixDateTime.Now;*/

                // Remove from Current
                lock (_lock) _currentDataItems.Remove(newDataItem.DataItemKey);

                // Call Overridable Method
                OnDataItemAdd(newDataItem);

                // Create SHDR string to send
                var sendItem = new ShdrDataItem(newDataItem);
                if (!OutputTimestamps) sendItem.Timestamp = 0;
                var shdrLine = sendItem.ToString();

                var success = WriteLine(shdrLine);
                if (success)
                {
                    // Update Last Sent DataItems
                    UpdateLastDataItems(new List<ShdrDataItem> { newDataItem });
                }

                return success;
            }

            return false;
        }

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


        protected void UpdateLastDataItems(IEnumerable<ShdrDataItem> dataItems)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                // Find the most recent Observation for each DataItemKey
                var dataItemKeys = dataItems.Select(o => o.DataItemKey).Distinct();
                foreach (var dataItemKey in dataItemKeys)
                {
                    var keyDataItems = dataItems.Where(o => o.DataItemKey == dataItemKey);
                    var mostRecent = keyDataItems.OrderByDescending(o => o.Timestamp).FirstOrDefault();
                    if (mostRecent != null)
                    {               
                        var lastDataItem = new ShdrDataItem(mostRecent);

                        lock (_lock)
                        {
                            _lastDataItems.Remove(lastDataItem.DataItemKey);
                            _lastDataItems.Add(lastDataItem.DataItemKey, lastDataItem);
                        }
                    }
                }
            }
        }


        protected bool WriteChangedDataItems()
        {
            // Get a list of all Current DataItems
            List<ShdrDataItem> dataItems;
            lock (_lock)
            {
                // Get List of DataItems that need to be Updated
                dataItems = new List<ShdrDataItem>();
                var items = _currentDataItems.Values;
                foreach (var item in items)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;

                        var sendItem = new ShdrDataItem(item);
                        if (!OutputTimestamps) sendItem.Timestamp = 0;
                        dataItems.Add(sendItem);
                    }
                }
            }

            if (!dataItems.IsNullOrEmpty())
            {
                // Create SHDR string to send
                var shdrLine = ShdrDataItem.ToString(dataItems);

                var success = WriteLine(shdrLine);
                if (success)
                {
                    // Update Last Sent DataItems
                    UpdateLastDataItems(dataItems);
                }

                return success;
            }

            return false;
        }

        protected bool WriteLastDataItems(long timestamp = 0)
        {
            // Get a list of all Last DataItems
            IEnumerable<ShdrDataItem> dataItems;
            lock (_lock) dataItems = _lastDataItems.Values.ToList();

            if (!dataItems.IsNullOrEmpty())
            {
                var sendItems = new List<ShdrDataItem>();
                foreach (var dataItem in dataItems)
                {
                    var sendItem = new ShdrDataItem(dataItem);
                    if (!OutputTimestamps) sendItem.Timestamp = 0;
                    sendItems.Add(sendItem);
                }

                // Create SHDR string to send
                var shdrLine = ShdrDataItem.ToString(sendItems);
                var success = WriteLine(shdrLine);
                if (success)
                {
                    // Update Last Sent DataItems
                    UpdateLastDataItems(dataItems);
                }

                return success;
            }

            return false;
        }


        private void SetDataItemsUnavailable(long timestamp = 0)
        {
            // Get a list of all Current DataItems
            IEnumerable<ShdrDataItem> dataItems;
            lock (_lock) dataItems = _currentDataItems.Values.ToList();

            if (!dataItems.IsNullOrEmpty())
            {
                var unavailableObservations = new List<ShdrDataItem>();
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Set each Observation to Unavailable
                foreach (var item in dataItems)
                {
                    // Create new Unavailable Observation
                    var unavailableObservation = new ShdrDataItem();
                    unavailableObservation.DeviceKey = item.DeviceKey;
                    unavailableObservation.DataItemKey = item.DataItemKey;
                    unavailableObservation.Timestamp = ts;
                    unavailableObservation.Unavailable();
                    unavailableObservations.Add(unavailableObservation);
                }

                // Add Observations (only will add those that are changed)
                AddDataItems(unavailableObservations);
            }
        }

        #endregion

        #region "Messages"

        protected virtual void OnMessageAdd(ShdrMessage message) { }


        public void AddMessage(string messageId, string value)
        {
            AddMessage(messageId, value, UnixDateTime.Now);
        }

        public void AddMessage(string messageId, string value, DateTime timestamp)
        {
            AddMessage(messageId, value, timestamp.ToUnixTime());
        }

        public void AddMessage(string messageId, string value, long timestamp)
        {
            AddMessage(new ShdrMessage(messageId, value, timestamp));
        }

        public void AddMessage(string messageId, string value, string nativeCode)
        {
            AddMessage(messageId, value, nativeCode, UnixDateTime.Now);
        }

        public void AddMessage(string messageId, string value, string nativeCode, DateTime timestamp)
        {
            AddMessage(messageId, value, nativeCode, timestamp.ToUnixTime());
        }

        public void AddMessage(string messageId, string value, string nativeCode, long timestamp)
        {
            AddMessage(new ShdrMessage(messageId, value, nativeCode, timestamp));
        }

        public void AddMessage(ShdrMessage message)
        {
            if (message != null)
            {
                var newMessage = new ShdrMessage(message);

                // Set the DeviceKey
                newMessage.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (!OutputTimestamps) newMessage.Timestamp = 0;
                else if (newMessage.Timestamp <= 0) newMessage.Timestamp = UnixDateTime.Now;

                // Get the Current Observation (if exists)
                ShdrMessage currentMessage;
                lock (_lock) _currentMessages.TryGetValue(newMessage.DataItemKey, out currentMessage);

                // Check to see if new Observation is the same as the Current
                var add = true;
                if (currentMessage != null && FilterDuplicates)
                {
                    add = !ObjectExtensions.ByteArraysEqual(newMessage.ChangeId, currentMessage.ChangeId);
                }

                if (add)
                {
                    // Add to Current
                    lock (_lock)
                    {
                        _currentMessages.Remove(newMessage.DataItemKey);
                        _currentMessages.Add(newMessage.DataItemKey, newMessage);
                    }

                    // Call Overridable Method
                    OnMessageAdd(newMessage);
                }
            }
        }

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


        public bool SendMessage(string dataItemId, string value)
        {
            return SendMessage(dataItemId, value, UnixDateTime.Now);
        }

        public bool SendMessage(string dataItemId, string value, DateTime timestamp)
        {
            return SendMessage(dataItemId, value, timestamp.ToUnixTime());
        }

        public bool SendMessage(string dataItemId, string value, long timestamp)
        {
            return SendMessage(new ShdrMessage(dataItemId, value, timestamp));
        }

        public bool SendMessage(string dataItemId, string value, string nativeCode)
        {
            return SendMessage(dataItemId, value, nativeCode, UnixDateTime.Now);
        }

        public bool SendMessage(string dataItemId, string value, string nativeCode, DateTime timestamp)
        {
            return SendMessage(dataItemId, value, nativeCode, timestamp.ToUnixTime());
        }

        public bool SendMessage(string dataItemId, string value, string nativeCode, long timestamp)
        {
            return SendMessage(new ShdrMessage(dataItemId, value, nativeCode, timestamp));
        }

        public bool SendMessage(ShdrMessage message)
        {
            if (message != null)
            {
                var newMessage = new ShdrMessage(message);

                // Set the DeviceKey
                newMessage.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (!OutputTimestamps) newMessage.Timestamp = 0;
                else if (newMessage.Timestamp <= 0) newMessage.Timestamp = UnixDateTime.Now;

                // Remove from Current
                lock (_lock) _currentMessages.Remove(newMessage.DataItemKey);

                // Call Overridable Method
                OnMessageAdd(newMessage);

                // Create SHDR string to send
                var shdrLine = newMessage.ToString();

                var success = WriteLine(shdrLine);
                if (success)
                {
                    // Update Last Sent Messages
                    UpdateLastMessages(new List<ShdrMessage> { newMessage });
                }

                return success;
            }

            return false;
        }

        public bool SendMessages(IEnumerable<ShdrMessage> messages)
        {
            var success = true;

            if (!messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    if (!SendMessage(message)) success = false;
                }

                return success;
            }

            return false;
        }


        protected void UpdateLastMessages(IEnumerable<ShdrMessage> messages)
        {
            if (!messages.IsNullOrEmpty())
            {
                // Find the most recent Observation for each DataItemKey
                var messageKeys = messages.Select(o => o.DataItemKey).Distinct();
                foreach (var messageKey in messageKeys)
                {
                    var keyMessages = messages.Where(o => o.DataItemKey == messageKey);
                    var mostRecent = keyMessages.OrderByDescending(o => o.Timestamp).FirstOrDefault();

                    lock (_lock)
                    {
                        _lastMessages.Remove(mostRecent.DataItemKey);
                        _lastMessages.Add(mostRecent.DataItemKey, mostRecent);
                    }
                }
            }
        }


        protected bool WriteChangedMessages()
        {
            // Get a list of all Current Messages
            List<ShdrMessage> messages;
            lock (_lock)
            {
                // Get List of Messages that need to be Updated
                messages = new List<ShdrMessage>();
                var items = _currentMessages.Values;
                foreach (var item in items)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;
                        messages.Add(item);
                    }
                }
            }

            if (!messages.IsNullOrEmpty())
            {
                var success = false;

                foreach (var item in messages)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent Messages
                    UpdateLastMessages(messages);
                }

                return success;
            }

            return false;
        }

        protected bool WriteLastMessages(long timestamp = 0)
        {
            // Get a list of all Last Messages
            IEnumerable<ShdrMessage> messages;
            lock (_lock) messages = _lastMessages.Values.ToList();

            if (!messages.IsNullOrEmpty())
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;
                var success = false;

                foreach (var item in messages)
                {
                    item.Timestamp = ts;

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent Messages
                    UpdateLastMessages(messages);
                }

                return success;
            }

            return false;
        }


        private void SetMessagesUnavailable(long timestamp = 0)
        {
            // Get a list of all Current Messages
            IEnumerable<ShdrMessage> messages;
            lock (_lock) messages = _currentMessages.Values.ToList();

            if (!messages.IsNullOrEmpty())
            {
                var unavailableObservations = new List<ShdrMessage>();
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Set each Observation to Unavailable
                foreach (var item in messages)
                {
                    // Create new Unavailable Observation
                    var unavailableObservation = new ShdrMessage();
                    unavailableObservation.DeviceKey = item.DeviceKey;
                    unavailableObservation.DataItemKey = item.DataItemKey;
                    unavailableObservation.Timestamp = ts;
                    unavailableObservation.Unavailable();
                    unavailableObservations.Add(unavailableObservation);
                }

                // Add Observations (only will add those that are changed)
                AddMessages(unavailableObservations);
            }
        }

        #endregion

        #region "Conditions"

        protected virtual void OnConditionAdd(ShdrCondition condition) { }


        public void AddCondition(ShdrCondition condition)
        {
            if (condition != null)
            {
                var newCondition = new ShdrCondition(condition);

                // Set the DeviceKey
                newCondition.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (!newCondition.FaultStates.IsNullOrEmpty())
                {
                    foreach (var faultState in newCondition.FaultStates)
                    {
                        // Set Timestamp (if not already set)
                        if (!OutputTimestamps) faultState.Timestamp = 0;
                        else if (faultState.Timestamp <= 0) faultState.Timestamp = UnixDateTime.Now;
                    }
                }

                // Get the Current Observation (if exists)
                ShdrCondition currentCondition;
                lock (_lock) _currentConditions.TryGetValue(newCondition.DataItemKey, out currentCondition);

                // Check to see if new Observation is the same as the Current
                var add = true;
                if (currentCondition != null && FilterDuplicates)
                {
                    add = !ObjectExtensions.ByteArraysEqual(newCondition.ChangeId, currentCondition.ChangeId);
                }

                if (add)
                {
                    // Add to Current
                    lock (_lock)
                    {
                        _currentConditions.Remove(newCondition.DataItemKey);
                        _currentConditions.Add(newCondition.DataItemKey, newCondition);
                    }

                    // Call Overridable Method
                    OnConditionAdd(newCondition);
                }
            }
        }

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


        public bool SendCondition(ShdrCondition condition)
        {
            if (condition != null)
            {
                var newCondition = new ShdrCondition(condition);

                // Set the DeviceKey
                newCondition.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (!newCondition.FaultStates.IsNullOrEmpty())
                {
                    foreach (var faultState in newCondition.FaultStates)
                    {
                        // Set Timestamp (if not already set)
                        if (!OutputTimestamps) faultState.Timestamp = 0;
                        else if (faultState.Timestamp <= 0) faultState.Timestamp = UnixDateTime.Now;
                    }
                }

                // Remove from Current
                lock (_lock) _currentConditions.Remove(newCondition.DataItemKey);

                // Call Overridable Method
                OnConditionAdd(newCondition);

                // Create SHDR string to send
                var shdrLine = newCondition.ToString();

                var success = WriteLine(shdrLine);
                if (success)
                {
                    // Update Last Sent DataItems
                    UpdateLastConditions(new List<ShdrCondition> { newCondition });
                }

                return success;
            }

            return false;
        }

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


        protected void UpdateLastConditions(IEnumerable<ShdrCondition> conditions)
        {
            if (!conditions.IsNullOrEmpty())
            {
                foreach (var condition in conditions)
                {
                    lock (_lock)
                    {
                        _lastConditions.Remove(condition.DataItemKey);
                        _lastConditions.Add(condition.DataItemKey, condition);
                    }
                }
            }
        }


        protected bool WriteChangedConditions()
        {
            var conditions = new List<ShdrCondition>();
            var faultStates = new List<ShdrFaultState>();

            lock (_lock)
            {
                // Get List of Conditions that need to be Updated
                var items = _currentConditions.Values;
                foreach (var item in items)
                {
                    var add = false;

                    if (!item.FaultStates.IsNullOrEmpty())
                    {
                        foreach (var faultState in item.FaultStates)
                        {
                            if (!faultState.IsSent)
                            {
                                add = true;
                                faultState.IsSent = true;
                                faultStates.Add(faultState);
                            }
                        }
                    }

                    if (add) conditions.Add(item);
                }
            }

            if (!conditions.IsNullOrEmpty() && !faultStates.IsNullOrEmpty())
            {
                var success = false;

                foreach (var item in faultStates)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent Conditions
                    UpdateLastConditions(conditions);
                }

                return success;
            }

            return false;
        }


        protected bool WriteLastConditions(long timestamp = 0)
        {
            // Get a list of all Last Conditions
            IEnumerable<ShdrCondition> conditions;
            lock (_lock) conditions = _lastConditions.Values.ToList();

            if (!conditions.IsNullOrEmpty())
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;
                var success = false;

                foreach (var item in conditions)
                {
                    if (!item.FaultStates.IsNullOrEmpty())
                    {
                        foreach (var faultState in item.FaultStates)
                        {
                            faultState.Timestamp = ts;
                        }
                    }

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent Conditions
                    UpdateLastConditions(conditions);
                }

                return success;
            }

            return false;
        }


        private void SetConditionsUnavailable(long timestamp = 0)
        {
            // Get a list of all Current Conditions
            IEnumerable<ShdrCondition> conditions;
            lock (_lock) conditions = _currentConditions.Values.ToList();

            if (!conditions.IsNullOrEmpty())
            {
                var unavailableConditions = new List<ShdrCondition>();
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Set all of the Conditions to UNAVAILABLE
                foreach (var condition in conditions)
                {
                    var unavailableCondition = new ShdrCondition(condition.DataItemKey);
                    unavailableCondition.DeviceKey = condition.DeviceKey;
                    unavailableCondition.Unavailable(ts);
                    unavailableConditions.Add(unavailableCondition);
                }

                // Add Conditions (only will add those that are changed)
                AddConditions(unavailableConditions);
            }
        }

        #endregion

        #region "TimeSeries"

        protected virtual void OnTimeSeriesAdd(ShdrTimeSeries timeSeries) { }


        public void AddTimeSeries(ShdrTimeSeries timeSeries)
        {
            if (timeSeries != null)
            {
                var newTimeSeries = new ShdrTimeSeries(timeSeries);

                // Set the DeviceKey
                newTimeSeries.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (!OutputTimestamps) newTimeSeries.Timestamp = 0;
                else if (newTimeSeries.Timestamp <= 0) newTimeSeries.Timestamp = UnixDateTime.Now;

                // Get the Current Observation (if exists)
                ShdrTimeSeries currentTimeSeries;
                lock (_lock) _currentTimeSeries.TryGetValue(newTimeSeries.DataItemKey, out currentTimeSeries);

                // Check to see if new Observation is the same as the Current
                var add = true;
                if (currentTimeSeries != null && FilterDuplicates)
                {
                    add = !ObjectExtensions.ByteArraysEqual(newTimeSeries.ChangeId, currentTimeSeries.ChangeId);
                }

                if (add)
                {
                    // Add to Current
                    lock (_lock)
                    {
                        _currentTimeSeries.Remove(newTimeSeries.DataItemKey);
                        _currentTimeSeries.Add(newTimeSeries.DataItemKey, newTimeSeries);
                    }

                    // Call Overridable Method
                    OnTimeSeriesAdd(newTimeSeries);
                }
            }
        }

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


        public bool SendTimeSeries(ShdrTimeSeries timeSeries)
        {
            if (timeSeries != null)
            {
                var newTimeSeries = new ShdrTimeSeries(timeSeries);

                // Set the DeviceKey
                newTimeSeries.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (!OutputTimestamps) newTimeSeries.Timestamp = 0;
                else if (newTimeSeries.Timestamp <= 0) newTimeSeries.Timestamp = UnixDateTime.Now;

                // Remove from Current
                lock (_lock) _currentTimeSeries.Remove(newTimeSeries.DataItemKey);

                // Call Overridable Method
                OnTimeSeriesAdd(newTimeSeries);

                // Create SHDR string to send
                var shdrLine = newTimeSeries.ToString();

                var success = WriteLine(shdrLine);
                if (success)
                {
                    // Update Last Sent TimeSeries
                    UpdateLastTimeSeries(new List<ShdrTimeSeries> { newTimeSeries });
                }

                return success;
            }

            return false;
        }

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


        protected void UpdateLastTimeSeries(IEnumerable<ShdrTimeSeries> timeSeries)
        {
            if (!timeSeries.IsNullOrEmpty())
            {
                // Find the most recent Observation for each DataItemKey
                var timeSeriesKeys = timeSeries.Select(o => o.DataItemKey).Distinct();
                foreach (var timeSeriesKey in timeSeriesKeys)
                {
                    var keyTimeSeriess = timeSeries.Where(o => o.DataItemKey == timeSeriesKey);
                    var mostRecent = keyTimeSeriess.OrderByDescending(o => o.Timestamp).FirstOrDefault();

                    lock (_lock)
                    {
                        _lastTimeSeries.Remove(mostRecent.DataItemKey);
                        _lastTimeSeries.Add(mostRecent.DataItemKey, mostRecent);
                    }
                }
            }
        }


        protected bool WriteChangedTimeSeries()
        {
            // Get a list of all Current TimeSeriess
            List<ShdrTimeSeries> timeSeries;
            lock (_lock)
            {
                // Get List of TimeSeries that need to be Updated
                timeSeries = new List<ShdrTimeSeries>();
                var items = _currentTimeSeries.Values;
                foreach (var item in items)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;
                        timeSeries.Add(item);
                    }
                }
            }

            if (!timeSeries.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var item in timeSeries)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent TimeSeries
                    UpdateLastTimeSeries(timeSeries);
                }

                return success;
            }

            return false;
        }

        protected bool WriteLastTimeSeries(long timestamp = 0)
        {
            // Get a list of all Last TimeSeries
            IEnumerable<ShdrTimeSeries> timeSeries;
            lock (_lock) timeSeries = _lastTimeSeries.Values.ToList();

            if (!timeSeries.IsNullOrEmpty())
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;
                bool success = false;

                foreach (var item in timeSeries)
                {
                    item.Timestamp = ts;

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent TimeSeries
                    UpdateLastTimeSeries(timeSeries);
                }

                return success;
            }

            return false;
        }


        private void SetTimeSeriesUnavailable(long timestamp = 0)
        {
            // Get a list of all Current TimeSeriess
            IEnumerable<ShdrTimeSeries> timeSeries;
            lock (_lock) timeSeries = _currentTimeSeries.Values.ToList();

            if (!timeSeries.IsNullOrEmpty())
            {
                var unavailableObservations = new List<ShdrTimeSeries>();
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Set each Observation to Unavailable
                foreach (var item in timeSeries)
                {
                    // Create new Unavailable Observation
                    var unavailableObservation = new ShdrTimeSeries();
                    unavailableObservation.DeviceKey = item.DeviceKey;
                    unavailableObservation.DataItemKey = item.DataItemKey;
                    unavailableObservation.Timestamp = ts;
                    unavailableObservation.IsUnavailable = true;
                    unavailableObservations.Add(unavailableObservation);
                }

                // Add Observations (only will add those that are changed)
                AddTimeSeries(unavailableObservations);
            }
        }

        #endregion

        #region "DataSet"

        protected virtual void OnDataSetAdd(ShdrDataSet dataSet) { }


        public void AddDataSet(ShdrDataSet dataSet)
        {
            if (dataSet != null)
            {
                var newDataSet = new ShdrDataSet(dataSet);

                // Set the DeviceKey
                newDataSet.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (!OutputTimestamps) newDataSet.Timestamp = 0;
                else if (newDataSet.Timestamp <= 0) newDataSet.Timestamp = UnixDateTime.Now;

                // Get the Current Observation (if exists)
                ShdrDataSet currentDataSet;
                lock (_lock) _currentDataSets.TryGetValue(newDataSet.DataItemKey, out currentDataSet);

                // Check to see if new Observation is the same as the Current
                var add = true;
                if (currentDataSet != null && FilterDuplicates)
                {
                    add = !ObjectExtensions.ByteArraysEqual(newDataSet.ChangeId, currentDataSet.ChangeId);
                }

                if (add)
                {
                    // Add to Current
                    lock (_lock)
                    {
                        _currentDataSets.Remove(newDataSet.DataItemKey);
                        _currentDataSets.Add(newDataSet.DataItemKey, newDataSet);
                    }

                    // Call Overridable Method
                    OnDataSetAdd(newDataSet);
                }
            }
        }

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


        public bool SendDataSet(ShdrDataSet dataSet)
        {
            if (dataSet != null)
            {
                var newDataSet = new ShdrDataSet(dataSet);

                // Set the DeviceKey
                newDataSet.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (!OutputTimestamps) newDataSet.Timestamp = 0;
                else if (newDataSet.Timestamp <= 0) newDataSet.Timestamp = UnixDateTime.Now;

                // Remove from Current
                lock (_lock) _currentDataSets.Remove(newDataSet.DataItemKey);

                // Call Overridable Method
                OnDataSetAdd(newDataSet);

                // Create SHDR string to send
                var shdrLine = newDataSet.ToString();

                var success = WriteLine(shdrLine);
                if (success)
                {
                    // Update Last Sent TimeSeries
                    UpdateLastDataSets(new List<ShdrDataSet> { newDataSet });
                }

                return success;
            }

            return false;
        }

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


        protected void UpdateLastDataSets(IEnumerable<ShdrDataSet> dataSets)
        {
            if (!dataSets.IsNullOrEmpty())
            {
                // Find the most recent Observation for each DataItemKey
                var dataSetKeys = dataSets.Select(o => o.DataItemKey).Distinct();
                foreach (var dataSetKey in dataSetKeys)
                {
                    var keyDataSets = dataSets.Where(o => o.DataItemKey == dataSetKey);
                    var mostRecent = keyDataSets.OrderByDescending(o => o.Timestamp).FirstOrDefault();

                    lock (_lock)
                    {
                        _lastDataSets.Remove(mostRecent.DataItemKey);
                        _lastDataSets.Add(mostRecent.DataItemKey, mostRecent);
                    }
                }
            }
        }


        protected bool WriteChangedDataSets()
        {
            // Get a list of all Current DataSets
            List<ShdrDataSet> dataSets;
            lock (_lock)
            {
                // Get List of DataSet that need to be Updated
                dataSets = new List<ShdrDataSet>();
                var items = _currentDataSets.Values;
                foreach (var item in items)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;
                        dataSets.Add(item);
                    }
                }
            }

            if (!dataSets.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var item in dataSets)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent DataSet
                    UpdateLastDataSets(dataSets);
                }

                return success;
            }

            return false;
        }

        protected bool WriteLastDataSets(long timestamp = 0)
        {
            // Get a list of all Last DataSet
            IEnumerable<ShdrDataSet> dataSets;
            lock (_lock) dataSets = _lastDataSets.Values.ToList();

            if (!dataSets.IsNullOrEmpty())
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;
                bool success = false;

                foreach (var item in dataSets)
                {
                    item.Timestamp = ts;

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent DataSet
                    UpdateLastDataSets(dataSets);
                }

                return success;
            }

            return false;
        }


        private void SetDataSetsUnavailable(long timestamp = 0)
        {
            // Get a list of all Current DataSets
            IEnumerable<ShdrDataSet> dataSet;
            lock (_lock) dataSet = _currentDataSets.Values.ToList();

            if (!dataSet.IsNullOrEmpty())
            {
                var unavailableObservations = new List<ShdrDataSet>();
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Set each Observation to Unavailable
                foreach (var item in dataSet)
                {
                    // Create new Unavailable Observation
                    var unavailableObservation = new ShdrDataSet();
                    unavailableObservation.DeviceKey = item.DeviceKey;
                    unavailableObservation.DataItemKey = item.DataItemKey;
                    unavailableObservation.Timestamp = ts;
                    unavailableObservation.IsUnavailable = true;
                    unavailableObservations.Add(unavailableObservation);
                }

                // Add Observations (only will add those that are changed)
                AddDataSets(unavailableObservations);
            }
        }

        #endregion

        #region "Table"

        protected virtual void OnTableAdd(ShdrTable table) { }


        public void AddTable(ShdrTable table)
        {
            if (table != null)
            {
                var newTable = new ShdrTable(table);

                // Set the DeviceKey
                newTable.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (!OutputTimestamps) newTable.Timestamp = 0;
                else if (newTable.Timestamp <= 0) newTable.Timestamp = UnixDateTime.Now;

                // Get the Current Observation (if exists)
                ShdrTable currentTable;
                lock (_lock) _currentTables.TryGetValue(newTable.DataItemKey, out currentTable);

                // Check to see if new Observation is the same as the Current
                var add = true;
                if (currentTable != null && FilterDuplicates)
                {
                    add = !ObjectExtensions.ByteArraysEqual(newTable.ChangeId, currentTable.ChangeId);
                }

                if (add)
                {
                    // Add to Current
                    lock (_lock)
                    {
                        _currentTables.Remove(newTable.DataItemKey);
                        _currentTables.Add(newTable.DataItemKey, newTable);
                    }

                    // Call Overridable Method
                    OnTableAdd(newTable);
                }
            }
        }

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


        public bool SendTable(ShdrTable table)
        {
            if (table != null)
            {
                var newTable = new ShdrTable(table);

                // Set the DeviceKey
                newTable.DeviceKey = DeviceKey;

                // Set Timestamp (if not already set)
                if (!OutputTimestamps) newTable.Timestamp = 0;
                else if (newTable.Timestamp <= 0) newTable.Timestamp = UnixDateTime.Now;

                // Remove from Current
                lock (_lock) _currentTables.Remove(newTable.DataItemKey);

                // Call Overridable Method
                OnTableAdd(newTable);

                // Create SHDR string to send
                var shdrLine = newTable.ToString();

                var success = WriteLine(shdrLine);
                if (success)
                {
                    // Update Last Sent
                    UpdateLastTables(new List<ShdrTable> { newTable });
                }

                return success;
            }

            return false;
        }

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


        protected void UpdateLastTables(IEnumerable<ShdrTable> tables)
        {
            if (!tables.IsNullOrEmpty())
            {
                // Find the most recent Observation for each DataItemKey
                var tableKeys = tables.Select(o => o.DataItemKey).Distinct();
                foreach (var tableKey in tableKeys)
                {
                    var keyTables = tables.Where(o => o.DataItemKey == tableKey);
                    var mostRecent = keyTables.OrderByDescending(o => o.Timestamp).FirstOrDefault();

                    lock (_lock)
                    {
                        _lastTables.Remove(mostRecent.DataItemKey);
                        _lastTables.Add(mostRecent.DataItemKey, mostRecent);
                    }
                }
            }
        }


        protected bool WriteChangedTables()
        {
            // Get a list of all Current Tables
            List<ShdrTable> tables;
            lock (_lock)
            {
                // Get List of Table that need to be Updated
                tables = new List<ShdrTable>();
                var items = _currentTables.Values;
                foreach (var item in items)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;
                        tables.Add(item);
                    }
                }
            }

            if (!tables.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var item in tables)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent Table
                    UpdateLastTables(tables);
                }

                return success;
            }

            return false;
        }

        protected bool WriteLastTables(long timestamp = 0)
        {
            // Get a list of all Last Table
            IEnumerable<ShdrTable> tables;
            lock (_lock) tables = _lastTables.Values.ToList();

            if (!tables.IsNullOrEmpty())
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;
                bool success = false;

                foreach (var item in tables)
                {
                    item.Timestamp = ts;

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent Table
                    UpdateLastTables(tables);
                }

                return success;
            }

            return false;
        }


        private void SetTablesUnavailable(long timestamp = 0)
        {
            // Get a list of all Current Tables
            IEnumerable<ShdrTable> table;
            lock (_lock) table = _currentTables.Values.ToList();

            if (!table.IsNullOrEmpty())
            {
                var unavailableObservations = new List<ShdrTable>();
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Set each Observation to Unavailable
                foreach (var item in table)
                {
                    // Create new Unavailable Observation
                    var unavailableObservation = new ShdrTable();
                    unavailableObservation.DeviceKey = item.DeviceKey;
                    unavailableObservation.DataItemKey = item.DataItemKey;
                    unavailableObservation.Timestamp = ts;
                    unavailableObservation.IsUnavailable = true;
                    unavailableObservations.Add(unavailableObservation);
                }

                // Add Observations (only will add those that are changed)
                AddTables(unavailableObservations);
            }
        }

        #endregion


        #region "Assets"

        protected virtual void OnAssetAdd(ShdrAsset asset) { }


        /// <summary>
        /// Add the specified MTConnect Asset and sends it to the Agent
        /// </summary>
        /// <param name="asset">The Asset to send</param>
        public void SendAsset(Assets.IAsset asset)
        {
            SendAsset(new ShdrAsset(asset));
        }

        /// <summary>
        /// Add the specified MTConnect Asset and sends it to the Agent
        /// </summary>
        /// <param name="asset">The Asset to send</param>
        private void SendAsset(ShdrAsset asset)
        {
            if (asset != null)
            {
                // Set Timestamp (if not already set)
                if (asset.Timestamp > 0) asset.Timestamp = UnixDateTime.Now;

                lock (_lock)
                {
                    // Check to see if Asset already exists in list
                    var existing = _lastAssets.FirstOrDefault(o => o.Key == asset.AssetId).Value;
                    if (existing == null)
                    {
                        _lastAssets.Add(asset.AssetId, asset);
                    }
                    else
                    {
                        _lastAssets.Remove(asset.AssetId);
                        _lastAssets.Add(asset.AssetId, asset);
                    }
                }

                var shdrLine = asset.ToString(MultilineAssets);
                WriteLine(shdrLine);
            }
        }

        /// <summary>
        /// Add the specified MTConnect Assets and sends them to the Agent
        /// </summary>
        /// <param name="assets">The Assets to send</param>
        public void SendAssets(IEnumerable<Assets.IAsset> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                var items = new List<ShdrAsset>();
                foreach (var item in assets)
                {
                    items.Add(new ShdrAsset(item));
                }

                SendAssets(items);
            }
        }

        /// <summary>
        /// Add the specified MTConnect Assets and sends them to the Agent
        /// </summary>
        /// <param name="assets">The Assets to send</param>
        private void SendAssets(IEnumerable<ShdrAsset> assets)
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
        /// Add the specified MTConnect Asset
        /// </summary>
        /// <param name="asset">The Asset to add</param>
        public void AddAsset(Assets.IAsset asset)
        {
            AddAsset(new ShdrAsset(asset));
        }

        /// <summary>
        /// Add the specified MTConnect Asset
        /// </summary>
        /// <param name="asset">The Asset to add</param>
        private void AddAsset(ShdrAsset asset)
        {
            if (asset != null)
            {
                // Set Timestamp (if not already set)
                if (asset.Timestamp <= 0) asset.Timestamp = UnixDateTime.Now;

                // Get the Current Asset (if exists)
                ShdrAsset currentAsset;
                lock (_lock) _currentAssets.TryGetValue(asset.AssetId, out currentAsset);

                // Check to see if new Asset is the same as the Current
                var add = true;
                if (currentAsset != null && FilterDuplicates)
                {
                    add = !ObjectExtensions.ByteArraysEqual(asset.ChangeId, currentAsset.ChangeId);
                }

                if (add)
                {
                    // Add to Current
                    lock (_lock)
                    {
                        _currentAssets.Remove(asset.AssetId);
                        _currentAssets.Add(asset.AssetId, asset);
                    }

                    // Call Overridable Method
                    OnAssetAdd(asset);
                }
            }
        }

        /// <summary>
        /// Add the specified MTConnect Assets
        /// </summary>
        /// <param name="assets">The Assets to add</param>
        public void AddAssets(IEnumerable<Assets.IAsset> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                var items = new List<ShdrAsset>();
                foreach (var item in assets)
                {
                    items.Add(new ShdrAsset(item));
                }

                AddAssets(items);
            }
        }

        /// <summary>
        /// Add the specified MTConnect Assets
        /// </summary>
        /// <param name="assets">The Assets to send</param>
        private void AddAssets(IEnumerable<ShdrAsset> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                foreach (var item in assets)
                {
                    AddAsset(item);
                }
            }
        }


        protected void UpdateLastAsset(IEnumerable<ShdrAsset> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                // Find the most recent Asset for each AssetId
                var assetKeys = assets.Select(o => o.AssetId).Distinct();
                foreach (var assetKey in assetKeys)
                {
                    var keyAssets = assets.Where(o => o.AssetId == assetKey);
                    var mostRecent = keyAssets.OrderByDescending(o => o.Timestamp).FirstOrDefault();

                    lock (_lock)
                    {
                        _lastAssets.Remove(mostRecent.AssetId);
                        _lastAssets.Add(mostRecent.AssetId, mostRecent);
                    }
                }
            }
        }


        protected bool WriteChangedAssets()
        {
            // Get a list of all Current Assets
            List<ShdrAsset> assets;
            lock (_lock)
            {
                // Get List of Table that need to be Updated
                assets = new List<ShdrAsset>();
                var items = _currentAssets.Values;
                foreach (var item in items)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;
                        assets.Add(item);
                    }
                }
            }

            if (!assets.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var item in assets)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString(MultilineAssets);
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent Asset
                    UpdateLastAsset(assets);
                }

                return success;
            }

            return false;
        }

        protected bool WriteAllAssets()
        {
            // Get a list of all Assets
            IEnumerable<ShdrAsset> assets;
            lock (_lock) assets = _lastAssets.Values.ToList();

            if (!assets.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var item in assets)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString(MultilineAssets);
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                return success;
            }

            return false;
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
        public void AddDevice(Devices.IDevice device)
        {
            AddDevice(new ShdrDevice(device));
        }

        /// <summary>
        /// Add the specified MTConnect Device to the queue to be written to the adapter stream
        /// </summary>
        /// <param name="device">The Device to add</param>
        private void AddDevice(ShdrDevice device)
        {
            if (device != null)
            {
                lock (_lock)
                {
                    // Check to see if Device already exists in list
                    var existing = _devices.FirstOrDefault(o => o.Key == device.DeviceUuid).Value;
                    if (existing == null)
                    {
                        _devices.Add(device.DeviceUuid, device);
                    }
                    else
                    {
                        if (existing.ChangeId != device.ChangeId)
                        {
                            _devices.Remove(device.DeviceUuid);
                            _devices.Add(device.DeviceUuid, device);
                        }
                    }
                }

                var shdrLine = device.ToString();
                WriteLine(shdrLine);
            }
        }

        /// <summary>
        /// Add the specified MTConnect Devices to the queue to be written to the adapter stream
        /// </summary>
        /// <param name="devices">The Devices to add</param>
        public void AddDevices(IEnumerable<Devices.IDevice> devices)
        {
            if (!devices.IsNullOrEmpty())
            {
                var items = new List<ShdrDevice>();
                foreach (var item in devices)
                {
                    items.Add(new ShdrDevice(item));
                }

                AddDevices(items);
            }
        }

        /// <summary>
        /// Add the specified MTConnect Devices to the queue to be written to the adapter stream
        /// </summary>
        /// <param name="devices">The Devices to add</param>
        private void AddDevices(IEnumerable<ShdrDevice> devices)
        {
            if (!devices.IsNullOrEmpty())
            {
                foreach (var item in devices)
                {
                    AddDevice(item);
                }
            }
        }

        protected bool WriteAllDevices()
        {
            // Get a list of all Devices
            IEnumerable<ShdrDevice> devices;
            lock (_lock) devices = _devices.Values.ToList();

            if (!devices.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var item in devices)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                return success;
            }

            return false;
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