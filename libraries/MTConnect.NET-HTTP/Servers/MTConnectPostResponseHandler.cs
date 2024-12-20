// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Errors;
using MTConnect.Servers.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MTConnect.Servers
{
    class MTConnectPostResponseHandler : MTConnectHttpResponseHandler
    {
        public Func<MTConnectAssetInputArgs, bool> ProcessFunction { get; set; }


        public MTConnectPostResponseHandler(
            IHttpServerConfiguration serverConfiguration, 
            IMTConnectAgentBroker mtconnectAgent, 
            ILogger logger = null) 
            : base(
                serverConfiguration, 
                mtconnectAgent, 
                logger) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
        {
            var response = new MTConnectHttpResponse();

            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.Path != null && httpResponse != null)
            {
                var requestBytes = await ReadRequestBytes(context.Request.Body);
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

        private static async Task<byte[]> ReadRequestBytes(Stream inputStream)
        {
            if (inputStream != null)
            {
                try
                {
                    var bufferSize = 1048576 * 2; // 2 MB
                    var bytes = new byte[bufferSize];
                    await inputStream.ReadAsync(bytes, 0, bytes.Length);

                    return TrimEnd(bytes);
                }
                catch { }
            }

            return null;
        }

        public static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);

            Array.Resize(ref array, lastIndex + 1);

            return array;
        }
    }
}
