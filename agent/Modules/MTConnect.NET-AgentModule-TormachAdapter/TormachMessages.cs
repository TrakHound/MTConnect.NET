// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MTConnect.Tormach
{
    /// <summary>
    /// The machine configuration sent by the PathPilot status server (<c>"type": "info"</c>).
    /// </summary>
    public class TormachInfo
    {
        /// <summary>The axis letters, in the order of the LinuxCNC position arrays.</summary>
        public const string AxisLetters = "XYZABCUVW";


        /// <summary>The status server version.</summary>
        public string Version { get; set; }

        /// <summary>The machine name ([EMC] MACHINE in the PathPilot configuration).</summary>
        public string Machine { get; set; }

        /// <summary>The configured axis letters ([TRAJ] COORDINATES, ex. <c>X Y Z A</c>).</summary>
        public string Coordinates { get; set; }

        /// <summary>The machine linear units per millimeter (1 = mm, 0.03937 = inch).</summary>
        public double? LinearUnits { get; set; }


        /// <summary>
        /// Returns the configured axes, without duplicates (ex. gantry axes), in configuration order.
        /// </summary>
        /// <returns>The axis letters.</returns>
        public IEnumerable<string> GetAxes()
        {
            return (Coordinates ?? "")
                .ToUpperInvariant()
                .Where(c => AxisLetters.IndexOf(c) >= 0)
                .Select(c => c.ToString())
                .Distinct();
        }

        /// <summary>
        /// Returns whether an axis is rotary (<c>A</c>, <c>B</c> or <c>C</c>).
        /// </summary>
        /// <param name="axis">The axis letter.</param>
        /// <returns><c>true</c> for a rotary axis.</returns>
        public static bool IsRotary(string axis) => axis == "A" || axis == "B" || axis == "C";

        /// <summary>
        /// Returns a string that changes when the machine configuration changes.
        /// </summary>
        /// <returns>The signature.</returns>
        public string GetSignature() => $"{Machine}|{string.Join(",", GetAxes())}";
    }

    /// <summary>
    /// The machine status sent by the PathPilot status server (<c>"type": "status"</c>).
    /// Positions and velocities are in machine units.
    /// </summary>
    public class TormachStatus
    {
        /// <summary>Whether LinuxCNC (PathPilot) is running.</summary>
        public bool Available { get; set; }
        /// <summary>The task mode: <c>MANUAL</c>, <c>AUTO</c> or <c>MDI</c>.</summary>
        public string TaskMode { get; set; }
        /// <summary>The task state: <c>ESTOP</c>, <c>ESTOP_RESET</c>, <c>OFF</c> or <c>ON</c>.</summary>
        public string TaskState { get; set; }
        /// <summary>The interpreter state: <c>IDLE</c>, <c>READING</c>, <c>PAUSED</c> or <c>WAITING</c>.</summary>
        public string InterpState { get; set; }
        /// <summary>Whether the machine is in emergency stop.</summary>
        public bool Estop { get; set; }
        /// <summary>Whether motion is paused (feed hold).</summary>
        public bool Paused { get; set; }
        /// <summary>The loaded program file.</summary>
        public string File { get; set; }
        /// <summary>The program line being executed.</summary>
        public int Line { get; set; }
        /// <summary>The tool in the spindle.</summary>
        public int Tool { get; set; }
        /// <summary>The active work offset (1 = G54 ... 9 = G59.3).</summary>
        public int G5xIndex { get; set; }
        /// <summary>The machine position (X Y Z A B C U V W).</summary>
        public double[] ActualPosition { get; set; }
        /// <summary>The active work offset (X Y Z A B C U V W).</summary>
        public double[] G5xOffset { get; set; }
        /// <summary>The G92 offset (X Y Z A B C U V W).</summary>
        public double[] G92Offset { get; set; }
        /// <summary>The tool offset (X Y Z A B C U V W).</summary>
        public double[] ToolOffset { get; set; }
        /// <summary>The current velocity, in machine units per second.</summary>
        public double? CurrentVelocity { get; set; }
        /// <summary>The feedrate override (1 = 100%).</summary>
        public double? Feedrate { get; set; }
        /// <summary>The rapid override (1 = 100%).</summary>
        public double? Rapidrate { get; set; }
        /// <summary>The commanded spindle speed in RPM (negative = counterclockwise).</summary>
        public double? SpindleSpeed { get; set; }
        /// <summary>Whether the spindle is on.</summary>
        public bool SpindleEnabled { get; set; }
        /// <summary>The spindle override (1 = 100%).</summary>
        public double? SpindleOverride { get; set; }
        /// <summary>Whether optional stop (M1) is on.</summary>
        public bool OptionalStop { get; set; }
        /// <summary>The machine linear units per millimeter.</summary>
        public double? LinearUnits { get; set; }
    }

    /// <summary>
    /// Parses messages sent by the PathPilot status server.
    /// </summary>
    public static class TormachMessageParser
    {
        /// <summary>
        /// Parses one line sent by the status server.
        /// </summary>
        /// <param name="line">The JSON line.</param>
        /// <returns>A <see cref="TormachInfo"/>, a <see cref="TormachStatus"/>, or <c>null</c> if the line is not a known message.</returns>
        public static object Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;

            try
            {
                using (var document = JsonDocument.Parse(line))
                {
                    var root = document.RootElement;
                    if (root.ValueKind != JsonValueKind.Object) return null;

                    switch (GetString(root, "type"))
                    {
                        case "info": return ParseInfo(root);
                        case "status": return ParseStatus(root);
                        default: return null;
                    }
                }
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static TormachInfo ParseInfo(JsonElement root)
        {
            return new TormachInfo
            {
                Version = GetString(root, "version"),
                Machine = GetString(root, "machine"),
                Coordinates = GetString(root, "coordinates"),
                LinearUnits = GetDouble(root, "linear_units")
            };
        }

        private static TormachStatus ParseStatus(JsonElement root)
        {
            return new TormachStatus
            {
                Available = GetBool(root, "available"),
                TaskMode = GetString(root, "task_mode"),
                TaskState = GetString(root, "task_state"),
                InterpState = GetString(root, "interp_state"),
                Estop = GetBool(root, "estop"),
                Paused = GetBool(root, "paused"),
                File = GetString(root, "file"),
                Line = (int)(GetDouble(root, "line") ?? 0),
                Tool = (int)(GetDouble(root, "tool") ?? 0),
                G5xIndex = (int)(GetDouble(root, "g5x_index") ?? 0),
                ActualPosition = GetArray(root, "actual_position"),
                G5xOffset = GetArray(root, "g5x_offset"),
                G92Offset = GetArray(root, "g92_offset"),
                ToolOffset = GetArray(root, "tool_offset"),
                CurrentVelocity = GetDouble(root, "current_vel"),
                Feedrate = GetDouble(root, "feedrate"),
                Rapidrate = GetDouble(root, "rapidrate"),
                SpindleSpeed = GetDouble(root, "spindle_speed"),
                SpindleEnabled = GetBool(root, "spindle_enabled"),
                SpindleOverride = GetDouble(root, "spindle_override"),
                OptionalStop = GetBool(root, "optional_stop"),
                LinearUnits = GetDouble(root, "linear_units")
            };
        }

        private static string GetString(JsonElement root, string name)
        {
            JsonElement value;
            return root.TryGetProperty(name, out value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
        }

        private static double? GetDouble(JsonElement root, string name)
        {
            JsonElement value;
            return root.TryGetProperty(name, out value) && value.ValueKind == JsonValueKind.Number ? value.GetDouble() : (double?)null;
        }

        private static bool GetBool(JsonElement root, string name)
        {
            JsonElement value;
            return root.TryGetProperty(name, out value) && value.ValueKind == JsonValueKind.True;
        }

        private static double[] GetArray(JsonElement root, string name)
        {
            JsonElement value;
            if (!root.TryGetProperty(name, out value) || value.ValueKind != JsonValueKind.Array) return null;

            var values = new List<double>();
            foreach (var item in value.EnumerateArray())
            {
                values.Add(item.ValueKind == JsonValueKind.Number ? item.GetDouble() : 0);
            }
            return values.ToArray();
        }
    }
}
