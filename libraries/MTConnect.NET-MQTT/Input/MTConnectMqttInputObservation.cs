using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    public class MTConnectMqttInputObservation
    {
        [JsonPropertyName("dataItem")]
        public string DataItem { get; set; }

        [JsonPropertyName("values")]
        public Dictionary<string, string> Values { get; set; }


        public MTConnectMqttInputObservation() { }

        public MTConnectMqttInputObservation(string dataItemKey, Dictionary<string, string> values)
        {
            DataItem = dataItemKey;
            Values = values;
        }
    }
}
