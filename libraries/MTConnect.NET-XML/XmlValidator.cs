// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace MTConnect
{
    internal static class XmlValidator
    {
        // Reading and compiling the MTConnect XSDs is far more expensive than validating a response, and the
        // schemas only change with configuration, so compiled schema sets are cached per set of schema sources.
        // XmlSchemaSet is not documented as thread-safe, so each cached set is used by one validation at a time:
        // validations take a set from the pool (compiling a new one when it is empty) and return it afterwards.
        private const int MaxCachedSchemaConfigurations = 16;
        private const int MaxPooledSetsPerConfiguration = 8;

        private static readonly ConcurrentDictionary<string, ConcurrentBag<CompiledSchemas>> _schemaPools = new ConcurrentDictionary<string, ConcurrentBag<CompiledSchemas>>();


        private sealed class CompiledSchemas
        {
            public XmlSchemaSet Schemas;
            public List<string> Errors; // Errors reading / compiling the schemas, reported on every validation
        }


        public static XmlValidationResponse Validate(Stream documentXml, IEnumerable<string> schemaXmls = null)
        {
            if (documentXml == null) return new XmlValidationResponse(false, new List<string>());

            // If no Schema specified then return as Success
            if (schemaXmls.IsNullOrEmpty()) return new XmlValidationResponse(true, new List<string>());

            var sources = schemaXmls.ToArray();
            var pool = GetPool(sources);
            if (!pool.TryTake(out var compiled)) compiled = Compile(sources);

            var errors = new List<string>(compiled.Errors);

            try
            {
                var readerSettings = new XmlReaderSettings();
                readerSettings.Schemas = compiled.Schemas;
                readerSettings.ValidationType = ValidationType.Schema;
                readerSettings.ValidationEventHandler += (s, e) =>
                {
                    errors.Add($"(XML Validation {e.Severity}) : {e.Exception.Source} Line {e.Exception.LineNumber}, {e.Exception.LinePosition} : {e.Message}");
                };

                // Validation happens as the document is read; no need to build a DOM
                using (var xmlReader = XmlReader.Create(documentXml, readerSettings))
                {
                    while (xmlReader.Read()) { }
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
            finally
            {
                if (pool.Count < MaxPooledSetsPerConfiguration) pool.Add(compiled);
            }

            return new XmlValidationResponse(errors.Count == 0, errors);
        }

        private static ConcurrentBag<CompiledSchemas> GetPool(string[] sources)
        {
            var key = string.Join("\u0000", sources.Select(o => o ?? string.Empty));

            if (!_schemaPools.TryGetValue(key, out var pool))
            {
                // Keep the cache bounded if schemas are supplied dynamically
                if (_schemaPools.Count >= MaxCachedSchemaConfigurations) _schemaPools.Clear();

                pool = _schemaPools.GetOrAdd(key, _ => new ConcurrentBag<CompiledSchemas>());
            }

            return pool;
        }

        private static CompiledSchemas Compile(string[] schemaXmls)
        {
            var errors = new List<string>();
            var schemaSet = new XmlSchemaSet();

            foreach (var schemaXml in schemaXmls)
            {
                XmlSchema schema = null;

                try
                {
                    // Strip XSD 1.1-only constructs (xs:assert,
                    // xs:override, notNamespace) before handing
                    // the source to the BCL XSD 1.0 reader.
                    // The official MTConnect XSDs from v1.3
                    // onwards carry XSD 1.1 features that
                    // XmlSchema.Read otherwise rejects with
                    // "notNamespace not supported".
                    var preprocessed = schemaXml != null
                        ? XsdPreprocessor.StripXsd11Constructs(schemaXml)
                        : null;

                    using (var reader = new StringReader(preprocessed ?? string.Empty))
                    {
                        schema = XmlSchema.Read(reader, null);
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

                if (schema != null)
                {
                    try
                    {
                        schemaSet.Add(schema);
                    }
                    catch (XmlSchemaException ex)
                    {
                        errors.Add($"(XML Validation Error) : Error Adding XSD Schema : {ex.SourceUri} Line {ex.LineNumber}, {ex.LinePosition} : {ex.Message}");
                    }
                }
            }

            try
            {
                schemaSet.Compile();
            }
            catch (XmlSchemaException ex)
            {
                errors.Add($"(XML Validation Error) : Error Adding XSD Schema : {ex.SourceUri} Line {ex.LineNumber}, {ex.LinePosition} : {ex.Message}");
            }

            return new CompiledSchemas { Schemas = schemaSet, Errors = errors };
        }
    }
}
