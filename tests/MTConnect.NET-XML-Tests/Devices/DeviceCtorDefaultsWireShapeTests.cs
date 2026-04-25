// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text;
using MTConnect.Devices;
using MTConnect.Devices.Xml;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices
{
    /// <summary>
    /// Wire-shape smoke for the constructor-defaults removal contract:
    /// after the campaign, a freshly-constructed <see cref="Device"/>
    /// with no explicit fields set produces XML with empty <c>id</c>,
    /// <c>name</c>, and <c>uuid</c> attribute values rather than
    /// placeholder strings.
    ///
    /// This is the closest available approximation to the plan's
    /// HTTP+MQTT Docker E2E. The full agent E2E surface (probe
    /// emission against an XSD validator across an agent restart) is
    /// not yet plumbed on <c>upstream/master</c>; once the bootstrap
    /// prelude lands the higher-fidelity scenarios can be added.
    /// </summary>
    [TestFixture]
    public class DeviceCtorDefaultsWireShapeTests
    {
        [Test]
        public void Default_constructed_Device_emits_empty_identity_attributes()
        {
            var device = new Device();
            // The campaign deliberately leaves identity unset; serialize
            // straight to XML to confirm the wire shape carries empty
            // attribute values rather than placeholder strings.
            var xml = Encoding.UTF8.GetString(XmlDevice.ToXml(device, indent: false));

            Assert.That(xml, Does.Contain("id=\"\""), "Expected empty id attribute on default-constructed Device.");
            Assert.That(xml, Does.Contain("name=\"\""), "Expected empty name attribute on default-constructed Device.");
            Assert.That(xml, Does.Contain("uuid=\"\""), "Expected empty uuid attribute on default-constructed Device.");
            Assert.That(xml, Does.Not.Contain("name=\"dev\""),
                "Default ctor must not emit the placeholder name 'dev'.");
        }

        [Test]
        public void Caller_set_identity_round_trips_through_XML()
        {
            var device = new Device
            {
                Id = "device-1",
                Name = "spindle-A",
                Uuid = "uuid-A"
            };

            var xml = Encoding.UTF8.GetString(XmlDevice.ToXml(device, indent: false));

            Assert.That(xml, Does.Contain("id=\"device-1\""));
            Assert.That(xml, Does.Contain("name=\"spindle-A\""));
            Assert.That(xml, Does.Contain("uuid=\"uuid-A\""));
        }

        [Test]
        public void Sequential_default_Devices_emit_identical_empty_uuid_attributes()
        {
            // The original #136 symptom: GUID drift across constructions.
            // Pinned at the wire-shape layer in case a future change
            // masks it inside the POCO accessors.
            var first = Encoding.UTF8.GetString(XmlDevice.ToXml(new Device(), indent: false));
            var second = Encoding.UTF8.GetString(XmlDevice.ToXml(new Device(), indent: false));

            Assert.That(first, Does.Contain("uuid=\"\""));
            Assert.That(second, Does.Contain("uuid=\"\""));
        }
    }
}
