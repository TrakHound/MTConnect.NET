// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.Devices
{
    public partial class Composition : IComposition
    {
        private static readonly Version DefaultMaximumVersion = null;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version14;
        private static readonly Dictionary<string, string> _typeIds = new Dictionary<string, string>();
        private static readonly object _lock = new object();

        private static Dictionary<string, Type> _types;


        public MTConnectEntityType EntityType => MTConnectEntityType.Composition;


        public virtual IEnumerable<IDataItem> DataItems { get; set; }

        /// <summary>
        /// The Container (Component or Device) that this Composition is directly associated with
        /// </summary>
        public IContainer Parent { get; set; }


		private string _hash;
		/// <summary>
		/// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
		/// </summary>
		public string Hash
		{
			get
			{
				if (_hash == null) _hash = GenerateHash();
				return _hash;
			}
		}

        /// <summary>
        /// The Agent InstanceId that produced this Device
        /// </summary>
        public ulong InstanceId { get; set; }


        /// <summary>
        /// The text description that describes what the Composition Type represents
        /// </summary>
        public virtual string TypeDescription => DescriptionText;


        /// <summary>
        /// The full path of IDs that describes the location of the Composition in the Device
        /// </summary>
        public string IdPath => GenerateIdPath(this);

        /// <summary>
        /// The list of IDs (in order) that describes the location of the Composition in the Device
        /// </summary>
        public string[] IdPaths => GenerateIdPaths(this);

        /// <summary>
        /// The full path of Types that describes the location of the Composition in the Device
        /// </summary>
        public string TypePath => GenerateTypePath(this);

        /// <summary>
        /// The list of Types (in order) that describes the location of the Composition in the Device
        /// </summary>
        public string[] TypePaths => GenerateTypePaths(this);


        /// <summary>
        /// The maximum MTConnect Version that this Composition Type is valid 
        /// (if set, this indicates that the Type has been Deprecated in the MTConnect Standard version specified)
        /// </summary>
        public virtual Version MaximumVersion => DefaultMaximumVersion;

        /// <summary>
        /// The minimum MTConnect Version that this Composition Type is valid 
        /// </summary>
        public virtual Version MinimumVersion => DefaultMinimumVersion;


		public string DataItemIdFormat { get; set; }

		public string CompositionIdFormat { get; set; }

		public string ComponentIdFormat { get; set; }


		public Composition()
        {
            //Id = StringFunctions.RandomString(10);
            DataItems = new List<IDataItem>();

			DataItemIdFormat = Component._defaultDataItemIdFormat;
			CompositionIdFormat = Component._defaultCompositionIdFormat;
			ComponentIdFormat = Component._defaultComponentIdFormat;
		}


        public string GenerateHash()
        {
            return GenerateHash(this);
        }

        public static string GenerateHash(IComposition composition)
        {
            if (composition != null)
            {
                var ids = new List<string>();

                ids.Add(ObjectExtensions.GetHashPropertyString(composition).ToSHA1Hash());

                // Add DataItem Change Ids
                if (!composition.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in composition.DataItems)
                    {
                        ids.Add(dataItem.Hash);
                    }
                }

                return StringFunctions.ToSHA1Hash(ids.ToArray());
            }

            return null;
        }


        public static string CreateId(string parentId, string name)
        {
            return $"{parentId}_{name}";
        }

        public static string CreateId(string parentId, string name, string suffix)
        {
            if (!string.IsNullOrEmpty(suffix))
            {
                return $"{parentId}_{name}_{suffix}";
            }
            else
            {
                return $"{parentId}_{name}";
            }
        }


        public static string[] GenerateIdPaths(IComposition composition)
        {
            if (composition != null)
            {
                var types = new List<string>();

                if (composition.Parent != null && !composition.Parent.IdPaths.IsNullOrEmpty())
                {
                    types.AddRange(composition.Parent.IdPaths);
                }

                types.Add(composition.Id);

                return types.ToArray();
            }

            return null;
        }

        public static string GenerateIdPath(IComposition composition)
        {
            if (composition != null && !composition.IdPaths.IsNullOrEmpty())
            {
                return string.Join("/", composition.IdPaths);
            }

            return null;
        }

        public static string[] GenerateTypePaths(IComposition composition)
        {
            if (composition != null)
            {
                var types = new List<string>();

                if (composition.Parent != null && !composition.Parent.TypePaths.IsNullOrEmpty())
                {
                    types.AddRange(composition.Parent.TypePaths);
                }

                types.Add(composition.Type);

                return types.ToArray();
            }

            return null;
        }

        public static string GenerateTypePath(IComposition composition)
        {
            if (composition != null && !composition.TypePaths.IsNullOrEmpty())
            {
                return string.Join("/", composition.TypePaths);
            }

            return null;
        }


        public static Composition Create(IComposition component)
        {
            var type = GetCompositionType(component.Type);
            if (type != component.GetType())
            {
                var obj = Create(type);
                obj.Id = component.Id;
                obj.Uuid = component.Uuid;
                obj.Name = component.Name;
                obj.NativeName = component.NativeName;
                obj.Type = component.Type;

                // Set Composition Description
                if (component.Description != null)
                {
                    var description = new Description();
                    description.Manufacturer = component.Description.Manufacturer;
                    description.Model = component.Description.Model;
                    description.SerialNumber = component.Description.SerialNumber;
                    description.Station = component.Description.Station;
                    description.Value = component.Description.Value;
                    obj.Description = description;
                }

                obj.SampleRate = component.SampleRate;
                obj.SampleInterval = component.SampleInterval;
                obj.References = component.References;
                obj.Configuration = component.Configuration;
                obj.CoordinateSystemIdRef = component.CoordinateSystemIdRef;

                obj.DataItems = component.DataItems;

                return obj;
            }

            return (Composition)component;
        }

        public static Composition Create(string type)
        {
            var t = GetCompositionType(type);
            return Create(t);
        }

        public static Composition Create(Type type)
        {
            if (type != null)
            {
                var constructor = type.GetConstructor(System.Type.EmptyTypes);
                if (constructor != null)
                {
                    try
                    {
                        return (Composition)Activator.CreateInstance(type);
                    }
                    catch { }
                }
            }

            return new Composition();
        }

        public static IEnumerable<string> GetTypes()
        {
            if (_types == null) _types = GetAllTypes();

            return _types.Keys;
        }

        private static Type GetCompositionType(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    string typeId;
                    lock (_lock)
                    {
                        _typeIds.TryGetValue(type, out typeId);
                        if (typeId == null)
                        {
                            typeId = type.ToPascalCase();
                            _typeIds.Add(type, typeId);
                        }
                    }

                    if (_types.TryGetValue(typeId, out Type t))
                    {
                        return t;
                    }
                }
            }

            return typeof(Composition);
        }

        private static Dictionary<string, Type> GetAllTypes()
        {
            var assemblies = Assemblies.Get();
            if (!assemblies.IsNullOrEmpty())
            {
                var types = assemblies
                    .SelectMany(
                        x => x.GetMatchingTypesInAssembly(
                            t => typeof(Composition).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));

                if (!types.IsNullOrEmpty())
                {
                    var objs = new Dictionary<string, Type>();
                    var regex = new Regex("(.*)Composition");

                    foreach (var type in types)
                    {
                        var match = regex.Match(type.Name);
                        if (match.Success && match.Groups.Count > 1)
                        {
                            var key = match.Groups[1].Value;
                            if (!objs.ContainsKey(key)) objs.Add(key, type);
                        }
                    }

                    return objs;
                }
            }

            return new Dictionary<string, Type>();
        }


        public static Composition Process(IComposition composition, Version mtconnectVersion = null)
        {
            if (composition != null)
            {
                var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersions.Max;

                // Check Version Compatibilty
                if (composition.MinimumVersion != null && version < composition.MinimumVersion) return null;

                // Create a new Instance of the Composition that will instantiate a new Derived class (if found)
                var obj = Create(composition.Type);
                obj.Id = composition.Id;
                obj.Uuid = composition.Uuid;
                obj.Name = composition.Name;
                obj.NativeName = composition.NativeName;
                obj.Type = composition.Type;
                obj.Parent = composition.Parent;

                // Set Composition Description
                if (composition.Description != null)
                {
                    var description = new Description();
                    description.Manufacturer = composition.Description.Manufacturer;
                    if (version >= MTConnectVersions.Version12) description.Model = composition.Description.Model;
                    description.SerialNumber = composition.Description.SerialNumber;
                    description.Station = composition.Description.Station;
                    description.Value = composition.Description.Value;
                    obj.Description = description;
                }

                if (version < MTConnectVersions.Version12) obj.SampleRate = composition.SampleRate;
                if (version >= MTConnectVersions.Version12) obj.SampleInterval = composition.SampleInterval;
                if (version >= MTConnectVersions.Version13) obj.References = composition.References;
                if (version >= MTConnectVersions.Version17) obj.Configuration = composition.Configuration;
                if (version >= MTConnectVersions.Version18) obj.CoordinateSystemIdRef = composition.CoordinateSystemIdRef;

                // Add DataItems
                if (!composition.DataItems.IsNullOrEmpty())
                {
                    var dataItems = new List<IDataItem>();

                    foreach (var dataItem in composition.DataItems)
                    {
                        var dataItemObj = DataItem.Process(dataItem, version);
                        if (dataItemObj != null) dataItems.Add(dataItemObj);
                    }

                    obj.DataItems = dataItems;
                }

                return obj;
            }

            return null;
        }


        #region "DataItems"

        /// <summary>
        /// Return a list of All DataItems
        /// </summary>
        public IEnumerable<IDataItem> GetDataItems()
        {
            var l = new List<IDataItem>();

            // Add Root DataItems
            if (!DataItems.IsNullOrEmpty()) l.AddRange(DataItems);

            // Add Composition DataItems
            if (!Compositions.IsNullOrEmpty())
            {
                foreach (var composition in Compositions)
                {
                    if (!composition.DataItems.IsNullOrEmpty()) l.AddRange(composition.DataItems);
                }
            }

            // Add Component DataItems
            if (!Components.IsNullOrEmpty())
            {
                foreach (var component in Components)
                {
                    var componentDataItems = GetDataItems(component);
                    if (!componentDataItems.IsNullOrEmpty()) l.AddRange(componentDataItems);
                }
            }

            return !l.IsNullOrEmpty() ? l : null;
        }

        private IEnumerable<IDataItem> GetDataItems(IComponent component)
        {
            var l = new List<IDataItem>();

            // Add Root DataItems
            if (!component.DataItems.IsNullOrEmpty()) l.AddRange(component.DataItems);

            // Add Composition DataItems
            if (!component.Compositions.IsNullOrEmpty())
            {
                foreach (var composition in component.Compositions)
                {
                    if (!composition.DataItems.IsNullOrEmpty()) l.AddRange(composition.DataItems);
                }
            }

            // Add SubComponent DataItems
            if (!component.Components.IsNullOrEmpty())
            {
                // Get SubComponent DataItems
                foreach (var subComponent in component.Components)
                {
                    var componentDataItems = GetDataItems(subComponent);
                    if (!componentDataItems.IsNullOrEmpty()) l.AddRange(componentDataItems);
                }
            }

            return !l.IsNullOrEmpty() ? l : null;
        }

        /// <summary>
        /// Return the DataItem matching either the ID, Name, or Source of the specified Key
        /// </summary>
        public IDataItem GetDataItemByKey(string dataItemKey)
        {
            if (!string.IsNullOrEmpty(dataItemKey))
            {
                var dataItems = GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    // Check DataItem ID
                    var dataItem = dataItems.FirstOrDefault(o => o.Id == dataItemKey);

                    // Check DataItem Name
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Name == dataItemKey);

                    // Check DataItem Source DataItemId
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Source != null && o.Source.DataItemId == dataItemKey);

                    // Check DataItem Source Result
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Source != null && o.Source.Value == dataItemKey);

                    // Return DataItem
                    return dataItem;
                }
            }

            return null;
        }

        /// <summary>
        /// Return the first DataItem matching the Type
        /// </summary>
        public IDataItem GetDataItemByType(string type, SearchType searchType = SearchType.Child)
        {
            if (!string.IsNullOrEmpty(type))
            {
                IEnumerable<IDataItem> dataItems = null;
                switch (searchType)
                {
                    case SearchType.Child: dataItems = DataItems; break;
                    case SearchType.AnyLevel: dataItems = GetDataItems(); break;
                }

                if (!dataItems.IsNullOrEmpty())
                {
                    return dataItems.FirstOrDefault(o => o.Type == type);
                }
            }

            return null;
        }

        /// <summary>
        /// Return the first DataItem matching the Type and SubType
        /// </summary>
        public IDataItem GetDataItemByType(string type, string subType, SearchType searchType = SearchType.Child)
        {
            if (!string.IsNullOrEmpty(type))
            {
                IEnumerable<IDataItem> dataItems = null;
                switch (searchType)
                {
                    case SearchType.Child: dataItems = DataItems; break;
                    case SearchType.AnyLevel: dataItems = GetDataItems(); break;
                }

                if (!dataItems.IsNullOrEmpty())
                {
                    return dataItems.FirstOrDefault(o => o.Type == type && o.SubType == subType);
                }
            }

            return null;
        }

        /// <summary>
        /// Return the first DataItem matching the Type
        /// </summary>
        public IDataItem GetDataItem<TDataItem>(string subType = null, SearchType searchType = SearchType.Child) where TDataItem : IDataItem
        {
            var typeIdField = typeof(TDataItem).GetField("TypeId");
            if (typeIdField != null)
            {
                var typeId = typeIdField.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(typeId))
                {
                    return GetDataItemByType(typeId, subType, searchType);
                }
            }

            return null;
        }

        /// <summary>
        /// Return All DataItems matching the Type
        /// </summary>
        public IEnumerable<IDataItem> GetDataItemsByType(string type, SearchType searchType = SearchType.Child)
        {
            if (!string.IsNullOrEmpty(type))
            {
                IEnumerable<IDataItem> dataItems = null;
                switch (searchType)
                {
                    case SearchType.Child: dataItems = DataItems; break;
                    case SearchType.AnyLevel: dataItems = GetDataItems(); break;
                }

                if (!dataItems.IsNullOrEmpty())
                {
                    return dataItems.Where(o => o.Type == type);
                }
            }

            return null;
        }

        /// <summary>
        /// Return All DataItems matching the Type and SubType
        /// </summary>
        public IEnumerable<IDataItem> GetDataItemsByType(string type, string subType, SearchType searchType = SearchType.Child)
        {
            if (!string.IsNullOrEmpty(type))
            {
                IEnumerable<IDataItem> dataItems = null;
                switch (searchType)
                {
                    case SearchType.Child: dataItems = DataItems; break;
                    case SearchType.AnyLevel: dataItems = GetDataItems(); break;
                }

                if (!dataItems.IsNullOrEmpty())
                {
                    return dataItems.Where(o => o.Type == type && o.SubType == subType);
                }
            }

            return null;
        }

        /// <summary>
        /// Return All DataItems matching the Type and SubType
        /// </summary>
        public IEnumerable<IDataItem> GetDataItems<TDataItem>(string subType = null, SearchType searchType = SearchType.Child) where TDataItem : IDataItem
        {
            var typeIdField = typeof(TDataItem).GetField("TypeId");
            if (typeIdField != null)
            {
                var typeId = typeIdField.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(typeId))
                {
                    return GetDataItemsByType(typeId, subType, searchType);
                }
            }

            return null;
        }


        /// <summary>
        /// Add a DataItem to the Component
        /// </summary>
        /// <param name="dataItem">The DataItem to add</param>
        public void AddDataItem(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                ((DataItem)dataItem).Container = this;
                ((DataItem)dataItem).CompositionId = Id;

                if (!string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(dataItem.Id)) Component.ResetId(this, dataItem);

                var dataItems = new List<IDataItem>();

                if (!DataItems.IsNullOrEmpty())
                {
                    dataItems.AddRange(DataItems);
                }

                dataItems.Add(dataItem);
                DataItems = dataItems;
            }
        }

        /// <summary>
        /// Add a DataItem to the Component
        /// </summary>
        public void AddDataItem<TDataItem>() where TDataItem : IDataItem
        {
            var constructor = typeof(TDataItem).GetConstructor(new Type[] { });
            if (constructor != null)
            {
                try
                {
                    var dataItem = (DataItem)constructor.Invoke(null);
                    AddDataItem(dataItem);
                }
                catch { }
            }
        }

        /// <summary>
        /// Add a DataItem to the Component
        /// </summary>
        public void AddDataItem<TDataItem>(string name, object subType = null) where TDataItem : IDataItem
        {
            var constructor = typeof(TDataItem).GetConstructor(new Type[] { });
            if (constructor != null)
            {
                try
                {
                    var dataItem = (DataItem)constructor.Invoke(null);
                    dataItem.Name = name;
                    dataItem.SubType = subType?.ToString();
                    AddDataItem(dataItem);
                }
                catch { }
            }
        }

        /// <summary>
        /// Add a DataItem to the Component
        /// </summary>
        public void AddDataItem<TDataItem>(object subType) where TDataItem : IDataItem
        {
            var constructor = typeof(TDataItem).GetConstructor(new Type[] { });
            if (constructor != null)
            {
                try
                {
                    var dataItem = (DataItem)constructor.Invoke(null);
                    dataItem.SubType = subType?.ToString();
                    AddDataItem(dataItem);
                }
                catch { }
            }
        }

        public void AddDataItem(DataItemCategory category, string type, string subType = null, string dataItemId = null)
        {
            if (!string.IsNullOrEmpty(type))
            {
                AddDataItem(new DataItem(category, type, subType, dataItemId));
            }
        }

        /// <summary>
        /// Add DataItems to the Component
        /// </summary>
        /// <param name="dataItems">The DataItems to add</param>
        public void AddDataItems(IEnumerable<IDataItem> dataItems)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                foreach (var dataItem in dataItems)
                {
                    AddDataItem(dataItem);
                }
            }
        }


        /// <summary>
        /// Remove a DataItem from the Component
        /// </summary>
        /// <param name="dataItemId">The ID of the DataItem to remove</param>
        public void RemoveDataItem(string dataItemId)
        {
            if (!DataItems.IsNullOrEmpty())
            {
                var dataItems = new List<IDataItem>();
                dataItems.AddRange(DataItems);
                dataItems.RemoveAll(o => o.Id == dataItemId);
                DataItems = dataItems;
            }
        }

        #endregion

    }
}