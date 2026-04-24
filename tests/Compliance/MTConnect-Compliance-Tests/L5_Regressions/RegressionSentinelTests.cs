using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L5_Regressions
{
    // Layer 5 — per-version + per-issue regression pins. Once a
    // version or bug fix lands, a test here locks its presence so
    // a later refactor cannot silently drop it.
    [TestFixture]
    public class RegressionSentinelTests
    {
        [Test]
        public void Compliance_harness_L5_project_loads()
        {
            Assert.That(typeof(RegressionSentinelTests).Namespace, Is.EqualTo("MTConnect.Compliance.Tests.L5_Regressions"));
        }
    }
}
