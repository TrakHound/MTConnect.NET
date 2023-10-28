// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
                Translation = transformation.Translation.ToString();
                Rotation = transformation.Rotation.ToString();
            }
        }


        public ITransformation ToTransformation()
        {
            var transformation = new Transformation();
            transformation.Translation = UnitVector3D.FromString(Translation);
            transformation.Rotation = Degree3D.FromString(Rotation);
            return transformation;
        }
    }
}