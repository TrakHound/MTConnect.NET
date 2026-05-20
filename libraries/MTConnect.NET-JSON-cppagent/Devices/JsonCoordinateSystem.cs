// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration
    /// <c>CoordinateSystem</c> in the cppagent-compatible shape. The origin
    /// is emitted inline as a numeric JSON array when expressed as a literal
    /// point, or by reference as an <c>OriginDataSet</c>. Converts to and
    /// from the strongly-typed <see cref="CoordinateSystem"/> model.
    /// </summary>
    public class JsonCoordinateSystem
    {
        /// <summary>
        /// The unique <c>id</c> of the coordinate system.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The descriptive name of the coordinate system.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The native name of the coordinate system used on the equipment.
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the parent coordinate system this
        /// one is defined relative to.
        /// </summary>
        [JsonPropertyName("parentIdRef")]
        public string ParentIdRef { get; set; }

        /// <summary>
        /// The kind of coordinate system (for example WORLD, BASE,
        /// MACHINE).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The origin of the coordinate system as a numeric JSON array,
        /// when expressed inline.
        /// </summary>
        [JsonPropertyName("Origin")]
        public IEnumerable<double> Origin { get; set; }

        /// <summary>
        /// The origin of the coordinate system as a data set, when
        /// expressed by reference.
        /// </summary>
        [JsonPropertyName("OriginDataSet")]
        public JsonOriginDataSet OriginDataSet { get; set; }

        /// <summary>
        /// The translation and rotation relating the coordinate system to
        /// its parent.
        /// </summary>
        [JsonPropertyName("Transformation")]
        public JsonTransformation Transformation { get; set; }

        /// <summary>
        /// Free-form description of the coordinate system.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCoordinateSystem() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICoordinateSystem"/>, selecting the inline numeric
        /// array or data-set representation for the origin.
        /// </summary>
        public JsonCoordinateSystem(ICoordinateSystem coordinateSystem)
        {
            if (coordinateSystem != null)
            {
                Id = coordinateSystem.Id;
                Name = coordinateSystem.Name;
                NativeName = coordinateSystem.NativeName;
                ParentIdRef = coordinateSystem.ParentIdRef;
                Type = coordinateSystem.Type.ToString();
                if (coordinateSystem.Origin is IOriginDataSet originDataSet) OriginDataSet = new JsonOriginDataSet(originDataSet);
                else if (coordinateSystem.Origin is IOrigin origin) Origin = JsonHelper.ToJsonArray(origin);
                if (coordinateSystem.Transformation != null) Transformation = new JsonTransformation(coordinateSystem.Transformation);
                Description = coordinateSystem.Description;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ICoordinateSystem"/>, parsing the type enumeration
        /// and preferring the data-set representation of the origin when
        /// present.
        /// </summary>
        public ICoordinateSystem ToCoordinateSystem()
        {
            var coordinateSystem = new CoordinateSystem();
            coordinateSystem.Id = Id;
            coordinateSystem.Name = Name;
            coordinateSystem.NativeName = NativeName;
            coordinateSystem.ParentIdRef = ParentIdRef;
            coordinateSystem.Type = Type.ConvertEnum<CoordinateSystemType>();
            if (OriginDataSet != null) coordinateSystem.Origin = OriginDataSet.ToOriginDataSet();
            else coordinateSystem.Origin = JsonHelper.ToOrigin(Origin);
            if (Transformation != null) coordinateSystem.Transformation = Transformation.ToTransformation();
            coordinateSystem.Description = Description;
            return coordinateSystem;
        }
    }
}
