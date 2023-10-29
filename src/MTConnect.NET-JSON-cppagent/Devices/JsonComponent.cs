// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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

        [JsonPropertyName("Description")]
        public JsonDescription Description { get; set; }

        [JsonPropertyName("Configuration")]
        public JsonConfiguration Configuration { get; set; }

        [JsonPropertyName("DataItems")]
        public JsonDataItems DataItems { get; set; }

        [JsonPropertyName("Compositions")]
        public JsonCompositions Compositions { get; set; }

        [JsonPropertyName("Components")]
        public JsonComponents Components { get; set; }

        [JsonPropertyName("References")]
        public JsonReferenceContainer References { get; set; }


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