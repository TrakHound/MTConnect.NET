// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonCompositions
    {
        [JsonPropertyName("Composition")]
        public IEnumerable<JsonComposition> Compositions { get; set; }


        public JsonCompositions() { }

        public JsonCompositions(IEnumerable<IComposition> dataItems)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                var jsonCompositions = new List<JsonComposition>();

                foreach (var dataItem in dataItems) jsonCompositions.Add(new JsonComposition(dataItem));

                Compositions = jsonCompositions;
            }
        }


        public IEnumerable<IComposition> ToCompositions()
        {
            var dataItems = new List<IComposition>();

            // Guard the source collection, not the freshly-allocated empty
            // sink — the original `!dataItems.IsNullOrEmpty()` check tested
            // the empty list and silently dropped every Composition on the
            // envelope read path. Same bug family as the runtime-type loss
            // in ToComposition: payload arrives, nothing is reconstructed.
            if (!Compositions.IsNullOrEmpty())
            {
                foreach (var dataItem in Compositions) dataItems.Add(dataItem.ToComposition());
            }

            return dataItems;
        }
    }
}