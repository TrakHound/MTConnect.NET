// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Channels;

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

        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("Origin")]
        public IEnumerable<double> Origin { get; set; }

        [JsonPropertyName("Transformation")]
        public JsonTransformation Transformation { get; set; }

        [JsonPropertyName("Axis")]
        public IEnumerable<double> Axis { get; set; }


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
                if (motion.Origin != null) Origin = motion.Origin.ToJsonArray();
                if (motion.Transformation != null) Transformation = new JsonTransformation(motion.Transformation);
                Axis = motion.Axis.ToJsonArray();

                if (motion.Description != null)
                {
                    Description = motion.Description.Value;
                }
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
            motion.Axis = JsonHelper.ToUnitVector3D(Axis);
            motion.Origin = JsonHelper.ToUnitVector3D(Origin);
            if (Transformation != null) motion.Transformation = Transformation.ToTransformation();

            if (Description != null)
            {
                var description = new Description();
                description.Value = Description;
                motion.Description = description;
            }

            return motion;
        }
    }
}