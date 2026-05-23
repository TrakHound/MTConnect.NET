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

            // Items deserialised from the "Agent" envelope key must be
            // re-tagged with Agent.TypeId after reconstruction; JsonDevice
            // is type-agnostic and ToDevice() returns a generic Device
            // (Type = Device.TypeId from the Device ctor). Without this
            // re-tag, the Agent vs Device distinction is silently erased
            // on the read path even though the wire envelope keys are
            // symmetric (cf. MTConnect v2.7 DevicesType XSD lines 5029-5051
            // and cppagent JSON v2 reference).
            if (!Agents.IsNullOrEmpty())
            {
                foreach (var jsonAgent in Agents)
                {
                    var agent = jsonAgent.ToDevice();
                    if (agent != null) agent.Type = Agent.TypeId;
                    devices.Add(agent);
                }
            }

            if (!Devices.IsNullOrEmpty())
            {
                foreach (var device in Devices) devices.Add(device.ToDevice());
            }

            return devices;
        }
    }
}