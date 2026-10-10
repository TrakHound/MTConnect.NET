// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.AgentModule.TormachAdapter.Tests
{
    /// <summary>
    /// A TCP server that sends PathPilot status server messages (one JSON object per line).
    /// </summary>
    public class FakeStatusServer : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _stop = new CancellationTokenSource();
        private readonly List<TcpClient> _clients = new List<TcpClient>();
        private int _connectionCount;


        /// <summary>The TCP port.</summary>
        public int Port { get; }

        /// <summary>The info message sent when a client connects.</summary>
        public string Info { get; set; } = "{\"type\":\"info\",\"version\":\"1.0\",\"machine\":\"tormach_mill_1100\",\"coordinates\":\"X Y Z A\",\"linear_units\":0.03937007874}";

        /// <summary>The status fields sent every interval (merged into the message).</summary>
        public Dictionary<string, string> Status { get; } = new Dictionary<string, string>
        {
            { "available", "true" },
            { "task_mode", "\"AUTO\"" },
            { "task_state", "\"ON\"" },
            { "interp_state", "\"READING\"" },
            { "estop", "false" },
            { "paused", "false" },
            { "file", "\"/home/operator/gcode/bracket.nc\"" },
            { "line", "120" },
            { "tool", "5" },
            { "g5x_index", "1" },
            { "actual_position", "[1.5,-2.25,0.1,90.5,0,0,0,0,0]" },
            { "g5x_offset", "[1.0,-2.5,0,0,0,0,0,0,0]" },
            { "g92_offset", "[0,0,0,0,0,0,0,0,0]" },
            { "tool_offset", "[0,0,0.5,0,0,0,0,0,0]" },
            { "current_vel", "0.25" },
            { "feedrate", "1.0" },
            { "rapidrate", "0.5" },
            { "spindle_speed", "-3000.0" },
            { "spindle_enabled", "true" },
            { "spindle_override", "1.1" },
            { "optional_stop", "true" },
            { "linear_units", "0.03937007874" }
        };

        /// <summary>When <c>true</c>, status messages are not sent (simulates a hung server).</summary>
        public bool Silent { get; set; }

        /// <summary>The number of connections accepted.</summary>
        public int ConnectionCount => _connectionCount;


        /// <summary>Starts the server on the specified port (0 = any free port).</summary>
        /// <param name="port">The TCP port.</param>
        public FakeStatusServer(int port = 0)
        {
            _listener = new TcpListener(IPAddress.Loopback, port);
            _listener.Start();
            Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            _ = Task.Run(Accept);
            _ = Task.Run(Broadcast);
        }

        private async Task Accept()
        {
            while (!_stop.IsCancellationRequested)
            {
                TcpClient client;
                try { client = await _listener.AcceptTcpClientAsync(_stop.Token); }
                catch { return; }

                Interlocked.Increment(ref _connectionCount);
                Send(client, Info);
                lock (_clients) _clients.Add(client);
            }
        }

        private async Task Broadcast()
        {
            while (!_stop.IsCancellationRequested)
            {
                if (!Silent)
                {
                    string message;
                    lock (Status) message = "{\"type\":\"status\"," + string.Join(",", Status.Select(o => $"\"{o.Key}\":{o.Value}")) + "}";

                    lock (_clients)
                    {
                        foreach (var client in _clients.ToList())
                        {
                            if (!Send(client, message)) _clients.Remove(client);
                        }
                    }
                }

                try { await Task.Delay(50, _stop.Token); }
                catch { return; }
            }
        }

        /// <summary>Sets a status field (a JSON value).</summary>
        /// <param name="key">The field name.</param>
        /// <param name="json">The JSON value.</param>
        public void Set(string key, string json)
        {
            lock (Status) Status[key] = json;
        }

        private static bool Send(TcpClient client, string message)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(message + "\n");
                client.GetStream().Write(bytes, 0, bytes.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Stops the server and closes every connection.</summary>
        public void Dispose()
        {
            _stop.Cancel();
            _listener.Stop();
            lock (_clients)
            {
                foreach (var client in _clients) client.Close();
                _clients.Clear();
            }
        }
    }
}
