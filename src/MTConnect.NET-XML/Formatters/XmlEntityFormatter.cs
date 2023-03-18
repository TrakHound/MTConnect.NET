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


        public string Format(IDevice device, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (device != null)
            {
                try
                {
                    using (var writer = new StringWriter())
                    {
                        // Use XmlWriter to write XML to stream
                        var xmlWriter = XmlWriter.Create(writer, XmlFunctions.XmlWriterSettings);
                        XmlDevice.WriteXml(xmlWriter, device);
                        return writer.ToString();
                    }
                }
                catch { }
            }

            return null;
        }

        public string Format(IComponent component, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (component != null)
            {
                try
                {
                    using (var writer = new StringWriter())
                    {
                        // Use XmlWriter to write XML to stream
                        var xmlWriter = XmlWriter.Create(writer, XmlFunctions.XmlWriterSettings);
                        XmlComponent.WriteXml(xmlWriter, component);
                        return writer.ToString();
                    }
                }
                catch { }
            }

            return null;
        }

        public string Format(IComposition composition, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (composition != null)
            {
                try
                {
                    using (var writer = new StringWriter())
                    {
                        // Use XmlWriter to write XML to stream
                        var xmlWriter = XmlWriter.Create(writer, XmlFunctions.XmlWriterSettings);
                        XmlComposition.WriteXml(xmlWriter, composition);
                        return writer.ToString();
                    }
                }
                catch { }
            }

            return null;
        }

        public string Format(IDataItem dataItem, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (dataItem != null)
            {
                try
                {
                    using (var writer = new StringWriter())
                    {
                        // Use XmlWriter to write XML to stream
                        var xmlWriter = XmlWriter.Create(writer, XmlFunctions.XmlWriterSettings);
                        XmlDataItem.WriteXml(xmlWriter, dataItem);
                        return writer.ToString();
                    }
                }
                catch { }
            }

            return null;
        }

        public string Format(IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (observation != null)
            {
                try
                {
                    using (var writer = new StringWriter())
                    {
                        // Use XmlWriter to write XML to stream
                        var xmlWriter = XmlWriter.Create(writer, XmlFunctions.XmlWriterSettings);
                        XmlObservation.WriteXml(xmlWriter, observation);
                        return writer.ToString();
                    }
                }
                catch { }
            }

            return null;
        }

        public string Format(IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (!observations.IsNullOrEmpty())
            {
                var s = "";

                foreach (var observation in observations)
                {
                    try
                    {
                        using (var writer = new StringWriter())
                        {
                            // Use XmlWriter to write XML to stream
                            var xmlWriter = XmlWriter.Create(writer, XmlFunctions.XmlWriterSettings);
                            XmlObservation.WriteXml(xmlWriter, observation);
                            var x = writer.ToString();
                            if (!string.IsNullOrEmpty(x))
                            {
                                if (string.IsNullOrEmpty(x)) s += x;
                                else s += "\r\n" + x;
                            }
                        }
                    }
                    catch { }
                }

                return !string.IsNullOrEmpty(s) ? s : null;
            }

            return null;
        }

        public string Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (asset != null)
            {
                return XmlAsset.ToXml(asset, true);
            }

            return null;
        }


        public FormattedEntityReadResult<IDevice> CreateDevice(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read Document
            var entity = XmlDevice.FromXml(content);
            var success = entity != null;

            return new FormattedEntityReadResult<IDevice>(entity, success, messages, warnings, errors);
        }

        public FormattedEntityReadResult<IAsset> CreateAsset(string assetType, byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            // Read Document
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