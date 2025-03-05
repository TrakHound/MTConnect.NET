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
    public class XmlEntityFormatter : IEntityFormatter
    {
        public string Id => "XML";

        public string ContentType => "application/xml";


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

        public FormatWriteResult Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (asset != null)
            {
                var stream = XmlAsset.ToXml(asset, true);
                return FormatWriteResult.Successful(stream, ContentType);
            }

            return FormatWriteResult.Error();
        }


        public FormatReadResult<IDevice> CreateDevice(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            IDevice entity = null;
            bool success = false;

            try
            {
                //byte[] bytes;
                //using (var memoryStream = new MemoryStream())
                //{
                //    content.CopyTo(memoryStream);
                //    bytes = memoryStream.ToArray();
                //}

                // Read Document
                //document = XmlDevicesResponseDocument.FromXml(bytes);
                //document = XmlDevicesResponseDocument.FromXml(content);
                //success = document != null;

                if (content != null && content.Length > 0)
                {
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("x", "http://example.com/namespace");

                    //var nameTable = new NameTable();
                    //var namespaceManager = new XmlNamespaceManager(nameTable);
                    //namespaceManager.AddNamespace("x", "http://www.example.com/data");

                    //XmlDocument doc = new XmlDocument();
                    //doc.LoadXml(xmlString);
                    //doc.PreserveWhitespace = true;

                    //var document = new XmlDocument();
                    ////document.XmlResolver = new XmlNamespaceResolver();
                    //document.Load(content);
                    //var nodeReader = new XmlNodeReader(document);

                    //XmlNode root = document.DocumentElement;

                    //string xpath = "//Element";
                    //XmlNodeList nodes = root.SelectNodes(xpath, namespaceManager);

                    var readerSettings = new XmlReaderSettings()
                    {
                        IgnoreWhitespace = true,
                        IgnoreComments = true,
                        IgnoreProcessingInstructions = true,
                        //XmlResolver = new XmlNamespaceResolver()
                        //DtdProcessing = DtdProcessing.Ignore,
                        //NameTable = nameTable
                    };

                    //using (var xmlReader = XmlReader.Create(nodeReader, readerSettings))
                    //{
                    //    entity = XmlDevicesResponseDocument.ReadDeviceXml(xmlReader);
                    //    success = entity != null;
                    //}

                    using (var xmlReader = XmlReader.Create(content, readerSettings))
                    {
                        entity = XmlDevicesResponseDocument.ReadDeviceXml(xmlReader);
                        success = entity != null;
                    }

                    //var readerSettings = new XmlReaderSettings()
                    //{
                    //    CheckCharacters = false,
                    //    ConformanceLevel = ConformanceLevel.Document,
                    //    DtdProcessing = DtdProcessing.Ignore,
                    //    IgnoreComments = true,
                    //    IgnoreProcessingInstructions = true,
                    //    IgnoreWhitespace = true,
                    //    ValidationType = ValidationType.None
                    //};

                    //using (var xmlReader = XmlReader.Create(content, readerSettings))
                    //{
                    //    var namespaceManager = new XmlNamespaceManager(xmlReader.NameTable);
                    //    namespaceManager.AddNamespace("x", "http://www.example.com/data");

                    //    entity = XmlDevicesResponseDocument.ReadDeviceXml(xmlReader);
                    //    success = entity != null;
                    //}

                    //using (var streamReader = new StreamReader(content))
                    //{
                    //    using (var xmlReader = new XmlExtendableReader(streamReader, settings, true))
                    //    {
                    //        entity = XmlDevicesResponseDocument.ReadDeviceXml(xmlReader);
                    //        success = entity != null;
                    //    }
                    //}


                    //using (var streamReader = new StreamReader(content))
                    //{
                    //    using (var xmlReader = new IgnoreNamespaceXmlTextReader(streamReader))
                    //    {
                    //        entity = XmlDevicesResponseDocument.ReadDeviceXml(xmlReader);
                    //        success = entity != null;
                    //    }
                    //}

                    //var readerSettings = new XmlReaderSettings();
                    //readerSettings.XmlResolver = null;
                    //readerSettings.DtdProcessing = DtdProcessing.Parse;
                    ////readerSettings.XmlResolver = new XmlNamespaceResolver();

                    //using (var streamReader = new StreamReader(content))
                    //{
                    //    //using (var textReader = new TextReader(content, readerSettings))
                    //    //using (var xmlReader = XmlReader.Create(content, readerSettings))
                    //    using (var xmlReader = new IgnoreNamespaceXmlTextReader(streamReader))
                    //    {
                    //        entity = XmlDevicesResponseDocument.ReadDeviceXml(xmlReader);
                    //        success = entity != null;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                messages.Add(ex.Message);

                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    messages.Add(innerException.Message);
                    innerException = innerException.InnerException;
                }
            }

            return new FormatReadResult<IDevice>(entity, success, messages, warnings, errors);

            //byte[] bytes;
            //using (var memoryStream = new MemoryStream())
            //{
            //    content.CopyTo(memoryStream);
            //    bytes = memoryStream.ToArray();
            //}

            //// Read Entity
            //var entity = XmlDevice.FromXml(bytes);
            //var success = entity != null;

            //return new FormatReadResult<IDevice>(entity, success, messages, warnings, errors);
        }

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