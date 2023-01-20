// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDevicesDocument
    {
        [JsonPropertyName("header")]
        public JsonDevicesHeader Header { get; set; }

        [JsonPropertyName("devices")]
        public IEnumerable<JsonDevice> Devices { get; set; }

        //[JsonPropertyName("interfaces")]
        //public IEnumerable<IInterface> Interfaces { get; set; }


        public JsonDevicesDocument() { }

        public JsonDevicesDocument(IDevicesResponseDocument document)
        {
            if (document != null)
            {
                Header = new JsonDevicesHeader(document.Header);

                if (!document.Devices.IsNullOrEmpty())
                {
                    var devices = new List<JsonDevice>();

                    foreach (var device in document.Devices) devices.Add(new JsonDevice(device));

                    Devices = devices;
                }

                //Interfaces = document.Interfaces;
            }
        }


        public DevicesResponseDocument ToDocument()
        {
            var document = new DevicesResponseDocument();

            document.Header = Header.ToDevicesHeader();

            if (!Devices.IsNullOrEmpty())
            {
                var devices = new List<Device>();

                foreach (var device in Devices) devices.Add(device.ToDevice());

                document.Devices = devices;
            }

            //document.Interfaces = Interfaces;

            return document;
        }
    }
}