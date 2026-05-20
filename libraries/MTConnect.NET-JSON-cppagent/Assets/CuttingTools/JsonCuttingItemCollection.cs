// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for the counted cutting-item
    /// collection of a CuttingTool, in the cppagent shape that pairs
    /// the inline item count with the typed item list under <c>list</c>.
    /// </summary>
    public class JsonCuttingItemCollection
    {
        /// <summary>
        /// The number of cutting items in the collection, captured at
        /// construction for round-trip and inspection convenience.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// The cutting items themselves under the cppagent <c>list</c>
        /// key.
        /// </summary>
        [JsonPropertyName("list")]
        public JsonCuttingItems CuttingItems { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingItemCollection() { }

        /// <summary>
        /// Initializes the collection from a cutting-item sequence,
        /// caching the count and projecting each item into the typed
        /// <see cref="JsonCuttingItems"/> container.
        /// </summary>
        public JsonCuttingItemCollection(IEnumerable<ICuttingItem> cuttingItems)
        {
            if (!cuttingItems.IsNullOrEmpty())
            {
                Count = cuttingItems.Count();

                CuttingItems = new JsonCuttingItems(cuttingItems);
            }
        }


        /// <summary>
        /// Flattens the typed cutting-item container back into a
        /// uniform <see cref="ICuttingItem"/> sequence.
        /// </summary>
        public IEnumerable<ICuttingItem> ToCuttingItems()
        {
            if (CuttingItems != null)
            {
                var cuttingItems = new List<ICuttingItem>();

                foreach (var cuttingItem in CuttingItems.CuttingItems)
                {
                    cuttingItems.Add(cuttingItem.ToCuttingItem());
                }

                return cuttingItems;
            }

            return null;
        }
    }
}