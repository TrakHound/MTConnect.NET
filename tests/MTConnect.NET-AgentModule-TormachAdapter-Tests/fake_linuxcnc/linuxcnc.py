# Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
# TrakHound Inc. licenses this file to you under the MIT license.

"""
A fake linuxcnc module used to run pathpilot_status_server.py in tests.
Simulates an inch 4-axis mill (X Y Z A) running a program.
"""

MODE_MANUAL, MODE_AUTO, MODE_MDI = 1, 2, 3
STATE_ESTOP, STATE_ESTOP_RESET, STATE_OFF, STATE_ON = 1, 2, 3, 4
INTERP_IDLE, INTERP_READING, INTERP_PAUSED, INTERP_WAITING = 1, 2, 3, 4


class ini(object):
    def __init__(self, path):
        pass

    def find(self, section, key):
        return {("EMC", "MACHINE"): "tormach_mill_1100", ("TRAJ", "COORDINATES"): "X Y Z A"}.get((section, key))


class stat(object):
    def __init__(self):
        self.task_mode = MODE_AUTO
        self.task_state = STATE_ON
        self.interp_state = INTERP_READING
        self.estop = 0
        self.paused = False
        self.file = "/home/operator/gcode/bracket.nc"
        self.current_line = 120
        self.tool_in_spindle = 5
        self.g5x_index = 2
        self.actual_position = (1.5, -2.25, 0.1, 90.5, 0.0, 0.0, 0.0, 0.0, 0.0)
        self.g5x_offset = (1.0, -2.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
        self.g92_offset = (0.0,) * 9
        self.tool_offset = (0.0, 0.0, 0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
        self.current_vel = 0.25
        self.feedrate = 1.0
        self.rapidrate = 0.5
        self.spindle_speed = 3000.0
        self.spindle_enabled = 1
        self.spindlerate = 1.1
        self.optional_stop = True
        self.linear_units = 1 / 25.4
        self.axis_mask = 0x0F
        self.ini_filename = "/home/operator/tmc/configs/tormach_mill/tormach_mill_base.ini"

    def poll(self):
        pass
