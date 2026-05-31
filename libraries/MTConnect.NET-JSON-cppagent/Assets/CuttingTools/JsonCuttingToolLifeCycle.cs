// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a <c>CuttingToolLifeCycle</c> in the
    /// cppagent-compatible shape. Converts to and from the strongly-typed
    /// <see cref="CuttingToolLifeCycle"/> model.
    /// </summary>
    public class JsonCuttingToolLifeCycle
    {
        /// <summary>
        /// The program tool group the tool is assigned to.
        /// </summary>
        [JsonPropertyName("ProgramToolGroup")]
        public string ProgramToolGroup { get; set; }

        /// <summary>
        /// The program tool number the tool is referenced by.
        /// </summary>
        [JsonPropertyName("ProgramToolNumber")]
        public string ProgramToolNumber { get; set; }

        /// <summary>
        /// The connection code identifying the machine-side interface of the
        /// tool.
        /// </summary>
        [JsonPropertyName("ConnectionCodeMachineSide")]
        public string ConnectionCodeMachineSide { get; set; }

        /// <summary>
        /// The status values of the cutting tool.
        /// </summary>
        [JsonPropertyName("CutterStatus")]
        public JsonCutterStatusCollection CutterStatus { get; set; }

        /// <summary>
        /// The number of times the tool has been reconditioned.
        /// </summary>
        [JsonPropertyName("ReconditionCount")]
        public JsonReconditionCount ReconditionCount { get; set; }

        /// <summary>
        /// The measured and remaining tool life values.
        /// </summary>
        [JsonPropertyName("ToolLife")]
        public IEnumerable<JsonToolLife> ToolLife { get; set; }

        /// <summary>
        /// The location of the tool in the machine.
        /// </summary>
        [JsonPropertyName("Location")]
        public JsonLocation Location { get; set; }

        /// <summary>
        /// The spindle speed limits the tool is intended to operate within.
        /// </summary>
        [JsonPropertyName("ProcessSpindleSpeed")]
        public JsonProcessSpindleSpeed ProcessSpindleSpeed { get; set; }

        /// <summary>
        /// The feed rate limits the tool is intended to operate within.
        /// </summary>
        [JsonPropertyName("ProcessFeedRate")]
        public JsonProcessFeedRate ProcessFeedRate { get; set; }

        /// <summary>
        /// The dimensional measurements of the tool assembly.
        /// </summary>
        [JsonPropertyName("Measurements")]
        public JsonMeasurements Measurements { get; set; }

        /// <summary>
        /// The cutting items making up the tool.
        /// </summary>
        [JsonPropertyName("CuttingItems")]
        public JsonCuttingItemCollection CuttingItems { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingToolLifeCycle() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICuttingToolLifeCycle"/>.
        /// </summary>
        public JsonCuttingToolLifeCycle(ICuttingToolLifeCycle cuttingToolLifeCycle)
        {
            if (cuttingToolLifeCycle != null)
            {
                ProgramToolNumber = cuttingToolLifeCycle.ProgramToolNumber;
                ProgramToolGroup = cuttingToolLifeCycle.ProgramToolGroup;
                ConnectionCodeMachineSide = cuttingToolLifeCycle.ConnectionCodeMachineSide;
                if (cuttingToolLifeCycle.ReconditionCount != null) ReconditionCount = new JsonReconditionCount(cuttingToolLifeCycle.ReconditionCount);
                if (cuttingToolLifeCycle.Location != null) Location = new JsonLocation(cuttingToolLifeCycle.Location);
                if (cuttingToolLifeCycle.ProcessSpindleSpeed != null) ProcessSpindleSpeed = new JsonProcessSpindleSpeed(cuttingToolLifeCycle.ProcessSpindleSpeed);
                if (cuttingToolLifeCycle.ProcessFeedRate != null) ProcessFeedRate = new JsonProcessFeedRate(cuttingToolLifeCycle.ProcessFeedRate);

                // CutterStatus
                if (!cuttingToolLifeCycle.CutterStatus.IsNullOrEmpty())
                {
                    CutterStatus = new JsonCutterStatusCollection(cuttingToolLifeCycle.CutterStatus);
                }

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
                    Measurements = new JsonMeasurements(cuttingToolLifeCycle.Measurements);
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
        /// <see cref="ICuttingToolLifeCycle"/>.
        /// </summary>
        public ICuttingToolLifeCycle ToCuttingToolLifeCycle()
        {
            var cuttingToolLifeCycle = new CuttingToolLifeCycle();

            if (ReconditionCount != null) cuttingToolLifeCycle.ReconditionCount = ReconditionCount.ToReconditionCount();
            if (Location != null) cuttingToolLifeCycle.Location = Location.ToLocation();
            cuttingToolLifeCycle.ProgramToolGroup = ProgramToolGroup;
            cuttingToolLifeCycle.ProgramToolNumber = ProgramToolNumber;
            if (ProcessSpindleSpeed != null) cuttingToolLifeCycle.ProcessSpindleSpeed = ProcessSpindleSpeed.ToProcessSpindleSpeed();
            if (ProcessFeedRate != null) cuttingToolLifeCycle.ProcessFeedRate = ProcessFeedRate.ToProcessFeedrate();
            cuttingToolLifeCycle.ConnectionCodeMachineSide = ConnectionCodeMachineSide;

            // CutterStatus
            if (CutterStatus != null)
            {
                var statuses = new List<CutterStatusType>();
                foreach (var cutterStatus in CutterStatus.Status)
                {
                    statuses.Add(cutterStatus.ToCutterStatus());
                }
                cuttingToolLifeCycle.CutterStatus = statuses;
            }

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
            if (Measurements != null)
            {
                cuttingToolLifeCycle.Measurements = Measurements.ToMeasurements();
            }

            if (CuttingItems != null)
            {
                cuttingToolLifeCycle.CuttingItems = CuttingItems.ToCuttingItems();
            }

            return cuttingToolLifeCycle;
        }
    }
}