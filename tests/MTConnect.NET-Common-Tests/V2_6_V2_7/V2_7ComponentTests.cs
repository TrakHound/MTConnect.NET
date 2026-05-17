using MTConnect.Devices.Components;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.V2_6_V2_7
{
    // Pins the v2.7 Component subclasses (PinTool, ToolHolder).
    //
    //   - XMI: https://github.com/mtconnect/mtconnect_sysml_model @ tag v2.7
    //          UML classes under Device Information Model > Components:
    //            * PinTool      — pin-style tooling component
    //            * ToolHolder   — tool-holder component
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
    //          (each TypeId appears in the ComponentType enumeration).
    //   - Prose: MTConnect Standard Part_2.0_Devices_v2.7 section 7 "Component
    //          types" — describes intended use of each Component subclass.
    [TestFixture]
    public class V2_7ComponentTests
    {
        [Test]
        public void PinToolComponent_constructs_with_correct_type()
        {
            var c = new PinToolComponent();
            Assert.That(c.Type, Is.EqualTo("PinTool"));
            Assert.That(c.Name, Is.EqualTo("pinTool"));
            Assert.That(PinToolComponent.TypeId, Is.EqualTo("PinTool"));
        }

        [Test]
        public void ToolHolderComponent_constructs_with_correct_type()
        {
            var c = new ToolHolderComponent();
            Assert.That(c.Type, Is.EqualTo("ToolHolder"));
            Assert.That(c.Name, Is.EqualTo("toolHolder"));
            Assert.That(ToolHolderComponent.TypeId, Is.EqualTo("ToolHolder"));
        }
    }
}
