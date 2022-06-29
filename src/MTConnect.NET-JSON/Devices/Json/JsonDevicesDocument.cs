// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using MTConnect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDevicesDocument
    {
        [JsonPropertyName("header")]
        public MTConnectDevicesHeader Header { get; set; }

        [JsonPropertyName("devices")]
        public List<JsonDevice> Devices { get; set; }

        [JsonPropertyName("interfaces")]
        public IEnumerable<IInterface> Interfaces { get; set; }


        public JsonDevicesDocument() { }

        public JsonDevicesDocument(IDevicesResponseDocument document)
        {
            if (document != null)
            {
                var header = new MTConnectDevicesHeader();
                header.InstanceId = document.Header.InstanceId;
                header.Version = document.Header.Version;
                header.Sender = document.Header.Sender;
                header.BufferSize = document.Header.BufferSize;
                header.AssetBufferSize = document.Header.AssetBufferSize;
                header.AssetCount = document.Header.AssetCount;
                header.DeviceModelChangeTime = document.Header.DeviceModelChangeTime;
                header.TestIndicator = document.Header.TestIndicator;
                header.CreationTime = document.Header.CreationTime;
                Header = header;

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
