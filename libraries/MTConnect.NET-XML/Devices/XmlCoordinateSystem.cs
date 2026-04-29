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
    [XmlRoot("CoordinateSystem")]
    public class XmlCoordinateSystem
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("nativeName")]
        public string NativeName { get; set; }

        [XmlAttribute("parentIdRef")]
        public string ParentIdRef { get; set; }

        [XmlAttribute("type")]
        public CoordinateSystemType Type { get; set; }

        [XmlElement("Origin")]
        public XmlOrigin Origin { get; set; }

        [XmlElement("OriginDataSet")]
        public XmlOriginDataSet OriginDataSet { get; set; }

        [XmlElement("Transformation")]
        public XmlTransformation Transformation { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }


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
