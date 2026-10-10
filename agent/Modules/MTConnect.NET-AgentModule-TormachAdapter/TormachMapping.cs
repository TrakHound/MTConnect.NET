// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events;
using System;

namespace MTConnect.Tormach
{
    /// <summary>
    /// Converts PathPilot (LinuxCNC) values to MTConnect values.
    /// </summary>
    public static class TormachMapping
    {
        private static readonly string[] WorkOffsets = { "54", "55", "56", "57", "58", "59", "59.1", "59.2", "59.3" };


        /// <summary>
        /// Maps the LinuxCNC task mode.
        /// </summary>
        /// <param name="taskMode"><c>MANUAL</c>, <c>AUTO</c> or <c>MDI</c>.</param>
        /// <returns>The ControllerMode, or <c>null</c> if unknown.</returns>
        public static ControllerMode? MapControllerMode(string taskMode)
        {
            switch (taskMode)
            {
                case "AUTO": return ControllerMode.AUTOMATIC;
                case "MDI": return ControllerMode.MANUAL_DATA_INPUT;
                case "MANUAL": return ControllerMode.MANUAL;
                default: return null;
            }
        }

        /// <summary>
        /// Maps the LinuxCNC task and interpreter state.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>The Execution, or <c>null</c> if unknown.</returns>
        public static Execution? MapExecution(TormachStatus status)
        {
            // Not ready while in emergency stop or with the machine off
            if (status.Estop || (status.TaskState != null && status.TaskState != "ON")) return Execution.STOPPED;

            switch (status.InterpState)
            {
                case "IDLE": return Execution.READY;
                case "PAUSED": return Execution.FEED_HOLD;
                case "READING":
                case "WAITING": return status.Paused ? Execution.FEED_HOLD : Execution.ACTIVE;
                default: return null;
            }
        }

        /// <summary>
        /// Returns the program name (file name without the directory).
        /// </summary>
        /// <param name="file">The program file path.</param>
        /// <returns>The file name, or an empty string if no program is loaded.</returns>
        public static string GetProgramName(string file)
        {
            if (string.IsNullOrEmpty(file)) return "";

            var index = Math.Max(file.LastIndexOf('/'), file.LastIndexOf('\\'));
            return index >= 0 ? file.Substring(index + 1) : file;
        }

        /// <summary>
        /// Returns the work offset number (ex. <c>54</c>, <c>59.1</c>) for a LinuxCNC g5x index.
        /// </summary>
        /// <param name="index">1 = G54 ... 9 = G59.3.</param>
        /// <returns>The work offset, or <c>null</c> if the index is out of range.</returns>
        public static string GetWorkOffset(int index) => index >= 1 && index <= WorkOffsets.Length ? WorkOffsets[index - 1] : null;

        /// <summary>
        /// Converts a value in machine linear units to millimeters.
        /// </summary>
        /// <param name="value">The value in machine units.</param>
        /// <param name="linearUnits">Machine linear units per millimeter (1 = mm, 0.03937 = inch).</param>
        /// <returns>The value in millimeters.</returns>
        public static double ToMillimeters(double value, double? linearUnits)
        {
            return linearUnits.HasValue && linearUnits.Value > 0 ? value / linearUnits.Value : value;
        }

        /// <summary>
        /// Returns the work coordinate position of an axis (machine position minus the work, G92 and tool offsets).
        /// Rotation of the work coordinate system (G10 L2 R) is not applied.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="index">The axis index (0 = X ... 8 = W).</param>
        /// <returns>The work position in machine units, or <c>null</c> if the position is not available.</returns>
        public static double? GetWorkPosition(TormachStatus status, int index)
        {
            var position = Get(status.ActualPosition, index);
            if (!position.HasValue) return null;

            return position.Value - (Get(status.G5xOffset, index) ?? 0) - (Get(status.G92Offset, index) ?? 0) - (Get(status.ToolOffset, index) ?? 0);
        }

        /// <summary>
        /// Converts a LinuxCNC override ratio to percent.
        /// </summary>
        /// <param name="ratio">The override (1 = 100%).</param>
        /// <returns>The override in percent.</returns>
        public static double ToPercent(double ratio) => Math.Round(ratio * 100, 1);


        internal static double? Get(double[] values, int index) => values != null && index >= 0 && index < values.Length ? values[index] : (double?)null;
    }
}
