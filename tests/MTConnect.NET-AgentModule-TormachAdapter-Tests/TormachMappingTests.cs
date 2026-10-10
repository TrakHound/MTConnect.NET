// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events;
using MTConnect.Tormach;
using NUnit.Framework;
using System.Linq;

namespace MTConnect.AgentModule.TormachAdapter.Tests
{
    /// <summary>
    /// Pins the parsing of status server messages and the conversion of LinuxCNC values to MTConnect values.
    /// </summary>
    [TestFixture]
    public class TormachMappingTests
    {
        /// <summary>Task modes map to ControllerMode.</summary>
        [TestCase("AUTO", ControllerMode.AUTOMATIC)]
        [TestCase("MDI", ControllerMode.MANUAL_DATA_INPUT)]
        [TestCase("MANUAL", ControllerMode.MANUAL)]
        public void MapControllerMode_maps_task_mode(string taskMode, ControllerMode expected)
        {
            Assert.That(TormachMapping.MapControllerMode(taskMode), Is.EqualTo(expected));
        }

        /// <summary>Task and interpreter states map to Execution.</summary>
        [TestCase("ON", "IDLE", false, false, Execution.READY)]
        [TestCase("ON", "READING", false, false, Execution.ACTIVE)]
        [TestCase("ON", "WAITING", false, false, Execution.ACTIVE)]
        [TestCase("ON", "READING", true, false, Execution.FEED_HOLD)]
        [TestCase("ON", "PAUSED", false, false, Execution.FEED_HOLD)]
        [TestCase("OFF", "IDLE", false, false, Execution.STOPPED)]
        [TestCase("ESTOP", "IDLE", false, true, Execution.STOPPED)]
        [TestCase("ON", "READING", false, true, Execution.STOPPED)]
        public void MapExecution_maps_state(string taskState, string interpState, bool paused, bool estop, Execution expected)
        {
            var status = new TormachStatus { TaskState = taskState, InterpState = interpState, Paused = paused, Estop = estop };
            Assert.That(TormachMapping.MapExecution(status), Is.EqualTo(expected));
        }

        /// <summary>The program name is the file name without the directory.</summary>
        [TestCase("/home/operator/gcode/bracket.nc", "bracket.nc")]
        [TestCase("bracket.nc", "bracket.nc")]
        [TestCase("", "")]
        [TestCase(null, "")]
        public void GetProgramName_strips_directory(string file, string expected)
        {
            Assert.That(TormachMapping.GetProgramName(file), Is.EqualTo(expected));
        }

        /// <summary>The g5x index maps to the work offset number.</summary>
        [TestCase(1, "54")]
        [TestCase(6, "59")]
        [TestCase(7, "59.1")]
        [TestCase(9, "59.3")]
        [TestCase(0, null)]
        [TestCase(10, null)]
        public void GetWorkOffset_maps_index(int index, string expected)
        {
            Assert.That(TormachMapping.GetWorkOffset(index), Is.EqualTo(expected));
        }

        /// <summary>Values in inch machine units are converted to millimeters.</summary>
        [TestCase(1.0, 1.0 / 25.4, 25.4)]
        [TestCase(10.0, 1.0, 10.0)]
        [TestCase(10.0, null, 10.0)]
        public void ToMillimeters_converts_units(double value, double? linearUnits, double expected)
        {
            Assert.That(TormachMapping.ToMillimeters(value, linearUnits), Is.EqualTo(expected).Within(1e-9));
        }

        /// <summary>The work position is the machine position minus the work, G92 and tool offsets.</summary>
        [Test]
        public void GetWorkPosition_subtracts_offsets()
        {
            var status = new TormachStatus
            {
                ActualPosition = new[] { 1.5, 0, 0.1 },
                G5xOffset = new[] { 1.0, 0, 0 },
                G92Offset = new[] { 0.25, 0, 0 },
                ToolOffset = new[] { 0, 0, 0.5 }
            };

            Assert.That(TormachMapping.GetWorkPosition(status, 0), Is.EqualTo(0.25).Within(1e-9));
            Assert.That(TormachMapping.GetWorkPosition(status, 2), Is.EqualTo(-0.4).Within(1e-9));
            Assert.That(TormachMapping.GetWorkPosition(status, 5), Is.Null);
        }

        /// <summary>Info messages are parsed; axes are read from the coordinates.</summary>
        [Test]
        public void Parse_reads_info()
        {
            var info = (TormachInfo)TormachMessageParser.Parse("{\"type\":\"info\",\"version\":\"1.0\",\"machine\":\"tormach_mill_1100\",\"coordinates\":\"X Y Y Z A\",\"linear_units\":0.0393}");

            Assert.That(info.Machine, Is.EqualTo("tormach_mill_1100"));
            Assert.That(info.GetAxes().ToArray(), Is.EqualTo(new[] { "X", "Y", "Z", "A" }), "duplicate (gantry) axes are removed");
            Assert.That(info.LinearUnits, Is.EqualTo(0.0393));
        }

        /// <summary>Status messages are parsed.</summary>
        [Test]
        public void Parse_reads_status()
        {
            var status = (TormachStatus)TormachMessageParser.Parse(
                "{\"type\":\"status\",\"available\":true,\"task_mode\":\"AUTO\",\"interp_state\":\"READING\",\"line\":12,\"actual_position\":[1.5,2],\"spindle_speed\":null,\"optional_stop\":true}");

            Assert.That(status.Available, Is.True);
            Assert.That(status.TaskMode, Is.EqualTo("AUTO"));
            Assert.That(status.Line, Is.EqualTo(12));
            Assert.That(status.ActualPosition, Is.EqualTo(new[] { 1.5, 2.0 }));
            Assert.That(status.SpindleSpeed, Is.Null);
            Assert.That(status.OptionalStop, Is.True);
        }

        /// <summary>Unknown and invalid lines are ignored.</summary>
        [TestCase("{\"type\":\"other\"}")]
        [TestCase("not json")]
        [TestCase("[1,2]")]
        [TestCase("")]
        public void Parse_ignores_unknown_lines(string line)
        {
            Assert.That(TormachMessageParser.Parse(line), Is.Null);
        }
    }
}
