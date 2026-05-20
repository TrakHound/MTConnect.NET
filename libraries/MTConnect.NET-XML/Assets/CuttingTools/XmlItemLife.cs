// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for a cutting item's <c>ItemLife</c> counter.
    /// Mirrors the on-the-wire element, where the accumulated value is the
    /// element text and its metadata are attributes, and converts to the
    /// strongly-typed <see cref="ItemLife"/> model.
    /// </summary>
    public class XmlItemLife
    {
        /// <summary>
        /// The current accumulated life value, carried as the element text.
        /// </summary>
        [XmlText]
        public double Value { get; set; }

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
        /// The value at which the item is considered expired, as the raw
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
        /// Converts this surrogate into the strongly-typed
        /// <see cref="ItemLife"/>, parsing the type, count direction, and
        /// numeric metadata.
        /// </summary>
        public IItemLife ToItemLife()
        {
            var itemLife = new ItemLife();
            itemLife.Value = Value;
            itemLife.Type = Type.ConvertEnum<ToolLifeType>();
            itemLife.CountDirection = CountDirection.ConvertEnum<CountDirectionType>();
            if (Warning != null) itemLife.Warning = Warning.ToDouble();
            if (Limit != null) itemLife.Limit = Limit.ToDouble();
            if (Initial != null) itemLife.Initial = Initial.ToDouble();
            return itemLife;
        }

        /// <summary>
        /// Writes the given item-life counters to <paramref name="writer"/>,
        /// one <c>ItemLife</c> element per counter, omitting the warning,
        /// limit, and initial attributes that are not set.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IEnumerable<IItemLife> itemLifes)
        {
            if (itemLifes != null)
            {
                foreach (var itemLife in itemLifes)
                {
                    writer.WriteStartElement("ItemLife");
                    writer.WriteAttributeString("type", itemLife.Type.ToString());
                    writer.WriteAttributeString("countDirection", itemLife.CountDirection.ToString());
                    if (itemLife.Warning != null) writer.WriteAttributeString("warning", itemLife.Warning.ToString());
                    if (itemLife.Limit != null) writer.WriteAttributeString("limit", itemLife.Limit.ToString());
                    if (itemLife.Initial != null) writer.WriteAttributeString("initial", itemLife.Initial.ToString());
                    writer.WriteString(itemLife.Value.ToString());
                    writer.WriteEndElement();
                }
            }
        }
    }
}