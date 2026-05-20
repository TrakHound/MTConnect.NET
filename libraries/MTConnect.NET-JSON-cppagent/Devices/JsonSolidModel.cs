// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration <c>SolidModel</c> in
    /// the cppagent-compatible shape. References to an external 3D model file
    /// are emitted as scalar properties, while the inline scale is emitted as
    /// a numeric JSON array rather than a space-separated string. Converts to
    /// and from the strongly-typed <see cref="SolidModel"/> model.
    /// </summary>
    public class JsonSolidModel
    {
        /// <summary>
        /// The unique <c>id</c> of the solid model.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of another <c>SolidModel</c> this one
        /// derives from.
        /// </summary>
        [JsonPropertyName("solidModelIdRef")]
        public string SolidModelIdRef { get; set; }

        /// <summary>
        /// URL pointing at the external 3D model resource.
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// Reference to an item inside the external solid-model resource.
        /// </summary>
        [JsonPropertyName("itemRef")]
        public string ItemRef { get; set; }

        /// <summary>
        /// IANA media type of the referenced 3D model resource (for example
        /// <c>STEP</c>, <c>STL</c>, <c>GLTF</c>).
        /// </summary>
        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the coordinate system the solid
        /// model is expressed in.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The translation and rotation relating the solid model to its
        /// parent coordinate system.
        /// </summary>
        [JsonPropertyName("Transformation")]
        public JsonTransformation Transformation { get; set; }

        /// <summary>
        /// The scale of the solid model as a numeric JSON array, when
        /// expressed inline.
        /// </summary>
        [JsonPropertyName("Scale")]
        public IEnumerable<double> Scale { get; set; }

        /// <summary>
        /// The scale of the solid model as a data set, when expressed by
        /// reference.
        /// </summary>
        [JsonPropertyName("ScaleDataSet")]
        public JsonScaleDataSet ScaleDataSet { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSolidModel() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ISolidModel"/>, selecting the inline numeric-array or
        /// data-set representation for the scale.
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
                else if (solidModel.Scale is IScale scale) Scale = JsonHelper.ToJsonArray(scale);
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISolidModel"/>, parsing the media-type enumeration and
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
            else solidModel.Scale = JsonHelper.ToScale(Scale);
            return solidModel;
        }
    }
}
