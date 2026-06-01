using NUnit.Framework;

namespace MTConnect.NET_JSON_Tests
{
    /// <summary>Pins the behaviour expressed by the test name: sanity tests.</summary>
    [TestFixture]
    public class SanityTests
    {
        /// <summary>Pins the behaviour expressed by the test name: project loads and references m t connect n e t j s o n.</summary>
        [Test]
        public void Project_loads_and_references_MTConnect_NET_JSON()
        {
            var type = System.Type.GetType("MTConnect.JsonFunctions, MTConnect.NET-JSON");
            Assert.That(type, Is.Not.Null, "JsonFunctions must resolve via the MTConnect.NET-JSON project reference");
            Assert.That(type!.Assembly.GetName().Name, Is.EqualTo("MTConnect.NET-JSON"));
        }
    }
}
