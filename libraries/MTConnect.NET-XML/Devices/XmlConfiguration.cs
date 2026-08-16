// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a component's <c>Configuration</c>, the
    /// container for its coordinate systems, motion, relationships, sensor
    /// settings, geometry, and specifications. Mirrors the on-the-wire element
    /// and converts to and from the strongly-typed <see cref="Configuration"/>
    /// model.
    /// </summary>
    /// <remarks>
    /// Any child element that is neither a standard configuration child nor a
    /// namespaced vendor extension declared on this surrogate is captured
    /// into <see cref="VendorExtensions"/> as a raw <see cref="XmlElement"/>
    /// via <see cref="XmlAnyElementAttribute"/>, then projected onto
    /// <see cref="IConfiguration.VendorExtensions"/> as an
    /// <see cref="XElement"/>. This mirrors the MTConnect XSD's
    /// <c>ComponentConfigurationType</c> which permits any element that
    /// substitutes for the abstract <c>AbstractConfiguration</c> element —
    /// i.e. any element a vendor XSD has declared with
    /// <c>substitutionGroup='AbstractConfiguration'</c>.
    /// </remarks>
    [XmlRoot("Configuration")]
    public class XmlConfiguration
    {
        /// <summary>
        /// The coordinate systems the component's geometry is expressed in.
        /// </summary>
        [XmlArray("CoordinateSystems")]
        [XmlArrayItem("CoordinateSystem", typeof(XmlCoordinateSystem))]
        public List<XmlCoordinateSystem> CoordinateSystems { get; set; }

        /// <summary>
        /// The kinematic relationship of the component to its parent.
        /// </summary>
        [XmlElement("Motion")]
        public XmlMotion Motion { get; set; }

        /// <summary>
        /// The relationships from this component to assets, devices, and other
        /// components.
        /// </summary>
        [XmlArray("Relationships")]
        [XmlArrayItem("AssetRelationship", typeof(XmlAssetRelationship))]
        [XmlArrayItem("DeviceRelationship", typeof(XmlDeviceRelationship))]
        [XmlArrayItem("ComponentRelationship", typeof(XmlComponentRelationship))]
        public List<XmlConfigurationRelationship> Relationships { get; set; }

        /// <summary>
        /// The sensor calibration and channel settings of the component.
        /// </summary>
        [XmlElement("SensorConfiguration")]
        public XmlSensorConfiguration SensorConfiguration { get; set; }

        /// <summary>
        /// The 3D geometry reference for the component.
        /// </summary>
        [XmlElement("SolidModel")]
        public XmlSolidModel SolidModel { get; set; }

        /// <summary>
        /// The specifications constraining the component's measured values.
        /// </summary>
        [XmlArray("Specifications")]
        [XmlArrayItem("Specification", typeof(XmlSpecification))]
        [XmlArrayItem("ProcessSpecification", typeof(XmlProcessSpecification))]
        public List<XmlAbstractSpecification> Specifications { get; set; }

        /// <summary>
        /// Vendor-extension child elements captured verbatim from the
        /// <c>&lt;Configuration&gt;</c> element by <see cref="XmlAnyElementAttribute"/>
        /// — every child not bound to a strongly-typed slot above lands here.
        /// See <see cref="IConfiguration.VendorExtensions"/> for the standard
        /// citation.
        /// </summary>
        [XmlAnyElement]
        public XmlElement[] VendorExtensions { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="Configuration"/>, projecting each nested collection into
        /// its model representation.
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
            if (!Relationships.IsNullOrEmpty())
            {
                var relationships = new List<IConfigurationRelationship>();
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
                foreach (var specification in Specifications)
                {
                    specifications.Add(specification.ToSpecification());
                }
                configuration.Specifications = specifications;
            }

            // Vendor Extensions — every unrecognised child element captured by
            // [XmlAnyElement] projects to an XElement on the model. Elements are
            // preserved verbatim so downstream consumers see the exact
            // vendor-namespaced XML the operator authored.
            if (VendorExtensions != null && VendorExtensions.Length > 0)
            {
                var extensions = new List<XElement>(VendorExtensions.Length);
                foreach (var element in VendorExtensions)
                {
                    if (element != null)
                    {
                        extensions.Add(XElement.Parse(element.OuterXml, LoadOptions.PreserveWhitespace));
                    }
                }

                if (extensions.Count > 0)
                {
                    configuration.VendorExtensions = extensions;
                }
            }

            return configuration;
        }

        /// <summary>
        /// Writes the given <see cref="IConfiguration"/> to
        /// <paramref name="writer"/> as a <c>Configuration</c> element,
        /// dispatching each relationship to its concrete writer and optionally
        /// preceding the element with an explanatory comment.
        /// </summary>
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
                        if (typeof(IAssetRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            XmlAssetRelationship.WriteXml(writer, (IAssetRelationship)relationship);
                        }
                        else if (typeof(IComponentRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            XmlComponentRelationship.WriteXml(writer, (IComponentRelationship)relationship);
                        }
                        else if (typeof(IDeviceRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            XmlDeviceRelationship.WriteXml(writer, (IDeviceRelationship)relationship);
                        }
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

                // Write Vendor Extensions — each XElement is a fully-formed,
                // vendor-namespaced element that a vendor XSD declares as a
                // substitution of the standard AbstractConfiguration abstract
                // element. Serialise verbatim via WriteRaw so namespace
                // declarations and mixed content are preserved as authored.
                if (configuration.VendorExtensions != null)
                {
                    foreach (var extension in configuration.VendorExtensions)
                    {
                        if (extension == null) continue;
                        writer.WriteRaw(extension.ToString(SaveOptions.DisableFormatting));
                    }
                }

                writer.WriteEndElement();
            }
        }
    }
}