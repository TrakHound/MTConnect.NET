using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L4_CrossImpl
{
    // Layer 4 — cross-implementation parity against the reference
    // cppagent. Docker-gated via [Category("RequiresDocker")] +
    // MTCONNECT_E2E_DOCKER=true. Non-whitelisted divergence fails the
    // job; whitelist entries in Fixtures/cross-impl-whitelist.json
    // each carry a sunset date.
    [TestFixture]
    public class CppAgentParitySentinelTests
    {
        [Test]
        public void Compliance_harness_L4_project_loads()
        {
            Assert.That(typeof(CppAgentParitySentinelTests).Namespace, Is.EqualTo("MTConnect.Compliance.Tests.L4_CrossImpl"));
        }
    }
}
