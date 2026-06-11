// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Platform
{
    // Pins the [SupportedOSPlatform("windows")] decoration on every type or
    // method that was annotated by commit dd2eb424 (2026-05-22) to silence
    // the CA1416 platform-compatibility analyzer on .NET 8. The attribute
    // type ships in System.Runtime on .NET 5.0+ but is absent from net4x
    // and netstandard2.0; PR #194 wraps each declaration plus its
    // using-directive in `#if NET5_0_OR_GREATER ... #endif` so the
    // Release pack survives the full TFM matrix.
    //
    // This fixture only exercises the net8.0 build path (the only TFM
    // every test project targets), and so cannot directly fail under the
    // pre-fix code (net8.0 keeps the attribute regardless of the wrap).
    // Its value is REGRESSION-PREVENTIVE: a future contributor who
    // removes the attribute outright — or who narrows the wrap to a
    // condition that excludes net8.0 — fails this test, and the
    // protection the original commit added stays in place.
    /// <summary>Pins SupportedOSPlatform("windows") on every site PR #194 wraps.</summary>
    [TestFixture]
    [Category("MultiTfmCompat")]
    public class SupportedOSPlatformAttributePresenceTests
    {
        private static void AssertWindowsPlatform(MemberInfo member, string description)
        {
            var attributes = member
                .GetCustomAttributes(typeof(SupportedOSPlatformAttribute), inherit: false)
                .Cast<SupportedOSPlatformAttribute>()
                .ToArray();

            Assert.That(attributes, Is.Not.Empty,
                $"{description}: expected [SupportedOSPlatform] attribute.");
            Assert.That(
                attributes.Any(a => string.Equals(a.PlatformName, "windows", StringComparison.Ordinal)),
                Is.True,
                $"{description}: expected PlatformName == \"windows\" but got " +
                $"[{string.Join(", ", attributes.Select(a => a.PlatformName))}].");
        }

        /// <summary>MTConnect.Services.WindowsService carries the Windows-only attribute.</summary>
        [Test]
        public void WindowsService_carries_supported_os_platform_windows()
        {
            // WindowsService is internal — reach it via Assembly.GetType
            // rather than typeof(...).
            var servicesAssembly = typeof(MTConnect.Services.MTConnectAgentService).Assembly;
            var windowsServiceType = servicesAssembly.GetType(
                "MTConnect.Services.WindowsService", throwOnError: true);
            AssertWindowsPlatform(windowsServiceType!,
                "MTConnect.Services.WindowsService");
        }

        /// <summary>MTConnect.Services.MTConnectAgentService carries the Windows-only attribute.</summary>
        [Test]
        public void MTConnectAgentService_carries_supported_os_platform_windows()
        {
            AssertWindowsPlatform(typeof(MTConnect.Services.MTConnectAgentService),
                "MTConnect.Services.MTConnectAgentService");
        }

        /// <summary>MTConnect.Services.MTConnectAdapterService carries the Windows-only attribute.</summary>
        [Test]
        public void MTConnectAdapterService_carries_supported_os_platform_windows()
        {
            AssertWindowsPlatform(typeof(MTConnect.Services.MTConnectAdapterService),
                "MTConnect.Services.MTConnectAdapterService");
        }

        /// <summary>The agent-application Service class carries the Windows-only attribute.</summary>
        [Test]
        public void AgentApplications_Service_carries_supported_os_platform_windows()
        {
            // Both agent-application and adapter-application Service
            // types live under the same MTConnect.Applications
            // namespace; distinguish by assembly.
            var agentServiceType = typeof(MTConnect.Applications.IMTConnectAgentApplication).Assembly
                .GetType("MTConnect.Applications.Service", throwOnError: true);
            AssertWindowsPlatform(agentServiceType!,
                "MTConnect.Applications.Service (agent application)");
        }

        /// <summary>The adapter-application Service class carries the Windows-only attribute.</summary>
        [Test]
        public void AdapterApplications_Service_carries_supported_os_platform_windows()
        {
            var adapterServiceType = typeof(MTConnect.Applications.IMTConnectAdapterApplication).Assembly
                .GetType("MTConnect.Applications.Service", throwOnError: true);
            AssertWindowsPlatform(adapterServiceType!,
                "MTConnect.Applications.Service (adapter application)");
        }

        /// <summary>The Ceen HTTP-server socket-handle sites carry the Windows-only attribute.</summary>
        [Test]
        public void CeenHttpServer_socket_handle_sites_carry_supported_os_platform_windows()
        {
            // Both annotated members are internal to MTConnect.NET-HTTP:
            //   * Ceen.Httpd.HttpServer.InterProcessBridge.HandleRequest
            //   * Ceen.Httpd.HttpServer.RunClient (private static)
            // Reach them via Assembly.GetType + reflection.
            var httpAssembly = typeof(MTConnect.Servers.Http.MTConnectHttpRequests).Assembly;
            var httpServerType = httpAssembly.GetType(
                "Ceen.Httpd.HttpServer", throwOnError: true)!;
            var socketInformationType = typeof(System.Net.Sockets.SocketInformation);

            // The [SupportedOSPlatform("windows")] attribute lives on
            // Ceen.Httpd.HttpServer.AppDomainBridge.HandleRequest(
            // SocketInformation, EndPoint, string) — AppDomainBridge,
            // not InterProcessBridge, is the AppDomain-marshallable
            // sibling that wraps the duplicated socket handle. The
            // RunClient(SocketInformation,...) private static method on
            // HttpServer itself is the second site.
            var bridgeType = httpServerType.GetNestedType(
                "AppDomainBridge", BindingFlags.Public | BindingFlags.NonPublic);
            Assert.That(bridgeType, Is.Not.Null,
                "Ceen.Httpd.HttpServer.AppDomainBridge nested type not found.");

            const BindingFlags allInstance = BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance;
            // HandleRequest has two overloads; the SocketInformation-typed
            // one is the Windows-only path the attribute decorates. The
            // Socket-typed overload is cross-platform.
            var handleRequestCandidates = bridgeType!.GetMethods(allInstance)
                .Where(m => m.Name == "HandleRequest")
                .ToArray();
            var handleRequest = handleRequestCandidates
                .FirstOrDefault(m => m.GetParameters().Length > 0
                    && m.GetParameters()[0].ParameterType == socketInformationType);
            Assert.That(handleRequest, Is.Not.Null,
                "Ceen.Httpd.HttpServer.AppDomainBridge.HandleRequest(SocketInformation,...) overload not found among: " +
                string.Join("; ",
                    handleRequestCandidates.Select(m => $"{m.Name}({string.Join(",", m.GetParameters().Select(p => p.ParameterType.FullName))})")));

            const BindingFlags allStatic = BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            var runClient = httpServerType.GetMethods(allStatic)
                .FirstOrDefault(m => m.Name == "RunClient"
                    && m.GetParameters().Length > 0
                    && m.GetParameters()[0].ParameterType == socketInformationType);
            Assert.That(runClient, Is.Not.Null,
                "Ceen.Httpd.HttpServer.RunClient(SocketInformation,...) overload not found.");

            AssertWindowsPlatform(handleRequest!,
                "Ceen.Httpd.HttpServer.AppDomainBridge.HandleRequest");
            AssertWindowsPlatform(runClient!,
                "Ceen.Httpd.HttpServer.RunClient(SocketInformation,...)");
        }
    }
}
