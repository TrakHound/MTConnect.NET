// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Fanuc;
using MTConnect.Observations.Events;
using NUnit.Framework;

namespace MTConnect.AgentModule.FanucAdapter.Tests
{
    /// <summary>
    /// Pins the conversion of FOCAS values to MTConnect values.
    /// </summary>
    [TestFixture]
    public class FanucMappingTests
    {
        /// <summary>ODBST aut values map to ControllerMode.</summary>
        [TestCase(0, ControllerMode.MANUAL_DATA_INPUT)]
        [TestCase(1, ControllerMode.AUTOMATIC)]
        [TestCase(10, ControllerMode.AUTOMATIC)]
        [TestCase(3, ControllerMode.EDIT)]
        [TestCase(4, ControllerMode.MANUAL)]
        [TestCase(5, ControllerMode.MANUAL)]
        [TestCase(9, ControllerMode.MANUAL)]
        public void MapControllerMode_maps_known_modes(short mode, ControllerMode expected)
        {
            Assert.That(FanucMapping.MapControllerMode(mode), Is.EqualTo(expected));
        }

        /// <summary>No selected mode and unknown values are not mapped.</summary>
        [TestCase(2)]
        [TestCase(99)]
        public void MapControllerMode_returns_null_for_no_mode(short mode)
        {
            Assert.That(FanucMapping.MapControllerMode(mode), Is.Null);
        }

        /// <summary>ODBST run values map to Execution; emergency stop reports STOPPED.</summary>
        [TestCase(0, 0, Execution.READY)]
        [TestCase(1, 0, Execution.STOPPED)]
        [TestCase(2, 0, Execution.FEED_HOLD)]
        [TestCase(3, 0, Execution.ACTIVE)]
        [TestCase(4, 0, Execution.INTERRUPTED)]
        [TestCase(3, 1, Execution.STOPPED)]
        public void MapExecution_maps_run_status(short run, short emergency, Execution expected)
        {
            Assert.That(FanucMapping.MapExecution(run, emergency), Is.EqualTo(expected));
        }

        /// <summary>The emergency status maps to EmergencyStop.</summary>
        [TestCase(0, EmergencyStop.ARMED)]
        [TestCase(1, EmergencyStop.TRIGGERED)]
        [TestCase(2, EmergencyStop.ARMED)]
        public void MapEmergencyStop_maps_emergency(short emergency, EmergencyStop expected)
        {
            Assert.That(FanucMapping.MapEmergencyStop(emergency), Is.EqualTo(expected));
        }

        /// <summary>Alarm status values above 1 are warnings.</summary>
        [TestCase(0, null)]
        [TestCase(1, null)]
        [TestCase(2, "BATTERY LOW")]
        [TestCase(3, "FAN ALARM")]
        public void GetWarningMessage_returns_warning(short alarm, string expected)
        {
            Assert.That(FanucMapping.GetWarningMessage(alarm), Is.EqualTo(expected));
        }

        /// <summary>Alarm codes are the type prefix followed by the 4 digit number.</summary>
        [TestCase(3, 10, "PS0010")]
        [TestCase(6, 401, "SV0401")]
        [TestCase(4, 500, "OT0500")]
        [TestCase(99, 1, "AL0001")]
        public void GetAlarmCode_formats_code(short type, int number, string expected)
        {
            Assert.That(FanucMapping.GetAlarmCode(new FocasAlarm { Type = type, Number = number }), Is.EqualTo(expected));
        }

        /// <summary>Feedrates are converted to mm/s, including feed per revolution.</summary>
        [TestCase(600, 0, 0, 10)]
        [TestCase(60, 1, 0, 25.4)]
        [TestCase(0.2, 3, 1200, 4)]
        [TestCase(0.01, 4, 600, 2.54)]
        public void ToMillimetersPerSecond_converts_feedrate(double feedrate, short unit, double spindleSpeed, double expected)
        {
            Assert.That(FanucMapping.ToMillimetersPerSecond(feedrate, unit, spindleSpeed), Is.EqualTo(expected).Within(1e-9));
        }

        /// <summary>Inch positions are converted to millimeters; mm and degrees are not.</summary>
        [TestCase(1.0, 0, 1.0)]
        [TestCase(1.0, 1, 25.4)]
        [TestCase(90.0, 2, 90.0)]
        public void ToMillimeters_converts_inch(double position, short unit, double expected)
        {
            Assert.That(FanucMapping.ToMillimeters(position, unit), Is.EqualTo(expected).Within(1e-9));
        }

        /// <summary>The feedrate override is the inverse of G0012 (*FV, negative logic).</summary>
        [TestCase(155, 100)]
        [TestCase(255, 0)]
        [TestCase(105, 150)]
        public void GetFeedrateOverride_inverts_signals(int g0012, int expected)
        {
            Assert.That(FanucMapping.GetFeedrateOverride((byte)g0012), Is.EqualTo(expected));
        }

        /// <summary>The rapid override comes from ROV1 / ROV2, or the 1% signals when HROV is on.</summary>
        [TestCase(0, 0, 100)]
        [TestCase(1, 0, 50)]
        [TestCase(2, 0, 25)]
        [TestCase(3, 0, 0)]
        [TestCase(0, 0x80 | (0x7F & ~75), 75)]
        public void GetRapidOverride_reads_signals(int g0014, int g0096, int expected)
        {
            Assert.That(FanucMapping.GetRapidOverride((byte)g0014, (byte)g0096), Is.EqualTo(expected));
        }

        /// <summary>CNC interface addresses are offset by 1000 per path.</summary>
        [TestCase((ushort)12, 1, (ushort)12)]
        [TestCase((ushort)12, 2, (ushort)1012)]
        [TestCase((ushort)96, 3, (ushort)2096)]
        public void GetPathAddress_offsets_path(ushort address, int path, ushort expected)
        {
            Assert.That(FanucMapping.GetPathAddress(address, path), Is.EqualTo(expected));
        }

        /// <summary>The CNC model is built from the system information.</summary>
        [Test]
        public void SystemInfo_builds_model_and_firmware()
        {
            var info = new FocasSystemInfo { AddInfo = 2, CncType = "30", MtType = " M", Series = "G421", Version = "15.0" };

            Assert.That(info.Model, Is.EqualTo("30i-M"));
            Assert.That(info.Firmware, Is.EqualTo("G421 15.0"));
        }
    }
}
