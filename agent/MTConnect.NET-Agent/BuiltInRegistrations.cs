// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

extern alias FanucAdapter;
extern alias HaasAdapter;
extern alias HttpAdapter;
extern alias HttpServer;
extern alias MqttAdapter;
extern alias MqttBroker;
extern alias MqttRelay;
extern alias ShdrAdapter;
extern alias TormachAdapter;
extern alias WebUI;

using MTConnect.Agents;
using MTConnect.Formatters;

namespace MTConnect.Applications
{
    /// <summary>
    /// Registers the modules, processors and formatters that ship with the agent, so they
    /// load without depending on the scan of DLLs in the application directory (which finds
    /// nothing under single-file or trimmed publishing). The scan still runs, so a
    /// third-party module or formatter DLL copied next to the agent keeps working.
    /// </summary>
    internal static class BuiltInRegistrations
    {
        public static void Register()
        {
            // Agent modules (each is only started when it is enabled in agent.config.yaml)
            MTConnectAgentModules.Register<FanucAdapter::MTConnect.Modules.Module>();
            MTConnectAgentModules.Register<HaasAdapter::MTConnect.Modules.Module>();
            MTConnectAgentModules.Register<HttpAdapter::MTConnect.Modules.Module>();
            MTConnectAgentModules.Register<HttpServer::MTConnect.Modules.Http.Module>();
            MTConnectAgentModules.Register<MqttAdapter::MTConnect.Modules.Module>();
            MTConnectAgentModules.Register<MqttBroker::MTConnect.Modules.Module>();
            MTConnectAgentModules.Register<MqttRelay::MTConnect.Module>();
            MTConnectAgentModules.Register<ShdrAdapter::MTConnect.Modules.Module>();
            MTConnectAgentModules.Register<TormachAdapter::MTConnect.Modules.Module>();
            MTConnectAgentModules.Register<WebUI::MTConnect.Modules.WebUI.Module>();

            // Agent processors
            MTConnectAgentProcessors.Register<MTConnect.Processors.Processor>();

            // Response document formatters (the formats the agent can serve)
            ResponseDocumentFormatter.Register(new MTConnect.Formatters.Xml.XmlResponseDocumentFormatter());
            ResponseDocumentFormatter.Register(new MTConnect.Formatters.JsonResponseDocumentFormatter());
            ResponseDocumentFormatter.Register(new MTConnect.Formatters.JsonHttpResponseDocumentFormatter());

            // Entity formatters
            EntityFormatter.Register(new MTConnect.Formatters.Xml.XmlEntityFormatter());
            EntityFormatter.Register(new MTConnect.Formatters.JsonEntityFormatter());
            EntityFormatter.Register(new MTConnect.Formatters.JsonHttpEntityFormatter());

            // Input and path formatters
            InputFormatter.Register(new MTConnect.Formatters.JsonInputFormatter());
            PathFormatter.Register(new MTConnect.Formatters.Xml.XmlPathFormatter());
        }
    }
}
