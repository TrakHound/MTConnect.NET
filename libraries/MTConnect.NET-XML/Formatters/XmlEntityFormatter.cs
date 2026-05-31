// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.Xml;
using MTConnect.Devices;
using MTConnect.Devices.Xml;
using MTConnect.Observations;
using MTConnect.Streams.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace MTConnect.Formatters.Xml
{
    /// <summary>
    /// <see cref="IEntityFormatter"/> that serializes and deserializes
    /// individual MTConnect entities (devices, observations, and assets) to and
    /// from the MTConnect XML representation.
    /// </summary>
    public class XmlEntityFormatter : IEntityFormatter
    {
        /// <summary>
        /// The identifier this formatter is selected by; <c>XML</c>.
        /// </summary>
        public string Id => "XML";

        /// <summary>
        /// The MIME content type the formatter produces;
        /// <c>application/xml</c>.
        /// </summary>
        public string ContentType => "application/xml";


        /// <summary>
        /// Serializes a single <see cref="IDevice"/> to XML, honouring the
        /// <c>indentOutput</c> option.
        /// </summary>
        public FormatWriteResult Format(IDevice device, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (device != null)
            {
                var indentOuput = GetFormatterOption<bool>(options, "indentOutput");
                var settings = indentOuput ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                try
                {
                    var outputStream = new MemoryStream();

                    // Use XmlWriter to write XML to stream
                    using (var xmlWriter = XmlWriter.Create(outputStream, settings))
                    {
                        XmlDevice.WriteXml(xmlWriter, device);
                        xmlWriter.Flush();

                        return FormatWriteResult.Successful(outputStream, ContentType);
                    }
                }
                catch { }
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes a single <see cref="IObservation"/> to XML, honouring the
        /// <c>indentOutput</c> option.
        /// </summary>
        public FormatWriteResult Format(IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (observation != null)
            {
                var indentOuput = GetFormatterOption<bool>(options, "indentOutput");
                var settings = indentOuput ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                try
                {
                    var outputStream = new MemoryStream();

                    // Use XmlWriter to write XML to stream
                    using (var xmlWriter = XmlWriter.Create(outputStream, settings))
                    {
                        XmlObservation.WriteXml(xmlWriter, observation);
                        xmlWriter.Flush();

                        return FormatWriteResult.Successful(outputStream, ContentType);
                    }
                }
                catch { }
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes a sequence of <see cref="IObservation"/> values to a
        /// single XML stream, honouring the <c>indentOutput</c> option.
        /// </summary>
        public FormatWriteResult Format(IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (!observations.IsNullOrEmpty())
            {
                var indentOuput = GetFormatterOption<bool>(options, "indentOutput");
                var settings = indentOuput ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                try
                {
                    var outputStream = new MemoryStream();

                    // Use XmlWriter to write XML to stream
                    using (var xmlWriter = XmlWriter.Create(outputStream, settings))
                    {
                        foreach (var observation in observations)
                        {
                            XmlObservation.WriteXml(xmlWriter, observation);
                            xmlWriter.Flush();
                        }
                    }

                    return FormatWriteResult.Successful(outputStream, ContentType);
                }
                catch { }
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes a single <see cref="IAsset"/> to indented XML.
        /// </summary>
        public FormatWriteResult Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (asset != null)
            {
                var stream = XmlAsset.ToXml(asset, true);
                return FormatWriteResult.Successful(stream, ContentType);
            }

            return FormatWriteResult.Error();
        }


        /// <summary>
        /// Deserializes an <see cref="IDevice"/> from an XML stream, reporting
        /// success when an entity was produced.
        /// </summary>
        public FormatReadResult<IDevice> CreateDevice(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                content.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            // Read Entity
            var entity = XmlDevice.FromXml(bytes);
            var success = entity != null;

            return new FormatReadResult<IDevice>(entity, success, messages, warnings, errors);
        }

        /// <summary>
        /// Deserializes an <see cref="IComponent"/> from an XML stream,
        /// reporting success when an entity was produced.
        /// </summary>
        public FormatReadResult<IComponent> CreateComponent(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                content.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            // Read Entity
            var entity = XmlComponent.FromXml(bytes);
            var success = entity != null;

            return new FormatReadResult<IComponent>(entity, success, messages, warnings, errors);
        }

        /// <summary>
        /// Deserializes an <see cref="IComposition"/> from an XML stream,
        /// reporting success when an entity was produced.
        /// </summary>
        public FormatReadResult<IComposition> CreateComposition(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                content.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            // Read Entity
            var entity = XmlComposition.FromXml(bytes);
            var success = entity != null;

            return new FormatReadResult<IComposition>(entity, success, messages, warnings, errors);
        }

        /// <summary>
        /// Deserializes an <see cref="IDataItem"/> from an XML stream,
        /// reporting success when an entity was produced.
        /// </summary>
        public FormatReadResult<IDataItem> CreateDataItem(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                content.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            // Read Entity
            var entity = XmlDataItem.FromXml(bytes);
            var success = entity != null;

            return new FormatReadResult<IDataItem>(entity, success, messages, warnings, errors);
        }

        /// <summary>
        /// Deserializes an <see cref="IAsset"/> of the named type from an XML
        /// stream, reporting success when an entity was produced.
        /// </summary>
        public FormatReadResult<IAsset> CreateAsset(string assetType, Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                content.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            // Read Entity
            var entity = XmlAsset.FromXml(assetType, bytes);
            var success = entity != null;

            return new FormatReadResult<IAsset>(entity, success, messages, warnings, errors);
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