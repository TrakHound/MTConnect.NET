// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Fanuc;
using NUnit.Framework;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MTConnect.AgentModule.FanucAdapter.Tests
{
    /// <summary>
    /// Tests the decoding helpers of <see cref="FocasClient"/>, and the native FOCAS library
    /// when it is present next to the tests (it is not included in the repository).
    /// </summary>
    [TestFixture]
    public class FocasClientTests
    {
        /// <summary>FOCAS integer values are scaled by their number of decimal places.</summary>
        [TestCase(123456, (short)3, 123.456)]
        [TestCase(-1500, (short)4, -0.15)]
        [TestCase(8000, (short)0, 8000.0)]
        public void Scale_applies_decimal_places(int data, short dec, double expected)
        {
            Assert.That(FocasClient.Scale(data, dec), Is.EqualTo(expected).Within(1e-9));
        }

        /// <summary>Axis and spindle names include their suffix; 0 and space are ignored.</summary>
        [TestCase("X\0", "X")]
        [TestCase("X1", "X1")]
        [TestCase("S1  ", "S1")]
        [TestCase("Z ", "Z")]
        public void ReadName_includes_suffix(string bytes, string expected)
        {
            var buffer = Encoding.ASCII.GetBytes(bytes);
            Assert.That(FocasClient.ReadName(buffer, 0, buffer.Length), Is.EqualTo(expected));
        }

        /// <summary>
        /// With the FOCAS library present, connecting to a port with no CNC fails with a socket error
        /// (this checks the library loads and cnc_allclibhndl3 is called correctly).
        /// </summary>
        [Test]
        public void Connect_fails_with_socket_error_when_no_cnc()
        {
            // Find a free port with nothing listening
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            using (var client = new FocasClient())
            {
                var code = client.Connect("127.0.0.1", port, 2);
                if (code == FocasReturnCodes.EW_NODLL) Assert.Ignore("FOCAS library (Fwlib64.dll / Fwlib32.dll) is not present");

                Assert.That(code, Is.EqualTo(FocasReturnCodes.EW_SOCKET));
                Assert.That(client.IsConnected, Is.False);
            }
        }
    }
}
