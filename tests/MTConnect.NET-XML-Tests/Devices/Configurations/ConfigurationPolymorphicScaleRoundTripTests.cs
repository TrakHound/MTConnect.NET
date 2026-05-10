// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Xml;
using MTConnect.Tests.XML.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices.Configurations
{
    /// <summary>
    /// Pins the <c>&lt;Scale&gt;</c> vs <c>&lt;ScaleDataSet&gt;</c>
    /// polymorphism on <c>&lt;SolidModel&gt;</c> in MTConnect v2.7's XML
    /// wire shape. ScaleDataSet uses XYZ keys per XYZDataSetType.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractScale</c>, <c>Scale</c>, <c>ScaleDataSet</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (elements <c>&lt;Scale&gt;</c>, <c>&lt;ScaleDataSet&gt;</c> in
    /// <c>SolidModelType</c>).</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicScaleRoundTripTests
    {
        // ---------------- positive: simple Scale ----------------

        [Test]
        public void Simple_Scale_serialises_to_text_element()
        {
            var sm = new SolidModel
            {
                Id = "sm1",
                MediaType = MediaType.STL,
                Scale = new Scale { Value = "1 1 1" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlSolidModel.WriteXml(w, sm));

            Assert.That(xml, Does.Contain("<Scale>1 1 1</Scale>"));
            Assert.That(xml, Does.Not.Contain("<ScaleDataSet"));
        }

        [Test]
        public void Simple_Scale_deserialises_to_IScale()
        {
            const string xml = "<SolidModel id=\"sm1\" mediaType=\"STL\">"
                + "<Scale>2 2 2</Scale></SolidModel>";

            var wire = XmlRoundTripHelper.Read<XmlSolidModel>(xml);
            var sm = wire.ToSolidModel();

            Assert.That(sm.Scale, Is.InstanceOf<IScale>());
            Assert.That(sm.Scale, Is.Not.InstanceOf<IScaleDataSet>());
            Assert.That(((IScale)sm.Scale).Value, Is.EqualTo("2 2 2"));
        }

        // ---------------- positive: ScaleDataSet ----------------

        [Test]
        public void ScaleDataSet_serialises_to_keyed_entries()
        {
            var sm = new SolidModel
            {
                Id = "sm2",
                MediaType = MediaType.OBJ,
                Scale = new ScaleDataSet { X = 1.5, Y = 2.5, Z = 3.5 }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlSolidModel.WriteXml(w, sm));

            Assert.That(xml, Does.Contain("<ScaleDataSet>"));
            Assert.That(xml, Does.Contain("<Entry key=\"X\">1.5</Entry>"));
            Assert.That(xml, Does.Contain("<Entry key=\"Y\">2.5</Entry>"));
            Assert.That(xml, Does.Contain("<Entry key=\"Z\">3.5</Entry>"));
        }

        [Test]
        public void ScaleDataSet_deserialises_to_IScaleDataSet()
        {
            const string xml = "<SolidModel id=\"sm2\" mediaType=\"STL\">"
                + "<ScaleDataSet>"
                + "<Entry key=\"X\">1.5</Entry>"
                + "<Entry key=\"Y\">2.5</Entry>"
                + "<Entry key=\"Z\">3.5</Entry>"
                + "</ScaleDataSet></SolidModel>";

            var wire = XmlRoundTripHelper.Read<XmlSolidModel>(xml);
            var sm = wire.ToSolidModel();

            Assert.That(sm.Scale, Is.InstanceOf<IScaleDataSet>());
            var ds = (IScaleDataSet)sm.Scale;
            Assert.That(ds.X, Is.EqualTo(1.5));
            Assert.That(ds.Y, Is.EqualTo(2.5));
            Assert.That(ds.Z, Is.EqualTo(3.5));
        }

        // ---------------- negative ----------------

        [Test]
        public void ScaleDataSet_with_illegal_key_drops_value_without_corruption()
        {
            const string xml = "<ScaleDataSet>"
                + "<Entry key=\"W\">99.0</Entry>"
                + "<Entry key=\"X\">1.0</Entry>"
                + "</ScaleDataSet>";

            var wire = XmlRoundTripHelper.Read<XmlScaleDataSet>(xml);
            var ds = wire.ToScaleDataSet();

            Assert.That(ds.X, Is.EqualTo(1.0));
            Assert.That(ds.Y, Is.EqualTo(0.0));
            Assert.That(ds.Z, Is.EqualTo(0.0));
        }

        [Test]
        public void ScaleDataSet_with_unparseable_value_falls_back_to_zero()
        {
            const string xml = "<ScaleDataSet>"
                + "<Entry key=\"X\">not-a-number</Entry>"
                + "</ScaleDataSet>";

            var wire = XmlRoundTripHelper.Read<XmlScaleDataSet>(xml);
            var ds = wire.ToScaleDataSet();

            Assert.That(ds.X, Is.EqualTo(0.0));
        }

        [Test]
        public void ScaleDataSet_with_empty_value_text_falls_back_to_zero()
        {
            const string xml = "<ScaleDataSet>"
                + "<Entry key=\"X\"></Entry>"
                + "</ScaleDataSet>";

            var wire = XmlRoundTripHelper.Read<XmlScaleDataSet>(xml);
            var ds = wire.ToScaleDataSet();

            Assert.That(ds.X, Is.EqualTo(0.0));
        }

        [Test]
        public void Empty_Scale_yields_empty_string_value()
        {
            const string xml = "<Scale></Scale>";

            var wire = XmlRoundTripHelper.Read<XmlScale>(xml);
            var s = wire.ToScale();

            Assert.That(s.Value, Is.Null.Or.Empty);
        }

        [Test]
        public void Null_scale_property_emits_no_Scale_element()
        {
            var sm = new SolidModel
            {
                Id = "sm3",
                MediaType = MediaType.STL,
                Scale = null
            };

            var xml = XmlRoundTripHelper.Write(w => XmlSolidModel.WriteXml(w, sm));

            Assert.That(xml, Does.Not.Contain("<Scale"));
            Assert.That(xml, Does.Not.Contain("<ScaleDataSet"));
        }

        [Test]
        public void Both_Scale_and_ScaleDataSet_present_DataSet_wins()
        {
            const string xml = "<SolidModel id=\"sm\" mediaType=\"STL\">"
                + "<Scale>1 1 1</Scale>"
                + "<ScaleDataSet>"
                + "<Entry key=\"X\">9</Entry>"
                + "</ScaleDataSet></SolidModel>";

            var wire = XmlRoundTripHelper.Read<XmlSolidModel>(xml);
            var sm = wire.ToSolidModel();

            Assert.That(sm.Scale, Is.InstanceOf<IScaleDataSet>());
            Assert.That(((IScaleDataSet)sm.Scale).X, Is.EqualTo(9.0));
        }

        [Test]
        public void Null_solidModel_writes_nothing()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlSolidModel.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }

        [Test]
        public void Scale_writes_nothing_for_null_input()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlScale.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }

        [Test]
        public void ScaleDataSet_writes_nothing_for_null_input()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlScaleDataSet.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }

        [Test]
        public void ScaleDataSet_with_null_entries_list_yields_zero_components()
        {
            const string xml = "<ScaleDataSet/>";

            var wire = XmlRoundTripHelper.Read<XmlScaleDataSet>(xml);
            var ds = wire.ToScaleDataSet();

            Assert.That(ds.X, Is.EqualTo(0.0));
            Assert.That(ds.Y, Is.EqualTo(0.0));
            Assert.That(ds.Z, Is.EqualTo(0.0));
        }
    }
}
