// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets
{
    /// <summary>
    /// An Asset XML element is a container type XML element used to organize
    /// information describing an entity that is not a piece of equipment.
    /// </summary>
    public class XmlAsset
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlAsset));


        /// <summary>
        /// The unique identifier for the MTConnect Asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The type for the MTConnect Asset
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The time this MTConnect Asset was last modified.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The piece of equipments UUID that supplied this data.
        /// </summary>
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <summary>
        /// This is an optional attribute that is an indicator that the MTConnect
        /// Asset has been removed from the piece of equipment.
        /// </summary>
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        /// <summary>
        /// An optional element that can contain any descriptive content.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [XmlIgnore]
        public string Xml { get; set; }


        public static IAsset FromXml(Type type, string xml)
        {
            if (type != null && !string.IsNullOrEmpty(xml))
            {
                // Check if IAsset
                if (typeof(IAsset).IsAssignableFrom(type))
                {
                    try
                    {
                        xml = xml.Trim();

                        // Create an XmlSerializer using the specified Type
                        var serializer = new XmlSerializer(type);

                        using (var textReader = new StringReader(Namespaces.Clear(xml)))
                        {
                            using (var xmlReader = XmlReader.Create(textReader))
                            {
                                // Deserialize and cast to IAsset
                                var asset = (IAsset)serializer.Deserialize(xmlReader);
                                if (asset != null)
                                {
                                    asset.Xml = xml;
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
                    var serializer = new XmlSerializer(asset.GetType());
                    serializer.Serialize(writer, asset, namespaces);

                    var xml = writer.ToString();
                    var regexPattern = $@"(<{asset.Type}[.\s\S]*(?:(?:<\/{asset.Type}>)|(?:\/>)))";
                    var regex = new Regex(regexPattern);
                    var match = regex.Match(xml);
                    if (match.Success)
                    {
                        return Namespaces.Clear(match.Groups[1].Value);
                    }
                }
            }
            catch { }

            return null;
        }
    }

    /// <summary>
    /// An Asset XML element is a container type XML element used to organize
    /// information describing an entity that is not a piece of equipment.
    /// </summary>
    public class XmlAsset<T> where T : IAsset
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(T));


        /// <summary>
        /// The unique identifier for the MTConnect Asset.
        /// </summary>
        [XmlAttribute("assetId")]
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The type for the MTConnect Asset
        /// </summary>
        [XmlIgnore]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The time this MTConnect Asset was last modified.
        /// </summary>
        [XmlAttribute("timestamp")]
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The piece of equipments UUID that supplied this data.
        /// </summary>
        [XmlAttribute("deviceUuid")]
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <summary>
        /// This is an optional attribute that is an indicator that the MTConnect
        /// Asset has been removed from the piece of equipment.
        /// </summary>
        [XmlAttribute("removed")]
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool RemovedSpecified => Removed;

        /// <summary>
        /// An optional element that can contain any descriptive content.
        /// </summary>
        [XmlElement("Description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public string Xml { get; set; }


        public static T FromXml(string xml)
        {
            return (T)XmlAsset.FromXml(typeof(T), xml);
        }

        public string ToXml(bool indent = false)
        {
            try
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                using (var writer = new StringWriter())
                {
                    _serializer.Serialize(writer, this, namespaces);

                    var xml = writer.ToString();
                    var regexPattern = $@"(<{Type}[.\s\S]*(?:(?:<\/{Type}>)|(?:\/>)))";
                    var regex = new Regex(regexPattern);
                    var match = regex.Match(xml);
                    if (match.Success)
                    {
                        return Namespaces.Clear(match.Groups[1].Value);
                    }
                }
            }
            catch { }

            return null;
        }
    }
}
