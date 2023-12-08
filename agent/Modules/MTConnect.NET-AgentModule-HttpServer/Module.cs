// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Servers.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace MTConnect.Modules.Http
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "http-server";
        private const string ModuleId = "HTTP Server";

        private readonly Logger _httpLogger = LogManager.GetLogger("http-server-logger");
        private readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");
        private readonly ModuleConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private MTConnectHttpServer _httpServer;

        public Module(IMTConnectAgentBroker mtconnectAgent, object controllerConfiguration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _mtconnectAgent = mtconnectAgent;
            _configuration = AgentApplicationConfiguration.GetConfiguration<ModuleConfiguration>(controllerConfiguration);
        }


        protected override void OnStartAfterLoad()
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
            _httpLogger.Info($"[HTTP-Server] : Listening at " + prefix + "..");
        }

        private void HttpListenerStopped(object sender, string prefix)
        {
            _httpLogger.Info($"[HTTP-Server] : Listener Stopped for " + prefix);
        }

        private void HttpServerCertificateLoaded(object sender, X509Certificate2 certificate)
        {
            _httpLogger.Info($"[HTTP-Server] : TLS Certificate Loaded : {certificate.ToString()}");
        }

        private void HttpListenerException(object sender, Exception exception)
        {
            _httpLogger.Warn($"[HTTP-Server] : Listener Exception : " + exception.Message);
        }

        private void HttpClientConnected(object sender, IHttpRequest request)
        {
            _httpLogger.Debug($"[HTTP-Server] : Http Client Connected : (" + request.Method + ") : " + request.RemoteEndPoint + " : " + request.OriginalPath);
        }

        private void HttpClientDisconnected(object sender, string remoteEndPoint)
        {
            _httpLogger.Debug($"[HTTP-Server] : Http Client Disconnected : " + remoteEndPoint);
        }

        private void HttpClientException(object sender, Exception exception)
        {
            _httpLogger.Debug($"[HTTP-Server] : Http Client Exception : " + exception.Message);
        }

        private void HttpResponseSent(object sender, MTConnectHttpResponse response)
        {
            var totalTime = response.ResponseDuration + response.FormatDuration + response.WriteDuration;

            var logResponseMessage = new List<string>();
            logResponseMessage.Add("[HTTP-Server]");
            logResponseMessage.Add("Http Response Sent");
            logResponseMessage.Add($"{response.StatusCode}");
            logResponseMessage.Add($"{response.ContentType}");
            logResponseMessage.Add($"Agent Time {response.ResponseDuration.ToString("N3")}ms");
            logResponseMessage.Add($"Format Time {response.FormatDuration.ToString("N3")}ms");
            logResponseMessage.Add($"Write Time {response.WriteDuration.ToString("N3")}ms");
            logResponseMessage.Add($"Total Time {totalTime.ToString("N3")}ms");
            _httpLogger.Info(string.Join(" : ", logResponseMessage));

            // Format Messages
            if (!response.FormatMessages.IsNullOrEmpty())
            {
                foreach (var message in response.FormatMessages)
                {
                    _agentValidationLogger.Debug($"[HTTP-Server] : Formatter Message : {message}");
                }
            }

            // Format Warnings
            if (!response.FormatWarnings.IsNullOrEmpty())
            {
                foreach (var warning in response.FormatWarnings)
                {
                    _agentValidationLogger.Warn($"[HTTP-Server] : Formatter Warning : {warning}");
                }
            }

            // Format Errors
            if (!response.FormatErrors.IsNullOrEmpty())
            {
                foreach (var error in response.FormatErrors)
                {
                    _agentValidationLogger.Error($"[HTTP-Server] : Formatter Error : {error}");
                }
            }
        }

        #endregion

    }
}