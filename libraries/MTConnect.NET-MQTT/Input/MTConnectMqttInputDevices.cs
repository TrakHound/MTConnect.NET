using MTConnect.Devices;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    /// <summary>
    /// Payload published over MQTT to announce a set of devices to a downstream MTConnect agent.
    /// The map keys are device UUIDs and the values are the serialised device documents in the
    /// chosen MTConnect format; consumers replay this against their device cache so an agent can
    /// reconstruct the upstream device model without polling.
    /// </summary>
    public class MTConnectMqttInputDevices
    {
        /// <summary>Map of device UUID to the serialised MTConnect device document for that UUID.</summary>
        [JsonPropertyName("devices")]
        public Dictionary<string, string> Devices { get; set; }


        /// <summary>Creates an empty container for serialiser-driven deserialisation.</summary>
        public MTConnectMqttInputDevices() { }

        /// <summary>Reserved single-device constructor; currently kept for API parity with the multi-device overload pending implementation in upstream releases.</summary>
        /// <param name="device">The device whose UUID and document would be encoded.</param>
        public MTConnectMqttInputDevices(IDevice device)
        {
            if (device != null && !string.IsNullOrEmpty(device.Uuid))
            {

            }
        }

        /// <summary>Reserved many-device constructor; currently kept for API parity with the single-device overload pending implementation in upstream releases.</summary>
        /// <param name="devices">The devices whose UUIDs and documents would be encoded.</param>
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
