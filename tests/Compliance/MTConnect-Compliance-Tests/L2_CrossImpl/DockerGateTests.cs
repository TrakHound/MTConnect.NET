// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L2_CrossImpl
{
    /// <summary>
    /// Pins <see cref="DockerGate"/>: a RequiresDocker fixture runs when Docker is reachable, is
    /// skipped when it is not, and fails instead when <c>MTCONNECT_E2E_DOCKER</c> asks for the Docker
    /// suites (accepting the same truthy values as <c>tools/test.ps1</c>).
    /// </summary>
    [TestFixture]
    [Category("DockerGate")]
    public class DockerGateTests
    {
        [TestCase("true")]
        [TestCase("TRUE")]
        [TestCase("yes")]
        [TestCase("on")]
        [TestCase("1")]
        [TestCase(" true ")]
        public void Truthy_values_ask_for_the_Docker_suites(string value)
        {
            Assert.That(DockerGate.IsRequested(value), Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("false")]
        [TestCase("0")]
        [TestCase("no")]
        [TestCase("off")]
        [TestCase("enabled")]
        public void Other_values_do_not_ask_for_the_Docker_suites(string? value)
        {
            Assert.That(DockerGate.IsRequested(value), Is.False);
        }

        [TestCase(null)]
        [TestCase("true")]
        [TestCase("false")]
        public void Fixtures_run_when_Docker_is_reachable(string? value)
        {
            Assert.That(DockerGate.Decide(dockerReachable: true, value), Is.EqualTo(DockerGateDecision.Run));
        }

        [TestCase(null)]
        [TestCase("false")]
        [TestCase("0")]
        public void Fixtures_are_skipped_when_Docker_is_unreachable_and_not_asked_for(string? value)
        {
            Assert.That(DockerGate.Decide(dockerReachable: false, value), Is.EqualTo(DockerGateDecision.Skip));
        }

        [TestCase("true")]
        [TestCase("1")]
        public void Fixtures_fail_when_Docker_is_unreachable_but_asked_for(string value)
        {
            Assert.That(DockerGate.Decide(dockerReachable: false, value), Is.EqualTo(DockerGateDecision.Fail));
        }
    }
}
