// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.AgentModule.HaasAdapter.Tests
{
    /// <summary>
    /// A TCP server that answers Haas Q-commands the way an NGC control does
    /// (replies start with '&gt;' and end with CR/LF; unknown commands reply "?, ?Qxxx").
    /// </summary>
    public class FakeHaasControl : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _stop = new CancellationTokenSource();
        private readonly List<TcpClient> _clients = new List<TcpClient>();
        private int _connectionCount;


        /// <summary>The TCP port the control listens on.</summary>
        public int Port { get; }

        /// <summary>When <c>true</c>, replies are framed with STX / ETB (older controls) instead of the NGC '&gt;' prefix.</summary>
        public bool Framed { get; set; }

        /// <summary>When <c>true</c>, the first ?Q100 on each connection is not answered (as documented for the NGC).</summary>
        public bool IgnoreFirstQ100 { get; set; }

        /// <summary>When <c>true</c>, a reply to another client's command is sent before every reply.</summary>
        public bool SendOtherClientReplies { get; set; }

        /// <summary>Commands that reply as unrecognized.</summary>
        public HashSet<string> UnsupportedCommands { get; } = new HashSet<string>();

        /// <summary>The ?Q104 mode.</summary>
        public string Mode { get; set; } = "MEM";

        /// <summary>The ?Q500 reply.</summary>
        public string Q500 { get; set; } = "PROGRAM, O01234, IDLE, PARTS, 42";

        /// <summary>Every command received, in order.</summary>
        public ConcurrentQueue<string> Commands { get; } = new ConcurrentQueue<string>();

        /// <summary>The number of connections accepted.</summary>
        public int ConnectionCount => _connectionCount;


        /// <summary>Starts the control on the specified port (0 = any free port).</summary>
        /// <param name="port">The TCP port.</param>
        public FakeHaasControl(int port = 0)
        {
            _listener = new TcpListener(IPAddress.Loopback, port);
            _listener.Start();
            Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            _ = Task.Run(Accept);
        }

        /// <summary>Returns how many times a command was received.</summary>
        /// <param name="command">The command.</param>
        /// <returns>The number of times it was received.</returns>
        public int CountOf(string command) => Commands.Count(o => o == command);


        private async Task Accept()
        {
            while (!_stop.IsCancellationRequested)
            {
                TcpClient client;
                try { client = await _listener.AcceptTcpClientAsync(_stop.Token); }
                catch { return; }

                Interlocked.Increment(ref _connectionCount);
                lock (_clients) _clients.Add(client);
                _ = Task.Run(() => Serve(client));
            }
        }

        private async Task Serve(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                var reader = new StreamReader(stream, Encoding.ASCII);
                var firstQ100 = true;

                while (true)
                {
                    var command = await reader.ReadLineAsync();
                    if (command == null) return;

                    Commands.Enqueue(command);

                    if (command == "?Q100" && firstQ100)
                    {
                        firstQ100 = false;
                        if (IgnoreFirstQ100) continue;
                    }

                    var reply = new StringBuilder();
                    if (SendOtherClientReplies) reply.Append(Format("M30 #1, 999"));
                    reply.Append(Format(GetReply(command)));

                    var bytes = Encoding.ASCII.GetBytes(reply.ToString());
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
            catch { }
        }

        private string Format(string body) => Framed ? $"\u0002{body}\u0017\r\n>" : $">{body}\r\n";

        private string GetReply(string command)
        {
            if (UnsupportedCommands.Contains(command)) return $"?, {command}";

            switch (command)
            {
                case "?Q100": return "SERIAL NUMBER, 1234567";
                case "?Q101": return "SOFTWARE VERSION, 100.24.000.1200";
                case "?Q102": return "MODEL, VF-2";
                case "?Q104": return $"MODE, {Mode}";
                case "?Q201": return "USING TOOL, 4";
                case "?Q300": return "P.O. TIME, 06282:17:13";
                case "?Q301": return "C.S. TIME, 00098:18:29";
                case "?Q303": return "LAST CYCLE, 00000:00:13";
                case "?Q304": return "PREV CYCLE, 00000:01:05";
                case "?Q403": return "M30 #2, 17";
                case "?Q500": return Q500;
                case "?Q600 5021": return "MACRO, 1.5";
                case "?Q600 5022": return "MACRO, -2.25";
                case "?Q600 5023": return "MACRO, 0.1";
                case "?Q600 5024": return "MACRO, 90.5";
                case "?Q600 5041": return "MACRO, 0.5";
                case "?Q600 5042": return "MACRO, 0.25";
                case "?Q600 5043": return "MACRO, -1.";
                case "?Q600 5044": return "MACRO, 45.";
                case "?Q600 3027": return "MACRO, 8000.";
                case "?Q600 1098": return "MACRO, 23.4";
                case "?Q600 4132": return "MACRO, 54.";
                case "?Q600 3030": return "MACRO, 1.";
                case "?Q600 3033": return "MACRO, 0.";
                case "?Q600 13013": return "MACRO, 75.";
                default: return $"?, {command}";
            }
        }

        /// <summary>Stops the control and closes every connection.</summary>
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
