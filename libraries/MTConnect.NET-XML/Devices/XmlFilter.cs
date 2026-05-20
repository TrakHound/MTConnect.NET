// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a DataItem <c>Filter</c>, which
    /// constrains when a data item reports observations (for example a
    /// <c>MINIMUM_DELTA</c> or <c>PERIOD</c> filter).
    /// </summary>
    public class XmlFilter
    {
        /// <summary>
        /// The filter kind, carried by the <c>type</c> attribute (for example
        /// <c>MINIMUM_DELTA</c> or <c>PERIOD</c>).
        /// </summary>
        [XmlAttribute("type")]
        public DataItemFilterType Type { get; set; }

        /// <summary>
        /// The filter threshold value, carried as the element's text content.
        /// </summary>
        [XmlText]
        public double Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Filter"/>,
        /// copying the filter type and threshold.
        /// </summary>
        public IFilter ToFilter()
        {
            var filter = new Filter();
            filter.Type = Type;
            filter.Value = Value;
            return filter;
        }

        /// <summary>
        /// Writes the <c>Filter</c> element for the supplied model, emitting the
        /// <c>type</c> attribute and the threshold value as element content.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IFilter filter)
        {
            if (filter != null)
            {
                writer.WriteStartElement("Filter");
                writer.WriteAttributeString("type", filter.Type.ToString());
                writer.WriteValue(filter.Value);
                writer.WriteEndElement();
            }
        }
    }
}