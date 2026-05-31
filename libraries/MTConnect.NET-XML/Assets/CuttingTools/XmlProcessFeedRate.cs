// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for the <c>ProcessFeedRate</c> of a
    /// CuttingTool life cycle, carrying the feed rate the tool is intended to
    /// run at with optional min/max/nominal bounds.
    /// </summary>
    public class XmlProcessFeedRate
    {
        /// <summary>
        /// The maximum recommended feed rate, carried by the <c>maximum</c>
        /// attribute.
        /// </summary>
        [XmlAttribute("maximum")]
        public string Maximum { get; set; }

        /// <summary>
        /// The minimum recommended feed rate, carried by the <c>minimum</c>
        /// attribute.
        /// </summary>
        [XmlAttribute("minimum")]
        public string Minimum { get; set; }

        /// <summary>
        /// The nominal (target) feed rate, carried by the <c>nominal</c>
        /// attribute.
        /// </summary>
        [XmlAttribute("nominal")]
        public string Nominal { get; set; }

        /// <summary>
        /// The feed rate value, carried as the element's text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ProcessFeedRate"/>, parsing each populated attribute and
        /// the value to a number.
        /// </summary>
        public IProcessFeedRate ToProcessFeedRate()
        {
            var processFeedRate = new ProcessFeedRate();
            if (!string.IsNullOrEmpty(Maximum)) processFeedRate.Maximum = Maximum.ToDouble();
            if (!string.IsNullOrEmpty(Minimum)) processFeedRate.Minimum = Minimum.ToDouble();
            if (!string.IsNullOrEmpty(Nominal)) processFeedRate.Nominal = Nominal.ToDouble();
            if (!string.IsNullOrEmpty(Value)) processFeedRate.Value = Value.ToDouble();
            return processFeedRate;
        }

        /// <summary>
        /// Writes the <c>ProcessFeedRate</c> element, emitting the
        /// min/max/nominal attributes and value only when populated.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IProcessFeedRate processFeedRate)
        {
            if (processFeedRate != null)
            {
                writer.WriteStartElement("ProcessFeedRate");

                if (processFeedRate.Maximum != null) writer.WriteAttributeString("maximum", processFeedRate.Maximum.ToString());
                if (processFeedRate.Minimum != null) writer.WriteAttributeString("minimum", processFeedRate.Minimum.ToString());
                if (processFeedRate.Nominal != null) writer.WriteAttributeString("nominal", processFeedRate.Nominal.ToString());
                if (processFeedRate.Value != null) writer.WriteString(processFeedRate.Value.ToString());

                writer.WriteEndElement();
            }
        }
    }
}