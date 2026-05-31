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
    /// XML serialization surrogate used when an asset element is encountered
    /// whose <c>assetType</c> is not recognized by the strongly-typed model.
    /// The raw XML is retained so the asset can still be round-tripped or
    /// resolved later once its concrete type is known.
    /// </summary>
    public class XmlUnknownAsset : XmlAsset
    {
        /// <summary>
        /// The raw XML fragment of the unrecognized asset element, preserved
        /// verbatim so no information is lost during deserialization.
        /// </summary>
        [XmlIgnore]
        public string Xml { get; set; }


        /// <summary>
        /// Attempts to resolve the asset to its concrete type by name and
        /// deserialize the supplied XML bytes; returns <c>null</c> when the
        /// <paramref name="type"/> name is not a registered asset type.
        /// </summary>
        public static new IAsset FromXml(string type, byte[] xmlBytes)
        {
            var asset = Asset.Create(type);
            if (asset != null)
            {
                return FromXml(asset.GetType(), xmlBytes);
            }

            return default;
        }

        //public static IAsset FromXml(Type type, byte[] xmlBytes)
        //{
        //    if (type != null && xmlBytes != null && xmlBytes.Length > 0)
        //    {
        //        // Check if IAsset
        //        if (typeof(IAsset).IsAssignableFrom(type))
        //        {
        //            try
        //            {
        //                var xml = Encoding.UTF8.GetString(xmlBytes);
        //                xml = xml.Trim();

        //                // Create an XmlSerializer using the specified Type
        //                XmlSerializer serializer;
        //                lock (_lock)
        //                {
        //                    _serializers.TryGetValue(type, out serializer);
        //                    if (serializer == null)
        //                    {
        //                        serializer = new XmlSerializer(type);
        //                        _serializers.Add(type, serializer);
        //                    }
        //                }                

        //                using (var textReader = new StringReader(Namespaces.Clear(xml)))
        //                {
        //                    using (var xmlReader = XmlReader.Create(textReader))
        //                    {
        //                        // Deserialize and cast to IAsset
        //                        var asset = (IAsset)serializer.Deserialize(xmlReader);
        //                        if (asset != null)
        //                        {
        //                            return asset;
        //                        }
        //                    }
        //                }
        //            }
        //            catch { }
        //        }
        //    }

        //    return default;
        //}


        //public static string ToXml(IAsset asset, bool indent = false)
        //{
        //    try
        //    {
        //        var namespaces = new XmlSerializerNamespaces();
        //        namespaces.Add("", "");

        //        using (var writer = new StringWriter())
        //        {
        //            XmlSerializer serializer;
        //            lock (_lock)
        //            {
        //                _serializers.TryGetValue(asset.GetType(), out serializer);
        //                if (serializer == null)
        //                {
        //                    serializer = new XmlSerializer(asset.GetType());
        //                    _serializers.Add(asset.GetType(), serializer);
        //                }
        //            }

        //            serializer.Serialize(writer, asset, namespaces);

        //            var xml = writer.ToString();
        //            var regexPattern = $@"(<{asset.Type}[.\s\S]*(?:(?:<\/{asset.Type}>)|(?:\/>)))";
        //            var regex = new Regex(regexPattern);
        //            var match = regex.Match(xml);
        //            if (match.Success)
        //            {
        //                xml = Namespaces.Clear(match.Groups[1].Value);

        //                return XmlFunctions.FormatXml(xml, indent, false, true);
        //            }
        //        }
        //    }
        //    catch { }

        //    return null;
        //}
    }
}