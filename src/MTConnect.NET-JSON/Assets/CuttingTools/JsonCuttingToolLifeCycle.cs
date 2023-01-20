// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools.Measurements;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class JsonCuttingToolLifeCycle
    {
        [JsonPropertyName("cutterStatus")]
        public IEnumerable<string> CutterStatus { get; set; }

        [JsonPropertyName("reconditionCount")]
        public JsonReconditionCount ReconditionCount { get; set; }

        [JsonPropertyName("toolLife")]
        public JsonToolLife ToolLife { get; set; }

        [JsonPropertyName("location")]
        public JsonLocation Location { get; set; }

        [JsonPropertyName("programToolGroup")]
        public string ProgramToolGroup { get; set; }

        [JsonPropertyName("programToolNumber")]
        public string ProgramToolNumber { get; set; }

        [JsonPropertyName("processSpindleSpeed")]
        public JsonProcessSpindleSpeed ProcessSpindleSpeed { get; set; }

        [JsonPropertyName("processFeedrate")]
        public JsonProcessFeedrate ProcessFeedrate { get; set; }

        [JsonPropertyName("connectionCodeMachineSide")]
        public string ConnectionCodeMachineSide { get; set; }

        [JsonPropertyName("measurements")]
        public IEnumerable<JsonMeasurement> Measurements { get; set; }

        [JsonPropertyName("cuttingItems")]
        public JsonCuttingItemCollection CuttingItems { get; set; }


        public JsonCuttingToolLifeCycle() { }

        public JsonCuttingToolLifeCycle(CuttingToolLifeCycle cuttingToolLifeCycle)
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
                if (cuttingToolLifeCycle.ToolLife != null) ToolLife = new JsonToolLife(cuttingToolLifeCycle.ToolLife);
                if (cuttingToolLifeCycle.Location != null) Location = new JsonLocation(cuttingToolLifeCycle.Location);
                ProgramToolGroup = cuttingToolLifeCycle.ProgramToolGroup;
                ProgramToolNumber = cuttingToolLifeCycle.ProgramToolNumber;
                if (cuttingToolLifeCycle.ProcessSpindleSpeed != null) ProcessSpindleSpeed = new JsonProcessSpindleSpeed(cuttingToolLifeCycle.ProcessSpindleSpeed);
                if (cuttingToolLifeCycle.ProcessFeedrate != null) ProcessFeedrate = new JsonProcessFeedrate(cuttingToolLifeCycle.ProcessFeedrate);
                ConnectionCodeMachineSide = cuttingToolLifeCycle.ConnectionCodeMachineSide;

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


        public CuttingToolLifeCycle ToCuttingToolLifeCycle()
        {
            var cuttingToolLifeCycle = new CuttingToolLifeCycle();

            // CutterStatus
            if (!CutterStatus.IsNullOrEmpty())
            {
                var statuses = new List<CutterStatus>();
                foreach (var cutterStatus in CutterStatus)
                {
                    statuses.Add(cutterStatus.ConvertEnum<CutterStatus>());
                }
                cuttingToolLifeCycle.CutterStatus = statuses;
            }

            if (ReconditionCount != null) cuttingToolLifeCycle.ReconditionCount = ReconditionCount.ToReconditionCount();
            if (ToolLife != null) cuttingToolLifeCycle.ToolLife = ToolLife.ToToolLife();
            if (Location != null) cuttingToolLifeCycle.Location = Location.ToLocation();
            cuttingToolLifeCycle.ProgramToolGroup = ProgramToolGroup;
            cuttingToolLifeCycle.ProgramToolNumber = ProgramToolNumber;
            if (ProcessSpindleSpeed != null) cuttingToolLifeCycle.ProcessSpindleSpeed = ProcessSpindleSpeed.ToProcessSpindleSpeed();
            if (ProcessFeedrate != null) cuttingToolLifeCycle.ProcessFeedrate = ProcessFeedrate.ToProcessFeedrate();
            cuttingToolLifeCycle.ConnectionCodeMachineSide = ConnectionCodeMachineSide;

            // Measurements
            if (!Measurements.IsNullOrEmpty())
            {
                var measurements = new List<Measurement>();
                foreach (var measurement in Measurements)
                {
                    measurements.Add(measurement.ToMeasurement());
                }
                cuttingToolLifeCycle.Measurements = measurements;
            }

            if (CuttingItems != null)
            {
                cuttingToolLifeCycle.CuttingItems = CuttingItems.ToCuttingItemCollection();
            }

            return cuttingToolLifeCycle;
        }
    }
}