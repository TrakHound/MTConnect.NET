// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.RawMaterials
{
    public class JsonMaterial
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("lot")]
        public string Lot { get; set; }

        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonPropertyName("manufacturingDate")]
        public DateTime? ManufacturingDate { get; set; }

        [JsonPropertyName("manufacturingCode")]
        public string ManufacturingCode { get; set; }

        [JsonPropertyName("materialCode")]
        public string MaterialCode { get; set; }


        public JsonMaterial() { }

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