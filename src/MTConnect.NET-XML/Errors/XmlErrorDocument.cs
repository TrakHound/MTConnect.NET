// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Errors.Xml
{
    [XmlRoot("MTConnectError")]
    public class XmlErrorResponseDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlErrorResponseDocument));


        [XmlElement("Header")]
        public XmlErrorHeader Header { get; set; }

        [XmlArray("Errors")]
        public List<XmlError> Errors { get; set; }

        [XmlIgnore]
        public Version Version { get; set; }


        public IErrorResponseDocument ToResponseDocument()
        {
            var document = new ErrorResponseDocument();
            document.Header = Header.ToErrorHeader();
            document.Version = Version;

            if (!Errors.IsNullOrEmpty())
            {
                var errors = new List<IError>();
                foreach (var error in Errors)
                {
                    errors.Add(error.ToError());
                }
                document.Errors = errors;
            }

            return document;
        }


        public static IErrorResponseDocument FromXml(byte[] xmlBytes)
        {
            if (xmlBytes != null && xmlBytes.Length > 0)
            {
                try
                {
                    // Clean whitespace and Encoding Marks (BOM)
                    var bytes = XmlFunctions.SanitizeBytes(xmlBytes);

                    var xml = Encoding.UTF8.GetString(bytes);
                    xml = xml.Trim();

                    var version = MTConnectVersion.Get(xml);

                    using (var textReader = new StringReader(Namespaces.Clear(xml)))
                    {
                        using (var xmlReader = XmlReader.Create(textReader))
                        {
                            var doc = (XmlErrorResponseDocument)_serializer.Deserialize(xmlReader);
                            if (doc != null)
                            {
                                doc.Version = version;
                                return doc.ToResponseDocument();
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }


        public static byte[] ToXmlBytes(
            IErrorResponseDocument document,
            bool indentOutput = true,
            bool outputComments = false,
            string stylesheet = null
            )
        {
            if (document != null && document.Header != null)
            {
                try
                {
                    var mtconnectStreamsNamespace = Namespaces.GetStreams(document.Version.Major, document.Version.Minor);

                    using (var stream = new MemoryStream())
                    {
                        // Set the XmlWriterSettings to use
                        var xmlWriterSettings = indentOutput ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                        // Use XmlWriter to write XML to stream
                        using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                        {
                            WriteXml(xmlWriter, document, indentOutput, outputComments, stylesheet);
                            return stream.ToArray();
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static void WriteXml(
            XmlWriter writer,
            IErrorResponseDocument document,
            bool indentOutput = true,
            bool outputComments = false,
            string stylesheet = null
            )
        {
            if (document != null && !document.Errors.IsNullOrEmpty())
            {
                var ns = Namespaces.GetError(document.Version.Major, document.Version.Minor);

                writer.WriteStartDocument();
                if (indentOutput) writer.WriteWhitespace(XmlFunctions.NewLine);

                // Add Stylesheet
                if (!string.IsNullOrEmpty(stylesheet))
                {
                    writer.WriteRaw($"<?xml-stylesheet type=\"text/xsl\" href=\"{stylesheet}?version={document.Version}\"?>");
                    if (indentOutput) writer.WriteWhitespace(XmlFunctions.NewLine);
                }

                // Add Header Comment
                if (outputComments) XmlFunctions.WriteHeaderComment(writer, indentOutput);

                // Write Root Document Element
                writer.WriteStartElement("MTConnectError", ns);

                // Write Namespace Declarations
                writer.WriteAttributeString("xmlns", null, null, ns);
                writer.WriteAttributeString("xmlns", "m", null, ns);
                writer.WriteAttributeString("xmlns", "xsi", null, Namespaces.DefaultXmlSchemaInstance);

                // Write Schema Location
                writer.WriteAttributeString("xsi", "schemaLocation", null, Schemas.GetStreams(document.Version.Major, document.Version.Minor));

                // Write Header
                XmlErrorHeader.WriteXml(writer, document.Header);

                writer.WriteStartElement("Errors");

                // Write Errors
                foreach (var error in document.Errors)
                {
                    writer.WriteStartElement("Error");
                    writer.WriteAttributeString("errorCode", error.ErrorCode.ToString());
                    writer.WriteString(error.Value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // Errors
                writer.WriteEndElement(); // MTConnectError
                writer.WriteEndDocument();
                writer.Flush();
            }
        }
    }
}
