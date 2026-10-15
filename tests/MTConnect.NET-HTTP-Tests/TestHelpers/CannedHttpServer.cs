// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Tests.Http.TestHelpers
{
    /// <summary>
    /// A loopback TCP listener that answers the first connection with fixed bytes (then closes it), or
    /// accepts it and never answers, to drive the HTTP clients through exact framing and failure cases.
    /// </summary>
    internal sealed class CannedHttpServer : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _stop = new CancellationTokenSource();

        public int Port { get; }
        public string Authority => $"127.0.0.1:{Port}";
        public string BaseUrl => $"http://127.0.0.1:{Port}";


        private CannedHttpServer(Func<NetworkStream, CancellationToken, Task> respond)
        {
            _listener = new TcpListener(IPAddress.Loopback, 0);
            _listener.Start();
            Port = ((IPEndPoint)_listener.LocalEndpoint).Port;

            _ = Task.Run(async () =>
            {
                try
                {
                    while (!_stop.IsCancellationRequested)
                    {
                        var client = await _listener.AcceptTcpClientAsync();
                        _ = Task.Run(async () =>
                        {
                            using (client)
                            using (var stream = client.GetStream())
                            {
                                await ReadRequestHeadersAsync(stream);
                                await respond(stream, _stop.Token);
                            }
                        });
                    }
                }
                catch { }
            });
        }

        /// <summary>Responds with <paramref name="response"/> and closes the connection.</summary>
        public static CannedHttpServer Respond(byte[] response)
        {
            return new CannedHttpServer(async (stream, token) =>
            {
                await stream.WriteAsync(response, 0, response.Length, token);
                await stream.FlushAsync(token);
            });
        }

        /// <summary>Accepts the request and never responds.</summary>
        public static CannedHttpServer NeverRespond()
        {
            return new CannedHttpServer(async (stream, token) =>
            {
                try { await Task.Delay(Timeout.Infinite, token); } catch (OperationCanceledException) { }
            });
        }

        public static byte[] HttpResponse(string status, string contentType, byte[] body, string? extraHeaders = null)
        {
            var head = $"HTTP/1.1 {status}\r\nContent-Type: {contentType}\r\nContent-Length: {body.Length}\r\nConnection: close\r\n{extraHeaders}\r\n";
            return Concat(Encoding.ASCII.GetBytes(head), body);
        }

        public static byte[] MultipartResponseHead(string boundary)
        {
            return Encoding.ASCII.GetBytes($"HTTP/1.1 200 OK\r\nContent-Type: multipart/x-mixed-replace;boundary={boundary}\r\nConnection: close\r\n\r\n");
        }

        public static byte[] Concat(params byte[][] parts)
        {
            var length = 0;
            foreach (var part in parts) length += part.Length;
            var result = new byte[length];
            var offset = 0;
            foreach (var part in parts) { Buffer.BlockCopy(part, 0, result, offset, part.Length); offset += part.Length; }
            return result;
        }

        public void Dispose()
        {
            _stop.Cancel();
            _listener.Stop();
        }

        private static async Task ReadRequestHeadersAsync(NetworkStream stream)
        {
            // Read until the blank line that ends the request headers (GET requests carry no body)
            var buffer = new byte[1];
            var last4 = 0;
            while (await stream.ReadAsync(buffer, 0, 1) == 1)
            {
                last4 = (last4 << 8) | buffer[0];
                if (last4 == 0x0D0A0D0A) return; // "\r\n\r\n"
            }
        }
    }
}
