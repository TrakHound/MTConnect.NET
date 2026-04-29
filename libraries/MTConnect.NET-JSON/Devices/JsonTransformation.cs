// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonTransformation
    {
        [JsonPropertyName("translation")]
        public JsonTranslation Translation { get; set; }

        [JsonPropertyName("translationDataSet")]
        public JsonTranslationDataSet TranslationDataSet { get; set; }

        [JsonPropertyName("rotation")]
        public JsonRotation Rotation { get; set; }

        [JsonPropertyName("rotationDataSet")]
        public JsonRotationDataSet RotationDataSet { get; set; }


        public JsonTransformation() { }

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
