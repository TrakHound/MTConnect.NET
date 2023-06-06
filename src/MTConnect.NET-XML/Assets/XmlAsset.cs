// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml
{
    /// <summary>
    /// An Asset XML element is a container type XML element used to organize
    /// information describing an entity that is not a piece of equipment.
    /// </summary>
    public class XmlAsset
    {
        private static readonly Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();
        private static readonly object _lock = new object();


        /// <summary>
        /// The unique identifier for the MTConnect Asset.
        /// </summary>
        [XmlAttribute("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The type for the MTConnect Asset
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The time this MTConnect Asset was last modified.
        /// </summary>
        [XmlAttribute("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The piece of equipments UUID that supplied this data.
        /// </summary>
        [XmlAttribute("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <summary>
        /// This is an optional attribute that is an indicator that the MTConnect
        /// Asset has been removed from the piece of equipment.
        /// </summary>
        [XmlAttribute("removed")]
        public bool Removed { get; set; }

        /// <summary>
        /// An optional element that can contain any descriptive content.
        /// </summary>
        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlIgnore]
        public string Xml { get; set; }


        public static IAsset FromXml(string type, byte[] xmlBytes)
        {
            var asset = Asset.Create(type);
            if (asset != null)
            {
                return FromXml(asset.GetType(), xmlBytes);
            }

            return default;
        }

        public static IAsset FromXml(Type type, byte[] xmlBytes)
        {
            if (type != null && xmlBytes != null && xmlBytes.Length > 0)
            {
                // Check if IAsset
                if (typeof(IAsset).IsAssignableFrom(type))
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

                        using (var textReader = new StringReader(Namespaces.Clear(xml)))
                        {
                            using (var xmlReader = XmlReader.Create(textReader))
                            {
                                // Deserialize and cast to IAsset
                                var asset = (IAsset)serializer.Deserialize(xmlReader);
                                if (asset != null)
                                {
                                    return asset;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }

            return default;
        }


        public static string ToXml(IAsset asset, bool indent = false)
        {
            try
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                using (var writer = new StringWriter())
                {
                    XmlSerializer serializer;
                    lock (_lock)
                    {
                        _serializers.TryGetValue(asset.GetType(), out serializer);
                        if (serializer == null)
                        {
                            serializer = new XmlSerializer(asset.GetType());
                            _serializers.Add(asset.GetType(), serializer);
                        }
                    }

                    serializer.Serialize(writer, asset, namespaces);

                    var xml = writer.ToString();
                    var regexPattern = $@"(<{asset.Type}[.\s\S]*(?:(?:<\/{asset.Type}>)|(?:\/>)))";
                    var regex = new Regex(regexPattern);
                    var match = regex.Match(xml);
                    if (match.Success)
                    {
                        xml = Namespaces.Clear(match.Groups[1].Value);

                        return XmlFunctions.FormatXml(xml, indent, false, true);
                    }
                }
            }
            catch { }

            return null;
        }
    }
}