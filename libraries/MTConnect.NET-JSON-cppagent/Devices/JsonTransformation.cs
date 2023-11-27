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

        [JsonPropertyName("Rotation")]
        public IEnumerable<double> Rotation { get; set; }


        public JsonTransformation() { }

        public JsonTransformation(ITransformation transformation)
        {
            if (transformation != null)
            {
                Translation = transformation.Translation.ToJsonArray();
                Rotation = transformation.Rotation.ToJsonArray();
            }
        }


        public ITransformation ToTransformation()
        {
            var transformation = new Transformation();
            transformation.Translation = JsonHelper.ToUnitVector3D(Translation);
            transformation.Rotation = JsonHelper.ToDegree3D(Rotation);
            return transformation;
        }
    }
}