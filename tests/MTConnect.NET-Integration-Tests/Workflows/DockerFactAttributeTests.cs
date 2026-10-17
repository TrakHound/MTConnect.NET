// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Tests.Shared;
using Xunit;

namespace MTConnect.Tests.Integration.Workflows
{
    /// <summary>
    /// Pins <see cref="DockerFactAttribute"/>: a Docker-backed test is skipped only when Docker is
    /// unreachable and the Docker suites were not asked for; it runs when Docker is reachable, and
    /// when <c>MTCONNECT_E2E_DOCKER</c> asks for the suites (so an unreachable Docker fails it).
    /// </summary>
    public class DockerFactAttributeTests
    {
        /// <summary>
        /// Pins the behaviour expressed by the test name: skips only when docker is unreachable and not asked for.
        /// </summary>
        [Fact]
        public void Skips_only_when_Docker_is_unreachable_and_not_asked_for()
        {
            Assert.Equal(DockerAvailability.SkipMessage, DockerFactAttribute.GetSkipReason(DockerGateDecision.Skip));
            Assert.Null(DockerFactAttribute.GetSkipReason(DockerGateDecision.Run));
            Assert.Null(DockerFactAttribute.GetSkipReason(DockerGateDecision.Fail));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: decision follows reachability and the environment.
        /// </summary>
        [Theory]
        [InlineData(true, null, "Run")]
        [InlineData(true, "true", "Run")]
        [InlineData(false, null, "Skip")]
        [InlineData(false, "false", "Skip")]
        [InlineData(false, "true", "Fail")]
        [InlineData(false, "YES", "Fail")]
        [InlineData(false, "1", "Fail")]
        public void Decision_follows_reachability_and_the_environment(bool reachable, string? environmentValue, string expected)
        {
            Assert.Equal(expected, DockerAvailability.Decide(reachable, environmentValue).ToString());
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: attribute matches the current environment.
        /// </summary>
        [Fact]
        public void Attribute_matches_the_current_environment()
        {
            var attribute = new DockerFactAttribute();

            Assert.Equal(DockerFactAttribute.GetSkipReason(DockerAvailability.Current), attribute.Skip);
        }
    }
}
