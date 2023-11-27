// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCuttingItems
    {
        [JsonPropertyName("CuttingItem")]
        public IEnumerable<JsonCuttingItem> CuttingItems { get; set; }


        public JsonCuttingItems() { }

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