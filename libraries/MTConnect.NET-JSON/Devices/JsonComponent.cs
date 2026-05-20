// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect <c>Component</c>. Mirrors a
    /// Component element of an MTConnectDevices document so the JSON serializer
    /// can read and write the on-the-wire shape, then converts to and from the
    /// strongly-typed <see cref="Component"/> model. Components nest recursively
    /// and carry the device's data items and compositions.
    /// </summary>
    public class JsonComponent
    {
        /// <summary>
        /// The unique <c>id</c> of the component within the device.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The MTConnect component <c>type</c> (for example Axes, Controller,
        /// or Linear).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The optional human-readable <c>name</c> of the component. Omitted
        /// from the JSON output when not set.
        /// </summary>
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Name { get; set; }

        /// <summary>
        /// The name the component is known by in the native data source, when
        /// it differs from <see cref="Name"/>.
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The interval, in milliseconds, between samples of the component's
        /// data, when reported.
        /// </summary>
        [JsonPropertyName("sampleInterval")]
        public double? SampleInterval { get; set; }

        /// <summary>
        /// The rate, in samples per second, at which the component's data is
        /// sampled, when reported.
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        /// <summary>
        /// The universally unique identifier of the component instance.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of a CoordinateSystem the component's
        /// values are expressed relative to.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The descriptive metadata (manufacturer, model, serial number) for
        /// the component.
        /// </summary>
        [JsonPropertyName("description")]
        public JsonDescription Description { get; set; }

        /// <summary>
        /// The configuration (coordinate systems, motion, relationships,
        /// specifications) of the component.
        /// </summary>
        [JsonPropertyName("configuration")]
        public JsonConfiguration Configuration { get; set; }

        /// <summary>
        /// The data items reported directly by the component.
        /// </summary>
        [JsonPropertyName("dataItems")]
        public IEnumerable<JsonDataItem> DataItems { get; set; }

        /// <summary>
        /// The child components nested within the component.
        /// </summary>
        [JsonPropertyName("components")]
        public IEnumerable<JsonComponent> Components { get; set; }

        /// <summary>
        /// The compositions (lower-level structural elements) of the component.
        /// </summary>
        [JsonPropertyName("compositions")]
        public IEnumerable<JsonComposition> Compositions { get; set; }

        /// <summary>
        /// The references from the component to other components and data
        /// items.
        /// </summary>
        [JsonPropertyName("references")]
        public JsonReferenceContainer References { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonComponent() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IComponent"/>, recursively converting the description,
        /// configuration, references, data items, compositions, and child
        /// components.
        /// </summary>
        public JsonComponent(IComponent component)
        {
            if (component != null)
            {
                Id = component.Id;
                Uuid = component.Uuid;
                if (!string.IsNullOrEmpty(component.Name)) Name = component.Name;
                NativeName = component.NativeName;
                Type = component.Type;
                if (component.Description != null) Description = new JsonDescription(component.Description);
                if (component.SampleRate > 0) SampleRate = component.SampleRate;
                if (component.SampleInterval > 0) SampleInterval = component.SampleInterval;

                // References
                if (!component.References.IsNullOrEmpty()) References = new JsonReferenceContainer(component.References);

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


        /// <summary>
        /// Returns the JSON representation of this surrogate.
        /// </summary>
        public override string ToString() => JsonFunctions.Convert(this);

        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Component"/>,
        /// recursively converting the description, configuration, references,
        /// data items, compositions, and child components.
        /// </summary>
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
            if (References != null) component.References = References.ToReferences();

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