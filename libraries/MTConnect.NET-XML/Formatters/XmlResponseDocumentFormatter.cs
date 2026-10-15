// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.Xml;
using MTConnect.Devices;
using MTConnect.Devices.Xml;
using MTConnect.Errors;
using MTConnect.Errors.Xml;
using MTConnect.Streams;
using MTConnect.Streams.Output;
using MTConnect.Streams.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;

namespace MTConnect.Formatters.Xml
{
    /// <summary>
    /// <see cref="IResponseDocumentFormatter"/> that serializes and
    /// deserializes whole MTConnect response documents (Devices, Streams,
    /// Assets, and Error) to and from the MTConnect XML representation, with
    /// optional XSD validation and stylesheet linking.
    /// </summary>
    /// <remarks>
    /// The <c>validationLevel</c> option accepts <c>0</c> / <c>Ignore</c>, <c>1</c> / <c>Warning</c> or
    /// <c>2</c> / <c>Strict</c> (names are case-insensitive). Validation only runs when at least one
    /// <c>schema</c> option supplies an XSD.
    /// </remarks>
    public class XmlResponseDocumentFormatter : IResponseDocumentFormatter
    {
        private const int ValidationIgnore = 0;
        private const int ValidationWarning = 1;
        private const int ValidationStrict = 2;

        private const string ValidationSuccessfulMessage = "XML Validation Successful";
        private const string ValidationSkippedMessage = "XML Validation Skipped : No XSD Schema was supplied";


        /// <summary>
        /// The identifier this formatter is selected by; <c>XML</c>.
        /// </summary>
        public string Id => "XML";

        /// <summary>
        /// The MIME content type the formatter produces;
        /// <c>application/xml</c>.
        /// </summary>
        public string ContentType => "application/xml";


        /// <summary>
        /// Serializes an <see cref="IDevicesResponseDocument"/> to XML,
        /// honouring the indent, comment, stylesheet, and validation-level
        /// options.
        /// </summary>
        public FormatWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var stylesheet = GetFormatterOption(options, "devicesStyle.location");
            var indentOutput = GetBoolOption(options, "indentOutput");
            var outputComments = GetBoolOption(options, "outputComments");

            return Write(() => XmlDevicesResponseDocument.ToXmlStream(document, null, stylesheet, indentOutput, outputComments), options, true);
        }

        /// <summary>
        /// Serializes an <see cref="IStreamsResponseOutputDocument"/> to XML,
        /// honouring the indent, comment, extended-namespace, stylesheet, and
        /// validation-level options.
        /// </summary>
        public FormatWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var extendedNamespaces = GetFormatterOptionsJson<Configurations.NamespaceConfiguration>(options, "namespace");
            var stylesheet = GetFormatterOption(options, "streamsStyle.location");
            var indentOutput = GetBoolOption(options, "indentOutput");
            var outputComments = GetBoolOption(options, "outputComments");

            var streamsDocument = document;
            var result = Write(() => XmlStreamsResponseDocument.ToXmlStream(ref streamsDocument, extendedNamespaces, stylesheet, indentOutput, outputComments), options, true);
            document = streamsDocument;

            return result;
        }

        /// <summary>
        /// Serializes an <see cref="IAssetsResponseDocument"/> to XML,
        /// honouring the indent, comment, stylesheet, and validation-level
        /// options.
        /// </summary>
        public FormatWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var stylesheet = GetFormatterOption(options, "assetsStyle.location");
            var indentOutput = GetBoolOption(options, "indentOutput");
            var outputComments = GetBoolOption(options, "outputComments");

            return Write(() => XmlAssetsResponseDocument.ToXmlStream(document, indentOutput, outputComments, stylesheet), options, true);
        }

        /// <summary>
        /// Serializes an <see cref="IErrorResponseDocument"/> to XML, honouring
        /// the indent, comment, stylesheet, and validation-level options.
        /// </summary>
        /// <remarks>
        /// Unlike the other documents, a validation failure at the <c>Strict</c> level is returned as a
        /// warning: the Error document is the reply sent when a request cannot be answered, so rejecting
        /// it would leave the client with no response at all.
        /// </remarks>
        public FormatWriteResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var stylesheet = GetFormatterOption(options, "errorStyle.location");
            var indentOutput = GetBoolOption(options, "indentOutput");
            var outputComments = GetBoolOption(options, "outputComments");

            return Write(() => XmlErrorResponseDocument.ToXmlStream(document, indentOutput, outputComments, stylesheet), options, false);
        }


        /// <summary>
        /// Deserializes an <see cref="IDevicesResponseDocument"/> from an XML
        /// stream, optionally validating against the supplied XSD schemas and
        /// reporting validation problems as warnings or errors per the
        /// validation level.
        /// </summary>
        /// <remarks>The stream is read from its current position; a seekable stream is returned to that position afterwards.</remarks>
        public FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            return Read(content, options, XmlDevicesResponseDocument.FromXml);
        }

        /// <summary>
        /// Deserializes an <see cref="IStreamsResponseDocument"/> from an XML
        /// stream, optionally validating against the supplied XSD schemas and
        /// reporting validation problems as warnings or errors per the
        /// validation level.
        /// </summary>
        /// <remarks>The stream is read from its current position; a seekable stream is returned to that position afterwards.</remarks>
        public FormatReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            return Read(content, options, bytes =>
            {
                using (var stream = new MemoryStream(bytes, false))
                using (var xmlReader = XmlReader.Create(stream))
                {
                    return XmlStreamsResponseDocument.ReadXml(xmlReader);
                }
            });
        }

        /// <summary>
        /// Deserializes an <see cref="IAssetsResponseDocument"/> from an XML
        /// stream, optionally validating against the supplied XSD schemas and
        /// reporting validation problems as warnings or errors per the
        /// validation level.
        /// </summary>
        /// <remarks>The stream is read from its current position; a seekable stream is returned to that position afterwards.</remarks>
        public FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            return Read<IAssetsResponseDocument>(content, options, XmlAssetsResponseDocument.FromXml);
        }

        /// <summary>
        /// Deserializes an <see cref="IErrorResponseDocument"/> from an XML
        /// stream, optionally validating against the supplied XSD schemas and
        /// reporting validation problems as warnings or errors per the
        /// validation level.
        /// </summary>
        /// <remarks>The stream is read from its current position; a seekable stream is returned to that position afterwards.</remarks>
        public FormatReadResult<IErrorResponseDocument> CreateErrorResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            return Read(content, options, XmlErrorResponseDocument.FromXml);
        }


        #region "Write / Read"

        // Shared by every Format overload: serialize, optionally validate, and wrap the outcome.
        // strictRejectsInvalid = false downgrades a Strict validation failure to a warning (see the Error overload).
        private FormatWriteResult Write(Func<Stream> createXml, IEnumerable<KeyValuePair<string, string>> options, bool strictRejectsInvalid)
        {
            var schemas = GetFormatterOptions(options, "schema");
            var validationLevel = GetValidationLevel(options);

            try
            {
                var xml = createXml();
                if (xml == null || xml.Length == 0) return FormatWriteResult.Error();

                Rewind(xml);

                if (validationLevel <= ValidationIgnore)
                {
                    return FormatWriteResult.Successful(xml, ContentType);
                }

                if (schemas.Count == 0)
                {
                    return FormatWriteResult.Successful(xml, ContentType, ValidationSkippedMessage);
                }

                // Validate XML against XSD Schema
                var validationResponse = XmlValidator.Validate(xml, schemas);
                Rewind(xml);

                if (validationResponse.Success)
                {
                    return FormatWriteResult.Successful(xml, ContentType, ValidationSuccessfulMessage);
                }

                if (validationLevel >= ValidationStrict && strictRejectsInvalid)
                {
                    return FormatWriteResult.Error(validationResponse.Errors);
                }

                return FormatWriteResult.Warning(xml, ContentType, validationResponse.Errors);
            }
            catch (Exception ex)
            {
                return FormatWriteResult.Error(GetExceptionMessages(ex));
            }
        }

        // Shared by every Create*ResponseDocument method. The content is read into memory once so that
        // validation and parsing each get the whole document (validation used to consume the stream and
        // leave nothing for the parser) and so that non-seekable streams can be read.
        private static FormatReadResult<T> Read<T>(Stream content, IEnumerable<KeyValuePair<string, string>> options, Func<byte[], T> parse) where T : class
        {
            T document = null;
            var success = false;
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            if (content == null)
            {
                errors.Add("No content to read");
                return new FormatReadResult<T>(document, success, messages, warnings, errors);
            }

            var startPosition = content.CanSeek ? content.Position : -1;

            try
            {
                var bytes = ReadAllBytes(content);
                if (bytes.Length > 0)
                {
                    var validationLevel = GetValidationLevel(options);
                    if (validationLevel > ValidationIgnore)
                    {
                        var schemas = GetFormatterOptions(options, "schema");
                        if (schemas.Count == 0)
                        {
                            messages.Add(ValidationSkippedMessage);
                        }
                        else
                        {
                            // Validate XML against XSD Schema
                            using (var validationStream = new MemoryStream(bytes, false))
                            {
                                var validationResponse = XmlValidator.Validate(validationStream, schemas);
                                if (validationResponse.Success)
                                {
                                    messages.Add(ValidationSuccessfulMessage);
                                }
                                else if (!validationResponse.Errors.IsNullOrEmpty())
                                {
                                    if (validationLevel >= ValidationStrict) errors.AddRange(validationResponse.Errors);
                                    else warnings.AddRange(validationResponse.Errors);
                                }
                            }
                        }
                    }

                    // Read Document
                    document = parse(bytes);
                    success = document != null;
                }
                else
                {
                    errors.Add("No content to read");
                }
            }
            catch (Exception ex)
            {
                errors.AddRange(GetExceptionMessages(ex));
            }
            finally
            {
                // Leave a seekable stream where the caller had it, so it can be read again (e.g. as an Error document)
                if (startPosition >= 0 && content.CanSeek)
                {
                    try { content.Seek(startPosition, SeekOrigin.Begin); }
                    catch (ObjectDisposedException) { }
                }
            }

            return new FormatReadResult<T>(document, success, messages, warnings, errors);
        }

        private static byte[] ReadAllBytes(Stream content)
        {
            // Avoid a copy when handed a whole MemoryStream from its start (the readers copy before modifying)
            if (content is MemoryStream memoryStream && memoryStream.Position == 0 &&
                memoryStream.TryGetBuffer(out var buffer) && buffer.Offset == 0 && buffer.Count == buffer.Array.Length)
            {
                return buffer.Array;
            }

            using (var copy = new MemoryStream())
            {
                content.CopyTo(copy);
                return copy.ToArray();
            }
        }

        private static void Rewind(Stream stream)
        {
            if (stream.CanSeek && stream.Position > 0) stream.Seek(0, SeekOrigin.Begin);
        }

        private static List<string> GetExceptionMessages(Exception ex)
        {
            var messages = new List<string>();

            var exception = ex;
            while (exception != null)
            {
                messages.Add(exception.Message);
                exception = exception.InnerException;
            }

            return messages;
        }

        #endregion

        #region "Options"

        private static string GetFormatterOption(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            if (!options.IsNullOrEmpty())
            {
                var x = options.FirstOrDefault(o => o.Key == key).Value;
                if (!string.IsNullOrEmpty(x)) return x;
            }

            return null;
        }

        private static bool GetBoolOption(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            var x = GetFormatterOption(options, key);
            return x != null && bool.TryParse(x.Trim(), out var value) && value;
        }

        // Accepts the numeric level (as the HTTP server sends it) or the level name
        private static int GetValidationLevel(IEnumerable<KeyValuePair<string, string>> options)
        {
            var x = GetFormatterOption(options, "validationLevel");
            if (x != null)
            {
                x = x.Trim();

                if (int.TryParse(x, NumberStyles.Integer, CultureInfo.InvariantCulture, out var level))
                {
                    return level;
                }

                if (string.Equals(x, "Ignore", StringComparison.OrdinalIgnoreCase)) return ValidationIgnore;
                if (string.Equals(x, "Warning", StringComparison.OrdinalIgnoreCase)) return ValidationWarning;
                if (string.Equals(x, "Strict", StringComparison.OrdinalIgnoreCase)) return ValidationStrict;
            }

            return ValidationIgnore;
        }

        private static List<string> GetFormatterOptions(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            var l = new List<string>();

            if (!options.IsNullOrEmpty())
            {
                foreach (var option in options)
                {
                    if (option.Key == key && !string.IsNullOrEmpty(option.Value)) l.Add(option.Value);
                }
            }

            return l;
        }

        private static IEnumerable<T> GetFormatterOptionsJson<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            var l = new List<T>();

            foreach (var value in GetFormatterOptions(options, key))
            {
                try
                {
                    var obj = JsonSerializer.Deserialize<T>(value);
                    if (obj != null) l.Add(obj);
                }
                catch (JsonException) { }
            }

            return l;
        }

        #endregion
    }
}
