// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Json.CuttingTools.Measurements;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a <c>CuttingItem</c>, an individual
    /// insert or cutting edge of a cutting tool. Mirrors the on-the-wire shape
    /// so the JSON serializer can read and write it, then converts to and from
    /// the strongly-typed <see cref="CuttingItem"/> model.
    /// </summary>
    public class JsonCuttingItem
    {
        /// <summary>
        /// The index or range of indices identifying the cutting item's
        /// position on the tool.
        /// </summary>
        [JsonPropertyName("indices")]
        public string Indices { get; set; }

        /// <summary>
        /// The unique identifier of the cutting item.
        /// </summary>
        [JsonPropertyName("itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// The manufacturers of the cutting item.
        /// </summary>
        [JsonPropertyName("manufacturers")]
        public IEnumerable<string> Manufacturers { get; set; }

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
        /// The program tool group the cutting item is assigned to.
        /// </summary>
        [JsonPropertyName("programToolGroup")]
        public string ProgramToolGroup { get; set; }

        /// <summary>
        /// The measured and remaining life values of the cutting item.
        /// </summary>
        [JsonPropertyName("itemLife")]
        public IEnumerable<JsonItemLife> ItemLife { get; set; }

        /// <summary>
        /// The status values of the cutting item (for example NEW or USED),
        /// serialized as enumeration names.
        /// </summary>
        [JsonPropertyName("cutterStatus")]
        public IEnumerable<string> CutterStatus { get; set; }

        /// <summary>
        /// The dimensional measurements of the cutting item.
        /// </summary>
        [JsonPropertyName("measurements")]
        public IEnumerable<JsonMeasurement> Measurements { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingItem() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICuttingItem"/>, converting cutter statuses to strings
        /// and each nested item life and measurement.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ICuttingItem"/>, parsing cutter statuses and converting
        /// each nested item life and measurement.
        /// </summary>
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
                var measurements = new List<IToolingMeasurement>();
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