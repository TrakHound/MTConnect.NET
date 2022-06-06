// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using MTConnect.Formatters;
using System.Collections.Generic;

namespace MTConnect.Http
{
    public struct MTConnectHttpResponse
    {
        public bool Success { get; set; }

        public string Content { get; set; }

        public string ContentType { get; set; }

        public int StatusCode { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public long FormatDuration { get; set; }

        public long ResponseDuration { get; set; }


        public MTConnectHttpResponse(IDevicesResponseDocument document, string documentFormat, long responseDuration, IEnumerable<KeyValuePair<string, string>> formatOptions)
        {
            Success = false;
            Content = null;
            ContentType = null;
            StatusCode = 500;
            Errors = null;
            FormatDuration = 0;
            ResponseDuration = responseDuration;

            if (document != null)
            {
                var result = ResponseDocumentFormatter.Format(documentFormat, document, formatOptions);
                Content = result.Content;
                ContentType = result.ContentType;
                FormatDuration = result.ResponseDuration;
                Success = result.Success;
                StatusCode = result.Success ? 200 : 500;
                Errors = result.Errors;
            }
        }

        public MTConnectHttpResponse(IStreamsResponseDocument document, string documentFormat, long responseDuration, IEnumerable<KeyValuePair<string, string>> formatOptions)
        {
            Success = false;
            Content = null;
            ContentType = null;
            StatusCode = 500;
            Errors = null;
            FormatDuration = 0;
            ResponseDuration = responseDuration;

            if (document != null)
            {
                var result = ResponseDocumentFormatter.Format(documentFormat, document, formatOptions);
                Content = result.Content;
                ContentType = result.ContentType;
                FormatDuration = result.ResponseDuration;
                Success = result.Success;
                StatusCode = result.Success ? 200 : 500;
            }
        }

        public MTConnectHttpResponse(IAssetsResponseDocument document, string documentFormat, long responseDuration, IEnumerable<KeyValuePair<string, string>> formatOptions)
        {
            Success = false;
            Content = null;
            ContentType = null;
            StatusCode = 500;
            Errors = null;
            FormatDuration = 0;
            ResponseDuration = responseDuration;

            if (document != null)
            {
                var result = ResponseDocumentFormatter.Format(documentFormat, document, formatOptions);
                Content = result.Content;
                ContentType = result.ContentType;
                FormatDuration = result.ResponseDuration;
                Success = result.Success;
                StatusCode = result.Success ? 200 : 500;
            }
        }

        public MTConnectHttpResponse(IErrorResponseDocument document, int statusCode, string documentFormat, long responseDuration, IEnumerable<KeyValuePair<string, string>> formatOptions)
        {
            Success = false;
            Content = null;
            ContentType = null;
            StatusCode = statusCode;
            Errors = null;
            FormatDuration = 0;
            ResponseDuration = responseDuration;

            if (document != null)
            {
                var result = ResponseDocumentFormatter.Format(documentFormat, document, formatOptions);
                Content = result.Content;
                ContentType = result.ContentType;
                FormatDuration = result.ResponseDuration;
                Success = result.Success;
                StatusCode = result.Success ? 200 : 500;
            }
        }
    }
}
