// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.References;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// An abstract XML Element.
    /// Replaced in the XML document by types of Component elements representing physical and logical parts of the Device.
    /// There can be multiple types of Component XML Elements in the document.
    /// </summary>
    public class JsonComponent
    {
        /// <summary>
        /// The unique identifier for this Component in the document.
        /// An id MUST be unique across all the id attributes in the document.
        /// An XML ID-type.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The type of component
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The name of the Component.
        /// Name is an optional attribute.
        /// If provided, Name MUST be unique within a type of Component or subComponent.
        /// It is recommended that duplicate names SHOULD NOT occur within a Device.
        /// An NMTOKEN XML type.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The name the device manufacturer assigned to the Component.
        /// If the native name is not provided it MUST be the Name.
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a component until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures. 
        /// If the sample interval is smaller than one millisecond, the number can be represented as a floating point number.
        /// For example, an interval of 100 microseconds would be 0.1.
        /// </summary>
        [JsonPropertyName("sampleInterval")]
        public double? SampleInterval { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY sampleInterval)
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        /// <summary>
        /// A unique identifier that will only refer to this Component.
        /// For example, this can be the manufacturer's code or the serial number.
        /// The uuid should be alphanumeric and not exceeding 255 characters.
        /// An NMTOKEN XML type.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// An element that can contain any descriptive content. 
        /// This can contain information about the Component and manufacturer specific details.
        /// </summary>
        [JsonPropertyName("description")]
        public IDescription Description { get; set; }

        /// <summary>
        /// An XML element that contains technical information about a piece of equipment describing its physical layout or functional characteristics.
        /// </summary>
        [JsonPropertyName("configuration")]
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// A container for the Data Entities associated with this Component element.
        /// </summary>
        [JsonPropertyName("dataItems")]
        public List<JsonDataItem> DataItems { get; set; }

        /// <summary>
        /// A container for Lower Level Component XML elements associated with this parent Component.
        /// </summary>
        [JsonPropertyName("components")]
        public List<JsonComponent> Components { get; set; }

        /// <summary>
        /// A container for the Composition elements associated with this Component element.
        /// </summary>
        [JsonPropertyName("compositions")]
        public List<JsonComposition> Compositions { get; set; }

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        [JsonPropertyName("references")]
        public List<IReference> References { get; set; }


        public JsonComponent() { }

        public JsonComponent(IComponent component)
        {
            if (component != null)
            {
                Id = component.Id;
                Uuid = component.Uuid;
                Name = component.Name;
                NativeName = component.NativeName;
                Type = component.Type;
                Description = component.Description;
                if (component.SampleRate > 0) SampleRate = component.SampleRate;
                if (component.SampleInterval > 0) SampleInterval = component.SampleInterval;
                if (!component.References.IsNullOrEmpty()) References = component.References.ToList();
                Configuration = component.Configuration;

                // DataItems
                if (!component.DataItems.IsNullOrEmpty())
                {
                    DataItems = new List<JsonDataItem>();

                    foreach (var dataItem in component.DataItems)
                    {
                        DataItems.Add(new JsonDataItem(dataItem));
                    }
                }

                // Compositions
                if (!component.Compositions.IsNullOrEmpty())
                {
                    Compositions = new List<JsonComposition>();

                    foreach (var composition in component.Compositions)
                    {
                        Compositions.Add(new JsonComposition(composition));
                    }
                }

                // Components
                if (!component.Components.IsNullOrEmpty())
                {
                    Components = new List<JsonComponent>();

                    foreach (var subcomponent in component.Components)
                    {
                        Components.Add(new JsonComponent(subcomponent));
                    }
                }
            }
        }

        public Component ToComponent()
        {
            var component = new Component();

            component.Id = Id;
            component.Uuid = Uuid;
            component.Name = Name;
            component.NativeName = NativeName;
            component.Type = Type;
            component.Description = Description;
            component.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            component.SampleInterval = SampleInterval.HasValue ? SampleInterval.Value : 0;
            component.References = References;
            component.Configuration = Configuration;

            // DataItems
            if (!DataItems.IsNullOrEmpty())
            {
                var dataItems = new List<DataItem>();
                foreach (var dataItem in DataItems)
                {
                    dataItems.Add(dataItem.ToDataItem());
                }
                component.DataItems = dataItems;
            }

            // Compositions
            if (!Compositions.IsNullOrEmpty())
            {
                var compositions = new List<Composition>();
                foreach (var composition in Compositions)
                {
                    compositions.Add(composition.ToComposition());
                }
                component.Compositions = compositions;
            }

            // Components
            if (!Components.IsNullOrEmpty())
            {
                var components = new List<Component>();
                foreach (var subcomponent in Components)
                {
                    components.Add(subcomponent.ToComponent());
                }
                component.Components = components;
            }

            return component;
        }
    }
}
