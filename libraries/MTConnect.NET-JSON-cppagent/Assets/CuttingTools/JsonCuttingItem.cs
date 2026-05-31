// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a <c>CuttingItem</c> in the
    /// cppagent-compatible shape. Manufacturers are joined into a single
    /// comma-separated string. Converts to and from the strongly-typed
    /// <see cref="CuttingItem"/> model.
    /// </summary>
    public class JsonCuttingItem
    {
        /// <summary>
        /// The unique identifier of the cutting item.
        /// </summary>
        [JsonPropertyName("itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// The index or range of indices identifying the cutting item's
        /// position on the tool.
        /// </summary>
        [JsonPropertyName("indices")]
        public string Indices { get; set; }

        /// <summary>
        /// The material grade of the cutting item.
        /// </summary>
        [JsonPropertyName("grade")]
        public string Grade { get; set; }

        /// <summary>
        /// The free-form description of the cutting item.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The location of the cutting item relative to the tool.
        /// </summary>
        [JsonPropertyName("locus")]
        public string Locus { get; set; }

        /// <summary>
        /// The manufacturers of the cutting item, joined as a comma-separated
        /// string.
        /// </summary>
        [JsonPropertyName("manufacturers")]
        public string Manufacturers { get; set; }

        /// <summary>
        /// The program tool group the cutting item is assigned to.
        /// </summary>
        [JsonPropertyName("programToolGroup")]
        public string ProgramToolGroup { get; set; }

        /// <summary>
        /// The measured and remaining life values of the cutting item.
        /// </summary>
        [JsonPropertyName("ItemLife")]
        public IEnumerable<JsonItemLife> ItemLife { get; set; }

        /// <summary>
        /// The status values of the cutting item.
        /// </summary>
        [JsonPropertyName("CutterStatus")]
        public JsonCutterStatusCollection CutterStatus { get; set; }

        /// <summary>
        /// The dimensional measurements of the cutting item.
        /// </summary>
        [JsonPropertyName("Measurements")]
        public JsonMeasurements Measurements { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingItem() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICuttingItem"/>, joining manufacturers with a comma.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="ICuttingItem"/>,
        /// splitting comma-separated manufacturers back into a collection.
        /// </summary>
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