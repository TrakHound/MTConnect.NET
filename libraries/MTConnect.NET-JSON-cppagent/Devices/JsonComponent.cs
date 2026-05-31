// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect <c>Component</c> in the
    /// cppagent-compatible shape. The component's <c>type</c> is implicit in
    /// the parent's container property name, so this surrogate omits it and
    /// receives it back when reconstructed via
    /// <see cref="ToComponent(string)"/>.
    /// </summary>
    public class JsonComponent
    {
        /// <summary>
        /// The unique <c>id</c> of the component.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The optional human-readable <c>name</c> of the component. Omitted
        /// from the JSON output when not set.
        /// </summary>
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Name { get; set; }

        /// <summary>
        /// The name the component is known by in the native data source.
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
        /// The descriptive metadata for the component.
        /// </summary>
        [JsonPropertyName("Description")]
        public JsonDescription Description { get; set; }

        /// <summary>
        /// The configuration of the component.
        /// </summary>
        [JsonPropertyName("Configuration")]
        public JsonConfiguration Configuration { get; set; }

        /// <summary>
        /// The data items reported directly by the component, wrapped in a
        /// counted container.
        /// </summary>
        [JsonPropertyName("DataItems")]
        public JsonDataItems DataItems { get; set; }

        /// <summary>
        /// The compositions of the component, wrapped in a counted container.
        /// </summary>
        [JsonPropertyName("Compositions")]
        public JsonCompositions Compositions { get; set; }

        /// <summary>
        /// The child components, wrapped in a counted container.
        /// </summary>
        [JsonPropertyName("Components")]
        public JsonComponents Components { get; set; }

        /// <summary>
        /// The references from the component to other components and data
        /// items.
        /// </summary>
        [JsonPropertyName("References")]
        public JsonReferenceContainer References { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonComponent() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IComponent"/>, wrapping the child collections in their
        /// counted container types.
        /// </summary>
        public JsonComponent(IComponent component)
        {
            if (component != null)
            {
                Id = component.Id;
                Uuid = component.Uuid;
                if (!string.IsNullOrEmpty(component.Name)) Name = component.Name;
                NativeName = component.NativeName;
                //Type = component.Type;
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
                    DataItems = new JsonDataItems(component.DataItems);
                }

                // Compositions
                if (!component.Compositions.IsNullOrEmpty())
                {
                    Compositions = new JsonCompositions(component.Compositions);
                }

                // Components
                if (!component.Components.IsNullOrEmpty())
                {
                    Components = new JsonComponents(component.Components);
                }
            }
        }


        /// <summary>
        /// Returns the JSON representation of this surrogate.
        /// </summary>
        public override string ToString() => JsonFunctions.Convert(this);

        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Component"/>,
        /// applying <paramref name="componentType"/> as the component's
        /// MTConnect type (which is implicit in the wire format) and
        /// unwrapping each child container.
        /// </summary>
        public Component ToComponent(string componentType)
        {
            // Route construction through the typed factory so the runtime
            // type discriminator survives the envelope read path. A naked
            // `new Component()` collapses every typed subclass declared in
            // libraries/MTConnect.NET-Common/Devices/Components/*.g.cs back
            // to the abstract base, breaking `component is AxesComponent`
            // and the cppagent JSON v2 keyed-by-type re-serialisation that
            // JsonComponents drives off the runtime type. Mirrors the
            // factory pattern JsonDataItem.ToDataItem already uses.
            var component = Component.Create(componentType);
            if (component == null) component = new Component();

            component.Id = Id;
            component.Uuid = Uuid;
            component.Name = Name;
            component.NativeName = NativeName;
            component.Type = componentType;
            if (Description != null) component.Description = Description.ToDescription();
            component.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            component.SampleInterval = SampleInterval.HasValue ? SampleInterval.Value : 0;

            // References
            if (References != null) component.References = References.ToReferences();

            // Configuration
            if (Configuration != null) component.Configuration = Configuration.ToConfiguration();


            // DataItems
            if (DataItems != null) component.DataItems = DataItems.ToDataItems();

            // Compositions
            if (Compositions != null) component.Compositions = Compositions.ToCompositions();

            // Components
            if (Components != null) component.Components = Components.ToComponents();

            return component;
        }
    }
}