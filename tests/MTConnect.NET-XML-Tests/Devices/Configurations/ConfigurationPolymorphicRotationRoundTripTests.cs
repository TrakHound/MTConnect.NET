// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Xml;
using MTConnect.Tests.XML.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices.Configurations
{
    /// <summary>
    /// Pins the <c>&lt;Rotation&gt;</c> vs <c>&lt;RotationDataSet&gt;</c>
    /// polymorphism on <c>&lt;Transformation&gt;</c> in MTConnect v2.7's
    /// XML wire shape. RotationDataSet uses ABC keys (not XYZ) per the
    /// ABCDataSetType definition.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractRotation</c>, <c>Rotation</c>,
    /// <c>RotationDataSet</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (elements <c>&lt;Rotation&gt;</c>, <c>&lt;RotationDataSet&gt;</c> in
    /// <c>TransformationType</c>; <c>ABCDataSetType</c> with KeyType A|B|C).</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicRotationRoundTripTests
    {
        // ---------------- positive: simple Rotation ----------------

        [Test]
        public void Simple_Rotation_serialises_to_text_element()
        {
            var transformation = new Transformation
            {
                Rotation = new Rotation { Value = "10 20 30" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlTransformation.WriteXml(w, transformation));

            Assert.That(xml, Does.Contain("<Rotation>10 20 30</Rotation>"));
            Assert.That(xml, Does.Not.Contain("<RotationDataSet"));
        }

        [Test]
        public void Simple_Rotation_deserialises_to_IRotation()
        {
            const string xml = "<Transformation>"
                + "<Rotation>10 20 30</Rotation></Transformation>";

            var wire = XmlRoundTripHelper.Read<XmlTransformation>(xml);
            var t = wire.ToTransformation();

            Assert.That(t.Rotation, Is.InstanceOf<IRotation>());
            Assert.That(t.Rotation, Is.Not.InstanceOf<IRotationDataSet>());
            Assert.That(((IRotation)t.Rotation).Value, Is.EqualTo("10 20 30"));
        }

        // ---------------- positive: RotationDataSet ----------------

        [Test]
        public void RotationDataSet_serialises_to_keyed_entries()
        {
            var transformation = new Transformation
            {
                Rotation = new RotationDataSet { A = "10", B = "20", C = "30" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlTransformation.WriteXml(w, transformation));

            Assert.That(xml, Does.Contain("<RotationDataSet>"));
            Assert.That(xml, Does.Contain("<Entry key=\"A\">10</Entry>"));
            Assert.That(xml, Does.Contain("<Entry key=\"B\">20</Entry>"));
            Assert.That(xml, Does.Contain("<Entry key=\"C\">30</Entry>"));
        }

        [Test]
        public void RotationDataSet_deserialises_to_IRotationDataSet()
        {
            const string xml = "<Transformation>"
                + "<RotationDataSet>"
                + "<Entry key=\"A\">10</Entry>"
                + "<Entry key=\"B\">20</Entry>"
                + "<Entry key=\"C\">30</Entry>"
                + "</RotationDataSet></Transformation>";

            var wire = XmlRoundTripHelper.Read<XmlTransformation>(xml);
            var t = wire.ToTransformation();

            Assert.That(t.Rotation, Is.InstanceOf<IRotationDataSet>());
            var ds = (IRotationDataSet)t.Rotation;
            Assert.That(ds.A, Is.EqualTo("10"));
            Assert.That(ds.B, Is.EqualTo("20"));
            Assert.That(ds.C, Is.EqualTo("30"));
        }

        // ---------------- negative ----------------

        [Test]
        public void RotationDataSet_with_X_key_drops_value_without_corruption()
        {
            // RotationDataSet keys are A|B|C — X is wrong-namespace for an
            // ABCDataSetType.
            const string xml = "<RotationDataSet>"
                + "<Entry key=\"X\">99</Entry>"
                + "<Entry key=\"A\">1</Entry>"
                + "</RotationDataSet>";

            var wire = XmlRoundTripHelper.Read<XmlRotationDataSet>(xml);
            var ds = wire.ToRotationDataSet();

            Assert.That(ds.A, Is.EqualTo("1"));
            Assert.That(ds.B, Is.Null);
            Assert.That(ds.C, Is.Null);
        }

        [Test]
        public void Empty_Rotation_yields_empty_string_value()
        {
            const string xml = "<Rotation></Rotation>";

            var wire = XmlRoundTripHelper.Read<XmlRotation>(xml);
            var r = wire.ToRotation();

            Assert.That(r.Value, Is.Null.Or.Empty);
        }

        [Test]
        public void Null_rotation_property_emits_no_Rotation_element()
        {
            var transformation = new Transformation
            {
                Rotation = null,
                Translation = new Translation { Value = "1 2 3" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlTransformation.WriteXml(w, transformation));

            Assert.That(xml, Does.Not.Contain("<Rotation"));
            Assert.That(xml, Does.Not.Contain("<RotationDataSet"));
            Assert.That(xml, Does.Contain("<Translation>1 2 3</Translation>"));
        }

        [Test]
        public void Both_Rotation_and_RotationDataSet_present_DataSet_wins()
        {
            // The spec's xs:choice rejects both; pin that ToTransformation
            // narrows DataSet over the simple form.
            const string xml = "<Transformation>"
                + "<Rotation>1 2 3</Rotation>"
                + "<RotationDataSet>"
                + "<Entry key=\"A\">9</Entry>"
                + "</RotationDataSet></Transformation>";

            var wire = XmlRoundTripHelper.Read<XmlTransformation>(xml);
            var t = wire.ToTransformation();

            Assert.That(t.Rotation, Is.InstanceOf<IRotationDataSet>());
            Assert.That(((IRotationDataSet)t.Rotation).A, Is.EqualTo("9"));
        }

        [Test]
        public void Null_transformation_writes_nothing()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlTransformation.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }

        [Test]
        public void Rotation_writes_nothing_for_null_input()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlRotation.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }

        [Test]
        public void RotationDataSet_writes_nothing_for_null_input()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlRotationDataSet.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }

        [Test]
        public void RotationDataSet_skips_null_or_empty_values_on_write()
        {
            // ABC keys with null/empty values are omitted from the XML.
            var ds = new RotationDataSet { A = "", B = null, C = "30" };

            var xml = XmlRoundTripHelper.Write(w => XmlRotationDataSet.WriteXml(w, ds));

            Assert.That(xml, Does.Not.Contain("key=\"A\""));
            Assert.That(xml, Does.Not.Contain("key=\"B\""));
            Assert.That(xml, Does.Contain("<Entry key=\"C\">30</Entry>"));
        }
    }
}
