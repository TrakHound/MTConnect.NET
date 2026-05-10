// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
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
        public JsonOrigin Origin { get; set; }

        [JsonPropertyName("originDataSet")]
        public JsonOriginDataSet OriginDataSet { get; set; }

        [JsonPropertyName("transformation")]
        public JsonTransformation Transformation { get; set; }

        [JsonPropertyName("axis")]
        public JsonAxis Axis { get; set; }

        [JsonPropertyName("axisDataSet")]
        public JsonAxisDataSet AxisDataSet { get; set; }


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
                if (motion.Description != null) Description = motion.Description;
                if (motion.Origin is IOriginDataSet originDataSet) OriginDataSet = new JsonOriginDataSet(originDataSet);
                else if (motion.Origin is IOrigin origin) Origin = new JsonOrigin(origin);
                if (motion.Transformation != null) Transformation = new JsonTransformation(motion.Transformation);
                if (motion.Axis is IAxisDataSet axisDataSet) AxisDataSet = new JsonAxisDataSet(axisDataSet);
                else if (motion.Axis is IAxis axis) Axis = new JsonAxis(axis);
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
            if (AxisDataSet != null) motion.Axis = AxisDataSet.ToAxisDataSet();
            else if (Axis != null) motion.Axis = Axis.ToAxis();
            if (OriginDataSet != null) motion.Origin = OriginDataSet.ToOriginDataSet();
            else if (Origin != null) motion.Origin = Origin.ToOrigin();
            if (Transformation != null) motion.Transformation = Transformation.ToTransformation();
            if (Description != null) motion.Description = Description;
            return motion;
        }
    }
}
