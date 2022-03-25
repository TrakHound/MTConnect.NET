// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Configuration contains technical information about a component describing its physical layout,
    /// functional characteristics, and relationships with other components within a piece of equipment.
    /// </summary>
    [XmlRoot("Configuration")]
    public class XmlConfiguration
    {
        /// <summary>
        /// CoordinateSystems organizes CoordinateSystem elements for a Component and its children.
        /// </summary>
        [XmlArray("CoordinateSystems")]
        [XmlArrayItem("CoordinateSystem", typeof(XmlCoordinateSystem))]
        public List<XmlCoordinateSystem> CoordinateSystems { get; set; }

        [XmlIgnore]
        public bool CoordinateSystemsSpecified => !CoordinateSystems.IsNullOrEmpty();

        /// <summary>
        /// Motion defines the movement of the Component relative to a coordinate system.
        /// </summary>
        [XmlElement("Motion")]
        public XmlMotion Motion { get; set; }

        /// <summary>
        /// Relationships organizes Relationship elements for a Component.
        /// </summary>
        [XmlArray("Relationships")]
        [XmlArrayItem("DeviceRelationship", typeof(XmlDeviceRelationship))]
        [XmlArrayItem("ComponentRelationShip", typeof(XmlComponentRelationship))]
        public List<XmlRelationship> Relationships { get; set; }

        [XmlIgnore]
        public bool RelationshipsSpecified => !Relationships.IsNullOrEmpty();

        /// <summary>
        /// SensorConfiguration contains configuration information about a Sensor.
        /// </summary>
        [XmlElement("SensorConfiguration")]
        public XmlSensorConfiguration SensorConfiguration { get; set; }

        /// <summary>
        /// SolidModel references a file with the three-dimensional geometry of the Component or Composition.
        /// </summary>
        [XmlElement("SolidModel")]
        public XmlSolidModel SolidModel { get; set; }

        /// <summary>
        /// Specifications organizes Specification elements for a Component. 
        /// </summary>
        [XmlArray("Specifications")]
        [XmlArrayItem("Specification", typeof(XmlSpecification))]
        [XmlArrayItem("ProcessSpecification", typeof(XmlProcessSpecification))]
        public List<XmlAbstractSpecification> Specifications { get; set; }

        [XmlIgnore]
        public bool SpecificationsSpecified => !Specifications.IsNullOrEmpty();


        public XmlConfiguration() { }

        public XmlConfiguration(IConfiguration configuration)
        {
            if (configuration != null)
            {
                // Coordinate Systems
                if (!configuration.CoordinateSystems.IsNullOrEmpty())
                {
                    var coordinateSystems = new List<XmlCoordinateSystem>();
                    foreach (var coordinateSystem in configuration.CoordinateSystems)
                    {
                        coordinateSystems.Add(new XmlCoordinateSystem(coordinateSystem));
                    }
                    CoordinateSystems = coordinateSystems;
                }

                // Motion
                if (configuration.Motion != null)
                {
                    Motion = new XmlMotion(configuration.Motion);
                }

                // Relationships
                if (!configuration.Relationships.IsNullOrEmpty())
                {
                    var relationships = new List<XmlRelationship>();
                    foreach (var relationship in configuration.Relationships)
                    {
                        if (relationship.GetType() == typeof(DeviceRelationship))
                        {
                            relationships.Add(new XmlDeviceRelationship((DeviceRelationship)relationship));
                        }

                        if (relationship.GetType() == typeof(ComponentRelationship))
                        {
                            relationships.Add(new XmlComponentRelationship((ComponentRelationship)relationship));
                        }
                    }
                    Relationships = relationships;
                }

                // Sensor Configuration
                if (configuration.SensorConfiguration != null)
                {
                    SensorConfiguration = new XmlSensorConfiguration(configuration.SensorConfiguration);
                }

                // SolidModel
                if (configuration.SolidModel != null)
                {
                    SolidModel = new XmlSolidModel(configuration.SolidModel);
                }

                // Specifications
                if (!configuration.Specifications.IsNullOrEmpty())
                {
                    var specifications = new List<XmlAbstractSpecification>();
                    foreach (var specification in configuration.Specifications)
                    {
                        if (specification.GetType() == typeof(Specification))
                        {
                            specifications.Add(new XmlSpecification((Specification)specification));
                        }

                        if (specification.GetType() == typeof(ProcessSpecification))
                        {
                            specifications.Add(new XmlProcessSpecification((ProcessSpecification)specification));
                        }
                    }
                    Specifications = specifications;
                }
            }
        }

        public Configuration ToConfiguration()
        {
            var configuration = new Configuration();
            
            // Coordinate Systems
            if (!CoordinateSystems.IsNullOrEmpty())
            {
                var coordinateSystems = new List<CoordinateSystem>();
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
            if (!Relationships.IsNullOrEmpty())
            {
                var relationships = new List<Relationship>();
                foreach (var relationship in Relationships)
                {
                    relationships.Add(relationship.ToRelationship());
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
            if (!Specifications.IsNullOrEmpty())
            {
                var specifications = new List<AbstractSpecification>();
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
