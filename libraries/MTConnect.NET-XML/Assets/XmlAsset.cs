// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private static readonly Dictionary<Type, Action<XmlWriter, IAsset>> _writers = new Dictionary<Type, Action<XmlWriter, IAsset>>();
        private static readonly object _lock = new object();
        private static readonly Lazy<Dictionary<string, Type>> _types = new Lazy<Dictionary<string, Type>>(GetAllTypes);
        private static readonly Regex _typeNameRegex = new Regex("^Xml(.+)Asset$");

        // The MTConnect version of the document being written by WriteXml on this thread (null when unknown)
        [ThreadStatic]
        private static Version _writeVersion;

        // Settings for the discarding writer each asset is first written to (see WriteXml)
        private static readonly XmlWriterSettings _rehearsalWriterSettings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };


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
        /// The asset hash (an asset attribute in MTConnect 2.2 through 2.5).
        /// </summary>
        [XmlAttribute("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// The free-form description of the asset, read from its <c>Description</c> element.
        /// </summary>
        [XmlIgnore]
        public string Description { get; set; }

        /// <summary>
        /// The <c>Description</c> element as read; sets <see cref="Description"/> to its text, or to its
        /// inner markup when it holds child elements. Used by the XML serializer only.
        /// </summary>
        [XmlAnyElement("Description", Namespace = "")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public XmlElement DescriptionElement
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    var hasElements = value.ChildNodes.OfType<XmlNode>().Any(o => o.NodeType == XmlNodeType.Element);
                    Description = (hasElements ? value.InnerXml : value.InnerText)?.Trim();
                }
                else
                {
                    Description = null;
                }
            }
        }


        /// <summary>
        /// Converts this surrogate into the strongly-typed asset model.
        /// Overridden by each concrete asset surrogate; the base implementation
        /// returns <c>null</c>.
        /// </summary>
        public virtual IAsset ToAsset() { return null; }

        /// <summary>
        /// Copies the properties common to every asset onto <paramref name="asset"/>; called by the
        /// concrete surrogates' <see cref="ToAsset"/>.
        /// </summary>
        protected void CopyCommonProperties(Asset asset)
        {
            if (asset != null)
            {
                asset.AssetId = AssetId;
                asset.Timestamp = Timestamp;
                asset.DeviceUuid = DeviceUuid;
                asset.Removed = Removed;
                asset.Hash = !string.IsNullOrEmpty(Hash) ? Hash : null;
                asset.Description = !string.IsNullOrEmpty(Description) ? Description : null;
            }
        }


        /// <summary>
        /// Deserializes an asset XML document into a strongly-typed
        /// <see cref="IAsset"/>, selecting the surrogate that matches the given
        /// asset type name. Returns <c>null</c> if the input cannot be parsed;
        /// <see cref="TryFromXml(string, byte[], out IAsset, out string)"/> also reports why.
        /// </summary>
        public static IAsset FromXml(string type, byte[] xmlBytes)
        {
            TryFromXml(type, xmlBytes, out var asset, out _);
            return asset;
        }

        /// <summary>
        /// Deserializes an asset XML document into a strongly-typed
        /// <see cref="IAsset"/> using the given surrogate
        /// <paramref name="type"/>. Returns <c>null</c> if the input cannot be parsed.
        /// </summary>
        public static IAsset FromXml(Type type, byte[] xmlBytes)
        {
            TryFromXml(type, xmlBytes, out var asset, out _);
            return asset;
        }

        /// <summary>
        /// Deserializes an asset XML document, selecting the surrogate that matches the given asset
        /// type name. On failure returns <c>false</c> with the reason in <paramref name="error"/>.
        /// </summary>
        public static bool TryFromXml(string type, byte[] xmlBytes, out IAsset asset, out string error)
        {
            var xmlType = GetAssetType(type);
            if (xmlType == null)
            {
                asset = null;
                error = $"No XML serializer is available for asset type '{type}'";
                return false;
            }

            return TryFromXml(xmlType, xmlBytes, out asset, out error);
        }

        /// <summary>
        /// Deserializes an asset XML document using the given surrogate <paramref name="type"/>,
        /// caching the <see cref="XmlSerializer"/> per type. On failure returns <c>false</c> with the
        /// reason in <paramref name="error"/>.
        /// </summary>
        public static bool TryFromXml(Type type, byte[] xmlBytes, out IAsset asset, out string error)
        {
            asset = null;
            error = null;

            if (type == null || !typeof(XmlAsset).IsAssignableFrom(type))
            {
                error = $"'{type}' is not an XML asset type";
                return false;
            }

            if (xmlBytes == null || xmlBytes.Length < 1)
            {
                error = "The asset XML is empty";
                return false;
            }

            try
            {
                var serializer = GetSerializer(type);

                // Read with the MTConnect namespaces ignored, rather than removing namespace declarations
                // from the text (which broke prefixed content and stripped an embedded QIF document's namespace)
                using (var xmlReader = MTConnectNamespaceXmlReader.Create(xmlBytes))
                {
                    var xmlAsset = (XmlAsset)serializer.Deserialize(xmlReader);
                    asset = xmlAsset?.ToAsset();
                }

                if (asset == null) error = $"The XML did not produce a {type.Name} asset";
            }
            catch (Exception ex)
            {
                // XmlSerializer reports the position in the exception and the cause in its inner exception
                error = ex.InnerException != null ? $"{ex.Message} {ex.InnerException.Message}" : ex.Message;
            }

            return asset != null;
        }

        private static XmlSerializer GetSerializer(Type type)
        {
            lock (_lock)
            {
                if (!_serializers.TryGetValue(type, out var serializer))
                {
                    serializer = new XmlSerializer(type);
                    _serializers.Add(type, serializer);
                }

                return serializer;
            }
        }


        /// <summary>
        /// Writes the given <see cref="IAsset"/> to <paramref name="writer"/> by
        /// dispatching to the <c>WriteXml</c> method of the concrete surrogate
        /// that matches the asset's type, using the MTConnect 2.6+ element layout.
        /// Writes nothing when the type has no surrogate or the asset cannot be written.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IAsset asset)
        {
            WriteXml(writer, asset, null);
        }

        /// <summary>
        /// Writes the given <see cref="IAsset"/> to <paramref name="writer"/> for a document of the given
        /// MTConnect version, which decides where (or whether) elements whose position changed between
        /// versions, such as <c>Description</c>, are written; <c>null</c> uses the 2.6+ layout.
        /// Writes nothing when the type has no surrogate or the asset cannot be written.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IAsset asset, Version mtconnectVersion)
        {
            if (writer == null || asset == null) return;

            var write = GetWriter(asset.Type);
            if (write == null) return;

            var previousVersion = _writeVersion;
            _writeVersion = mtconnectVersion;
            try
            {
                // Write to a discarding writer first: a surrogate that throws part-way (an asset whose Type
                // does not match its class, an invalid character, ...) would otherwise leave an unclosed
                // element in the real document, and every later asset would be written inside it
                try
                {
                    using (var rehearsal = XmlWriter.Create(Stream.Null, _rehearsalWriterSettings))
                    {
                        write(rehearsal, asset);
                    }
                }
                catch
                {
                    return;
                }

                write(writer, asset);
            }
            finally
            {
                _writeVersion = previousVersion;
            }
        }

        /// <summary>
        /// Writes the attributes common to every asset (<c>assetId</c>,
        /// <c>deviceUuid</c>, <c>timestamp</c>, <c>removed</c>, and <c>hash</c>) onto the
        /// current element, omitting those that are not set. Intended to be called by
        /// concrete surrogates after opening their root element.
        /// </summary>
        public static void WriteCommonXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null)
            {
                // Write Properties
                if (!string.IsNullOrEmpty(asset.AssetId)) writer.WriteAttributeString("assetId", asset.AssetId);
                if (!string.IsNullOrEmpty(asset.DeviceUuid)) writer.WriteAttributeString("deviceUuid", asset.DeviceUuid);
                writer.WriteAttributeString("timestamp", FormatTimestamp(asset.Timestamp));
                if (asset.Removed) writer.WriteAttributeString("removed", "true");
                if (!string.IsNullOrEmpty(asset.Hash)) writer.WriteAttributeString("hash", asset.Hash);
            }
        }

        /// <summary>
        /// Writes the elements common to every asset that MTConnect 2.6 and later place last
        /// (currently <c>Description</c>). Intended to be called by concrete surrogates as the last
        /// content of their element; writes nothing for a document of an earlier version.
        /// </summary>
        public static void WriteCommonElementsXml(XmlWriter writer, IAsset asset)
        {
            if (_writeVersion == null || _writeVersion >= MTConnectVersions.Version26)
            {
                WriteDescriptionXml(writer, asset);
            }
        }

        /// <summary>
        /// Writes the <c>Description</c> element where versions before 2.6 place it, as the first child
        /// of a <c>CuttingTool</c> or <c>CuttingToolArchetype</c> (the only assets those versions give
        /// one). Intended to be called by those surrogates right after their attributes; writes nothing
        /// for a 2.6+ document or when no version is known.
        /// </summary>
        public static void WriteLeadingCommonElementsXml(XmlWriter writer, IAsset asset)
        {
            if (_writeVersion != null && _writeVersion < MTConnectVersions.Version26)
            {
                WriteDescriptionXml(writer, asset);
            }
        }

        private static void WriteDescriptionXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null && !string.IsNullOrEmpty(asset.Description))
            {
                writer.WriteElementString("Description", asset.Description);
            }
        }

        // Timestamps are UTC; one of unspecified kind is taken to be UTC already
        private static string FormatTimestamp(DateTime timestamp)
        {
            if (timestamp.Kind == DateTimeKind.Local) timestamp = timestamp.ToUniversalTime();
            else if (timestamp.Kind == DateTimeKind.Unspecified) timestamp = DateTime.SpecifyKind(timestamp, DateTimeKind.Utc);

            return XmlFunctions.GetTimestamp(timestamp);
        }


        // The writer must not close the stream it writes to, since the stream is returned
        private static XmlWriterSettings CloseOutputDisabled(XmlWriterSettings settings)
        {
            var copy = settings.Clone();
            copy.CloseOutput = false;
            return copy;
        }

        /// <summary>
        /// Serializes the given <see cref="IAsset"/> to a <see cref="Stream"/>
        /// of XML, optionally indented, returning <c>null</c> when the asset's type
        /// has no surrogate or serialization fails.
        /// </summary>
        public static Stream ToXml(IAsset asset, bool indent = false)
        {
            var write = asset != null ? GetWriter(asset.Type) : null;
            if (write == null) return null;

            try
            {
                // Not disposed here: it is returned to the caller
                var stream = new MemoryStream();

                // Set the XmlWriterSettings to use
                var xmlWriterSettings = indent ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                // Use XmlWriter to write XML to stream
                using (var xmlWriter = XmlWriter.Create(stream, CloseOutputDisabled(xmlWriterSettings)))
                {
                    write(xmlWriter, asset);
                    xmlWriter.Flush();
                }

                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
            catch { }

            return null;
        }

        // The concrete surrogate's static WriteXml(XmlWriter, IAsset), bound once per type
        private static Action<XmlWriter, IAsset> GetWriter(string type)
        {
            var xmlType = GetAssetType(type);
            if (xmlType == null) return null;

            lock (_lock)
            {
                if (_writers.TryGetValue(xmlType, out var cached)) return cached;
            }

            Action<XmlWriter, IAsset> write = null;
            var method = xmlType.GetMethod(
                "WriteXml",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
                null,
                new[] { typeof(XmlWriter), typeof(IAsset) },
                null);

            if (method != null && method.ReturnType == typeof(void))
            {
                write = (Action<XmlWriter, IAsset>)Delegate.CreateDelegate(typeof(Action<XmlWriter, IAsset>), method);
            }

            lock (_lock)
            {
                _writers[xmlType] = write;
            }

            return write;
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
                if (_types.Value.TryGetValue(type, out Type t))
                {
                    return t;
                }
            }

            return null;
        }

        private static Dictionary<string, Type> GetAllTypes()
        {
            var objs = new Dictionary<string, Type>();

            // This library's surrogates first, so a same-named type elsewhere cannot displace one
            var builtIn = typeof(XmlAsset).Assembly;
            var assemblies = new[] { builtIn }.Concat((Assemblies.Get() ?? Enumerable.Empty<Assembly>()).Where(o => o != builtIn));

            var types = assemblies.SelectMany(
                x => x.GetMatchingTypesInAssembly(
                    t => typeof(XmlAsset).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(XmlAsset) && t != typeof(XmlUnknownAsset)));

            foreach (var type in types)
            {
                var match = _typeNameRegex.Match(type.Name);
                if (match.Success)
                {
                    var key = match.Groups[1].Value;
                    if (!objs.ContainsKey(key)) objs.Add(key, type);
                }
            }

            return objs;
        }
    }
}
