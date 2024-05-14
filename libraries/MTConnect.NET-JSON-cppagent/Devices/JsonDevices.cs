// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDevices
    {
        [JsonPropertyName("Agent")]
        public IEnumerable<JsonDevice> Agents { get; set; }

        [JsonPropertyName("Device")]
        public IEnumerable<JsonDevice> Devices { get; set; }


        public JsonDevices() { }

        public JsonDevices(IDevicesResponseDocument document)
        {
            if (document != null)
            {
                if (!document.Devices.IsNullOrEmpty())
                {
                    var agents = new List<JsonDevice>();
                    var devices = new List<JsonDevice>();

                    foreach (var device in document.Devices)
                    {
                        if (device.Type == Agent.TypeId) agents.Add(new JsonDevice(device));
                        else devices.Add(new JsonDevice(device));
                    }

                    Agents = !agents.IsNullOrEmpty() ? agents : null;
                    Devices = !devices.IsNullOrEmpty() ? devices : null;
                }
            }
        }


        public IEnumerable<IDevice> ToDevices()
        {
            var devices = new List<IDevice>();

            if (!Agents.IsNullOrEmpty())
            {
                foreach (var device in Agents) devices.Add(device.ToDevice());
            }

            if (!Devices.IsNullOrEmpty())
            {
                foreach (var device in Devices) devices.Add(device.ToDevice());
            }

            return devices;
        }
    }
}