// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <CoordinateSystem> of type CoordinateSystemType. Origin / Transformation /
//   OriginDataSet form a single xs:choice (only one is permitted).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class CoordinateSystem (UML ID `_19_0_3_45f01b9_1579100679936_1279_16310`).

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>CoordinateSystem</c>,
    /// the spatial reference frame a component's geometry is expressed in.
    /// Mirrors the on-the-wire element and converts to and from the
    /// strongly-typed <see cref="CoordinateSystem"/> model.
    /// </summary>
    [XmlRoot("CoordinateSystem")]
    public class XmlCoordinateSystem
    {
        /// <summary>
        /// The unique identifier of the coordinate system within the device.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The optional human-readable name of the coordinate system.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The name the coordinate system is known by on its native control.
        /// </summary>
        [XmlAttribute("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The <c>id</c> of the parent coordinate system this one is defined
        /// relative to.
        /// </summary>
        [XmlAttribute("parentIdRef")]
        public string ParentIdRef { get; set; }

        /// <summary>
        /// The kind of coordinate system, such as <c>MACHINE</c> or
        /// <c>WORK</c>.
        /// </summary>
        [XmlAttribute("type")]
        public CoordinateSystemType Type { get; set; }

        /// <summary>
        /// The origin expressed as a coordinate triple; mutually exclusive with
        /// <see cref="OriginDataSet"/> and <see cref="Transformation"/>.
        /// </summary>
        [XmlElement("Origin")]
        public XmlOrigin Origin { get; set; }

        /// <summary>
        /// The origin expressed as a data set; mutually exclusive with
        /// <see cref="Origin"/> and <see cref="Transformation"/>.
        /// </summary>
        [XmlElement("OriginDataSet")]
        public XmlOriginDataSet OriginDataSet { get; set; }

        /// <summary>
        /// The translation and rotation from the parent coordinate system;
        /// mutually exclusive with <see cref="Origin"/> and
        /// <see cref="OriginDataSet"/>.
        /// </summary>
        [XmlElement("Transformation")]
        public XmlTransformation Transformation { get; set; }

        /// <summary>
        /// The free-form description of the coordinate system.
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="CoordinateSystem"/>, resolving the origin choice in
        /// favour of the data-set form when present.
        /// </summary>
        public ICoordinateSystem ToCoordinateSystem()
        {
            var coordinateSystem = new CoordinateSystem();
            coordinateSystem.Id = Id;
            coordinateSystem.Name = Name;
            coordinateSystem.NativeName = NativeName;
            coordinateSystem.ParentIdRef = ParentIdRef;
            coordinateSystem.Type = Type;
            if (OriginDataSet != null) coordinateSystem.Origin = OriginDataSet.ToOriginDataSet();
            else if (Origin != null) coordinateSystem.Origin = Origin.ToOrigin();
            if (Transformation != null) coordinateSystem.Transformation = Transformation.ToTransformation();
            coordinateSystem.Description = Description;
            return coordinateSystem;
        }

        /// <summary>
        /// Writes the given <see cref="ICoordinateSystem"/> to
        /// <paramref name="writer"/> as a <c>CoordinateSystem</c> element,
        /// emitting the data-set form of the origin when the model carries it.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ICoordinateSystem coordinateSystem)
        {
            if (coordinateSystem != null)
            {
                writer.WriteStartElement("CoordinateSystem");

                // Write Properties
                writer.WriteAttributeString("id", coordinateSystem.Id);
                writer.WriteAttributeString("type", coordinateSystem.Type.ToString());
                if (!string.IsNullOrEmpty(coordinateSystem.Name)) writer.WriteAttributeString("name", coordinateSystem.Name);
                if (!string.IsNullOrEmpty(coordinateSystem.NativeName)) writer.WriteAttributeString("nativeName", coordinateSystem.NativeName);
                if (!string.IsNullOrEmpty(coordinateSystem.ParentIdRef)) writer.WriteAttributeString("parentIdRefd", coordinateSystem.ParentIdRef);

                // Write Description
                if (!string.IsNullOrEmpty(coordinateSystem.Description))
                {
                    writer.WriteStartElement("Description");
                    writer.WriteString(coordinateSystem.Description);
                    writer.WriteEndElement();
                }

                // Write Origin (or OriginDataSet)
                if (coordinateSystem.Origin is IOriginDataSet originDataSet)
                {
                    XmlOriginDataSet.WriteXml(writer, originDataSet);
                }
                else if (coordinateSystem.Origin is IOrigin origin)
                {
                    XmlOrigin.WriteXml(writer, origin);
                }

                // Write Transformation
                if (coordinateSystem.Transformation != null)
                {
                    XmlTransformation.WriteXml(writer, coordinateSystem.Transformation);
                }

                writer.WriteEndElement();
            }
        }
    }
}
