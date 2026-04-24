using NUnit.Framework;

namespace MTConnect.NET_JSON_Tests
{
    [TestFixture]
    public class SanityTests
    {
        [Test]
        public void Project_loads_and_references_MTConnect_NET_JSON()
        {
            var type = System.Type.GetType("MTConnect.JsonFunctions, MTConnect.NET-JSON");
            Assert.That(type, Is.Not.Null, "JsonFunctions must resolve via the MTConnect.NET-JSON project reference");
            Assert.That(type!.Assembly.GetName().Name, Is.EqualTo("MTConnect.NET-JSON"));
        }
    }
}
