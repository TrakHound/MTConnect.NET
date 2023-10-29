// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDevicesResponseDocument
    {
        [JsonPropertyName("MTConnectDevices")]
        public JsonMTConnectDevices MTConnectDevices { get; set; }


        public JsonDevicesResponseDocument() { }

        public JsonDevicesResponseDocument(IDevicesResponseDocument document)
        {
            if (document != null)
            {
                MTConnectDevices = new JsonMTConnectDevices(document);
            }
        }


        public IDevicesResponseDocument ToDocument()
        {
            if (MTConnectDevices != null)
            {
                return MTConnectDevices.ToDocument();
            }

            return null;
        }
    }
}