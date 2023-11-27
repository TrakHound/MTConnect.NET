using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    public class MTConnectMqttInputAsset
    {
        [JsonPropertyName("dataItemKey")]
        public string DataItemKey { get; set; }

        [JsonPropertyName("values")]
        public Dictionary<string, string> Values { get; set; }


        public MTConnectMqttInputAsset() { }

        public MTConnectMqttInputAsset(string dataItemKey, Dictionary<string, string> values)
        {
            DataItemKey = dataItemKey;
            Values = values;
        }
    }
}
