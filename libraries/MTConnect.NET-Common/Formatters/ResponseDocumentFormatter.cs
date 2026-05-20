// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using MTConnect.Streams.Output;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MTConnect.Formatters
{
    /// <summary>
    /// Static facade that discovers and caches the <see cref="IResponseDocumentFormatter"/> implementations available across loaded assemblies and dispatches response-document serialization/deserialization to the one matching a requested document format, timing each operation.
    /// </summary>
    public static class ResponseDocumentFormatter
    {
        private static readonly ConcurrentDictionary<string, IResponseDocumentFormatter> _formatters = new ConcurrentDictionary<string, IResponseDocumentFormatter>();
        private static bool _firstRead = true;


        /// <summary>
        /// Returns the MIME content type produced by the response-document formatter registered for the given format, or null when no such formatter exists.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the document format.</param>
        public static string GetContentType(string documentFormatterId)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                return formatter.ContentType;
            }

            return null;
        }


        /// <summary>
        /// Serializes a Devices response document using the formatter for the given format, returning an error result when no matching formatter exists. The result's response duration is set to the elapsed time.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the document format.</param>
        /// <param name="document">The Devices document to serialize.</param>
        /// <param name="formatOptions">Optional format-specific key/value options.</param>
        public static FormatWriteResult Format(string documentFormatterId, IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> formatOptions = null)
        {
            var stpw = Stopwatch.StartNew();

            FormatWriteResult result = FormatWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Format the Response Document using the Formatter
                result = formatter.Format(document, formatOptions);
            }

            stpw.Stop();
            result.ResponseDuration = stpw.GetElapsedMilliseconds();

            return result;
        }

        /// <summary>
        /// Serializes a Streams response document using the formatter for the given format, returning an error result when no matching formatter exists. The document is passed by reference so streaming serializers may release it as it is consumed. The result's response duration is set to the elapsed time.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the document format.</param>
        /// <param name="document">The Streams output document to serialize.</param>
        /// <param name="formatOptions">Optional format-specific key/value options.</param>
        public static FormatWriteResult Format(string documentFormatterId, ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> formatOptions = null)
        {
            var stpw = Stopwatch.StartNew();

            FormatWriteResult result = FormatWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Format the Response Document using the Formatter
                result = formatter.Format(ref document, formatOptions);
            }

            stpw.Stop();
            result.ResponseDuration = stpw.GetElapsedMilliseconds();

            return result;
        }

        /// <summary>
        /// Serializes an Assets response document using the formatter for the given format, returning an error result when no matching formatter exists. The result's response duration is set to the elapsed time.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the document format.</param>
        /// <param name="document">The Assets document to serialize.</param>
        /// <param name="formatOptions">Optional format-specific key/value options.</param>
        public static FormatWriteResult Format(string documentFormatterId, IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> formatOptions = null)
        {
            var stpw = Stopwatch.StartNew();

            FormatWriteResult result = FormatWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Format the Response Document using the Formatter
                result = formatter.Format(document, formatOptions);
            }

            stpw.Stop();
            result.ResponseDuration = stpw.GetElapsedMilliseconds();

            return result;
        }

        /// <summary>
        /// Serializes an Error response document using the formatter for the given format, returning an error result when no matching formatter exists. The result's response duration is set to the elapsed time.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the document format.</param>
        /// <param name="document">The Error document to serialize.</param>
        /// <param name="formatOptions">Optional format-specific key/value options.</param>
        public static FormatWriteResult Format(string documentFormatterId, IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> formatOptions = null)
        {
            var stpw = Stopwatch.StartNew();

            FormatWriteResult result = FormatWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Format the Response Document using the Formatter
                result = formatter.Format(document, formatOptions);
            }

            stpw.Stop();
            result.ResponseDuration = stpw.GetElapsedMilliseconds();

            return result;
        }


        /// <summary>
        /// Deserializes a Devices response document from the given content stream using the formatter for the given format; returns an error result when no matching formatter exists.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the document format.</param>
        /// <param name="content">The serialized Devices document content.</param>
        public static FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(string documentFormatterId, Stream content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Response Document using the Formatter
                return formatter.CreateDevicesResponseDocument(content);
            }

            return FormatReadResult<IDevicesResponseDocument>.Error(null, $"Document Formatter Not found for \"{documentFormatterId}\"");
        }

        /// <summary>
        /// Deserializes a Streams response document from the given content stream using the formatter for the given format; returns an error result when no matching formatter exists.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the document format.</param>
        /// <param name="content">The serialized Streams document content.</param>
        public static FormatReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(string documentFormatterId, Stream content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Response Document using the Formatter
                return formatter.CreateStreamsResponseDocument(content);
            }

            return FormatReadResult<IStreamsResponseDocument>.Error(null, $"Document Formatter Not found for \"{documentFormatterId}\"");
        }

        /// <summary>
        /// Deserializes an Assets response document from the given content stream using the formatter for the given format; returns an error result when no matching formatter exists.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the document format.</param>
        /// <param name="content">The serialized Assets document content.</param>
        public static FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(string documentFormatterId, Stream content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Response Document using the Formatter
                return formatter.CreateAssetsResponseDocument(content);
            }

            return FormatReadResult<IAssetsResponseDocument>.Error(null, $"Document Formatter Not found for \"{documentFormatterId}\"");
        }

        /// <summary>
        /// Deserializes an Error response document from the given content stream using the formatter for the given format; returns an error result when no matching formatter exists.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the document format.</param>
        /// <param name="content">The serialized Error document content.</param>
        public static FormatReadResult<IErrorResponseDocument> CreateErrorResponseDocument(string documentFormatterId, Stream content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Response Document using the Formatter
                return formatter.CreateErrorResponseDocument(content);
            }

            return FormatReadResult<IErrorResponseDocument>.Error(null, $"Document Formatter Not found for \"{documentFormatterId}\"");
        }


        private static IResponseDocumentFormatter GetFormatter(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (_firstRead)
                {
                    AddFormatters();
                    _firstRead = false;
                }

                // Normalize the FormatterId
                var formatterId = id.ToLower();

                _formatters.TryGetValue(formatterId, out var formatter);
                return formatter;
            }

            return null;
        }


        private static void AddFormatters()
        {
            var assemblies = Assemblies.Get();
            if (!assemblies.IsNullOrEmpty())
            {
                var types = new List<Type>();

                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var assemblyTypes = assembly.GetTypes();
                        if (!assemblyTypes.IsNullOrEmpty())
                        {
                            foreach (var type in assemblyTypes)
                            {
                                if (typeof(IResponseDocumentFormatter).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                                {
                                    types.Add(type);
                                }
                            }
                        }
                    }
                    catch { }
                }

                if (!types.IsNullOrEmpty())
                {
                    foreach (var type in types)
                    {
                        try
                        {
                            // Create new Instance of the Formatter and add to cached dictionary
                            var formatter = (IResponseDocumentFormatter)Activator.CreateInstance(type);

                            // Normalize the FormatterId
                            var formatterId = formatter.Id.ToLower();

                            _formatters.TryAdd(formatterId, formatter);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}