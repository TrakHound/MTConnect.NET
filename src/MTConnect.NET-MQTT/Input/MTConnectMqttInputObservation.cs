using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    public class MTConnectMqttInputObservation
    {
        [JsonPropertyName("dataItemKey")]
        public string DataItemKey { get; set; }

        [JsonPropertyName("values")]
        public Dictionary<string, string> Values { get; set; }


        public MTConnectMqttInputObservation() { }

        public MTConnectMqttInputObservation(string dataItemKey, Dictionary<string, string> values)
        {
            DataItemKey = dataItemKey;
            Values = values;
        }
    }
}
