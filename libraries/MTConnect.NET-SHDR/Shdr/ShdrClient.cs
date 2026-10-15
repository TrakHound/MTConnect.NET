// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.Xml;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Xml;
using System;
using System.IO;
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
        /// <summary>The literal SHDR PING command (<c>* PING\n</c>) sent to the adapter to probe for a heartbeat PONG response.</summary>
        public const string PingMessage = "* PING\n";

        /// <summary>The read buffer size in bytes used when consuming the adapter socket; sized at 1 MB to accommodate multi-line device documents.</summary>
        public const int BufferSize = 1048576; // 1 MB

        /// <summary>Default heartbeat interval (in milliseconds) the client expects to see PONG responses at; 10 seconds.</summary>
        public const int DefaultPongHeartbeat = 10000; // 10 seconds

        /// <summary>Default TCP send/receive timeout (in milliseconds); 30 seconds.</summary>
        public const int DefaultConnectionTimeout = 30000; // 30 seconds

        /// <summary>Default delay (in milliseconds) before attempting to reconnect after the adapter connection is lost; 10 seconds.</summary>
        public const int DefaultReconnectInterval = 10000; // 10 seconds

        // Read chunk size; lines are reassembled across reads (up to BufferSize per line)
        private const int ReadBufferSize = 65536;

        // How often the heartbeat watchdog checks the connection
        private const int WatchdogInterval = 250;

        private static readonly Regex _pongRegex = new Regex(@"\* PONG ([0-9]*)", RegexOptions.Compiled);

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
        /// Gets or Sets whether Heartbeat PING requests are not sent if data has been received within the Heartbeat period
        /// </summary>
        public bool IgnoreHeartbeatOnChange { get; set; }


        /// <summary>
        /// Raised when a client connection is established
        /// </summary>
        public event EventHandler<string> Connected;

        /// <summary>
        /// Raised when the client is disconnected
        /// </summary>
        public event EventHandler<string> Disconnected;

        /// <summary>
        /// Raised when an error occurs during the connection
        /// </summary>
        public event EventHandler<Exception> ConnectionError;

        /// <summary>
        /// Raised when the client is not connected but is listening for new connections
        /// </summary>
        public event EventHandler<string> Listening;


        /// <summary>
        /// Raised when a Pong message response is received from the Adapter
        /// </summary>
        public event EventHandler<string> PongReceived;

        /// <summary>
        /// Raised when a Ping message request is sent to the Adapter
        /// </summary>
        public event EventHandler<string> PingSent;


        /// <summary>
        /// Raised when an SHDR Protocol line message is received from the Adapter
        /// </summary>
        public event EventHandler<string> ProtocolReceived;

        /// <summary>
        /// Raised when an SHDR Command message is received from the Adapter
        /// </summary>
#pragma warning disable CS0067 // event is part of the public API surface, raised by subclasses
        public event EventHandler<string> CommandReceived;
#pragma warning restore CS0067


        /// <summary>Creates an unconfigured SHDR client with a fresh random <see cref="Id"/> and the heartbeat-on-change suppression enabled.</summary>
        public ShdrClient()
        {
            Id = StringFunctions.RandomString(10);
            IgnoreHeartbeatOnChange = true;
        }

        /// <summary>Creates an SHDR client connecting to <paramref name="hostname"/>:<paramref name="port"/> with optional timeout and reconnect overrides.</summary>
        public ShdrClient(string hostname, int port, int connectionTimeout = DefaultConnectionTimeout, int reconnectInterval = DefaultReconnectInterval)
        {
            Id = StringFunctions.RandomString(10);
            Hostname = hostname;
            Port = port;
            ConnectionTimeout = connectionTimeout;
            ReconnectInterval = reconnectInterval;
            IgnoreHeartbeatOnChange = true;
        }

        /// <summary>Creates an SHDR client scoped to a single device key on <paramref name="hostname"/>:<paramref name="port"/> with optional timeout and reconnect overrides.</summary>
        public ShdrClient(string hostname, int port, string deviceKey, int connectionTimeout = DefaultConnectionTimeout, int reconnectInterval = DefaultReconnectInterval)
        {
            Id = StringFunctions.RandomString(10);
            Hostname = hostname;
            Port = port;
            DeviceKey = deviceKey;
            ConnectionTimeout = connectionTimeout;
            ReconnectInterval = reconnectInterval;
            IgnoreHeartbeatOnChange = true;
        }

        /// <summary>Creates an SHDR client populated from <paramref name="configuration"/>; a null configuration falls back to the parameterless defaults.</summary>
        public ShdrClient(ShdrClientConfiguration configuration)
        {
            Id = StringFunctions.RandomString(10);
            IgnoreHeartbeatOnChange = true;

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


        /// <summary>Launches the listen-and-reconnect loop on a background task. Returns immediately; <see cref="Connected"/> is raised once the adapter socket is established.</summary>
        public void Start()
        {
            // Starting again stops the previous loop, which only ever checks its own token and socket
            var stop = new CancellationTokenSource();
            var previous = Interlocked.Exchange(ref _stop, stop);
            CancelQuietly(previous);

            _ = Task.Run(() => ListenForAdapter(stop.Token));
        }

        /// <summary>Signals the background loop to exit and disconnect from the adapter.</summary>
        public void Stop()
        {
            CancelQuietly(_stop);
        }

        private static void CancelQuietly(CancellationTokenSource source)
        {
            try
            {
                source?.Cancel();
            }
            catch (ObjectDisposedException) { }
        }


        /// <summary>Override-point called immediately after the TCP connection to the adapter is established (before any data is read).</summary>
        protected virtual void OnConnect() { }

        /// <summary>Override-point called when the adapter connection drops (clean shutdown, timeout, or transport error).</summary>
        protected virtual void OnDisconnect() { }


        /// <summary>Override-point that resolves a DataItem id/name to the live <see cref="IDataItem"/>; consumed by the SHDR parser to determine the observation representation.</summary>
        protected virtual IDataItem OnGetDataItem(string dataItemKey) { return null; }

        /// <summary>Override-point called for every parsed scalar Sample/Event observation.</summary>
        protected virtual void OnDataItemReceived(ShdrDataItem dataItem) { }

        /// <summary>Override-point called for every parsed Condition fault state.</summary>
        protected virtual void OnConditionFaultStateReceived(ShdrFaultState faultState) { }

        /// <summary>Override-point called for every parsed Message event.</summary>
        protected virtual void OnMessageReceived(ShdrMessage message) { }

        /// <summary>Override-point called for every parsed DataSet observation.</summary>
        protected virtual void OnDataSetReceived(ShdrDataSet dataSet) { }

        /// <summary>Override-point called for every parsed Table observation.</summary>
        protected virtual void OnTableReceived(ShdrTable table) { }

        /// <summary>Override-point called for every parsed TimeSeries observation.</summary>
        protected virtual void OnTimeSeriesReceived(ShdrTimeSeries timeSeries) { }


        /// <summary>Override-point called when an asset publish (<c>@ASSET@</c>) has been parsed and reassembled.</summary>
        protected virtual void OnAssetReceived(IAsset asset) { }

        /// <summary>Override-point called when a <c>@REMOVE_ASSET@</c> SHDR line targeting <paramref name="assetId"/> at <paramref name="timestamp"/> has been parsed.</summary>
        protected virtual void OnRemoveAssetReceived(string assetId, long timestamp) { }

        /// <summary>Override-point called when a <c>@REMOVE_ALL_ASSETS@</c> SHDR line targeting <paramref name="assetType"/> at <paramref name="timestamp"/> has been parsed.</summary>
        protected virtual void OnRemoveAllAssetsReceived(string assetType, long timestamp) { }


        /// <summary>Override-point called when a multi-line <c>@DEVICE@</c> document has been reassembled and parsed back into an <see cref="IDevice"/>.</summary>
        protected virtual void OnDeviceReceived(IDevice device) { }



        private async Task ListenForAdapter(CancellationToken cancel)
        {
            var reconnectInterval = Math.Max(ReconnectInterval, 100);

            while (!cancel.IsCancellationRequested)
            {
                var connected = false;

                // Each connection owns its socket, so a newer Start() can never close it (or be closed by it)
                var client = new TcpClient(GetIpAddressType(Hostname));

                // Stop() closes the socket, which ends any blocking connect / read immediately
                using (cancel.Register(() => CloseQuietly(client)))
                {
                    try
                    {
                        await ConnectAsync(client, cancel).ConfigureAwait(false);
                        client.SendTimeout = ConnectionTimeout;

                        Connected.Raise(this, $"Connected to Adapter at {Hostname} on Port {Port}", null);
                        connected = true;

                        OnConnect();

                        using (var stream = client.GetStream())
                        {
                            await RunConnection(client, stream, cancel).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex) when (!cancel.IsCancellationRequested)
                    {
                        ConnectionError.Raise(this, ex, null);
                    }
                    catch (Exception) { /* Stopped */ }
                    finally
                    {
                        CloseQuietly(client);

                        if (connected)
                        {
                            Disconnected.Raise(this, $"Disconnected from {Hostname} on Port {Port}", null);

                            OnDisconnect();
                        }
                    }
                }

                if (cancel.IsCancellationRequested) break;

                // Wait for the ReconnectInterval (in milliseconds) until continuing while loop
                try
                {
                    await Task.Delay(reconnectInterval, cancel).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                Listening.Raise(this, $"Listening for connection from {Hostname} on Port {Port}", null);
            }
        }

        private async Task ConnectAsync(TcpClient client, CancellationToken cancel)
        {
            // Bound the connect too; a ConnectionTimeout of 0 used to wait forever
            var timeout = ConnectionTimeout > 0 ? ConnectionTimeout : DefaultConnectionTimeout;

            var connect = client.ConnectAsync(Hostname, Port);
            var completed = await Task.WhenAny(connect, Task.Delay(timeout, cancel)).ConfigureAwait(false);
            if (completed != connect)
            {
                CloseQuietly(client);
                cancel.ThrowIfCancellationRequested();
                throw new TimeoutException($"Timed out connecting to Adapter at {Hostname} on Port {Port} ({timeout} ms)");
            }

            await connect.ConfigureAwait(false);
        }

        // Reads lines until the adapter disconnects, the connection goes silent, or the client is stopped.
        // A watchdog sends PING heartbeats and closes the socket when nothing has been received for too long.
        private async Task RunConnection(TcpClient client, NetworkStream stream, CancellationToken cancel)
        {
            var reader = new ShdrLineReader(BufferSize);
            var state = new ConnectionState();
            state.LastReceived = UnixDateTime.Now;

            // Send Initial PING Request
            SendPing(stream, state, $"Initial PING sent to : {Hostname} on Port {Port}");

            using (var connectionStop = CancellationTokenSource.CreateLinkedTokenSource(cancel))
            {
                var watchdog = Task.Run(() => Watchdog(client, stream, state, connectionStop.Token));

                try
                {
                    var buffer = new byte[ReadBufferSize];

                    while (!cancel.IsCancellationRequested)
                    {
                        var read = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                        if (read <= 0) break; // The adapter closed the connection

                        Interlocked.Exchange(ref state.LastReceived, UnixDateTime.Now);

                        reader.Append(buffer, read);

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (ProcessLine(line, state)) Interlocked.Exchange(ref state.LastDataReceived, UnixDateTime.Now);
                        }

                        if (reader.Overflowed)
                        {
                            ConnectionError.Raise(this, new InvalidDataException($"SHDR line from {Hostname} on Port {Port} exceeded {BufferSize} bytes without a line terminator and was discarded"), null);
                        }
                    }
                }
                catch (Exception) when (state.TimedOut != null)
                {
                    // The watchdog closed the socket; report why instead of the resulting read error
                }
                finally
                {
                    connectionStop.Cancel();
                    try { await watchdog.ConfigureAwait(false); } catch { }
                }

                if (state.TimedOut != null) throw state.TimedOut;
            }
        }

        private async Task Watchdog(TcpClient client, NetworkStream stream, ConnectionState state, CancellationToken cancel)
        {
            try
            {
                while (!cancel.IsCancellationRequested)
                {
                    await Task.Delay(WatchdogInterval, cancel).ConfigureAwait(false);

                    var now = UnixDateTime.Now;
                    var heartbeat = Volatile.Read(ref state.Heartbeat);

                    // Adapters that answer PING are expected to send something (data or PONG) every heartbeat;
                    // allow two heartbeats. Legacy adapters without heartbeats are bounded by ConnectionTimeout.
                    var silentLimit = heartbeat > 0 ? heartbeat * 2 : ConnectionTimeout;
                    var silentFor = (now - Interlocked.Read(ref state.LastReceived)) / TimeSpan.TicksPerMillisecond;
                    if (silentLimit > 0 && silentFor > silentLimit)
                    {
                        state.TimedOut = new TimeoutException($"No data received from Adapter at {Hostname} on Port {Port} for {silentFor} ms");
                        CloseQuietly(client);
                        return;
                    }

                    // Send PING Heartbeat if needed
                    var pingInterval = (long)(heartbeat > 0 ? heartbeat : DefaultPongHeartbeat) * TimeSpan.TicksPerMillisecond;
                    var dataRecently = (now - Interlocked.Read(ref state.LastDataReceived)) <= pingInterval;
                    if ((!IgnoreHeartbeatOnChange || !dataRecently) && (now - Interlocked.Read(ref state.LastPingSent)) > pingInterval)
                    {
                        SendPing(stream, state, $"PING sent to : {Hostname} on Port {Port}");
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                // A failed PING write means the connection is gone
                state.TimedOut = ex;
                CloseQuietly(client);
            }
        }

        private void SendPing(NetworkStream stream, ConnectionState state, string message)
        {
            var messageBytes = Encoding.ASCII.GetBytes(PingMessage);
            stream.Write(messageBytes, 0, messageBytes.Length);
            Interlocked.Exchange(ref state.LastPingSent, UnixDateTime.Now);
            PingSent.Raise(this, message, null);
        }

        private static void CloseQuietly(TcpClient client)
        {
            try
            {
                client?.Close();
            }
            catch { }
        }


        // Per-connection state shared by the read loop and the watchdog. Multiline asset / device state lives here
        // (not per TCP read), so a multiline block that arrives over several reads is still reassembled.
        private sealed class ConnectionState
        {
            public long LastReceived;
            public long LastDataReceived;
            public long LastPingSent;
            public int Heartbeat; // 0 until the adapter answers a PING
            public Exception TimedOut;

            public bool MultilineAsset;
            public long MultilineAssetTimestamp;
            public string MultilineAssetId;
            public string MultilineAssetType;

            public bool MultilineDevice;
            public string MultilineDeviceUuid;
            public string MultilineDeviceType;

            public string MultilineId;
            public readonly StringBuilder MultilineContent = new StringBuilder();
        }

        // Returns true when the line carried data (anything other than a PONG)
        private bool ProcessLine(string line, ConnectionState state)
        {
            if (string.IsNullOrEmpty(line)) return false;

            // Multiline content is taken verbatim until its end marker
            if (state.MultilineAsset || state.MultilineDevice)
            {
                if (state.MultilineAsset && ShdrAsset.IsAssetMultilineEnd(state.MultilineId, line))
                {
                    // By type name, like single-line assets: the Type overload expects the XML surrogate type, and
                    // passing the model type (Asset.GetAssetType) made every multiline asset fail to parse
                    var asset = XmlAsset.FromXml(state.MultilineAssetType, Encoding.UTF8.GetBytes(state.MultilineContent.ToString()));
                    if (asset != null)
                    {
                        OnAssetReceived(asset);
                    }

                    state.MultilineContent.Clear();
                    state.MultilineAsset = false;
                }
                else if (state.MultilineDevice && ShdrDevice.IsDeviceMultilineEnd(state.MultilineId, line))
                {
                    var device = XmlDevice.FromXml(Encoding.UTF8.GetBytes(state.MultilineContent.ToString()));
                    if (device != null)
                    {
                        OnDeviceReceived(device);
                    }

                    state.MultilineContent.Clear();
                    state.MultilineDevice = false;
                }
                else
                {
                    // Keep the line break so unindented continuation lines do not run together
                    state.MultilineContent.Append(line).Append('\n');
                }

                // Raise ProtocolReceived Event passing the Line that was read as a parameter
                ProtocolReceived.Raise(this, line, null);
                return true;
            }

            // Detect if message is a Command (Ping or Agent Command)
            if (line[0] == '*')
            {
                if (line.StartsWith("* PONG"))
                {
                    var heartbeat = GetPongHeartbeat(line);
                    Volatile.Write(ref state.Heartbeat, heartbeat);

                    PongReceived.Raise(this, $"PONG Received from : {Hostname} on Port {Port} : Heartbeat = {heartbeat}ms", null);
                    return false;
                }

                return true;
            }

            if (ShdrAsset.IsAssetMultilineBegin(line))
            {
                state.MultilineAssetTimestamp = ShdrAsset.ReadTimestamp(line);
                state.MultilineAssetId = ShdrAsset.ReadAssetId(line);
                state.MultilineAssetType = ShdrAsset.ReadAssetType(line);
                state.MultilineId = ShdrAsset.ReadAssetMultilineId(line);
                state.MultilineContent.Clear();
                state.MultilineAsset = true;
            }
            else if (ShdrAsset.IsAssetLine(line))
            {
                var shdrAsset = ShdrAsset.FromString(line);
                if (shdrAsset != null)
                {
                    var asset = XmlAsset.FromXml(shdrAsset.AssetType, Encoding.UTF8.GetBytes(shdrAsset.Xml));
                    if (asset != null)
                    {
                        OnAssetReceived(asset);
                    }
                }
            }
            else if (ShdrAsset.IsAssetRemove(line))
            {
                var removeAssetTimestamp = ShdrAsset.ReadTimestamp(line);
                var removeAssetId = ShdrAsset.ReadRemoveAssetId(line);
                if (!string.IsNullOrEmpty(removeAssetId))
                {
                    OnRemoveAssetReceived(removeAssetId, removeAssetTimestamp);
                }
            }
            else if (ShdrAsset.IsAssetRemoveAll(line))
            {
                var removeAssetTimestamp = ShdrAsset.ReadTimestamp(line);
                var removeAssetType = ShdrAsset.ReadRemoveAllAssetType(line);
                if (!string.IsNullOrEmpty(removeAssetType))
                {
                    OnRemoveAllAssetsReceived(removeAssetType, removeAssetTimestamp);
                }
            }
            else if (ShdrDevice.IsDeviceMultilineBegin(line))
            {
                state.MultilineDeviceUuid = ShdrDevice.ReadDeviceUuid(line);
                state.MultilineDeviceType = ShdrDevice.ReadDeviceType(line);
                state.MultilineId = ShdrDevice.ReadDeviceMultilineId(line);
                state.MultilineContent.Clear();
                state.MultilineDevice = true;
            }
            else if (ShdrDevice.IsDeviceLine(line))
            {
                var shdrDevice = ShdrDevice.FromString(line);
                if (shdrDevice != null && shdrDevice.Device != null)
                {
                    OnDeviceReceived(shdrDevice.Device);
                }
            }
            else
            {
                ProcessProtocol(line);
            }

            // Raise ProtocolReceived Event passing the Line that was read as a parameter
            ProtocolReceived.Raise(this, line, null);
            return true;
        }


        private static int GetPongHeartbeat(string input)
        {
            var match = _pongRegex.Match(input);
            if (match.Success && match.Groups.Count > 1)
            {
                var heartbeat = match.Groups[1].Value.ToInt();
                if (heartbeat > 0) return heartbeat;
            }

            return DefaultPongHeartbeat;
        }


        private void ProcessProtocol(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                // Get DataItem based on Key in line
                var dataItem = GetDataItem(line);
                if (dataItem != null && dataItem.Category == DataItemCategory.CONDITION)
                {
                    var condition = ShdrFaultState.FromString(line);
                    if (condition != null) OnConditionFaultStateReceived(condition);
                }
                else if (dataItem != null && dataItem.Type == Devices.DataItems.MessageDataItem.TypeId)
                {
                    var message = ShdrMessage.FromString(line);
                    if (message != null) OnMessageReceived(message);
                }
                else if (dataItem != null && dataItem.Representation == DataItemRepresentation.TABLE)
                {
                    var table = ShdrTable.FromString(line);
                    if (table != null) OnTableReceived(table);
                }
                else if (dataItem != null && dataItem.Representation == DataItemRepresentation.DATA_SET)
                {
                    var dataSet = ShdrDataSet.FromString(line);
                    if (dataSet != null) OnDataSetReceived(dataSet);
                }
                else if (dataItem != null && dataItem.Representation == DataItemRepresentation.TIME_SERIES)
                {
                    var timeSeries = ShdrTimeSeries.FromString(line);
                    if (timeSeries != null) OnTimeSeriesReceived(timeSeries);
                }
                else
                {
                    // Plain key|value pairs. Pass every pair on, even when the first key is unknown, so one
                    // unrecognised key does not drop the other DataItems on the line (receivers resolve each key).
                    var dataItems = ShdrDataItem.FromString(line);
                    if (!dataItems.IsNullOrEmpty())
                    {
                        foreach (var item in dataItems) OnDataItemReceived(item);
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
                var x = ShdrLine.GetNextSegment(line);
                var y = ShdrLine.GetNextValue(x);

                // "device:key" prefix: use the same split (last ':') as the line parsers
                if (y != null && y.Contains(":"))
                {
                    var i = y.LastIndexOf(':');
                    y = y.Substring(i + 1);
                }

                return y;
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


    /// <summary>
    /// Splits a byte stream into SHDR lines. Bytes are kept across reads until a line feed arrives, a trailing
    /// carriage return is removed, and each line is decoded as UTF-8 (so a multi-byte character split across
    /// reads is decoded correctly). A partial line longer than the limit is discarded.
    /// </summary>
    internal sealed class ShdrLineReader
    {
        private readonly int _maxLineLength;
        private byte[] _buffer = new byte[4096];
        private int _start;
        private int _end;

        /// <summary>True when the last <see cref="Append"/> discarded an over-long partial line.</summary>
        public bool Overflowed { get; private set; }


        public ShdrLineReader(int maxLineLength)
        {
            _maxLineLength = maxLineLength;
        }


        public void Append(byte[] bytes, int count)
        {
            Overflowed = false;

            // Compact, then grow if needed
            if (_start > 0)
            {
                Buffer.BlockCopy(_buffer, _start, _buffer, 0, _end - _start);
                _end -= _start;
                _start = 0;
            }

            if (_end + count > _buffer.Length)
            {
                var size = _buffer.Length;
                while (size < _end + count) size *= 2;
                Array.Resize(ref _buffer, size);
            }

            Buffer.BlockCopy(bytes, 0, _buffer, _end, count);
            _end += count;

            // A partial line that never ends would grow without bound
            if (_end - _start > _maxLineLength && Array.IndexOf(_buffer, (byte)'\n', _start, _end - _start) < 0)
            {
                _start = 0;
                _end = 0;
                Overflowed = true;
            }
        }

        /// <summary>Returns the next complete line (without its terminator), or <c>null</c> when none is buffered.</summary>
        public string ReadLine()
        {
            var newLine = Array.IndexOf(_buffer, (byte)'\n', _start, _end - _start);
            if (newLine < 0) return null;

            var length = newLine - _start;
            if (length > 0 && _buffer[newLine - 1] == (byte)'\r') length--;

            var line = Encoding.UTF8.GetString(_buffer, _start, length);
            _start = newLine + 1;
            return line;
        }
    }
}
