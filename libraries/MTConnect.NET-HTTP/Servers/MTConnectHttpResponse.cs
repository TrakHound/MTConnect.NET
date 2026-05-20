// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.IO;

namespace MTConnect.Servers.Http
{
    /// <summary>
    /// The HTTP-transport-ready result of formatting one MTConnect response document. The struct
    /// captures the formatted byte stream together with the negotiated content type, the HTTP
    /// status code to return, formatter diagnostics, and timing metrics so the host can both
    /// write the response and report transport-level instrumentation.
    /// </summary>
    public struct MTConnectHttpResponse
    {
        /// <summary>True when the document was formatted without unrecoverable errors; false sets <see cref="StatusCode"/> to 500.</summary>
        public bool Success { get; set; }

        /// <summary>The formatted MTConnect document, ready to be copied to the HTTP response body.</summary>
        public Stream Content { get; set; }

        /// <summary>The MIME content type chosen by the formatter (e.g. <c>application/xml</c>, <c>application/json</c>).</summary>
        public string ContentType { get; set; }

        /// <summary>The HTTP status code that should accompany the response (200 on success, 500 on formatter failure, or the explicit code supplied for error documents).</summary>
        public int StatusCode { get; set; }

        /// <summary>Informational messages produced by the formatter (for example notes about applied defaults).</summary>
        public IEnumerable<string> FormatMessages { get; set; }

        /// <summary>Non-fatal warnings produced by the formatter (e.g. soft schema violations under <c>Warning</c> validation level).</summary>
        public IEnumerable<string> FormatWarnings { get; set; }

        /// <summary>Errors produced by the formatter (populated when <see cref="Success"/> is false).</summary>
        public IEnumerable<string> FormatErrors { get; set; }

        /// <summary>Time in milliseconds the formatter spent serialising the document.</summary>
        public double FormatDuration { get; set; }

        /// <summary>Time in milliseconds the agent spent producing the underlying response document (excluding HTTP write).</summary>
        public double ResponseDuration { get; set; }

        /// <summary>Time in milliseconds spent writing <see cref="Content"/> to the HTTP response body; filled in by the transport after the write completes.</summary>
        public double WriteDuration { get; set; }


        /// <summary>
        /// Formats a <c>probe</c>-style devices response document for the wire using the supplied
        /// document format and format options. On success <see cref="Content"/> holds the encoded
        /// MTConnectDevices document and <see cref="StatusCode"/> is 200; otherwise 500 with
        /// <see cref="FormatErrors"/> populated.
        /// </summary>
        /// <param name="document">The devices response document produced by the agent.</param>
        /// <param name="documentFormat">The document format key (e.g. <c>xml</c>, <c>json</c>) to serialise to.</param>
        /// <param name="responseDuration">Time in milliseconds spent building <paramref name="document"/>.</param>
        /// <param name="formatOptions">Key/value formatter options passed verbatim to the registered formatter.</param>
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

        /// <summary>
        /// Formats a <c>current</c> or <c>sample</c> streams response document for the wire. The
        /// document is taken by <c>ref</c> because the streaming formatter may rent buffers that
        /// it returns to the caller, allowing them to be released after the response is written.
        /// </summary>
        /// <param name="document">The streams response document produced by the agent. Passed by ref to allow buffer reuse.</param>
        /// <param name="documentFormat">The document format key to serialise to.</param>
        /// <param name="responseDuration">Time in milliseconds spent building <paramref name="document"/>.</param>
        /// <param name="formatOptions">Key/value formatter options passed verbatim to the registered formatter.</param>
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

        /// <summary>
        /// Formats an <c>assets</c> or single <c>asset</c> response document for the wire.
        /// </summary>
        /// <param name="document">The assets response document produced by the agent.</param>
        /// <param name="documentFormat">The document format key to serialise to.</param>
        /// <param name="responseDuration">Time in milliseconds spent building <paramref name="document"/>.</param>
        /// <param name="formatOptions">Key/value formatter options passed verbatim to the registered formatter.</param>
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

        /// <summary>
        /// Formats an <c>MTConnectError</c> response document for the wire. The caller supplies
        /// the HTTP status code (e.g. 400, 404) explicitly because MTConnect agents return errors
        /// with a wide range of status codes that the formatter itself does not infer.
        /// </summary>
        /// <param name="document">The error response document produced by the agent.</param>
        /// <param name="statusCode">The HTTP status code to use (typically 400 or 404 for client errors, 500 for server errors).</param>
        /// <param name="documentFormat">The document format key to serialise to.</param>
        /// <param name="responseDuration">Time in milliseconds spent building <paramref name="document"/>.</param>
        /// <param name="formatOptions">Key/value formatter options passed verbatim to the registered formatter.</param>
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
