// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonConfiguration
    {
        [JsonPropertyName("coordinateSystems")]
        public IEnumerable<JsonCoordinateSystem> CoordinateSystems { get; set; }

        [JsonPropertyName("motion")]
        public JsonMotion Motion { get; set; }

        [JsonPropertyName("relationships")]
        public JsonRelationshipContainer Relationships { get; set; }

        [JsonPropertyName("sensorConfiguration")]
        public JsonSensorConfiguration SensorConfiguration { get; set; }

        [JsonPropertyName("solidModel")]
        public JsonSolidModel SolidModel { get; set; }

        [JsonPropertyName("specifications")]
        public IEnumerable<JsonAbstractSpecification> Specifications { get; set; }


        public JsonConfiguration() { }

        public JsonConfiguration(IConfiguration configuration)
        {
            if (configuration != null)
            {
                // CoordinateSystems
                if (!configuration.CoordinateSystems.IsNullOrEmpty())
                {
                    var coordinateSystems = new List<JsonCoordinateSystem>();
                    foreach (var coordinateSystem in configuration.CoordinateSystems)
                    {
                        coordinateSystems.Add(new JsonCoordinateSystem(coordinateSystem));
                    }
                    CoordinateSystems = coordinateSystems;
                }

                // Motion
                if (configuration.Motion != null)
                {
                    Motion = new JsonMotion(configuration.Motion);
                }

                // Relationships
                if (!configuration.Relationships.IsNullOrEmpty())
                {
                    var relationships = new JsonRelationshipContainer();
                    foreach (var relationship in configuration.Relationships)
                    {
                        // ComponentRelationship
                        if (typeof(IComponentRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            if (relationships.ComponentRelationships == null) relationships.ComponentRelationships = new List<JsonRelationship>();
                            relationships.ComponentRelationships.Add(new JsonRelationship((IComponentRelationship)relationship));
                        }

                        // DataItemRelationship
                        if (typeof(IDataItemRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            if (relationships.DataItemRelationships == null) relationships.DataItemRelationships = new List<JsonRelationship>();
                            relationships.DataItemRelationships.Add(new JsonRelationship((IDataItemRelationship)relationship));
                        }

                        // DeviceRelationship
                        if (typeof(IDeviceRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            if (relationships.DeviceRelationships == null) relationships.DeviceRelationships = new List<JsonRelationship>();
                            relationships.DeviceRelationships.Add(new JsonRelationship((IDeviceRelationship)relationship));
                        }

                        // SpecificationRelationship
                        if (typeof(ISpecificationRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            if (relationships.SpecificationRelationships == null) relationships.SpecificationRelationships = new List<JsonRelationship>();
                            relationships.SpecificationRelationships.Add(new JsonRelationship((ISpecificationRelationship)relationship));
                        }
                    }
                    Relationships = relationships;
                }

                // SensorConfiguration
                if (configuration.SensorConfiguration != null)
                {
                    SensorConfiguration = new JsonSensorConfiguration(configuration.SensorConfiguration);
                }

                // SolidModel
                if (configuration.SolidModel != null)
                {
                    SolidModel = new JsonSolidModel(configuration.SolidModel);
                }

                // Specifications
                if (!configuration.Specifications.IsNullOrEmpty())
                {
                    var specifications = new List<JsonAbstractSpecification>();
                    foreach (var specification in configuration.Specifications)
                    {
                        specifications.Add(new JsonAbstractSpecification(specification));
                    }
                    Specifications = specifications;
                }
            }
        }


        public IConfiguration ToConfiguration()
        {
            var configuration = new Configuration();
            
            // Coordinate Systems
            if (!CoordinateSystems.IsNullOrEmpty())
            {
                var coordinateSystems = new List<ICoordinateSystem>();
                foreach (var coordinateSystem in CoordinateSystems)
                {
                    coordinateSystems.Add(coordinateSystem.ToCoordinateSystem());
                }
                configuration.CoordinateSystems = coordinateSystems;
            }

            // Motion
            if (Motion != null)
            {
                configuration.Motion = Motion.ToMotion();
            }

            // Relationships
            if (Relationships != null)
            {
                var relationships = new List<IConfigurationRelationship>();

                // AssetRelationship
                if (!Relationships.AssetRelationships.IsNullOrEmpty())
                {
                    foreach (var relationship in Relationships.AssetRelationships)
                    {
                        relationships.Add(relationship.ToAssetRelationship());
                    }
                }

                // ComponentRelationship
                if (!Relationships.ComponentRelationships.IsNullOrEmpty())
                {
                    foreach (var relationship in Relationships.ComponentRelationships)
                    {
                        relationships.Add(relationship.ToComponentRelationship());
                    }
                }

                //// DataItemRelationship
                //if (!Relationships.DataItemRelationships.IsNullOrEmpty())
                //{
                //    foreach (var relationship in Relationships.DataItemRelationships)
                //    {
                //        relationships.Add(relationship.ToDataItemRelationship());
                //    }
                //}

                // DeviceRelationship
                if (!Relationships.DeviceRelationships.IsNullOrEmpty())
                {
                    foreach (var relationship in Relationships.DeviceRelationships)
                    {
                        relationships.Add(relationship.ToDeviceRelationship());
                    }
                }

                //// SpecificationRelationship
                //if (!Relationships.SpecificationRelationships.IsNullOrEmpty())
                //{
                //    foreach (var relationship in Relationships.SpecificationRelationships)
                //    {
                //        relationships.Add(relationship.ToSpecificationRelationship());
                //    }
                //}

                configuration.Relationships = relationships;
            }

            // Sensor Configuration
            if (SensorConfiguration != null)
            {
                configuration.SensorConfiguration = SensorConfiguration.ToSensorConfiguration();
            }

            // Solid Model
            if (SolidModel != null)
            {
                configuration.SolidModel = SolidModel.ToSolidModel();
            }

            // Specifications
            if (!Specifications.IsNullOrEmpty())
            {
                var specifications = new List<ISpecification>();
                foreach (var specification in Specifications)
                {
                    specifications.Add(specification.ToSpecification());
                }
                configuration.Specifications = specifications;
            }

            return configuration;
        }
    }
}