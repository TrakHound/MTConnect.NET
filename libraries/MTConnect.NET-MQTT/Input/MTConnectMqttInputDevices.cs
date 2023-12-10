using MTConnect.Devices;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    public class MTConnectMqttInputDevices
    {
        [JsonPropertyName("devices")]
        public Dictionary<string, string> Devices { get; set; }


        public MTConnectMqttInputDevices() { }

        public MTConnectMqttInputDevices(IDevice device)
        {
            if (device != null && !string.IsNullOrEmpty(device.Uuid))
            {

            }
        }

        public MTConnectMqttInputDevices(IEnumerable<IDevice> devices)
        {
            if (!devices.IsNullOrEmpty())
            {
                foreach (var device in devices)
                {
                    if (device != null && !string.IsNullOrEmpty(device.Uuid))
                    {

                    }
                }
            }
        }
    }
}
