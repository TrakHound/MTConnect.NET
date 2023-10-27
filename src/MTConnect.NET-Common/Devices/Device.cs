// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Devices
{
    public partial class Device : IDevice
    {
        public const string TypeId = "Device";
        private static readonly Version DefaultMaximumVersion = null;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version10;

        public MTConnectEntityType EntityType => MTConnectEntityType.Device;


        /// <summary>
        /// The Agent InstanceId that produced this Device
        /// </summary>
        public long InstanceId { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.1
        /// </summary>
        public string Iso841Class { get; set; }

        public IEnumerable<IDataItem> DataItems { get; set; }

        /// <summary>
        /// The Type of Device
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The Container that this Device is directly associated with
        /// </summary>
        public IContainer Parent { get; set; }


        /// <summary>
        /// The text description that describes what the Device Type represents
        /// </summary>
        public virtual string TypeDescription => DescriptionText;

        /// <summary>
        /// Gets whether the Device is an Organizer Type
        /// </summary>
        public bool IsOrganizer => false;


        /// <summary>
        /// The full path of IDs that describes the location of the Device in the Device
        /// </summary>
        public string IdPath => Id;

        /// <summary>
        /// The list of IDs (in order) that describes the location of the Device in the Device
        /// </summary>
        public string[] IdPaths => new string[] { Id };

        /// <summary>
        /// The full path of Types that describes the location of the Device in the Device
        /// </summary>
        public string TypePath => Type;

        /// <summary>
        /// The list of Types (in order) that describes the location of the Device in the Device
        /// </summary>
        public string[] TypePaths => new string[] { Type };


        /// <summary>
        /// The maximum MTConnect Version that this Device Type is valid 
        /// (if set, this indicates that the Type has been Deprecated in the MTConnect Standard version specified)
        /// </summary>
        public virtual Version MaximumVersion => DefaultMaximumVersion;

        /// <summary>
        /// The minimum MTConnect Version that this Device Type is valid 
        /// </summary>
        public virtual Version MinimumVersion => DefaultMinimumVersion;


        public Device()
        {
            Id = StringFunctions.RandomString(10);
            Name = "dev";
            Uuid = Guid.NewGuid().ToString();
            Type = TypeId;
            DataItems = new List<IDataItem>();
            Components = new List<IComponent>();
            Compositions = new List<IComposition>();
        }


        public string GenerateHash()
        {
            return GenerateHash(this);
        }

        public static string GenerateHash(IDevice device)
        {
            if (device != null)
            {
                var ids = new List<string>();

                var hashMembers = new string[10];
                hashMembers[0] = $"uuid:{device.Uuid}";
                hashMembers[1] = $"id:{device.Id}";
                hashMembers[2] = $"name:{device.Name}";
                hashMembers[3] = $"type:{device.Type}";
                hashMembers[4] = $"nativeName:{device.NativeName}";
                hashMembers[5] = $"sampleInterval:{device.SampleInterval}";
                hashMembers[6] = $"sampleRate:{device.SampleRate}";
                hashMembers[7] = $"coordinateSystemIdRef:{device.CoordinateSystemIdRef}";
                hashMembers[8] = $"iso841Class:{device.Iso841Class}";
                hashMembers[9] = $"mtconnectVersion:{device.MTConnectVersion}";
                var propertiesHash = string.Join(";", hashMembers).ToSHA1Hash();
                ids.Add(propertiesHash);

                // Add DataItem Change Ids
                if (!device.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in device.DataItems)
                    {
                        ids.Add(dataItem.Hash);
                    }
                }

                // Add Composition Change Ids
                if (!device.Compositions.IsNullOrEmpty())
                {
                    foreach (var composition in device.Compositions)
                    {
                        ids.Add(composition.Hash);
                    }
                }

                // Add Component Change Ids
                if (!device.Components.IsNullOrEmpty())
                {
                    foreach (var component in device.Components)
                    {
                        ids.Add(component.Hash);
                    }
                }

                return StringFunctions.ToSHA1Hash(ids.ToArray());
            }

            return null;
        }

        public static string CreateDeviceHash(IDevice device)
        {
            var s = ObjectExtensions.GetHashPropertyString(device);
            return s.ToMD5Hash();
        }


        #region "Components"

        /// <summary>
        /// Return a list of All Components
        /// </summary>
        public IEnumerable<IComponent> GetComponents()
        {
            return GetComponents(this);
        }

        private static List<IComponent> GetComponents(IDevice device)
        {
            var l = new List<IComponent>();

            if (device != null && !device.Components.IsNullOrEmpty())
            {
                foreach (var subComponent in device.Components)
                {
                    var components = GetComponents(subComponent);
                    if (components != null && components.Count > 0) l.AddRange(components);
                }
            }
            return l.Count > 0 ? l : null;
        }

        private static List<IComponent> GetComponents(IComponent component)
        {
            var l = new List<IComponent>();
            l.Add(component);

            if (!component.Components.IsNullOrEmpty())
            {
                foreach (var subComponent in component.Components)
                {
                    var components = GetComponents(subComponent);
                    if (components != null && components.Count > 0) l.AddRange(components);
                }
            }
            return l.Count > 0 ? l : null;
        }


        /// <summary>
        /// Add a Component to the Device
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

                var organizerType = Organizers.GetOrganizerType(component.Type);
                if (organizerType != null)
                {
                    if (!components.Any(o => o.Type == organizerType))
                    {
                        var organizer = Component.Create(organizerType);
                        if (organizer != null)
                        {
                            organizer.AddComponent(component);
                            components.Add(organizer);
                        }
                    }
                    else 
                    {
                        var organizer = (Component)components.FirstOrDefault(o => o.Type == organizerType);
                        if (organizer != null)
                        {
                            organizer.AddComponent(component);
                        }
                    }
                }
                else
                {
                    components.Add(component);
                }

                Components = components;
            }
        }

        /// <summary>
        /// Add Components to the Device
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
        /// Remove a Component from the Device
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

                ((Component)component).Components = components;
                //component.Components = components;
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
        /// Add a Composition to the Device
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
        /// Add Compositions to the Device
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
        /// Remove a Composition from the Device
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
                        ((Component)component).Compositions = compositions;
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

        private static IEnumerable<IDataItem> GetDataItems(IComponent component)
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

                    // Check DataItem Source Value
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Source != null && o.Source.Value == dataItemKey);

                    // Return DataItem
                    return dataItem;
                }
            }

            return null;
        }


        /// <summary>
        /// Add a DataItem to the Device
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
        /// Add DataItems to the Device
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
        /// Remove a DataItem from the Device
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


        public static Device Process(IDevice device, Version mtconnectVersion)
        {
            if (device != null)
            {
                Device obj = null;

                if (device.Type == TypeId) obj = new Device();
                else if (device.Type == Agent.TypeId) obj = new Agent();

                // Don't Ouput Agent Device if Version < 1.7
                if (device.Type == Agent.TypeId && mtconnectVersion < MTConnectVersions.Version17) return null;

                if (obj != null)
                {
                    obj.Id = device.Id;
                    obj.Name = device.Name;
                    obj.NativeName = device.NativeName;
                    obj.Uuid = device.Uuid;
                    obj.InstanceId = device.InstanceId;
                    obj.Type = device.Type;
                    obj.Parent = device;

                    // Set Device Description
                    if (device.Description != null)
                    {
                        var description = new Description();
                        description.Manufacturer = device.Description.Manufacturer;
                        if (mtconnectVersion >= MTConnectVersions.Version12) description.Model = device.Description.Model;
                        description.SerialNumber = device.Description.SerialNumber;
                        description.Station = device.Description.Station;
                        description.Value = device.Description.Value;
                        obj.Description = description;
                    }

                    if (mtconnectVersion < MTConnectVersions.Version12) obj.Iso841Class = device.Iso841Class;
                    if (mtconnectVersion < MTConnectVersions.Version12) obj.SampleRate = device.SampleRate;
                    if (mtconnectVersion >= MTConnectVersions.Version12) obj.SampleInterval = device.SampleInterval;
                    if (mtconnectVersion >= MTConnectVersions.Version13) obj.References = device.References;
                    if (mtconnectVersion >= MTConnectVersions.Version17) obj.Configuration = device.Configuration;
                    if (mtconnectVersion >= MTConnectVersions.Version18) obj.CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                    obj.MTConnectVersion = device.MTConnectVersion != null ? device.MTConnectVersion : mtconnectVersion.ToString();
                    if (mtconnectVersion >= MTConnectVersions.Version22) obj.Hash = device.Hash;

                    // Add DataItems
                    if (!device.DataItems.IsNullOrEmpty())
                    {
                        var dataItems = new List<IDataItem>();

                        foreach (var dataItem in device.DataItems)
                        {
                            var dataItemObj = DataItem.Process(dataItem, mtconnectVersion);
                            if (dataItemObj != null) dataItems.Add(dataItemObj);
                        }

                        obj.DataItems = dataItems;
                    }

                    // Add Compositions
                    if (!device.Compositions.IsNullOrEmpty())
                    {
                        var compositions = new List<IComposition>();

                        foreach (var composition in device.Compositions)
                        {
                            var compositionObj = Composition.Process(composition, mtconnectVersion);
                            if (compositionObj != null) compositions.Add(compositionObj);
                        }

                        obj.Compositions = compositions;
                    }

                    // Add Components
                    if (!device.Components.IsNullOrEmpty())
                    {
                        var components = new List<IComponent>();

                        foreach (var component in device.Components)
                        {
                            var componentObj = Component.Process(component, mtconnectVersion);
                            if (componentObj != null) components.Add(componentObj);
                        }

                        obj.Components = components;
                    }

                    return obj;
                }
            }

            return null;
        }
    }
}