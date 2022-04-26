// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public class Asset : IAsset
    {
        private static Dictionary<string, Type> _types;


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


        public static IAsset Create(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    if (_types.TryGetValue(type, out Type t))
                    {
                        var constructor = t.GetConstructor(System.Type.EmptyTypes);
                        if (constructor != null)
                        {
                            try
                            {
                                return (IAsset)Activator.CreateInstance(t);
                            }
                            catch { }
                        }
                    }
                }
            }

            return null;
        }

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
                            t => typeof(IAsset).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));

                if (!types.IsNullOrEmpty())
                {
                    var objs = new Dictionary<string, Type>();
                    var regex = new Regex("(.*)Asset");

                    foreach (var type in types)
                    {
                        if (type.Name != "Asset" && type.Name != "Asset`1")
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


        public virtual IAsset Process(Version mtconnectVersion)
        {
            return this;
        }

        public virtual AssetValidationResult IsValid(Version mtconnectVersion)
        {
            return new AssetValidationResult(true);
        }
    }

    /// <summary>
    /// An Asset XML element is a container type XML element used to organize
    /// information describing an entity that is not a piece of equipment.
    /// </summary>
    public class Asset<T> : IAsset where T : IAsset
    {
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


        public virtual IAsset Process(Version mtconnectVersion)
        {
            return this;
        }

        public virtual AssetValidationResult IsValid(Version mtconnectVersion)
        {
            return new AssetValidationResult(true);
        }
    }
}
