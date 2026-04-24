using NUnit.Framework;

namespace MTConnect.NET_AgentModule_MqttRelay_Tests
{
    [TestFixture]
    public class SanityTests
    {
        [Test]
        public void Project_loads_and_references_MTConnect_NET_AgentModule_MqttRelay()
        {
            var moduleType = System.Type.GetType("MTConnect.Module, MTConnect.NET-AgentModule-MqttRelay");
            Assert.That(moduleType, Is.Not.Null, "MqttRelay Module type must resolve via the project reference");
            Assert.That(moduleType!.Assembly.GetName().Name, Is.EqualTo("MTConnect.NET-AgentModule-MqttRelay"));
        }
    }
}
