// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    /// <summary>
    /// The request / response handling shared by the Probe, Current, Sample and Asset clients: send one
    /// request, decode the body, and read it as the expected document or, failing that, as an MTConnectError
    /// document (which Agents send with 4xx status codes such as 404 NO_DEVICE or 400 INVALID_REQUEST).
    /// </summary>
    internal static class MTConnectHttpDocumentRequest
    {
        /// <summary>
        /// The client events a request reports through.
        /// </summary>
        internal sealed class Handlers
        {
            public Action<IErrorResponseDocument> MTConnectError { get; set; }
            public Action<IFormatReadResult> FormatError { get; set; }
            public Action<Exception> ConnectionError { get; set; }
            public Action<Exception> InternalError { get; set; }
        }


        public static HttpRequestMessage CreateRequest(Uri uri, string contentType, string documentFormat, IEnumerable<HttpContentEncoding> contentEncodings)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = uri;

            // Add 'Accept' HTTP Header
            if (string.IsNullOrEmpty(contentType)) contentType = ResponseDocumentFormatter.GetContentType(documentFormat);
            if (!string.IsNullOrEmpty(contentType)) request.Headers.Add(HttpHeaders.Accept, contentType);

            // Add 'Accept-Encoding' HTTP Header
            if (!contentEncodings.IsNullOrEmpty())
            {
                foreach (var contentEncoding in contentEncodings)
                {
                    request.Headers.Add(HttpHeaders.AcceptEncoding, contentEncoding.ToString().ToLower());
                }
            }

            return request;
        }

        /// <summary>
        /// Synchronous form of <see cref="SendAsync"/>. The work runs on the thread pool, so blocking on it from a
        /// thread with a synchronization context (e.g. a UI thread) cannot deadlock.
        /// </summary>
        public static T Send<T>(HttpClient httpClient, Func<HttpRequestMessage> createRequest, string documentFormat, int timeout, Func<string, Stream, FormatReadResult<T>> read, Handlers handlers) where T : class
        {
            return Task.Run(() => SendAsync(httpClient, createRequest, documentFormat, timeout, read, handlers, CancellationToken.None)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends the request and returns the document, or <c>null</c> after reporting what went wrong through
        /// <paramref name="handlers"/>. <paramref name="timeout"/> (milliseconds, 0 = none) bounds the whole request.
        /// </summary>
        public static async Task<T> SendAsync<T>(HttpClient httpClient, Func<HttpRequestMessage> createRequest, string documentFormat, int timeout, Func<string, Stream, FormatReadResult<T>> read, Handlers handlers, CancellationToken cancellationToken) where T : class
        {
            using (var timeoutSource = new CancellationTokenSource())
            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token))
            {
                if (timeout > 0) timeoutSource.CancelAfter(timeout);

                try
                {
                    using (var request = createRequest())
                    using (var response = await httpClient.SendAsync(request, linked.Token).ConfigureAwait(false))
                    {
                        Stream documentStream = null;
                        if (response.Content != null)
                        {
#if NET5_0_OR_GREATER
                            documentStream = await response.Content.ReadAsStreamAsync(linked.Token).ConfigureAwait(false);
#else
                            documentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
                        }

                        return ReadDocument(response, documentStream, documentFormat, read, handlers);
                    }
                }
                catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    handlers.ConnectionError?.Invoke(new TimeoutException($"HTTP Request Timeout Exceeded ({timeout} ms)"));
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                {
                    handlers.ConnectionError?.Invoke(ex);
                }
                catch (OperationCanceledException) { /* Ignore Task Cancelled */ }
                catch (HttpRequestException ex)
                {
                    handlers.ConnectionError?.Invoke(ex);
                }
                catch (Exception ex)
                {
                    handlers.InternalError?.Invoke(ex);
                }
            }

            return null;
        }

        internal static T ReadDocument<T>(HttpResponseMessage response, Stream documentStream, string documentFormat, Func<string, Stream, FormatReadResult<T>> read, Handlers handlers) where T : class
        {
            Stream stream = null;
            if (documentStream != null && documentStream.CanSeek && documentStream.Length > 0)
            {
                // Handle Compression Encoding
                var contentEncoding = MTConnectHttpResponse.GetContentHeaderValue(response, HttpHeaders.ContentEncoding);
                stream = MTConnectHttpResponse.HandleContentEncoding(contentEncoding, documentStream);
                if (stream != null && stream.CanSeek && stream.Position > 0) stream.Seek(0, SeekOrigin.Begin);
            }

            if (response.IsSuccessStatusCode && stream != null)
            {
                var formatResult = read(documentFormat, stream);
                if (formatResult.Success && formatResult.Content != null) return formatResult.Content;

                // Not the expected document: it may be an MTConnectError document
                if (TryReadError(stream, documentFormat, handlers)) return null;

                // Raise Format Error
                handlers.FormatError?.Invoke(formatResult);
                return null;
            }

            // Agents describe 4xx / 5xx failures in an MTConnectError document
            if (stream != null && TryReadError(stream, documentFormat, handlers)) return null;

            handlers.ConnectionError?.Invoke(new HttpRequestException($"HTTP {(int)response.StatusCode} {response.ReasonPhrase}"));
            return null;
        }

        private static bool TryReadError(Stream stream, string documentFormat, Handlers handlers)
        {
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

            var errorFormatResult = ResponseDocumentFormatter.CreateErrorResponseDocument(documentFormat, stream);
            if (errorFormatResult.Success && errorFormatResult.Content != null)
            {
                handlers.MTConnectError?.Invoke(errorFormatResult.Content);
                return true;
            }

            return false;
        }
    }
}
