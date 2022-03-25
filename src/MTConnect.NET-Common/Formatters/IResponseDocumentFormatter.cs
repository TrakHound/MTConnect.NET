// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public interface IResponseDocumentFormatter
    {
        string Id { get; }

        string ContentType { get; }


        FormattedDocumentResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedDocumentResult Format(IStreamsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedDocumentResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedDocumentResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);


        IDevicesResponseDocument CreateDevicesResponseDocument(string content);

        IStreamsResponseDocument CreateStreamsResponseDocument(string content);

        IAssetsResponseDocument CreateAssetsResponseDocument(string content);

        IErrorResponseDocument CreateErrorResponseDocument(string content);
    }
}
