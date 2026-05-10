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
        public JsonScale Scale { get; set; }

        [JsonPropertyName("scaleDataSet")]
        public JsonScaleDataSet ScaleDataSet { get; set; }


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
                if (solidModel.Scale is IScaleDataSet scaleDataSet) ScaleDataSet = new JsonScaleDataSet(scaleDataSet);
                else if (solidModel.Scale is IScale scale) Scale = new JsonScale(scale);
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
            if (ScaleDataSet != null) solidModel.Scale = ScaleDataSet.ToScaleDataSet();
            else if (Scale != null) solidModel.Scale = Scale.ToScale();
            return solidModel;
        }
    }
}
