// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Json.CuttingTools.Measurements;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a <c>CuttingToolLifeCycle</c>, the
    /// in-service state of a cutting tool. Mirrors the on-the-wire shape so the
    /// JSON serializer can read and write it, then converts to and from the
    /// strongly-typed <see cref="CuttingToolLifeCycle"/> model.
    /// </summary>
    public class JsonCuttingToolLifeCycle
    {
        /// <summary>
        /// The status values of the cutting tool (for example NEW, USED, or
        /// EXPIRED), serialized as enumeration names.
        /// </summary>
        [JsonPropertyName("cutterStatus")]
        public IEnumerable<string> CutterStatus { get; set; }

        /// <summary>
        /// The number of times the tool has been reconditioned.
        /// </summary>
        [JsonPropertyName("reconditionCount")]
        public JsonReconditionCount ReconditionCount { get; set; }

        /// <summary>
        /// The measured and remaining tool life values.
        /// </summary>
        [JsonPropertyName("toolLife")]
        public IEnumerable<JsonToolLife> ToolLife { get; set; }

        /// <summary>
        /// The location of the tool in the machine, such as a pot or spindle.
        /// </summary>
        [JsonPropertyName("location")]
        public JsonLocation Location { get; set; }

        /// <summary>
        /// The program tool group the tool is assigned to.
        /// </summary>
        [JsonPropertyName("programToolGroup")]
        public string ProgramToolGroup { get; set; }

        /// <summary>
        /// The program tool number the tool is referenced by.
        /// </summary>
        [JsonPropertyName("programToolNumber")]
        public string ProgramToolNumber { get; set; }

        /// <summary>
        /// The spindle speed limits the tool is intended to operate within.
        /// </summary>
        [JsonPropertyName("processSpindleSpeed")]
        public JsonProcessSpindleSpeed ProcessSpindleSpeed { get; set; }

        /// <summary>
        /// The feed rate limits the tool is intended to operate within.
        /// </summary>
        [JsonPropertyName("processFeedRate")]
        public JsonProcessFeedrate ProcessFeedRate { get; set; }

        /// <summary>
        /// The connection code identifying the machine-side interface of the
        /// tool.
        /// </summary>
        [JsonPropertyName("connectionCodeMachineSide")]
        public string ConnectionCodeMachineSide { get; set; }

        /// <summary>
        /// The dimensional measurements of the tool assembly.
        /// </summary>
        [JsonPropertyName("measurements")]
        public IEnumerable<JsonMeasurement> Measurements { get; set; }

        /// <summary>
        /// The cutting items (inserts or edges) making up the tool.
        /// </summary>
        [JsonPropertyName("cuttingItems")]
        public JsonCuttingItemCollection CuttingItems { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingToolLifeCycle() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICuttingToolLifeCycle"/>, converting cutter statuses to
        /// strings and each nested measurement, tool life, and cutting item.
        /// </summary>
        public JsonCuttingToolLifeCycle(ICuttingToolLifeCycle cuttingToolLifeCycle)
        {
            if (cuttingToolLifeCycle != null)
            {
                // CutterStatus
                if (!cuttingToolLifeCycle.CutterStatus.IsNullOrEmpty())
                {
                    var statuses = new List<string>();
                    foreach (var cutterStatus in cuttingToolLifeCycle.CutterStatus)
                    {
                        statuses.Add(cutterStatus.ToString());
                    }
                    CutterStatus = statuses;
                }

                if (cuttingToolLifeCycle.ReconditionCount != null) ReconditionCount = new JsonReconditionCount(cuttingToolLifeCycle.ReconditionCount);
                if (cuttingToolLifeCycle.Location != null) Location = new JsonLocation(cuttingToolLifeCycle.Location);
                ProgramToolGroup = cuttingToolLifeCycle.ProgramToolGroup;
                ProgramToolNumber = cuttingToolLifeCycle.ProgramToolNumber;
                if (cuttingToolLifeCycle.ProcessSpindleSpeed != null) ProcessSpindleSpeed = new JsonProcessSpindleSpeed(cuttingToolLifeCycle.ProcessSpindleSpeed);
                if (cuttingToolLifeCycle.ProcessFeedRate != null) ProcessFeedRate = new JsonProcessFeedrate(cuttingToolLifeCycle.ProcessFeedRate);
                ConnectionCodeMachineSide = cuttingToolLifeCycle.ConnectionCodeMachineSide;

                // ToolLife
                if (!cuttingToolLifeCycle.ToolLife.IsNullOrEmpty())
                {
                    var jsonToolLife = new List<JsonToolLife>();
                    foreach (var toolLife in cuttingToolLifeCycle.ToolLife)
                    {
                        jsonToolLife.Add(new JsonToolLife(toolLife));
                    }
                    ToolLife = jsonToolLife;
                }

                // Measurements
                if (!cuttingToolLifeCycle.Measurements.IsNullOrEmpty())
                {
                    var measurements = new List<JsonMeasurement>();
                    foreach (var measurement in cuttingToolLifeCycle.Measurements)
                    {
                        measurements.Add(new JsonMeasurement(measurement));
                    }
                    Measurements = measurements;
                }

                // CuttingItems
                if (cuttingToolLifeCycle.CuttingItems != null)
                {
                    CuttingItems = new JsonCuttingItemCollection(cuttingToolLifeCycle.CuttingItems);
                }
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ICuttingToolLifeCycle"/>, parsing cutter statuses and
        /// converting each nested measurement, tool life, and cutting item.
        /// </summary>
        public ICuttingToolLifeCycle ToCuttingToolLifeCycle()
        {
            var cuttingToolLifeCycle = new CuttingToolLifeCycle();

            // CutterStatus
            if (!CutterStatus.IsNullOrEmpty())
            {
                var statuses = new List<CutterStatusType>();
                foreach (var cutterStatus in CutterStatus)
                {
                    statuses.Add(cutterStatus.ConvertEnum<CutterStatusType>());
                }
                cuttingToolLifeCycle.CutterStatus = statuses;
            }

            if (ReconditionCount != null) cuttingToolLifeCycle.ReconditionCount = ReconditionCount.ToReconditionCount();
            if (Location != null) cuttingToolLifeCycle.Location = Location.ToLocation();
            cuttingToolLifeCycle.ProgramToolGroup = ProgramToolGroup;
            cuttingToolLifeCycle.ProgramToolNumber = ProgramToolNumber;
            if (ProcessSpindleSpeed != null) cuttingToolLifeCycle.ProcessSpindleSpeed = ProcessSpindleSpeed.ToProcessSpindleSpeed();
            if (ProcessFeedRate != null) cuttingToolLifeCycle.ProcessFeedRate = ProcessFeedRate.ToProcessFeedrate();
            cuttingToolLifeCycle.ConnectionCodeMachineSide = ConnectionCodeMachineSide;

            // ToolLife
            if (!ToolLife.IsNullOrEmpty())
            {
                var toolifes = new List<IToolLife>();
                foreach (var toolife in ToolLife)
                {
                    toolifes.Add(toolife.ToToolLife());
                }
                cuttingToolLifeCycle.ToolLife = toolifes;
            }

            // Measurements
            if (!Measurements.IsNullOrEmpty())
            {
                var measurements = new List<IToolingMeasurement>();
                foreach (var measurement in Measurements)
                {
                    measurements.Add(measurement.ToMeasurement());
                }
                cuttingToolLifeCycle.Measurements = measurements;
            }

            if (CuttingItems != null)
            {
                cuttingToolLifeCycle.CuttingItems = CuttingItems.ToCuttingItems();
            }

            return cuttingToolLifeCycle;
        }
    }
}