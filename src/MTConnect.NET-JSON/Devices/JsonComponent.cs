// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonComponent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        [JsonPropertyName("sampleInterval")]
        public double? SampleInterval { get; set; }

        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        [JsonPropertyName("description")]
        public JsonDescription Description { get; set; }

        [JsonPropertyName("configuration")]
        public JsonConfiguration Configuration { get; set; }

        [JsonPropertyName("dataItems")]
        public IEnumerable<JsonDataItem> DataItems { get; set; }

        [JsonPropertyName("components")]
        public IEnumerable<JsonComponent> Components { get; set; }

        [JsonPropertyName("compositions")]
        public IEnumerable<JsonComposition> Compositions { get; set; }

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