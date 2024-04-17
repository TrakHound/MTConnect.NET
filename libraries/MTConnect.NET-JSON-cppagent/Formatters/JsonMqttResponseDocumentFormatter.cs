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
    public class JsonMqttResponseDocumentFormatter : JsonHttpResponseDocumentFormatter
    {
        public override string Id => "JSON-cppagent-mqtt";

        public override string ContentType => "application/json";


        public override FormatWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            if (!document.Devices.IsNullOrEmpty())
            {
                var device = document.Devices.FirstOrDefault();
                var outputStream = new MemoryStream();
                JsonSerializer.Serialize(outputStream, new JsonDeviceContainer(device), jsonOptions);
                if (outputStream != null && outputStream.Length > 0)
                {
                    return FormatWriteResult.Successful(outputStream, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

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


        public override FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            try
            {
                // Read Document
                var devices = JsonSerializer.Deserialize<JsonDeviceContainer>(content);
                var success = devices != null;

                var responseDocument = new DevicesResponseDocument();

                var device = devices.ToDevice();
                if (device != null)
                {
                    responseDocument.Devices = new IDevice[] { device };
                }

                return new FormatReadResult<IDevicesResponseDocument>(responseDocument, success);
            }
            catch (Exception ex)
            {
                var messages = ex.Message != null ? new string[] { ex.Message } : null;

                return new FormatReadResult<IDevicesResponseDocument>(null, false, errors: messages);
            }
        }

        public override FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonAssetContainer>(content);
            var success = document != null;

            return new FormatReadResult<IAssetsResponseDocument>(document.ToAssetsDocument(), success);
        }
    }
}