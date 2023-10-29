// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class XmlProcessFeedRate
    {
        [XmlAttribute("maximum")]
        public string Maximum { get; set; }

        [XmlAttribute("minimum")]
        public string Minimum { get; set; }

        [XmlAttribute("nominal")]
        public string Nominal { get; set; }

        [XmlText]
        public string Value { get; set; }


        public IProcessFeedRate ToProcessFeedRate()
        {
            var processFeedRate = new ProcessFeedRate();
            if (!string.IsNullOrEmpty(Maximum)) processFeedRate.Maximum = Maximum.ToDouble();
            if (!string.IsNullOrEmpty(Minimum)) processFeedRate.Minimum = Minimum.ToDouble();
            if (!string.IsNullOrEmpty(Nominal)) processFeedRate.Nominal = Nominal.ToDouble();
            if (!string.IsNullOrEmpty(Value)) processFeedRate.Value = Value;
            return processFeedRate;
        }

        public static void WriteXml(XmlWriter writer, IProcessFeedRate processFeedRate)
        {
            if (processFeedRate != null)
            {
                writer.WriteStartElement("ProcessFeedRate");

                if (processFeedRate.Maximum != null) writer.WriteAttributeString("maximum", processFeedRate.Maximum.ToString());
                if (processFeedRate.Minimum != null) writer.WriteAttributeString("minimum", processFeedRate.Minimum.ToString());
                if (processFeedRate.Nominal != null) writer.WriteAttributeString("nominal", processFeedRate.Nominal.ToString());
                if (processFeedRate.Value != null) writer.WriteString(processFeedRate.Value);

                writer.WriteEndElement();
            }
        }
    }
}