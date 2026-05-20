// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for the <c>CuttingItems</c> collection of
    /// a cutting tool, including the redundant count attribute so consumers
    /// can validate the cardinality without enumerating the collection.
    /// </summary>
    public class JsonCuttingItemCollection
    {
        /// <summary>
        /// The number of cutting items in <see cref="CuttingItems"/>.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }


        /// <summary>
        /// The cutting items of the tool.
        /// </summary>
        [JsonPropertyName("cuttingItems")]
        public IEnumerable<JsonCuttingItem> CuttingItems { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingItemCollection() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed cutting item
        /// collection, populating both <see cref="Count"/> and
        /// <see cref="CuttingItems"/>.
        /// </summary>
        public JsonCuttingItemCollection(IEnumerable<ICuttingItem> cuttingItems)
        {
            if (!cuttingItems.IsNullOrEmpty())
            {
                Count = cuttingItems.Count();

                // CuttingItems
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
        }


        /// <summary>
        /// Converts the surrogate cutting items back to their strongly-typed
        /// form, or null when the collection is empty.
        /// </summary>
        public IEnumerable<ICuttingItem> ToCuttingItems()
        {
            // CuttingItems
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