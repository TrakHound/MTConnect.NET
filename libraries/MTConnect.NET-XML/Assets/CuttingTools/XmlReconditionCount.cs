// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for the <c>ReconditionCount</c> of a
    /// CuttingTool life cycle, recording how many times the tool has been
    /// reconditioned and the maximum number of reconditioning cycles allowed.
    /// </summary>
    public class XmlReconditionCount
    {
        /// <summary>
        /// The maximum number of reconditioning cycles permitted, carried by
        /// the <c>maximumCount</c> attribute.
        /// </summary>
        [XmlAttribute("maximumCount")]
        public string MaximumCount { get; set; }

        /// <summary>
        /// The number of times the tool has been reconditioned, carried as the
        /// element's text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ReconditionCount"/>, parsing the maximum count (when
        /// present) and the value to integers.
        /// </summary>
        public IReconditionCount ToReconditionCount()
        {
            var reconditionCount = new ReconditionCount();
            if (MaximumCount != null) reconditionCount.MaximumCount = MaximumCount.ToInt();
            reconditionCount.Value = Value.ToInt();
            return reconditionCount;
        }

        /// <summary>
        /// Writes the <c>ReconditionCount</c> element, emitting the
        /// <c>maximumCount</c> attribute only when present and the count as
        /// element content.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IReconditionCount reconditionCount)
        {
            if (reconditionCount != null)
            {
                writer.WriteStartElement("ReconditionCount");
                if (reconditionCount.MaximumCount != null) writer.WriteAttributeString("maximumCount", reconditionCount.MaximumCount.ToString());
                writer.WriteString(reconditionCount.Value.ToString());
                writer.WriteEndElement();
            }
        }
    }
}