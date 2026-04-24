using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L2_XmiOclAssertions
{
    // Layer 2 — XMI / OCL assertions. Version-specific OCL rules from the
    // mtconnect_sysml_model repo run against library-emitted envelopes.
    // Divergent rules across versions live as per-version OCL files.
    [TestFixture]
    public class OclSentinelTests
    {
        [Test]
        public void Compliance_harness_L2_project_loads()
        {
            Assert.That(typeof(OclSentinelTests).Namespace, Is.EqualTo("MTConnect.Compliance.Tests.L2_XmiOclAssertions"));
        }
    }
}
