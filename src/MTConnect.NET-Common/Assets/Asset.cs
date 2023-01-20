// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Extensions;
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
    /// It is used in the manufacturing process, but is not permanently associated with a single piece of equipment. 
    /// It can be removed from the piece of equipment without compromising its function, and can be associated with other pieces of equipment during its lifecycle.
    /// </summary>
    public class Asset : IAsset
    {
        private static Dictionary<string, Type> _types;

        private long _timestamp;
        private DateTime _dateTime;


        /// <summary>
        /// The unique identifier for the MTConnect Asset.
        /// </summary>
        [XmlAttribute("assetId")]
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The type for the MTConnect Asset
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool TypeSpecified => false;

        /// <summary>
        /// The time this MTConnect Asset was last modified.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public long Timestamp
        {
            get => _timestamp;
            set
            {
                _timestamp = value;
                if (_dateTime.ToUnixTime() != _timestamp) _dateTime = _timestamp.ToDateTime();
            }
        }

        /// <summary>
        /// The time this MTConnect Asset was last modified.
        /// </summary>
        [XmlAttribute("timestamp")]
        [JsonPropertyName("timestamp")]
        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                _dateTime = value;
                if (_timestamp.ToDateTime() != _dateTime) _timestamp = _dateTime.ToUnixTime();
            }
        }

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
        [XmlAttribute("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }


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
            if (mtconnectVersion < MTConnectVersions.Version12) return null;

            return this;
        }

        public virtual AssetValidationResult IsValid(Version mtconnectVersion)
        {
            return new AssetValidationResult(true);
        }
    }
}