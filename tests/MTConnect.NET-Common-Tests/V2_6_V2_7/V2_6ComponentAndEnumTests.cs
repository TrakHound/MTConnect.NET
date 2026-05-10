using System;
using MTConnect.Devices.Components;
using MTConnect.Devices.Configurations;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.V2_6_V2_7
{
    // Pins the non-DataItem v2.6 surface: new Component subclasses + the
    // MediaType enum's QIF_MBD addition.
    //
    //   - XMI: mtconnect/mtconnect_sysml_model @ v2.6 (SHA 08185447bf86…)
    //          * UML class `CuttingTorch` — Component Types package
    //          * UML class `Electrode`    — Component Types package
    //          * Enum `MediaTypeEnum` value `QIF_MBD`
    //   - XSD: schemas.mtconnect.org/schemas/MTConnectDevices_2.6.xsd
    //          (Component element list + MediaType simpleType enumeration)
    //   - Prose: MTConnect Standard Part_3.0_Devices_v2.6
    //          §3.4.18 "CuttingTorch" / §3.4.21 "Electrode"
    //          §4.7.2.5 MediaType (introduces QIF_MBD)
    [TestFixture]
    public class V2_6ComponentAndEnumTests
    {
        // Source: XMI v2.6 UML `CuttingTorch` (Component Types); XSD v2.6
        // `<xs:element name="CuttingTorch">`.
        [Test]
        public void CuttingTorchComponent_constructs_with_correct_type()
        {
            var c = new CuttingTorchComponent();
            Assert.That(c.Type, Is.EqualTo("CuttingTorch"));
            Assert.That(c.Name, Is.EqualTo("cuttingTorch"));
            Assert.That(CuttingTorchComponent.TypeId, Is.EqualTo("CuttingTorch"));
        }

        // Source: XMI v2.6 UML `Electrode` (Component Types); XSD v2.6
        // `<xs:element name="Electrode">`.
        [Test]
        public void ElectrodeComponent_constructs_with_correct_type()
        {
            var c = new ElectrodeComponent();
            Assert.That(c.Type, Is.EqualTo("Electrode"));
            Assert.That(c.Name, Is.EqualTo("electrode"));
            Assert.That(ElectrodeComponent.TypeId, Is.EqualTo("Electrode"));
        }

        // Source: XMI v2.6 enum `MediaTypeEnum` member `QIF_MBD`. XSD v2.6 lists
        // QIF_MBD inside the MediaType simpleType enumeration. Prose
        // Part_3.0_Devices_v2.6 §4.7.2.5 introduces "ISO 10303 QIF model-based
        // design" as the rationale.
        [Test]
        public void MediaType_QIF_MBD_value_present_in_v2_6()
        {
            Assert.That(Enum.IsDefined(typeof(MediaType), "QIF_MBD"), Is.True);
        }
    }
}
