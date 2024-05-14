// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCuttingToolLifeCycle
    {
        [JsonPropertyName("ProgramToolGroup")]
        public string ProgramToolGroup { get; set; }

        [JsonPropertyName("ProgramToolNumber")]
        public string ProgramToolNumber { get; set; }

        [JsonPropertyName("ConnectionCodeMachineSide")]
        public string ConnectionCodeMachineSide { get; set; }

        [JsonPropertyName("CutterStatus")]
        public JsonCutterStatusCollection CutterStatus { get; set; }

        [JsonPropertyName("ReconditionCount")]
        public JsonReconditionCount ReconditionCount { get; set; }

        [JsonPropertyName("ToolLife")]
        public IEnumerable<JsonToolLife> ToolLife { get; set; }

        [JsonPropertyName("Location")]
        public JsonLocation Location { get; set; }

        [JsonPropertyName("ProcessSpindleSpeed")]
        public JsonProcessSpindleSpeed ProcessSpindleSpeed { get; set; }

        [JsonPropertyName("ProcessFeedRate")]
        public JsonProcessFeedRate ProcessFeedRate { get; set; }

        [JsonPropertyName("Measurements")]
        public JsonMeasurements Measurements { get; set; }

        [JsonPropertyName("CuttingItems")]
        public JsonCuttingItemCollection CuttingItems { get; set; }


        public JsonCuttingToolLifeCycle() { }

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