// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration <c>Motion</c>, which
    /// describes how a component moves relative to its parent. Mirrors the
    /// on-the-wire shape so the JSON serializer can read and write it, then
    /// converts to and from the strongly-typed <see cref="Motion"/> model.
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
        /// The kind of motion (for example REVOLUTE, PRISMATIC, or FIXED).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// How the motion is actuated (DIRECT, VIRTUAL, or NONE).
        /// </summary>
        [JsonPropertyName("actuation")]
        public string Actuation { get; set; }

        /// <summary>
        /// The free-form description of the motion.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The origin of the motion as a coordinate triple, when expressed
        /// inline.
        /// </summary>
        [JsonPropertyName("origin")]
        public JsonOrigin Origin { get; set; }

        /// <summary>
        /// The origin of the motion as a data set, when expressed by
        /// reference.
        /// </summary>
        [JsonPropertyName("originDataSet")]
        public JsonOriginDataSet OriginDataSet { get; set; }

        /// <summary>
        /// The translation and rotation relating the motion to its parent.
        /// </summary>
        [JsonPropertyName("transformation")]
        public JsonTransformation Transformation { get; set; }

        /// <summary>
        /// The axis of the motion as a vector, when expressed inline.
        /// </summary>
        [JsonPropertyName("axis")]
        public JsonAxis Axis { get; set; }

        /// <summary>
        /// The axis of the motion as a data set, when expressed by reference.
        /// </summary>
        [JsonPropertyName("axisDataSet")]
        public JsonAxisDataSet AxisDataSet { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonMotion() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IMotion"/>, selecting the inline or data-set
        /// representation for the origin and axis based on the source type.
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
                if (motion.Description != null) Description = motion.Description;
                if (motion.Origin is IOriginDataSet originDataSet) OriginDataSet = new JsonOriginDataSet(originDataSet);
                else if (motion.Origin is IOrigin origin) Origin = new JsonOrigin(origin);
                if (motion.Transformation != null) Transformation = new JsonTransformation(motion.Transformation);
                if (motion.Axis is IAxisDataSet axisDataSet) AxisDataSet = new JsonAxisDataSet(axisDataSet);
                else if (motion.Axis is IAxis axis) Axis = new JsonAxis(axis);
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
            else if (Axis != null) motion.Axis = Axis.ToAxis();
            if (OriginDataSet != null) motion.Origin = OriginDataSet.ToOriginDataSet();
            else if (Origin != null) motion.Origin = Origin.ToOrigin();
            if (Transformation != null) motion.Transformation = Transformation.ToTransformation();
            if (Description != null) motion.Description = Description;
            return motion;
        }
    }
}
