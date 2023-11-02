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

        [JsonPropertyName("list")]
        public JsonCuttingItems CuttingItems { get; set; }


        public JsonCuttingItemCollection() { }

        public JsonCuttingItemCollection(IEnumerable<ICuttingItem> cuttingItems)
        {
            if (!cuttingItems.IsNullOrEmpty())
            {
                Count = cuttingItems.Count();

                CuttingItems = new JsonCuttingItems(cuttingItems);
            }
        }


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