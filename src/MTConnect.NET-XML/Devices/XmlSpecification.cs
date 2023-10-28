// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Specification")]
    public class XmlSpecification : XmlAbstractSpecification
    {
        [XmlElement("Maximum")]
        public double? Maximum { get; set; }

        [XmlElement("UpperLimit")]
        public double? UpperLimit { get; set; }

        [XmlElement("UpperWarning")]
        public double? UpperWarning { get; set; }

        [XmlElement("Nominal")]
        public double? Nominal { get; set; }

        [XmlElement("LowerLimit")]
        public double? LowerLimit { get; set; }

        [XmlElement("LowerWarning")]
        public double? LowerWarning { get; set; }

        [XmlElement("Minimum")]
         public double? Minimum { get; set; }


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
            specification.Originator = Originator;
            specification.Maximum = Maximum;
            specification.UpperLimit = UpperLimit;
            specification.UpperWarning = UpperWarning;
            specification.Nominal = Nominal;
            specification.LowerLimit = LowerLimit;
            specification.LowerWarning = LowerWarning;
            specification.Minimum = Minimum;
            return specification;
        }

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
                if (specification.Originator != Originator.MANUFACTURER) writer.WriteAttributeString("originator", specification.Originator.ToString());
            }
        }

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