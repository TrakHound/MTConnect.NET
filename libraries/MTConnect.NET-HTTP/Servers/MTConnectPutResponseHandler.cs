// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Errors;
using MTConnect.Servers.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Servers
{
    class MTConnectPutResponseHandler : MTConnectHttpResponseHandler
    {
        public Func<MTConnectObservationInputArgs, bool> ProcessFunction { get; set; }


        public MTConnectPutResponseHandler(IHttpServerConfiguration serverConfiguration, IMTConnectAgentBroker mtconnectAgent) : base(serverConfiguration, mtconnectAgent) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
        {
            var response = new MTConnectHttpResponse();

            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.Path != null && httpResponse != null)
            {
                if (_serverConfiguration.AllowPut && (_serverConfiguration.AllowPutFrom.IsNullOrEmpty() || _serverConfiguration.AllowPutFrom.Contains(httpRequest.GetRemoteIP())))
                {
					if (httpRequest.QueryString != null && httpRequest.QueryString.Count > 0)
					{
						var urlSegments = GetUriSegments(httpRequest.Path);

						// Read DeviceKey from URL Path
						var deviceKey = httpRequest.Path?.Trim('/');
						if (urlSegments.Length > 1) deviceKey = urlSegments[urlSegments.Length - 1];

						// Get list of KeyValuePairs from Url Query
						var items = new List<KeyValuePair<string, string>>();
						foreach (var key in httpRequest.QueryString.Keys)
						{
							var urlValue = httpRequest.QueryString[key];
							if (!string.IsNullOrEmpty(urlValue))
							{
								// Decode the input that was read from the URL
								var value = System.Web.HttpUtility.UrlDecode(urlValue);

								// Add to list of items
								items.Add(new KeyValuePair<string, string>(key, value));
							}
						}

						if (!items.IsNullOrEmpty())
						{
							var success = false;

							foreach (var item in items)
							{
								if (ProcessFunction != null)
								{
									var args = new MTConnectObservationInputArgs();
									args.DeviceKey = deviceKey;
									args.DataItemKey = item.Key;
									args.Value = item.Value;

									success = ProcessFunction(args);
								}

								// Call the OnObservationInput method that is intended to be overridden by a derived class
								//success = OnObservationInput(deviceKey, item.Key, item.Value);
								if (!success) break;
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
				else
				{
					response.StatusCode = 403;
				}
            }

            return response;
        }
    }
}
