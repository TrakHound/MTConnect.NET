// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml
{
    /// <summary>
    /// Base XML serialization surrogate for MTConnect assets. Carries the
    /// attributes common to every asset and provides the type-driven
    /// (de)serialization plumbing that dispatches to the concrete asset
    /// surrogate matching an asset's type.
    /// </summary>
    public class XmlAsset
    {
        private static readonly Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();
        private static readonly object _lock = new object();
        private static Dictionary<string, Type> _types;


        /// <summary>
        /// The identifier that uniquely identifies the asset.
        /// </summary>
        [XmlAttribute("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The MTConnect asset type, such as <c>CuttingTool</c> or <c>File</c>.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The time the asset was last modified.
        /// </summary>
        [XmlAttribute("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The UUID of the device that supplied the asset.
        /// </summary>
        [XmlAttribute("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <summary>
        /// Whether the asset has been removed.
        /// </summary>
        [XmlAttribute("removed")]
        public bool Removed { get; set; }

        /// <summary>
        /// The free-form description of the asset.
        /// </summary>
        [XmlAttribute("description")]
        public string Description { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed asset model.
        /// Overridden by each concrete asset surrogate; the base implementation
        /// returns <c>null</c>.
        /// </summary>
        public virtual IAsset ToAsset() { return null; }


        /// <summary>
        /// Deserializes an asset XML document into a strongly-typed
        /// <see cref="IAsset"/>, selecting the surrogate that matches the given
        /// asset type name.
        /// </summary>
        public static IAsset FromXml(string type, byte[] xmlBytes)
        {
            var asset = Asset.Create(type);
            if (asset != null)
            {
                var xmlType = GetAssetType(type);
                if (xmlType == null) xmlType = typeof(XmlUnknownAsset);

                return FromXml(xmlType, xmlBytes);
            }

            return default;
        }

        /// <summary>
        /// Deserializes an asset XML document into a strongly-typed
        /// <see cref="IAsset"/> using the given surrogate
        /// <paramref name="type"/>, caching the <see cref="XmlSerializer"/> per
        /// type. Returns the default value if the input cannot be parsed.
        /// </summary>
        public static IAsset FromXml(Type type, byte[] xmlBytes)
        {
            if (type != null && xmlBytes != null && xmlBytes.Length > 0)
            {
                // Check if XmlAsset
                if (typeof(XmlAsset).IsAssignableFrom(type))
                {
                    try
                    {
                        var xml = Encoding.UTF8.GetString(xmlBytes);
                        xml = xml.Trim();

                        // Create an XmlSerializer using the specified Type
                        XmlSerializer serializer;
                        lock (_lock)
                        {
                            _serializers.TryGetValue(type, out serializer);
                            if (serializer == null)
                            {
                                serializer = new XmlSerializer(type);
                                _serializers.Add(type, serializer);
                            }
                        }

                        xml = Namespaces.Clear(xml);

                        using (var textReader = new StringReader(xml))
                        {
                            using (var xmlReader = XmlReader.Create(textReader))
                            {
                                // Deserialize and cast to IAsset
                                var asset = (XmlAsset)serializer.Deserialize(xmlReader);
                                if (asset != null)
                                {
                                    return asset.ToAsset();
                                }
                            }
                        }
                    }
                    catch { }
                }
            }

            return default;
        }

        /// <summary>
        /// Writes the given <see cref="IAsset"/> to <paramref name="writer"/> by
        /// dispatching to the <c>WriteXml</c> method of the concrete surrogate
        /// that matches the asset's type.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IAsset asset)
        {
            try
            {
                var assetType = GetAssetType(asset.Type);
                if (assetType != null)
                {
                    var writeXmlMethod = assetType.GetMethod("WriteXml");
                    if (writeXmlMethod != null)
                    {
                        writeXmlMethod.Invoke(null, new object[] { writer, asset });
                    }
                    else
                    {
                        // Write Unknown ?
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Writes the attributes common to every asset (<c>assetId</c>,
        /// <c>deviceUuid</c>, <c>timestamp</c>, and <c>removed</c>) onto the
        /// current element. Intended to be called by concrete surrogates after
        /// opening their root element.
        /// </summary>
        public static void WriteCommonXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null)
            {
                // Write Properties
                writer.WriteAttributeString("assetId", asset.AssetId);
                writer.WriteAttributeString("deviceUuid", asset.DeviceUuid);
                writer.WriteAttributeString("timestamp", asset.Timestamp.ToString("o"));
                if (asset.Removed) writer.WriteAttributeString("removed", "true");
            }
        }


        /// <summary>
        /// Serializes the given <see cref="IAsset"/> to a <see cref="Stream"/>
        /// of XML, optionally indented, returning <c>null</c> if serialization
        /// fails.
        /// </summary>
        public static Stream ToXml(IAsset asset, bool indent = false)
        {
            try
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                using (var stream = new MemoryStream())
                {
                    // Set the XmlWriterSettings to use
                    var xmlWriterSettings = indent ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                    // Use XmlWriter to write XML to stream
                    using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        var assetType = GetAssetType(asset.Type);
                        if (assetType != null)
                        {
                            var writeXmlMethod = assetType.GetMethod("WriteXml");
                            if (writeXmlMethod != null)
                            {
                                writeXmlMethod.Invoke(null, new object[] { xmlWriter, asset });
                            }
                            else
                            {
                                // Write Unknown ?
                            }
                        }

                        xmlWriter.Flush();

                        return stream;
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Resolves the concrete surrogate <see cref="Type"/> registered for
        /// the given asset type name, or <c>null</c> when none is known. The
        /// surrogate map is built once on first use.
        /// </summary>
        public static Type GetAssetType(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    if (_types.TryGetValue(type, out Type t))
                    {
                        return t;
                    }
                }
            }

            return null;
        }

        private static Dictionary<string, Type> GetAllTypes()
        {
            var assemblies = Assemblies.Get();
            if (!assemblies.IsNullOrEmpty())
            {
                var types = assemblies
                    .SelectMany(
                        x => x.GetMatchingTypesInAssembly(
                            t => !t.IsInterface && !t.IsAbstract));

                if (!types.IsNullOrEmpty())
                {
                    var objs = new Dictionary<string, Type>();
                    var regex = new Regex("Xml(.+)Asset");

                    foreach (var type in types)
                    {
                        if (type.Name != "XmlAsset" && type.Name != "XmlUnknownAsset" && type.Name != "XmlAsset`1")
                        {
                            var match = regex.Match(type.Name);
                            if (match.Success && match.Groups.Count > 1)
                            {
                                var key = match.Groups[1].Value;
                                if (!objs.ContainsKey(key)) objs.Add(key, type);
                            }
                        }
                    }

                    return objs;
                }
            }

            return new Dictionary<string, Type>();
        }
    }
}