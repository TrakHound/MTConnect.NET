// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// Outer JSON envelope for a Probe/Devices response document in
    /// the cppagent-compatible shape, with the actual content wrapped
    /// in a single <c>MTConnectDevices</c> property to mirror the XML
    /// root element name.
    /// </summary>
    public class JsonDevicesResponseDocument
    {
        /// <summary>
        /// The wrapped devices document.
        /// </summary>
        [JsonPropertyName("MTConnectDevices")]
        public JsonMTConnectDevices MTConnectDevices { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDevicesResponseDocument() { }

        /// <summary>
        /// Initializes the envelope from a strongly-typed devices
        /// document.
        /// </summary>
        public JsonDevicesResponseDocument(IDevicesResponseDocument document)
        {
            if (document != null)
            {
                MTConnectDevices = new JsonMTConnectDevices(document);
            }
        }


        /// <summary>
        /// Unwraps the envelope and converts it to a strongly-typed
        /// devices document, returning <c>null</c> when the envelope is
        /// empty.
        /// </summary>
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