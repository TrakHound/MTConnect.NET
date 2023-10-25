// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.Assets
{
    public partial class Asset : IAsset
    {
        private static Dictionary<string, Type> _types;


        internal string _uuid;
        /// <summary>
        /// The UUID of the MTConnect Entity
        /// </summary>
        public string Uuid
        {
            get
            {
                _uuid = $"{DeviceUuid}.{AssetId}.{Timestamp}";
                return _uuid;
            }
            set => _uuid = value;
        }

        public MTConnectEntityType EntityType => MTConnectEntityType.Asset;


        /// <summary>
        /// The type for the MTConnect Asset
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The Agent InstanceId that produced this Asset
        /// </summary>
        public long InstanceId { get; set; }


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


        public IAsset Process(Version mtconnectVersion)
        {
			if (mtconnectVersion < MTConnectVersions.Version12) return null;

			if (mtconnectVersion < MTConnectVersions.Version22) Hash = null;

			return OnProcess(mtconnectVersion);
		}

        protected virtual IAsset OnProcess(Version mtconnectVersion)
        {
			return this;
		}

        public virtual AssetValidationResult IsValid(Version mtconnectVersion)
        {
            return new AssetValidationResult(true);
        }

        public virtual string GenerateHash()
        {
			return null;
        }
	}
}