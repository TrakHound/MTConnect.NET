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