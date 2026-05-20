// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>Configuration</c> in the
    /// cppagent-compatible shape. Aggregates the optional configuration
    /// sub-elements (coordinate systems, motion, relationships, sensor
    /// configuration, solid model, specifications), each emitted only
    /// when present. Relationships and specifications are routed through
    /// their typed sub-containers so that
    /// <c>ComponentRelationship</c>/<c>DeviceRelationship</c> and
    /// <c>Specification</c>/<c>ProcessSpecification</c> are kept in
    /// separate JSON sibling arrays. Converts to and from the
    /// strongly-typed <see cref="Configuration"/> model.
    /// </summary>
    public class JsonConfiguration
    {
        /// <summary>
        /// The coordinate systems defined for the component.
        /// </summary>
        [JsonPropertyName("CoordinateSystems")]
        public JsonCoordinateSystems CoordinateSystems { get; set; }

        /// <summary>
        /// The motion definition for the component.
        /// </summary>
        [JsonPropertyName("Motion")]
        public JsonMotion Motion { get; set; }

        /// <summary>
        /// Relationships to other components, devices, or assets,
        /// partitioned by relationship kind in the surrogate container.
        /// </summary>
        [JsonPropertyName("Relationships")]
        public JsonRelationshipContainer Relationships { get; set; }

        /// <summary>
        /// The sensor configuration definition for the component.
        /// </summary>
        [JsonPropertyName("SensorConfiguration")]
        public JsonSensorConfiguration SensorConfiguration { get; set; }

        /// <summary>
        /// The solid model reference for the component.
        /// </summary>
        [JsonPropertyName("SolidModel")]
        public JsonSolidModel SolidModel { get; set; }

        /// <summary>
        /// Specifications constraining the component, partitioned into
        /// scalar specifications and process specifications.
        /// </summary>
        [JsonPropertyName("Specifications")]
        public JsonSpecificationContainer Specifications { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonConfiguration() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IConfiguration"/>, partitioning relationships into
        /// component versus device kinds and specifications into scalar
        /// versus process kinds for serialization.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IConfiguration"/>, flattening the typed
        /// relationship and specification sub-containers back into the
        /// uniform model collections.
        /// </summary>
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