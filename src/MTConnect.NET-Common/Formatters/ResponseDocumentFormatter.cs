// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using MTConnect.Streams.Output;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MTConnect.Formatters
{
    public static class ResponseDocumentFormatter
    {
        private static readonly ConcurrentDictionary<string, IResponseDocumentFormatter> _formatters = new ConcurrentDictionary<string, IResponseDocumentFormatter>();
        private static bool _firstRead = true;


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


        public static FormattedDocumentWriteResult Format(string documentFormatterId, IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> formatOptions = null)
        {
            var stpw = Stopwatch.StartNew();

            FormattedDocumentWriteResult result = FormattedDocumentWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Format the Response Document using the Formatter
                result = formatter.Format(document, formatOptions);
            }

            stpw.Stop();
            result.ResponseDuration = (double)stpw.ElapsedTicks / 10000;

            return result;
        }

        public static FormattedDocumentWriteResult Format(string documentFormatterId, ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> formatOptions = null)
        {
            var stpw = Stopwatch.StartNew();

            FormattedDocumentWriteResult result = FormattedDocumentWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Format the Response Document using the Formatter
                result = formatter.Format(ref document, formatOptions);
            }

            stpw.Stop();
            result.ResponseDuration = (double)stpw.ElapsedTicks / 10000;

            return result;
        }

        public static FormattedDocumentWriteResult Format(string documentFormatterId, IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> formatOptions = null)
        {
            var stpw = Stopwatch.StartNew();

            FormattedDocumentWriteResult result = FormattedDocumentWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Format the Response Document using the Formatter
                result = formatter.Format(document, formatOptions);
            }

            stpw.Stop();
            result.ResponseDuration = (double)stpw.ElapsedTicks / 10000;

            return result;
        }

        public static FormattedDocumentWriteResult Format(string documentFormatterId, IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> formatOptions = null)
        {
            var stpw = Stopwatch.StartNew();

            FormattedDocumentWriteResult result = FormattedDocumentWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Format the Response Document using the Formatter
                result = formatter.Format(document, formatOptions);
            }

            stpw.Stop();
            result.ResponseDuration = (double)stpw.ElapsedTicks / 10000;

            return result;
        }


        public static FormattedDocumentReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(string documentFormatterId, byte[] content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Response Document using the Formatter
                return formatter.CreateDevicesResponseDocument(content);
            }

            return FormattedDocumentReadResult<IDevicesResponseDocument>.Error(null, $"Document Formatter Not found for \"{documentFormatterId}\"");
        }

        public static FormattedDocumentReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(string documentFormatterId, byte[] content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Response Document using the Formatter
                return formatter.CreateStreamsResponseDocument(content);
            }

            return FormattedDocumentReadResult<IStreamsResponseDocument>.Error(null, $"Document Formatter Not found for \"{documentFormatterId}\"");
        }

        public static FormattedDocumentReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(string documentFormatterId, byte[] content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Response Document using the Formatter
                return formatter.CreateAssetsResponseDocument(content);
            }

            return FormattedDocumentReadResult<IAssetsResponseDocument>.Error(null, $"Document Formatter Not found for \"{documentFormatterId}\"");
        }

        public static FormattedDocumentReadResult<IErrorResponseDocument> CreateErrorResponseDocument(string documentFormatterId, byte[] content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Response Document using the Formatter
                return formatter.CreateErrorResponseDocument(content);
            }

            return FormattedDocumentReadResult<IErrorResponseDocument>.Error(null, $"Document Formatter Not found for \"{documentFormatterId}\"");
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
                // Get IResponseDocumentFormatter Types
                var types = assemblies
                    .SelectMany(
                        x => x.GetMatchingTypesInAssembly(
                            t => typeof(IResponseDocumentFormatter).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));

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
