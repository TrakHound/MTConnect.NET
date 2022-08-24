// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Motion;
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
        public string Description { get; set; }

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
            motion.Axis = Axis;
            motion.Origin = Origin;
            if (Transformation != null) motion.Transformation = Transformation.ToTransformation();
            motion.Description = Description;
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
                if (!string.IsNullOrEmpty(motion.Description))
                {
                    writer.WriteStartElement("Description");
                    writer.WriteString(motion.Description);
                    writer.WriteEndElement();
                }

                // Write Origin
                if (motion.Origin != null)
                {
                    writer.WriteStartElement("Origin");
                    writer.WriteString(motion.Origin);
                    writer.WriteEndElement();
                }

                // Write Transformation
                XmlTransformation.WriteXml(writer, motion.Transformation);

                // Write Axis
                if (motion.Axis != null)
                {
                    writer.WriteStartElement("Axis");
                    writer.WriteString(motion.Axis);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}
