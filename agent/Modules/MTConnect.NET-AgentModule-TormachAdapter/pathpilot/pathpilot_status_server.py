#!/usr/bin/env python
# Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
# TrakHound Inc. licenses this file to you under the MIT license.

"""
PathPilot status server for the MTConnect.NET Tormach Adapter agent module.

Runs on a Tormach PathPilot controller, reads the machine status using the
LinuxCNC Python API (linuxcnc.stat) and sends it to every connected client
as one JSON object per line:

    {"type": "info", ...}     sent when a client connects and when the configuration changes
    {"type": "status", ...}   sent every interval

The server only reads status; it never sends commands to the machine and does
not read the LinuxCNC error channel (which would take messages away from
PathPilot).

Usage:
    python pathpilot_status_server.py [--port 7890] [--bind 0.0.0.0] [--interval 500]

Compatible with Python 2.7 and Python 3. Uses only the standard library and
the linuxcnc module included with PathPilot.
"""

import argparse
import json
import os
import socket
import sys
import threading
import time

VERSION = "1.0"
DEFAULT_PORT = 7890
AXIS_LETTERS = "XYZABCUVW"


def _names(module, pairs):
    """Map linuxcnc constants to names, ignoring constants a version does not define."""
    names = {}
    for attribute, name in pairs:
        if hasattr(module, attribute):
            names[getattr(module, attribute)] = name
    return names


class StatusReader(object):
    """Reads linuxcnc.stat() and builds status / info messages."""

    def __init__(self, linuxcnc_module):
        self.linuxcnc = linuxcnc_module
        self.stat = None
        self.modes = _names(linuxcnc_module, [("MODE_MANUAL", "MANUAL"), ("MODE_AUTO", "AUTO"), ("MODE_MDI", "MDI")])
        self.states = _names(linuxcnc_module, [("STATE_ESTOP", "ESTOP"), ("STATE_ESTOP_RESET", "ESTOP_RESET"),
                                               ("STATE_OFF", "OFF"), ("STATE_ON", "ON")])
        self.interp_states = _names(linuxcnc_module, [("INTERP_IDLE", "IDLE"), ("INTERP_READING", "READING"),
                                                      ("INTERP_PAUSED", "PAUSED"), ("INTERP_WAITING", "WAITING")])

    def poll(self):
        """Polls LinuxCNC. Returns False if LinuxCNC (PathPilot) is not running."""
        try:
            if self.stat is None:
                self.stat = self.linuxcnc.stat()
            self.stat.poll()
            return True
        except Exception:
            self.stat = None
            return False

    def read_info(self):
        s = self.stat
        info = {"type": "info", "version": VERSION, "machine": None, "coordinates": None,
                "linear_units": _number(getattr(s, "linear_units", None)),
                "axis_mask": int(getattr(s, "axis_mask", 0) or 0)}

        ini_path = getattr(s, "ini_filename", None)
        if ini_path:
            try:
                ini = self.linuxcnc.ini(ini_path)
                info["machine"] = ini.find("EMC", "MACHINE")
                info["coordinates"] = ini.find("TRAJ", "COORDINATES")
            except Exception:
                pass

        if not info["coordinates"]:
            info["coordinates"] = " ".join(letter for i, letter in enumerate(AXIS_LETTERS) if info["axis_mask"] & (1 << i))

        return info

    def read_status(self):
        s = self.stat
        speed, enabled, override = self._spindle()

        return {
            "type": "status",
            "available": True,
            "timestamp": time.time(),
            "task_mode": self.modes.get(getattr(s, "task_mode", None)),
            "task_state": self.states.get(getattr(s, "task_state", None)),
            "interp_state": self.interp_states.get(getattr(s, "interp_state", None)),
            "estop": bool(getattr(s, "estop", False)),
            "paused": bool(getattr(s, "paused", False)),
            "file": getattr(s, "file", "") or "",
            "line": int(getattr(s, "current_line", 0) or 0),
            "tool": int(getattr(s, "tool_in_spindle", 0) or 0),
            "g5x_index": int(getattr(s, "g5x_index", 0) or 0),
            "actual_position": _numbers(getattr(s, "actual_position", None)),
            "g5x_offset": _numbers(getattr(s, "g5x_offset", None)),
            "g92_offset": _numbers(getattr(s, "g92_offset", None)),
            "tool_offset": _numbers(getattr(s, "tool_offset", None)),
            "current_vel": _number(getattr(s, "current_vel", None)),
            "feedrate": _number(getattr(s, "feedrate", None)),
            "rapidrate": _number(getattr(s, "rapidrate", None)),
            "spindle_speed": speed,
            "spindle_enabled": enabled,
            "spindle_override": override,
            "optional_stop": bool(getattr(s, "optional_stop", False)),
            "linear_units": _number(getattr(s, "linear_units", None)),
        }

    def _spindle(self):
        s = self.stat
        spindles = getattr(s, "spindle", None)
        if isinstance(spindles, (list, tuple)) and spindles and isinstance(spindles[0], dict):
            # LinuxCNC 2.8+: list of spindle dictionaries
            first = spindles[0]
            return _number(first.get("speed")), bool(first.get("enabled")), _number(first.get("override"))

        # LinuxCNC 2.7
        return (_number(getattr(s, "spindle_speed", None)), bool(getattr(s, "spindle_enabled", False)),
                _number(getattr(s, "spindlerate", None)))


def _number(value):
    try:
        return float(value) if value is not None else None
    except (TypeError, ValueError):
        return None


def _numbers(values):
    try:
        return [float(v) for v in values] if values is not None else None
    except (TypeError, ValueError):
        return None


class StatusServer(object):
    """Accepts clients and sends them the latest messages."""

    def __init__(self, reader, bind="0.0.0.0", port=DEFAULT_PORT, interval=0.5):
        self.reader = reader
        self.bind = bind
        self.port = port
        self.interval = interval
        self.clients = []
        self.lock = threading.Lock()
        self.info = None
        self.running = False
        self.listener = None

    def start(self):
        self.listener = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.listener.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.listener.bind((self.bind, self.port))
        self.listener.listen(5)
        self.port = self.listener.getsockname()[1]
        self.running = True

        thread = threading.Thread(target=self._accept)
        thread.daemon = True
        thread.start()

    def stop(self):
        self.running = False
        try:
            self.listener.close()
        except Exception:
            pass
        with self.lock:
            for client in self.clients:
                _close(client)
            self.clients = []

    def run(self):
        while self.running:
            self.update()
            time.sleep(self.interval)

    def update(self):
        """Polls LinuxCNC once and sends the result to every client."""
        if self.reader.poll():
            info = self.reader.read_info()
            if info != self.info:
                self.info = info
                self._broadcast(info)
            self._broadcast(self.reader.read_status())
        else:
            self.info = None
            self._broadcast({"type": "status", "available": False, "timestamp": time.time()})

    def _accept(self):
        while self.running:
            try:
                client, _ = self.listener.accept()
            except Exception:
                if self.running:
                    time.sleep(0.5)
                continue

            client.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
            if self.info is not None and not _send(client, self.info):
                continue
            with self.lock:
                self.clients.append(client)

    def _broadcast(self, message):
        with self.lock:
            connected = []
            for client in self.clients:
                if _send(client, message):
                    connected.append(client)
                else:
                    _close(client)
            self.clients = connected


def _send(client, message):
    try:
        client.sendall((json.dumps(message, separators=(",", ":")) + "\n").encode("utf-8"))
        return True
    except Exception:
        return False


def _close(client):
    try:
        client.close()
    except Exception:
        pass


def main(argv=None):
    parser = argparse.ArgumentParser(description="PathPilot status server for MTConnect.NET")
    parser.add_argument("--port", type=int, default=DEFAULT_PORT, help="TCP port (default %d)" % DEFAULT_PORT)
    parser.add_argument("--bind", default="0.0.0.0", help="address to listen on (default all)")
    parser.add_argument("--interval", type=int, default=500, help="status interval in milliseconds (default 500)")
    args = parser.parse_args(argv)

    try:
        import linuxcnc
    except ImportError:
        sys.stderr.write("The linuxcnc Python module was not found. Run this script on the PathPilot controller.\n")
        return 1

    server = StatusServer(StatusReader(linuxcnc), args.bind, args.port, max(args.interval, 50) / 1000.0)
    server.start()
    sys.stdout.write("PathPilot status server %s listening on %s:%d (pid %d)\n" % (VERSION, args.bind, server.port, os.getpid()))
    sys.stdout.flush()

    try:
        server.run()
    except KeyboardInterrupt:
        pass
    finally:
        server.stop()
    return 0


if __name__ == "__main__":
    sys.exit(main())
