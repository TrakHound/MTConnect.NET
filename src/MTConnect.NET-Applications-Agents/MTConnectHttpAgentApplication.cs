// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Configurations;
using MTConnect.Servers.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An MTConnect Agent application with a built in HTTP server to support the MTConnect Rest Api Protocol.
    /// Supports Command line arguments, Device management, Buffer management, Logging, Windows Service, and Configuration File management
    /// </summary>
    public class MTConnectHttpAgentApplication : MTConnectAgentApplication
    {
        private const string DefaultServiceName = "MTConnect-Agent-HTTP";
        private const string DefaultServiceDisplayName = "MTConnect HTTP Agent";
        private const string DefaultServiceDescription = "MTConnect Agent using HTTP to provide access to device information using the MTConnect Standard";


        private readonly Logger _httpLogger = LogManager.GetLogger("http-logger");

        private MTConnectHttpServer _httpServer;
        private IHttpAgentApplicationConfiguration _configuration;
        private int _port = 0;


        public MTConnectHttpAgentApplication()
        {
            ServiceName = DefaultServiceName;
            ServiceDisplayName = DefaultServiceDisplayName;
            ServiceDescription = DefaultServiceDescription;
        }


        /// <summary>
        /// Add the Http Port as the 3rd command line argument
        /// </summary>
        /// <param name="args"></param>
        protected override void OnCommandLineArgumentsRead(string[] args)
        {
            int port = 0;

            if (!args.IsNullOrEmpty())
            {
                // Port
                if (args.Length > 2)
                {
                    if (int.TryParse(args[2], out var p))
                    {
                        port = p;
                        _applicationLogger.Info($"Agent HTTP Port = {port}");
                    }
                }
            }

            _port = port;
        }

        protected override IAgentApplicationConfiguration OnConfigurationFileRead(string configurationPath)
        {
            // Read the Configuration File
            var configuration = AgentConfiguration.Read<HttpAgentApplicationConfiguration>(configurationPath);
            base.OnAgentConfigurationUpdated(configuration);
            _configuration = configuration;
            return _configuration;
        }

        protected override void OnAgentConfigurationWatcherInitialize(IAgentApplicationConfiguration configuration)
        {
            _agentConfigurationWatcher = new AgentConfigurationFileWatcher<HttpAgentApplicationConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
        }

        protected override void OnAgentConfigurationUpdated(AgentConfiguration configuration)
        {
            _configuration = configuration as IHttpAgentApplicationConfiguration;
        }

        protected virtual MTConnectHttpAgentServer OnHttpServerInitialize(int port)
        {
            return new MTConnectHttpAgentServer(_configuration, Agent, null, port);
        }

        protected override void OnStartAgent(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false) 
        {
            // Intialize the Http Server
            _httpServer = OnHttpServerInitialize(_port);

            // Setup Http Server Logging
            if (_verboseLogging)
            {
                _httpServer.ListenerStarted += HttpListenerStarted;
                _httpServer.ListenerStopped += HttpListenerStopped;
                _httpServer.ListenerException += HttpListenerException;
                _httpServer.ClientConnected += HttpClientConnected;
                _httpServer.ClientDisconnected += HttpClientDisconnected;
                _httpServer.ClientException += HttpClientException;
                _httpServer.ResponseSent += HttpResponseSent;
            }

            // Start the Http Server
            _httpServer.Start();
        }

        protected override void OnStopAgent() 
        {
            if (_httpServer != null) _httpServer.Stop();
            Thread.Sleep(2000); // Delay 2 seconds to allow Http Server to stop
        }


        #region "Help"

        protected override string OnPrintHelpUsage()
        {
            return "[http_port]";
        }

        protected override void OnPrintHelpArguments()
        {
            Console.WriteLine(string.Format("{0,20}  |  {1,5}", "http_port", "Specifies the TCP Port to use for the HTTP Server"));
            Console.WriteLine(string.Format("{0,20}     {1,5}", "", "Note : This overrides what is read from the Configuration file"));
        }

        #endregion

        #region "Logging"

        private void HttpListenerStarted(object sender, string prefix)
        {
            _httpLogger.Info($"[Http Server] : Listening at " + prefix + "..");
        }

        private void HttpListenerStopped(object sender, string prefix)
        {
            _httpLogger.Info($"[Http Server] : Listener Stopped for " + prefix);
        }

        private void HttpListenerException(object sender, Exception exception)
        {
            _httpLogger.Warn($"[Http Server] : Listener Exception : " + exception.Message);
        }

        private void HttpClientConnected(object sender, HttpListenerRequest request)
        {
            _httpLogger.Info($"[Http Server] : Http Client Connected : (" + request.HttpMethod + ") : " + request.LocalEndPoint + " : " + request.Url);
        }

        private void HttpClientDisconnected(object sender, string remoteEndPoint)
        {
            _httpLogger.Debug($"[Http Server] : Http Client Disconnected : " + remoteEndPoint);
        }

        private void HttpClientException(object sender, Exception exception)
        {
            _httpLogger.Log(_logLevel, $"[Http Server] : Http Client Exception : " + exception.Message);
        }

        private void HttpResponseSent(object sender, MTConnectHttpResponse response)
        {
            var totalTime = response.ResponseDuration + response.FormatDuration + response.WriteDuration;
            _httpLogger.Info($"[Http Server] : Http Response Sent : {response.StatusCode} : {response.ContentType} : Agent Process Time {response.ResponseDuration}ms : Document Format Time {response.FormatDuration}ms : Write Time {response.WriteDuration}ms : Total Response Time {totalTime}ms");

            // Format Messages
            if (!response.FormatMessages.IsNullOrEmpty())
            {
                foreach (var message in response.FormatMessages)
                {
                    _agentValidationLogger.Debug($"[Http Server] : Formatter Message : {message}");
                }
            }

            // Format Warnings
            if (!response.FormatWarnings.IsNullOrEmpty())
            {
                foreach (var warning in response.FormatWarnings)
                {
                    _agentValidationLogger.Warn($"[Http Server] : Formatter Warning : {warning}");
                }
            }

            // Format Errors
            if (!response.FormatErrors.IsNullOrEmpty())
            {
                foreach (var error in response.FormatErrors)
                {
                    _agentValidationLogger.Error($"[Http Server] : Formatter Error : {error}");
                }
            }
        }

        #endregion
    }
}