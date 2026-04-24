using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L1_XsdValidation
{
    // Layer 1 — XSD validation. Every MT.NET-emitted envelope validates
    // against the matching-version XSD. Matrix = envelope-kind × version
    // × printer-variant. Rows are added per-version by the plan that
    // adds that version's support (see docs/testing/v2-6.md + v2-7.md).
    [TestFixture]
    public class XsdSentinelTests
    {
        [Test]
        public void Compliance_harness_L1_project_loads()
        {
            Assert.That(typeof(XsdSentinelTests).Namespace, Is.EqualTo("MTConnect.Compliance.Tests.L1_XsdValidation"));
        }
    }
}
