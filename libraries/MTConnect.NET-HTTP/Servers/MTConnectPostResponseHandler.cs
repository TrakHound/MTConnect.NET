// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Errors;
using MTConnect.Servers.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Servers
{
    /// <summary>
    /// Ceen request handler for asset ingestion over HTTP POST (POST /
    /// and POST /asset/{assetId}). Accepts an asset document in the
    /// request body, parses it according to the negotiated
    /// documentFormat, and forwards the result to the configured
    /// ProcessFunction for storage in the agent's asset buffer. The
    /// response carries an HTTP status (200 on accept, 400 on malformed
    /// payload, 500 on storage failure) and an optional MTConnectError
    /// body when negotiated.
    /// </summary>
    class MTConnectPostResponseHandler : MTConnectHttpResponseHandler
    {
        private const int MaxRequestBodySize = 1048576 * 2; // 2 MB

        public Func<MTConnectAssetInputArgs, bool> ProcessFunction { get; set; }


        public MTConnectPostResponseHandler(IHttpServerConfiguration serverConfiguration, IMTConnectAgentBroker mtconnectAgent) : base(serverConfiguration, mtconnectAgent) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
        {
            var response = new MTConnectHttpResponse();

            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.Path != null && httpResponse != null)
            {
                // POST writes to the Agent just like PUT, so it is governed by the same AllowPut / AllowPutFrom settings
                if (!_serverConfiguration.AllowPut || (!_serverConfiguration.AllowPutFrom.IsNullOrEmpty() && !_serverConfiguration.AllowPutFrom.Contains(httpRequest.GetRemoteIP())))
                {
                    response.StatusCode = 403;
                    return response;
                }

                // Reject bodies larger than the read buffer instead of silently truncating them
                if (httpRequest.Headers.TryGetValue("Content-Length", out var contentLengthHeader) &&
                    long.TryParse(contentLengthHeader, out var contentLength) && contentLength > MaxRequestBodySize)
                {
                    response.StatusCode = 413;
                    return response;
                }

                var requestBytes = await ReadRequestBytes(context.Request.Body, cancellationToken);

                // Without a Content-Length (e.g. chunked), a full buffer may mean more data follows
                if (requestBytes != null && requestBytes.Length >= MaxRequestBodySize && await HasMoreData(context.Request.Body, cancellationToken))
                {
                    response.StatusCode = 413;
                    return response;
                }

                if (!requestBytes.IsNullOrEmpty())
                {
                    var urlSegments = GetUriSegments(httpRequest.Path);

                    // Read AssetId from URL Path
                    var assetId = httpRequest.Path?.Trim('/');
                    if (urlSegments.Length > 1) assetId = urlSegments[urlSegments.Length - 1];

                    if (!string.IsNullOrEmpty(assetId))
                    {
                        // Get Device Key (UUID or Name)
                        var deviceKey = httpRequest.QueryString["device"];

                        // Get the Asset Type
                        var assetType = httpRequest.QueryString["type"];

                        // Set Document Format
                        var documentFormat = DocumentFormat.XML;
                        if (httpRequest.ContentType == "application/json") documentFormat = DocumentFormat.JSON;

                        // Call the OnAssetInput method that is intended to be overridden by a derived class
                        //var success = OnAssetInput(assetId, deviceKey, assetType, requestBytes, documentFormat);
                        bool success = false;

                        if (ProcessFunction != null)
                        {
                            var args = new MTConnectAssetInputArgs();
                            args.AssetId = assetId;
                            args.AssetType = assetType;
                            args.DeviceKey = deviceKey;
                            args.DocumentFormat = documentFormat;
                            args.RequestBody = requestBytes;

                            success = ProcessFunction(args);
                        }

                        if (success)
                        {
                            // Write the "<success/>" respone to the Http Response Stream
                            // along with a 200 Status Code
                            await WriteResponse("<success/>", httpResponse, HttpStatusCode.OK);
                        }
                        else
                        {
                            // Return MTConnectError Response Document along with a 404 Http Status Code
                            var errorDocument = _mtconnectAgent.GetErrorResponseDocument(ErrorCode.UNSUPPORTED, $"Cannot find device: {deviceKey}");
                            var mtconnectResponse = new MTConnectHttpResponse(errorDocument, 404, DocumentFormat.XML, 0, null);
                            await WriteResponse(mtconnectResponse, httpResponse);
                        }
                    }
                }
            }

            return response;
        }

        private static async Task<bool> HasMoreData(Stream inputStream, CancellationToken cancellationToken)
        {
            if (inputStream == null) return false;

            try
            {
                var probe = new byte[1];
                return await inputStream.ReadAsync(probe, 0, 1, cancellationToken) > 0;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<byte[]> ReadRequestBytes(Stream inputStream, CancellationToken cancellationToken)
        {
            if (inputStream != null)
            {
                try
                {
                    var bufferSize = MaxRequestBodySize;
                    var bytes = new byte[bufferSize];

                    // CA2022 short-read accumulator — TFM-uniform. Every supported
                    // TFM lands on the same shape: loop ReadAsync until EOF or the
                    // buffer is full, then truncate to the actually-filled length.
                    // The underlying stream (Ceen.Httpd.LimitedBodyStream or the
                    // hosting server's request body) already respects Content-Length
                    // framing at a lower layer; the accumulator only needs to
                    // survive multi-segment TCP arrivals. Matches the boost::beast
                    // HTTP parser cppagent uses (src/mtconnect/sink/rest_sink/
                    // session_impl.cpp:176-181), which likewise returns the exact
                    // body length rather than a zero-padded buffer needing TrimEnd.
                    //
                    // Do NOT re-introduce a ReadExactlyAsync-into-fixed-buffer +
                    // TrimEnd shape: ReadExactlyAsync on a body smaller than the
                    // buffer throws EndOfStreamException (silently swallowed by the
                    // outer catch, dropping the request), and TrimEnd would corrupt
                    // any payload whose final legitimate byte is 0x00.
                    //
                    // Cancellation is threaded to every ReadAsync so a client
                    // abort (Ceen surfaces it as the OnRequestReceived
                    // cancellationToken parameter) short-circuits the accumulator
                    // rather than draining the full 2 MB. Matches the sibling
                    // LimitedBodyStream.DiscardAllAsync which likewise takes a
                    // CancellationToken and forwards it to its ReadAsync loop.
                    var totalRead = 0;
                    while (totalRead < bytes.Length)
                    {
                        var read = await inputStream.ReadAsync(bytes, totalRead, bytes.Length - totalRead, cancellationToken);
                        if (read == 0) break;
                        totalRead += read;
                    }
                    if (totalRead == bytes.Length)
                        return bytes;
                    var result = new byte[totalRead];
                    Array.Copy(bytes, 0, result, 0, totalRead);
                    return result;
                }
                catch (OperationCanceledException)
                {
                    // Caller-driven cancellation is a legitimate signal, not a
                    // transport error. Propagate so the outer Ceen pipeline can
                    // honour the abort — swallowing here would translate the
                    // aborted request into a benign 404 / null-body response and
                    // mask the abort from telemetry and the request lifecycle.
                    throw;
                }
                catch (Exception ex)
                {
                    // Transport / IO failures are intentionally swallowed to the
                    // caller's null-return contract (a malformed asset POST must
                    // not tear down the request pipeline), but leave a Trace
                    // breadcrumb naming the exception type so operators tailing
                    // a Trace listener can distinguish "aborted upstream" from
                    // "silent drop" without recompiling. Trace is BCL-only and
                    // costs nothing when no listener is attached; the original
                    // "swallow everything" semantics are preserved.
                    //
                    // Strip CR / LF from ex.Message before emitting so a nested
                    // exception whose Message carries a newline cannot split
                    // the trace line and forge a second-record entry when a
                    // TextWriterTraceListener / FileLogTraceListener is wired
                    // — OWASP A09 log-format-injection defence.
                    var safeMessage = ex.Message == null
                        ? string.Empty
                        : ex.Message.Replace('\r', ' ').Replace('\n', ' ');
                    System.Diagnostics.Trace.WriteLine(
                        $"MTConnectPostResponseHandler.ReadRequestBytes swallowed "
                        + $"{ex.GetType().FullName}: {safeMessage}");
                }
            }

            return null;
        }
    }
}
