// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Threading;
using DotNet.Testcontainers.Configurations;

namespace MTConnect.Tests.Shared
{
    /// <summary>
    /// What a Docker-backed (Testcontainers) test does when it starts.
    /// </summary>
    internal enum DockerGateDecision
    {
        /// <summary>Docker is reachable: run the test.</summary>
        Run,

        /// <summary>Docker is not reachable and the Docker suites were not asked for: skip the test.</summary>
        Skip,

        /// <summary>Docker is not reachable but <c>MTCONNECT_E2E_DOCKER</c> asks for the Docker suites: fail.</summary>
        Fail
    }

    /// <summary>
    /// Whether the Docker-backed tests can run. Linked into the test projects that use Testcontainers
    /// (via <c>&lt;Compile Include="..\Shared\DockerAvailability.cs" /&gt;</c>), which turn its decision
    /// into their framework's skip: a plain <c>dotnet test</c> without Docker reports those tests as
    /// skipped rather than failed, while <c>MTCONNECT_E2E_DOCKER</c> set to a truthy value (as
    /// <c>tools/test.ps1 --e2e</c> and CI's Docker jobs set it) turns an unreachable Docker into a
    /// failure instead of a silent skip.
    /// </summary>
    internal static class DockerAvailability
    {
        /// <summary>
        /// The environment variable that asks for the Docker-gated suites (see docs/cli/test-sh.md).
        /// </summary>
        public const string EnvironmentVariable = "MTCONNECT_E2E_DOCKER";

        // The truthy values tools/test.ps1 (Get-E2EEnabled) and tools/test.sh accept
        private static readonly string[] _truthyValues = new[] { "true", "yes", "on", "1" };

        private static readonly TimeSpan _pingTimeout = TimeSpan.FromSeconds(10);

        // Probed once per test run and shared by every Docker-backed test
        private static readonly Lazy<bool> _isReachable = new Lazy<bool>(() => Ping(_pingTimeout), LazyThreadSafetyMode.ExecutionAndPublication);


        /// <summary>
        /// Whether a Docker endpoint resolves (Testcontainers' own discovery) and answers a ping.
        /// Probed once and cached.
        /// </summary>
        public static bool IsReachable => _isReachable.Value;

        /// <summary>
        /// The current <see cref="EnvironmentVariable"/> value.
        /// </summary>
        public static string? EnvironmentValue => Environment.GetEnvironmentVariable(EnvironmentVariable);

        /// <summary>
        /// The decision for the current environment.
        /// </summary>
        public static DockerGateDecision Current => Decide(IsReachable, EnvironmentValue);

        /// <summary>
        /// The message for a skipped Docker-backed test.
        /// </summary>
        public static string SkipMessage => $"Docker is not reachable; skipping this Docker-backed test. Start Docker (and set {EnvironmentVariable}=true to make an unreachable Docker a failure) to run it.";

        /// <summary>
        /// The message for a failed Docker-backed test, given the <see cref="EnvironmentVariable"/> value.
        /// </summary>
        public static string FailMessage(string? environmentValue) => $"{EnvironmentVariable}={environmentValue} asks for the Docker suites, but Docker is not reachable.";


        /// <summary>
        /// Whether <paramref name="value"/> (the <see cref="EnvironmentVariable"/> value) asks for the Docker suites.
        /// </summary>
        public static bool IsRequested(string? value)
        {
            return !string.IsNullOrWhiteSpace(value) && _truthyValues.Contains(value!.Trim().ToLowerInvariant());
        }

        /// <summary>
        /// Decides whether a Docker-backed test runs, is skipped, or fails.
        /// </summary>
        public static DockerGateDecision Decide(bool dockerReachable, string? environmentValue)
        {
            if (dockerReachable) return DockerGateDecision.Run;
            return IsRequested(environmentValue) ? DockerGateDecision.Fail : DockerGateDecision.Skip;
        }

        private static bool Ping(TimeSpan timeout)
        {
            try
            {
                var endpoint = TestcontainersSettings.OS.DockerEndpointAuthConfig;
                if (endpoint == null) return false;

                using (var configuration = endpoint.GetDockerClientConfiguration(Guid.NewGuid()))
                using (var client = configuration.CreateClient())
                using (var cts = new CancellationTokenSource(timeout))
                {
                    client.System.PingAsync(cts.Token).GetAwaiter().GetResult();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
