// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
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
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;

namespace MTConnect.Formatters.Xml
{
    public class XmlResponseDocumentFormatter : IResponseDocumentFormatter
    {
        public string Id => "XML";

        public string ContentType => "application/xml";


        public FormatWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
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

            var xml = XmlDevicesResponseDocument.ToXmlStream(document, null, stylesheet, indentOutput, outputComments);
            if (xml != null && xml.Length > 0)
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schemas);
                    if (validationResponse.Success)
                    {
                        return FormatWriteResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        if (validationLevel < 2) return FormatWriteResult.Warning(xml, ContentType, validationResponse.Errors);
                        else return FormatWriteResult.Error(validationResponse.Errors);
                    }
                }
                else
                {
                    return FormatWriteResult.Successful(xml, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

        public FormatWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
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

            var xml = XmlStreamsResponseDocument.ToXmlStream(ref document, extendedNamespaces, stylesheet, indentOutput, outputComments);
            if (xml != null && xml.Length > 0)
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schemas);
                    if (validationResponse.Success)
                    {
                        return FormatWriteResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        if (validationLevel < 2) return FormatWriteResult.Warning(xml, ContentType, validationResponse.Errors);
                        else return FormatWriteResult.Error(validationResponse.Errors);
                    }
                }
                else
                {
                    return FormatWriteResult.Successful(xml, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

        public FormatWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
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

            var xml = XmlAssetsResponseDocument.ToXmlStream(document, indentOutput, outputComments, stylesheet);
            if (xml != null && xml.Length > 0)
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schemas);
                    if (validationResponse.Success)
                    {
                        return FormatWriteResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        if (validationLevel < 2) return FormatWriteResult.Warning(xml, ContentType, validationResponse.Errors);
                        else return FormatWriteResult.Error(validationResponse.Errors);
                    }
                }
                else
                {
                    return FormatWriteResult.Successful(xml, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

        public FormatWriteResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
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

            var xml = XmlErrorResponseDocument.ToXmlStream(document, indentOutput, outputComments, stylesheet);
            if (xml != null)
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schemas);
                    if (validationResponse.Success)
                    {
                        return FormatWriteResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        return FormatWriteResult.Warning(xml, ContentType, validationResponse.Errors);
                    }
                }
                else
                {
                    return FormatWriteResult.Successful(xml, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }


        public FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            IDevicesResponseDocument document = null;
            var success = false;
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

            try
            {
                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    content.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }

                // Read Document
                document = XmlDevicesResponseDocument.FromXml(bytes);
                success = document != null;
            }
            catch (Exception ex)
            {
                messages.Add(ex.Message);
            }

            return new FormatReadResult<IDevicesResponseDocument>(document, success, messages, warnings, errors);
        }

        public FormatReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            IStreamsResponseDocument document = null;
            var success = false;
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

            try
            {
                if (content != null && content.Length > 0)
                {
                    using (var xmlReader = XmlReader.Create(content))
                    {
                        document = XmlStreamsResponseDocument.ReadXml(xmlReader);
                        success = document != null;
                    }
                }
            }
            catch (Exception ex)
            {
                messages.Add(ex.Message);
            }
            
            return new FormatReadResult<IStreamsResponseDocument>(document, success, messages, warnings, errors);
        }

        public FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
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

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                content.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            // Read Document
            var document = XmlAssetsResponseDocument.FromXml(bytes);
            var success = document != null;

            return new FormatReadResult<IAssetsResponseDocument>(document, success, messages, warnings, errors);
        }

        public FormatReadResult<IErrorResponseDocument> CreateErrorResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
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

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                content.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            // Read Document
            var document = XmlErrorResponseDocument.FromXml(bytes);
            var success = document != null;

            return new FormatReadResult<IErrorResponseDocument>(document, success, messages, warnings, errors);
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