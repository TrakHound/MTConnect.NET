// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Motion")]
    public class XmlMotion
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("parentIdRef")]
        public string ParentIdRef { get; set; }

        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        [XmlAttribute("type")]
        public MotionType Type { get; set; }

        [XmlAttribute("actuation")]
        public MotionActuationType Actuation { get; set; }

        [XmlElement("Description")]
        public XmlDescription Description { get; set; }

        [XmlElement("Origin")]
        public string Origin { get; set; }

        [XmlElement("Transformation")]
        public XmlTransformation Transformation { get; set; }

        [XmlElement("Axis")]
        public string Axis { get; set; }


        public IMotion ToMotion()
        {
            var motion = new Motion();
            motion.Id = Id;
            motion.ParentIdRef = ParentIdRef;
            motion.CoordinateSystemIdRef = CoordinateSystemIdRef;
            motion.Type = Type;
            motion.Actuation = Actuation;
            motion.Axis = UnitVector3D.FromString(Axis);
            motion.Origin = UnitVector3D.FromString(Origin);
            if (Transformation != null) motion.Transformation = Transformation.ToTransformation();
            if (Description != null) motion.Description = Description.ToDescription();
            return motion;
        }

        public static void WriteXml(XmlWriter writer, IMotion motion)
        {
            if (motion != null)
            {
                writer.WriteStartElement("Motion");

                // Write Properties
                writer.WriteAttributeString("id", motion.Id);
                if (!string.IsNullOrEmpty(motion.ParentIdRef)) writer.WriteAttributeString("parentIdRef", motion.ParentIdRef);
                if (!string.IsNullOrEmpty(motion.CoordinateSystemIdRef)) writer.WriteAttributeString("coordinateSystemIdRef", motion.CoordinateSystemIdRef);
                writer.WriteAttributeString("type", motion.Type.ToString());
                writer.WriteAttributeString("actuation", motion.Actuation.ToString());

                // Write Description
                if (motion.Description != null)
                {
                    XmlDescription.WriteXml(writer, motion.Description);
                }

                // Write Origin
                if (motion.Origin != null)
                {
                    writer.WriteStartElement("Origin");
                    writer.WriteString(motion.Origin.ToString());
                    writer.WriteEndElement();
                }

                // Write Transformation
                XmlTransformation.WriteXml(writer, motion.Transformation);

                // Write Axis
                if (motion.Axis != null)
                {
                    writer.WriteStartElement("Axis");
                    writer.WriteString(motion.Axis.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}