// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using Ceen.Httpd;
using MTConnect.Agents;
using MTConnect.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Servers.Http
{
    /// <summary>
    /// An Http Web Server for processing MTConnect REST Api Requests
    /// </summary>
    public class MTConnectHttpServer : IDisposable
    {
        protected readonly IMTConnectAgentBroker _mtconnectAgent;
        protected readonly IHttpServerConfiguration _configuration;
        private CancellationTokenSource _stop;


        /// <summary>
        /// Event for when an error occurs with a MTConnectHttpResponse is written to the HTTP Client
        /// </summary>
        public event EventHandler<MTConnectHttpResponse> ResponseSent;

        /// <summary>
        /// Event for when the HttpListener is started
        /// </summary>
        /// <returns>URL Prefix that the HttpListener is listening for requests on</returns>
        public event EventHandler<string> ServerStarted;

        /// <summary>
        /// Event for when the HttpListener is stopped
        /// </summary>
        /// <returns>URL Prefix that the HttpListener is listening for requests on</returns>
        public event EventHandler<string> ServerStopped;

        public event EventHandler<X509Certificate2> ServerCertificateLoaded;


        public event EventHandler<string> ServerLogRecevied;

        /// <summary>
        /// Event for when an error occurs with the HttpListener
        /// </summary>
        public event EventHandler<Exception> ServerException;


        /// <summary>
        /// Event for when a client makes a request to the server
        /// </summary>
        public event EventHandler<IHttpRequest> ClientConnected;

        /// <summary>
        /// Event for when a client completes a request or disconnects from the server
        /// </summary>
        public event EventHandler<string> ClientDisconnected;

        /// <summary>
        /// Event for when an error occurs with the HttpListenerRequest
        /// </summary>
        public event EventHandler<Exception> ClientException;


        public MTConnectHttpServer(IHttpServerConfiguration configuration, IMTConnectAgentBroker mtconnectAgent)
        {
            _mtconnectAgent = mtconnectAgent;
            _configuration = configuration;
        }


        /// <summary>
        /// Method run when an Observation is attempted to be added to the MTConnect Agent from an HTTP PUT request
        /// </summary>
        /// <returns>Returns False if a Device cannot be found from the specified DeviceKey</returns>
        protected virtual bool OnObservationInput(MTConnectObservationInputArgs args) { return false; }

        /// <summary>
        /// Method run when an Asset is attempted to be added to the MTConnect Agent from an HTTP PUT request
        /// </summary>
        /// <returns>Returns False if a Device cannot be found from the specified DeviceKey</returns>
        protected virtual bool OnAssetInput(MTConnectAssetInputArgs args) { return false; }

        /// <summary>
        /// Method run after the initial ServerConfig is set but before the server is started. Used to edit the configuration and/or to add routes
        /// </summary>
        protected virtual void OnConfigureServer(ServerConfig serverConfig) { }

        /// <summary>
        /// Method run on a Static File request
        /// </summary>
        protected virtual Stream OnProcessStatic(MTConnectStaticFileRequest request) { return null; }

        /// <summary>
        /// Method run when creating the Format Options after an MTConnect REST response is processed and before it is returned
        /// </summary>
        protected virtual List<KeyValuePair<string, string>> OnCreateFormatOptions(MTConnectFormatOptionsArgs args) { return null; }


        public void Start()
        {
            _stop = new CancellationTokenSource();

            if (_configuration != null)
            {
                _ = Task.Run(() => StartServer(_configuration, _stop.Token));
            }
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
        }

        public void Dispose() { Stop(); }


        private async Task StartServer(IHttpServerConfiguration serverConfiguration, CancellationToken cancellationToken)
        {
            if (serverConfiguration != null)
            {
                string endpointId;
                if (!string.IsNullOrEmpty(serverConfiguration.Server)) endpointId = $"{serverConfiguration.Server}, Port = {serverConfiguration.Port}";
                else endpointId = $"Port = {serverConfiguration.Port}";

                try
                {
                    do
                    {
                        var errorOccurred = false;

                        try
                        {
                            var config = CreateServerConfig(serverConfiguration);

                            var useSsl = false;
                            if (serverConfiguration.Tls != null)
                            {
                                useSsl = true;

                                var certificateLoadResult = serverConfiguration.Tls.GetCertificate();
                                if (certificateLoadResult.Success)
                                {
                                    config.SSLCertificate = certificateLoadResult.Certificate;
                                    if (ServerCertificateLoaded != null) ServerCertificateLoaded.Invoke(this, certificateLoadResult.Certificate);
                                }
                                else if (certificateLoadResult.Exception != null)
                                {
                                    if (ServerException != null) ServerException.Invoke(this, certificateLoadResult.Exception);
                                }
                            }

                            EndPoint endpoint = null;
                            if (!string.IsNullOrEmpty(serverConfiguration.Server))
                            {
								var hostEntry = Dns.GetHostEntry(serverConfiguration.Server);
                                if (hostEntry != null && !hostEntry.AddressList.IsNullOrEmpty())
                                {
                                    var hostAddress = hostEntry.AddressList.FirstOrDefault(o => o.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                                    if (hostAddress != null)
                                    {
										endpoint = new IPEndPoint(hostAddress, serverConfiguration.Port);
									}
                                }
                            }
                            else endpoint = new IPEndPoint(IPAddress.Any, serverConfiguration.Port);


                            if (ServerStarted != null) ServerStarted.Invoke(this, endpointId);

                            await HttpServer.ListenAsync(endpoint, useSsl, config, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            errorOccurred = true;
                            if (ServerException != null) ServerException.Invoke(this, ex);
                        }
                        finally
                        {
                            if (ServerStopped != null) ServerStopped.Invoke(this, endpointId);
                        }

                        // Delay 1 second for errors (to prevent a reoccurring error from overloading)
                        if (errorOccurred) await Task.Delay(1000);
                    }
                    while (!cancellationToken.IsCancellationRequested);
                }
                catch
                {
                    // Ignore Task Canceled Exceptions
                }
            }
        }
        private ServerConfig CreateServerConfig(IHttpServerConfiguration serverConfiguration)
        {
            var serverConfig = new ServerConfig();

            serverConfig.KeepAliveMaxRequests = 100;
            serverConfig.KeepAliveTimeoutSeconds = 60;
            serverConfig.MaxActiveRequests = 100;
            serverConfig.RequestIdleTimeoutSeconds = 60;
            serverConfig.MaxProcessingTimeSeconds = 60;

            // Setup the Probe Request Handler
            var probeHandler = new MTConnectProbeResponseHandler(serverConfiguration, _mtconnectAgent);
            probeHandler.ResponseSent += ResponseSent;
            probeHandler.ClientConnected += ClientConnected;
            probeHandler.ClientDisconnected += ClientDisconnected;
            probeHandler.ClientException += ClientException;
            probeHandler.CreateFormatOptionsFunction = OnCreateFormatOptions;

            // Setup the Current Request Handler
            var currentHandler = new MTConnectCurrentResponseHandler(serverConfiguration, _mtconnectAgent);
            currentHandler.ResponseSent += ResponseSent;
            currentHandler.ClientConnected += ClientConnected;
            currentHandler.ClientDisconnected += ClientDisconnected;
            currentHandler.ClientException += ClientException;
            currentHandler.CreateFormatOptionsFunction = OnCreateFormatOptions;

            // Setup the Sample Request Handler
            var sampleHandler = new MTConnectSampleResponseHandler(serverConfiguration, _mtconnectAgent);
            sampleHandler.ResponseSent += ResponseSent;
            sampleHandler.ClientConnected += ClientConnected;
            sampleHandler.ClientDisconnected += ClientDisconnected;
            sampleHandler.ClientException += ClientException;
            sampleHandler.CreateFormatOptionsFunction = OnCreateFormatOptions;

            // Setup the Assets Request Handler
            var assetsHandler = new MTConnectAssetsResponseHandler(serverConfiguration, _mtconnectAgent);
            assetsHandler.ResponseSent += ResponseSent;
            assetsHandler.ClientConnected += ClientConnected;
            assetsHandler.ClientDisconnected += ClientDisconnected;
            assetsHandler.ClientException += ClientException;
            assetsHandler.CreateFormatOptionsFunction = OnCreateFormatOptions;

            // Setup the Asset Request Handler
            var assetHandler = new MTConnectAssetResponseHandler(serverConfiguration, _mtconnectAgent);
            assetHandler.ResponseSent += ResponseSent;
            assetHandler.ClientConnected += ClientConnected;
            assetHandler.ClientDisconnected += ClientDisconnected;
            assetHandler.ClientException += ClientException;
            assetHandler.CreateFormatOptionsFunction = OnCreateFormatOptions;

			// Setup the Put Request Handler
			var putHandler = new MTConnectPutResponseHandler(serverConfiguration, _mtconnectAgent);
			putHandler.ResponseSent += ResponseSent;
			putHandler.ClientConnected += ClientConnected;
			putHandler.ClientDisconnected += ClientDisconnected;
			putHandler.ClientException += ClientException;
			putHandler.ProcessFunction = OnObservationInput;
			putHandler.CreateFormatOptionsFunction = OnCreateFormatOptions;

			// Setup the Post Request Handler
			var postHandler = new MTConnectPostResponseHandler(serverConfiguration, _mtconnectAgent);
			postHandler.ResponseSent += ResponseSent;
			postHandler.ClientConnected += ClientConnected;
			postHandler.ClientDisconnected += ClientDisconnected;
			postHandler.ClientException += ClientException;
			postHandler.ProcessFunction = OnAssetInput;
			postHandler.CreateFormatOptionsFunction = OnCreateFormatOptions;

			// Setup the Static Request Handler
			var staticHandler = new MTConnectStaticResponseHandler(serverConfiguration, _mtconnectAgent);
            staticHandler.ResponseSent += ResponseSent;
            staticHandler.ClientConnected += ClientConnected;
            staticHandler.ClientDisconnected += ClientDisconnected;
            staticHandler.ClientException += ClientException;
            staticHandler.ProcessFunction = OnProcessStatic;
            staticHandler.CreateFormatOptionsFunction = OnCreateFormatOptions;

            // Setup Routes (Processed in Order)
            serverConfig.AddRoute(async (context) =>
            {
                return await DeviceRootHandler(probeHandler, context);
            });
			serverConfig.AddRoute(async (context) =>
			{
				return await PutHandler(putHandler, context);
			});
            serverConfig.AddRoute(async (context) =>
            {
                return await PostHandler(postHandler, context);
            });
			serverConfig.AddRoute("/", probeHandler);
            serverConfig.AddRoute("/probe", probeHandler);
            serverConfig.AddRoute("/*/probe", probeHandler);
            serverConfig.AddRoute("/current", currentHandler);
            serverConfig.AddRoute("/*/current", currentHandler);
            serverConfig.AddRoute("/sample", sampleHandler);
            serverConfig.AddRoute("/*/sample", sampleHandler);
            serverConfig.AddRoute("/assets", assetsHandler);
            serverConfig.AddRoute("/*/assets", assetsHandler);
            serverConfig.AddRoute("/asset/*", assetHandler);
            serverConfig.AddRoute(staticHandler);

            return serverConfig;
        }

        private async Task<bool> DeviceRootHandler(MTConnectProbeResponseHandler handler, IHttpContext context)
        {
            if (context != null && context.Request != null && context.Request.Path != null)
            {
                if (context.Request.Method == HttpMethod.Get.Method)
                {
                    var deviceKey = context.Request.Path.Trim('/');
                    if (!string.IsNullOrEmpty(deviceKey))
                    {
                        var device = _mtconnectAgent.GetDevice(deviceKey);
                        if (device != null)
                        {
                            await handler.HandleAsync(context);

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static async Task<bool> PutHandler(MTConnectPutResponseHandler handler, IHttpContext context)
        {
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.Path != null && httpResponse != null)
            {
                if (httpRequest.Method == HttpMethod.Put.Method)
                {
                    await handler.HandleAsync(context);

                    return true;
                }
            }

            return false;
        }

        private static async Task<bool> PostHandler(MTConnectPostResponseHandler handler, IHttpContext context)
        {
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.Path != null && httpResponse != null)
            {
                if (httpRequest.Method == HttpMethod.Post.Method)
                {
                    await handler.HandleAsync(context);

                    return true;
                }
            }

            return false;
        }
    }
}