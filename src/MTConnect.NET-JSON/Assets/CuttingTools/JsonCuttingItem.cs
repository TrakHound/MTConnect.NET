// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Json.CuttingTools;
using MTConnect.Assets.Json.CuttingTools.Measurements;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
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

        public JsonCuttingItem(ICuttingItem cuttingItem)
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


        public ICuttingItem ToCuttingItem()
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
                var itemLifes = new List<IItemLife>();
                foreach (var itemLife in ItemLife)
                {
                    itemLifes.Add(itemLife.ToItemLife());
                }
                cuttingItem.ItemLife = itemLifes;
            }

            // CutterStatus
            if (!CutterStatus.IsNullOrEmpty())
            {
                var statuses = new List<CutterStatusType>();
                foreach (var cutterStatus in CutterStatus)
                {
                    statuses.Add(cutterStatus.ConvertEnum<CutterStatusType>());
                }
                cuttingItem.CutterStatus = statuses;
            }

            // Measurements
            if (!Measurements.IsNullOrEmpty())
            {
                var measurements = new List<IMeasurement>();
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