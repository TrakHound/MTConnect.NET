// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Devices.Configurations;
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
    public class XmlAsset
    {
        private static readonly Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();
        private static readonly object _lock = new object();
        private static Dictionary<string, Type> _types;


        [XmlAttribute("assetId")]
        public string AssetId { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("timestamp")]
        public DateTime Timestamp { get; set; }

        [XmlAttribute("deviceUuid")]
        public string DeviceUuid { get; set; }

        [XmlAttribute("removed")]
        public bool Removed { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }


        public virtual IAsset ToAsset() { return null; }


        public static IAsset FromXml(string type, byte[] xmlBytes)
        {
            var asset = Asset.Create(type);
            if (asset != null)
            {
                var xmlType = GetAssetType(type);
                if (xmlType == null) xmlType = typeof(XmlUnknownAsset);

                return FromXml(xmlType, xmlBytes);
                //return FromXml(asset.GetType(), xmlBytes);
            }

            return default;
        }

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


        public static string ToXml(IAsset asset, bool indent = false)
        {
            try
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                using (var writer = new StringWriter())
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

                    //XmlSerializer serializer = null;
                    //lock (_lock)
                    //{
                    //    var assetType = GetAssetType(asset.Type);
                    //    if (assetType != null)
                    //    {
                    //        _serializers.TryGetValue(assetType, out serializer);
                    //        //_serializers.TryGetValue(asset.GetType(), out serializer);
                    //        if (serializer == null)
                    //        {
                    //            serializer = new XmlSerializer(assetType);
                    //            _serializers.Add(assetType, serializer);
                    //        }
                    //    }
                    //}

                        //if (serializer != null)
                        //{


                        //    //serializer.Serialize(writer, asset, namespaces);

                        //    //var xml = writer.ToString();
                        //    //var regexPattern = $@"(<{asset.Type}[.\s\S]*(?:(?:<\/{asset.Type}>)|(?:\/>)))";
                        //    //var regex = new Regex(regexPattern);
                        //    //var match = regex.Match(xml);
                        //    //if (match.Success)
                        //    //{
                        //    //    xml = Namespaces.Clear(match.Groups[1].Value);

                        //    //    return XmlFunctions.FormatXml(xml, indent, false, true);
                        //    //}
                        //}
                }
            }
            catch { }

            return null;
        }


        //public static IAsset Create(string type)
        //{
        //    if (!string.IsNullOrEmpty(type))
        //    {
        //        if (_types == null) _types = GetAllTypes();

        //        if (!_types.IsNullOrEmpty())
        //        {
        //            if (_types.TryGetValue(type, out Type t))
        //            {
        //                var constructor = t.GetConstructor(System.Type.EmptyTypes);
        //                if (constructor != null)
        //                {
        //                    try
        //                    {
        //                        return (IAsset)Activator.CreateInstance(t);
        //                    }
        //                    catch { }
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}

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