// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.Json;
using MTConnect.Devices;
using MTConnect.Devices.Json;
using MTConnect.Errors;
using MTConnect.Streams;
using MTConnect.Streams.Json;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MTConnect.Formatters
{
    public class JsonResponseDocumentFormatter : IResponseDocumentFormatter
    {
        public string Id => "JSON";

        public string ContentType => "application/json";


        public FormattedDocumentWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var json = JsonSerializer.SerializeToUtf8Bytes(new JsonDevicesDocument(document), jsonOptions);
            if (!json.IsNullOrEmpty())
            {
                return FormattedDocumentWriteResult.Successful(json, ContentType);
            }

            return FormattedDocumentWriteResult.Error();
        }

        public FormattedDocumentWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var json = JsonSerializer.SerializeToUtf8Bytes(new JsonStreamsDocument(document), jsonOptions);
            if (!json.IsNullOrEmpty())
            {
                return FormattedDocumentWriteResult.Successful(json, ContentType);
            }

            return FormattedDocumentWriteResult.Error();
        }

        public FormattedDocumentWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var json = JsonSerializer.SerializeToUtf8Bytes(new JsonAssetsDocument(document), jsonOptions);
            if (!json.IsNullOrEmpty())
            {
                return FormattedDocumentWriteResult.Successful(json, ContentType);
            }

            return FormattedDocumentWriteResult.Error();
        }

        public FormattedDocumentWriteResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var json = JsonSerializer.SerializeToUtf8Bytes(document, jsonOptions);
            if (!json.IsNullOrEmpty())
            {
                return FormattedDocumentWriteResult.Successful(json, ContentType);
            }

            return FormattedDocumentWriteResult.Error();
        }


        public FormattedDocumentReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<DevicesResponseDocument>(content);
            var success = document != null;

            return new FormattedDocumentReadResult<IDevicesResponseDocument>(document, success);
        }

        public FormattedDocumentReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<StreamsResponseDocument>(content);
            var success = document != null;

            return new FormattedDocumentReadResult<IStreamsResponseDocument>(document, success);
        }

        public FormattedDocumentReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<AssetsResponseDocument>(content);
            var success = document != null;

            return new FormattedDocumentReadResult<IAssetsResponseDocument>(document, success);
        }

        public FormattedDocumentReadResult<IErrorResponseDocument> CreateErrorResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<ErrorResponseDocument>(content);
            var success = document != null;

            return new FormattedDocumentReadResult<IErrorResponseDocument>(document, success);
        }


        private static T GetFormatterOption<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            if (!options.IsNullOrEmpty())
            {
                var x = options.FirstOrDefault(o => o.Key == key).Value;
                if (!string.IsNullOrEmpty(x))
                {
                    try
                    {
                        return (T)Convert.ChangeType(x, typeof(T));
                    }
                    catch { }
                }
            }

            return default;
        }

        private static IEnumerable<T> GetFormatterOptions<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            var l = new List<T>();

            if (!options.IsNullOrEmpty())
            {
                var x = options.Where(o => o.Key == key);
                if (!x.IsNullOrEmpty())
                {
                    foreach (var y in x)
                    {
                        if (!string.IsNullOrEmpty(y.Value))
                        {
                            try
                            {
                                var obj = (T)Convert.ChangeType(y.Value, typeof(T));
                                l.Add(obj);
                            }
                            catch { }
                        }
                    }
                }
            }

            return l;
        }
    }
}