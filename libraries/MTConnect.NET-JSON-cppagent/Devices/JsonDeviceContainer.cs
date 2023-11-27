// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDeviceContainer
    {
        [JsonPropertyName("Agent")]
        public JsonDevice Agent { get; set; }

        [JsonPropertyName("Device")]
        public JsonDevice Device { get; set; }


        public JsonDeviceContainer() { }

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