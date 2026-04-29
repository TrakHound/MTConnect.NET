// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Tests.XML.TestHelpers
{
    /// <summary>
    /// Shared round-trip primitives for the Configuration polymorphic XML
    /// fixtures. Each leaf class exposes a static <c>WriteXml(XmlWriter, ...)</c>
    /// for serialization and a parameterless ctor + <c>To&lt;Leaf&gt;()</c>
    /// instance method for deserialization (driven by XmlSerializer).
    /// </summary>
    internal static class XmlRoundTripHelper
    {
        /// <summary>
        /// Invokes a producer that writes one root element via XmlWriter and
        /// returns the produced XML as a string.
        /// </summary>
        public static string Write(Action<XmlWriter> writer)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = false
            };
            using (var xw = XmlWriter.Create(sb, settings))
            {
                writer(xw);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Deserialises an XML fragment back into a wire-format class using
        /// XmlSerializer. The fragment must have the wire-format type's
        /// declared XmlRoot element name.
        /// </summary>
        public static T Read<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var sr = new StringReader(xml);
            using var reader = XmlReader.Create(sr, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            });
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
