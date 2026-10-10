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
        [TestCase(">MODE, ZERO", ControllerMode.MANUAL)]
        [TestCase("MODE, LIST PROG", ControllerMode.EDIT)]
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

        /// <summary>Replies are matched to the command that was sent by their label.</summary>
        [TestCase(">SERIAL NUMBER, 1234567", "?Q100", true)]
        [TestCase(">M30 #1, 380", "?Q100", false)]
        [TestCase(">STATUS, BUSY", "?Q500", true)]
        [TestCase(">PROGRAM, MDI, IDLE, PARTS, 380", "?Q500", true)]
        [TestCase(">MACRO, 0.0", "?Q600 3027", true)]
        [TestCase(">MODEL, VF-2", "?Q104", false)]
        [TestCase(">MODE, MDI", "?Q102", false)]
        [TestCase(">?, ?Q105", "?Q105", true)]
        [TestCase(">?, ?Q105", "?Q303", false)]
        [TestCase("", "?Q100", false)]
        public void IsReplyTo_matches_reply_label(string reply, string command, bool expected)
        {
            Assert.That(HaasResponse.IsReplyTo(reply, command), Is.EqualTo(expected));
        }

        /// <summary>The "unrecognized command" reply is detected.</summary>
        [TestCase(">?, ?Q105", true)]
        [TestCase(">SERIAL NUMBER, 1234567", false)]
        [TestCase(null, false)]
        public void IsUnsupported_detects_unrecognized_command(string reply, bool expected)
        {
            Assert.That(HaasResponse.IsUnsupported(reply), Is.EqualTo(expected));
        }

        /// <summary>Single value replies return the value following the label.</summary>
        [TestCase(">SOFTWARE VERSION, 100.17.000.2037", "SOFTWARE VERSION", "100.17.000.2037")]
        [TestCase(">MODEL, CSMD-G2", "MODEL", "CSMD-G2")]
        [TestCase(">USING TOOL, 4", "USING TOOL", "4")]
        [TestCase(">MODEL, CSMD-G2", "MODE", null)]
        [TestCase(">MODEL, ", "MODEL", null)]
        public void ParseValue_returns_value(string reply, string label, string expected)
        {
            Assert.That(HaasResponse.ParseValue(reply, label), Is.EqualTo(expected));
        }

        /// <summary>Counter replies are parsed as integers.</summary>
        [TestCase(">M30 #1, 380", "M30 #1", 380)]
        [TestCase(">TOOL CHANGES, 35", "TOOL CHANGES", 35)]
        public void ParseInteger_returns_value(string reply, string label, int expected)
        {
            Assert.That(HaasResponse.ParseInteger(reply, label), Is.EqualTo(expected));
        }

        /// <summary>Timer replies (hours:minutes:seconds) are converted to seconds.</summary>
        [TestCase(">P.O. TIME, 06282:17:13", "P.O. TIME", 22616233L)]
        [TestCase(">C.S. TIME, 00098:18:29", "C.S. TIME", 353909L)]
        [TestCase(">LAST CYCLE, 00000:00:13", "LAST CYCLE", 13L)]
        public void ParseDuration_returns_seconds(string reply, string label, long expected)
        {
            Assert.That(HaasResponse.ParseDuration(reply, label), Is.EqualTo(expected));
        }

        /// <summary>Invalid timer replies are not parsed.</summary>
        [TestCase(">P.O. TIME, 06282:17", "P.O. TIME")]
        [TestCase(">P.O. TIME, abc", "P.O. TIME")]
        [TestCase(">LAST CYCLE, 00000:00:13", "PREV CYCLE")]
        public void ParseDuration_returns_null_when_not_recognized(string reply, string label)
        {
            Assert.That(HaasResponse.ParseDuration(reply, label), Is.Null);
        }
    }
}
