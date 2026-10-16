// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Configurations;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L2_CrossImpl
{
    /// <summary>
    /// What a <c>[Category("RequiresDocker")]</c> fixture does when it starts.
    /// </summary>
    internal enum DockerGateDecision
    {
        /// <summary>Docker is reachable: run the fixture.</summary>
        Run,

        /// <summary>Docker is not reachable and the Docker suites were not asked for: skip the fixture.</summary>
        Skip,

        /// <summary>Docker is not reachable but <c>MTCONNECT_E2E_DOCKER</c> asks for the Docker suites: fail.</summary>
        Fail
    }

    /// <summary>
    /// Gate for the Testcontainers-backed fixtures. Called first in their <c>[OneTimeSetUp]</c>, it
    /// skips the fixture (<see cref="Assert.Ignore(string)"/>) when Docker is not reachable, so a plain
    /// <c>dotnet test</c> without Docker reports the fixtures as skipped rather than failed. When
    /// <c>MTCONNECT_E2E_DOCKER</c> is truthy (as <c>tools/test.ps1 --e2e</c> and CI's Docker jobs set
    /// it) the Docker suites were asked for, so an unreachable Docker fails the fixture instead of
    /// silently skipping it.
    /// </summary>
    internal static class DockerGate
    {
        /// <summary>
        /// The environment variable that asks for the Docker-gated suites (see docs/cli/test-sh.md).
        /// </summary>
        public const string EnvironmentVariable = "MTCONNECT_E2E_DOCKER";

        // The truthy values tools/test.ps1 (Get-E2EEnabled) and tools/test.sh accept
        private static readonly string[] _truthyValues = new[] { "true", "yes", "on", "1" };

        private static readonly TimeSpan _pingTimeout = TimeSpan.FromSeconds(10);

        // Probed once per test run and shared by every gated fixture
        private static readonly Lazy<Task<bool>> _dockerReachable = new Lazy<Task<bool>>(() => IsDockerReachableAsync(_pingTimeout));


        /// <summary>
        /// Whether <paramref name="value"/> (the <see cref="EnvironmentVariable"/> value) asks for the Docker suites.
        /// </summary>
        public static bool IsRequested(string? value)
        {
            return !string.IsNullOrWhiteSpace(value) && _truthyValues.Contains(value.Trim().ToLowerInvariant());
        }

        /// <summary>
        /// Decides whether a gated fixture runs, is skipped, or fails.
        /// </summary>
        public static DockerGateDecision Decide(bool dockerReachable, string? environmentValue)
        {
            if (dockerReachable) return DockerGateDecision.Run;
            return IsRequested(environmentValue) ? DockerGateDecision.Fail : DockerGateDecision.Skip;
        }

        /// <summary>
        /// Skips (or, when the Docker suites were asked for, fails) the calling fixture when Docker is not
        /// reachable; returns normally when it is.
        /// </summary>
        public static async Task RequireDockerAsync()
        {
            var reachable = await _dockerReachable.Value.ConfigureAwait(false);
            var environmentValue = Environment.GetEnvironmentVariable(EnvironmentVariable);

            switch (Decide(reachable, environmentValue))
            {
                case DockerGateDecision.Skip:
                    Assert.Ignore($"Docker is not reachable; skipping this RequiresDocker fixture. Start Docker (and set {EnvironmentVariable}=true to make an unreachable Docker a failure) to run it.");
                    break;

                case DockerGateDecision.Fail:
                    Assert.Fail($"{EnvironmentVariable}={environmentValue} asks for the Docker suites, but Docker is not reachable.");
                    break;
            }
        }

        /// <summary>
        /// Whether a Docker endpoint resolves (Testcontainers' own discovery) and answers a ping within
        /// <paramref name="timeout"/>.
        /// </summary>
        internal static async Task<bool> IsDockerReachableAsync(TimeSpan timeout)
        {
            try
            {
                var endpoint = TestcontainersSettings.OS.DockerEndpointAuthConfig;
                if (endpoint == null) return false;

                using (var configuration = endpoint.GetDockerClientConfiguration(Guid.NewGuid()))
                using (var client = configuration.CreateClient())
                using (var cts = new CancellationTokenSource(timeout))
                {
                    await client.System.PingAsync(cts.Token).ConfigureAwait(false);
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
