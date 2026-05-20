// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a typed container of
    /// <see cref="JsonCoordinateSystem"/> items, keyed by the singular
    /// cppagent element name <c>CoordinateSystem</c>.
    /// </summary>
    public class JsonCoordinateSystems
    {
        /// <summary>
        /// The coordinate systems in the container.
        /// </summary>
        [JsonPropertyName("CoordinateSystem")]
        public IEnumerable<JsonCoordinateSystem> CoordinateSystems { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCoordinateSystems() { }

        /// <summary>
        /// Initializes the container from a coordinate-system
        /// sequence.
        /// </summary>
        public JsonCoordinateSystems(IEnumerable<ICoordinateSystem> coordinateSystems)
        {
            if (!coordinateSystems.IsNullOrEmpty())
            {
                var jsonCoordinateSystems = new List<JsonCoordinateSystem>();

                foreach (var coordinateSystem in coordinateSystems) jsonCoordinateSystems.Add(new JsonCoordinateSystem(coordinateSystem));

                CoordinateSystems = jsonCoordinateSystems;
            }
        }


        /// <summary>
        /// Flattens the container back into a uniform
        /// <see cref="ICoordinateSystem"/> sequence.
        /// </summary>
        public IEnumerable<ICoordinateSystem> ToCoordinateSystems()
        {
            var coordinateSystems = new List<ICoordinateSystem>();

            if (!CoordinateSystems.IsNullOrEmpty())
            {
                foreach (var coordinateSystem in CoordinateSystems) coordinateSystems.Add(coordinateSystem.ToCoordinateSystem());
            }

            return coordinateSystems;
        }
    }
}