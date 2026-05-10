// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Xml;
using MTConnect.Tests.XML.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices.Configurations
{
    /// <summary>
    /// Pins the <c>&lt;Translation&gt;</c> vs <c>&lt;TranslationDataSet&gt;</c>
    /// polymorphism on <c>&lt;Transformation&gt;</c> in MTConnect v2.7's XML
    /// wire shape. TranslationDataSet uses XYZ keys per XYZDataSetType.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractTranslation</c>, <c>Translation</c>,
    /// <c>TranslationDataSet</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (elements <c>&lt;Translation&gt;</c>, <c>&lt;TranslationDataSet&gt;</c>
    /// in <c>TransformationType</c>).</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicTranslationRoundTripTests
    {
        // ---------------- positive: simple Translation ----------------

        [Test]
        public void Simple_Translation_serialises_to_text_element()
        {
            var t = new Transformation
            {
                Translation = new Translation { Value = "1 2 3" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlTransformation.WriteXml(w, t));

            Assert.That(xml, Does.Contain("<Translation>1 2 3</Translation>"));
            Assert.That(xml, Does.Not.Contain("<TranslationDataSet"));
        }

        [Test]
        public void Simple_Translation_deserialises_to_ITranslation()
        {
            const string xml = "<Transformation>"
                + "<Translation>1 2 3</Translation></Transformation>";

            var wire = XmlRoundTripHelper.Read<XmlTransformation>(xml);
            var t = wire.ToTransformation();

            Assert.That(t.Translation, Is.InstanceOf<ITranslation>());
            Assert.That(t.Translation, Is.Not.InstanceOf<ITranslationDataSet>());
            Assert.That(((ITranslation)t.Translation).Value, Is.EqualTo("1 2 3"));
        }

        // ---------------- positive: TranslationDataSet ----------------

        [Test]
        public void TranslationDataSet_serialises_to_keyed_entries()
        {
            var t = new Transformation
            {
                Translation = new TranslationDataSet { X = "1", Y = "2", Z = "3" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlTransformation.WriteXml(w, t));

            Assert.That(xml, Does.Contain("<TranslationDataSet>"));
            Assert.That(xml, Does.Contain("<Entry key=\"X\">1</Entry>"));
            Assert.That(xml, Does.Contain("<Entry key=\"Y\">2</Entry>"));
            Assert.That(xml, Does.Contain("<Entry key=\"Z\">3</Entry>"));
        }

        [Test]
        public void TranslationDataSet_deserialises_to_ITranslationDataSet()
        {
            const string xml = "<Transformation>"
                + "<TranslationDataSet>"
                + "<Entry key=\"X\">7</Entry>"
                + "<Entry key=\"Y\">8</Entry>"
                + "<Entry key=\"Z\">9</Entry>"
                + "</TranslationDataSet></Transformation>";

            var wire = XmlRoundTripHelper.Read<XmlTransformation>(xml);
            var t = wire.ToTransformation();

            Assert.That(t.Translation, Is.InstanceOf<ITranslationDataSet>());
            var ds = (ITranslationDataSet)t.Translation;
            Assert.That(ds.X, Is.EqualTo("7"));
            Assert.That(ds.Y, Is.EqualTo("8"));
            Assert.That(ds.Z, Is.EqualTo("9"));
        }

        // ---------------- negative ----------------

        [Test]
        public void TranslationDataSet_with_illegal_key_drops_value_without_corruption()
        {
            const string xml = "<TranslationDataSet>"
                + "<Entry key=\"W\">99</Entry>"
                + "<Entry key=\"X\">1</Entry>"
                + "</TranslationDataSet>";

            var wire = XmlRoundTripHelper.Read<XmlTranslationDataSet>(xml);
            var ds = wire.ToTranslationDataSet();

            Assert.That(ds.X, Is.EqualTo("1"));
            Assert.That(ds.Y, Is.Null);
            Assert.That(ds.Z, Is.Null);
        }

        [Test]
        public void Empty_Translation_yields_empty_string_value()
        {
            const string xml = "<Translation></Translation>";

            var wire = XmlRoundTripHelper.Read<XmlTranslation>(xml);
            var t = wire.ToTranslation();

            Assert.That(t.Value, Is.Null.Or.Empty);
        }

        [Test]
        public void Null_translation_property_emits_no_Translation_element()
        {
            var t = new Transformation
            {
                Translation = null,
                Rotation = new Rotation { Value = "1 2 3" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlTransformation.WriteXml(w, t));

            Assert.That(xml, Does.Not.Contain("<Translation"));
            Assert.That(xml, Does.Not.Contain("<TranslationDataSet"));
            Assert.That(xml, Does.Contain("<Rotation>1 2 3</Rotation>"));
        }

        [Test]
        public void Both_Translation_and_TranslationDataSet_present_DataSet_wins()
        {
            const string xml = "<Transformation>"
                + "<Translation>1 2 3</Translation>"
                + "<TranslationDataSet>"
                + "<Entry key=\"X\">9</Entry>"
                + "</TranslationDataSet></Transformation>";

            var wire = XmlRoundTripHelper.Read<XmlTransformation>(xml);
            var t = wire.ToTransformation();

            Assert.That(t.Translation, Is.InstanceOf<ITranslationDataSet>());
            Assert.That(((ITranslationDataSet)t.Translation).X, Is.EqualTo("9"));
        }

        [Test]
        public void Translation_writes_nothing_for_null_input()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlTranslation.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }

        [Test]
        public void TranslationDataSet_writes_nothing_for_null_input()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlTranslationDataSet.WriteXml(w, null));
            Assert.That(xml, Is.Empty);
        }

        [Test]
        public void TranslationDataSet_skips_null_entries_on_write()
        {
            var ds = new TranslationDataSet { X = "1", Y = null, Z = "" };

            var xml = XmlRoundTripHelper.Write(w => XmlTranslationDataSet.WriteXml(w, ds));

            Assert.That(xml, Does.Contain("<Entry key=\"X\">1</Entry>"));
            Assert.That(xml, Does.Not.Contain("key=\"Y\""));
            Assert.That(xml, Does.Not.Contain("key=\"Z\""));
        }

        [Test]
        public void Transformation_round_trip_preserves_both_Translation_and_Rotation()
        {
            // The Transformation container holds two independent xs:choice
            // slots. Pin that both can land DataSet variants in one
            // round-trip without leaking into each other.
            var input = new Transformation
            {
                Translation = new TranslationDataSet { X = "1", Y = "2", Z = "3" },
                Rotation = new RotationDataSet { A = "10", B = "20", C = "30" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlTransformation.WriteXml(w, input));
            var wire = XmlRoundTripHelper.Read<XmlTransformation>(xml);
            var output = wire.ToTransformation();

            Assert.That(output.Translation, Is.InstanceOf<ITranslationDataSet>());
            Assert.That(output.Rotation, Is.InstanceOf<IRotationDataSet>());
            Assert.That(((ITranslationDataSet)output.Translation).X, Is.EqualTo("1"));
            Assert.That(((IRotationDataSet)output.Rotation).A, Is.EqualTo("10"));
        }
    }
}
