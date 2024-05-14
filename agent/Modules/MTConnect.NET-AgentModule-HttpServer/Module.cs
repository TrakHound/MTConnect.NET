// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Logging;
using MTConnect.Servers.Http;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace MTConnect.Modules.Http
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "http-server";
        private const string ModuleId = "HTTP Server";
        private const string HttpServerLogId = "HTTP-Server";
        private const string ValidationLogId = "Agent-Validation";

        private readonly HttpServerModuleConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private MTConnectHttpServer _httpServer;

        public Module(IMTConnectAgentBroker mtconnectAgent, object controllerConfiguration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _mtconnectAgent = mtconnectAgent;
            _configuration = AgentApplicationConfiguration.GetConfiguration<HttpServerModuleConfiguration>(controllerConfiguration);
        }


        protected override void OnStartAfterLoad(bool initializeDataItems)
        {
            // Intialize the Http Server
            _httpServer = new MTConnectShdrHttpAgentServer(_configuration, _mtconnectAgent);

            _httpServer.ServerStarted += HttpListenerStarted;
            _httpServer.ServerStopped += HttpListenerStopped;
            _httpServer.ServerCertificateLoaded += HttpServerCertificateLoaded;

            // Setup Http Server Logging
            _httpServer.ServerException += HttpListenerException;
            _httpServer.ClientConnected += HttpClientConnected;
            _httpServer.ClientDisconnected += HttpClientDisconnected;
            _httpServer.ClientException += HttpClientException;
            _httpServer.ResponseSent += HttpResponseSent;

            // Start the Http Server
            _httpServer.Start();
        }

        protected override void OnStop()
        {
            if (_httpServer != null)
            {
                _httpServer.Stop();
                _httpServer.Dispose();
            }
        }


        #region "Logging"

        private void HttpListenerStarted(object sender, string prefix)
        {
            Log(MTConnectLogLevel.Information, $"Listening at " + prefix + "..", HttpServerLogId);
        }

        private void HttpListenerStopped(object sender, string prefix)
        {
            Log(MTConnectLogLevel.Information, $"Listener Stopped for " + prefix, HttpServerLogId);
        }

        private void HttpServerCertificateLoaded(object sender, X509Certificate2 certificate)
        {
            Log(MTConnectLogLevel.Debug, $"TLS Certificate Loaded : {certificate.ToString()}", HttpServerLogId);
        }

        private void HttpListenerException(object sender, Exception exception)
        {
            Log(MTConnectLogLevel.Warning, $"Listener Exception : " + exception.Message, HttpServerLogId);
        }

        private void HttpClientConnected(object sender, IHttpRequest request)
        {
            Log(MTConnectLogLevel.Debug, $"Http Client Connected : (" + request.Method + ") : " + request.RemoteEndPoint + " : " + request.OriginalPath, HttpServerLogId);
        }

        private void HttpClientDisconnected(object sender, string remoteEndPoint)
        {
            Log(MTConnectLogLevel.Debug, $"Http Client Disconnected : " + remoteEndPoint, HttpServerLogId);
        }

        private void HttpClientException(object sender, Exception exception)
        {
            Log(MTConnectLogLevel.Debug, $"Http Client Exception : " + exception.Message, HttpServerLogId);
        }

        private void HttpResponseSent(object sender, MTConnectHttpResponse response)
        {
            var totalTime = response.ResponseDuration + response.FormatDuration + response.WriteDuration;

            var logResponseMessage = new List<string>();
            logResponseMessage.Add("Http Response Sent");
            logResponseMessage.Add($"{response.StatusCode}");
            logResponseMessage.Add($"{response.ContentType}");
            logResponseMessage.Add($"Agent Time {response.ResponseDuration.ToString("N3")}ms");
            logResponseMessage.Add($"Format Time {response.FormatDuration.ToString("N3")}ms");
            logResponseMessage.Add($"Write Time {response.WriteDuration.ToString("N3")}ms");
            logResponseMessage.Add($"Total Time {totalTime.ToString("N3")}ms");
            Log(MTConnectLogLevel.Debug, string.Join(" : ", logResponseMessage), HttpServerLogId);

            // Format Messages
            if (!response.FormatMessages.IsNullOrEmpty())
            {
                foreach (var message in response.FormatMessages)
                {
                    Log(MTConnectLogLevel.Debug, $"Formatter Message : {message}", ValidationLogId);
                }
            }

            // Format Warnings
            if (!response.FormatWarnings.IsNullOrEmpty())
            {
                foreach (var warning in response.FormatWarnings)
                {
                    Log(MTConnectLogLevel.Warning, $"Formatter Warning : {warning}", ValidationLogId);
                }
            }

            // Format Errors
            if (!response.FormatErrors.IsNullOrEmpty())
            {
                foreach (var error in response.FormatErrors)
                {
                    Log(MTConnectLogLevel.Error, $"Formatter Error : {error}", ValidationLogId);
                }
            }
        }

        #endregion

    }
}