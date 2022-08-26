// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Assets.Xml;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.Xml;
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


        protected virtual void OnConnect() { }

        protected virtual void OnDisconnect() { }


        protected virtual IDataItem OnGetDataItem(string dataItemKey) { return null; }

        protected virtual void OnDataItemReceived(ShdrDataItem dataItem) { }

        protected virtual void OnConditionFaultStateReceived(ShdrFaultState faultState) { }

        protected virtual void OnMessageReceived(ShdrMessage message) { }

        protected virtual void OnDataSetReceived(ShdrDataSet dataSet) { }

        protected virtual void OnTableReceived(ShdrTable table) { }

        protected virtual void OnTimeSeriesReceived(ShdrTimeSeries timeSeries) { }


        protected virtual void OnAssetReceived(IAsset asset) { }

        protected virtual void OnRemoveAssetReceived(string assetId, long timestamp) { }

        protected virtual void OnRemoveAllAssetsReceived(string assetType, long timestamp) { }


        protected virtual void OnDeviceReceived(IDevice device) { }



        private async Task ListenForAdapter(CancellationToken cancel)
        {
            var reconnectInterval = Math.Max(ReconnectInterval, 100);
            var connected = false;

            var heartbeat = DefaultPongHeartbeat;
            long lastResponse = 0;
            long now = UnixDateTime.Now;

            var buffer = new byte[BufferSize];
            int bufferIndex = 0;
            char[] chars = new char[BufferSize];
            var bufferString = new StringBuilder();
            var responseString = new StringBuilder();
            var encoder = new ASCIIEncoding();

            try
            {
                while (!cancel.IsCancellationRequested)
                {
                    try
                    {
                        // Create new TCP Client
                        var addressFamily = GetIpAddressType(Hostname);
                        _client = new TcpClient(addressFamily);
                        _client.Connect(Hostname, Port);
                        _client.ReceiveTimeout = ConnectionTimeout;
                        _client.SendTimeout = ConnectionTimeout;

                        Connected?.Invoke(this, $"Connected to Adapter at {Hostname} on Port {Port}");
                        connected = true;

                        OnConnect();

                        // Get the TCP Client Stream
                        using (var stream = _client.GetStream())
                        {
                            // Send Initial PING Request
                            var messageBytes = Encoding.ASCII.GetBytes(PingMessage);
                            stream.Write(messageBytes, 0, messageBytes.Length);
                            PingSent?.Invoke(this, $"Initial PING sent to : {Hostname} on Port {Port}");

                            // Read the Initial PONG Response
                            bufferIndex = stream.Read(buffer, 0, buffer.Length);

                            while (!cancel.IsCancellationRequested)
                            {
                                now = UnixDateTime.Now;

                                if (bufferIndex > 0)
                                {
                                    Array.Copy(encoder.GetChars(buffer, 0, bufferIndex), 0, chars, 0, bufferIndex);

                                    var hasResponse = ProcessResponse(ref chars, bufferIndex);

                                    if (hasResponse) lastResponse = now;
                                    else _lastHeartbeat = now;

                                    // Clear Buffers
                                    Array.Clear(buffer, 0, bufferIndex);
                                    Array.Clear(chars, 0, bufferIndex);
                                }

                                // Send PING Heartbeat if needed
                                if ((now - lastResponse) > heartbeat * 10000 && (now - _lastHeartbeat) > heartbeat * 10000)
                                {
                                    messageBytes = Encoding.ASCII.GetBytes(PingMessage);
                                    stream.Write(messageBytes, 0, messageBytes.Length);
                                    PingSent?.Invoke(this, $"PING sent to : {Hostname} on Port {Port}");
                                    _lastHeartbeat = now;
                                }

                                // Read Next Chunk if new Data is Available
                                if (stream.DataAvailable)
                                {
                                    bufferIndex = stream.Read(buffer, 0, buffer.Length);
                                }
                                else
                                {
                                    bufferIndex = 0;
                                }

                                Thread.Sleep(1);
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
                            _client.Dispose();

                            if (connected)
                            {
                                Disconnected?.Invoke(this, $"Disconnected from {Hostname} on Port {Port}");

                                OnDisconnect();
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

                if (_client != null)
                {
                    _client.Close();
                    _client.Dispose();

                    GC.Collect();

                    if (connected)
                    {
                        Disconnected?.Invoke(this, $"Disconnected from {Hostname} on Port {Port}");

                        OnDisconnect();
                    }
                }

                connected = false;
            }
        }

        private bool ProcessResponse(ref char[] chars, int length)
        {
            var response = new string(chars, 0, length);
            
            if (response.Contains("\n"))
            {
                var lines = response.Split("\n");
                if (lines != null && lines.Length > 0)
                {
                    var j = 0;
                    int heartbeat;

                    bool multilineAsset = false;
                    long multilineAssetTimestamp = 0;
                    string multilineAssetId = null;
                    string multilineAssetType = null;

                    bool multilineDevice = false;
                    string multilineDeviceUuid = null;
                    string multilineDeviceType = null;

                    string multilineId = null;
                    var multilineContent = new StringBuilder();

                    var found = false;

                    do
                    {
                        var line = lines[j];
                        if (!string.IsNullOrEmpty(line))
                        {
                            // Detect if message is a Command (Ping or Agent Command)
                            if (line[0] == '*')
                            {
                                if (line.StartsWith("* PONG"))
                                {
                                    heartbeat = GetPongHeartbeat(line);

                                    PongReceived?.Invoke(this, $"PONG Received from : {Hostname} on Port {Port} : Heartbeat = {heartbeat}ms");
                                }
                                else
                                {
                                    //await ProcessCommand(line);

                                    //lastResponse = now;

                                    // Raise CommandReceived Event passing the Line that was read as a parameter
                                    //CommandReceived?.Invoke(this, line);

                                    found = true;
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

                                found = true;
                            }
                            else if (multilineAsset && ShdrAsset.IsAssetMultilineEnd(multilineId, line))
                            {
                                var assetType = Asset.GetAssetType(multilineAssetType);
                                if (assetType != null)
                                {
                                    var asset = XmlAsset.FromXml(assetType, Encoding.UTF8.GetBytes(multilineContent.ToString()));
                                    if (asset != null)
                                    {
                                        OnAssetReceived(asset);
                                    }
                                }

                                multilineContent.Clear();
                                multilineAsset = false;

                                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                ProtocolReceived?.Invoke(this, line);

                                found = true;
                            }
                            else if (multilineAsset)
                            {
                                multilineContent.Append(line);

                                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                ProtocolReceived?.Invoke(this, line);

                                found = true;
                            }
                            else if (ShdrAsset.IsAssetLine(line))
                            {
                                var shdrAsset = ShdrAsset.FromString(line);
                                if (shdrAsset != null)
                                {
                                    var assetType = Asset.GetAssetType(shdrAsset.AssetType);
                                    if (assetType != null)
                                    {
                                        var asset = XmlAsset.FromXml(assetType, Encoding.UTF8.GetBytes(shdrAsset.Xml));
                                        if (asset != null)
                                        {
                                            OnAssetReceived(asset);
                                        }
                                    }
                                }

                                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                ProtocolReceived?.Invoke(this, line);

                                found = true;
                            }
                            else if (ShdrAsset.IsAssetRemove(line))
                            {
                                var removeAssetTimestamp = ShdrAsset.ReadTimestamp(line);
                                var removeAssetId = ShdrAsset.ReadRemoveAssetId(line);
                                if (!string.IsNullOrEmpty(removeAssetId))
                                {
                                    OnRemoveAssetReceived(removeAssetId, removeAssetTimestamp);
                                }

                                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                ProtocolReceived?.Invoke(this, line);

                                found = true;
                            }
                            else if (ShdrAsset.IsAssetRemoveAll(line))
                            {
                                var removeAssetTimestamp = ShdrAsset.ReadTimestamp(line);
                                var removeAssetType = ShdrAsset.ReadRemoveAllAssetType(line);
                                if (!string.IsNullOrEmpty(removeAssetType))
                                {
                                    OnRemoveAllAssetsReceived(removeAssetType, removeAssetTimestamp);
                                }

                                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                ProtocolReceived?.Invoke(this, line);

                                found = true;
                            }
                            else if (ShdrDevice.IsDeviceMultilineBegin(line))
                            {
                                multilineDeviceUuid = ShdrDevice.ReadDeviceUuid(line);
                                multilineDeviceType = ShdrDevice.ReadDeviceType(line);
                                multilineId = ShdrDevice.ReadDeviceMultilineId(line);
                                multilineContent.Clear();
                                multilineDevice = true;

                                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                ProtocolReceived?.Invoke(this, line);

                                found = true;
                            }
                            else if (multilineDevice && ShdrDevice.IsDeviceMultilineEnd(multilineId, line))
                            {
                                var device = XmlDevice.FromXml(Encoding.UTF8.GetBytes(multilineContent.ToString()));
                                if (device != null)
                                {
                                    OnDeviceReceived(device);
                                }

                                multilineContent.Clear();
                                multilineDevice = false;

                                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                ProtocolReceived?.Invoke(this, line);

                                found = true;
                            }
                            else if (multilineDevice)
                            {
                                multilineContent.Append(line);

                                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                ProtocolReceived?.Invoke(this, line);

                                found = true;
                            }
                            else if (ShdrDevice.IsDeviceLine(line))
                            {
                                var shdrDevice = ShdrDevice.FromString(line);
                                if (shdrDevice != null && shdrDevice.Device != null)
                                {
                                    OnDeviceReceived(shdrDevice.Device);
                                }

                                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                ProtocolReceived?.Invoke(this, line);

                                found = true;
                            }
                            else
                            {
                                ProcessProtocol(line);

                                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                ProtocolReceived?.Invoke(this, line);

                                found = true;
                            }
                        }

                        j++;
                    }
                    while (j < lines.Length);

                    return found;
                }
            }

            return false;
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

        private void ProcessProtocol(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                // Get DataItem based on Key in line
                var dataItem = GetDataItem(line);
                if (dataItem != null)
                {
                    if (dataItem.Category == DataItemCategory.CONDITION)
                    {
                        var condition = ShdrFaultState.FromString(line);
                        if (condition != null) OnConditionFaultStateReceived(condition);
                    }
                    else if (dataItem.Type == Devices.DataItems.Events.MessageDataItem.TypeId)
                    {
                        var message = ShdrMessage.FromString(line);
                        if (message != null) OnMessageReceived(message);
                    }
                    else if (dataItem.Representation == DataItemRepresentation.TABLE)
                    {
                        var table = ShdrTable.FromString(line);
                        if (table != null) OnTableReceived(table);
                    }
                    else if (dataItem.Representation == DataItemRepresentation.DATA_SET)
                    {
                        var dataSet = ShdrDataSet.FromString(line);
                        if (dataSet != null) OnDataSetReceived(dataSet);
                    }
                    else if (dataItem.Representation == DataItemRepresentation.TIME_SERIES)
                    {
                        var timeSeries = ShdrTimeSeries.FromString(line);
                        if (timeSeries != null) OnTimeSeriesReceived(timeSeries);
                    }
                    else
                    {
                        var dataItems = ShdrDataItem.FromString(line);
                        if (!dataItems.IsNullOrEmpty())
                        {
                            foreach (var item in dataItems) OnDataItemReceived(item);
                        }
                    }
                }
            }
        }

        private IDataItem GetDataItem(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                // Get the DataItemKey from the SHDR line
                var key = GetDataItemKey(line);
                if (!string.IsNullOrEmpty(key))
                {
                    return OnGetDataItem(key);
                }
            }

            return null;
        }


        private static string GetDataItemKey(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                var x = ShdrLine.GetNextValue(line);
                var y = ShdrLine.GetNextSegment(line);

                var timestamp = ShdrLine.GetTimestamp(x);
                if (timestamp.HasValue)
                {
                    x = ShdrLine.GetNextValue(y);

                    if (x.Contains(':'))
                    {
                        var i = x.IndexOf(':');
                        x = x.Substring(i + 1);
                    }
                }

                return x;
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
