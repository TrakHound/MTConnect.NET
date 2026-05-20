// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for the top-level
    /// <c>MTConnectDevices</c> document in the cppagent-compatible
    /// shape. Sits inside a <see cref="JsonDevicesResponseDocument"/>
    /// envelope and carries the wire-format version, the agent header,
    /// and the document's devices. Converts to and from the
    /// strongly-typed <see cref="DevicesResponseDocument"/> model.
    /// </summary>
    public class JsonMTConnectDevices
    {
        /// <summary>
        /// The wire-format version of the cppagent JSON envelope
        /// emitted by this producer.
        /// </summary>
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

        /// <summary>
        /// The MTConnect Agent header (instance id, creation time,
        /// version, etc.).
        /// </summary>
        [JsonPropertyName("Header")]
        public JsonDevicesHeader Header { get; set; }

        /// <summary>
        /// The keyed device container holding every device in the
        /// document.
        /// </summary>
        [JsonPropertyName("Devices")]
        public JsonDevices Devices { get; set; }

        //[JsonPropertyName("interfaces")]
        //public IEnumerable<IInterface> Interfaces { get; set; }


        /// <summary>
        /// Initializes a fresh container, defaulting
        /// <see cref="JsonVersion"/> to the current emitter version.
        /// </summary>
        public JsonMTConnectDevices()
        {
            JsonVersion = 2;
        }

        /// <summary>
        /// Initializes the container from a strongly-typed
        /// <see cref="IDevicesResponseDocument"/>, capturing the agent
        /// schema version (note that this is the agent's standard
        /// release, distinct from <see cref="JsonVersion"/> above).
        /// </summary>
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


        /// <summary>
        /// Converts the container to a strongly-typed
        /// <see cref="DevicesResponseDocument"/>.
        /// </summary>
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