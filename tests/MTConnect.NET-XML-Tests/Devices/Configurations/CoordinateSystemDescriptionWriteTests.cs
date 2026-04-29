// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Xml;
using MTConnect.Tests.XML.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices.Configurations
{
    /// <summary>
    /// Pins the optional-attribute and optional-child-element write paths on
    /// <c>XmlCoordinateSystem.WriteXml(...)</c> and the <c>Transformation</c>
    /// branch on <c>ToCoordinateSystem()</c>. Each of <c>name</c>,
    /// <c>nativeName</c>, <c>parentIdRef</c>, <c>&lt;Description&gt;</c>,
    /// and <c>&lt;Transformation&gt;</c> is optional in the XSD
    /// (attributes <c>use='optional'</c>; the child element
    /// <c>minOccurs='0'</c>). Non-empty values emit the surface; null / empty
    /// values suppress it.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML class <c>CoordinateSystem</c>,
    /// UML ID <c>_19_0_3_45f01b9_1579100679936_1279_16310</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (<c>CoordinateSystemType</c> attribute group + child element
    /// <c>&lt;Description&gt;</c> with <c>minOccurs='0'</c>; child element
    /// <c>&lt;Transformation&gt;</c> with <c>minOccurs='0'</c>).</item>
    /// <item>Prose — <see href="https://docs.mtconnect.org/"/> Part 2 (Devices)
    /// section on Configuration / CoordinateSystem.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class CoordinateSystemDescriptionWriteTests
    {
        // ---------------- Description child element ----------------

        [Test]
        public void NonEmpty_Description_emits_Description_element()
        {
            var cs = new CoordinateSystem
            {
                Id = "csd1",
                Type = CoordinateSystemType.MACHINE,
                Description = "primary machine frame"
            };

            var xml = XmlRoundTripHelper.Write(w => XmlCoordinateSystem.WriteXml(w, cs));

            Assert.That(xml, Does.Contain("<Description>primary machine frame</Description>"));
        }

        [Test]
        public void Null_Description_emits_no_Description_element()
        {
            var cs = new CoordinateSystem
            {
                Id = "csd2",
                Type = CoordinateSystemType.MACHINE,
                Description = null
            };

            var xml = XmlRoundTripHelper.Write(w => XmlCoordinateSystem.WriteXml(w, cs));

            Assert.That(xml, Does.Not.Contain("<Description"));
        }

        [Test]
        public void Empty_Description_emits_no_Description_element()
        {
            var cs = new CoordinateSystem
            {
                Id = "csd3",
                Type = CoordinateSystemType.MACHINE,
                Description = string.Empty
            };

            var xml = XmlRoundTripHelper.Write(w => XmlCoordinateSystem.WriteXml(w, cs));

            Assert.That(xml, Does.Not.Contain("<Description"));
        }

        // ---------------- Optional name / nativeName / parentIdRef attributes ----------------

        [Test]
        public void Optional_attributes_emit_when_set()
        {
            var cs = new CoordinateSystem
            {
                Id = "csa1",
                Type = CoordinateSystemType.WORLD,
                Name = "wcs",
                NativeName = "wcs-native",
                ParentIdRef = "csa0"
            };

            var xml = XmlRoundTripHelper.Write(w => XmlCoordinateSystem.WriteXml(w, cs));

            Assert.That(xml, Does.Contain("name=\"wcs\""));
            Assert.That(xml, Does.Contain("nativeName=\"wcs-native\""));
            // Note: production code emits the (xsd-misnamed) attribute "parentIdRefd"
            // — pin the actual emitted spelling so the cell-under-test is exercised.
            Assert.That(xml, Does.Contain("parentIdRefd=\"csa0\""));
        }

        // ---------------- Transformation child element ----------------

        [Test]
        public void NonNull_Transformation_emits_Transformation_element_on_write()
        {
            var cs = new CoordinateSystem
            {
                Id = "cst1",
                Type = CoordinateSystemType.MACHINE,
                Transformation = new Transformation
                {
                    Translation = new Translation { Value = "1 2 3" }
                }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlCoordinateSystem.WriteXml(w, cs));

            Assert.That(xml, Does.Contain("<Transformation>"));
            Assert.That(xml, Does.Contain("<Translation>1 2 3</Translation>"));
        }

        [Test]
        public void Transformation_round_trips_through_ToCoordinateSystem()
        {
            const string xml = "<CoordinateSystem id=\"cst2\" type=\"WORLD\">"
                + "<Transformation>"
                + "<Translation>4 5 6</Translation>"
                + "</Transformation></CoordinateSystem>";

            var wire = XmlRoundTripHelper.Read<XmlCoordinateSystem>(xml);
            var cs = wire.ToCoordinateSystem();

            Assert.That(cs.Transformation, Is.Not.Null);
            Assert.That(cs.Transformation.Translation, Is.InstanceOf<ITranslation>());
            Assert.That(((ITranslation)cs.Transformation.Translation!).Value, Is.EqualTo("4 5 6"));
        }
    }
}
