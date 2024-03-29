// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Collections.Generic;
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
        public JsonDescription Description { get; set; }

        /// <summary>
        /// An XML element that contains technical information about a piece of equipment describing its physical layout or functional characteristics.
        /// </summary>
        [JsonPropertyName("configuration")]
        public JsonConfiguration Configuration { get; set; }

        /// <summary>
        /// A container for the Data Entities associated with this Component element.
        /// </summary>
        [JsonPropertyName("dataItems")]
        public IEnumerable<JsonDataItem> DataItems { get; set; }

        /// <summary>
        /// A container for Lower Level Component XML elements associated with this parent Component.
        /// </summary>
        [JsonPropertyName("components")]
        public IEnumerable<JsonComponent> Components { get; set; }

        /// <summary>
        /// A container for the Composition elements associated with this Component element.
        /// </summary>
        [JsonPropertyName("compositions")]
        public IEnumerable<JsonComposition> Compositions { get; set; }

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        [JsonPropertyName("references")]
        public IEnumerable<JsonReference> References { get; set; }


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
                if (component.Description != null) Description = new JsonDescription(component.Description);
                if (component.SampleRate > 0) SampleRate = component.SampleRate;
                if (component.SampleInterval > 0) SampleInterval = component.SampleInterval;

                // References
                if (!component.References.IsNullOrEmpty())
                {
                    var references = new List<JsonReference>();
                    foreach (var reference in component.References)
                    {
                        references.Add(new JsonReference(reference));
                    }
                    References = references;
                }

                // Configuration
                if (component.Configuration != null) Configuration = new JsonConfiguration(component.Configuration);

                // DataItems
                if (!component.DataItems.IsNullOrEmpty())
                {
                    var dataItems = new List<JsonDataItem>();
                    foreach (var dataItem in component.DataItems)
                    {
                        dataItems.Add(new JsonDataItem(dataItem));
                    }
                    DataItems = dataItems;
                }

                // Compositions
                if (!component.Compositions.IsNullOrEmpty())
                {
                    var compositions = new List<JsonComposition>();
                    foreach (var composition in component.Compositions)
                    {
                        compositions.Add(new JsonComposition(composition));
                    }
                    Compositions = compositions;
                }

                // Components
                if (!component.Components.IsNullOrEmpty())
                {
                    var subcomponents = new List<JsonComponent>();
                    foreach (var subcomponent in component.Components)
                    {
                        subcomponents.Add(new JsonComponent(subcomponent));
                    }
                    Components = subcomponents;
                }
            }
        }


        public override string ToString() => JsonFunctions.Convert(this);

        public Component ToComponent()
        {
            var component = new Component();

            component.Id = Id;
            component.Uuid = Uuid;
            component.Name = Name;
            component.NativeName = NativeName;
            component.Type = Type;
            if (Description != null) component.Description = Description.ToDescription();
            component.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            component.SampleInterval = SampleInterval.HasValue ? SampleInterval.Value : 0;

            // References
            if (!References.IsNullOrEmpty())
            {
                var references = new List<IReference>();
                foreach (var reference in References)
                {
                    references.Add(reference.ToReference());
                }
                component.References = references;
            }

            // Configuration
            if (Configuration != null) component.Configuration = Configuration.ToConfiguration();

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