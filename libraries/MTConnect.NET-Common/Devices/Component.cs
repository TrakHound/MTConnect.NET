// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.Devices
{
    public partial class Component : IComponent
    {
        internal const string _defaultComponentIdFormat = "{parent.id}_{component.name}";
        internal const string _defaultCompositionIdFormat = "{parent.id}_{composition.type}";
        internal const string _defaultDataItemIdFormat = "{component.id}_{dataitem.name}{?:_{dataitem.sub_type}}";

        private const string _idFormatPattern = @"\{([^\{\.]+)\.([^\{\.]+)\}|\{\?\:(.*)\{([^\{\.]+)\.([^\{\.]+)\}(.*)\}";
        private static readonly Regex _idFormatRegex = new Regex(_idFormatPattern, RegexOptions.Compiled);


        private static readonly Version DefaultMaximumVersion = null;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version10;
        private static readonly Dictionary<string, string> _typeIds = new Dictionary<string, string>();
        private static readonly object _lock = new object();

        private static Dictionary<string, Type> _types;
        private static Dictionary<string, string> _typeDescriptions;

        private struct IdFormatConfiguration
        {
            public string Pattern { get; set; }
            public string Prefix { get; set; }
            public string Suffix { get; set; }
            public string Object { get; set; }
            public string Property { get; set; }
            public bool IsOptional { get; set; }
        }


        public MTConnectEntityType EntityType => MTConnectEntityType.Component;


        public IEnumerable<IDataItem> DataItems { get; set; }

        /// <summary>
        /// The Type of Component
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The Container (Component or Device) that this Component is directly associated with
        /// </summary>
        public IContainer Parent { get; set; }

        /// <summary>
        /// The Agent InstanceId that produced this Device
        /// </summary>
        public ulong InstanceId { get; set; }


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
		/// The text description that describes what the Component Type represents
		/// </summary>
		public virtual string TypeDescription => DescriptionText;

        /// <summary>
        /// Gets whether the Component is an Organizer Type
        /// </summary>
        public bool IsOrganizer => Organizers.Components.Contains(Type);


        /// <summary>
        /// The full path of IDs that describes the location of the Component in the Device
        /// </summary>
        public string IdPath => GenerateIdPath(this);

        /// <summary>
        /// The list of IDs (in order) that describes the location of the Component in the Device
        /// </summary>
        public string[] IdPaths => GenerateIdPaths(this);

        /// <summary>
        /// The full path of Types that describes the location of the Component in the Device
        /// </summary>
        public string TypePath => GenerateTypePath(this);

        /// <summary>
        /// The list of Types (in order) that describes the location of the Component in the Device
        /// </summary>
        public string[] TypePaths => GenerateTypePaths(this);


        /// <summary>
        /// The maximum MTConnect Version that this Component Type is valid 
        /// (if set, this indicates that the Type has been Deprecated in the MTConnect Standard version specified)
        /// </summary>
        public virtual Version MaximumVersion => DefaultMaximumVersion;

        /// <summary>
        /// The minimum MTConnect Version that this Component Type is valid 
        /// </summary>
        public virtual Version MinimumVersion => DefaultMinimumVersion;


        public string ComponentIdFormat { get; set; }

        public string CompositionIdFormat { get; set; }

        public string DataItemIdFormat { get; set; }


        public Component()
        {
            Components = new List<IComponent>();
            Compositions = new List<IComposition>();
            DataItems = new List<IDataItem>();

			ComponentIdFormat = _defaultComponentIdFormat;
			CompositionIdFormat = _defaultCompositionIdFormat;
            DataItemIdFormat = _defaultDataItemIdFormat;
        }


        public string GenerateHash()
        {
            return GenerateHash(this);
        }

        public static string GenerateHash(IComponent component)
        {
            if (component != null)
            {
                var ids = new List<string>();

                ids.Add(ObjectExtensions.GetHashPropertyString(component).ToSHA1Hash());

                // Add DataItem Change Ids
                if (!component.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in component.DataItems)
                    {
                        ids.Add(dataItem.Hash);
                    }
                }

                // Add Composition Change Ids
                if (!component.Compositions.IsNullOrEmpty())
                {
                    foreach (var composition in component.Compositions)
                    {
                        ids.Add(composition.Hash);
                    }
                }

                // Add Component Change Ids
                if (!component.Components.IsNullOrEmpty())
                {
                    foreach (var subcomponent in component.Components)
                    {
                        ids.Add(subcomponent.Hash);
                    }
                }

                return StringFunctions.ToSHA1Hash(ids.ToArray());
            }

            return null;
        }



        public static string[] GenerateIdPaths(IComponent component)
        {
            if (component != null)
            {
                var types = new List<string>();

                if (component.Parent != null && !component.Parent.IdPaths.IsNullOrEmpty())
                {
                    types.AddRange(component.Parent.IdPaths);
                }

                types.Add(component.Id);

                return types.ToArray();
            }

            return null;
        }

        public static string GenerateIdPath(IComponent component)
        {
            if (component != null && !component.IdPaths.IsNullOrEmpty())
            {
                return string.Join("/", component.IdPaths);
            }

            return null;
        }

        public static string[] GenerateTypePaths(IComponent component)
        {
            if (component != null)
            {
                var types = new List<string>();

                if (component.Parent != null && !component.Parent.TypePaths.IsNullOrEmpty())
                {
                    types.AddRange(component.Parent.TypePaths);
                }

                types.Add(component.Type);

                return types.ToArray();
            }

            return null;
        }

        public static string GenerateTypePath(IComponent component)
        {
            if (component != null && !component.TypePaths.IsNullOrEmpty())
            {
                return string.Join("/", component.TypePaths);
            }

            return null;
        }


        #region "Components"

        /// <summary>
        /// Return a list of All Components
        /// </summary>
        public IEnumerable<IComponent> GetComponents()
        {
            var l = new List<IComponent>();

            if (!Components.IsNullOrEmpty())
            {
                foreach (var subComponent in Components)
                {
                    var components = GetComponents(subComponent);
                    if (!components.IsNullOrEmpty()) l.AddRange(components);
                }
            }
            return !l.IsNullOrEmpty() ? l : null;
        }

		private IEnumerable<IComponent> GetComponents(IComponent component)
        {
            var l = new List<IComponent>();

            l.Add(component);

            if (!component.Components.IsNullOrEmpty())
            {
                foreach (var subComponent in component.Components)
                {
                    var components = GetComponents(subComponent);
                    if (!components.IsNullOrEmpty()) l.AddRange(components);
                }
            }

            return !l.IsNullOrEmpty() ? l : null;
        }

		/// <summary>
		/// Return the first Component matching the Type
		/// </summary>
		public IComponent GetComponent(string type, string name = null, SearchType searchType = SearchType.AnyLevel)
		{
			if (!string.IsNullOrEmpty(type))
			{
				IEnumerable<IComponent> components = null;
				switch (searchType)
				{
					case SearchType.Child: components = Components; break;
					case SearchType.AnyLevel: components = GetComponents(); break;
				}

				if (!components.IsNullOrEmpty())
				{
					if (!string.IsNullOrEmpty(name))
					{
						return components.FirstOrDefault(o => o.Type == type && o.Name == name);
					}
					else
					{
						return components.FirstOrDefault(o => o.Type == type);
					}
				}
			}

			return null;
		}

        /// <summary>
        /// Return the first Component matching the Type
        /// </summary>
        public IComponent GetComponent<TComponent>(string name = null, SearchType searchType = SearchType.AnyLevel) where TComponent : IComponent
        {
            var typeIdField = typeof(TComponent).GetField("TypeId");
            if (typeIdField != null)
            {
                var typeId = typeIdField.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(typeId))
                {
                    return GetComponent(typeId, name, searchType);
                }
            }

            return null;
        }

        /// <summary>
        /// Return All Components matching the Type
        /// </summary>
        public IEnumerable<IComponent> GetComponents(string type, string name = null, SearchType searchType = SearchType.AnyLevel)
		{
			if (!string.IsNullOrEmpty(type))
			{
				IEnumerable<IComponent> components = null;
				switch (searchType)
				{
					case SearchType.Child: components = Components; break;
					case SearchType.AnyLevel: components = GetComponents(); break;
				}

				if (!components.IsNullOrEmpty())
				{
                    if (!string.IsNullOrEmpty(name))
                    {
						return components.Where(o => o.Type == type && o.Name == name);
					}
                    else
                    {
						return components.Where(o => o.Type == type);
					}
				}
			}

			return null;
		}

        /// <summary>
        /// Return All Components matching the Type
        /// </summary>
        public IEnumerable<IComponent> GetComponents<TComponent>(string name = null, SearchType searchType = SearchType.AnyLevel) where TComponent : IComponent
        {
            var typeIdField = typeof(TComponent).GetField("TypeId");
            if (typeIdField != null)
            {
                var typeId = typeIdField.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(typeId))
                {
                    return GetComponents(typeId, name, searchType);
                }
            }

            return null;
        }


        /// <summary>
        /// Add a Component to the Component
        /// </summary>
        /// <param name="component">The Component to add</param>
        public void AddComponent(IComponent component)
        {
            if (component != null)
            {
                component.Parent = this;

                // Set ID
                if (!string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(component.Id))
                {
                    ResetIds(component);
				}

                var components = new List<IComponent>();

                if (!Components.IsNullOrEmpty())
                {
                    components.AddRange(Components);
                }

                components.Add(component);
                Components = components;
            }
        }

        /// <summary>
        /// Add Components to the Component
        /// </summary>
        /// <param name="components">The Components to add</param>
        public void AddComponents(IEnumerable<IComponent> components)
        {
            if (!components.IsNullOrEmpty())
            {
				foreach (var component in components)
				{
					AddComponent(component);
				}
			}
        }


        /// <summary>
        /// Remove a Component from the Component
        /// </summary>
        /// <param name="componentId">The ID of the Component to remove</param>
        public void RemoveComponent(string componentId)
        {
            if (!Components.IsNullOrEmpty())
            {
                var components = new List<IComponent>();
                components.AddRange(Components);

                components.RemoveAll(o => o.Id == componentId);

                foreach (var subComponent in components)
                {
                    RemoveComponent(subComponent, componentId);
                }

                Components = components;
            }
        }

        private void RemoveComponent(IComponent component, string componentId)
        {
            if (component != null && !component.Components.IsNullOrEmpty())
            {
                var components = new List<IComponent>();
                components.AddRange(component.Components);
                components.RemoveAll(o => o.Id == componentId);

                foreach (var subComponent in components)
                {
                    RemoveComponent(subComponent, componentId);
                }

                ((Component)component).AddComponents(components);
            }
        }

        #endregion

        #region "Compositions"

        /// <summary>
        /// Return a list of All Compositions
        /// </summary>
        public IEnumerable<IComposition> GetCompositions()
        {
            var l = new List<IComposition>();

            if (!Compositions.IsNullOrEmpty())
            {
                foreach (var composition in Compositions)
                {
                    l.Add(composition);
                }
            }

            if (!Components.IsNullOrEmpty())
            {
                foreach (var subComponent in Components)
                {
                    var components = GetCompositions(subComponent);
                    if (!components.IsNullOrEmpty()) l.AddRange(components);
                }
            }
            return !l.IsNullOrEmpty() ? l : null;
        }

        private IEnumerable<IComposition> GetCompositions(IComponent component)
        {
            var l = new List<IComposition>();

            if (!component.Compositions.IsNullOrEmpty())
            {
                foreach (var composition in component.Compositions)
                {
                    l.Add(composition);
                }
            }

            if (!component.Components.IsNullOrEmpty())
            {
                foreach (var subComponent in component.Components)
                {
                    var compositions = GetCompositions(subComponent);
                    if (!compositions.IsNullOrEmpty()) l.AddRange(compositions);
                }
            }

            return !l.IsNullOrEmpty() ? l : null;
        }

        /// <summary>
        /// Return the first Composition matching the Type
        /// </summary>
        public IComposition GetComposition(string type, string name = null, SearchType searchType = SearchType.AnyLevel)
        {
            if (!string.IsNullOrEmpty(type))
            {
                IEnumerable<IComposition> components = null;
                switch (searchType)
                {
                    case SearchType.Child: components = Compositions; break;
                    case SearchType.AnyLevel: components = GetCompositions(); break;
                }

                if (!components.IsNullOrEmpty())
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        return components.FirstOrDefault(o => o.Type == type && o.Name == name);
                    }
                    else
                    {
                        return components.FirstOrDefault(o => o.Type == type);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Return the first Composition matching the Type
        /// </summary>
        public IComposition GetComposition<TComposition>(string name = null, SearchType searchType = SearchType.AnyLevel) where TComposition : IComposition
        {
            var typeIdField = typeof(TComposition).GetField("TypeId");
            if (typeIdField != null)
            {
                var typeId = typeIdField.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(typeId))
                {
                    return GetComposition(typeId, name, searchType);
                }
            }

            return null;
        }

        /// <summary>
        /// Return All Compositions matching the Type
        /// </summary>
        public IEnumerable<IComposition> GetCompositions(string type, string name = null, SearchType searchType = SearchType.AnyLevel)
        {
            if (!string.IsNullOrEmpty(type))
            {
                IEnumerable<IComposition> components = null;
                switch (searchType)
                {
                    case SearchType.Child: components = Compositions; break;
                    case SearchType.AnyLevel: components = GetCompositions(); break;
                }

                if (!components.IsNullOrEmpty())
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        return components.Where(o => o.Type == type && o.Name == name);
                    }
                    else
                    {
                        return components.Where(o => o.Type == type);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Return All Compositions matching the Type
        /// </summary>
        public IEnumerable<IComposition> GetCompositions<TComposition>(string name = null, SearchType searchType = SearchType.AnyLevel) where TComposition : IComposition
        {
            var typeIdField = typeof(TComposition).GetField("TypeId");
            if (typeIdField != null)
            {
                var typeId = typeIdField.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(typeId))
                {
                    return GetCompositions(typeId, name, searchType);
                }
            }

            return null;
        }


        /// <summary>
        /// Add a Composition to the Composition
        /// </summary>
        /// <param name="composition">The Composition to add</param>
        public void AddComposition(IComposition composition)
        {
            if (composition != null)
            {
                composition.Parent = this;

                // Set ID
                if (!string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(composition.Id))
                {
                    ResetIds(composition);
                }

                var components = new List<IComposition>();

                if (!Compositions.IsNullOrEmpty())
                {
                    components.AddRange(Compositions);
                }

                components.Add(composition);
                Compositions = components;
            }
        }

        /// <summary>
        /// Add Compositions to the Composition
        /// </summary>
        /// <param name="compositions">The Compositions to add</param>
        public void AddCompositions(IEnumerable<IComposition> compositions)
        {
            if (!compositions.IsNullOrEmpty())
            {
                foreach (var component in compositions)
                {
                    AddComposition(component);
                }
            }
        }


        /// <summary>
        /// Remove a Composition from the Composition
        /// </summary>
        /// <param name="compositionId">The ID of the Composition to remove</param>
        public void RemoveComposition(string compositionId)
        {
            if (!Compositions.IsNullOrEmpty())
            {
                var compositions = new List<IComposition>();
                compositions.AddRange(Compositions);
                compositions.RemoveAll(o => o.Id == compositionId);

                Compositions = compositions;
            }
        }

        private void RemoveComposition(IComponent component, string compositionId)
        {
            if (component != null && !component.Compositions.IsNullOrEmpty())
            {
                var compositions = new List<IComposition>();
                compositions.AddRange(component.Compositions);
                compositions.RemoveAll(o => o.Id == compositionId);

                ((Component)component).AddCompositions(compositions);
            }
        }

        #endregion

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

                if (!string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(dataItem.Id)) ResetId(this, dataItem);

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

            var components = GetComponents();
            if (!components.IsNullOrEmpty())
            {
                foreach (var component in components)
                {
                    if (!component.DataItems.IsNullOrEmpty())
                    {
                        var dataItems = new List<IDataItem>();
                        dataItems.AddRange(component.DataItems);
                        dataItems.RemoveAll(o => o.Id == dataItemId);
                        component.DataItems = dataItems;
                    }
                }
            }
        }

		#endregion

		#region "ID"

		internal static void ResetIds(IComponent component)
		{
            if (component != null)
            {
				((Component)component).Id = CreateContainerId(component, component.ComponentIdFormat);

				// Set Child Component IDs
				if (!component.Components.IsNullOrEmpty())
				{
					foreach (var subcomponent in component.Components)
					{
                        ResetIds(subcomponent);
					}
				}

                // Set Child Composition IDs
                if (!component.Compositions.IsNullOrEmpty())
                {
                    foreach (var composition in component.Compositions)
                    {
                        ResetIds(composition);
                    }
                }

                // Set Child DataItem IDs
                if (!component.DataItems.IsNullOrEmpty())
				{
					foreach (var dataItem in component.DataItems)
					{
                        ResetId(component, dataItem);
					}
				}
			}
		}

        internal static void ResetIds(IComposition composition)
        {
            if (composition != null)
            {
                ((Composition)composition).Id = CreateContainerId(composition, composition.CompositionIdFormat);

                // Set Child DataItem IDs
                if (!composition.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in composition.DataItems)
                    {
                        ((DataItem)dataItem).CompositionId = composition.Id;
                        ResetId(composition, dataItem);
                    }
                }
            }
        }

        internal static void ResetId(IContainer container, IDataItem dataItem)
		{
			if (container != null && dataItem != null)
			{
				((DataItem)dataItem).Id = CreateDataItemId(container, dataItem, container.DataItemIdFormat);
			}
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


		public static string CreateContainerId(IContainer container, string pattern)
		{
			if (container != null)
			{
                var result = pattern;

                var configurations = GetIdFormatConfigurations(pattern);
                if (configurations != null)
                {
                    foreach (var configuration in configurations)
                    {
                        var obj = GetIdFormatObject(container, configuration);
                        var value = GetPropertyValue(obj, configuration.Property);
                        result = result.Replace(configuration.Pattern, value);
                    }
                }

                return result;
			}

			return null;
		}

		public static string CreateDataItemId(IContainer container, IDataItem dataItem, string pattern)
        {
            if (container != null && dataItem != null)
            {
				var result = pattern;

				var configurations = GetIdFormatConfigurations(pattern);
				if (configurations != null)
				{
					foreach (var configuration in configurations)
					{
						var obj = GetIdFormatObject(container, dataItem, configuration);
						var value = GetPropertyValue(obj, configuration.Property);
                        if (value != null)
                        {
                            result = result.Replace(configuration.Pattern, $"{configuration.Prefix}{value}{configuration.Suffix}");
                        }
                        else if (configuration.IsOptional)
                        {
                            result = result.Replace(configuration.Pattern, "");
                        }
					}
				}

				return result;
			}

            return null;
        }

        private static object GetIdFormatObject(IContainer container, IdFormatConfiguration configuration)
        { 
            if (container != null)
            {
                switch (configuration.Object.ToLower())
                {
                    case "parent": return container.Parent;
                    case "device": return container;
                    case "component": return container;
                    case "composition": return container;
                }
            }

            return null;
        }

		private static object GetIdFormatObject(IContainer container, IDataItem dataItem, IdFormatConfiguration configuration)
		{
			if (container != null && dataItem != null)
			{
				switch (configuration.Object.ToLower())
				{
					case "parent": return container.Parent;
					case "device": return container;
					case "component": return container;
					case "composition": return container;
					case "dataitem": return dataItem;
				}
			}

			return null;
		}

		private static IEnumerable<IdFormatConfiguration> GetIdFormatConfigurations(string pattern)
		{
			if (!string.IsNullOrEmpty(pattern))
			{
                try
                {
                    var configurations = new List<IdFormatConfiguration>();

                    var matches = _idFormatRegex.Matches(pattern);
                    if (matches != null)
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Groups != null && match.Groups.Count > 4)
                            {
                                var configuration = new IdFormatConfiguration();
                                configuration.Pattern = match.Groups[0].Value;

                                // Required
                                if (!string.IsNullOrEmpty(match.Groups[1].Value)) configuration.Object = match.Groups[1].Value;
                                if (!string.IsNullOrEmpty(match.Groups[2].Value)) configuration.Property = match.Groups[2].Value;

                                // Optional
                                if (!string.IsNullOrEmpty(match.Groups[4].Value) && !string.IsNullOrEmpty(match.Groups[5].Value))
                                {
                                    configuration.Prefix = match.Groups[3].Value;
                                    configuration.Object = match.Groups[4].Value;
                                    configuration.Property = match.Groups[5].Value;
                                    configuration.Suffix = match.Groups[6].Value;
                                    configuration.IsOptional = true;
                                }                              

                                configurations.Add(configuration);
                            }
                        }
                    }

                    return configurations;
                }
                catch { }
			}

			return null;
		}

		private static string GetPropertyValue(object obj, string propertyName)
        {
            if (obj != null && !string.IsNullOrEmpty(propertyName))
            {
                var objType = obj.GetType();

                var property = objType.GetProperty(propertyName.ToTitleCase());
                if (property != null)
                {
                    var val = property.GetValue(obj)?.ToString();

                    switch (property.Name)
                    {
                        case "Type": val = val.ToCamelCase(); break;
                        case "SubType": val = val.ToCamelCase(); break;
                    }

                    return val;
                }
            }

            return null;
        }

        #endregion


        public static Component Create(IComponent component)
        {
            var type = GetComponentType(component.Type);
            if (type != component.GetType())
            {
                var obj = Create(type);
                obj.Id = component.Id;
                obj.Uuid = component.Uuid;
                obj.Name = component.Name;
                obj.NativeName = component.NativeName;
                obj.Type = component.Type;

                // Set Component Description
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

                obj.Components = component.Components;
                obj.Compositions = component.Compositions;
                obj.DataItems = component.DataItems;

                return obj;
            }

            return (Component)component;
        }

        public static Component Create(string type)
        {
            var t = GetComponentType(type);
            return Create(t);
        }

        public static Component Create(Type type)
        {
            if (type != null)
            {
                var constructor = type.GetConstructor(System.Type.EmptyTypes);
                if (constructor != null)
                {
                    try
                    {
                        return (Component)Activator.CreateInstance(type);
                    }
                    catch { }
                }
            }

            return new Component();
        }

        public static IEnumerable<string> GetTypes()
        {
            if (_types == null) _types = GetAllTypes();

            return _types.Keys;
        }

        public static IEnumerable<KeyValuePair<string, string>> GetTypeDescriptions()
        {
            if (_typeDescriptions == null)
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    _typeDescriptions = new Dictionary<string, string>();

                    foreach (var type in _types)
                    {
                        var instance = Create(type.Value);
                        if (instance != null)
                        {
                            _typeDescriptions.Remove(instance.Type);
                            _typeDescriptions.Add(instance.Type, instance.TypeDescription);
                        }
                    }
                }
            }

            return _typeDescriptions;
        }

        private static Type GetComponentType(string type)
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

            return typeof(Component);
        }

        //private static string GetComponentTypeDescription(string type)
        //{
        //    var t = GetComponentType(type);
        //    if (t != null)
        //    {
        //        var field = t.GetField(nameof(DescriptionText));
        //        if (field != null)
        //        {
        //            return field.GetValue()
        //        }
        //    }

        //    return null;
        //}

        private static Dictionary<string, Type> GetAllTypes()
        {
            var assemblies = Assemblies.Get();
            if (!assemblies.IsNullOrEmpty())
            {
                var types = assemblies
                    .SelectMany(
                        x => x.GetMatchingTypesInAssembly(
                            t => typeof(Component).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));

                if (!types.IsNullOrEmpty())
                {
                    var objs = new Dictionary<string, Type>();
                    var regex = new Regex("(.*)Component");

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


        public static bool IsCompatible(IComponent component, Version mtconnectVersion)
        {
            if (component != null)
            {
                return component.MinimumVersion != null && mtconnectVersion >= component.MinimumVersion;
            }

            return false;
        }

        public static Component Process(IComponent component, Version mtconnectVersion = null)
        {
            if (component != null)
            {
                var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersions.Max;

                // Check Version Compatibilty
                if (component.MinimumVersion != null && version < component.MinimumVersion) return null;

                // Create a new Instance of the Component that will instantiate a new Derived class (if found)
                var obj = Create(component.Type);
                obj.Id = component.Id;
                obj.Uuid = component.Uuid;
                obj.Name = component.Name;
                obj.NativeName = component.NativeName;
                obj.Type = component.Type;
                obj.Parent = component.Parent;

                // Set Component Description
                if (component.Description != null)
                {
                    var description = new Description();
                    description.Manufacturer = component.Description.Manufacturer;
                    if (version >= MTConnectVersions.Version12) description.Model = component.Description.Model;
                    description.SerialNumber = component.Description.SerialNumber;
                    description.Station = component.Description.Station;
                    description.Value = component.Description.Value;
                    obj.Description = description;
                }

                if (version < MTConnectVersions.Version12) obj.SampleRate = component.SampleRate;
                if (version >= MTConnectVersions.Version12) obj.SampleInterval = component.SampleInterval;
                if (version >= MTConnectVersions.Version13) obj.References = component.References;
                if (version >= MTConnectVersions.Version17) obj.Configuration = component.Configuration;
                if (version >= MTConnectVersions.Version18) obj.CoordinateSystemIdRef = component.CoordinateSystemIdRef;

                // Add DataItems
                if (!component.DataItems.IsNullOrEmpty())
                {
                    var dataItems = new List<IDataItem>();

                    foreach (var dataItem in component.DataItems)
                    {
                        var dataItemObj = DataItem.Process(dataItem, version);
                        if (dataItemObj != null) dataItems.Add(dataItemObj);
                    }

                    obj.DataItems = dataItems;
                }

                // Add Compositions
                if (!component.Compositions.IsNullOrEmpty())
                {
                    var compositions = new List<IComposition>();

                    foreach (var composition in component.Compositions)
                    {
                        var compositionObj = Composition.Process(composition, version);
                        if (compositionObj != null) compositions.Add(compositionObj);
                    }

                    obj.Compositions = compositions;
                }

                // Add Components
                if (!component.Components.IsNullOrEmpty())
                {
                    var subcomponents = new List<IComponent>();

                    foreach (var subcomponent in component.Components)
                    {
                        var subcomponentObj = Process(subcomponent, version);
                        if (subcomponentObj != null) subcomponents.Add(subcomponentObj);
                    }

                    obj.Components = subcomponents;
                }

                return obj;
            }

            return null;
        }
    }
}