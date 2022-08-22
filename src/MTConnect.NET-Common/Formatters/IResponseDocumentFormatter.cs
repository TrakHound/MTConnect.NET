// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using MTConnect.Streams.Output;
using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public interface IResponseDocumentFormatter
    {
        string Id { get; }

        string ContentType { get; }


        FormattedDocumentWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedDocumentWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedDocumentWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedDocumentWriteResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);


        FormattedDocumentReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedDocumentReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedDocumentReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedDocumentReadResult<IErrorResponseDocument> CreateErrorResponseDocument(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);
    }
}
