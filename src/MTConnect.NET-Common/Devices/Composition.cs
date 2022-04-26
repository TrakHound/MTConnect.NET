// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// Composition XML elements are used to describe the lowest level physical building blocks of a piece of equipment contained within a Component.
    /// </summary>
    public class Composition : IComposition
    {
        public const string DescriptionText = "Composition XML elements are used to describe the lowest level physical building blocks of a piece of equipment contained within a Component.";


        private static readonly Version DefaultMaximumVersion = MTConnectVersions.Max;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version14;

        private static Dictionary<string, Type> _types;


        /// <summary>
        /// The XPath address of the Component
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public string XPath { get; set; }

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
        /// An element that can contain any descriptive content. 
        /// This can contain information about the Component and manufacturer specific details.
        /// </summary>
        [XmlElement("Description")]
        [JsonPropertyName("description")]
        public virtual IDescription Description { get; set; }

        /// <summary>
        /// An element that can contain descriptive content defining the configuration information for a Component.
        /// </summary>
        [XmlElement("Configuration")]
        [JsonPropertyName("configuration")]
        public IConfiguration Configuration { get; set; }

        [XmlIgnore]
        [JsonPropertyName("dataItems")]
        public virtual IEnumerable<IDataItem> DataItems { get; set; }

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
        public bool ReferencesSpecified => !References.IsNullOrEmpty();

        [XmlIgnore]
        [JsonIgnore]
        public virtual string TypeDescription => DescriptionText;



        public Composition()
        {
            DataItems = new List<IDataItem>();
            MaximumVersion = DefaultMaximumVersion;
            MinimumVersion = DefaultMinimumVersion;
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


        public static Composition Create(string type)
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
                                return (Composition)Activator.CreateInstance(t);
                            }
                            catch { }
                        }
                    }
                }
            }

            return new Composition();
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
                var obj = new Composition();
                obj.Id = composition.Id;
                obj.Uuid = composition.Uuid;
                obj.Name = composition.Name;
                obj.NativeName = composition.NativeName;
                obj.Type = composition.Type;

                // Set Composition Description
                if (composition.Description != null)
                {
                    var description = new Description();
                    description.Manufacturer = composition.Description.Manufacturer;
                    if (mtconnectVersion >= MTConnectVersions.Version12) description.Model = composition.Description.Model;
                    description.SerialNumber = composition.Description.SerialNumber;
                    description.Station = composition.Description.Station;
                    description.CDATA = composition.Description.CDATA;
                    obj.Description = description;
                }

                obj.SampleRate = composition.SampleRate;
                obj.SampleInterval = composition.SampleInterval;
                obj.References = composition.References;
                if (mtconnectVersion >= MTConnectVersions.Version17) obj.Configuration = composition.Configuration;

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
