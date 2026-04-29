// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonTransformation
    {
        [JsonPropertyName("Translation")]
        public IEnumerable<double> Translation { get; set; }

        [JsonPropertyName("TranslationDataSet")]
        public JsonTranslationDataSet TranslationDataSet { get; set; }

        [JsonPropertyName("Rotation")]
        public IEnumerable<double> Rotation { get; set; }

        [JsonPropertyName("RotationDataSet")]
        public JsonRotationDataSet RotationDataSet { get; set; }


        public JsonTransformation() { }

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
