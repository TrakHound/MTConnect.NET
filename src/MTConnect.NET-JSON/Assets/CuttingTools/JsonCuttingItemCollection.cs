// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCuttingItemCollection
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }


        [JsonPropertyName("cuttingItem")]
        public IEnumerable<JsonCuttingItem> CuttingItems { get; set; }


        public JsonCuttingItemCollection() { }

        public JsonCuttingItemCollection(CuttingItemCollection cuttingItemCollection)
        {
            if (cuttingItemCollection != null)
            {
                Count = cuttingItemCollection.Count;

                // CuttingItems
                if (!cuttingItemCollection.CuttingItems.IsNullOrEmpty())
                {
                    var cuttingItems = new List<JsonCuttingItem>();
                    foreach (var cuttingItem in cuttingItemCollection.CuttingItems)
                    {
                        cuttingItems.Add(new JsonCuttingItem(cuttingItem));
                    }
                    CuttingItems = cuttingItems;
                }
            }
        }


        public CuttingItemCollection ToCuttingItemCollection()
        {
            var cuttingItemCollection = new CuttingItemCollection();
            cuttingItemCollection.Count = Count;

            // CuttingItems
            if (!CuttingItems.IsNullOrEmpty())
            {
                var cuttingItems = new List<CuttingItem>();
                foreach (var cuttingItem in CuttingItems)
                {
                    cuttingItems.Add(cuttingItem.ToCuttingItem());
                }
                cuttingItemCollection.CuttingItems = cuttingItems;
            }

            return cuttingItemCollection;
        }
    }
}