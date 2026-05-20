// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.Json;
using MTConnect.Devices;
using MTConnect.Devices.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MTConnect.Formatters
{
    /// <summary>
    /// MQTT response document formatter that emits one entity per MQTT
    /// retained payload using the cppagent-compatible JSON shape.
    /// Unlike the HTTP formatter, the Devices payload is reduced to the
    /// first device in the document and the Assets payload is reduced
    /// to a single-asset container so each MQTT topic carries one
    /// entity.
    /// </summary>
    public class JsonMqttResponseDocumentFormatter : JsonHttpResponseDocumentFormatter
    {
        /// <summary>
        /// The formatter identifier exposed to the MQTT publishing layer,
        /// distinguishing it from the HTTP formatter.
        /// </summary>
        public override string Id => "JSON-cppagent-mqtt";

        /// <summary>
        /// The MQTT payload <c>Content-Type</c> emitted by this
        /// formatter.
        /// </summary>
        public override string ContentType => "application/json";


        /// <summary>
        /// Serializes the first device of the Devices response document
        /// as an individual MQTT payload using
        /// <see cref="JsonDeviceContainer"/>; returns an error result
        /// when the document carries no devices.
        /// </summary>
        public override FormatWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            // Serialize through the full JsonDevicesResponseDocument envelope so
            // MTConnectDevices.Devices is emitted as the cppagent JSON v2 keyed
            // object (separate Agent[] and Device[] keys), matching the MTConnect
            // v2.7 XSD DevicesType complex type. Mirrors JsonHttpResponseDocumentFormatter.
            if (!document.Devices.IsNullOrEmpty())
            {
                var outputStream = new MemoryStream();
                JsonSerializer.Serialize(outputStream, new JsonDevicesResponseDocument(document), jsonOptions);
                if (outputStream.Length > 0)
                {
                    return FormatWriteResult.Successful(outputStream, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes the Assets response document as an MQTT payload
        /// using the standard cppagent assets surrogate.
        /// </summary>
        public override FormatWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var outputStream = new MemoryStream();
            JsonSerializer.Serialize(outputStream, new JsonAssetsResponseDocument(document), jsonOptions);
            if (outputStream != null && outputStream.Length > 0)
            {
                return FormatWriteResult.Successful(outputStream, ContentType);
            }

            return FormatWriteResult.Error();
        }


        /// <summary>
        /// Deserializes a single device from an MQTT payload via
        /// <see cref="JsonDeviceContainer"/> and wraps it in a
        /// one-device Devices response document. Any deserialization
        /// exception is captured into the result's errors collection
        /// rather than propagated.
        /// </summary>
        public override FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            try
            {
                // Symmetric counterpart of the write path: deserialise the
                // full JsonDevicesResponseDocument envelope so multi-device
                // MTConnectDevices.Devices.{Agent[],Device[]} payloads round-
                // trip with their device count, identity, and Agent vs Device
                // type tagging intact. Mirrors JsonHttpResponseDocumentFormatter.
                var document = JsonSerializer.Deserialize<JsonDevicesResponseDocument>(content);
                var success = document != null;

                return new FormatReadResult<IDevicesResponseDocument>(document?.ToDocument(), success);
            }
            catch (Exception ex)
            {
                var messages = ex.Message != null ? new string[] { ex.Message } : null;

                return new FormatReadResult<IDevicesResponseDocument>(null, false, errors: messages);
            }
        }

        /// <summary>
        /// Deserializes a single asset from an MQTT payload via
        /// <see cref="JsonAssetContainer"/> and wraps it in a
        /// one-asset Assets response document.
        /// </summary>
        public override FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonAssetContainer>(content);
            var success = document != null;

            return new FormatReadResult<IAssetsResponseDocument>(document.ToAssetsDocument(), success);
        }
    }
}