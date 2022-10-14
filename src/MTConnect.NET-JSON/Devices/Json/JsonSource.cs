// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.DataItems;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonSource
    {
        [JsonPropertyName("componentId")]
        public string ComponentId { get; set; }

        [JsonPropertyName("dataItemId")]
        public string DataItemId { get; set; }

        [JsonPropertyName("compositionId")]
        public string CompositionId { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonSource() { }

        public JsonSource(ISource source)
        {
            if (source != null)
            {
                ComponentId = source.ComponentId;
                DataItemId = source.DataItemId;
                CompositionId = source.CompositionId;
                Value = source.Value;
            }
        }


        public ISource ToSource()
        {
            var source = new Source();
            source.ComponentId = ComponentId;
            source.DataItemId = DataItemId;
            source.CompositionId = CompositionId;
            source.Value = Value;
            return source;
        }
    }
}
