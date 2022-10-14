// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonTransformation
    {
        [JsonPropertyName("translation")]
        public string Translation { get; set; }

        [JsonPropertyName("rotation")]
        public string Rotation { get; set; }


        public JsonTransformation() { }

        public JsonTransformation(ITransformation transformation)
        {
            if (transformation != null)
            {
                Translation = transformation.Translation;
                Rotation = transformation.Rotation;
            }
        }


        public ITransformation ToTransformation()
        {
            var transformation = new Transformation();
            transformation.Translation = Translation;
            transformation.Rotation = Rotation;
            return transformation;
        }
    }
}
