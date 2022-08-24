// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace MTConnect
{
    internal static class XmlValidator
    {
        public static XmlValidationResponse Validate(byte[] documentXml, IEnumerable<string> schemaXmls = null)
        {
            var success = false;
            var errors = new List<string>();

            if (documentXml != null)
            {
                if (schemaXmls.IsNullOrEmpty())
                {
                    // If no Schema specified then return as Success
                    success = true;
                }
                else
                {
                    try
                    {
                        // Get list of XmlSchemas
                        var schemas = new List<XmlSchema>();
                        if (!schemaXmls.IsNullOrEmpty())
                        {
                            foreach (var schemaXml in schemaXmls)
                            {
                                try
                                {
                                    using (var reader = new StringReader(schemaXml))
                                    {
                                        var schema = XmlSchema.Read(reader, null);
                                        if (schema != null) schemas.Add(schema);
                                    }
                                }
                                catch (XmlSchemaException ex)
                                {
                                    errors.Add($"(XML Validation Error) : Error Reading XSD Schema : {ex.SourceUri} Line {ex.LineNumber}, {ex.LinePosition} : {ex.Message}");
                                }
                                catch (Exception ex)
                                {
                                    errors.Add($"(XML Validation Error) : Error Reading XSD Schema : {ex.Message}");
                                }
                            }
                        }

                        // Set XML Reader Settings
                        var readerSettings = new XmlReaderSettings();
                        foreach (var schema in schemas) readerSettings.Schemas.Add(schema);
                        readerSettings.ValidationType = ValidationType.Schema;

                        readerSettings.ValidationEventHandler += (s, e) =>
                        {
                            errors.Add($"(XML Validation {e.Severity}) : {e.Exception.Source} Line {e.Exception.LineNumber}, {e.Exception.LinePosition} : {e.Message}");
                        };

                        // Set XML Reader Settings
                        using (var reader = new MemoryStream(documentXml))
                        using (var xmlReader = XmlReader.Create(reader, readerSettings))
                        {
                            var document = new XmlDocument();
                            document.Load(xmlReader);

                            success = errors.IsNullOrEmpty();
                        }
                    }
                    catch (XmlSchemaException ex)
                    {
                        errors.Add($"(XML Validation Error) : Error Adding XSD Schema : {ex.SourceUri} Line {ex.LineNumber}, {ex.LinePosition} : {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"(XML Validation Error) : Error During Validation : {ex.Message}");
                    }
                }
            }

            return new XmlValidationResponse(success, errors);
        }
    }
}
