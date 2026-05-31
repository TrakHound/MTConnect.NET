// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration <c>Motion</c> in the
    /// cppagent-compatible shape. Inline coordinates are emitted as JSON
    /// arrays rather than space-separated strings. Converts to and from the
    /// strongly-typed <see cref="Motion"/> model.
    /// </summary>
    public class JsonMotion
    {
        /// <summary>
        /// The unique <c>id</c> of the motion definition.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the parent motion this motion is
        /// defined relative to.
        /// </summary>
        [JsonPropertyName("parentIdRef")]
        public string ParentIdRef { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the coordinate system the motion is
        /// expressed in.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The kind of motion (for example REVOLUTE or PRISMATIC).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// How the motion is actuated.
        /// </summary>
        [JsonPropertyName("actuation")]
        public string Actuation { get; set; }

        /// <summary>
        /// The free-form description of the motion.
        /// </summary>
        [JsonPropertyName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// The origin of the motion as a numeric JSON array, when expressed
        /// inline.
        /// </summary>
        [JsonPropertyName("Origin")]
        public IEnumerable<double> Origin { get; set; }

        /// <summary>
        /// The origin of the motion as a data set, when expressed by
        /// reference.
        /// </summary>
        [JsonPropertyName("OriginDataSet")]
        public JsonOriginDataSet OriginDataSet { get; set; }

        /// <summary>
        /// The translation and rotation relating the motion to its parent.
        /// </summary>
        [JsonPropertyName("Transformation")]
        public JsonTransformation Transformation { get; set; }

        /// <summary>
        /// The axis of the motion as a numeric JSON array, when expressed
        /// inline.
        /// </summary>
        [JsonPropertyName("Axis")]
        public IEnumerable<double> Axis { get; set; }

        /// <summary>
        /// The axis of the motion as a data set, when expressed by reference.
        /// </summary>
        [JsonPropertyName("AxisDataSet")]
        public JsonAxisDataSet AxisDataSet { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonMotion() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IMotion"/>, selecting the inline numeric-array or
        /// data-set representation for the origin and axis.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="IMotion"/>,
        /// parsing the type and actuation enumerations and preferring the
        /// data-set representation of the origin and axis when present.
        /// </summary>
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
