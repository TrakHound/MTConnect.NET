// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using MTConnect.Interfaces;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDevicesDocument
    {
        [JsonPropertyName("header")]
        public IMTConnectDevicesHeader Header { get; set; }

        [JsonPropertyName("devices")]
        public List<JsonDevice> Devices { get; set; }

        [JsonPropertyName("interfaces")]
        public IEnumerable<IInterface> Interfaces { get; set; }


        public JsonDevicesDocument() { }

        public JsonDevicesDocument(IDevicesResponseDocument document)
        {
            if (document != null)
            {
                Header = document.Header;

                if (!document.Devices.IsNullOrEmpty())
                {
                    var devices = new List<JsonDevice>();

                    foreach (var device in document.Devices) devices.Add(new JsonDevice(device));

                    Devices = devices;
                }

                Interfaces = document.Interfaces;
            }
        }


        public DevicesResponseDocument ToDocument()
        {
            var document = new DevicesResponseDocument();

            document.Header = Header;

            if (!Devices.IsNullOrEmpty())
            {
                var devices = new List<Device>();

                foreach (var device in Devices) devices.Add(device.ToDevice());

                document.Devices = devices;
            }

            document.Interfaces = Interfaces;

            return document;
        }
    }
}
