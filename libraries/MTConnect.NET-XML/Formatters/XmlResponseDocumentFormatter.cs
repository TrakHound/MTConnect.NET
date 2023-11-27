// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
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
using System.Linq;
using System.Text.Json;

namespace MTConnect.Formatters.Xml
{
    public class XmlResponseDocumentFormatter : IResponseDocumentFormatter
    {
        public string Id => "XML";

        public string ContentType => "application/xml";


        public FormattedDocumentWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read XSD Schema
            var schemas = GetFormatterOptions<string>(options, "schema");

            // Read Devices Stylesheet
            var stylesheet = GetFormatterOption<string>(options, "devicesStyle.location");

            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");

            // Read OutputComments Option passed to Formatter
            var outputComments = GetFormatterOption<bool>(options, "outputComments");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            var xml = XmlDevicesResponseDocument.ToXmlBytes(document, null, stylesheet, indentOutput, outputComments);
            if (xml != null && xml.Length > 0)
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schemas);
                    if (validationResponse.Success)
                    {
                        return FormattedDocumentWriteResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        if (validationLevel < 2) return FormattedDocumentWriteResult.Warning(xml, ContentType, validationResponse.Errors);
                        else return FormattedDocumentWriteResult.Error(validationResponse.Errors);
                    }
                }
                else
                {
                    return FormattedDocumentWriteResult.Successful(xml, ContentType);
                }
            }

            return FormattedDocumentWriteResult.Error();
        }

        public FormattedDocumentWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read XSD Schema
            var schemas = GetFormatterOptions<string>(options, "schema");

            // Read Extended Namespaces
            var extendedNamespaces = GetFormatterOptionsJson<Configurations.NamespaceConfiguration>(options, "namespace");

            // Read Devices Stylesheet
            var stylesheet = GetFormatterOption<string>(options, "streamsStyle.location");

            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");

            // Read OutputComments Option passed to Formatter
            var outputComments = GetFormatterOption<bool>(options, "outputComments");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            var xml = XmlStreamsResponseDocument.ToXmlBytes(ref document, extendedNamespaces, stylesheet, indentOutput, outputComments);
            if (xml != null && xml.Length > 0)
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schemas);
                    if (validationResponse.Success)
                    {
                        return FormattedDocumentWriteResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        if (validationLevel < 2) return FormattedDocumentWriteResult.Warning(xml, ContentType, validationResponse.Errors);
                        else return FormattedDocumentWriteResult.Error(validationResponse.Errors);
                    }
                }
                else
                {
                    return FormattedDocumentWriteResult.Successful(xml, ContentType);
                }
            }

            return FormattedDocumentWriteResult.Error();
        }

        public FormattedDocumentWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read XSD Schema
            var schemas = GetFormatterOptions<string>(options, "schema");

            // Read Devices Stylesheet
            var stylesheet = GetFormatterOption<string>(options, "assetsStyle.location");

            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");

            // Read OutputComments Option passed to Formatter
            var outputComments = GetFormatterOption<bool>(options, "outputComments");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            var xml = XmlAssetsResponseDocument.ToXmlBytes(document, indentOutput, outputComments, stylesheet);
            if (xml != null && xml.Length > 0)
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schemas);
                    if (validationResponse.Success)
                    {
                        return FormattedDocumentWriteResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        if (validationLevel < 2) return FormattedDocumentWriteResult.Warning(xml, ContentType, validationResponse.Errors);
                        else return FormattedDocumentWriteResult.Error(validationResponse.Errors);
                    }
                }
                else
                {
                    return FormattedDocumentWriteResult.Successful(xml, ContentType);
                }
            }

            return FormattedDocumentWriteResult.Error();
        }

        public FormattedDocumentWriteResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read XSD Schema
            var schemas = GetFormatterOptions<string>(options, "schema");

            // Read Devices Stylesheet
            var stylesheet = GetFormatterOption<string>(options, "errorStyle.location");

            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");

            // Read OutputComments Option passed to Formatter
            var outputComments = GetFormatterOption<bool>(options, "outputComments");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            var xml = XmlErrorResponseDocument.ToXmlBytes(document, indentOutput, outputComments, stylesheet);
            if (xml != null)
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schemas);
                    if (validationResponse.Success)
                    {
                        return FormattedDocumentWriteResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        return FormattedDocumentWriteResult.Warning(xml, ContentType, validationResponse.Errors);
                    }
                }
                else
                {
                    return FormattedDocumentWriteResult.Successful(xml, ContentType);
                }
            }

            return FormattedDocumentWriteResult.Error();
        }


        public FormattedDocumentReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read XSD Schema
            var schemas = GetFormatterOptions<string>(options, "schema");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            if (validationLevel > 0)
            {
                // Validate XML against XSD Schema
                var validationResponse = XmlValidator.Validate(content, schemas);
                if (validationResponse.Success)
                {
                    messages.Add("XML Validation Successful");
                }
                else if (!validationResponse.Errors.IsNullOrEmpty())
                {
                    if (validationLevel > 1)
                    {
                        errors.AddRange(validationResponse.Errors);
                    }
                    else
                    {
                        warnings.AddRange(validationResponse.Errors);
                    }
                }
            }

            // Read Document
            var document = XmlDevicesResponseDocument.FromXml(content);
            var success = document != null;

            return new FormattedDocumentReadResult<IDevicesResponseDocument>(document, success, messages, warnings, errors);
        }

        public FormattedDocumentReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read XSD Schema
            var schemas = GetFormatterOptions<string>(options, "schema");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            if (validationLevel > 0)
            {
                // Validate XML against XSD Schema
                var validationResponse = XmlValidator.Validate(content, schemas);
                if (validationResponse.Success)
                {
                    messages.Add("XML Validation Successful");
                }
                else if (!validationResponse.Errors.IsNullOrEmpty())
                {
                    if (validationLevel > 1)
                    {
                        errors.AddRange(validationResponse.Errors);
                    }
                    else
                    {
                        warnings.AddRange(validationResponse.Errors);
                    }
                }
            }

            // Read Document
            var document = XmlStreamsResponseDocument.FromXml(content);
            var success = document != null;

            return new FormattedDocumentReadResult<IStreamsResponseDocument>(document, success, messages, warnings, errors);
        }

        public FormattedDocumentReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read XSD Schema
            var schemas = GetFormatterOptions<string>(options, "schema");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            if (validationLevel > 0)
            {
                // Validate XML against XSD Schema
                var validationResponse = XmlValidator.Validate(content, schemas);
                if (validationResponse.Success)
                {
                    messages.Add("XML Validation Successful");
                }
                else if (!validationResponse.Errors.IsNullOrEmpty())
                {
                    if (validationLevel > 1)
                    {
                        errors.AddRange(validationResponse.Errors);
                    }
                    else
                    {
                        warnings.AddRange(validationResponse.Errors);
                    }
                }
            }

            // Read Document
            var document = XmlAssetsResponseDocument.FromXml(content);
            var success = document != null;

            return new FormattedDocumentReadResult<IAssetsResponseDocument>(document, success, messages, warnings, errors);
        }

        public FormattedDocumentReadResult<IErrorResponseDocument> CreateErrorResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read XSD Schema
            var schemas = GetFormatterOptions<string>(options, "schema");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            if (validationLevel > 0)
            {
                // Validate XML against XSD Schema
                var validationResponse = XmlValidator.Validate(content, schemas);
                if (validationResponse.Success)
                {
                    messages.Add("XML Validation Successful");
                }
                else if (!validationResponse.Errors.IsNullOrEmpty())
                {
                    if (validationLevel > 1)
                    {
                        errors.AddRange(validationResponse.Errors);
                    }
                    else
                    {
                        warnings.AddRange(validationResponse.Errors);
                    }
                }
            }

            // Read Document
            var document = XmlErrorResponseDocument.FromXml(content);
            var success = document != null;

            return new FormattedDocumentReadResult<IErrorResponseDocument>(document, success, messages, warnings, errors);
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

        private static T GetFormatterOptionJson<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            if (!options.IsNullOrEmpty())
            {
                var x = options.FirstOrDefault(o => o.Key == key).Value;
                if (!string.IsNullOrEmpty(x))
                {
                    try
                    {
                        return JsonSerializer.Deserialize<T>(x);
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

        private static IEnumerable<T> GetFormatterOptionsJson<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
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
                                var obj = JsonSerializer.Deserialize<T>(y.Value);
                                if (obj != null) l.Add(obj);
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