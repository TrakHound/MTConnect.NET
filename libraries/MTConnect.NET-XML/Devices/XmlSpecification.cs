// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>Specification</c>, the
    /// set of limits a measured value is expected to fall within. Mirrors the
    /// on-the-wire element and converts to the strongly-typed
    /// <see cref="Specification"/> model.
    /// </summary>
    [XmlRoot("Specification")]
    public class XmlSpecification : XmlAbstractSpecification
    {
        /// <summary>
        /// The upper conformance boundary; a value at or above this is
        /// non-conforming.
        /// </summary>
        [XmlElement("Maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// The upper boundary of the in-specification range.
        /// </summary>
        [XmlElement("UpperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper boundary indicating a warning condition.
        /// </summary>
        [XmlElement("UpperWarning")]
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The ideal or target value.
        /// </summary>
        [XmlElement("Nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower boundary of the in-specification range.
        /// </summary>
        [XmlElement("LowerLimit")]
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower boundary indicating a warning condition.
        /// </summary>
        [XmlElement("LowerWarning")]
        public double? LowerWarning { get; set; }

        /// <summary>
        /// The lower conformance boundary; a value at or below this is
        /// non-conforming.
        /// </summary>
        [XmlElement("Minimum")]
        public double? Minimum { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="Specification"/>, including the inherited identification
        /// attributes and the numeric limits.
        /// </summary>
        public override ISpecification ToSpecification()
        {
            var specification = new Specification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateSystemIdRef = CoordinateSystemIdRef;
            specification.Originator = Originator.ConvertEnum<Originator>();
            specification.Maximum = Maximum;
            specification.UpperLimit = UpperLimit;
            specification.UpperWarning = UpperWarning;
            specification.Nominal = Nominal;
            specification.LowerLimit = LowerLimit;
            specification.LowerWarning = LowerWarning;
            specification.Minimum = Minimum;
            return specification;
        }

        /// <summary>
        /// Writes the given <see cref="ISpecification"/> to
        /// <paramref name="writer"/>, choosing the element name and the
        /// limit-writing path that match the concrete specification kind.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ISpecification specification)
        {
            if (specification != null)
            {
                writer.WriteStartElement(specification.GetType().Name);

                WriteAbstractXml(writer, specification);

                if (typeof(IProcessSpecification).IsAssignableFrom(specification.GetType()))
                {
                    WriteProcessSpecificationXml(writer, specification as IProcessSpecification);
                }
                else if (typeof(ISpecification).IsAssignableFrom(specification.GetType()))
                {
                    WriteSpecificationXml(writer, specification as ISpecification);
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes the identification attributes shared by every specification
        /// kind, omitting the originator when it is the default
        /// (<c>MANUFACTURER</c>).
        /// </summary>
        public static void WriteAbstractXml(XmlWriter writer, ISpecification specification)
        {
            if (specification != null)
            {
                // Write Properties
                writer.WriteAttributeString("id", specification.Id);
                if (!string.IsNullOrEmpty(specification.Name)) writer.WriteAttributeString("name", specification.Name);
                if (!string.IsNullOrEmpty(specification.Type)) writer.WriteAttributeString("type", specification.Type);
                if (!string.IsNullOrEmpty(specification.SubType)) writer.WriteAttributeString("subType", specification.SubType);
                if (!string.IsNullOrEmpty(specification.DataItemIdRef)) writer.WriteAttributeString("dataItemRef", specification.DataItemIdRef);
                if (!string.IsNullOrEmpty(specification.Units)) writer.WriteAttributeString("units", specification.Units);
                if (!string.IsNullOrEmpty(specification.CompositionIdRef)) writer.WriteAttributeString("compositionIdRef", specification.CompositionIdRef);
                if (!string.IsNullOrEmpty(specification.CoordinateSystemIdRef)) writer.WriteAttributeString("coordinateSystemIdRef", specification.CoordinateSystemIdRef);
                if (specification.Originator != Devices.Configurations.Originator.MANUFACTURER) writer.WriteAttributeString("originator", specification.Originator.ToString());
            }
        }

        /// <summary>
        /// Writes the scalar limit elements of a plain
        /// <see cref="ISpecification"/>, emitting only those that are set.
        /// </summary>
        public static void WriteSpecificationXml(XmlWriter writer, ISpecification specification)
        {
            if (specification != null)
            {
                // Write Maximum
                if (specification.Maximum != null)
                {
                    writer.WriteStartElement("Maximum");
                    writer.WriteString(specification.Maximum.ToString());
                    writer.WriteEndElement();
                }

                // Write Upper Limit
                if (specification.UpperLimit != null)
                {
                    writer.WriteStartElement("UpperLimit");
                    writer.WriteString(specification.UpperLimit.ToString());
                    writer.WriteEndElement();
                }

                // Write Upper Warning
                if (specification.UpperWarning != null)
                {
                    writer.WriteStartElement("UpperWarning");
                    writer.WriteString(specification.UpperWarning.ToString());
                    writer.WriteEndElement();
                }

                // Write Nominal
                if (specification.Nominal != null)
                {
                    writer.WriteStartElement("Nominal");
                    writer.WriteString(specification.Nominal.ToString());
                    writer.WriteEndElement();
                }

                // Write Lower Limit
                if (specification.LowerLimit != null)
                {
                    writer.WriteStartElement("LowerLimit");
                    writer.WriteString(specification.LowerLimit.ToString());
                    writer.WriteEndElement();
                }

                // Write Lower Warning
                if (specification.LowerWarning != null)
                {
                    writer.WriteStartElement("LowerWarning");
                    writer.WriteString(specification.LowerWarning.ToString());
                    writer.WriteEndElement();
                }

                // Write Minimum
                if (specification.Minimum != null)
                {
                    writer.WriteStartElement("Minimum");
                    writer.WriteString(specification.Minimum.ToString());
                    writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Writes the control, specification, and alarm limit groups of an
        /// <see cref="IProcessSpecification"/>, emitting only the groups that
        /// are present.
        /// </summary>
        public static void WriteProcessSpecificationXml(XmlWriter writer, IProcessSpecification specification)
        {
            if (specification != null)
            {
                // Write Control Limits
                if (specification.ControlLimits != null)
                {
                    writer.WriteStartElement("ControlLimits");
                    XmlControlLimits.WriteXml(writer, specification.ControlLimits);
                    writer.WriteEndElement();
                }

                // Write Specification Limits
                if (specification.SpecificationLimits != null)
                {
                    writer.WriteStartElement("SpecificationLimits");
                    XmlSpecificationLimits.WriteXml(writer, specification.SpecificationLimits);
                    writer.WriteEndElement();
                }

                // Write Alarm Limits
                if (specification.AlarmLimits != null)
                {
                    writer.WriteStartElement("AlarmLimits");
                    XmlAlarmLimits.WriteXml(writer, specification.AlarmLimits);
                    writer.WriteEndElement();
                }
            }
        }
    }
}