// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Haas;
using System;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for the Haas Adapter Agent Module (<c>haas-adapter</c> in <c>agent.config.yaml</c>).
    /// </summary>
    public class HaasAdapterModuleConfiguration : HaasConfiguration
    {
        /// <summary>
        /// Value of <see cref="Connection"/> to connect over Ethernet.
        /// </summary>
        public const string EthernetConnection = "ethernet";

        /// <summary>
        /// Value of <see cref="Connection"/> to connect over RS-232.
        /// </summary>
        public const string SerialConnection = "serial";


        /// <summary>
        /// The UUID or Name of the Device that observations are added to.
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// When <c>true</c>, the module adds a Device (named <see cref="DeviceKey"/>) with the DataItems it reports.
        /// Set to <c>false</c> to use a Device defined in a Devices file instead. Default is <c>true</c>.
        /// </summary>
        public bool CreateDevice { get; set; } = true;

        /// <summary>
        /// The UUID of the Device created when <see cref="CreateDevice"/> is <c>true</c>. Defaults to <see cref="DeviceKey"/>.
        /// </summary>
        public string DeviceUuid { get; set; }

        /// <summary>
        /// The interval (in milliseconds) between reads of the Haas control. Default is 1000.
        /// </summary>
        public int ReadInterval { get; set; } = 1000;

        /// <summary>
        /// The connection type: <c>ethernet</c> or <c>serial</c>. Default is <c>ethernet</c>.
        /// </summary>
        public string Connection { get; set; } = EthernetConnection;


        /// <summary>
        /// The hostname or IP address of the Haas control (Ethernet).
        /// </summary>
        public string Server { get; set; } = "localhost";

        /// <summary>
        /// The TCP port of the Haas control (Ethernet, Setting 143). Default is 5051.
        /// </summary>
        public int Port { get; set; } = 5051;

        /// <summary>
        /// Maximum time (in milliseconds) to wait for the TCP connection to open (Ethernet). Default is 5000.
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5000;


        /// <summary>
        /// The serial port name (Serial, ex. <c>COM1</c> or <c>/dev/ttyUSB0</c>).
        /// </summary>
        public string ComPort { get; set; } = "COM1";

        /// <summary>
        /// The baud rate (Serial, Setting 11). Default is 19200.
        /// </summary>
        public int BaudRate { get; set; } = 19200;

        /// <summary>
        /// The parity (Serial, Setting 12): <c>None</c>, <c>Even</c>, <c>Odd</c>, <c>Mark</c> or <c>Space</c>. Default is <c>None</c>.
        /// </summary>
        public string Parity { get; set; } = "None";

        /// <summary>
        /// The number of data bits (Serial, Setting 13). Default is 7.
        /// </summary>
        public int DataBits { get; set; } = 7;

        /// <summary>
        /// The number of stop bits (Serial): <c>1</c> or <c>2</c>. Default is 1.
        /// </summary>
        public int StopBits { get; set; } = 1;


        /// <summary>
        /// Gets whether <see cref="Connection"/> is set to <c>serial</c>.
        /// </summary>
        public bool IsSerial => string.Equals(Connection, SerialConnection, StringComparison.OrdinalIgnoreCase);


        /// <summary>
        /// Returns the Ethernet settings from this configuration.
        /// </summary>
        /// <returns>A new <see cref="HaasEthernetConfiguration"/>.</returns>
        public HaasEthernetConfiguration ToEthernetConfiguration()
        {
            var configuration = new HaasEthernetConfiguration
            {
                Server = Server,
                Port = Port,
                ConnectionTimeout = ConnectionTimeout
            };
            CopyTo(configuration);
            return configuration;
        }

        /// <summary>
        /// Returns the serial port settings from this configuration.
        /// </summary>
        /// <returns>A new <see cref="HaasSerialConfiguration"/>.</returns>
        public HaasSerialConfiguration ToSerialConfiguration()
        {
            var configuration = new HaasSerialConfiguration
            {
                ComPort = ComPort,
                BaudRate = BaudRate,
                Parity = Parity,
                DataBits = DataBits,
                StopBits = StopBits
            };
            CopyTo(configuration);
            return configuration;
        }
    }
}
