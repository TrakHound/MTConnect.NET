// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Assets.Xml;
using MTConnect.Devices;
using MTConnect.Devices.Xml;
using MTConnect.Errors;
using MTConnect.Streams;
using MTConnect.Streams.Xml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Formatters
{
    public class XmlResponseDocumentFormatter : IResponseDocumentFormatter
    {
        public string Id => "XML";

        public string ContentType => "application/xml";


        public FormattedDocumentResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read XSD Schema
            var schema = GetFormatterOption<string>(options, "schema");

            // Read Devices Stylesheet
            var stylesheet = GetFormatterOption<string>(options, "devicesStyle.location");

            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");

            // Read OutputComments Option passed to Formatter
            var outputComments = GetFormatterOption<bool>(options, "outputComments");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            var xml = XmlDevicesResponseDocument.ToXml(document, null, stylesheet, indentOutput, outputComments);
            if (!string.IsNullOrEmpty(xml))
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schema);
                    if (validationResponse.Success)
                    {
                        return FormattedDocumentResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        if (validationLevel < 2) return FormattedDocumentResult.Warning(xml, ContentType, validationResponse.Errors);
                        else return FormattedDocumentResult.Error(validationResponse.Errors);                   
                    }
                }
                else
                {
                    return FormattedDocumentResult.Successful(xml, ContentType);
                }
            }

            return FormattedDocumentResult.Error();
        }

        public FormattedDocumentResult Format(IStreamsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read XSD Schema
            var schema = GetFormatterOption<string>(options, "schema");

            // Read Devices Stylesheet
            var stylesheet = GetFormatterOption<string>(options, "streamsStyle.location");

            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");

            // Read OutputComments Option passed to Formatter
            var outputComments = GetFormatterOption<bool>(options, "outputComments");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            var xml = XmlStreamsResponseDocument.ToXml(document, null, stylesheet, indentOutput, outputComments);
            if (!string.IsNullOrEmpty(xml))
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schema);
                    if (validationResponse.Success)
                    {
                        return FormattedDocumentResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        if (validationLevel < 2) return FormattedDocumentResult.Warning(xml, ContentType, validationResponse.Errors);
                        else return FormattedDocumentResult.Error(validationResponse.Errors);
                    }
                }
                else
                {
                    return FormattedDocumentResult.Successful(xml, ContentType);
                }
            }

            return FormattedDocumentResult.Error();
        }

        public FormattedDocumentResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read XSD Schema
            var schema = GetFormatterOption<string>(options, "schema");

            // Read Devices Stylesheet
            var stylesheet = GetFormatterOption<string>(options, "assetsStyle.location");

            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");

            // Read OutputComments Option passed to Formatter
            var outputComments = GetFormatterOption<bool>(options, "outputComments");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            var xml = XmlAssetsResponseDocument.ToXml(document, stylesheet, indentOutput, outputComments);
            if (!string.IsNullOrEmpty(xml))
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schema);
                    if (validationResponse.Success)
                    {
                        return FormattedDocumentResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        if (validationLevel < 2) return FormattedDocumentResult.Warning(xml, ContentType, validationResponse.Errors);
                        else return FormattedDocumentResult.Error(validationResponse.Errors);
                    }
                }
                else
                {
                    return FormattedDocumentResult.Successful(xml, ContentType);
                }
            }

            return FormattedDocumentResult.Error();
        }

        public FormattedDocumentResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read XSD Schema
            var schema = GetFormatterOption<string>(options, "schema");

            // Read Devices Stylesheet
            var stylesheet = GetFormatterOption<string>(options, "errorStyle.location");

            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");

            // Read OutputComments Option passed to Formatter
            var outputComments = GetFormatterOption<bool>(options, "outputComments");

            // Read Validation Level Option passed to Formatter (0 = Ignore, 1 = Warning, 2 = Strict)
            var validationLevel = GetFormatterOption<int>(options, "validationLevel");

            var xml = XmlErrorResponseDocument.ToXml(document, stylesheet, indentOutput, outputComments);
            if (!string.IsNullOrEmpty(xml))
            {
                if (validationLevel > 0)
                {
                    // Validate XML against XSD Schema
                    var validationResponse = XmlValidator.Validate(xml, schema);
                    if (validationResponse.Success)
                    {
                        return FormattedDocumentResult.Successful(xml, ContentType, "XML Validation Successful");
                    }
                    else
                    {
                        // Return Successful if ValidationLevel set to Warning
                        return FormattedDocumentResult.Warning(xml, ContentType, validationResponse.Errors);
                    }
                }
                else
                {
                    return FormattedDocumentResult.Successful(xml, ContentType);
                }
            }

            return FormattedDocumentResult.Error();
        }


        public IDevicesResponseDocument CreateDevicesResponseDocument(string content)
        {
            return XmlDevicesResponseDocument.FromXml(content);
        }

        public IStreamsResponseDocument CreateStreamsResponseDocument(string content)
        {
            return XmlStreamsResponseDocument.FromXml(content);
        }

        public IAssetsResponseDocument CreateAssetsResponseDocument(string content)
        {
            return XmlAssetsResponseDocument.FromXml(content);
        }

        public IErrorResponseDocument CreateErrorResponseDocument(string content)
        {
            return XmlErrorResponseDocument.FromXml(content);
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
    }
}
