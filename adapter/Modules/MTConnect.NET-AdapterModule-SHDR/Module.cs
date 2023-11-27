// Copyright(c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Adapters;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Logging;
using MTConnect.Shdr;
using System.Net.Sockets;
using System.Text;

namespace MTConnect
{
    public class Module : MTConnectAdapterModule
    {
        public const string ConfigurationTypeId = "shdr";

        private readonly object _lock = new object();
        private readonly ShdrAdapterModuleConfiguration _configuration;
        private readonly AgentClientConnectionListener _connectionListener;
        private readonly Dictionary<string, AgentClient> _clients = new Dictionary<string, AgentClient>();
        private CancellationTokenSource _stop;


        public Module(string id, object moduleConfiguration) : base(id)
        {
            _configuration = AdapterApplicationConfiguration.GetConfiguration<ShdrAdapterModuleConfiguration>(moduleConfiguration);
            if (_configuration == null) _configuration = new ShdrAdapterModuleConfiguration();

            _connectionListener = new AgentClientConnectionListener(_configuration.Port, _configuration.Heartbeat);
            _connectionListener.ClientConnected += AgentClientConnected;
            _connectionListener.ClientDisconnected += AgentClientDisconnected;
            _connectionListener.ClientPingReceived += AgentClientPingReceived;
            _connectionListener.ClientPongSent += AgentClientPongSent;
        }


        protected override void OnStart()
        {
            _stop = new CancellationTokenSource();

            // Start Agent Connection Listener
            _connectionListener.Start(_stop.Token);
        }

        protected override void OnStop()
        {
            if (_stop != null) _stop.Cancel();
            _connectionListener.Stop();
        }


        #region "Entities"

        public override bool AddObservations(IEnumerable<IObservationInput> observations)
        {
            // DataItems
            var dataItems = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.DataItem);
            if (!dataItems.IsNullOrEmpty())
            {
                var shdrDataItems = new List<ShdrDataItem>();
                foreach (var x in dataItems) shdrDataItems.Add(new ShdrDataItem(x));
                var shdrLine = ShdrDataItem.ToString(shdrDataItems);
                WriteLine(shdrLine);
                Console.WriteLine(shdrLine);
            }

            var messages = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.Message);

            // Conditions
            var conditions = observations.Where(o => ShdrObservation.GetObservationType(o) == ShdrObservationType.Condition);
            if (!conditions.IsNullOrEmpty())
            {
                foreach (var x in conditions)
                {
                    var shdrModel = new ShdrFaultState(new ConditionFaultStateObservationInput(x));
                    var shdrLine = shdrModel.ToString();
                    WriteLine(shdrLine);
                    Console.WriteLine(shdrLine);
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
                    WriteLine(shdrLine);
                    Console.WriteLine(shdrLine);
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
                    WriteLine(shdrLine);
                    Console.WriteLine(shdrLine);
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
                    WriteLine(shdrLine);
                    Console.WriteLine(shdrLine);
                }
            }

            return true;
        }

        public override bool AddAssets(IEnumerable<IAssetInput> assets)
        {
            foreach (var asset in assets)
            {
                var shdrModel = new ShdrAsset(asset.Asset);
                var shdrLine = shdrModel.ToString();
                WriteLine(shdrLine);
                Console.WriteLine(shdrLine);
            }

            return true;
        }

        public override bool AddDevices(IEnumerable<IDevice> devices)
        {
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
                            stream.ReadTimeout = _configuration.ConnectionTimeout;
                            stream.WriteTimeout = _configuration.ConnectionTimeout;

                            // Write the line (in bytes) to the Stream
                            stream.Write(bytes, 0, bytes.Length);

                            //LineSent?.Invoke(this, new AdapterEventArgs(client.Id, singleLine));
                        }
                        catch (Exception ex)
                        {
                            //SendError?.Invoke(this, new AdapterEventArgs(client.Id, ex.Message));
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
                    stream.ReadTimeout = _configuration.ConnectionTimeout;
                    stream.WriteTimeout = _configuration.ConnectionTimeout;

                    // Write the line (in bytes) to the Stream
                    await stream.WriteAsync(bytes, 0, bytes.Length);

                    //LineSent?.Invoke(this, new AdapterEventArgs(client.Id, line));

                    return true;
                }
                catch (Exception ex)
                {
                    //SendError?.Invoke(this, new AdapterEventArgs(client.Id, ex.Message));
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

    }
}