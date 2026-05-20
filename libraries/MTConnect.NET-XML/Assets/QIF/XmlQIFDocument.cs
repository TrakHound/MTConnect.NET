// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Assets.QIF
{
    /// <summary>
    /// XML serialization surrogate that captures an embedded QIF (Quality
    /// Information Framework) document verbatim. Custom
    /// <see cref="IXmlSerializable"/> handling is used so the foreign QIF XML
    /// is preserved as raw markup rather than mapped to a typed model.
    /// </summary>
    public class XmlQIFDocument : IXmlSerializable
    {
        /// <summary>
        /// The embedded QIF document as a raw XML fragment.
        /// </summary>
        public string QIFDocument { get; set; }


        #region "Xml Serialization"

        /// <summary>
        /// No-op; QIF content is written by the owning asset surrogate via its
        /// static <c>WriteXml</c>.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            // Use static WriteXml()
        }

        /// <summary>
        /// Reads the QIF document by capturing the element's outer XML
        /// verbatim, then advancing past it.
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            QIFDocument = reader.ReadOuterXml();

            // Advance Reader
            reader.Skip();
        }

        /// <summary>
        /// Returns <c>null</c>; no inline schema is provided for the foreign
        /// QIF content.
        /// </summary>
        public XmlSchema GetSchema()
        {
            return (null);
        }

        #endregion

    }
}
