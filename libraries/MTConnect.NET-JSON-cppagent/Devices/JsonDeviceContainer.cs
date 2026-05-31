// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate that wraps a single device of
    /// either kind (Agent or Device) in the cppagent-compatible shape.
    /// Mirrors <see cref="JsonDevices"/> but for the entity-level MQTT
    /// payload where each retained topic carries exactly one device.
    /// </summary>
    public class JsonDeviceContainer
    {
        /// <summary>
        /// The wrapped agent-self device, when the container holds
        /// one.
        /// </summary>
        [JsonPropertyName("Agent")]
        public JsonDevice Agent { get; set; }

        /// <summary>
        /// The wrapped real-equipment device, when the container holds
        /// one.
        /// </summary>
        [JsonPropertyName("Device")]
        public JsonDevice Device { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDeviceContainer() { }

        /// <summary>
        /// Initializes the container from a single
        /// <see cref="IDevice"/>, dispatching to the appropriate typed
        /// property by the device's <c>TypeId</c>.
        /// </summary>
        public JsonDeviceContainer(IDevice device)
        {
            if (device != null)
            {
                switch (device.Type)
                {
                    case Devices.Agent.TypeId:
                        Agent = new JsonDevice(device);
                        break;

                    default:
                        Device = new JsonDevice(device);
                        break;
                }
            }
        }


        /// <summary>
        /// Returns the contained device by inspecting the populated
        /// property, returning <c>null</c> when neither is populated.
        /// </summary>
        public IDevice ToDevice()
        {
            if (Agent != null)
            {
                return Agent.ToDevice();
            }

            if (Device != null)
            {
                return Device.ToDevice();
            }

            return null;
        }
    }
}