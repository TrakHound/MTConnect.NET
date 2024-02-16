// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.IO;

namespace MTConnect.Formatters
{
    public interface IResponseDocumentFormatter
    {
        string Id { get; }

        string ContentType { get; }


        FormatWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatWriteResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);


        FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatReadResult<IErrorResponseDocument> CreateErrorResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);
    }
}