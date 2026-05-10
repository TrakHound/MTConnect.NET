// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
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

        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("Origin")]
        public IEnumerable<double> Origin { get; set; }

        [JsonPropertyName("OriginDataSet")]
        public JsonOriginDataSet OriginDataSet { get; set; }

        [JsonPropertyName("Transformation")]
        public JsonTransformation Transformation { get; set; }

        [JsonPropertyName("Axis")]
        public IEnumerable<double> Axis { get; set; }

        [JsonPropertyName("AxisDataSet")]
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
                if (motion.Origin is IOriginDataSet originDataSet) OriginDataSet = new JsonOriginDataSet(originDataSet);
                else if (motion.Origin is IOrigin origin) Origin = JsonHelper.ToJsonArray(origin);
                if (motion.Transformation != null) Transformation = new JsonTransformation(motion.Transformation);
                if (motion.Axis is IAxisDataSet axisDataSet) AxisDataSet = new JsonAxisDataSet(axisDataSet);
                else if (motion.Axis is IAxis axis) Axis = JsonHelper.ToJsonArray(axis);

                if (motion.Description != null)
                {
                    Description = motion.Description;
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
            if (AxisDataSet != null) motion.Axis = AxisDataSet.ToAxisDataSet();
            else motion.Axis = JsonHelper.ToAxis(Axis);
            if (OriginDataSet != null) motion.Origin = OriginDataSet.ToOriginDataSet();
            else motion.Origin = JsonHelper.ToOrigin(Origin);
            if (Transformation != null) motion.Transformation = Transformation.ToTransformation();

            if (Description != null)
            {
                motion.Description = Description;
            }

            return motion;
        }
    }
}
