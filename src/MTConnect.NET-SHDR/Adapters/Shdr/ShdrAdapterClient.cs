// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Agents.Configuration;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// A client to connect to MTConnect Adapters using TCP and communicating using the SHDR Protocol
    /// </summary>
    public class ShdrAdapterClient
    {
        public const string PingMessage = "* PING\n";
        public const int BufferSize = 1048576;
        public const int DefaultPongHeartbeat = 60000;

        private readonly AdapterConfiguration _configuration;
        private readonly IMTConnectAgent _agent;
        private readonly IDevice _device;
        private TcpClient _client;
        private long _lastHeartbeat = 0;
        private CancellationTokenSource _stop;


        /// <summary>
        /// The unique ID of the Client Connection
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Address to listen for connections
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The TCP Port to listen for connections on
        /// </summary>
        public int Port { get; set; }
        
        /// <summary>
        /// The Interval (in milliseconds) to attempt to reconnect when a connection is lost
        /// </summary>
        public int ReconnectInterval { get; set; }


        /// <summary>
        /// Raised when the connection to the Adapter is established
        /// </summary>
        public EventHandler<string> AdapterConnected { get; set; }

        /// <summary>
        /// Raised when the connection to the Adapter is disconnected
        /// </summary>
        public EventHandler<string> AdapterDisconnected { get; set; }

        /// <summary>
        /// Raised when an error occurs during the connection to the Adapter
        /// </summary>
        public EventHandler<Exception> AdapterConnectionError { get; set; }


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


        public ShdrAdapterClient(
            string id,
            AdapterConfiguration configuration,
            IMTConnectAgent agent,
            IDevice device
            )
        {
            Id = id;
            _configuration = configuration;
            _agent = agent;
            _device = device;

            if (_configuration != null)
            {
                Server = _configuration.Host;
                Port = _configuration.Port;
                ReconnectInterval = _configuration.ReconnectInterval;
            }
        }

        public ShdrAdapterClient(
            AdapterConfiguration configuration,
            IMTConnectAgent agent,
            IDevice device,
            string server,
            int port
            )
        {
            _configuration = configuration;
            _agent = agent;
            _device = device;
            Server = server;
            Port = port;

            if (_configuration != null)
            {
                ReconnectInterval = _configuration.ReconnectInterval;
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

        private async Task ListenForAdapter(CancellationToken cancel)
        {
            var reconnectInterval = Math.Max(ReconnectInterval, 100);

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

                        // Create new TCP Client
                        var addressFamily = GetIpAddressType(Server);
                        _client = new TcpClient(addressFamily);
                        _client.Connect(Server, Port);

                        AdapterConnected?.Invoke(this, $"Connected to Adapter at {Server} on Port {Port}");

                        // Get the TCP Client Stream
                        using (var stream = _client.GetStream())
                        {
                            // Send Initial PING Request
                            var messageBytes = Encoding.ASCII.GetBytes(PingMessage);
                            await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cancel);
                            PingSent?.Invoke(this, $"Initial PING sent to : {Server} on Port {Port}");

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
                                                            PongReceived?.Invoke(this, $"PONG Received from : {Server} on Port {Port} : Heartbeat = {heartbeat}ms");
                                                        }
                                                        else
                                                        {
                                                            await ProcessCommand(line);

                                                            // Raise CommandReceived Event passing the Line that was read as a parameter
                                                            CommandReceived?.Invoke(this, line);
                                                        }
                                                    }
                                                    else if (ShdrAsset.IsAssetMultilineBegin(line))
                                                    {
                                                        multilineAssetId = ShdrAsset.ReadAssetId(line);
                                                        multilineAssetType = ShdrAsset.ReadAssetType(line);
                                                        multilineId = ShdrAsset.ReadAssetMultilineId(line);
                                                        multilineContent.Clear();
                                                        multilineAsset = true;
                                                    }
                                                    else if (ShdrAsset.IsAssetMultilineEnd(multilineId, line))
                                                    {
                                                        var assetType = Assets.Asset.GetAssetType(multilineAssetType);
                                                        if (assetType != null)
                                                        {
                                                            var asset = Assets.XmlAsset.FromXml(assetType, multilineContent.ToString());
                                                            if (asset != null)
                                                            {
                                                                await _agent.AddAssetAsync(_device.Uuid, asset);
                                                            }
                                                        }

                                                        multilineContent.Clear();
                                                        multilineAsset = false;
                                                    }
                                                    else if (multilineAsset)
                                                    {
                                                        multilineContent.Append(line);
                                                    }
                                                    else if (ShdrAsset.IsAssetLine(line))
                                                    {
                                                        var shdrAsset = ShdrAsset.FromString(line);
                                                        if (shdrAsset != null)
                                                        {
                                                            var assetType = Assets.Asset.GetAssetType(shdrAsset.Type);
                                                            if (assetType != null)
                                                            {
                                                                var asset = Assets.XmlAsset.FromXml(assetType, shdrAsset.Xml);
                                                                if (asset != null)
                                                                {
                                                                    await _agent.AddAssetAsync(_device.Uuid, asset);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await ProcessProtocol(line);

                                                        // Raise ProtocolReceived Event passing the Line that was read as a parameter
                                                        ProtocolReceived?.Invoke(this, line);
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
                                if ((now - _lastHeartbeat) > heartbeat)
                                {
                                    messageBytes = Encoding.ASCII.GetBytes(PingMessage);
                                    await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cancel);
                                    PingSent?.Invoke(this, $"PING sent to : {Server} on Port {Port}");
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
                    catch (TaskCanceledException ex) { }
                    catch (Exception ex)
                    {
                        AdapterConnectionError?.Invoke(this, ex);
                    }
                    finally
                    {
                        if (_client != null)
                        {
                            _client.Close();
                            AdapterDisconnected?.Invoke(this, $"Disconnected from {Server} on Port {Port}");

                            // Set DataItems to Unavailable if disconnected from Adapter
                            await SetDeviceUnavailable(UnixDateTime.Now);
                        }
                    }

                    // Wait for the ReconnectInterval (in milliseconds) until continuing while loop
                    await Task.Delay(reconnectInterval, cancel);
                }
            }
            catch (TaskCanceledException ex) { }
            catch (Exception ex)
            {
                AdapterConnectionError?.Invoke(this, ex);
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
            if (_agent != null && _device != null && !string.IsNullOrEmpty(line))
            {

            }
        }

        private async Task ProcessProtocol(string line)
        {
            if (_agent != null && _device != null && !string.IsNullOrEmpty(line))
            {
                if (ShdrAsset.IsAssetLine(line))
                {
                    var shdrAsset = ShdrAsset.FromString(line);
                    if (shdrAsset != null)
                    {
                        var assetType = Assets.Asset.GetAssetType(shdrAsset.Type);
                        if (assetType != null)
                        {
                            var asset = Assets.XmlAsset.FromXml(assetType, shdrAsset.Xml);
                            if (asset != null)
                            {
                                await _agent.AddAssetAsync(_device.Uuid, asset);
                            }
                        }
                    }
                }
                else
                {
                    // Get DataItem based on Key in line
                    var dataItem = GetDataItem(line);
                    if (dataItem != null)
                    {
                        if (dataItem.Category == DataItemCategory.CONDITION)
                        {
                            var condition = ShdrFaultState.FromString(line);
                            if (condition != null) await _agent.AddObservationAsync(_device.Uuid, condition);
                        }
                        else if (dataItem.Type == Devices.DataItems.Events.MessageDataItem.TypeId)
                        {
                            var message = ShdrMessage.FromString(line);
                            if (message != null) await _agent.AddObservationAsync(_device.Uuid, message);
                        }
                        else if (dataItem.Representation == DataItemRepresentation.TABLE)
                        {
                            var table = ShdrTable.FromString(line);
                            if (table != null) await _agent.AddObservationAsync(_device.Uuid, table);
                        }
                        else if (dataItem.Representation == DataItemRepresentation.DATA_SET)
                        {
                            var dataSet = ShdrDataSet.FromString(line);
                            if (dataSet != null) await _agent.AddObservationAsync(_device.Uuid, dataSet);
                        }
                        else if (dataItem.Representation == DataItemRepresentation.TIME_SERIES)
                        {
                            var timeSeries = ShdrTimeSeries.FromString(line);
                            if (timeSeries != null) await _agent.AddObservationAsync(_device.Uuid, timeSeries);
                        }
                        else
                        {
                            var dataItems = ShdrDataItem.FromString(line);
                            if (!dataItems.IsNullOrEmpty()) await _agent.AddObservationsAsync(_device.Uuid, dataItems);
                        }
                    }
                }
            }
        }

        private IDataItem GetDataItem(string line)
        {
            if (_device != null && !string.IsNullOrEmpty(line))
            {
                // Get the DataItemKey from the SHDR line
                var key = GetKey(line);

                // Return the DataItem matching the DataItemKey
                return _device.GetDataItemByKey(key);
            }

            return null;
        }


        private async Task SetDeviceUnavailable(long timestamp = 0)
        {
            await SetDataItemsUnavailable(timestamp);
            await SetConditionsUnavailable(timestamp);
        }

        private async Task SetDataItemsUnavailable(long timestamp = 0)
        {
            if (_agent != null && _device != null)
            {
                var dataItems = _device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    dataItems = dataItems.Where(o => o.Category != DataItemCategory.CONDITION);
                    if (!dataItems.IsNullOrEmpty())
                    {
                        foreach (var dataItem in dataItems)
                        {
                            var observation = new ObservationInput(dataItem.Id, Observation.Unavailable, timestamp);
                            await _agent.AddObservationAsync(_device.Uuid, observation);
                        }
                    }
                }
            }
        }

        private async Task SetConditionsUnavailable(long timestamp = 0)
        {
            if (_agent != null && _device != null)
            {
                var dataItems = _device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    dataItems = dataItems.Where(o => o.Category == DataItemCategory.CONDITION);
                    if (!dataItems.IsNullOrEmpty())
                    {
                        foreach (var dataItem in dataItems)
                        {
                            var condition = new ConditionObservationInput(dataItem.Id, ConditionLevel.UNAVAILABLE, timestamp);
                            await _agent.AddObservationAsync(_device.Uuid, condition);
                        }
                    }
                }
            }
        }

        private string GetKey(string line)
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
