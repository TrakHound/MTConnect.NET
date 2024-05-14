// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCuttingItemCollection
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }


        [JsonPropertyName("cuttingItems")]
        public IEnumerable<JsonCuttingItem> CuttingItems { get; set; }


        public JsonCuttingItemCollection() { }

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