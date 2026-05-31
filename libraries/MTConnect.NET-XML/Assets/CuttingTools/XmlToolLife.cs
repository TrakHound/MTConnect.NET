// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for a cutting tool's <c>ToolLife</c> counter.
    /// Mirrors the on-the-wire element, where the accumulated value is the
    /// element text and its metadata are attributes, and converts to the
    /// strongly-typed <see cref="ToolLife"/> model.
    /// </summary>
    public class XmlToolLife
    {
        /// <summary>
        /// The kind of life counter, such as <c>MINUTES</c> or <c>PART_COUNT</c>,
        /// as the raw attribute text.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// Whether the counter counts <c>UP</c> or <c>DOWN</c>, as the raw
        /// attribute text.
        /// </summary>
        [XmlAttribute("countDirection")]
        public string CountDirection { get; set; }

        /// <summary>
        /// The value at which a warning is raised, as the raw attribute text.
        /// </summary>
        [XmlAttribute("warning")]
        public string Warning { get; set; }

        /// <summary>
        /// The value at which the tool is considered expired, as the raw
        /// attribute text.
        /// </summary>
        [XmlAttribute("limit")]
        public string Limit { get; set; }

        /// <summary>
        /// The value the counter started from, as the raw attribute text.
        /// </summary>
        [XmlAttribute("initial")]
        public string Initial { get; set; }

        /// <summary>
        /// The current accumulated life value, carried as the element text.
        /// </summary>
        [XmlText]
        public double Value { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="ToolLife"/>, parsing the type, count direction, and
        /// numeric metadata.
        /// </summary>
        public IToolLife ToToolLife()
        {
            var toolLife = new ToolLife();
            toolLife.Value = Value;
            toolLife.Type = Type.ConvertEnum<ToolLifeType>();
            toolLife.CountDirection = CountDirection.ConvertEnum<CountDirectionType>();
            if (Warning != null) toolLife.Warning = Warning.ToDouble();
            if (Limit != null) toolLife.Limit = Limit.ToDouble();
            if (Initial != null) toolLife.Initial = Initial.ToDouble();
            return toolLife;
        }

        /// <summary>
        /// Writes the given tool-life counters to <paramref name="writer"/>,
        /// one <c>ToolLife</c> element per counter, omitting the warning,
        /// limit, and initial attributes that are not set.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IEnumerable<IToolLife> toolLifes)
        {
            if (toolLifes != null)
            {
                foreach (var toolLife in toolLifes)
                {
                    writer.WriteStartElement("ToolLife");
                    writer.WriteAttributeString("type", toolLife.Type.ToString());
                    writer.WriteAttributeString("countDirection", toolLife.CountDirection.ToString());
                    if (toolLife.Warning != null) writer.WriteAttributeString("warning", toolLife.Warning.ToString());
                    if (toolLife.Limit != null) writer.WriteAttributeString("limit", toolLife.Limit.ToString());
                    if (toolLife.Initial != null) writer.WriteAttributeString("initial", toolLife.Initial.ToString());
                    writer.WriteString(toolLife.Value.ToString());
                    writer.WriteEndElement();
                }
            }
        }
    }
}