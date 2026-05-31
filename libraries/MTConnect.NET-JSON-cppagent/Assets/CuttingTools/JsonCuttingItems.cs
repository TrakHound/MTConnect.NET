// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for the typed container of
    /// <see cref="JsonCuttingItem"/> entries inside a
    /// <see cref="JsonCuttingItemCollection"/>, keyed by the singular
    /// cppagent element name <c>CuttingItem</c>.
    /// </summary>
    public class JsonCuttingItems
    {
        /// <summary>
        /// The cutting items in the container.
        /// </summary>
        [JsonPropertyName("CuttingItem")]
        public IEnumerable<JsonCuttingItem> CuttingItems { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingItems() { }

        /// <summary>
        /// Initializes the container from a cutting-item sequence.
        /// </summary>
        public JsonCuttingItems(IEnumerable<ICuttingItem> cuttingItems)
        {
            if (!cuttingItems.IsNullOrEmpty())
            {
                var jsonCuttingItems = new List<JsonCuttingItem>();
                foreach (var cuttingItem in cuttingItems)
                {
                    jsonCuttingItems.Add(new JsonCuttingItem(cuttingItem));
                }
                CuttingItems = jsonCuttingItems;
            }
        }


        /// <summary>
        /// Flattens the container back into a uniform
        /// <see cref="ICuttingItem"/> sequence.
        /// </summary>
        public IEnumerable<ICuttingItem> ToCutterItems()
        {
            if (!CuttingItems.IsNullOrEmpty())
            {
                var cuttingItems = new List<ICuttingItem>();

                foreach (var cuttingItem in CuttingItems)
                {
                    cuttingItems.Add(cuttingItem.ToCuttingItem());
                }

                return cuttingItems;
            }

            return null;
        }
    }
}