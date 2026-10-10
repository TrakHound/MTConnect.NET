// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Haas;
using MTConnect.Observations.Events;
using NUnit.Framework;

namespace MTConnect.AgentModule.HaasAdapter.Tests
{
    /// <summary>
    /// Pins the parsing of Haas Q-command replies, including both framing
    /// styles (STX / ETB control characters and the '&gt;' prompt).
    /// </summary>
    [TestFixture]
    public class HaasResponseTests
    {
        /// <summary>Framing characters and the prompt are removed.</summary>
        [TestCase("\u0002SERIAL NUMBER, 1234567\u0017", "SERIAL NUMBER, 1234567")]
        [TestCase(">SERIAL NUMBER, 1234567\r\n", "SERIAL NUMBER, 1234567")]
        [TestCase("SERIAL NUMBER, 1234567\r\n>", "SERIAL NUMBER, 1234567")]
        [TestCase(">\r\n", null)]
        [TestCase(null, null)]
        public void Clean_removes_framing(string reply, string expected)
        {
            Assert.That(HaasResponse.Clean(reply), Is.EqualTo(expected));
        }

        /// <summary>Q104 modes map to MTConnect ControllerMode values.</summary>
        [TestCase("MODE, (MEM)", ControllerMode.AUTOMATIC)]
        [TestCase("\u0002MODE, (MDI)\u0017", ControllerMode.MANUAL_DATA_INPUT)]
        [TestCase(">MODE, (JOG)", ControllerMode.MANUAL)]
        [TestCase("MODE, (ZERO RET)", ControllerMode.MANUAL)]
        [TestCase("MODE, (EDIT)", ControllerMode.EDIT)]
        public void ParseMode_maps_known_modes(string reply, ControllerMode expected)
        {
            Assert.That(HaasResponse.ParseMode(reply), Is.EqualTo(expected));
        }

        /// <summary>Unknown modes and unrelated replies are not mapped.</summary>
        [TestCase("MODE, (SOMETHING NEW)")]
        [TestCase("PROGRAM, O01234, IDLE, PARTS, 42")]
        [TestCase("")]
        public void ParseMode_returns_null_when_not_recognized(string reply)
        {
            Assert.That(HaasResponse.ParseMode(reply), Is.Null);
        }

        /// <summary>A Q500 PROGRAM reply reports execution, program, part count and alarm state.</summary>
        [TestCase("PROGRAM, O01234, IDLE, PARTS, 42", Execution.READY, "O01234", 42, false)]
        [TestCase("\u0002PROGRAM, O01234, FEED HOLD, PARTS, 7\u0017", Execution.INTERRUPTED, "O01234", 7, false)]
        [TestCase(">PROGRAM, MDI, ALARM ON, PARTS, 0", Execution.STOPPED, "", 0, true)]
        public void ParseStatus_parses_program_reply(string reply, Execution execution, string program, int partCount, bool alarm)
        {
            var status = HaasResponse.ParseStatus(reply);

            Assert.That(status, Is.Not.Null);
            Assert.That(status.Execution, Is.EqualTo(execution));
            Assert.That(status.Program, Is.EqualTo(program));
            Assert.That(status.PartCount, Is.EqualTo(partCount));
            Assert.That(status.Alarm, Is.EqualTo(alarm));
        }

        /// <summary>BUSY (with or without the comma) reports ACTIVE and leaves the other values unknown.</summary>
        [TestCase("STATUS, BUSY")]
        [TestCase("\u0002STATUS BUSY\u0017")]
        public void ParseStatus_parses_busy_reply(string reply)
        {
            var status = HaasResponse.ParseStatus(reply);

            Assert.That(status, Is.Not.Null);
            Assert.That(status.Execution, Is.EqualTo(Execution.ACTIVE));
            Assert.That(status.Program, Is.Null);
            Assert.That(status.PartCount, Is.Null);
            Assert.That(status.Alarm, Is.Null);
        }

        /// <summary>An unknown program state is reported without an Execution value.</summary>
        [Test]
        public void ParseStatus_leaves_execution_unset_for_unknown_state()
        {
            var status = HaasResponse.ParseStatus("PROGRAM, O01234, SOMETHING, PARTS, 1");

            Assert.That(status, Is.Not.Null);
            Assert.That(status.Execution, Is.Null);
            Assert.That(status.PartCount, Is.EqualTo(1));
        }

        /// <summary>Unrelated replies are not parsed as a status.</summary>
        [TestCase("MODE, (MEM)")]
        [TestCase("PROGRAM, O01234")]
        [TestCase(null)]
        public void ParseStatus_returns_null_when_not_recognized(string reply)
        {
            Assert.That(HaasResponse.ParseStatus(reply), Is.Null);
        }

        /// <summary>Q600 replies are parsed using the invariant culture.</summary>
        [TestCase("MACRO, 123.4500", 123.45)]
        [TestCase("\u0002MACRO, -0.0010\u0017", -0.001)]
        [TestCase(">MACRO, 8000.", 8000.0)]
        public void ParseMacro_parses_value(string reply, double expected)
        {
            Assert.That(HaasResponse.ParseMacro(reply), Is.EqualTo(expected).Within(1e-9));
        }

        /// <summary>Invalid Q600 replies are not parsed.</summary>
        [TestCase("MACRO, ")]
        [TestCase("INVALID")]
        [TestCase(null)]
        public void ParseMacro_returns_null_when_not_recognized(string reply)
        {
            Assert.That(HaasResponse.ParseMacro(reply), Is.Null);
        }
    }
}
