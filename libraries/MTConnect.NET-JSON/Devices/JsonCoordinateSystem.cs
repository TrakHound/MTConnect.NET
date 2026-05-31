// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration
    /// <c>CoordinateSystem</c>, which defines a frame of reference for a
    /// component's values. Mirrors the on-the-wire shape so the JSON
    /// serializer can read and write it, then converts to and from the
    /// strongly-typed <see cref="CoordinateSystem"/> model.
    /// </summary>
    public class JsonCoordinateSystem
    {
        /// <summary>
        /// The unique <c>id</c> of the coordinate system.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The human-readable name of the coordinate system.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The name the coordinate system is known by in the native data
        /// source.
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the parent coordinate system this
        /// system is defined relative to.
        /// </summary>
        [JsonPropertyName("parentIdRef")]
        public string ParentIdRef { get; set; }

        /// <summary>
        /// The kind of coordinate system (for example WORLD, MACHINE, or
        /// OBJECT).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The origin of the coordinate system as a coordinate triple, when
        /// expressed inline.
        /// </summary>
        [JsonPropertyName("origin")]
        public JsonOrigin Origin { get; set; }

        /// <summary>
        /// The origin of the coordinate system as a data set, when expressed
        /// by reference.
        /// </summary>
        [JsonPropertyName("originDataSet")]
        public JsonOriginDataSet OriginDataSet { get; set; }

        /// <summary>
        /// The translation and rotation relating this coordinate system to its
        /// parent.
        /// </summary>
        [JsonPropertyName("transformation")]
        public JsonTransformation Transformation { get; set; }

        /// <summary>
        /// The free-form description of the coordinate system.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCoordinateSystem() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICoordinateSystem"/>, selecting the inline or data-set
        /// representation for the origin based on the source type.
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
                else if (coordinateSystem.Origin is IOrigin origin) Origin = new JsonOrigin(origin);
                if (coordinateSystem.Transformation != null) Transformation = new JsonTransformation(coordinateSystem.Transformation);
                Description = coordinateSystem.Description;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ICoordinateSystem"/>, parsing the type enumeration and
        /// preferring the data-set representation of the origin when present.
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
            else if (Origin != null) coordinateSystem.Origin = Origin.ToOrigin();
            if (Transformation != null) coordinateSystem.Transformation = Transformation.ToTransformation();
            coordinateSystem.Description = Description;
            return coordinateSystem;
        }
    }
}
