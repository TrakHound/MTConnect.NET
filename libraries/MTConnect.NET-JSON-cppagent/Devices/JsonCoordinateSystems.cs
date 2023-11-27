// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonCoordinateSystems
    {
        [JsonPropertyName("CoordinateSystem")]
        public IEnumerable<JsonCoordinateSystem> CoordinateSystems { get; set; }


        public JsonCoordinateSystems() { }

        public JsonCoordinateSystems(IEnumerable<ICoordinateSystem> coordinateSystems)
        {
            if (!coordinateSystems.IsNullOrEmpty())
            {
                var jsonCoordinateSystems = new List<JsonCoordinateSystem>();

                foreach (var coordinateSystem in coordinateSystems) jsonCoordinateSystems.Add(new JsonCoordinateSystem(coordinateSystem));

                CoordinateSystems = jsonCoordinateSystems;
            }
        }


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