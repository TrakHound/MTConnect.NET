// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.RawMaterials
{
    /// <summary>
    /// JSON serialization surrogate for the <c>Material</c> a raw material
    /// asset is made of. Mirrors the on-the-wire shape so the JSON serializer
    /// can read and write it, then converts to and from the strongly-typed
    /// <see cref="Material"/> model.
    /// </summary>
    public class JsonMaterial
    {
        /// <summary>
        /// The unique identifier of the material.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The human-readable name of the material.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the material.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The lot the material was produced in.
        /// </summary>
        [JsonPropertyName("lot")]
        public string Lot { get; set; }

        /// <summary>
        /// The manufacturer of the material.
        /// </summary>
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// The date the material was manufactured.
        /// </summary>
        [JsonPropertyName("manufacturingDate")]
        public DateTime? ManufacturingDate { get; set; }

        /// <summary>
        /// The manufacturer's code identifying the material.
        /// </summary>
        [JsonPropertyName("manufacturingCode")]
        public string ManufacturingCode { get; set; }

        /// <summary>
        /// The standardized code identifying the material.
        /// </summary>
        [JsonPropertyName("materialCode")]
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
        /// Converts this surrogate to a strongly-typed <see cref="IMaterial"/>.
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