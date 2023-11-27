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
    public class XmlUnknownAsset : XmlAsset
    {
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