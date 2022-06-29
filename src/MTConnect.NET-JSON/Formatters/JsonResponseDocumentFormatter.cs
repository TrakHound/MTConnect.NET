// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MTConnect.Assets.Json;
using MTConnect.Devices.Json;
using MTConnect.Streams.Json;

namespace MTConnect.Formatters
{
    public class JsonResponseDocumentFormatter : IResponseDocumentFormatter
    {
        public string Id => "JSON";

        public string ContentType => "application/json";


        public FormattedDocumentResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
               
            };

            var json = JsonSerializer.Serialize(new JsonDevicesDocument(document), jsonOptions);
            if (!string.IsNullOrEmpty(json))
            {
                return FormattedDocumentResult.Successful(json, ContentType);
            }

            return FormattedDocumentResult.Error();
        }

        public FormattedDocumentResult Format(IStreamsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            var json = JsonSerializer.Serialize(new JsonStreamsDocument(document), jsonOptions);
            if (!string.IsNullOrEmpty(json))
            {
                return FormattedDocumentResult.Successful(json, ContentType);
            }

            return FormattedDocumentResult.Error();
        }

        public FormattedDocumentResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            var json = JsonSerializer.Serialize(new JsonAssetsDocument(document), jsonOptions);
            if (!string.IsNullOrEmpty(json))
            {
                return FormattedDocumentResult.Successful(json, ContentType);
            }

            return FormattedDocumentResult.Error();
        }

        public FormattedDocumentResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var json = JsonSerializer.Serialize(document);
            if (!string.IsNullOrEmpty(json))
            {
                return FormattedDocumentResult.Successful(json, ContentType);
            }

            return FormattedDocumentResult.Error();
        }


        public IDevicesResponseDocument CreateDevicesResponseDocument(string content)
        {
            return JsonSerializer.Deserialize<DevicesResponseDocument>(content);
        }

        public IStreamsResponseDocument CreateStreamsResponseDocument(string content)
        {
            return JsonSerializer.Deserialize<StreamsResponseDocument>(content);
        }

        public IAssetsResponseDocument CreateAssetsResponseDocument(string content)
        {
            return JsonSerializer.Deserialize<AssetsResponseDocument>(content);
        }

        public IErrorResponseDocument CreateErrorResponseDocument(string content)
        {
            return JsonSerializer.Deserialize<ErrorResponseDocument>(content);
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
