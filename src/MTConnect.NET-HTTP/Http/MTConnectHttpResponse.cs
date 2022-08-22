// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Streams.Output;
using System.Collections.Generic;

namespace MTConnect.Http
{
    public struct MTConnectHttpResponse
    {
        public bool Success { get; set; }

        public byte[] Content { get; set; }

        public string ContentType { get; set; }

        public int StatusCode { get; set; }

        public IEnumerable<string> FormatMessages { get; set; }

        public IEnumerable<string> FormatWarnings { get; set; }

        public IEnumerable<string> FormatErrors { get; set; }

        public double FormatDuration { get; set; }

        public double ResponseDuration { get; set; }

        public double WriteDuration { get; set; }


        public MTConnectHttpResponse(
            IDevicesResponseDocument document,
            string documentFormat,
            double responseDuration,
            IEnumerable<KeyValuePair<string, string>> formatOptions
            )
        {
            Success = false;
            Content = null;
            ContentType = null;
            StatusCode = 500;
            FormatMessages = null;
            FormatWarnings = null;
            FormatErrors = null;
            FormatDuration = 0;
            ResponseDuration = responseDuration;
            WriteDuration = 0;

            if (document != null)
            {
                var result = ResponseDocumentFormatter.Format(documentFormat, document, formatOptions);
                Content = result.Content;
                ContentType = result.ContentType;
                FormatDuration = result.ResponseDuration;
                Success = result.Success;
                StatusCode = result.Success ? 200 : 500;
                FormatMessages = result.Messages;
                FormatWarnings = result.Warnings;
                FormatErrors = result.Errors;
            }
        }

        public MTConnectHttpResponse(
            ref IStreamsResponseOutputDocument document,
            string documentFormat,
            double responseDuration,
            IEnumerable<KeyValuePair<string, string>> formatOptions
            )
        {
            Success = false;
            Content = null;
            ContentType = null;
            StatusCode = 500;
            FormatMessages = null;
            FormatWarnings = null;
            FormatErrors = null;
            FormatDuration = 0;
            ResponseDuration = responseDuration;
            WriteDuration = 0;

            if (document != null)
            {
                var result = ResponseDocumentFormatter.Format(documentFormat, ref document, formatOptions);
                Content = result.Content;
                ContentType = result.ContentType;
                FormatDuration = result.ResponseDuration;
                Success = result.Success;
                StatusCode = result.Success ? 200 : 500;
                FormatMessages = result.Messages;
                FormatWarnings = result.Warnings;
                FormatErrors = result.Errors;
            }
        }

        public MTConnectHttpResponse(
            IAssetsResponseDocument document,
            string documentFormat,
            double responseDuration,
            IEnumerable<KeyValuePair<string, string>> formatOptions
            )
        {
            Success = false;
            Content = null;
            ContentType = null;
            StatusCode = 500;
            FormatMessages = null;
            FormatWarnings = null;
            FormatErrors = null;
            FormatDuration = 0;
            ResponseDuration = responseDuration;
            WriteDuration = 0;

            if (document != null)
            {
                var result = ResponseDocumentFormatter.Format(documentFormat, document, formatOptions);
                Content = result.Content;
                ContentType = result.ContentType;
                FormatDuration = result.ResponseDuration;
                Success = result.Success;
                StatusCode = result.Success ? 200 : 500;
                FormatMessages = result.Messages;
                FormatWarnings = result.Warnings;
                FormatErrors = result.Errors;
            }
        }

        public MTConnectHttpResponse(
            IErrorResponseDocument document,
            int statusCode,
            string documentFormat,
            double responseDuration,
            IEnumerable<KeyValuePair<string, string>> formatOptions
            )
        {
            Success = false;
            Content = null;
            ContentType = null;
            StatusCode = statusCode;
            FormatMessages = null;
            FormatWarnings = null;
            FormatErrors = null;
            FormatDuration = 0;
            ResponseDuration = responseDuration;
            WriteDuration = 0;

            if (document != null)
            {
                var result = ResponseDocumentFormatter.Format(documentFormat, document, formatOptions);
                Content = result.Content;
                ContentType = result.ContentType;
                FormatDuration = result.ResponseDuration;
                Success = result.Success;
                StatusCode = result.Success ? 200 : 500;
                FormatMessages = result.Messages;
                FormatWarnings = result.Warnings;
                FormatErrors = result.Errors;
            }
        }
    }
}
