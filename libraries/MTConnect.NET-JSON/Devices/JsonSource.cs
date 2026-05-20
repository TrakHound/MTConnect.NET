// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a DataItem <c>Source</c>, identifying
    /// the component, data item, or composition that originates the data item's
    /// observations.
    /// </summary>
    public class JsonSource
    {
        /// <summary>
        /// The <c>id</c> of the originating component.
        /// </summary>
        [JsonPropertyName("componentId")]
        public string ComponentId { get; set; }

        /// <summary>
        /// The <c>id</c> of the originating data item.
        /// </summary>
        [JsonPropertyName("dataItemId")]
        public string DataItemId { get; set; }

        /// <summary>
        /// The <c>id</c> of the originating composition.
        /// </summary>
        [JsonPropertyName("compositionId")]
        public string CompositionId { get; set; }

        /// <summary>
        /// The free-text source description.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSource() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed <see cref="ISource"/>.
        /// </summary>
        public JsonSource(ISource source)
        {
            if (source != null)
            {
                ComponentId = source.ComponentId;
                DataItemId = source.DataItemId;
                CompositionId = source.CompositionId;
                Value = source.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Source"/>.
        /// </summary>
        public ISource ToSource()
        {
            var source = new Source();
            source.ComponentId = ComponentId;
            source.DataItemId = DataItemId;
            source.CompositionId = CompositionId;
            source.Value = Value;
            return source;
        }
    }
}