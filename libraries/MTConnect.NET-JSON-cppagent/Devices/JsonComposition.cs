// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect <c>Composition</c> in the
    /// cppagent-compatible shape. Converts to and from the strongly-typed
    /// <see cref="Composition"/> model.
    /// </summary>
    public class JsonComposition
    {
        /// <summary>
        /// The unique <c>id</c> of the composition.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The MTConnect composition <c>type</c>.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The optional human-readable <c>name</c> of the composition.
        /// Omitted from the JSON output when not set.
        /// </summary>
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Name { get; set; }

        /// <summary>
        /// The name the composition is known by in the native data source.
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The interval, in milliseconds, between samples of the composition's
        /// data, when reported.
        /// </summary>
        [JsonPropertyName("sampleInterval")]
        public double? SampleInterval { get; set; }

        /// <summary>
        /// The rate, in samples per second, at which the composition's data is
        /// sampled, when reported.
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        /// <summary>
        /// The universally unique identifier of the composition instance.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of a CoordinateSystem the composition's
        /// values are expressed relative to.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The descriptive metadata for the composition.
        /// </summary>
        [JsonPropertyName("description")]
        public JsonDescription Description { get; set; }

        /// <summary>
        /// The configuration of the composition.
        /// </summary>
        [JsonPropertyName("configuration")]
        public JsonConfiguration Configuration { get; set; }

        /// <summary>
        /// The data items reported directly by the composition.
        /// </summary>
        [JsonPropertyName("dataItems")]
        public IEnumerable<JsonDataItem> DataItems { get; set; }

        /// <summary>
        /// The references from the composition to other components and data
        /// items.
        /// </summary>
        [JsonPropertyName("references")]
        public JsonReferenceContainer References { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonComposition() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IComposition"/>.
        /// </summary>
        public JsonComposition(IComposition composition)
        {
            if (composition != null)
            {
                Id = composition.Id;
                Uuid = composition.Uuid;
                if (!string.IsNullOrEmpty(composition.Name)) Name = composition.Name;
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


        /// <summary>
        /// Returns the JSON representation of this surrogate.
        /// </summary>
        public override string ToString() => JsonFunctions.Convert(this);

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="Composition"/>.
        /// </summary>
        public Composition ToComposition()
        {
            // Route construction through the typed factory so the runtime
            // type discriminator survives the envelope read path. A naked
            // `new Composition()` collapses every typed subclass declared
            // in libraries/MTConnect.NET-Common/Devices/Compositions/*.g.cs
            // back to the abstract base, breaking `composition is
            // ChuckComposition`-style branching downstream. Mirrors the
            // factory pattern JsonDataItem.ToDataItem already uses.
            var composition = Composition.Create(Type);
            if (composition == null) composition = new Composition();

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