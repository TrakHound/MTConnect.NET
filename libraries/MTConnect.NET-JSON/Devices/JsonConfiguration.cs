// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Component <c>Configuration</c>, the
    /// container for coordinate systems, motion, relationships, sensor and
    /// solid-model configuration, and specifications.
    /// </summary>
    public class JsonConfiguration
    {
        /// <summary>
        /// The coordinate systems defined on the component.
        /// </summary>
        [JsonPropertyName("coordinateSystems")]
        public IEnumerable<JsonCoordinateSystem> CoordinateSystems { get; set; }

        /// <summary>
        /// The motion definition relating the component to its parent.
        /// </summary>
        [JsonPropertyName("motion")]
        public JsonMotion Motion { get; set; }

        /// <summary>
        /// The relationships from the component to other components, data
        /// items, devices, and specifications, grouped by relationship kind.
        /// </summary>
        [JsonPropertyName("relationships")]
        public JsonRelationshipContainer Relationships { get; set; }

        /// <summary>
        /// The sensor configuration when the component is a sensor.
        /// </summary>
        [JsonPropertyName("sensorConfiguration")]
        public JsonSensorConfiguration SensorConfiguration { get; set; }

        /// <summary>
        /// The solid-model reference describing the component geometry.
        /// </summary>
        [JsonPropertyName("solidModel")]
        public JsonSolidModel SolidModel { get; set; }

        /// <summary>
        /// The specifications defined on the component.
        /// </summary>
        [JsonPropertyName("specifications")]
        public IEnumerable<JsonAbstractSpecification> Specifications { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonConfiguration() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IConfiguration"/>, converting each child and grouping
        /// relationships by their concrete relationship interface.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Configuration"/>,
        /// converting each child and flattening the grouped relationships back
        /// into a single relationship collection.
        /// </summary>
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