// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration <c>SolidModel</c>,
    /// which references the 3D geometry describing a component. Mirrors the
    /// on-the-wire shape so the JSON serializer can read and write it, then
    /// converts to and from the strongly-typed <see cref="SolidModel"/> model.
    /// </summary>
    public class JsonSolidModel
    {
        /// <summary>
        /// The unique <c>id</c> of the solid model.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of another solid model this model is
        /// derived from.
        /// </summary>
        [JsonPropertyName("solidModelIdRef")]
        public string SolidModelIdRef { get; set; }

        /// <summary>
        /// The hyperlink to the external model file.
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// The identifier of the specific item within the referenced model
        /// file.
        /// </summary>
        [JsonPropertyName("itemRef")]
        public string ItemRef { get; set; }

        /// <summary>
        /// The media type of the model file, serialized as the enumeration
        /// name.
        /// </summary>
        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the coordinate system the model
        /// geometry is expressed in.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The translation and rotation applied to the model geometry.
        /// </summary>
        [JsonPropertyName("transformation")]
        public JsonTransformation Transformation { get; set; }

        /// <summary>
        /// The scale applied to the model geometry as a vector, when expressed
        /// inline.
        /// </summary>
        [JsonPropertyName("scale")]
        public JsonScale Scale { get; set; }

        /// <summary>
        /// The scale applied to the model geometry as a data set, when
        /// expressed by reference.
        /// </summary>
        [JsonPropertyName("scaleDataSet")]
        public JsonScaleDataSet ScaleDataSet { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSolidModel() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ISolidModel"/>, selecting the inline or data-set
        /// representation for the scale based on the source type.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISolidModel"/>, parsing the media type enumeration and
        /// preferring the data-set representation of the scale when present.
        /// </summary>
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
