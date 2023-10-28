// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Configuration")]
    public class XmlConfiguration
    {
        [XmlArray("CoordinateSystems")]
        [XmlArrayItem("CoordinateSystem", typeof(XmlCoordinateSystem))]
        public List<XmlCoordinateSystem> CoordinateSystems { get; set; }

        [XmlElement("Motion")]
        public XmlMotion Motion { get; set; }

        [XmlArray("Relationships")]
        [XmlArrayItem("DeviceRelationship", typeof(XmlDeviceRelationship))]
        [XmlArrayItem("ComponentRelationShip", typeof(XmlComponentRelationship))]
        public List<XmlRelationship> Relationships { get; set; }

        [XmlElement("SensorConfiguration")]
        public XmlSensorConfiguration SensorConfiguration { get; set; }

        [XmlElement("SolidModel")]
        public XmlSolidModel SolidModel { get; set; }

        [XmlArray("Specifications")]
        [XmlArrayItem("Specification", typeof(XmlSpecification))]
        [XmlArrayItem("ProcessSpecification", typeof(XmlProcessSpecification))]
        public List<XmlAbstractSpecification> Specifications { get; set; }


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
            if (!Relationships.IsNullOrEmpty())
            {
                var relationships = new List<IRelationship>();
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
                var specifications = new List<ISpecification>();
                //var specifications = new List<IAbstractSpecification>();
                foreach (var specification in Specifications)
                {
                    specifications.Add(specification.ToSpecification());
                }
                configuration.Specifications = specifications;
            }

            return configuration;
        }

        public static void WriteXml(XmlWriter writer, IConfiguration configuration, bool outputComments)
        {
            if (configuration != null)
            {
                // Add Comments
                if (outputComments)
                {
                    writer.WriteComment($"Configuration : {Configuration.DescriptionText}");
                }

                writer.WriteStartElement("Configuration");

                // Write CoordinateSystems
                if (!configuration.CoordinateSystems.IsNullOrEmpty())
                {
                    writer.WriteStartElement("CoordinateSystems");
                    foreach (var coordinateSystem in configuration.CoordinateSystems)
                    {
                        XmlCoordinateSystem.WriteXml(writer, coordinateSystem);
                    }
                    writer.WriteEndElement();
                }

                // Write Motion
                XmlMotion.WriteXml(writer, configuration.Motion);

                // Write Relationships
                if (!configuration.Relationships.IsNullOrEmpty())
                {
                    writer.WriteStartElement("Relationships");
                    foreach (var relationship in configuration.Relationships)
                    {
                        XmlRelationship.WriteXml(writer, relationship);
                    }
                    writer.WriteEndElement();
                }

                // Write Sensor Configuration
                XmlSensorConfiguration.WriteXml(writer, configuration.SensorConfiguration);

                // Write Solid Model
                XmlSolidModel.WriteXml(writer, configuration.SolidModel);

                // Write Specifications
                if (!configuration.Specifications.IsNullOrEmpty())
                {
                    writer.WriteStartElement("Specifications");
                    foreach (var specification in configuration.Specifications)
                    {
                        XmlSpecification.WriteXml(writer, specification);
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}