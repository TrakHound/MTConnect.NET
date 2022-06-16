// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Shdr
{
    /// <summary>
    /// A client to read from a SHDR protocol connection
    /// </summary>
    public class ShdrClient
    {
        public const string PingMessage = "* PING\n";
        public const int BufferSize = 1048576; // 1 MB
        public const int DefaultPongHeartbeat = 10000; // 10 seconds
        public const int DefaultConnectionTimeout = 30000; // 30 seconds
        public const int DefaultReconnectInterval = 10000; // 10 seconds

        private TcpClient _client;
        private long _lastHeartbeat = 0;
        private CancellationTokenSource _stop;

        
        /// <summary>
        /// The unique ID of the Client Connection
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The UUID or Name of the MTConnect Device
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// The Address to listen for connections
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The Port to listen for connections on
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The length of time an adapter can be silent before it is disconnected. This is only for legacy adapters that do not support heartbeats. 
        /// If heartbeats are present, this will be ignored.
        /// </summary>
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// The Interval (in milliseconds) to attempt to reconnect when a connection is lost
        /// </summary>
        public int ReconnectInterval { get; set; }


        /// <summary>
        /// Raised when a client connection is established
        /// </summary>
        public EventHandler<string> Connected { get; set; }

        /// <summary>
        /// Raised when the client is disconnected
        /// </summary>
        public EventHandler<string> Disconnected { get; set; }

        /// <summary>
        /// Raised when an error occurs during the connection
        /// </summary>
        public EventHandler<Exception> ConnectionError { get; set; }

        /// <summary>
        /// Raised when the client is not connected but is listening for new connections
        /// </summary>
        public EventHandler<string> Listening { get; set; }


        /// <summary>
        /// Raised when a Pong message response is received from the Adapter
        /// </summary>
        public EventHandler<string> PongReceived { get; set; }

        /// <summary>
        /// Raised when a Ping message request is sent to the Adapter
        /// </summary>
        public EventHandler<string> PingSent { get; set; }


        /// <summary>
        /// Raised when an SHDR Protocol line message is received from the Adapter
        /// </summary>
        public EventHandler<string> ProtocolReceived { get; set; }

        /// <summary>
        /// Raised when an SHDR Command message is received from the Adapter
        /// </summary>
        public EventHandler<string> CommandReceived { get; set; }


        public ShdrClient()
        {
            Id = StringFunctions.RandomString(10);
        }

        public ShdrClient(string hostname, int port, int connectionTimeout = DefaultConnectionTimeout, int reconnectInterval = DefaultReconnectInterval)
        {
            Id = StringFunctions.RandomString(10);
            Hostname = hostname;
            Port = port;
            ConnectionTimeout = connectionTimeout;
            ReconnectInterval = reconnectInterval;
        }

        public ShdrClient(string hostname, int port, string deviceKey, int connectionTimeout = DefaultConnectionTimeout, int reconnectInterval = DefaultReconnectInterval)
        {
            Id = StringFunctions.RandomString(10);
            Hostname = hostname;
            Port = port;
            DeviceKey = deviceKey;
            ConnectionTimeout = connectionTimeout;
            ReconnectInterval = reconnectInterval;
        }

        public ShdrClient(ShdrClientConfiguration configuration)
        {
            Id = StringFunctions.RandomString(10);

            if (configuration != null)
            {
                Id = configuration.Id;
                DeviceKey = configuration.DeviceKey;
                Hostname = configuration.Hostname;
                Port = configuration.Port;
                ConnectionTimeout = configuration.ConnectionTimeout;
                ReconnectInterval = configuration.ReconnectInterval;
            }
        }


        public void Start()
        {
            _stop = new CancellationTokenSource();
            _= Task.Run(() => ListenForAdapter(_stop.Token));
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
        }


        protected virtual async Task OnConnect() { }

        protected virtual async Task OnDisconnect() { }


        protected virtual async Task<IDataItem> OnGetDataItem(string dataItemKey) { return null; }

        protected virtual async Task OnDataItemReceived(ShdrDataItem dataItem) { }

        protected virtual async Task OnConditionFaultStateReceived(ShdrFaultState faultState) { }

        protected virtual async Task OnMessageReceived(ShdrMessage message) { }

        protected virtual async Task OnDataSetReceived(ShdrDataSet dataSet) { }

        protected virtual async Task OnTableReceived(ShdrTable table) { }

        protected virtual async Task OnTimeSeriesReceived(ShdrTimeSeries timeSeries) { }


        protected virtual async Task OnAssetReceived(IAsset asset) { }

        protected virtual async Task OnRemoveAssetReceived(string assetId, long timestamp) { }

        protected virtual async Task OnRemoveAllAssetsReceived(string assetType, long timestamp) { }



        private async Task ListenForAdapter(CancellationToken cancel)
        {
            var reconnectInterval = Math.Max(ReconnectInterval, 100);
            var connected = false;

            try
            {
                while (!cancel.IsCancellationRequested)
                {
                    try
                    {
                        var heartbeat = DefaultPongHeartbeat;
                        var buffer = new byte[BufferSize];
                        int i = 0;
                        string response = "";
                        long lastResponse = 0;

                        // Create new TCP Client
                        var addressFamily = GetIpAddressType(Hostname);
                        _client = new TcpClient(addressFamily);
                        _client.Connect(Hostname, Port);
                        _client.ReceiveTimeout = ConnectionTimeout;
                        _client.SendTimeout = ConnectionTimeout;

                        Connected?.Invoke(this, $"Connected to Adapter at {Hostname} on Port {Port}");
                        connected = true;

                        await OnConnect();

                        // Get the TCP Client Stream
                        using (var stream = _client.GetStream())
                        {
                            // Send Initial PING Request
                            var messageBytes = Encoding.ASCII.GetBytes(PingMessage);
                            await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cancel);
                            PingSent?.Invoke(this, $"Initial PING sent to : {Hostname} on Port {Port}");

                            // Read the Initial PONG Response
                            i = await stream.ReadAsync(buffer, 0, buffer.Length, cancel);

                            while (!cancel.IsCancellationRequested)
                            {
                                var now = UnixDateTime.Now;

                                if (i > 0)
                                {
                                    // Get string from buffer
                                    var s = Encoding.ASCII.GetString(buffer, 0, i);

                                    // Add buffer to XML
                                    response += s;

                                    if (response.Contains("\n"))
                                    {
                                        var lines = response.Split('\n');
                                        if (!lines.IsNullOrEmpty())
                                        {
                                            var j = 0;

                                            bool multilineAsset = false;
                                            long multilineAssetTimestamp = 0;
                                            string multilineAssetId = null;
                                            string multilineAssetType = null;
                                            string multilineId = null;
                                            var multilineContent = new StringBuilder();

                                            do
                                            {
                                                var line = lines[j];

                                                if (!string.IsNullOrEmpty(line))
                                                {
                                                    if (line.StartsWith("*"))
                                                    {
                                                        if (line.StartsWith("* PONG"))
                                                        {
                                                            heartbeat = GetPongHeartbeat(line);
                                                            _lastHeartbeat = now;

                                                            PongReceived?.Invoke(this, $"PONG Received from : {Hostname} on Port {Port} : Heartbeat = {heartbeat}ms");
                                                        }
                                                        else
                                                        {
                                                            await ProcessCommand(line);

                                                            lastResponse = now;

                                                            // Raise CommandReceived Event passing the Line that was read as a parameter
                                                            CommandReceived?.Invoke(this, line);
                                                        }
                                                    }
                                                    else if (ShdrAsset.IsAssetMultilineBegin(line))
                                                    {
                                                        multilineAssetTimestamp = ShdrAsset.ReadTimestamp(line);
                                                        multilineAssetId = ShdrAsset.ReadAssetId(line);
                                                        multilineAssetType = ShdrAsset.ReadAssetType(line);
                                                        multilineId = ShdrAsset.ReadAssetMultilineId(line);
                                                        multilineContent.Clear();
                                                        multilineAsset = true;

                                                        // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                                        ProtocolReceived?.Invoke(this, line);

                                                        lastResponse = now;
                                                    }
                                                    else if (ShdrAsset.IsAssetMultilineEnd(multilineId, line))
                                                    {
                                                        var assetType = Asset.GetAssetType(multilineAssetType);
                                                        if (assetType != null)
                                                        {
                                                            var asset = XmlAsset.FromXml(assetType, multilineContent.ToString());
                                                            if (asset != null)
                                                            {
                                                                await OnAssetReceived(asset);
                                                            }
                                                        }

                                                        multilineContent.Clear();
                                                        multilineAsset = false;

                                                        // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                                        ProtocolReceived?.Invoke(this, line);

                                                        lastResponse = now;
                                                    }
                                                    else if (multilineAsset)
                                                    {
                                                        multilineContent.Append(line);

                                                        // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                                        ProtocolReceived?.Invoke(this, line);

                                                        lastResponse = now;
                                                    }
                                                    else if (ShdrAsset.IsAssetLine(line))
                                                    {
                                                        var shdrAsset = ShdrAsset.FromString(line);
                                                        if (shdrAsset != null)
                                                        {
                                                            var assetType = Asset.GetAssetType(shdrAsset.AssetType);
                                                            if (assetType != null)
                                                            {
                                                                var asset = XmlAsset.FromXml(assetType, shdrAsset.Xml);
                                                                if (asset != null)
                                                                {
                                                                    await OnAssetReceived(asset);
                                                                }
                                                            }
                                                        }

                                                        // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                                        ProtocolReceived?.Invoke(this, line);

                                                        lastResponse = now;
                                                    }
                                                    else if (ShdrAsset.IsAssetRemove(line))
                                                    {
                                                        var removeAssetTimestamp = ShdrAsset.ReadTimestamp(line);
                                                        var removeAssetId = ShdrAsset.ReadRemoveAssetId(line);
                                                        if (!string.IsNullOrEmpty(removeAssetId))
                                                        {
                                                            await OnRemoveAssetReceived(removeAssetId, removeAssetTimestamp);
                                                        }

                                                        // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                                        ProtocolReceived?.Invoke(this, line);

                                                        lastResponse = now;
                                                    }
                                                    else if (ShdrAsset.IsAssetRemoveAll(line))
                                                    {
                                                        var removeAssetTimestamp = ShdrAsset.ReadTimestamp(line);
                                                        var removeAssetType = ShdrAsset.ReadRemoveAllAssetType(line);
                                                        if (!string.IsNullOrEmpty(removeAssetType))
                                                        {
                                                            await OnRemoveAllAssetsReceived(removeAssetType, removeAssetTimestamp);
                                                        }

                                                        // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                                        ProtocolReceived?.Invoke(this, line);

                                                        lastResponse = now;
                                                    }
                                                    else
                                                    {
                                                        await ProcessProtocol(line);

                                                        // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                                        ProtocolReceived?.Invoke(this, line);

                                                        lastResponse = now;
                                                    }
                                                }

                                                j++;
                                            }
                                            while (j < lines.Length);
                                        }
                                    }

                                    response = "";

                                    // Clear Buffer
                                    Array.Clear(buffer, 0, buffer.Length);
                                }

                                // Send PING Heartbeat if needed
                                if ((now - lastResponse) > heartbeat * 10000 && (now - _lastHeartbeat) > heartbeat * 10000)
                                {
                                    messageBytes = Encoding.ASCII.GetBytes(PingMessage);
                                    await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cancel);
                                    PingSent?.Invoke(this, $"PING sent to : {Hostname} on Port {Port}");
                                }

                                // Read Next Chunk if new Data is Available
                                if (stream.DataAvailable)
                                {
                                    i = await stream.ReadAsync(buffer, 0, buffer.Length, cancel);
                                }

                                await Task.Delay(1, cancel);
                            }
                        }
                    }
                    catch (TaskCanceledException) { }
                    catch (Exception ex)
                    {
                        ConnectionError?.Invoke(this, ex);
                    }
                    finally
                    {
                        if (_client != null)
                        {
                            _client.Close();

                            if (connected)
                            {
                                Disconnected?.Invoke(this, $"Disconnected from {Hostname} on Port {Port}");

                                await OnDisconnect();
                            }
                        }

                        connected = false;
                    }

                    // Wait for the ReconnectInterval (in milliseconds) until continuing while loop
                    await Task.Delay(reconnectInterval, cancel);

                    Listening?.Invoke(this, $"Listening for connection from {Hostname} on Port {Port}");
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                ConnectionError?.Invoke(this, ex);
            }
        }


        private int GetPongHeartbeat(string input)
        {
            var regex = new Regex(@"\* PONG ([0-9]*)");
            var match = regex.Match(input);
            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value.ToInt();
            }

            return DefaultPongHeartbeat;
        }


        private async Task ProcessCommand(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {

            }
        }

        private async Task ProcessProtocol(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                // Get DataItem based on Key in line
                var dataItem = await GetDataItem(line);
                if (dataItem != null)
                {
                    if (dataItem.Category == DataItemCategory.CONDITION)
                    {
                        var condition = ShdrFaultState.FromString(line);
                        if (condition != null) await OnConditionFaultStateReceived(condition);
                    }
                    else if (dataItem.Type == Devices.DataItems.Events.MessageDataItem.TypeId)
                    {
                        var message = ShdrMessage.FromString(line);
                        if (message != null) await OnMessageReceived(message);
                    }
                    else if (dataItem.Representation == DataItemRepresentation.TABLE)
                    {
                        var table = ShdrTable.FromString(line);
                        if (table != null) await OnTableReceived(table);
                    }
                    else if (dataItem.Representation == DataItemRepresentation.DATA_SET)
                    {
                        var dataSet = ShdrDataSet.FromString(line);
                        if (dataSet != null) await OnDataSetReceived(dataSet);
                    }
                    else if (dataItem.Representation == DataItemRepresentation.TIME_SERIES)
                    {
                        var timeSeries = ShdrTimeSeries.FromString(line);
                        if (timeSeries != null) await OnTimeSeriesReceived(timeSeries);
                    }
                    else
                    {
                        var dataItems = ShdrDataItem.FromString(line);
                        if (!dataItems.IsNullOrEmpty())
                        {
                            foreach (var item in dataItems) await OnDataItemReceived(item);
                        }
                    }
                }
            }
        }

        private async Task<IDataItem> GetDataItem(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                // Get the DataItemKey from the SHDR line
                var key = GetKey(line);
                if (!string.IsNullOrEmpty(key))
                {
                    return await OnGetDataItem(key);
                }
            }

            return null;
        }


        private static string GetKey(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                var x = ShdrLine.GetNextValue(line);
                var y = ShdrLine.GetNextSegment(line);

                var timestamp = ShdrLine.GetTimestamp(x);
                if (timestamp.HasValue)
                {
                    return ShdrLine.GetNextValue(y);
                }
                else
                {
                    return x;
                }
            }

            return null;
        }

        private static AddressFamily GetIpAddressType(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (IPAddress.TryParse(input, out var address))
                {
                    return address.AddressFamily;
                }
            }

            return AddressFamily.InterNetwork;           
        }
    }
}
