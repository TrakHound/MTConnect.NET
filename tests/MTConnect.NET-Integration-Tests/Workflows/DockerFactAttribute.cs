// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Tests.Shared;
using Xunit;

namespace MTConnect.Tests.Integration.Workflows
{
    /// <summary>
    /// A <see cref="FactAttribute"/> for Testcontainers-backed tests: skipped, rather than failed,
    /// when Docker is not reachable (see <see cref="DockerAvailability"/>). xUnit v2 has no runtime
    /// skip, so the decision is made when the test is discovered. When <c>MTCONNECT_E2E_DOCKER</c>
    /// asks for the Docker suites the test is not skipped, and <see cref="MqttBrokerFixture"/> fails
    /// it with a clear message.
    /// </summary>
    public sealed class DockerFactAttribute : FactAttribute
    {
        /// <summary>
        /// Creates the attribute, skipping the test when Docker is not reachable and not asked for.
        /// </summary>
        public DockerFactAttribute()
        {
            var skip = GetSkipReason(DockerAvailability.Current);
            if (skip != null) Skip = skip;
        }

        /// <summary>
        /// The skip reason for <paramref name="decision"/>, or <c>null</c> when the test should run
        /// (or fail, which happens when it runs).
        /// </summary>
        internal static string? GetSkipReason(DockerGateDecision decision)
        {
            return decision == DockerGateDecision.Skip ? DockerAvailability.SkipMessage : null;
        }
    }
}
