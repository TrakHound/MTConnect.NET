// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonMTConnectDevices
    {
        [JsonPropertyName("jsonVersion")]
        public int JsonVersion { get; set; }

        [JsonPropertyName("schemaVersion")]
        public string SchemaVersion { get; set; }

        [JsonPropertyName("Header")]
        public JsonDevicesHeader Header { get; set; }

        [JsonPropertyName("Devices")]
        public JsonDevices Devices { get; set; }

        //[JsonPropertyName("interfaces")]
        //public IEnumerable<IInterface> Interfaces { get; set; }


        public JsonMTConnectDevices()
        {
            JsonVersion = 2;
            SchemaVersion = "2.0";
        }

        public JsonMTConnectDevices(IDevicesResponseDocument document)
        {
            JsonVersion = 2;
            SchemaVersion = "2.0";

            if (document != null)
            {
                Header = new JsonDevicesHeader(document.Header);

                Devices = new JsonDevices(document);

                //Interfaces = document.Interfaces;
            }
        }


        public DevicesResponseDocument ToDocument()
        {
            var document = new DevicesResponseDocument();

            document.Header = Header.ToDevicesHeader();

            document.Devices = Devices.ToDevices();

            //document.Interfaces = Interfaces;

            return document;
        }
    }
}