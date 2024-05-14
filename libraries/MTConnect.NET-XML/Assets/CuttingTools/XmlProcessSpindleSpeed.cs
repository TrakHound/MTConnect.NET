// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlProcessSpindleSpeed
    {
        [XmlAttribute("maximum")]
        public string Maximum { get; set; }

        [XmlAttribute("minimum")]
        public string Minimum { get; set; }

        [XmlAttribute("nominal")]
        public string Nominal { get; set; }

        [XmlText]
        public string Value { get; set; }


        public IProcessSpindleSpeed ToProcessSpindleSpeed()
        {
            var processSpindleSpeed = new ProcessSpindleSpeed();
            if (!string.IsNullOrEmpty(Maximum)) processSpindleSpeed.Maximum = Maximum.ToDouble();
            if (!string.IsNullOrEmpty(Minimum)) processSpindleSpeed.Minimum = Minimum.ToDouble();
            if (!string.IsNullOrEmpty(Nominal)) processSpindleSpeed.Nominal = Nominal.ToDouble();
            if (!string.IsNullOrEmpty(Value)) processSpindleSpeed.Value = Value.ToDouble();
            return processSpindleSpeed;
        }

        public static void WriteXml(XmlWriter writer, IProcessSpindleSpeed processSpindleSpeed)
        {
            if (processSpindleSpeed != null)
            {
                writer.WriteStartElement("ProcessSpindleSpeed");

                if (processSpindleSpeed.Maximum != null) writer.WriteAttributeString("maximum", processSpindleSpeed.Maximum.ToString());
                if (processSpindleSpeed.Minimum != null) writer.WriteAttributeString("minimum", processSpindleSpeed.Minimum.ToString());
                if (processSpindleSpeed.Nominal != null) writer.WriteAttributeString("nominal", processSpindleSpeed.Nominal.ToString());
                if (processSpindleSpeed.Value != null) writer.WriteString(processSpindleSpeed.Value.ToString());

                writer.WriteEndElement();
            }
        }
    }
}