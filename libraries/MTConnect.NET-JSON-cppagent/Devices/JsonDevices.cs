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

            // Items deserialised from the "Agent" envelope key must round-
            // trip as Agent instances, not as plain Device, so the agent
            // meta-device's type identity survives the read path. Calling
            // ToDevice() here and only re-tagging Type to Agent.TypeId is a
            // half-fix: it makes `device.Type == Agent.TypeId` true but
            // leaves `device is Agent` false because the runtime type is
            // still Device. Downstream code that branches on the runtime
            // type — including JsonDevices.ctor at line 31 above when a
            // document is re-serialised — silently collapses the
            // discriminator. The fix routes through JsonDevice.ToAgent(),
            // which constructs an Agent (Agent : Device) and copies the
            // shared field set via the private CopyFieldsInto helper, so
            // both `device is Agent` and `device.Type == Agent.TypeId` are
            // true. Cf. MTConnect v2.7 DevicesType XSD lines 5029-5051 and
            // the cppagent JSON v2 reference.
            if (!Agents.IsNullOrEmpty())
            {
                foreach (var jsonAgent in Agents) devices.Add(jsonAgent.ToAgent());
            }

            if (!Devices.IsNullOrEmpty())
            {
                foreach (var device in Devices) devices.Add(device.ToDevice());
            }

            return devices;
        }
    }
}