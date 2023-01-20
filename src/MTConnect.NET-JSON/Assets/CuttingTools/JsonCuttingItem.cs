// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets.CuttingTools.Measurements;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class JsonCuttingItem
    {
        [JsonPropertyName("indices")]
        public string Indices { get; set; }

        [JsonPropertyName("itemId")]
        public string ItemId { get; set; }

        [JsonPropertyName("manufacturers")]
        public string Manufacturers { get; set; }

        [JsonPropertyName("grade")]
        public string Grade { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("locus")]
        public string Locus { get; set; }

        [JsonPropertyName("itemLife")]
        public IEnumerable<JsonItemLife> ItemLife { get; set; }

        [JsonPropertyName("cutterStatus")]
        public IEnumerable<string> CutterStatus { get; set; }

        [JsonPropertyName("programToolGroup")]
        public string ProgramToolGroup { get; set; }

        [JsonPropertyName("measurements")]
        public IEnumerable<JsonMeasurement> Measurements { get; set; }


        public JsonCuttingItem() { }

        public JsonCuttingItem(CuttingItem cuttingItem)
        {
            if (cuttingItem != null)
            {
                Indices = cuttingItem.Indices;
                ItemId = cuttingItem.ItemId;
                Manufacturers = cuttingItem.Manufacturers;
                Grade = cuttingItem.Grade;
                Description = cuttingItem.Description;
                Locus = cuttingItem.Locus;
                ProgramToolGroup = cuttingItem.ProgramToolGroup;

                // ItemLife
                if (!cuttingItem.ItemLife.IsNullOrEmpty())
                {
                    var itemLifes = new List<JsonItemLife>();
                    foreach (var itemLife in cuttingItem.ItemLife)
                    {
                        itemLifes.Add(new JsonItemLife(itemLife));
                    }
                    ItemLife = itemLifes;
                }

                // CutterStatus
                if (!cuttingItem.CutterStatus.IsNullOrEmpty())
                {
                    var statuses = new List<string>();
                    foreach (var cutterStatus in cuttingItem.CutterStatus)
                    {
                        statuses.Add(cutterStatus.ToString());
                    }
                    CutterStatus = statuses;
                }

                // Measurements
                if (!cuttingItem.Measurements.IsNullOrEmpty())
                {
                    var measurements = new List<JsonMeasurement>();
                    foreach (var measurement in cuttingItem.Measurements)
                    {
                        measurements.Add(new JsonMeasurement(measurement));
                    }
                    Measurements = measurements;
                }
            }
        }


        public CuttingItem ToCuttingItem()
        {
            var cuttingItem = new CuttingItem();
            cuttingItem.Indices = Indices;
            cuttingItem.ItemId = ItemId;
            cuttingItem.Manufacturers = Manufacturers;
            cuttingItem.Grade = Grade;
            cuttingItem.Description = Description;
            cuttingItem.Locus = Locus;
            cuttingItem.ProgramToolGroup = ProgramToolGroup;

            // ItemLife
            if (!ItemLife.IsNullOrEmpty())
            {
                var itemLifes = new List<ItemLife>();
                foreach (var itemLife in ItemLife)
                {
                    itemLifes.Add(itemLife.ToItemLife());
                }
                cuttingItem.ItemLife = itemLifes;
            }

            // CutterStatus
            if (!CutterStatus.IsNullOrEmpty())
            {
                var statuses = new List<CutterStatus>();
                foreach (var cutterStatus in CutterStatus)
                {
                    statuses.Add(cutterStatus.ConvertEnum<CutterStatus>());
                }
                cuttingItem.CutterStatus = statuses;
            }

            // Measurements
            if (!Measurements.IsNullOrEmpty())
            {
                var measurements = new List<Measurement>();
                foreach (var measurement in Measurements)
                {
                    measurements.Add(measurement.ToMeasurement());
                }
                cuttingItem.Measurements = measurements;
            }

            return cuttingItem;
        }
    }
}
