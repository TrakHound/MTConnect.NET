// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonConfiguration
    {
        [JsonPropertyName("CoordinateSystems")]
        public JsonCoordinateSystems CoordinateSystems { get; set; }

        [JsonPropertyName("Motion")]
        public JsonMotion Motion { get; set; }

        [JsonPropertyName("Relationships")]
        public JsonRelationshipContainer Relationships { get; set; }

        [JsonPropertyName("SensorConfiguration")]
        public JsonSensorConfiguration SensorConfiguration { get; set; }

        [JsonPropertyName("SolidModel")]
        public JsonSolidModel SolidModel { get; set; }

        [JsonPropertyName("Specifications")]
        public JsonSpecificationContainer Specifications { get; set; }


        public JsonConfiguration() { }

        public JsonConfiguration(IConfiguration configuration)
        {
            if (configuration != null)
            {
                // CoordinateSystems
                if (!configuration.CoordinateSystems.IsNullOrEmpty())
                {
                    CoordinateSystems = new JsonCoordinateSystems(configuration.CoordinateSystems);
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

                        // DeviceRelationship
                        if (typeof(IDeviceRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            if (relationships.DeviceRelationships == null) relationships.DeviceRelationships = new List<JsonRelationship>();
                            relationships.DeviceRelationships.Add(new JsonRelationship((IDeviceRelationship)relationship));
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
                    var specifications = new JsonSpecificationContainer();
                    foreach (var specification in configuration.Specifications)
                    {
                        // Specification
                        if (typeof(ISpecification).IsAssignableFrom(specification.GetType()))
                        {
                            if (specifications.Specifications == null) specifications.Specifications = new List<JsonSpecification>();
                            specifications.Specifications.Add(new JsonSpecification((ISpecification)specification));
                        }

                        // ProcessSpecification
                        if (typeof(IProcessSpecification).IsAssignableFrom(specification.GetType()))
                        {
                            if (specifications.ProcessSpecifications == null) specifications.ProcessSpecifications = new List<JsonProcessSpecification>();
                            specifications.ProcessSpecifications.Add(new JsonProcessSpecification((IProcessSpecification)specification));
                        }
                    }
                    Specifications = specifications;
                }
            }
        }


        public IConfiguration ToConfiguration()
        {
            var configuration = new Configuration();
            
            // Coordinate Systems
            if (CoordinateSystems != null)
            {
                configuration.CoordinateSystems = CoordinateSystems.ToCoordinateSystems();
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

                // DeviceRelationship
                if (!Relationships.DeviceRelationships.IsNullOrEmpty())
                {
                    foreach (var relationship in Relationships.DeviceRelationships)
                    {
                        relationships.Add(relationship.ToDeviceRelationship());
                    }
                }

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
            if (Specifications != null)
            {
                var specifications = new List<ISpecification>();

                // AssetRelationship
                if (!Specifications.Specifications.IsNullOrEmpty())
                {
                    foreach (var specification in Specifications.Specifications)
                    {
                        specifications.Add(specification.ToSpecification());
                    }
                }

                // ComponentRelationship
                if (!Specifications.ProcessSpecifications.IsNullOrEmpty())
                {
                    foreach (var specification in Specifications.ProcessSpecifications)
                    {
                        specifications.Add(specification.ToSpecification());
                    }
                }

                configuration.Specifications = specifications;
            }

            return configuration;
        }
    }
}