// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <Motion> of type MotionType. Origin/OriginDataSet form an xs:choice
//   (only one is permitted), and Axis/AxisDataSet form a separate xs:choice.
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Motion (UML ID `EAID_1F084FBF_2AC7_41f6_8485_C356E6D7A9C1`).

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
        public XmlOrigin Origin { get; set; }

        [XmlElement("OriginDataSet")]
        public XmlOriginDataSet OriginDataSet { get; set; }

        [XmlElement("Transformation")]
        public XmlTransformation Transformation { get; set; }

        [XmlElement("Axis")]
        public XmlAxis Axis { get; set; }

        [XmlElement("AxisDataSet")]
        public XmlAxisDataSet AxisDataSet { get; set; }


        public IMotion ToMotion()
        {
            var motion = new Motion();
            motion.Id = Id;
            motion.ParentIdRef = ParentIdRef;
            motion.CoordinateSystemIdRef = CoordinateSystemIdRef;
            motion.Type = Type;
            motion.Actuation = Actuation;
            if (AxisDataSet != null) motion.Axis = AxisDataSet.ToAxisDataSet();
            else if (Axis != null) motion.Axis = Axis.ToAxis();
            if (OriginDataSet != null) motion.Origin = OriginDataSet.ToOriginDataSet();
            else if (Origin != null) motion.Origin = Origin.ToOrigin();
            if (Transformation != null) motion.Transformation = Transformation.ToTransformation();
            if (Description != null) motion.Description = Description.CDATA;
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

                // Write Origin (or OriginDataSet)
                if (motion.Origin is IOriginDataSet originDataSet)
                {
                    XmlOriginDataSet.WriteXml(writer, originDataSet);
                }
                else if (motion.Origin is IOrigin origin)
                {
                    XmlOrigin.WriteXml(writer, origin);
                }

                // Write Transformation
                XmlTransformation.WriteXml(writer, motion.Transformation);

                // Write Axis (or AxisDataSet)
                if (motion.Axis is IAxisDataSet axisDataSet)
                {
                    XmlAxisDataSet.WriteXml(writer, axisDataSet);
                }
                else if (motion.Axis is IAxis axis)
                {
                    XmlAxis.WriteXml(writer, axis);
                }

                writer.WriteEndElement();
            }
        }
    }
}
