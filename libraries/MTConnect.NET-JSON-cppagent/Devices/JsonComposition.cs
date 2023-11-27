// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonComposition
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

        [JsonPropertyName("references")]
        public JsonReferenceContainer References { get; set; }


        public JsonComposition() { }

        public JsonComposition(IComposition composition)
        {
            if (composition != null)
            {
                Id = composition.Id;
                Uuid = composition.Uuid;
                Name = composition.Name;
                NativeName = composition.NativeName;
                Type = composition.Type;
                if (composition.Description != null) Description = new JsonDescription(composition.Description);
                if (composition.SampleRate > 0) SampleRate = composition.SampleRate;
                if (composition.SampleInterval > 0) SampleInterval = composition.SampleInterval;

                // References
                if (!composition.References.IsNullOrEmpty()) References = new JsonReferenceContainer(composition.References);

                // Configuration
                if (composition.Configuration != null) Configuration = new JsonConfiguration(composition.Configuration);

                // DataItems
                if (!composition.DataItems.IsNullOrEmpty())
                {
                    var dataItems = new List<JsonDataItem>();
                    foreach (var dataItem in composition.DataItems)
                    {
                        dataItems.Add(new JsonDataItem(dataItem));
                    }
                    DataItems = dataItems;
                }
            }
        }


        public override string ToString() => JsonFunctions.Convert(this);

        public Composition ToComposition()
        {
            var composition = new Composition();

            composition.Id = Id;
            composition.Uuid = Uuid;
            composition.Name = Name;
            composition.NativeName = NativeName;
            composition.Type = Type;
            if (Description != null) composition.Description = Description.ToDescription();
            composition.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            composition.SampleInterval = SampleInterval.HasValue ? SampleInterval.Value : 0;

            // References
            if (References != null) composition.References = References.ToReferences();

            // Configuration
            if (Configuration != null) composition.Configuration = Configuration.ToConfiguration();

            // DataItems
            if (!DataItems.IsNullOrEmpty())
            {
                var dataItems = new List<DataItem>();
                foreach (var dataItem in DataItems)
                {
                    dataItems.Add(dataItem.ToDataItem());
                }
                composition.DataItems = dataItems;
            }

            return composition;
        }
    }
}