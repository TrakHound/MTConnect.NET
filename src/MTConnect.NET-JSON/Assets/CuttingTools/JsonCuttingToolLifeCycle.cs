// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Json.CuttingTools.Measurements;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCuttingToolLifeCycle
    {
        [JsonPropertyName("cutterStatus")]
        public IEnumerable<string> CutterStatus { get; set; }

        [JsonPropertyName("reconditionCount")]
        public JsonReconditionCount ReconditionCount { get; set; }

        [JsonPropertyName("toolLife")]
        public IEnumerable<JsonToolLife> ToolLife { get; set; }

        [JsonPropertyName("location")]
        public JsonLocation Location { get; set; }

        [JsonPropertyName("programToolGroup")]
        public string ProgramToolGroup { get; set; }

        [JsonPropertyName("programToolNumber")]
        public string ProgramToolNumber { get; set; }

        [JsonPropertyName("processSpindleSpeed")]
        public JsonProcessSpindleSpeed ProcessSpindleSpeed { get; set; }

        [JsonPropertyName("processFeedRate")]
        public JsonProcessFeedrate ProcessFeedRate { get; set; }

        [JsonPropertyName("connectionCodeMachineSide")]
        public string ConnectionCodeMachineSide { get; set; }

        [JsonPropertyName("measurements")]
        public IEnumerable<JsonMeasurement> Measurements { get; set; }

        [JsonPropertyName("cuttingItems")]
        public JsonCuttingItemCollection CuttingItems { get; set; }


        public JsonCuttingToolLifeCycle() { }

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

                if (CuttingItems != null) CuttingItems = new JsonCuttingItemCollection(cuttingToolLifeCycle.CuttingItems);
            }
        }


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
                var measurements = new List<IMeasurement>();
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