// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
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


        public FormattedEntityWriteResult Format(IDevice device, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (device != null)
            {
                var indentOuput = GetFormatterOption<bool>(options, "indentOutput");
                var settings = indentOuput ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        // Use XmlWriter to write XML to stream
                        using (var xmlWriter = XmlWriter.Create(outputStream, settings))
                        {
                            XmlDevice.WriteXml(xmlWriter, device);
                            xmlWriter.Flush();
                            
                            var bytes = outputStream.ToArray();
                            if (bytes != null)
                            {
                                return FormattedEntityWriteResult.Successful(bytes, ContentType);
                            }
                        }
                    }

                    //using (var writer = new StringWriter())
                    //{
                    //    // Use XmlWriter to write XML to stream
                    //    using (var xmlWriter = XmlWriter.Create(writer, settings))
                    //    {
                    //        XmlDevice.WriteXml(xmlWriter, device);
                    //        xmlWriter.Flush();
                    //        return writer.ToString();
                    //    }
                    //}
                }
                catch { }
            }

            return FormattedEntityWriteResult.Error();
        }

        //public FormattedEntityWriteResult Format(IComponent component, IEnumerable<KeyValuePair<string, string>> options = null)
        //{
        //    if (component != null)
        //    {
        //        try
        //        {
        //            using (var writer = new StringWriter())
        //            {
        //                // Use XmlWriter to write XML to stream
        //                var xmlWriter = XmlWriter.Create(writer, XmlFunctions.XmlWriterSettings);
        //                XmlComponent.WriteXml(xmlWriter, component);
        //                xmlWriter.Flush();
        //                return writer.ToString();
        //            }
        //        }
        //        catch { }
        //    }

        //    return FormattedEntityWriteResult.Error();
        //}

        //public FormattedEntityWriteResult Format(IComposition composition, IEnumerable<KeyValuePair<string, string>> options = null)
        //{
        //    if (composition != null)
        //    {
        //        try
        //        {
        //            using (var writer = new StringWriter())
        //            {
        //                // Use XmlWriter to write XML to stream
        //                var xmlWriter = XmlWriter.Create(writer, XmlFunctions.XmlWriterSettings);
        //                XmlComposition.WriteXml(xmlWriter, composition);
        //                xmlWriter.Flush();
        //                return writer.ToString();
        //            }
        //        }
        //        catch { }
        //    }

        //    return FormattedEntityWriteResult.Error();
        //}

        //public FormattedEntityWriteResult Format(IDataItem dataItem, IEnumerable<KeyValuePair<string, string>> options = null)
        //{
        //    if (dataItem != null)
        //    {
        //        try
        //        {
        //            var indentOuput = GetFormatterOption<bool>(options, "indentOutput");

        //            using (var writer = new StringWriter())
        //            {
        //                // Use XmlWriter to write XML to stream
        //                using (var xmlWriter = XmlWriter.Create(writer, XmlFunctions.XmlWriterSettings))
        //                {
        //                    XmlDataItem.WriteXml(xmlWriter, dataItem);
        //                    xmlWriter.Flush();
        //                    return XmlFunctions.FormatXml(writer.ToString(), indentOuput, false, true);
        //                }
        //            }
        //        }
        //        catch { }
        //    }

        //    return FormattedEntityWriteResult.Error();
        //}

        public FormattedEntityWriteResult Format(IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (observation != null)
            {
                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        // Use XmlWriter to write XML to stream
                        var xmlWriter = XmlWriter.Create(outputStream, XmlFunctions.XmlWriterSettings);
                        XmlObservation.WriteXml(xmlWriter, observation);

                        var bytes = outputStream.ToArray();
                        if (bytes != null)
                        {
                            return FormattedEntityWriteResult.Successful(bytes, ContentType);
                        }
                    }


                    //using (var writer = new StringWriter())
                    //{
                    //    // Use XmlWriter to write XML to stream
                    //    var xmlWriter = XmlWriter.Create(writer, XmlFunctions.XmlWriterSettings);
                    //    XmlObservation.WriteXml(xmlWriter, observation);

                    //    writer.

                    //    //return writer.ToString();
                    //}
                }
                catch { }
            }

            return FormattedEntityWriteResult.Error();
        }

        public FormattedEntityWriteResult Format(IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (!observations.IsNullOrEmpty())
            {
                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        foreach (var observation in observations)
                        {
                            // Use XmlWriter to write XML to stream
                            var xmlWriter = XmlWriter.Create(outputStream, XmlFunctions.XmlWriterSettings);
                            XmlObservation.WriteXml(xmlWriter, observation);
                        }

                        var bytes = outputStream.ToArray();
                        if (bytes != null)
                        {
                            return FormattedEntityWriteResult.Successful(bytes, ContentType);
                        }
                    }
                }
                catch { }
            }

            return FormattedEntityWriteResult.Error();
        }

        public FormattedEntityWriteResult Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (asset != null)
            {
                var bytes = XmlAsset.ToXml(asset, true);
                if (bytes != null)
                {
                    return FormattedEntityWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormattedEntityWriteResult.Error();
        }


        public FormattedEntityReadResult<IDevice> CreateDevice(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read Entity
            var entity = XmlDevice.FromXml(content);
            var success = entity != null;

            return new FormattedEntityReadResult<IDevice>(entity, success, messages, warnings, errors);
        }

        public FormattedEntityReadResult<IComponent> CreateComponent(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read Entity
            var entity = XmlComponent.FromXml(content);
            var success = entity != null;

            return new FormattedEntityReadResult<IComponent>(entity, success, messages, warnings, errors);
        }

        public FormattedEntityReadResult<IComposition> CreateComposition(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read Entity
            var entity = XmlComposition.FromXml(content);
            var success = entity != null;

            return new FormattedEntityReadResult<IComposition>(entity, success, messages, warnings, errors);
        }

        public FormattedEntityReadResult<IDataItem> CreateDataItem(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read Entity
            var entity = XmlDataItem.FromXml(content);
            var success = entity != null;

            return new FormattedEntityReadResult<IDataItem>(entity, success, messages, warnings, errors);
        }

        public FormattedEntityReadResult<IAsset> CreateAsset(string assetType, byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read Entity
            var entity = XmlAsset.FromXml(assetType, content);
            var success = entity != null;

            return new FormattedEntityReadResult<IAsset>(entity, success, messages, warnings, errors);
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