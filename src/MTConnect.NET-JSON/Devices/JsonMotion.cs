// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.Motion;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonMotion
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("parentIdRef")]
        public string ParentIdRef { get; set; }

        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("actuation")]
        public string Actuation { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("origin")]
        public string Origin { get; set; }

        [JsonPropertyName("transformation")]
        public JsonTransformation Transformation { get; set; }

        [JsonPropertyName("axis")]
        public string Axis { get; set; }


        public JsonMotion() { }

        public JsonMotion(IMotion motion)
        {
            if (motion != null)
            {
                Id = motion.Id;
                ParentIdRef = motion.ParentIdRef;
                CoordinateSystemIdRef = motion.CoordinateSystemIdRef;
                Type = motion.Type.ToString();
                Actuation = motion.Actuation.ToString();
                Description = motion.Description;
                Origin = motion.Origin;
                if (motion.Transformation != null) Transformation = new JsonTransformation(motion.Transformation);
                Axis = motion.Axis;
            }
        }


        public IMotion ToMotion()
        {
            var motion = new Motion();
            motion.Id = Id;
            motion.ParentIdRef = ParentIdRef;
            motion.CoordinateSystemIdRef = CoordinateSystemIdRef;
            motion.Type = Type.ConvertEnum<MotionType>();
            motion.Actuation = Actuation.ConvertEnum<MotionActuationType>();
            motion.Axis = Axis;
            motion.Origin = Origin;
            if (Transformation != null) motion.Transformation = Transformation.ToTransformation();
            motion.Description = Description;
            return motion;
        }
    }
}