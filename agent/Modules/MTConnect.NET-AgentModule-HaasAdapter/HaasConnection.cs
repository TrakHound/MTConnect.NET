// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MTConnect.Haas
{
    /// <summary>
    /// Base class for a connection to a Haas control that exchanges Q-commands
    /// over a byte stream (TCP or RS-232). Handles writing the command and
    /// reading the reply until a terminator is received or the response
    /// timeout expires.
    /// </summary>
    public abstract class HaasConnection : IDisposable
    {
        private const byte Stx = 0x02;
        private const byte Etb = 0x17;
        private const byte Prompt = (byte)'>';
        private const byte Lf = (byte)'\n';

        private readonly object _lock = new object();
        private bool _connectFailed;


        /// <summary>
        /// Maximum time (in milliseconds) to wait for a reply to a command.
        /// </summary>
        public int ResponseTimeout { get; set; } = 2000;

        /// <summary>
        /// Gets whether the underlying connection is currently open.
        /// </summary>
        public abstract bool IsConnected { get; }

        /// <summary>
        /// Raised when the connection emits a log message.
        /// </summary>
        public event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Opens the underlying connection and returns the stream used to exchange commands.
        /// </summary>
        /// <returns>The open stream, or <c>null</c> if the connection could not be opened.</returns>
        protected abstract Stream OnOpen();

        /// <summary>
        /// Closes the underlying connection and releases its resources.
        /// </summary>
        protected abstract void OnClose();


        private Stream _stream;

        // Receive buffer: replies are read in blocks rather than one byte (one socket / serial read) at a time
        private readonly byte[] _rx = new byte[4096];
        private int _rxPos;
        private int _rxLen;

        /// <summary>
        /// Opens the connection if it is not already open.
        /// </summary>
        /// <returns><c>true</c> if the connection is open.</returns>
        public bool Connect()
        {
            lock (_lock)
            {
                if (_stream != null && IsConnected) return true;

                CloseInternal();

                try
                {
                    _stream = OnOpen();
                    if (_stream != null)
                    {
                        _stream.ReadTimeout = ResponseTimeout;
                        _stream.WriteTimeout = ResponseTimeout;
                        _connectFailed = false;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // Only log the first failure so an offline machine doesn't flood the log
                    Log(_connectFailed ? MTConnectLogLevel.Debug : MTConnectLogLevel.Warning, $"Connection Failed : {ex.Message}");
                    _connectFailed = true;
                    CloseInternal();
                }

                return false;
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            lock (_lock) CloseInternal();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Dispose() => Close();


        /// <summary>
        /// Sends a Q-command (ex. <c>?Q500</c>) and returns the raw reply. Replies that belong to
        /// other commands (ex. sent by another client connected to the control) are ignored.
        /// </summary>
        /// <param name="command">The command to send, without a line terminator.</param>
        /// <param name="attempts">The number of times to send the command if no reply is received before
        /// <see cref="ResponseTimeout"/> expires. The control may not answer the first command after connecting.</param>
        /// <returns>The raw reply, or <c>null</c> if the connection is not open or no reply was received.
        /// A failed exchange closes the connection so the next call reconnects.</returns>
        public string SendCommand(string command, int attempts = 1)
        {
            lock (_lock)
            {
                var stream = _stream;
                if (stream == null) return null;

                for (var attempt = 1; ; attempt++)
                {
                    try
                    {
                        DiscardInput(stream);
                        _rxPos = _rxLen = 0; // Buffered bytes are stale input too

                        var data = Encoding.ASCII.GetBytes(command + "\r\n");
                        stream.Write(data, 0, data.Length);
                        stream.Flush();

                        return ReadReply(stream, command);
                    }
                    catch (Exception ex) when (attempt < attempts && IsTimeout(ex))
                    {
                        Log(MTConnectLogLevel.Debug, $"No reply to '{command}', retrying");
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"Command '{command}' Failed : {ex.Message}");
                        CloseInternal();
                        return null;
                    }
                }
            }
        }


        private string ReadReply(Stream stream, string command)
        {
            var timer = Stopwatch.StartNew();

            while (true)
            {
                var line = ReadLine(stream, timer);
                if (HaasResponse.IsReplyTo(line, command)) return line;

                Log(MTConnectLogLevel.Trace, $"Ignoring reply to another command : {line}");
            }
        }

        private int ReadByteBuffered(Stream stream)
        {
            if (_rxPos >= _rxLen)
            {
                // Returns as soon as any bytes are available (throws when ResponseTimeout expires)
                _rxLen = stream.Read(_rx, 0, _rx.Length);
                _rxPos = 0;
                if (_rxLen <= 0)
                {
                    _rxLen = 0;
                    return -1;
                }
            }

            return _rx[_rxPos++];
        }

        private string ReadLine(Stream stream, Stopwatch timer)
        {
            var buffer = new MemoryStream();
            var hasContent = false;

            while (true)
            {
                if (timer.ElapsedMilliseconds > ResponseTimeout) throw new TimeoutException("Timed out waiting for reply");

                // Throws IOException / TimeoutException when ResponseTimeout expires
                var b = ReadByteBuffered(stream);
                if (b < 0) throw new IOException("Connection closed by remote host");

                if (b == Stx) continue;
                if (b == Etb && hasContent) break;
                if ((b == Prompt || b == Lf) && hasContent) break;

                if (b != Prompt && !char.IsWhiteSpace((char)b)) hasContent = true;
                if (hasContent) buffer.WriteByte((byte)b);
            }

            return Encoding.ASCII.GetString(buffer.ToArray());
        }

        private static bool IsTimeout(Exception ex)
        {
            if (ex is TimeoutException) return true;

            var socketException = ex.InnerException as SocketException;
            return ex is IOException && socketException != null && socketException.SocketErrorCode == SocketError.TimedOut;
        }

        /// <summary>
        /// Drops any unread input (ex. the trailing '>' prompt or CR/LF from the previous reply)
        /// so it is not mistaken for the reply to the next command.
        /// </summary>
        /// <param name="stream">The stream returned by <see cref="OnOpen"/>.</param>
        protected virtual void DiscardInput(Stream stream) { }

        private void CloseInternal()
        {
            try
            {
                if (_stream != null) _stream.Dispose();
            }
            catch { }

            _stream = null;
            _rxPos = _rxLen = 0;

            try
            {
                OnClose();
            }
            catch { }
        }

        /// <summary>
        /// Emits a log message to subscribers of <see cref="LogReceived"/>.
        /// </summary>
        /// <param name="logLevel">The severity of the log message.</param>
        /// <param name="message">The log message text.</param>
        protected void Log(MTConnectLogLevel logLevel, string message)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message);
        }
    }
}
