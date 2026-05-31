// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a CoordinateSystem <c>Transformation</c>,
    /// expressing the translation and rotation that relate one coordinate
    /// system to its parent. Each of translation and rotation is carried either
    /// as a scalar value or as a keyed data set.
    /// </summary>
    public class JsonTransformation
    {
        /// <summary>
        /// The translation expressed as a scalar value.
        /// </summary>
        [JsonPropertyName("translation")]
        public JsonTranslation Translation { get; set; }

        /// <summary>
        /// The translation expressed as a keyed data set.
        /// </summary>
        [JsonPropertyName("translationDataSet")]
        public JsonTranslationDataSet TranslationDataSet { get; set; }

        /// <summary>
        /// The rotation expressed as a scalar value.
        /// </summary>
        [JsonPropertyName("rotation")]
        public JsonRotation Rotation { get; set; }

        /// <summary>
        /// The rotation expressed as a keyed data set.
        /// </summary>
        [JsonPropertyName("rotationDataSet")]
        public JsonRotationDataSet RotationDataSet { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonTransformation() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ITransformation"/>, selecting the scalar or data-set
        /// representation of translation and rotation as appropriate.
        /// </summary>
        public JsonTransformation(ITransformation transformation)
        {
            if (transformation != null)
            {
                if (transformation.Translation is ITranslationDataSet translationDataSet) TranslationDataSet = new JsonTranslationDataSet(translationDataSet);
                else if (transformation.Translation is ITranslation translation) Translation = new JsonTranslation(translation);
                if (transformation.Rotation is IRotationDataSet rotationDataSet) RotationDataSet = new JsonRotationDataSet(rotationDataSet);
                else if (transformation.Rotation is IRotation rotation) Rotation = new JsonRotation(rotation);
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="Transformation"/>, preferring the data-set form over the
        /// scalar form when both are present.
        /// </summary>
        public ITransformation ToTransformation()
        {
            var transformation = new Transformation();
            if (TranslationDataSet != null) transformation.Translation = TranslationDataSet.ToTranslationDataSet();
            else if (Translation != null) transformation.Translation = Translation.ToTranslation();
            if (RotationDataSet != null) transformation.Rotation = RotationDataSet.ToRotationDataSet();
            else if (Rotation != null) transformation.Rotation = Rotation.ToRotation();
            return transformation;
        }
    }
}
