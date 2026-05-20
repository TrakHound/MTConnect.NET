// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.RawMaterials
{
    /// <summary>
    /// JSON serialization surrogate for the <c>Material</c> sub-element of
    /// a RawMaterial asset in the cppagent-compatible shape. Identifies
    /// the material itself (composition, manufacturer, lot, manufacturing
    /// date and codes) independently of the consumable quantity reported
    /// on the containing RawMaterial asset. Converts to and from the
    /// strongly-typed <see cref="Material"/> model.
    /// </summary>
    public class JsonMaterial
    {
        /// <summary>
        /// The unique <c>id</c> of the material.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The descriptive name of the material.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The kind of material (for example BAR, SHEET, BLOCK).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The lot or batch number of the material.
        /// </summary>
        [JsonPropertyName("Lot")]
        public string Lot { get; set; }

        /// <summary>
        /// The manufacturer of the material.
        /// </summary>
        [JsonPropertyName("Manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// The date the material was manufactured.
        /// </summary>
        [JsonPropertyName("ManufacturingDate")]
        public DateTime? ManufacturingDate { get; set; }

        /// <summary>
        /// The manufacturer's part or production code for the material.
        /// </summary>
        [JsonPropertyName("ManufacturingCode")]
        public string ManufacturingCode { get; set; }

        /// <summary>
        /// The material composition code (for example an alloy or grade
        /// designation).
        /// </summary>
        [JsonPropertyName("MaterialCode")]
        public string MaterialCode { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonMaterial() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IMaterial"/>.
        /// </summary>
        public JsonMaterial(IMaterial material)
        {
            if (material != null)
            {
                Id = material.Id;
                Name = material.Name;
                Type = material.Type;
                Lot = material.Lot;
                Manufacturer = material.Manufacturer;
                ManufacturingDate = material.ManufacturingDate;
                ManufacturingCode = material.ManufacturingCode;
                MaterialCode = material.MaterialCode;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IMaterial"/>.
        /// </summary>
        public IMaterial ToMaterial()
        {
            var material = new Material();
            material.Id = Id;
            material.Name = Name;
            material.Type = Type;
            material.Lot = Lot;
            material.Manufacturer = Manufacturer;
            material.ManufacturingDate = ManufacturingDate;
            material.ManufacturingCode = ManufacturingCode;
            material.MaterialCode = MaterialCode;
            return material;
        }
    }
}