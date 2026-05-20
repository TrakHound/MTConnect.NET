// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Errors.Xml
{
    /// <summary>
    /// XML serialization surrogate for the root <c>MTConnectError</c> response
    /// document returned when the agent rejects a request. Mirrors the
    /// on-the-wire shape so it can be read and written, then converts to and
    /// from the strongly-typed <see cref="ErrorResponseDocument"/> model.
    /// </summary>
    [XmlRoot("MTConnectError")]
    public class XmlErrorResponseDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlErrorResponseDocument));


        /// <summary>
        /// The document <c>Header</c> describing the agent instance and version.
        /// </summary>
        [XmlElement("Header")]
        public XmlErrorHeader Header { get; set; }

        /// <summary>
        /// The reported errors, serialized as child <c>Error</c> elements of an
        /// <c>Errors</c> container.
        /// </summary>
        [XmlArray("Errors")]
        public List<XmlError> Errors { get; set; }

        /// <summary>
        /// The MTConnect schema version detected from the source XML; carried
        /// out of band so the correct namespace can be emitted on write.
        /// </summary>
        [XmlIgnore]
        public Version Version { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ErrorResponseDocument"/>, converting the header and each
        /// error and propagating the detected version.
        /// </summary>
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


        /// <summary>
        /// Deserializes an <c>MTConnectError</c> document from raw XML bytes,
        /// sanitizing the input, detecting the schema version, and clearing
        /// namespace prefixes before deserialization.
        /// </summary>
        /// <exception cref="XmlException">XML Exception thrown during Serialization</exception>
        public static IErrorResponseDocument FromXml(byte[] xmlBytes)
        {
            if (xmlBytes != null && xmlBytes.Length > 0)
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

            return null;
        }


        /// <summary>
        /// Serializes the error response document to a new XML stream, choosing
        /// indented or compact writer settings and optionally emitting a
        /// stylesheet reference and header comment.
        /// </summary>
        public static Stream ToXmlStream(
            IErrorResponseDocument document,
            bool indentOutput = true,
            bool outputComments = false,
            string stylesheet = null
            )
        {
            if (document != null && document.Header != null)
            {
                var mtconnectStreamsNamespace = Namespaces.GetStreams(document.Version.Major, document.Version.Minor);

                var outputStream = new MemoryStream();

                // Set the XmlWriterSettings to use
                var xmlWriterSettings = indentOutput ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                // Use XmlWriter to write XML to stream
                using (var xmlWriter = XmlWriter.Create(outputStream, xmlWriterSettings))
                {
                    WriteXml(xmlWriter, document, indentOutput, outputComments, stylesheet);
                    return outputStream;
                }
            }

            return null;
        }

        /// <summary>
        /// Writes the complete <c>MTConnectError</c> document, emitting the
        /// version-specific namespace and schema location, the header, and one
        /// <c>Error</c> element per reported error. No document is written when
        /// the response has no errors.
        /// </summary>
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