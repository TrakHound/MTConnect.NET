// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.References;
using MTConnect.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.Devices
{
    public partial class Component : IComponent
    {
        private static readonly Version DefaultMaximumVersion = null;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version10;
        private static readonly Dictionary<string, string> _typeIds = new Dictionary<string, string>();
        private static readonly object _lock = new object();

        private static Dictionary<string, Type> _types;
        private static Dictionary<string, string> _typeDescriptions;


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


        public Component()
        {
            Id = StringFunctions.RandomString(10);
            Components = new List<IComponent>();
            Compositions = new List<IComposition>();
            DataItems = new List<IDataItem>();
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
        /// Add a Component to the Component
        /// </summary>
        /// <param name="component">The Component to add</param>
        public void AddComponent(IComponent component)
        {
            if (component != null)
            {
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
                var newComponents = new List<IComponent>();

                if (!Components.IsNullOrEmpty())
                {
                    newComponents.AddRange(Components);
                }

                newComponents.AddRange(components);
                Components = newComponents;
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

            var components = GetComponents();
            if (!components.IsNullOrEmpty())
            {
                foreach (var component in components)
                {
                    if (!component.Compositions.IsNullOrEmpty())
                    {
                        l.AddRange(component.Compositions);
                    }
                }
            }

            return !l.IsNullOrEmpty() ? l : null;
        }


        /// <summary>
        /// Add a Composition to the Component
        /// </summary>
        /// <param name="composition">The Composition to add</param>
        public void AddComposition(IComposition composition)
        {
            if (composition != null)
            {
                var compositions = new List<IComposition>();

                if (!Compositions.IsNullOrEmpty())
                {
                    compositions.AddRange(Compositions);
                }

                compositions.Add(composition);
                Compositions = compositions;
            }
        }

        /// <summary>
        /// Add Compositions to the Component
        /// </summary>
        /// <param name="compositions">The Compositions to add</param>
        public void AddCompositions(IEnumerable<IComposition> compositions)
        {
            if (!compositions.IsNullOrEmpty())
            {
                var newCompositions = new List<IComposition>();

                if (!Compositions.IsNullOrEmpty())
                {
                    newCompositions.AddRange(Compositions);
                }

                newCompositions.AddRange(compositions);
                Compositions = newCompositions;
            }
        }


        /// <summary>
        /// Remove a Composition from the Component
        /// </summary>
        /// <param name="compositionId">The ID of the Composition to remove</param>
        public void RemoveComposition(string compositionId)
        {
            var components = GetComponents();
            if (!components.IsNullOrEmpty())
            {
                foreach (var component in components)
                {
                    if (!component.Compositions.IsNullOrEmpty())
                    {
                        var compositions = new List<IComposition>();
                        compositions.AddRange(component.Compositions);
                        compositions.RemoveAll(o => o.Id == compositionId);
                        ((Component)component).AddCompositions(compositions);
                        //component.Compositions = compositions;
                    }
                }
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
        /// Add a DataItem to the Component
        /// </summary>
        /// <param name="dataItem">The DataItem to add</param>
        public void AddDataItem(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                var dataItems = new List<IDataItem>();

                if (!DataItems.IsNullOrEmpty())
                {
                    dataItems.AddRange(DataItems);
                }

                dataItems.Add(dataItem);
                DataItems = dataItems;
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
                var newDataItems = new List<IDataItem>();

                if (!DataItems.IsNullOrEmpty())
                {
                    newDataItems.AddRange(DataItems);
                }

                newDataItems.AddRange(dataItems);
                DataItems = newDataItems;
            }
        }


        /// <summary>
        /// Remove a DataItem from the Component
        /// </summary>
        /// <param name="dataItemId">The ID of the DataItem to remove</param>
        public void RemoveDataItem(string dataItemId)
        {
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

        public static IComponent Process(IComponent component, Version mtconnectVersion)
        {
            if (component != null)
            {
                // Check Version Compatibilty
                if (component.MinimumVersion != null && mtconnectVersion < component.MinimumVersion) return null;

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
                    if (mtconnectVersion >= MTConnectVersions.Version12) description.Model = component.Description.Model;
                    description.SerialNumber = component.Description.SerialNumber;
                    description.Station = component.Description.Station;
                    description.Value = component.Description.Value;
                    obj.Description = description;
                }

                if (mtconnectVersion < MTConnectVersions.Version12) obj.SampleRate = component.SampleRate;
                if (mtconnectVersion >= MTConnectVersions.Version12) obj.SampleInterval = component.SampleInterval;
                if (mtconnectVersion >= MTConnectVersions.Version13) obj.References = component.References;
                if (mtconnectVersion >= MTConnectVersions.Version17) obj.Configuration = component.Configuration;
                if (mtconnectVersion >= MTConnectVersions.Version18) obj.CoordinateSystemIdRef = component.CoordinateSystemIdRef;

                // Add DataItems
                if (!component.DataItems.IsNullOrEmpty())
                {
                    var dataItems = new List<IDataItem>();

                    foreach (var dataItem in component.DataItems)
                    {
                        var dataItemObj = DataItem.Process(dataItem, mtconnectVersion);
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
                        var compositionObj = Composition.Process(composition, mtconnectVersion);
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
                        var subcomponentObj = Process(subcomponent, mtconnectVersion);
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