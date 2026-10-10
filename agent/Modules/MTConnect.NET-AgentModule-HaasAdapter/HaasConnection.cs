// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Logging;
using System;
using System.IO;
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
        /// Sends a Q-command (ex. <c>?Q500</c>) and returns the raw reply.
        /// </summary>
        /// <param name="command">The command to send, without a line terminator.</param>
        /// <returns>The raw reply, or <c>null</c> if the connection is not open or no reply was received.
        /// A failed exchange closes the connection so the next call reconnects.</returns>
        public string SendCommand(string command)
        {
            lock (_lock)
            {
                var stream = _stream;
                if (stream == null) return null;

                try
                {
                    DiscardInput(stream);

                    var data = Encoding.ASCII.GetBytes(command + "\r\n");
                    stream.Write(data, 0, data.Length);
                    stream.Flush();

                    return ReadReply(stream);
                }
                catch (Exception ex)
                {
                    Log(MTConnectLogLevel.Warning, $"Command '{command}' Failed : {ex.Message}");
                    CloseInternal();
                    return null;
                }
            }
        }


        private static string ReadReply(Stream stream)
        {
            var buffer = new MemoryStream();
            var hasContent = false;

            while (true)
            {
                // Throws IOException / TimeoutException when ResponseTimeout expires
                var b = stream.ReadByte();
                if (b < 0) throw new IOException("Connection closed by remote host");

                if (b == Stx) continue;
                if (b == Etb && hasContent) break;
                if ((b == Prompt || b == Lf) && hasContent) break;

                if (b != Prompt && !char.IsWhiteSpace((char)b)) hasContent = true;
                if (hasContent) buffer.WriteByte((byte)b);
            }

            return Encoding.ASCII.GetString(buffer.ToArray());
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
