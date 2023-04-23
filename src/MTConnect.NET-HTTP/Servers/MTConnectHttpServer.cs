// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using Ceen.Httpd;
using MTConnect.Agents;
using MTConnect.Configurations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Servers.Http
{
    public class MTConnectHttpServer : IDisposable
    {
        protected readonly IMTConnectAgentBroker _mtconnectAgent;
        protected readonly IHttpAgentConfiguration _configuration;
        private CancellationTokenSource _stop;


        /// <summary>
        /// Event Handler for when an error occurs with a MTConnectHttpResponse is written to the HTTP Client
        /// </summary>
        public EventHandler<MTConnectHttpResponse> ResponseSent { get; set; }

        /// <summary>
        /// Event Handler for when the HttpListener is started
        /// </summary>
        /// <returns>URL Prefix that the HttpListener is listening for requests on</returns>
        public EventHandler<string> ServerStarted { get; set; }

        /// <summary>
        /// Event Handler for when the HttpListener is stopped
        /// </summary>
        /// <returns>URL Prefix that the HttpListener is listening for requests on</returns>
        public EventHandler<string> ServerStopped { get; set; }

        public EventHandler<X509Certificate2> ServerCertificateLoaded { get; set; }

        /// <summary>
        /// Event Handler for when an error occurs with the HttpListener
        /// </summary>
        public EventHandler<Exception> ServerException { get; set; }


        /// <summary>
        /// Event Handler for when a client makes a request to the server
        /// </summary>
        public EventHandler<IHttpRequest> ClientConnected { get; set; }

        /// <summary>
        /// Event Handler for when a client completes a request or disconnects from the server
        /// </summary>
        public EventHandler<string> ClientDisconnected { get; set; }

        /// <summary>
        /// Event Handler for when an error occurs with the HttpListenerRequest
        /// </summary>
        public EventHandler<Exception> ClientException { get; set; }


        public MTConnectHttpServer(
            IHttpAgentConfiguration configuration,
            IMTConnectAgentBroker mtconnectAgent,
            IEnumerable<string> prefixes = null,
            int port = 0
            )
        {
            _mtconnectAgent = mtconnectAgent;
            _configuration = configuration;
        }


        /// <summary>
        /// Method run when an Observation is attempted to be added to the MTConnect Agent from an HTTP PUT request
        /// </summary>
        /// <returns>Returns False if a Device cannot be found from the specified DeviceKey</returns>
        protected virtual bool OnObservationInput(string deviceKey, string dataItemKey, string input)
        {
            return false;
        }

        /// <summary>
        /// Method run when an Asset is attempted to be added to the MTConnect Agent from an HTTP PUT request
        /// </summary>
        /// <returns>Returns False if a Device cannot be found from the specified DeviceKey</returns>
        protected virtual bool OnAssetInput(string assetId, string deviceKey, string assetType, byte[] requestBody, string documentFormat = DocumentFormat.XML)
        {
            return false;
        }


        public void Start()
        {
            _stop = new CancellationTokenSource();

            if (_configuration != null && !_configuration.Http.IsNullOrEmpty())
            {
                foreach (var httpServer in _configuration.Http)
                {
                    _ = Task.Run(() => StartServer(httpServer, _stop.Token));
                }
            }
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
        }

        public void Dispose() { Stop(); }


        private async Task StartServer(HttpServerConfiguration serverConfiguration, CancellationToken cancellationToken)
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
                            var config = CreateServerConfig();

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

                            EndPoint endpoint;
                            if (!string.IsNullOrEmpty(serverConfiguration.Server))
                            {
                                endpoint = new DnsEndPoint(serverConfiguration.Server, serverConfiguration.Port);
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
        private ServerConfig CreateServerConfig()
        {
            var serverConfig = new ServerConfig();

            serverConfig.KeepAliveMaxRequests = 100;
            serverConfig.KeepAliveTimeoutSeconds = 60;
            serverConfig.MaxActiveRequests = 100;
            serverConfig.RequestIdleTimeoutSeconds = 60;
            serverConfig.MaxProcessingTimeSeconds = 60;

            // Setup the Probe Request Handler
            var probeHandler = new MTConnectProbeResponseHandler(_configuration, _mtconnectAgent);
            probeHandler.ResponseSent += ResponseSent;
            probeHandler.ClientConnected += ClientConnected;
            probeHandler.ClientDisconnected += ClientDisconnected;
            probeHandler.ClientException += ClientException;
            serverConfig.AddRoute("/", probeHandler);
            serverConfig.AddRoute("[^\\/((?!probe|current|sample|assets).)*$]", probeHandler);
            serverConfig.AddRoute("/probe", probeHandler);
            serverConfig.AddRoute("/*/probe", probeHandler);

            // Setup the Current Request Handler
            var currentHandler = new MTConnectCurrentResponseHandler(_configuration, _mtconnectAgent);
            currentHandler.ResponseSent += ResponseSent;
            currentHandler.ClientConnected += ClientConnected;
            currentHandler.ClientDisconnected += ClientDisconnected;
            currentHandler.ClientException += ClientException;
            serverConfig.AddRoute("/current", currentHandler);
            serverConfig.AddRoute("/*/current", currentHandler);

            // Setup the Sample Request Handler
            var sampleHandler = new MTConnectSampleResponseHandler(_configuration, _mtconnectAgent);
            sampleHandler.ResponseSent += ResponseSent;
            sampleHandler.ClientConnected += ClientConnected;
            sampleHandler.ClientDisconnected += ClientDisconnected;
            sampleHandler.ClientException += ClientException;
            serverConfig.AddRoute("/sample", sampleHandler);
            serverConfig.AddRoute("/*/sample", sampleHandler);

            // Setup the Assets Request Handler
            var assetsHandler = new MTConnectAssetsResponseHandler(_configuration, _mtconnectAgent);
            assetsHandler.ResponseSent += ResponseSent;
            assetsHandler.ClientConnected += ClientConnected;
            assetsHandler.ClientDisconnected += ClientDisconnected;
            assetsHandler.ClientException += ClientException;
            serverConfig.AddRoute("/assets", assetsHandler);
            serverConfig.AddRoute("/*/assets", assetsHandler);

            // Setup the Asset Request Handler
            var assetHandler = new MTConnectAssetResponseHandler(_configuration, _mtconnectAgent);
            assetHandler.ResponseSent += ResponseSent;
            assetHandler.ClientConnected += ClientConnected;
            assetHandler.ClientDisconnected += ClientDisconnected;
            assetHandler.ClientException += ClientException;
            serverConfig.AddRoute("/asset/*", assetHandler);

            return serverConfig;
        }
    }
}