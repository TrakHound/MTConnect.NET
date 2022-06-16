// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Configurations;
using MTConnect.Observations.Input;
using MTConnect.Shdr;
using System;
using System.Collections.Generic;
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
        private readonly Dictionary<string, IEnumerable<ShdrDataItem>> _dataItems = new Dictionary<string, IEnumerable<ShdrDataItem>>();
        private readonly Dictionary<string, ShdrMessage> _messages = new Dictionary<string, ShdrMessage>();
        private readonly Dictionary<string, ShdrCondition> _conditions = new Dictionary<string, ShdrCondition>();
        private readonly Dictionary<string, ShdrTimeSeries> _timeSeries = new Dictionary<string, ShdrTimeSeries>();
        private readonly Dictionary<string, ShdrDataSet> _dataSets = new Dictionary<string, ShdrDataSet>();
        private readonly Dictionary<string, ShdrTable> _tables = new Dictionary<string, ShdrTable>();
        private readonly Dictionary<string, ShdrAsset> _assets = new Dictionary<string, ShdrAsset>();

        private CancellationTokenSource _stop;


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
        /// The interval (in milliseconds) at which new data is sent to the Agent
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Use multiline Assets
        /// </summary>
        public bool MultilineAssets { get; set; }


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



        public ShdrAdapter(string deviceKey, int port = 7878, int heartbeat = 10000)
        {
            DeviceKey = deviceKey;
            Port = port;
            Heartbeat = heartbeat;
            Timeout = 5000;
            Interval = 100;

            _connectionListener = new AgentClientConnectionListener(Port, heartbeat);
            _connectionListener.ClientConnected += ClientConnected;
            _connectionListener.ClientDisconnected += ClientDisconnected;
            _connectionListener.ClientPingReceived += ClientPingReceived;
            _connectionListener.ClientPongSent += ClientPongSent;
        }

        public ShdrAdapter(ShdrAdapterConfiguration configuration)
        {
            if (configuration != null)
            {
                DeviceKey = configuration.DeviceKey;
                Port = configuration.Port;
                Heartbeat = configuration.Heartbeat;
                Timeout = 5000;
                Interval = 100;

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

            // Start Write Queue
            _ = Task.Run(() => QueueWorker(_stop.Token));
        }

        /// <summary>
        /// Stops the adapter which also stops listening for Agent connections, disconnects any existing Agent connections, and stops the Queue for sending data to the Agent(s).
        /// </summary>
        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
            _connectionListener.Stop();
        }


        private async Task QueueWorker(CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    int interval = Math.Max(1, Interval); // Set Minimum of 1ms to prevent high CPU usage

                    var stpw = System.Diagnostics.Stopwatch.StartNew();

                    await WriteDataItemsAsync();
                    await WriteMessagesAsync();
                    await WriteConditionsAsync();
                    await WriteTimeSeriesAsync();
                    await WriteDataSetsAsync();
                    await WriteTablesAsync();
                    await WriteAssetsAsync();

                    stpw.Stop();

                    if (stpw.ElapsedMilliseconds < interval)
                    {
                        var waitInterval = interval - (int)stpw.ElapsedMilliseconds;

                        await Task.Delay(waitInterval, cancellationToken);
                    }

                } while (!cancellationToken.IsCancellationRequested);
            }
            catch (TaskCanceledException) { }
            catch (Exception) { }
        }


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

            var timestamp = UnixDateTime.Now;

            AddAllDataItems(clientId, timestamp);
            AddAllMessages(clientId, timestamp);
            AddAllConditions(clientId, timestamp);
            AddAllTimeSeries(clientId, timestamp);
            AddAllDataSets(clientId, timestamp);
            AddAllTables(clientId, timestamp);
            WriteAllAssets(clientId);

            AgentConnected?.Invoke(this, clientId);
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

        #region "Write"

        private bool WriteLine(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                // Write Line to each client in stored client list
                var clients = GetAgentClients();
                if (!clients.IsNullOrEmpty())
                {
                    foreach (var client in clients)
                    {
                        return WriteLineToClient(client, line);
                    }
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
                        return await WriteLineToClientAsync(client, line);
                    }
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
                    stream.Write(bytes, 0, bytes.Length);

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

        #endregion

        #region "DataItems"

        public ShdrDataItem GetDataItem(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                lock (_lock)
                {
                    if (_dataItems.TryGetValue(key, out var dataItems))
                    {
                        return dataItems.FirstOrDefault();
                    }
                }
            }

            return null;
        }

        public IEnumerable<ShdrDataItem> GetDataItems(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                lock (_lock)
                {
                    if (_dataItems.TryGetValue(key, out var dataItems))
                    {
                        return dataItems;
                    }
                }
            }

            return null;
        }

        public IEnumerable<ShdrDataItem> GetDataItems()
        {
            lock (_lock)
            {
                var x = new List<ShdrDataItem>();
                var values = _dataItems.Values.ToList();
                foreach (var dataItemValues in values)
                {
                    x.AddRange(dataItemValues);
                }
                return x;
            }
        }


        private bool UpdateDataItem(ShdrDataItem dataItem)
        {
            if (dataItem != null)
            {
                var success = false;

                lock (_lock)
                {
                    // Check to see if DataItem already exists in DataItem list
                    if (!_dataItems.TryGetValue(dataItem.DataItemKey, out var existing))
                    {
                        var x = new List<ShdrDataItem>();
                        x.Add(dataItem);

                        _dataItems.Add(dataItem.DataItemKey, x);
                        success = true;
                    }
                    else
                    {
                        var x = new List<ShdrDataItem>();
                        x.AddRange(existing);

                        if (!existing.Any(o => o.ChangeId == dataItem.ChangeId))
                        {
                            x.Add(dataItem);

                            dataItem.IsSent = false;

                            _dataItems.Remove(dataItem.DataItemKey);
                            _dataItems.Add(dataItem.DataItemKey, x);

                            success = true;
                        }
                    }
                }

                return success;
            }

            return false;
        }


        private bool WriteDataItems()
        {
            var dataItems = GetDataItems();
            if (!dataItems.IsNullOrEmpty())
            {
                var success = true;

                // Get List of DataItems that need to be Updated
                var items = new List<ShdrDataItem>();
                foreach (var item in dataItems)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;
                        items.Add(item);
                    }
                }

                if (!items.IsNullOrEmpty())
                {
                    // Create SHDR string to send
                    var shdrLine = ShdrDataItem.ToString(items);
                    success = WriteLine(shdrLine);
                }

                return success;
            }

            return false;
        }

        private async Task<bool> WriteDataItemsAsync()
        {
            var dataItems = GetDataItems();
            if (!dataItems.IsNullOrEmpty())
            {
                var success = true;

                // Get List of DataItems that need to be Updated
                var items = new List<ShdrDataItem>();
                foreach (var item in dataItems)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;
                        items.Add(item);
                    }
                }

                if (!items.IsNullOrEmpty())
                {
                    // Create SHDR string to send
                    var shdrLine = ShdrDataItem.ToString(items);
                    success = await WriteLineAsync(shdrLine);
                }

                return success;
            }

            return false;
        }


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
                // Set Timestamp (if not already set)
                if (dataItem.Timestamp <= 0) dataItem.Timestamp = UnixDateTime.Now;

                // Update DataItem
                UpdateDataItem(dataItem);
            }
        }

        public void AddDataItems(IEnumerable<ObservationInput> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                var items = new List<ShdrDataItem>();
                foreach (var observation in observations)
                {
                    items.Add(new ShdrDataItem(observation));
                }

                AddDataItems(items);
            }
        }

        public void AddDataItems(IEnumerable<ShdrDataItem> dataItems)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                // Get List of DataItems that need to be Updated
                foreach (var dataItem in dataItems)
                {
                    // Set Timestamp (if not already set)
                    if (dataItem.Timestamp <= 0) dataItem.Timestamp = UnixDateTime.Now;

                    // Update DataItem
                    UpdateDataItem(dataItem);
                }            
            }
        }


        private void AddAllDataItems(string clientId, long timestamp = 0)
        {
            var dataItems = GetDataItems();
            if (!dataItems.IsNullOrEmpty())
            {
                if (timestamp > 0)
                {
                    foreach (var dataItem in dataItems)
                    {
                        dataItem.Timestamp = timestamp;
                    }
                }

                // Create SHDR string to send
                var shdrLine = ShdrDataItem.ToString(dataItems);
                WriteLine(clientId, shdrLine);
            }
        }

        private async Task AddAllDataItemsAsync(string clientId, long timestamp = 0)
        {
            var dataItems = GetDataItems();
            if (!dataItems.IsNullOrEmpty())
            {
                if (timestamp > 0)
                {
                    foreach (var dataItem in dataItems)
                    {
                        dataItem.Timestamp = timestamp;
                    }
                }

                // Create SHDR string to send
                var shdrLine = ShdrDataItem.ToString(dataItems);
                await WriteLineAsync(clientId, shdrLine);
            }
        }


        private void SetDataItemsUnavailable(long timestamp = 0)
        {
            // Get list of All DataItems
            var dataItems = GetDataItems();
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

        public ShdrMessage GetMessage(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                lock (_lock)
                {
                    if (_messages.TryGetValue(key, out ShdrMessage message))
                    {
                        return message;
                    }
                }
            }

            return null;
        }

        public IEnumerable<ShdrMessage> GetMessages()
        {
            lock (_lock)
            {
                return _messages.Values.ToList();
            }
        }


        private bool UpdateMessage(ShdrMessage message)
        {
            if (message != null)
            {
                lock (_lock)
                {
                    // Check to see if Message already exists in Message list
                    if (!_messages.TryGetValue(message.DataItemKey, out var existing))
                    {
                        _messages.Add(message.DataItemKey, message);
                        return true;
                    }
                    else
                    {
                        if (existing.ChangeId != message.ChangeId)
                        {
                            message.IsSent = false;

                            _messages.Remove(message.DataItemKey);
                            _messages.Add(message.DataItemKey, message);

                            return true;
                        }
                    }
                }
            }

            return false;
        }


        private bool WriteMessages()
        {
            var messages = GetMessages();
            if (!messages.IsNullOrEmpty())
            {
                var success = true;

                // Get List of Messages that need to be Updated
                var items = new List<ShdrMessage>();
                foreach (var item in messages)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;
                        items.Add(item);
                    }
                }

                if (!items.IsNullOrEmpty())
                {
                    foreach (var item in items)
                    {
                        // Create SHDR string to send
                        var shdrLine = item.ToString();
                        success = WriteLine(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }

        private async Task<bool> WriteMessagesAsync()
        {
            var messages = GetMessages();
            if (!messages.IsNullOrEmpty())
            {
                // Get List of Messages that need to be Updated
                var items = new List<ShdrMessage>();
                foreach (var item in messages)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;
                        items.Add(item);
                    }
                }

                if (!items.IsNullOrEmpty())
                {
                    var success = true;

                    foreach (var item in items)
                    {
                        // Create SHDR string to send
                        var shdrLine = item.ToString();
                        success = await WriteLineAsync(shdrLine);
                        if (!success) break;
                    }

                    return success;
                }
            }

            return false;
        }


        public void AddMessage(string messageId, object value)
        {
            AddMessage(messageId, value, UnixDateTime.Now);
        }

        public void AddMessage(string messageId, object value, DateTime timestamp)
        {
            AddMessage(messageId, value, timestamp.ToUnixTime());
        }

        public void AddMessage(string messageId, object value, long timestamp)
        {
            AddMessage(new ShdrMessage(messageId, value, timestamp));
        }

        public void AddMessage(ShdrMessage message)
        {
            if (message != null)
            {
                // Set Timestamp (if not already set)
                if (message.Timestamp <= 0) message.Timestamp = UnixDateTime.Now;

                // Update Message
                UpdateMessage(message);
            }
        }

        public void AddMessages(IEnumerable<ShdrMessage> messages)
        {
            if (!messages.IsNullOrEmpty())
            {
                // Get List of Messages that need to be Updated
                foreach (var message in messages)
                {
                    // Set Timestamp (if not already set)
                    if (message.Timestamp <= 0) message.Timestamp = UnixDateTime.Now;

                    // Update Message
                    UpdateMessage(message);
                }
            }
        }


        private void AddAllMessages(string clientId, long timestamp = 0)
        {
            var messages = GetMessages();
            if (!messages.IsNullOrEmpty())
            {
                if (timestamp > 0)
                {
                    foreach (var message in messages)
                    {
                        message.Timestamp = timestamp;
                    }
                }

                foreach (var message in messages)
                {
                    // Create SHDR string to send
                    var shdrLine = message.ToString();
                    WriteLine(clientId, shdrLine);
                }
            }
        }

        private async Task AddAllMessagesAsync(string clientId, long timestamp = 0)
        {
            var messages = GetMessages();
            if (!messages.IsNullOrEmpty())
            {
                if (timestamp > 0)
                {
                    foreach (var message in messages)
                    {
                        message.Timestamp = timestamp;
                    }
                }

                foreach (var message in messages)
                {
                    // Create SHDR string to send
                    var shdrLine = message.ToString();
                    await WriteLineAsync(clientId, shdrLine);
                }
            }
        }


        private void SetMessagesUnavailable(long timestamp = 0)
        {
            // Get list of all Messages
            var messages = GetMessages();
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

        public ShdrCondition GetCondition(string dataItemKey)
        {
            if (!string.IsNullOrEmpty(dataItemKey))
            {
                lock (_lock)
                {
                    if (_conditions.TryGetValue(dataItemKey, out ShdrCondition condition))
                    {
                        return condition;
                    }
                }
            }

            return null;
        }

        public IEnumerable<ShdrCondition> GetConditions()
        {
            lock (_lock)
            {
                return _conditions.Values.ToList();
            }
        }


        private bool UpdateCondition(ShdrCondition condition)
        {
            if (condition != null)
            {
                lock (_lock)
                {
                    // Check to see if Condition already exists in DataItem list
                    var existing = _conditions.FirstOrDefault(o => o.Key == condition.DataItemKey).Value;
                    if (existing == null)
                    {
                        _conditions.Add(condition.DataItemKey, condition);
                        return true;
                    }
                    else
                    {
                        if (existing.ChangeId != condition.ChangeId)
                        {
                            condition.IsSent = false;

                            _conditions.Remove(condition.DataItemKey);
                            _conditions.Add(condition.DataItemKey, condition);
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        private bool WriteConditions()
        {
            var conditions = GetConditions();
            if (!conditions.IsNullOrEmpty())
            {
                var success = true;

                // Get List of Conditions that need to be Updated
                foreach (var condition in conditions)
                {
                    if (!condition.IsSent)
                    {
                        condition.IsSent = true;

                        // Create SHDR string to send
                        var shdrLine = condition.ToString();
                        success = WriteLine(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }

        private async Task<bool> WriteConditionsAsync()
        {
            var conditions = GetConditions();
            if (!conditions.IsNullOrEmpty())
            {
                var success = true;

                // Get List of Conditions that need to be Updated
                foreach (var condition in conditions)
                {
                    if (!condition.IsSent)
                    {
                        condition.IsSent = true;

                        // Create SHDR string to send
                        var shdrLine = condition.ToString();
                        success = await WriteLineAsync(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }


        public void AddCondition(ConditionObservationInput condition)
        {
            AddCondition(new ShdrFaultState(condition));
        }

        public void AddCondition(ShdrCondition condition)
        {
            if (condition != null)
            {
                if (!condition.FaultStates.IsNullOrEmpty())
                {
                    foreach (var faultState in condition.FaultStates)
                    {
                        // Set Timestamp (if not already set)
                        if (faultState.Timestamp <= 0) faultState.Timestamp = UnixDateTime.Now;
                    }
                }

                // Update Condition
                UpdateCondition(condition);
            }
        }

        public void AddConditions(IEnumerable<ShdrCondition> conditions)
        {
            if (!conditions.IsNullOrEmpty())
            {
                // Get List of Conditions that need to be Updated
                foreach (var condition in conditions)
                {
                    AddCondition(condition);
                }
            }
        }


        private void AddAllConditions(string clientId, long timestamp = 0)
        {
            var conditions = GetConditions();
            if (!conditions.IsNullOrEmpty())
            {
                foreach (var item in conditions)
                {
                    if (!item.FaultStates.IsNullOrEmpty())
                    {
                        foreach (var faultState in item.FaultStates)
                        {
                            if (timestamp > 0) faultState.Timestamp = timestamp;

                            // Create SHDR string to send
                            var shdrLine = faultState.ToString();
                            WriteLine(clientId, shdrLine);
                        }
                    }
                }
            }
        }

        private async Task AddAllConditionsAsync(string clientId, long timestamp = 0)
        {
            var conditions = GetConditions();
            if (!conditions.IsNullOrEmpty())
            {
                foreach (var item in conditions)
                {
                    if (!item.FaultStates.IsNullOrEmpty())
                    {
                        foreach (var faultState in item.FaultStates)
                        {
                            if (timestamp > 0) faultState.Timestamp = timestamp;

                            // Create SHDR string to send
                            var shdrLine = faultState.ToString();
                            WriteLine(clientId, shdrLine);
                        }
                    }
                }
            }
        }


        private void SetConditionsUnavailable(long timestamp = 0)
        {
            // Get a list of all Conditions
            var conditions = GetConditions();
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

        public ShdrTimeSeries GetTimeSeries(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                lock (_lock)
                {
                    if (_timeSeries.TryGetValue(key, out ShdrTimeSeries timeSeries))
                    {
                        return timeSeries;
                    }
                }
            }

            return null;
        }

        public IEnumerable<ShdrTimeSeries> GetTimeSeries()
        {
            lock (_lock)
            {
                return _timeSeries.Values.ToList();
            }
        }


        private bool UpdateTimeSeries(ShdrTimeSeries item)
        {
            if (item != null)
            {
                lock (_lock)
                {
                    // Check to see if TimeSeries already exists in DataItem list
                    var existing = _timeSeries.FirstOrDefault(o => o.Key == item.DataItemKey).Value;
                    if (existing == null)
                    {
                        _timeSeries.Add(item.DataItemKey, item);
                        return true;
                    }
                    else
                    {
                        if (existing.ChangeId != item.ChangeId)
                        {
                            item.IsSent = false;

                            _timeSeries.Remove(item.DataItemKey);
                            _timeSeries.Add(item.DataItemKey, item);
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        private bool WriteTimeSeries()
        {
            var timeSeries = GetTimeSeries();
            if (!timeSeries.IsNullOrEmpty())
            {
                var success = true;

                // Get List of TimeSeries that need to be Updated
                foreach (var item in timeSeries)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;

                        // Create SHDR string to send
                        var shdrLine = item.ToString();
                        success = WriteLine(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }

        private async Task<bool> WriteTimeSeriesAsync()
        {
            var timeSeries = GetTimeSeries();
            if (!timeSeries.IsNullOrEmpty())
            {
                var success = true;

                // Get List of TimeSeries that need to be Updated
                foreach (var item in timeSeries)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;

                        // Create SHDR string to send
                        var shdrLine = item.ToString();
                        success = await WriteLineAsync(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }


        public void AddTimeSeries(TimeSeriesObservationInput observation)
        {
            AddTimeSeries(new ShdrTimeSeries(observation));
        }

        public void AddTimeSeries(ShdrTimeSeries item)
        {
            if (item != null)
            {
                // Set Timestamp (if not already set)
                if (item.Timestamp <= 0) item.Timestamp = UnixDateTime.Now;

                // Update TimeSeries
                UpdateTimeSeries(item);
            }
        }

        public void AddTimeSeries(IEnumerable<TimeSeriesObservationInput> timeSeries)
        {
            if (!timeSeries.IsNullOrEmpty())
            {
                var items = new List<ShdrTimeSeries>();
                foreach (var item in timeSeries)
                {
                    items.Add(new ShdrTimeSeries(item));
                }

                AddTimeSeries(items);
            }
        }

        public void AddTimeSeries(IEnumerable<ShdrTimeSeries> timeSeries)
        {
            if (!timeSeries.IsNullOrEmpty())
            {
                // Get List of TimeSeries that need to be Updated
                foreach (var item in timeSeries)
                {
                    // Set Timestamp (if not already set)
                    if (item.Timestamp <= 0) item.Timestamp = UnixDateTime.Now;

                    // Update TimeSeries
                    UpdateTimeSeries(item);
                }
            }
        }


        private void AddAllTimeSeries(string clientId, long timestamp = 0)
        {
            var timeSeries = GetTimeSeries();
            if (!timeSeries.IsNullOrEmpty())
            {
                foreach (var item in timeSeries)
                {
                    if (timestamp > 0) item.Timestamp = timestamp;

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    WriteLine(clientId, shdrLine);
                }
            }
        }

        private async Task AddAllTimeSeriesAsync(string clientId, long timestamp = 0)
        {
            var timeSeries = GetTimeSeries();
            if (!timeSeries.IsNullOrEmpty())
            {
                foreach (var item in timeSeries)
                {
                    if (timestamp > 0) item.Timestamp = timestamp;

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    await WriteLineAsync(clientId, shdrLine);
                }
            }
        }


        private void SetTimeSeriesUnavailable(long timestamp = 0)
        {
            // Get list of TimeSeries
            var timeSeries = GetTimeSeries();
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

        #region "DataSets"

        public ShdrDataSet GetDataSet(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                lock (_lock)
                {
                    if (_dataSets.TryGetValue(key, out ShdrDataSet dataSet))
                    {
                        return dataSet;
                    }
                }
            }

            return null;
        }

        public IEnumerable<ShdrDataSet> GetDataSets()
        {
            lock (_lock)
            {
                return _dataSets.Values.ToList();
            }
        }


        private bool UpdateDataSet(ShdrDataSet item)
        {
            if (item != null)
            {
                lock (_lock)
                {
                    // Check to see if DataSet already exists in DataItem list
                    var existing = _dataSets.FirstOrDefault(o => o.Key == item.DataItemKey).Value;
                    if (existing == null)
                    {
                        _dataSets.Add(item.DataItemKey, item);
                        return true;
                    }
                    else
                    {
                        if (existing.ChangeId != item.ChangeId)
                        {
                            item.IsSent = false;

                            _dataSets.Remove(item.DataItemKey);
                            _dataSets.Add(item.DataItemKey, item);
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        private bool WriteDataSets()
        {
            var dataSets = GetDataSets();
            if (!dataSets.IsNullOrEmpty())
            {
                var success = true;

                // Get List of DataSet that need to be Updated
                foreach (var item in dataSets)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;

                        // Create SHDR string to send
                        var shdrLine = item.ToString();
                        success = WriteLine(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }

        private async Task<bool> WriteDataSetsAsync()
        {
            var dataSets = GetDataSets();
            if (!dataSets.IsNullOrEmpty())
            {
                var success = true;

                // Get List of DataSet that need to be Updated
                foreach (var item in dataSets)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;

                        // Create SHDR string to send
                        var shdrLine = item.ToString();
                        success = await WriteLineAsync(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }


        public void AddDataSet(DataSetObservationInput observation)
        {
            AddDataSet(new ShdrDataSet(observation));
        }

        public void AddDataSet(ShdrDataSet item)
        {
            if (item != null)
            {
                // Set Timestamp (if not already set)
                if (item.Timestamp <= 0) item.Timestamp = UnixDateTime.Now;

                // Update DataSet
                UpdateDataSet(item);
            }
        }

        public void AddDataSets(IEnumerable<DataSetObservationInput> dataSets)
        {
            if (!dataSets.IsNullOrEmpty())
            {
                var items = new List<ShdrDataSet>();
                foreach (var item in dataSets)
                {
                    items.Add(new ShdrDataSet(item));
                }

                AddDataSets(items);
            }
        }

        public void AddDataSets(IEnumerable<ShdrDataSet> dataSets)
        {
            if (!dataSets.IsNullOrEmpty())
            {
                // Get List of DataSet that need to be Updated
                foreach (var item in dataSets)
                {
                    // Set Timestamp (if not already set)
                    if (item.Timestamp <= 0) item.Timestamp = UnixDateTime.Now;

                    // Update DataSet
                    UpdateDataSet(item);
                }
            }
        }


        private void AddAllDataSets(string clientId, long timestamp = 0)
        {
            var dataSets = GetDataSets();
            if (!dataSets.IsNullOrEmpty())
            {
                foreach (var item in dataSets)
                {
                    if (timestamp > 0) item.Timestamp = timestamp;

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    WriteLine(clientId, shdrLine);
                }
            }
        }

        private async Task AddAllDataSetsAsync(string clientId, long timestamp = 0)
        {
            var dataSets = GetDataSets();
            if (!dataSets.IsNullOrEmpty())
            {
                foreach (var item in dataSets)
                {
                    if (timestamp > 0) item.Timestamp = timestamp;

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    await WriteLineAsync(clientId, shdrLine);
                }
            }
        }


        private void SetDataSetsUnavailable(long timestamp = 0)
        {
            // Get list of DataSets
            var dataSets = GetDataSets();
            if (!dataSets.IsNullOrEmpty())
            {
                var unavailableObservations = new List<ShdrDataSet>();
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Set each Observation to Unavailable
                foreach (var item in dataSets)
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

        #region "Tables"

        public ShdrTable GetTable(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                lock (_lock)
                {
                    if (_tables.TryGetValue(key, out ShdrTable table))
                    {
                        return table;
                    }
                }
            }

            return null;
        }

        public IEnumerable<ShdrTable> GetTables()
        {
            lock (_lock)
            {
                return _tables.Values.ToList();
            }
        }


        private bool UpdateTable(ShdrTable item)
        {
            if (item != null)
            {
                lock (_lock)
                {
                    // Check to see if Table already exists in DataItem list
                    var existing = _tables.FirstOrDefault(o => o.Key == item.DataItemKey).Value;
                    if (existing == null)
                    {
                        _tables.Add(item.DataItemKey, item);
                        return true;
                    }
                    else
                    {
                        if (existing.ChangeId != item.ChangeId)
                        {
                            item.IsSent = false;

                            _tables.Remove(item.DataItemKey);
                            _tables.Add(item.DataItemKey, item);
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        private bool WriteTables()
        {
            var tables = GetTables();
            if (!tables.IsNullOrEmpty())
            {
                var success = true;

                // Get List of Table that need to be Updated
                foreach (var item in tables)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;

                        // Create SHDR string to send
                        var shdrLine = item.ToString();
                        success = WriteLine(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }

        private async Task<bool> WriteTablesAsync()
        {
            var tables = GetTables();
            if (!tables.IsNullOrEmpty())
            {
                var success = true;

                // Get List of Table that need to be Updated
                foreach (var item in tables)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;

                        // Create SHDR string to send
                        var shdrLine = item.ToString();
                        success = await WriteLineAsync(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }


        public void AddTable(TableObservationInput observation)
        {
            AddTable(new ShdrTable(observation));
        }

        public void AddTable(ShdrTable item)
        {
            if (item != null)
            {
                // Set Timestamp (if not already set)
                if (item.Timestamp <= 0) item.Timestamp = UnixDateTime.Now;

                // Update Table
                UpdateTable(item);
            }
        }

        public void AddTables(IEnumerable<TableObservationInput> tables)
        {
            if (!tables.IsNullOrEmpty())
            {
                var items = new List<ShdrTable>();
                foreach (var item in tables)
                {
                    items.Add(new ShdrTable(item));
                }

                AddTables(items);
            }
        }

        public void AddTables(IEnumerable<ShdrTable> tables)
        {
            if (!tables.IsNullOrEmpty())
            {
                // Get List of Table that need to be Updated
                foreach (var item in tables)
                {
                    // Set Timestamp (if not already set)
                    if (item.Timestamp <= 0) item.Timestamp = UnixDateTime.Now;

                    // Update Table
                    UpdateTable(item);
                }
            }
        }


        private void AddAllTables(string clientId, long timestamp = 0)
        {
            var tables = GetTables();
            if (!tables.IsNullOrEmpty())
            {
                foreach (var item in tables)
                {
                    if (timestamp > 0) item.Timestamp = timestamp;

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    WriteLine(clientId, shdrLine);
                }
            }
        }

        private async Task AddAllTablesAsync(string clientId, long timestamp = 0)
        {
            var tables = GetTables();
            if (!tables.IsNullOrEmpty())
            {
                foreach (var item in tables)
                {
                    if (timestamp > 0) item.Timestamp = timestamp;

                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    await WriteLineAsync(clientId, shdrLine);
                }
            }
        }


        private void SetTablesUnavailable(long timestamp = 0)
        {
            // Get list of Tables
            var tables = GetTables();
            if (!tables.IsNullOrEmpty())
            {
                var unavailableObservations = new List<ShdrTable>();
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Set each Observation to Unavailable
                foreach (var item in tables)
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

        #region "Internal"

        private ShdrAsset GetAssetFromQueue(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                lock (_lock)
                {
                    if (_assets.TryGetValue(key, out ShdrAsset asset))
                    {
                        return asset;
                    }
                }
            }

            return null;
        }

        private IEnumerable<ShdrAsset> GetAssetsFromQueue()
        {
            lock (_lock)
            {
                return _assets.Values.ToList();
            }
        }


        private bool UpdateAsset(ShdrAsset asset)
        {
            if (asset != null)
            {
                lock (_lock)
                {
                    // Check to see if Asset already exists in DataItem list
                    var existing = _assets.FirstOrDefault(o => o.Key == asset.AssetId).Value;
                    if (existing == null)
                    {
                        _assets.Add(asset.AssetId, asset);
                        return true;
                    }
                    else
                    {
                        if (existing.ChangeId != asset.ChangeId)
                        {
                            _assets.Remove(asset.AssetId);
                            _assets.Add(asset.AssetId, asset);
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        private void AddAssetToQueue(ShdrAsset asset)
        {
            if (asset != null)
            {
                // Set Timestamp (if not already set)
                if (asset.Timestamp > 0) asset.Timestamp = UnixDateTime.Now;

                // Update Asset
                UpdateAsset(asset);
            }
        }

        private void AddAssetsToQueue(IEnumerable<ShdrAsset> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                // Get List of Assets that need to be Updated
                foreach (var item in assets)
                {
                    // Set Timestamp (if not already set)
                    if (item.Timestamp > 0) item.Timestamp = UnixDateTime.Now;

                    // Update Asset
                    UpdateAsset(item);
                }
            }
        }


        private bool WriteAssets()
        {
            var assets = GetAssetsFromQueue();
            if (!assets.IsNullOrEmpty())
            {
                var success = true;

                // Get List of Assets that need to be Updated
                foreach (var item in assets)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;

                        // Create SHDR string to send
                        var shdrLine = item.ToString(MultilineAssets);
                        success = WriteLine(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }

        private async Task<bool> WriteAssetsAsync()
        {
            var assets = GetAssetsFromQueue();
            if (!assets.IsNullOrEmpty())
            {
                var success = true;

                // Get List of Assets that need to be Updated
                foreach (var item in assets)
                {
                    if (!item.IsSent)
                    {
                        item.IsSent = true;

                        // Create SHDR string to send
                        var shdrLine = item.ToString(MultilineAssets);
                        success = await WriteLineAsync(shdrLine);
                        if (!success) break;
                    }
                }

                return success;
            }

            return false;
        }


        private bool WriteAllAssets(string clientId)
        {
            var assets = GetAssetsFromQueue();
            if (!assets.IsNullOrEmpty())
            {
                var success = true;

                foreach (var item in assets)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString(MultilineAssets);
                    success = WriteLine(clientId, shdrLine);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        private async Task<bool> WriteAllAssetsAsync(string clientId)
        {
            var assets = GetAssetsFromQueue();
            if (!assets.IsNullOrEmpty())
            {
                var success = true;

                foreach (var item in assets)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString(MultilineAssets);
                    success = await WriteLineAsync(clientId, shdrLine);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion


        /// <summary>
        /// Add the specified MTConnect Asset to the queue to be written to the adapter stream
        /// </summary>
        /// <param name="asset">The Asset to add</param>
        public void AddAsset(Assets.IAsset asset)
        {
            AddAssetToQueue(new ShdrAsset(asset));
        }

        /// <summary>
        /// Add the specified MTConnect Assets to the queue to be written to the adapter stream
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

                AddAssetsToQueue(items);
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
        /// Remove the specified Asset using the SHDR command @REMOVE_ASSET@
        /// </summary>
        /// <param name="assetId">The AssetId of the Asset to remove</param>
        /// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        public async Task RemoveAssetAsync(string assetId, long timestamp = 0)
        {
            // Create SHDR string to send
            var shdrLine = ShdrAsset.Remove(assetId, timestamp);

            // Write line to stream
            await WriteLineAsync(shdrLine);
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

        /// <summary>
        /// Remove all Assets of the specified Type using the SHDR command @REMOVE_ALL_ASSETS@
        /// </summary>
        /// <param name="assetType">The Type of the Assets to remove</param>
        /// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        public async Task RemoveAllAssetsAsync(string assetType, long timestamp = 0)
        {
            // Create SHDR string to send
            var shdrLine = ShdrAsset.RemoveAll(assetType, timestamp);

            // Write line to stream
            await WriteLineAsync(shdrLine);
        }

        #endregion
    }
}
