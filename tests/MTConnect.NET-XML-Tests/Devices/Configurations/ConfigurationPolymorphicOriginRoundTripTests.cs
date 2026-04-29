// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Xml;
using MTConnect.Tests.XML.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices.Configurations
{
    /// <summary>
    /// Pins the <c>&lt;Origin&gt;</c> vs <c>&lt;OriginDataSet&gt;</c>
    /// polymorphism on both <c>&lt;Motion&gt;</c> and
    /// <c>&lt;CoordinateSystem&gt;</c> in MTConnect v2.7's XML wire shape.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractOrigin</c>, <c>Origin</c>, <c>OriginDataSet</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (elements <c>&lt;Origin&gt;</c>, <c>&lt;OriginDataSet&gt;</c> in
    /// <c>MotionType</c> and <c>CoordinateSystemType</c>).</item>
    /// <item>Prose — <see href="https://docs.mtconnect.org/"/> Part 2 (Devices)
    /// section on Configuration / CoordinateSystem.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicOriginRoundTripTests
    {
        // ---------------- positive: Motion + simple Origin ----------------

        [Test]
        public void Simple_Origin_serialises_to_text_element_on_Motion()
        {
            var motion = new Motion
            {
                Id = "m1",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Origin = new Origin { Value = "1 2 3" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlMotion.WriteXml(w, motion));

            Assert.That(xml, Does.Contain("<Origin>1 2 3</Origin>"));
            Assert.That(xml, Does.Not.Contain("<OriginDataSet"));
        }

        [Test]
        public void Simple_Origin_deserialises_to_IOrigin_on_Motion()
        {
            const string xml = "<Motion id=\"m1\" type=\"PRISMATIC\" actuation=\"DIRECT\">"
                + "<Origin>4 5 6</Origin></Motion>";

            var wire = XmlRoundTripHelper.Read<XmlMotion>(xml);
            var motion = wire.ToMotion();

            Assert.That(motion.Origin, Is.InstanceOf<IOrigin>());
            Assert.That(motion.Origin, Is.Not.InstanceOf<IOriginDataSet>());
            Assert.That(((IOrigin)motion.Origin).Value, Is.EqualTo("4 5 6"));
        }

        // ---------------- positive: Motion + OriginDataSet ----------------

        [Test]
        public void OriginDataSet_serialises_to_keyed_entries_on_Motion()
        {
            var motion = new Motion
            {
                Id = "m2",
                Type = MotionType.REVOLUTE,
                Actuation = MotionActuationType.DIRECT,
                Origin = new OriginDataSet { X = "1", Y = "2", Z = "3" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlMotion.WriteXml(w, motion));

            Assert.That(xml, Does.Contain("<OriginDataSet>"));
            Assert.That(xml, Does.Contain("<Entry key=\"X\">1</Entry>"));
            Assert.That(xml, Does.Contain("<Entry key=\"Y\">2</Entry>"));
            Assert.That(xml, Does.Contain("<Entry key=\"Z\">3</Entry>"));
        }

        [Test]
        public void OriginDataSet_deserialises_to_IOriginDataSet_on_Motion()
        {
            const string xml = "<Motion id=\"m2\" type=\"REVOLUTE\" actuation=\"DIRECT\">"
                + "<OriginDataSet>"
                + "<Entry key=\"X\">7</Entry>"
                + "<Entry key=\"Y\">8</Entry>"
                + "<Entry key=\"Z\">9</Entry>"
                + "</OriginDataSet></Motion>";

            var wire = XmlRoundTripHelper.Read<XmlMotion>(xml);
            var motion = wire.ToMotion();

            Assert.That(motion.Origin, Is.InstanceOf<IOriginDataSet>());
            var ds = (IOriginDataSet)motion.Origin;
            Assert.That(ds.X, Is.EqualTo("7"));
            Assert.That(ds.Y, Is.EqualTo("8"));
            Assert.That(ds.Z, Is.EqualTo("9"));
        }

        // ---------------- positive: CoordinateSystem + simple Origin ----------------

        [Test]
        public void Simple_Origin_serialises_to_text_element_on_CoordinateSystem()
        {
            var cs = new CoordinateSystem
            {
                Id = "cs1",
                Type = CoordinateSystemType.MACHINE,
                Origin = new Origin { Value = "10 20 30" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlCoordinateSystem.WriteXml(w, cs));

            Assert.That(xml, Does.Contain("<Origin>10 20 30</Origin>"));
            Assert.That(xml, Does.Not.Contain("<OriginDataSet"));
        }

        [Test]
        public void Simple_Origin_deserialises_to_IOrigin_on_CoordinateSystem()
        {
            const string xml = "<CoordinateSystem id=\"cs1\" type=\"MACHINE\">"
                + "<Origin>10 20 30</Origin></CoordinateSystem>";

            var wire = XmlRoundTripHelper.Read<XmlCoordinateSystem>(xml);
            var cs = wire.ToCoordinateSystem();

            Assert.That(cs.Origin, Is.InstanceOf<IOrigin>());
            Assert.That(((IOrigin)cs.Origin).Value, Is.EqualTo("10 20 30"));
        }

        // ---------------- positive: CoordinateSystem + OriginDataSet ----------------

        [Test]
        public void OriginDataSet_serialises_to_keyed_entries_on_CoordinateSystem()
        {
            var cs = new CoordinateSystem
            {
                Id = "cs2",
                Type = CoordinateSystemType.WORLD,
                Origin = new OriginDataSet { X = "100", Y = "200", Z = "300" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlCoordinateSystem.WriteXml(w, cs));

            Assert.That(xml, Does.Contain("<OriginDataSet>"));
            Assert.That(xml, Does.Contain("<Entry key=\"X\">100</Entry>"));
        }

        [Test]
        public void OriginDataSet_deserialises_to_IOriginDataSet_on_CoordinateSystem()
        {
            const string xml = "<CoordinateSystem id=\"cs2\" type=\"WORLD\">"
                + "<OriginDataSet>"
                + "<Entry key=\"X\">100</Entry>"
                + "<Entry key=\"Y\">200</Entry>"
                + "<Entry key=\"Z\">300</Entry>"
                + "</OriginDataSet></CoordinateSystem>";

            var wire = XmlRoundTripHelper.Read<XmlCoordinateSystem>(xml);
            var cs = wire.ToCoordinateSystem();

            Assert.That(cs.Origin, Is.InstanceOf<IOriginDataSet>());
            var ds = (IOriginDataSet)cs.Origin;
            Assert.That(ds.X, Is.EqualTo("100"));
            Assert.That(ds.Y, Is.EqualTo("200"));
            Assert.That(ds.Z, Is.EqualTo("300"));
        }

        // ---------------- negative ----------------

        [Test]
        public void OriginDataSet_with_illegal_key_drops_value_without_corruption()
        {
            const string xml = "<OriginDataSet>"
                + "<Entry key=\"W\">99</Entry>"
                + "<Entry key=\"X\">1</Entry>"
                + "</OriginDataSet>";

            var wire = XmlRoundTripHelper.Read<XmlOriginDataSet>(xml);
            var ds = wire.ToOriginDataSet();

            Assert.That(ds.X, Is.EqualTo("1"));
            Assert.That(ds.Y, Is.Null);
            Assert.That(ds.Z, Is.Null);
        }

        [Test]
        public void Empty_Origin_yields_empty_string_value()
        {
            const string xml = "<Origin></Origin>";

            var wire = XmlRoundTripHelper.Read<XmlOrigin>(xml);
            var origin = wire.ToOrigin();

            Assert.That(origin.Value, Is.Null.Or.Empty);
        }

        [Test]
        public void Null_origin_property_emits_no_Origin_element_on_Motion()
        {
            var motion = new Motion
            {
                Id = "m3",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Origin = null
            };

            var xml = XmlRoundTripHelper.Write(w => XmlMotion.WriteXml(w, motion));

            Assert.That(xml, Does.Not.Contain("<Origin"));
            Assert.That(xml, Does.Not.Contain("<OriginDataSet"));
        }

        [Test]
        public void Null_origin_property_emits_no_Origin_element_on_CoordinateSystem()
        {
            var cs = new CoordinateSystem
            {
                Id = "cs3",
                Type = CoordinateSystemType.MACHINE,
                Origin = null
            };

            var xml = XmlRoundTripHelper.Write(w => XmlCoordinateSystem.WriteXml(w, cs));

            Assert.That(xml, Does.Not.Contain("<Origin"));
            Assert.That(xml, Does.Not.Contain("<OriginDataSet"));
        }

        [Test]
        public void Both_Origin_and_OriginDataSet_present_DataSet_wins()
        {
            // The spec's xs:choice rejects both being present; the deserialiser
            // accepts loosely (XmlSerializer fills both). Pin that
            // ToCoordinateSystem prefers the DataSet form.
            const string xml = "<CoordinateSystem id=\"cs4\" type=\"MACHINE\">"
                + "<Origin>1 2 3</Origin>"
                + "<OriginDataSet>"
                + "<Entry key=\"X\">9</Entry>"
                + "</OriginDataSet></CoordinateSystem>";

            var wire = XmlRoundTripHelper.Read<XmlCoordinateSystem>(xml);
            var cs = wire.ToCoordinateSystem();

            Assert.That(cs.Origin, Is.InstanceOf<IOriginDataSet>());
            Assert.That(((IOriginDataSet)cs.Origin).X, Is.EqualTo("9"));
        }

        [Test]
        public void Null_coordinateSystem_writes_nothing()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlCoordinateSystem.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }

        [Test]
        public void OriginDataSet_writes_nothing_for_null_input()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlOriginDataSet.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }

        [Test]
        public void Origin_writes_nothing_for_null_input()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlOrigin.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }
    }
}
