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

        /// <summary>
        /// Identifies this entity as an Asset within the MTConnect entity model.
        /// </summary>
        public MTConnectEntityType EntityType => MTConnectEntityType.Asset;


        /// <summary>
        /// The type for the MTConnect Asset
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The Agent InstanceId that produced this Asset
        /// </summary>
        public ulong InstanceId { get; set; }


        /// <summary>
        /// Instantiates the concrete asset class registered for the given asset type name via its parameterless constructor; returns null when the type is unknown or cannot be constructed.
        /// </summary>
        /// <param name="type">The MTConnect asset type name (e.g. "CuttingTool").</param>
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

        /// <summary>
        /// Resolves the concrete CLR type registered for the given asset type name without instantiating it; returns null when the type is unknown.
        /// </summary>
        /// <param name="type">The MTConnect asset type name (e.g. "CuttingTool").</param>
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


        /// <summary>
        /// Prepares this asset for inclusion in a response document for the given MTConnect version: excludes the asset entirely when the version predates 1.2, drops the Hash for versions before 2.2 (where it is not defined), then delegates per-type adjustment to <see cref="OnProcess"/>.
        /// </summary>
        /// <param name="mtconnectVersion">The MTConnect version the response document targets.</param>
        public IAsset Process(Version mtconnectVersion)
        {
			if (mtconnectVersion < MTConnectVersions.Version12) return null;

			if (mtconnectVersion < MTConnectVersions.Version22) Hash = null;

			return OnProcess(mtconnectVersion);
		}

        /// <summary>
        /// Per-type version adjustment hook invoked by <see cref="Process"/>; the base implementation returns the asset unchanged. Overrides may downgrade properties or return null to exclude the asset for a given version.
        /// </summary>
        /// <param name="mtconnectVersion">The MTConnect version the response document targets.</param>
        protected virtual IAsset OnProcess(Version mtconnectVersion)
        {
			return this;
		}

        /// <summary>
        /// Validates the asset against the given MTConnect version; the base implementation accepts all assets and is overridden by types with required fields or constraints.
        /// </summary>
        /// <param name="mtconnectVersion">The MTConnect version to validate against.</param>
        public virtual ValidationResult IsValid(Version mtconnectVersion)
        {
            return new ValidationResult(true);
        }

        /// <summary>
        /// Computes a content hash for change detection; the base implementation returns null and is overridden by concrete asset types that participate in hashing.
        /// </summary>
        /// <param name="includeTimestamp">When true, the asset timestamp is folded into the hash.</param>
        public virtual string GenerateHash(bool includeTimestamp = true)
        {
			return null;
        }
	}
}