// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a coordinate
    /// <c>Transformation</c> in the cppagent-compatible shape, made up
    /// of a translation and a rotation. Each component is emitted either
    /// inline as a numeric JSON array (when expressed as a literal
    /// vector) or by reference as a data set. Converts to and from the
    /// strongly-typed <see cref="Transformation"/> model.
    /// </summary>
    public class JsonTransformation
    {
        /// <summary>
        /// The translation vector as a numeric JSON array, when
        /// expressed inline.
        /// </summary>
        [JsonPropertyName("Translation")]
        public IEnumerable<double> Translation { get; set; }

        /// <summary>
        /// The translation expressed by reference as a data set.
        /// </summary>
        [JsonPropertyName("TranslationDataSet")]
        public JsonTranslationDataSet TranslationDataSet { get; set; }

        /// <summary>
        /// The rotation vector as a numeric JSON array, when expressed
        /// inline.
        /// </summary>
        [JsonPropertyName("Rotation")]
        public IEnumerable<double> Rotation { get; set; }

        /// <summary>
        /// The rotation expressed by reference as a data set.
        /// </summary>
        [JsonPropertyName("RotationDataSet")]
        public JsonRotationDataSet RotationDataSet { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonTransformation() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ITransformation"/>, selecting the inline numeric
        /// array or data-set representation independently for the
        /// translation and rotation.
        /// </summary>
        public JsonTransformation(ITransformation transformation)
        {
            if (transformation != null)
            {
                if (transformation.Translation is ITranslationDataSet translationDataSet) TranslationDataSet = new JsonTranslationDataSet(translationDataSet);
                else if (transformation.Translation is ITranslation translation) Translation = JsonHelper.ToJsonArray(translation);
                if (transformation.Rotation is IRotationDataSet rotationDataSet) RotationDataSet = new JsonRotationDataSet(rotationDataSet);
                else if (transformation.Rotation is IRotation rotation) Rotation = JsonHelper.ToJsonArray(rotation);
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ITransformation"/>, preferring the data-set
        /// representation of each component when present.
        /// </summary>
        public ITransformation ToTransformation()
        {
            var transformation = new Transformation();
            if (TranslationDataSet != null) transformation.Translation = TranslationDataSet.ToTranslationDataSet();
            else transformation.Translation = JsonHelper.ToTranslation(Translation);
            if (RotationDataSet != null) transformation.Rotation = RotationDataSet.ToRotationDataSet();
            else transformation.Rotation = JsonHelper.ToRotation(Rotation);
            return transformation;
        }
    }
}
