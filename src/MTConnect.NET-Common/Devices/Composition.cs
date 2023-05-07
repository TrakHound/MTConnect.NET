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
    /// <summary>
    /// Composition XML elements are used to describe the lowest level physical building blocks of a piece of equipment contained within a Component.
    /// </summary>
    public class Composition : IComposition
    {
        public const string DescriptionText = "Composition XML elements are used to describe the lowest level physical building blocks of a piece of equipment contained within a Component.";


        private static readonly Version DefaultMaximumVersion = null;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version14;
        private static readonly Dictionary<string, string> _typeIds = new Dictionary<string, string>();
        private static readonly object _lock = new object();

        private static Dictionary<string, Type> _types;


        public MTConnectEntityType EntityType => MTConnectEntityType.Composition;

        /// <summary>
        /// The unique identifier for this Component in the document.
        /// An id MUST be unique across all the id attributes in the document.
        /// An XML ID-type.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The type of component
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The name of the Component.
        /// Name is an optional attribute.
        /// If provided, Name MUST be unique within a type of Component or subComponent.
        /// It is recommended that duplicate names SHOULD NOT occur within a Device.
        /// An NMTOKEN XML type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name the device manufacturer assigned to the Component.
        /// If the native name is not provided it MUST be the Name.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a component until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures. 
        /// If the sample interval is smaller than one millisecond, the number can be represented as a floating point number.
        /// For example, an interval of 100 microseconds would be 0.1.
        /// </summary>
        public double SampleInterval { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY sampleInterval)
        /// </summary>
        public double SampleRate { get; set; }

        /// <summary>
        /// A unique identifier that will only refer to this Component.
        /// For example, this can be the manufacturer's code or the serial number.
        /// The uuid should be alphanumeric and not exceeding 255 characters.
        /// An NMTOKEN XML type.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Specifies the CoordinateSystem for this Composition and its children.
        /// </summary>
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// An element that can contain any descriptive content. 
        /// This can contain information about the Component and manufacturer specific details.
        /// </summary>
        public virtual IDescription Description { get; set; }

        /// <summary>
        /// An element that can contain descriptive content defining the configuration information for a Component.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        public virtual IEnumerable<IDataItem> DataItems { get; set; }

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        public IEnumerable<IReference> References { get; set; }


        /// <summary>
        /// The Container (Component or Device) that this Composition is directly associated with
        /// </summary>
        public IContainer Parent { get; set; }


        /// <summary>
        /// A MD5 Hash of the Composition that can be used to compare Composition objects
        /// </summary>
        public string ChangeId => CreateChangeId();


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


        public Composition()
        {
            Id = StringFunctions.RandomString(10);
            DataItems = new List<IDataItem>();
        }


        public string CreateChangeId()
        {
            return CreateChangeId(this);
        }

        public static string CreateChangeId(IComposition composition)
        {
            if (composition != null)
            {
                var ids = new List<string>();

                ids.Add(ObjectExtensions.GetChangeIdPropertyString(composition).ToMD5Hash());

                // Add DataItem Change Ids
                if (!composition.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in composition.DataItems)
                    {
                        ids.Add(dataItem.ChangeId);
                    }
                }

                return StringFunctions.ToMD5Hash(ids.ToArray());
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


        public static IComposition Process(IComposition composition, Version mtconnectVersion)
        {
            if (composition != null)
            {
                // Check Version Compatibilty
                if (composition.MinimumVersion != null && mtconnectVersion < composition.MinimumVersion) return null;

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
                    if (mtconnectVersion >= MTConnectVersions.Version12) description.Model = composition.Description.Model;
                    description.SerialNumber = composition.Description.SerialNumber;
                    description.Station = composition.Description.Station;
                    description.Value = composition.Description.Value;
                    obj.Description = description;
                }

                if (mtconnectVersion < MTConnectVersions.Version12) obj.SampleRate = composition.SampleRate;
                if (mtconnectVersion >= MTConnectVersions.Version12) obj.SampleInterval = composition.SampleInterval;
                if (mtconnectVersion >= MTConnectVersions.Version13) obj.References = composition.References;
                if (mtconnectVersion >= MTConnectVersions.Version17) obj.Configuration = composition.Configuration;
                if (mtconnectVersion >= MTConnectVersions.Version18) obj.CoordinateSystemIdRef = composition.CoordinateSystemIdRef;

                // Add DataItems
                if (!composition.DataItems.IsNullOrEmpty())
                {
                    var dataItems = new List<IDataItem>();

                    foreach (var dataItem in composition.DataItems)
                    {
                        var dataItemObj = DataItem.Process(dataItem, mtconnectVersion);
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

            return !l.IsNullOrEmpty() ? l : null;
        }

        private IEnumerable<IDataItem> GetDataItems(IComposition composition)
        {
            var l = new List<IDataItem>();

            // Add Root DataItems
            if (!composition.DataItems.IsNullOrEmpty()) l.AddRange(composition.DataItems);

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

                    // Check DataItem Source Value
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