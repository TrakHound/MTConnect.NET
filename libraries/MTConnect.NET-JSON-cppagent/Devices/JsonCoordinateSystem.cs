// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonCoordinateSystem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        [JsonPropertyName("parentIdRef")]
        public string ParentIdRef { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("Origin")]
        public IEnumerable<double> Origin { get; set; }

        [JsonPropertyName("OriginDataSet")]
        public JsonOriginDataSet OriginDataSet { get; set; }

        [JsonPropertyName("Transformation")]
        public JsonTransformation Transformation { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }


        public JsonCoordinateSystem() { }

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
