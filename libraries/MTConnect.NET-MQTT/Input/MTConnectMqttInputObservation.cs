using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    /// <summary>
    /// JSON payload for a single MTConnect observation published to or consumed from MQTT. The
    /// <see cref="DataItem"/> property holds the DataItem key (id or name) and <see cref="Values"/>
    /// is the value-key/value-string map that the agent feeds straight into an
    /// <see cref="MTConnect.Input.ObservationInput"/>. The shape is intentionally minimal so the
    /// timestamp can be supplied externally by the containing <see cref="MTConnectMqttInputObservations"/>.
    /// </summary>
    public class MTConnectMqttInputObservation
    {
        /// <summary>The DataItem key (id or name) the observation applies to.</summary>
        [JsonPropertyName("dataItem")]
        public string DataItem { get; set; }

        /// <summary>The value-key/value map (e.g. <c>Result</c>, <c>NativeCode</c>) carried by the observation.</summary>
        [JsonPropertyName("values")]
        public Dictionary<string, string> Values { get; set; }


        /// <summary>Creates an empty container for serialiser-driven deserialisation.</summary>
        public MTConnectMqttInputObservation() { }

        /// <summary>Constructs a populated observation payload with a DataItem key and its value map.</summary>
        /// <param name="dataItemKey">The DataItem key (id or name) the observation applies to.</param>
        /// <param name="values">The value-key/value map carried by the observation.</param>
        public MTConnectMqttInputObservation(string dataItemKey, Dictionary<string, string> values)
        {
            DataItem = dataItemKey;
            Values = values;
        }
    }
}
