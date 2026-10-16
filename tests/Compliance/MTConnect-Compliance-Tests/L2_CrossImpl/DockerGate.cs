// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;
using MTConnect.Tests.Shared;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L2_CrossImpl
{
    /// <summary>
    /// NUnit gate for the Testcontainers-backed fixtures, on <see cref="DockerAvailability"/>. Called
    /// first in their <c>[OneTimeSetUp]</c>, it skips the fixture (<see cref="Assert.Ignore(string)"/>)
    /// when Docker is not reachable, and fails it instead when <c>MTCONNECT_E2E_DOCKER</c> asks for the
    /// Docker suites.
    /// </summary>
    internal static class DockerGate
    {
        /// <summary>
        /// Skips (or, when the Docker suites were asked for, fails) the calling fixture when Docker is not
        /// reachable; returns normally when it is.
        /// </summary>
        public static async Task RequireDockerAsync()
        {
            // The probe blocks (once per run, cached), so it runs off the fixture's calling context
            var reachable = await Task.Run(() => DockerAvailability.IsReachable).ConfigureAwait(false);
            var environmentValue = DockerAvailability.EnvironmentValue;

            switch (DockerAvailability.Decide(reachable, environmentValue))
            {
                case DockerGateDecision.Skip:
                    Assert.Ignore(DockerAvailability.SkipMessage);
                    break;

                case DockerGateDecision.Fail:
                    Assert.Fail(DockerAvailability.FailMessage(environmentValue));
                    break;
            }
        }
    }
}
