// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Fanuc
{
    /// <summary>
    /// The DataItem keys (ID, Name, or Source) that <see cref="FanucDataSource"/> reports observations for.
    /// Path keys are used as-is for path 1 and prefixed with <c>p{n}_</c> for other paths (ex. <c>p2_execution</c>).
    /// </summary>
    public static class FanucDataItemKeys
    {
        /// <summary>AVAILABILITY event.</summary>
        public const string Availability = "avail";

        /// <summary>EMERGENCY_STOP event.</summary>
        public const string EmergencyStop = "estop";

        /// <summary>FIRMWARE (VERSION) event: the CNC software series and version.</summary>
        public const string Firmware = "firmware";

        /// <summary>EQUIPMENT_TIMER (POWERED) sample, in seconds.</summary>
        public const string PowerOnTime = "powerOnTime";

        /// <summary>EQUIPMENT_TIMER (OPERATING) sample: automatic operation time, in seconds.</summary>
        public const string OperatingTime = "operatingTime";

        /// <summary>EQUIPMENT_TIMER (WORKING) sample: cutting time, in seconds.</summary>
        public const string CuttingTime = "cuttingTime";

        /// <summary>PROCESS_TIMER (PROCESS) sample: the cycle time, in seconds.</summary>
        public const string CycleTime = "cycleTime";

        /// <summary>CONTROLLER_MODE event (path).</summary>
        public const string ControllerMode = "mode";

        /// <summary>EXECUTION event (path).</summary>
        public const string Execution = "execution";

        /// <summary>PROGRAM event (path).</summary>
        public const string Program = "program";

        /// <summary>LINE_LABEL event (path): the sequence number (N) being executed.</summary>
        public const string Line = "line";

        /// <summary>PART_COUNT event (path).</summary>
        public const string PartCount = "partCount";

        /// <summary>TOOL_NUMBER event (path).</summary>
        public const string ToolNumber = "tool";

        /// <summary>WORK_OFFSET event (path): the active work offset (ex. 54).</summary>
        public const string WorkOffset = "workOffset";

        /// <summary>PATH_FEEDRATE (ACTUAL) sample (path), in millimeters per second.</summary>
        public const string Feedrate = "feedrate";

        /// <summary>PATH_FEEDRATE_OVERRIDE (PROGRAMMED) event (path), in percent.</summary>
        public const string FeedrateOverride = "feedOverride";

        /// <summary>PATH_FEEDRATE_OVERRIDE (RAPID) event (path), in percent.</summary>
        public const string RapidOverride = "rapidOverride";

        /// <summary>SYSTEM condition (path): CNC alarms and warnings.</summary>
        public const string System = "system";

        /// <summary>Spindle ROTARY_VELOCITY (ACTUAL) sample (path), in revolutions per minute.</summary>
        public const string SpindleSpeed = "speed";

        /// <summary>Spindle LOAD sample (path), in percent.</summary>
        public const string SpindleLoad = "spindleLoad";

        /// <summary>Spindle ROTARY_VELOCITY_OVERRIDE event (path), in percent.</summary>
        public const string SpindleOverride = "spindleOverride";


        /// <summary>
        /// Returns the key of a path DataItem (ex. <c>execution</c> for path 1, <c>p2_execution</c> for path 2).
        /// </summary>
        /// <param name="key">The path 1 key.</param>
        /// <param name="path">The path number.</param>
        /// <returns>The DataItem key.</returns>
        public static string ForPath(string key, int path) => path <= 1 ? key : $"p{path}_{key}";

        /// <summary>
        /// Returns the key of the machine position of an axis (ex. <c>x_act</c>).
        /// </summary>
        /// <param name="axis">The axis name.</param>
        /// <returns>The DataItem key.</returns>
        public static string GetPositionKey(string axis) => $"{axis.ToLowerInvariant()}_act";

        /// <summary>
        /// Returns the key of the absolute (work coordinate) position of an axis (ex. <c>x_work</c>).
        /// </summary>
        /// <param name="axis">The axis name.</param>
        /// <returns>The DataItem key.</returns>
        public static string GetWorkPositionKey(string axis) => $"{axis.ToLowerInvariant()}_work";

        /// <summary>
        /// Returns the key of the servo load of an axis (ex. <c>x_load</c>).
        /// </summary>
        /// <param name="axis">The axis name.</param>
        /// <returns>The DataItem key.</returns>
        public static string GetLoadKey(string axis) => $"{axis.ToLowerInvariant()}_load";
    }

    /// <summary>
    /// The structure of a Fanuc CNC read when connecting: model, paths, axes and spindles.
    /// </summary>
    public class FanucMachineModel
    {
        /// <summary>The CNC model (ex. <c>30i-M</c>).</summary>
        public string Model { get; set; }

        /// <summary>The CNC software series and version.</summary>
        public string Firmware { get; set; }

        /// <summary>The paths of the CNC.</summary>
        public List<FanucPathModel> Paths { get; set; } = new List<FanucPathModel>();


        /// <summary>
        /// Returns a model with a single path and no axes, used before the CNC has been read.
        /// </summary>
        /// <returns>A new model.</returns>
        public static FanucMachineModel CreateDefault()
        {
            var model = new FanucMachineModel();
            model.Paths.Add(new FanucPathModel { Number = 1, Spindle = "S" });
            return model;
        }

        /// <summary>
        /// Returns a string that changes when any part of the model changes.
        /// </summary>
        /// <returns>The signature.</returns>
        public string GetSignature()
        {
            var paths = (Paths ?? new List<FanucPathModel>()).Select(p =>
                $"{p.Number}:{p.Spindle}:{string.Join(",", (p.Axes ?? new List<FanucAxisModel>()).Select(a => $"{a.Name}{(a.IsRotary ? "R" : "L")}"))}");
            return $"{Model}|{Firmware}|{string.Join("|", paths)}";
        }
    }

    /// <summary>A path of a Fanuc CNC.</summary>
    public class FanucPathModel
    {
        /// <summary>The path number (1 based).</summary>
        public int Number { get; set; }

        /// <summary>The axes of the path.</summary>
        public List<FanucAxisModel> Axes { get; set; } = new List<FanucAxisModel>();

        /// <summary>The name of the path's main spindle (ex. <c>S1</c>).</summary>
        public string Spindle { get; set; }
    }

    /// <summary>An axis of a Fanuc CNC path.</summary>
    public class FanucAxisModel
    {
        /// <summary>The axis name including suffix (ex. <c>X</c>, <c>X1</c>).</summary>
        public string Name { get; set; }

        /// <summary>Whether the axis is rotary (positions in degrees).</summary>
        public bool IsRotary { get; set; }
    }
}
