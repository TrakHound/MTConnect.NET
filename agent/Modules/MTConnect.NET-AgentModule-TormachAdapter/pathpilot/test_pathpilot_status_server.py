# Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
# TrakHound Inc. licenses this file to you under the MIT license.

"""
Tests for pathpilot_status_server.py using a fake linuxcnc module.

Run with:  python -m unittest test_pathpilot_status_server
"""

import json
import socket
import time
import unittest

import pathpilot_status_server as server


class FakeIni(object):
    def __init__(self, path):
        self.values = {("EMC", "MACHINE"): "tormach_mill_1100", ("TRAJ", "COORDINATES"): "X Y Z A"}

    def find(self, section, key):
        return self.values.get((section, key))


class FakeStat(object):
    """A linuxcnc.stat() with LinuxCNC 2.7 style spindle attributes."""

    def __init__(self):
        self.running = True
        self.task_mode = 2
        self.task_state = 4
        self.interp_state = 2
        self.estop = 0
        self.paused = False
        self.file = "/home/operator/gcode/bracket.nc"
        self.current_line = 120
        self.tool_in_spindle = 5
        self.g5x_index = 1
        self.actual_position = (1.5, -2.25, 0.1, 90.5, 0, 0, 0, 0, 0)
        self.g5x_offset = (1.0, -2.5, 0, 0, 0, 0, 0, 0, 0)
        self.g92_offset = (0,) * 9
        self.tool_offset = (0, 0, 0.5, 0, 0, 0, 0, 0, 0)
        self.current_vel = 0.25
        self.feedrate = 1.0
        self.rapidrate = 0.5
        self.spindle_speed = 3000.0
        self.spindle_enabled = 1
        self.spindlerate = 1.1
        self.optional_stop = True
        self.linear_units = 0.03937007874
        self.axis_mask = 0x0F
        self.ini_filename = "/home/operator/tmc/configs/tormach_mill/tormach_mill_base.ini"

    def poll(self):
        if not self.running:
            raise Exception("emcStatusBuffer invalid")


class FakeLinuxcnc(object):
    MODE_MANUAL, MODE_AUTO, MODE_MDI = 1, 2, 3
    STATE_ESTOP, STATE_ESTOP_RESET, STATE_OFF, STATE_ON = 1, 2, 3, 4
    INTERP_IDLE, INTERP_READING, INTERP_PAUSED, INTERP_WAITING = 1, 2, 3, 4
    ini = FakeIni

    def __init__(self):
        self.status = FakeStat()

    def stat(self):
        return self.status


class StatusReaderTests(unittest.TestCase):

    def test_reads_status_with_names(self):
        linuxcnc = FakeLinuxcnc()
        reader = server.StatusReader(linuxcnc)

        self.assertTrue(reader.poll())
        status = reader.read_status()

        self.assertEqual(status["task_mode"], "AUTO")
        self.assertEqual(status["task_state"], "ON")
        self.assertEqual(status["interp_state"], "READING")
        self.assertFalse(status["estop"])
        self.assertEqual(status["file"], "/home/operator/gcode/bracket.nc")
        self.assertEqual(status["line"], 120)
        self.assertEqual(status["tool"], 5)
        self.assertEqual(status["actual_position"][3], 90.5)
        self.assertEqual(status["spindle_speed"], 3000.0)
        self.assertTrue(status["spindle_enabled"])
        self.assertAlmostEqual(status["spindle_override"], 1.1)
        self.assertTrue(status["optional_stop"])
        json.dumps(status)  # serializable

    def test_reads_linuxcnc_28_spindle(self):
        linuxcnc = FakeLinuxcnc()
        linuxcnc.status.spindle = ({"speed": 1200.0, "enabled": 1, "override": 0.8},)
        reader = server.StatusReader(linuxcnc)
        reader.poll()

        status = reader.read_status()

        self.assertEqual(status["spindle_speed"], 1200.0)
        self.assertAlmostEqual(status["spindle_override"], 0.8)

    def test_reads_info_from_ini(self):
        reader = server.StatusReader(FakeLinuxcnc())
        reader.poll()

        info = reader.read_info()

        self.assertEqual(info["machine"], "tormach_mill_1100")
        self.assertEqual(info["coordinates"], "X Y Z A")
        self.assertAlmostEqual(info["linear_units"], 0.03937007874)

    def test_info_falls_back_to_axis_mask(self):
        linuxcnc = FakeLinuxcnc()
        linuxcnc.status.ini_filename = None
        reader = server.StatusReader(linuxcnc)
        reader.poll()

        self.assertEqual(reader.read_info()["coordinates"], "X Y Z A")

    def test_poll_fails_when_linuxcnc_not_running(self):
        linuxcnc = FakeLinuxcnc()
        linuxcnc.status.running = False

        self.assertFalse(server.StatusReader(linuxcnc).poll())


class StatusServerTests(unittest.TestCase):

    def setUp(self):
        self.linuxcnc = FakeLinuxcnc()
        self.server = server.StatusServer(server.StatusReader(self.linuxcnc), "127.0.0.1", 0, 0.05)
        self.server.start()
        self.clients = []

    def tearDown(self):
        for client, reader in self.clients:
            reader.close()
            client.close()
        self.server.stop()

    def connect(self):
        client = socket.create_connection(("127.0.0.1", self.server.port), timeout=2)
        time.sleep(0.1)  # let the server accept the client
        reader = client.makefile("r")
        self.clients.append((client, reader))
        return reader

    def test_sends_info_then_status(self):
        reader = self.connect()
        self.server.update()

        info = json.loads(reader.readline())
        status = json.loads(reader.readline())

        self.assertEqual(info["type"], "info")
        self.assertEqual(status["type"], "status")
        self.assertTrue(status["available"])

    def test_sends_info_to_clients_connecting_later(self):
        self.server.update()
        reader = self.connect()
        self.server.update()

        self.assertEqual(json.loads(reader.readline())["type"], "info")
        self.assertEqual(json.loads(reader.readline())["type"], "status")

    def test_reports_unavailable_when_linuxcnc_stops(self):
        reader = self.connect()
        self.linuxcnc.status.running = False
        self.server.update()

        status = json.loads(reader.readline())

        self.assertEqual(status["type"], "status")
        self.assertFalse(status["available"])


if __name__ == "__main__":
    unittest.main()
