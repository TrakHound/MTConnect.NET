// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.CoordinateSystems;
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
        public string Origin { get; set; }

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
            coordinateSystem.Origin = Origin;
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

                // Write Origin
                if (!string.IsNullOrEmpty(coordinateSystem.Origin))
                {
                    writer.WriteStartElement("Origin");
                    writer.WriteString(coordinateSystem.Origin);
                    writer.WriteEndElement();
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