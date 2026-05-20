// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a typed container of
    /// <see cref="JsonDataItem"/> items, keyed by the singular cppagent
    /// element name <c>DataItem</c>.
    /// </summary>
    public class JsonDataItems
    {
        /// <summary>
        /// The data items in the container.
        /// </summary>
        [JsonPropertyName("DataItem")]
        public IEnumerable<JsonDataItem> DataItems { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDataItems() { }

        /// <summary>
        /// Initializes the container from a data-item sequence.
        /// </summary>
        public JsonDataItems(IEnumerable<IDataItem> dataItems)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                var jsonDataItems = new List<JsonDataItem>();

                foreach (var dataItem in dataItems) jsonDataItems.Add(new JsonDataItem(dataItem));

                DataItems = jsonDataItems;
            }
        }


        /// <summary>
        /// Flattens the container back into a uniform
        /// <see cref="IDataItem"/> sequence.
        /// </summary>
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