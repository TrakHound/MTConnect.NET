// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MTConnect.Devices
{
    /// <summary>
    /// The primary container element of each device. 
    /// Device is contained within the top level Devices container. 
    /// There MAY be multiple Device elements in an XML document.
    /// </summary>
    public class Device
    {
        public const string TypeId = "Device";
        public const string DescriptionText = "The primary container element of each device. Device is contained within the top level Devices container. There MAY be multiple Device elements in an XML document.";


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
        public Description Description { get; set; }

        /// <summary>
        /// An XML element that contains technical information about a piece of equipment describing its physical layout or functional characteristics.
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// A container for the Data XML Elements provided by this Device.
        /// The data items define the measured values to be reported by this Device.
        /// </summary>
        public virtual List<DataItem> DataItems { get; set; }

        /// <summary>
        /// A container for SubComponent XML Elements.
        /// </summary>
        public virtual List<Component> Components { get; set; }

        /// <summary>
        /// A container for the Composition elements associated with this Device element.
        /// </summary>
        public virtual List<Composition> Compositions { get; set; }

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        public List<Reference> References { get; set; }

        /// <summary>
        /// A MD5 Hash of the Device that can be used to compare Device objects
        /// </summary>
        [JsonIgnore]
        public string ChangeId => CreateChangeId();

        [JsonIgnore]
        public virtual string TypeDescription => DescriptionText;



        public Device()
        {
            Type = TypeId;
            DataItems = new List<DataItem>();
            Components = new List<Component>();
            Compositions = new List<Composition>();
        }


        public string CreateChangeId()
        {
            return CreateChangeId(this);
        }

        public static string CreateChangeId(Device device)
        {
            if (device != null)
            {
                try
                {
                    var json = JsonSerializer.Serialize(device);
                    if (!string.IsNullOrEmpty(json))
                    {
                        return json.ToMD5Hash();
                    }
                }
                catch { }
            }

            return null;
        }


        /// <summary>
        /// Return a list of All DataItems
        /// </summary>
        public IEnumerable<DataItem> GetDataItems()
        {
            var l = new List<DataItem>();

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

        private IEnumerable<DataItem> GetDataItems(Component component)
        {
            var l = new List<DataItem>();

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
        /// Return a list of All Components
        /// </summary>
        public IEnumerable<Component> GetComponents()
        {
            var l = new List<Component>();

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

        private IEnumerable<Component> GetComponents(Component component)
        {
            var l = new List<Component>();

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


        #region "Types"

        public void AssignTypePaths()
        {
            // Set Root DataItems
            foreach (var dataItem in DataItems)
            {
                dataItem.TypePath = dataItem.Type;
            }

            // Set Root Components
            foreach (var component in Components)
            {
                AssignTypePaths(component.Type, component);
            }

            //// Set Root Components
            //foreach (var component in Components.Components)
            //{
            //    AssignTypePaths(component.Type, component);
            //}
        }

        private static void AssignTypePaths(string parentPath, Component component)
        {
            component.TypePath = parentPath;

            // Set Root DataItems
            foreach (var dataItem in component.DataItems)
            {
                dataItem.TypePath = parentPath + "/" + dataItem.Type;
            }

            // Set Root Components
            foreach (var subcomponent in component.Components)
            {
                AssignTypePaths(parentPath + "/" + subcomponent.Type, subcomponent);
            }

            //// Set Root Components
            //foreach (var subcomponent in component.SubComponents.Components)
            //{
            //    AssignTypePaths(parentPath + "/" + subcomponent.Type, subcomponent);
            //}
        }

        #endregion

        #region "Files"

        /// <summary>
        /// Gets a list of Devices from the specified file (ex. devices.xml)
        /// </summary>
        /// <param name="filePath">The path to the Device Configuration file</param>
        public static IEnumerable<Device> FromFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        var contents = File.ReadAllText(filePath);
                        if (!string.IsNullOrEmpty(contents))
                        {
                            var devicesDocument = DevicesDocument.FromXml(contents);
                            if (devicesDocument != null && devicesDocument.Devices != null && devicesDocument.Devices.Count() > 0)
                            {
                                var devices = new List<Device>();

                                foreach (var device in devicesDocument.Devices)
                                {
                                    devices.Add(device);
                                }

                                return devices;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

#if NETCOREAPP1_0_OR_GREATER

        /// <summary>
        /// Gets a list of Devices from the specified file (ex. devices.xml)
        /// </summary>
        /// <param name="filePath">The path to the Device Configuration file</param>
        public static async Task<IEnumerable<Device>> FromFileAsync(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        var contents = await File.ReadAllTextAsync(filePath);
                        if (!string.IsNullOrEmpty(contents))
                        {
                            var devicesDocument = DevicesDocument.FromXml(contents);
                            if (devicesDocument != null && devicesDocument.Devices != null && devicesDocument.Devices.Count() > 0)
                            {
                                var devices = new List<Device>();

                                foreach (var device in devicesDocument.Devices)
                                {
                                    devices.Add(device);
                                }

                                return devices;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

#endif

        /// <summary>
        /// Gets a list of Devices from files (ex. devices.xml) found in the specified directory path
        /// </summary>
        /// <param name="dirPath">The path to the directory containing Device Configuration files</param>
        public static IEnumerable<Device> FromFiles(string dirPath)
        {
            if (!string.IsNullOrEmpty(dirPath))
            {
                try
                {
                    if (Directory.Exists(dirPath))
                    {
                        var files = Directory.GetFiles(dirPath, "*.xml");
                        if (files != null && files.Length > 0)
                        {
                            foreach (var file in files)
                            {
                                var contents = File.ReadAllText(file);
                                if (!string.IsNullOrEmpty(contents))
                                {
                                    var devicesDocument = DevicesDocument.FromXml(contents);
                                    if (devicesDocument != null && devicesDocument.Devices != null && devicesDocument.Devices.Count() > 0)
                                    {
                                        var devices = new List<Device>();

                                        foreach (var device in devicesDocument.Devices)
                                        {
                                            devices.Add(device);
                                        }

                                        return devices;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

#if NETCOREAPP1_0_OR_GREATER

        /// <summary>
        /// Gets a list of Devices from files (ex. devices.xml) found in the specified directory path
        /// </summary>
        /// <param name="dirPath">The path to the directory containing Device Configuration files</param>
        public static async Task<IEnumerable<Device>> FromFilesAsync(string dirPath)
        {
            if (!string.IsNullOrEmpty(dirPath))
            {
                try
                {
                    if (Directory.Exists(dirPath))
                    {
                        var files = Directory.GetFiles(dirPath, "*.xml");
                        if (files != null && files.Length > 0)
                        {
                            foreach (var file in files)
                            {
                                var contents = await File.ReadAllTextAsync(file);
                                if (!string.IsNullOrEmpty(contents))
                                {
                                    var devicesDocument = DevicesDocument.FromXml(contents);
                                    if (devicesDocument != null && devicesDocument.Devices != null && devicesDocument.Devices.Count() > 0)
                                    {
                                        var devices = new List<Device>();

                                        foreach (var device in devicesDocument.Devices)
                                        {
                                            devices.Add(device);
                                        }

                                        return devices;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

#endif

        #endregion

        public static Device Process(Device device, Version version)
        {
            if (device != null)
            {
                Device obj = null;

                if (device.Type == TypeId) obj = new Device();
                else if (device.Type == Agent.TypeId) obj = new Agent();

                if (obj != null)
                {
                    obj.Id = device.Id;
                    obj.Name = device.Name;
                    obj.NativeName = device.NativeName;
                    obj.Uuid = device.Uuid;
                    obj.Type = device.Type;
                    obj.Description = device.Description;
                    obj.SampleRate = device.SampleRate;
                    obj.SampleInterval = device.SampleInterval;
                    obj.Iso841Class = device.Iso841Class;
                    obj.CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                    obj.MTConnectVersion = device.MTConnectVersion;
                    obj.Configuration = device.Configuration;
                    obj.References = device.References;
                    obj.Description = device.Description;

                    if (!device.DataItems.IsNullOrEmpty())
                    {
                        var dataItems = new List<DataItem>();

                        foreach (var dataItem in device.DataItems)
                        {
                            var dataItemObj = DataItem.Process(dataItem, version);
                            if (dataItemObj != null) dataItems.Add(dataItemObj);
                        }

                        obj.DataItems = dataItems;
                    }

                    if (!device.Compositions.IsNullOrEmpty())
                    {
                        var compositions = new List<Composition>();

                        foreach (var composition in device.Compositions)
                        {
                            var compositionObj = Composition.Process(composition, version);
                            if (compositionObj != null) compositions.Add(compositionObj);
                        }

                        obj.Compositions = compositions;
                    }

                    if (!device.Components.IsNullOrEmpty())
                    {
                        var components = new List<Component>();

                        foreach (var component in device.Components)
                        {
                            var componentObj = Component.Process(component, version);
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
