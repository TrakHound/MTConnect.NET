// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Writers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace MTConnect
{
    public class XmlValidator
    {
        private string _xml;
        private string _schema;


        public XmlValidator(string xml, string schema = null)
        {
            _xml = xml;
            _schema = schema;
        }

        private static IEnumerable<XmlSchema> ReadSchemas(string schemaXml)
        {
            var schemas = new List<XmlSchema>();

            if (!string.IsNullOrEmpty(schemaXml))
            {
                try
                {
                    using (var reader = new StringReader(schemaXml))
                    {
                        var schema = XmlSchema.Read(reader, null);
                        if (schema != null) schemas.Add(schema);
                    }
                }
                catch { }
            }

            return schemas;
        }

        public static XmlValidationResponse Validate(string documentXml, string schemaXml = null)
        {
            var success = false;
            var errors = new List<string>();

            try
            {
                // Get list of XmlSchemas
                var schemas = ReadSchemas(schemaXml);

                // Set XML Reader Settings
                var readerSettings = new XmlReaderSettings();
                foreach (var schema in schemas) readerSettings.Schemas.Add(schema);
                readerSettings.ValidationType = ValidationType.Schema;
                //readerSettings.ConformanceLevel = ConformanceLevel.Document;
                //readerSettings.ValidationFlags = XmlSchemaValidationFlags.None;
                readerSettings.ValidationEventHandler += (s, e) =>
                {
                    System.Console.WriteLine(e.Message);
                    errors.Add($"(XML Validation {e.Severity}) : {e.Exception.Source} Line {e.Exception.LineNumber}, {e.Exception.LinePosition} : {e.Message}");
                };

                // Set XML Reader Settings
                using (var stringReader = new StringReader(documentXml))
                using (var xmlReader = XmlReader.Create(stringReader, readerSettings))
                {
                    var document = new XmlDocument();
                    document.Load(xmlReader);

                    success = errors.IsNullOrEmpty();
                }
            }
            catch { }

            return new XmlValidationResponse(success, errors);
        }
    }
}
