// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonSolidModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("solidModelIdRef")]
        public string SolidModelIdRef { get; set; }

        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("itemRef")]
        public string ItemRef { get; set; }

        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; }

        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        [JsonPropertyName("transformation")]
        public JsonTransformation Transformation { get; set; }

        [JsonPropertyName("scale")]
        public string Scale { get; set; }


        public JsonSolidModel() { }

        public JsonSolidModel(ISolidModel solidModel)
        {
            if (solidModel != null)
            {
                Id = solidModel.Id;
                SolidModelIdRef = solidModel.SolidModelIdRef;
                Href = solidModel.Href;
                MediaType = solidModel.MediaType.ToString();
                CoordinateSystemIdRef = solidModel.CoordinateSystemIdRef;
                if (solidModel.Transformation != null) Transformation = new JsonTransformation(solidModel.Transformation);
                if (Scale != null) Scale = solidModel.Scale.ToString();
            }
        }


        public ISolidModel ToSolidModel()
        {
            var solidModel = new SolidModel();
            solidModel.Id = Id;
            solidModel.SolidModelIdRef = Id;
            solidModel.Href = Href;
            solidModel.ItemRef = ItemRef;
            solidModel.MediaType = MediaType.ConvertEnum<MediaType>();
            solidModel.CoordinateSystemIdRef = CoordinateSystemIdRef;
            if (Transformation != null) solidModel.Transformation = Transformation.ToTransformation();
            solidModel.Scale = UnitVector3D.FromString(Scale);
            return solidModel;
        }
    }
}