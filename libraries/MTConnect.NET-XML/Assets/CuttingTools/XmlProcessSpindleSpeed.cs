// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for the <c>ProcessSpindleSpeed</c> of a
    /// CuttingTool life cycle, carrying the spindle speed the tool is intended
    /// to run at with optional min/max/nominal bounds.
    /// </summary>
    public class XmlProcessSpindleSpeed
    {
        /// <summary>
        /// The maximum recommended spindle speed, carried by the
        /// <c>maximum</c> attribute.
        /// </summary>
        [XmlAttribute("maximum")]
        public string Maximum { get; set; }

        /// <summary>
        /// The minimum recommended spindle speed, carried by the
        /// <c>minimum</c> attribute.
        /// </summary>
        [XmlAttribute("minimum")]
        public string Minimum { get; set; }

        /// <summary>
        /// The nominal (target) spindle speed, carried by the <c>nominal</c>
        /// attribute.
        /// </summary>
        [XmlAttribute("nominal")]
        public string Nominal { get; set; }

        /// <summary>
        /// The spindle speed value, carried as the element's text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ProcessSpindleSpeed"/>, parsing each populated attribute
        /// and the value to a number.
        /// </summary>
        public IProcessSpindleSpeed ToProcessSpindleSpeed()
        {
            var processSpindleSpeed = new ProcessSpindleSpeed();
            if (!string.IsNullOrEmpty(Maximum)) processSpindleSpeed.Maximum = Maximum.ToDouble();
            if (!string.IsNullOrEmpty(Minimum)) processSpindleSpeed.Minimum = Minimum.ToDouble();
            if (!string.IsNullOrEmpty(Nominal)) processSpindleSpeed.Nominal = Nominal.ToDouble();
            if (!string.IsNullOrEmpty(Value)) processSpindleSpeed.Value = Value.ToDouble();
            return processSpindleSpeed;
        }

        /// <summary>
        /// Writes the <c>ProcessSpindleSpeed</c> element, emitting the
        /// min/max/nominal attributes and value only when populated.
        /// </summary>
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