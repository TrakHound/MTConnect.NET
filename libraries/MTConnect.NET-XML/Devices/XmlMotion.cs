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
    /// <summary>
    /// XML serialization surrogate for the MTConnect <c>Motion</c> element that
    /// defines the kinematic relationship of a component to its parent. Mirrors
    /// the on-the-wire element and converts to and from the strongly-typed
    /// <see cref="Motion"/> model.
    /// </summary>
    [XmlRoot("Motion")]
    public class XmlMotion
    {
        /// <summary>
        /// The unique identifier of the motion within the device.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The <c>id</c> of the parent motion this motion is defined relative
        /// to.
        /// </summary>
        [XmlAttribute("parentIdRef")]
        public string ParentIdRef { get; set; }

        /// <summary>
        /// The <c>id</c> of the coordinate system the motion is defined in.
        /// </summary>
        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The type of motion, such as <c>REVOLUTE</c> or <c>PRISMATIC</c>.
        /// </summary>
        [XmlAttribute("type")]
        public MotionType Type { get; set; }

        /// <summary>
        /// How the motion is actuated, such as <c>DIRECT</c> or <c>VIRTUAL</c>.
        /// </summary>
        [XmlAttribute("actuation")]
        public MotionActuationType Actuation { get; set; }

        /// <summary>
        /// The free-form description of the motion.
        /// </summary>
        [XmlElement("Description")]
        public XmlDescription Description { get; set; }

        /// <summary>
        /// The origin of the motion expressed as a coordinate triple; mutually
        /// exclusive with <see cref="OriginDataSet"/>.
        /// </summary>
        [XmlElement("Origin")]
        public XmlOrigin Origin { get; set; }

        /// <summary>
        /// The origin of the motion expressed as a data set; mutually exclusive
        /// with <see cref="Origin"/>.
        /// </summary>
        [XmlElement("OriginDataSet")]
        public XmlOriginDataSet OriginDataSet { get; set; }

        /// <summary>
        /// The translation and rotation applied relative to the parent motion.
        /// </summary>
        [XmlElement("Transformation")]
        public XmlTransformation Transformation { get; set; }

        /// <summary>
        /// The axis of motion expressed as a coordinate triple; mutually
        /// exclusive with <see cref="AxisDataSet"/>.
        /// </summary>
        [XmlElement("Axis")]
        public XmlAxis Axis { get; set; }

        /// <summary>
        /// The axis of motion expressed as a data set; mutually exclusive with
        /// <see cref="Axis"/>.
        /// </summary>
        [XmlElement("AxisDataSet")]
        public XmlAxisDataSet AxisDataSet { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed <see cref="Motion"/>,
        /// resolving the Origin and Axis choices in favour of the data-set form
        /// when present.
        /// </summary>
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

        /// <summary>
        /// Writes the given <see cref="IMotion"/> to <paramref name="writer"/>
        /// as a <c>Motion</c> element, emitting the data-set form of Origin and
        /// Axis when the model carries it.
        /// </summary>
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
