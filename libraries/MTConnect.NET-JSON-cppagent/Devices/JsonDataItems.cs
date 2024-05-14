// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDataItems
    {
        [JsonPropertyName("DataItem")]
        public IEnumerable<JsonDataItem> DataItems { get; set; }


        public JsonDataItems() { }

        public JsonDataItems(IEnumerable<IDataItem> dataItems)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                var jsonDataItems = new List<JsonDataItem>();

                foreach (var dataItem in dataItems) jsonDataItems.Add(new JsonDataItem(dataItem));

                DataItems = jsonDataItems;
            }
        }


        public IEnumerable<IDataItem> ToDataItems()
        {
            var dataItems = new List<IDataItem>();

            if (!DataItems.IsNullOrEmpty())
            {
                foreach (var dataItem in DataItems) dataItems.Add(dataItem.ToDataItem());
            }

            return dataItems;
        }
    }
}