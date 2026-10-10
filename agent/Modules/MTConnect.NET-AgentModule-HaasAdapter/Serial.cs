// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.IO.Ports;

namespace MTConnect.Haas
{
    /// <summary>
    /// RS-232 connection to a Haas control. The port is kept open between commands.
    /// </summary>
    public class Serial : HaasConnection
    {
        private readonly HaasSerialConfiguration _configuration;
        private SerialPort _port;


        /// <inheritdoc/>
        public override bool IsConnected => _port != null && _port.IsOpen;


        /// <summary>
        /// Initializes a new serial connection using the specified port settings.
        /// </summary>
        /// <param name="configuration">The serial port settings. These must match the RS-232 settings on the control (Settings 11-15).</param>
        public Serial(HaasSerialConfiguration configuration)
        {
            _configuration = configuration ?? new HaasSerialConfiguration();
        }


        /// <inheritdoc/>
        protected override Stream OnOpen()
        {
            var port = new SerialPort(_configuration.ComPort);
            port.BaudRate = _configuration.BaudRate;
            port.Parity = _configuration.GetParity();
            port.DataBits = _configuration.DataBits;
            port.StopBits = _configuration.GetStopBits();
            port.Handshake = Handshake.None;

            try
            {
                port.Open();
                _port = port;
                return port.BaseStream;
            }
            catch
            {
                port.Dispose();
                throw;
            }
        }

        /// <inheritdoc/>
        protected override void OnClose()
        {
            if (_port != null)
            {
                if (_port.IsOpen) _port.Close();
                _port.Dispose();
            }

            _port = null;
        }

        /// <inheritdoc/>
        protected override void DiscardInput(Stream stream)
        {
            if (_port != null && _port.IsOpen) _port.DiscardInBuffer();
        }
    }
}
