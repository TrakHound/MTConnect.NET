// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.Json;
using System.Collections.Generic;
using System.Text.Json;

namespace MTConnect.Formatters
{
    public class JsonMqttResponseDocumentFormatter : JsonHttpResponseDocumentFormatter
    {
        public override string Id => "JSON-cppagent-mqtt";

        public override string ContentType => "application/json";


        public override FormatWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var json = JsonSerializer.SerializeToUtf8Bytes(new JsonAssetsResponseDocument(document), jsonOptions);
            if (!json.IsNullOrEmpty())
            {
                return FormatWriteResult.Successful(json, ContentType);
            }

            return FormatWriteResult.Error();
        }

        public override FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonAssetContainer>(content);
            var success = document != null;

            return new FormatReadResult<IAssetsResponseDocument>(document.ToAssetsDocument(), success);
        }
    }
}