// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnectDevices response
    /// document, carrying the header and the device collection. Converts to
    /// and from the strongly-typed <see cref="DevicesResponseDocument"/> model.
    /// </summary>
    public class JsonDevicesDocument
    {
        /// <summary>
        /// The document header.
        /// </summary>
        [JsonPropertyName("header")]
        public JsonDevicesHeader Header { get; set; }

        /// <summary>
        /// The devices reported in the document.
        /// </summary>
        [JsonPropertyName("devices")]
        public IEnumerable<JsonDevice> Devices { get; set; }

        //[JsonPropertyName("interfaces")]
        //public IEnumerable<IInterface> Interfaces { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDevicesDocument() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDevicesResponseDocument"/>, converting the header and
        /// each device.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="DevicesResponseDocument"/>, converting the header and
        /// each device.
        /// </summary>
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