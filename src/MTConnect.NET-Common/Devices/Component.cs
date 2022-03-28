// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.Devices
{
    /// <summary>
    /// An abstract XML Element.
    /// Replaced in the XML document by types of Component elements representing physical and logical parts of the Device.
    /// There can be multiple types of Component XML Elements in the document.
    /// </summary>
    public class Component : IComponent
    {
        public const string DescriptionText = "An abstract XML Element. Replaced in the XML document by types of Component elements representing physical and logical parts of the Device. There can be multiple types of Component XML Elements in the document.";

        private static readonly Version DefaultMaximumVersion = MTConnectVersions.Max;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version10;

        private static Dictionary<string, Type> _types;


        /// <summary>
        /// The path of the Component by Type
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public string TypePath { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual Version MaximumVersion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual Version MinimumVersion { get; set; }


        /// <summary>
        /// The unique identifier for this Component in the document.
        /// An id MUST be unique across all the id attributes in the document.
        /// An XML ID-type.
        /// </summary>
        [XmlAttribute("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The type of component
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The name of the Component.
        /// Name is an optional attribute.
        /// If provided, Name MUST be unique within a type of Component or subComponent.
        /// It is recommended that duplicate names SHOULD NOT occur within a Device.
        /// An NMTOKEN XML type.
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The name the device manufacturer assigned to the Component.
        /// If the native name is not provided it MUST be the Name.
        /// </summary>
        [XmlAttribute("nativeName")]
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a component until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures. 
        /// If the sample interval is smaller than one millisecond, the number can be represented as a floating point number.
        /// For example, an interval of 100 microseconds would be 0.1.
        /// </summary>
        [XmlAttribute("sampleInterval")]
        [JsonPropertyName("sampleInterval")]
        public double SampleInterval { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY sampleInterval)
        /// </summary>
        [XmlAttribute("sampleRate")]
        [JsonPropertyName("sampleRate")]
        public double SampleRate { get; set; }

        /// <summary>
        /// A unique identifier that will only refer to this Component.
        /// For example, this can be the manufacturer's code or the serial number.
        /// The uuid should be alphanumeric and not exceeding 255 characters.
        /// An NMTOKEN XML type.
        /// </summary>
        [XmlAttribute("uuid")]
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        [XmlAttribute("coordinateSystemIdRef")]
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// An element that can contain any descriptive content. 
        /// This can contain information about the Component and manufacturer specific details.
        /// </summary>
        [XmlElement("Description")]
        [JsonPropertyName("description")]
        public virtual IDescription Description { get; set; }

        /// <summary>
        /// An XML element that contains technical information about a piece of equipment describing its physical layout or functional characteristics.
        /// </summary>
        [XmlElement("Configuration")]
        [JsonPropertyName("configuration")]
        public IConfiguration Configuration { get; set; }

        [XmlIgnore]
        [JsonPropertyName("dataItems")]
        public virtual IEnumerable<IDataItem> DataItems { get; set; }

        [XmlIgnore]
        [JsonPropertyName("components")]
        public virtual IEnumerable<IComponent> Components { get; set; }

        /// <summary>
        /// A container for the Composition elements associated with this Component element.
        /// </summary>
        [XmlArray("Compositions")]
        [XmlArrayItem("Composition", typeof(Composition))]
        [JsonPropertyName("compositions")]
        public virtual IEnumerable<IComposition> Compositions { get; set; }

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        [XmlArray("References")]
        [XmlArrayItem("ComponentReference", typeof(ComponentReference))]
        [XmlArrayItem("DataItemReference", typeof(DataItemReference))]
        [JsonPropertyName("references")]
        public IEnumerable<IReference> References { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string TypeDescription => DescriptionText;



        public Component()
        {
            Components = new List<IComponent>();
            Compositions = new List<IComposition>();
            DataItems = new List<IDataItem>();
            MaximumVersion = DefaultMaximumVersion;
            MinimumVersion = DefaultMinimumVersion;
        }


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



        public static Component Create(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    var titleType = type.ToPascalCase();

                    if (_types.TryGetValue(titleType, out Type t))
                    {
                        var constructor = t.GetConstructor(System.Type.EmptyTypes);
                        if (constructor != null)
                        {
                            try
                            {
                                return (Component)Activator.CreateInstance(t);
                            }
                            catch { }
                        }
                    }
                }
            }

            return new Component();
        }

        private static Dictionary<string, Type> GetAllTypes()
        {
            var assemblies = Assemblies.Get();
            if (!assemblies.IsNullOrEmpty())
            {
                var allTypes = assemblies.SelectMany(x => x.GetTypes());

                var types = allTypes.Where(x => typeof(Component).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
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


        public static IComponent Process(IComponent component, Version mtconnectVersion)
        {
            if (component != null)
            {
                var obj = Create(component.Type);
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
                    if (mtconnectVersion >= MTConnectVersions.Version12) description.Model = component.Description.Model;
                    description.SerialNumber = component.Description.SerialNumber;
                    description.Station = component.Description.Station;
                    description.CDATA = component.Description.CDATA;
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

                // Don't return an Empty Component
                if (obj.Components.IsNullOrEmpty() && obj.Compositions.IsNullOrEmpty() && obj.DataItems.IsNullOrEmpty())
                {
                    return null;
                }

                // Check Version Compatibilty
                if (mtconnectVersion >= obj.MinimumVersion && mtconnectVersion <= obj.MaximumVersion)
                {
                    return obj;
                }
            }

            return null;
        }
    }
}
