// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// The primary container element of each device. 
    /// Device is contained within the top level Devices container. 
    /// There MAY be multiple Device elements in an XML document.
    /// </summary>
    public class Device : IDevice
    {
        public const string TypeId = "Device";
        public const string DescriptionText = "The primary container element of each device. Device is contained within the top level Devices container. There MAY be multiple Device elements in an XML document.";

        private static readonly Version DefaultMaximumVersion = MTConnectVersions.Max;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version10;

        public virtual Version MaximumVersion => DefaultMaximumVersion;

        public virtual Version MinimumVersion => DefaultMinimumVersion;


        /// <summary>
        /// The unique identifier for this Device in the document.
        /// An id MUST be unique across all the id attributes in the document.
        /// An XML ID-type.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the Device.
        /// THis name should be unique within the XML document to allow for easier data integration.
        /// An NMTOKEN XML type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique identifier that will only refer ot this Device.
        /// For example, this may be the manufacturer's code and the serial number.
        /// The uuid shoudl be alphanumeric and not exceeding 255 characters.
        /// An NMTOKEN XML type.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// The type of Device
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.1
        /// </summary>
        public string Iso841Class { get; set; }

        /// <summary>
        /// The name the device manufacturer assigned to this Device.
        /// If the native name is not provided, it MUST be the name.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a device until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures.
        /// If the sample interval is smaller than one millisecond, the number can be represented as a floating point number.
        /// For example, an interval of 100 microseconds would be 0.1.
        /// </summary>
        public double SampleInterval { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)
        /// </summary>
        public double SampleRate { get; set; }

        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The MTConnect version of the Devices Information Model used to configure
        /// the information to be published for a piece of equipment in an MTConnect Response Document.
        /// </summary>
        public virtual Version MTConnectVersion { get; set; }

        /// <summary>
        /// An element that can contain any descriptive content. 
        /// This can contain information about the Component and manufacturer specific details.
        /// </summary>
        public virtual IDescription Description { get; set; }

        /// <summary>
        /// An XML element that contains technical information about a piece of equipment describing its physical layout or functional characteristics.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// A container for the Data XML Elements provided by this Device.
        /// The data items define the measured values to be reported by this Device.
        /// </summary>
        public virtual IEnumerable<IDataItem> DataItems { get; set; }

        /// <summary>
        /// A container for SubComponent XML Elements.
        /// </summary>
        public virtual IEnumerable<IComponent> Components { get; set; }

        /// <summary>
        /// A container for the Composition elements associated with this Device element.
        /// </summary>
        public virtual IEnumerable<IComposition> Compositions { get; set; }

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        public IEnumerable<IReference> References { get; set; }


        /// <summary>
        /// The Parent of this Device
        /// </summary>
        [JsonIgnore]
        public IContainer Parent { get; set; }


        /// <summary>
        /// A MD5 Hash of the Device that can be used to compare Device objects
        /// </summary>
        [JsonIgnore]
        public string ChangeId => CreateChangeId();

        /// <summary>
        /// A description specific to the type of Device
        /// </summary>
        [JsonIgnore]
        public virtual string TypeDescription => DescriptionText;

        [JsonIgnore]
        public bool IsOrganizer => false;


        [JsonIgnore]
        public string IdPath => Id;

        [JsonIgnore]
        public string[] IdPaths => new string[] { Id };

        [JsonIgnore]
        public string TypePath => Type;

        [JsonIgnore]
        public string[] TypePaths => new string[] { Type };


        public Device()
        {
            Type = TypeId;
            DataItems = new List<IDataItem>();
            Components = new List<IComponent>();
            Compositions = new List<IComposition>();
        }


        public string CreateChangeId()
        {
            return CreateChangeId(this);
        }

        public static string CreateChangeId(IDevice device)
        {
            if (device != null)
            {
                var ids = new List<string>();

                ids.Add(ObjectExtensions.GetChangeIdPropertyString(device).ToMD5Hash());

                // Add DataItem Change Ids
                if (!device.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in device.DataItems)
                    {
                        ids.Add(dataItem.ChangeId);
                    }
                }

                // Add Composition Change Ids
                if (!device.Compositions.IsNullOrEmpty())
                {
                    foreach (var composition in device.Compositions)
                    {
                        ids.Add(composition.ChangeId);
                    }
                }

                // Add Component Change Ids
                if (!device.Components.IsNullOrEmpty())
                {
                    foreach (var component in device.Components)
                    {
                        ids.Add(component.ChangeId);
                    }
                }

                return StringFunctions.ToMD5Hash(ids.ToArray());
            }

            return null;
        }

        public static string CreateDeviceChangeId(IDevice device)
        {
            var s = ObjectExtensions.GetChangeIdPropertyString(device);
            return s.ToMD5Hash();
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

                components.Add(component);
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

                component.Components = components;
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
                        component.Compositions = compositions;
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

                    // Check DataItem Source CDATA
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Source != null && o.Source.CDATA == dataItemKey);

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
                    obj.Type = device.Type;

                    // Set Device Description
                    if (device.Description != null)
                    {
                        var description = new Description();
                        description.Manufacturer = device.Description.Manufacturer;
                        if (mtconnectVersion >= MTConnectVersions.Version12) description.Model = device.Description.Model;
                        description.SerialNumber = device.Description.SerialNumber;
                        description.Station = device.Description.Station;
                        description.CDATA = device.Description.CDATA;
                        obj.Description = description;
                    }

                    if (mtconnectVersion < MTConnectVersions.Version12) obj.Iso841Class = device.Iso841Class;
                    if (mtconnectVersion < MTConnectVersions.Version12) obj.SampleRate = device.SampleRate;
                    if (mtconnectVersion >= MTConnectVersions.Version12) obj.SampleInterval = device.SampleInterval;
                    if (mtconnectVersion >= MTConnectVersions.Version13) obj.References = device.References;
                    if (mtconnectVersion >= MTConnectVersions.Version17) obj.Configuration = device.Configuration;
                    if (mtconnectVersion >= MTConnectVersions.Version18) obj.CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                    if (mtconnectVersion >= MTConnectVersions.Version17) obj.MTConnectVersion = device.MTConnectVersion;

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
