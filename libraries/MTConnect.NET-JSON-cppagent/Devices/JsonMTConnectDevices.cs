// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonMTConnectDevices
    {
        [JsonPropertyName("jsonVersion")]
        public int JsonVersion { get; set; }

        /// <summary>
        /// Top-level <c>schemaVersion</c> identifies the envelope schema
        /// this DOCUMENT conforms to — the wire format the producer chose
        /// to emit. It is distinct from <c>Header.schemaVersion</c>, which
        /// identifies the AGENT's configured MTConnect Standard release
        /// (what the data inside refers to). The two fields are populated
        /// from independent sources and are not interchangeable.
        /// </summary>
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
        }

        public JsonMTConnectDevices(IDevicesResponseDocument document)
        {
            JsonVersion = 2;

            if (document != null)
            {
                SchemaVersion = document.Version?.ToString();

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