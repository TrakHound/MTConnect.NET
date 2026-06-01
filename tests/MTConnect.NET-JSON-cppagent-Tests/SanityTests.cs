using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests
{
    /// <summary>Pins the behaviour expressed by the test name: sanity tests.</summary>
    [TestFixture]
    public class SanityTests
    {
        /// <summary>Pins the behaviour expressed by the test name: project loads and references m t connect n e t j s o n cppagent.</summary>
        [Test]
        public void Project_loads_and_references_MTConnect_NET_JSON_cppagent()
        {
            var type = System.Type.GetType("MTConnect.JsonFunctions, MTConnect.NET-JSON-cppagent");
            Assert.That(type, Is.Not.Null, "JsonFunctions must resolve via the MTConnect.NET-JSON-cppagent project reference");
            Assert.That(type!.Assembly.GetName().Name, Is.EqualTo("MTConnect.NET-JSON-cppagent"));
        }
    }
}
