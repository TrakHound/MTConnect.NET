// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCuttingItem
    {
        [JsonPropertyName("itemId")]
        public string ItemId { get; set; }

        [JsonPropertyName("indices")]
        public string Indices { get; set; }

        [JsonPropertyName("grade")]
        public string Grade { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("locus")]
        public string Locus { get; set; }

        [JsonPropertyName("manufacturers")]
        public string Manufacturers { get; set; }

        [JsonPropertyName("programToolGroup")]
        public string ProgramToolGroup { get; set; }

        [JsonPropertyName("ItemLife")]
        public IEnumerable<JsonItemLife> ItemLife { get; set; }

        [JsonPropertyName("CutterStatus")]
        public JsonCutterStatusCollection CutterStatus { get; set; }

        [JsonPropertyName("Measurements")]
        public JsonMeasurements Measurements { get; set; }


        public JsonCuttingItem() { }

        public JsonCuttingItem(ICuttingItem cuttingItem)
        {
            if (cuttingItem != null)
            {
                Indices = cuttingItem.Indices;
                ItemId = cuttingItem.ItemId;
                if (!cuttingItem.Manufacturers.IsNullOrEmpty()) Manufacturers = string.Join(", ", cuttingItem.Manufacturers);
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
                    CutterStatus = new JsonCutterStatusCollection(cuttingItem.CutterStatus);
                }

                // Measurements
                if (!cuttingItem.Measurements.IsNullOrEmpty())
                {
                    Measurements = new JsonMeasurements(cuttingItem.Measurements);
                }
            }
        }


        public ICuttingItem ToCuttingItem()
        {
            var cuttingItem = new CuttingItem();
            cuttingItem.Indices = Indices;
            cuttingItem.ItemId = ItemId;
            if (!string.IsNullOrEmpty(Manufacturers)) cuttingItem.Manufacturers = Manufacturers.Split(',');
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
            if (CutterStatus != null)
            {
                var statuses = new List<CutterStatusType>();
                foreach (var cutterStatus in CutterStatus.Status)
                {
                    statuses.Add(cutterStatus.ToCutterStatus());
                }
                cuttingItem.CutterStatus = statuses;
            }

            // Measurements
            if (Measurements != null)
            {
                cuttingItem.Measurements = Measurements.ToMeasurements();
            }

            return cuttingItem;
        }
    }
}