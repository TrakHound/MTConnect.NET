// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the Translation polymorphism on <c>Transformation</c> in the
    /// standard JSON wire format.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractTranslation</c>, <c>Translation</c>,
    /// <c>TranslationDataSet</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (the JSON shape mirrors XSD <c>TransformationType</c>'s Translation
    /// choice).</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicTranslationRoundTripTests
    {
        // ---------------- positive: simple Translation ----------------

        [Test]
        public void Simple_Translation_serialises_to_value_field()
        {
            var t = new Transformation
            {
                Translation = new Translation { Value = "1 2 3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonTransformation(t));

            Assert.That(json, Does.Contain("\"translation\":{\"value\":\"1 2 3\"}"));
            Assert.That(json, Does.Not.Contain("translationDataSet"));
        }

        [Test]
        public void Simple_Translation_deserialises_to_ITranslation()
        {
            const string json = "{\"translation\":{\"value\":\"1 2 3\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var t = wire.ToTransformation();

            Assert.That(t.Translation, Is.InstanceOf<ITranslation>());
            Assert.That(((ITranslation)t.Translation).Value, Is.EqualTo("1 2 3"));
        }

        // ---------------- positive: TranslationDataSet ----------------

        [Test]
        public void TranslationDataSet_serialises_to_xyz_object()
        {
            var t = new Transformation
            {
                Translation = new TranslationDataSet { X = "1", Y = "2", Z = "3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonTransformation(t));

            Assert.That(json, Does.Contain("\"translationDataSet\":{\"x\":\"1\",\"y\":\"2\",\"z\":\"3\"}"));
        }

        [Test]
        public void TranslationDataSet_deserialises_to_ITranslationDataSet()
        {
            const string json = "{\"translationDataSet\":{\"x\":\"7\",\"y\":\"8\",\"z\":\"9\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var t = wire.ToTransformation();

            Assert.That(t.Translation, Is.InstanceOf<ITranslationDataSet>());
            var ds = (ITranslationDataSet)t.Translation;
            Assert.That(ds.X, Is.EqualTo("7"));
            Assert.That(ds.Y, Is.EqualTo("8"));
            Assert.That(ds.Z, Is.EqualTo("9"));
        }

        // ---------------- negative ----------------

        [Test]
        public void Both_translation_and_translationDataSet_present_DataSet_wins()
        {
            const string json = "{\"translation\":{\"value\":\"1 2 3\"},"
                + "\"translationDataSet\":{\"x\":\"9\",\"y\":\"0\",\"z\":\"0\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var t = wire.ToTransformation();

            Assert.That(t.Translation, Is.InstanceOf<ITranslationDataSet>());
            Assert.That(((ITranslationDataSet)t.Translation).X, Is.EqualTo("9"));
        }

        [Test]
        public void JsonTranslation_default_constructor_yields_null_value()
        {
            var jt = new JsonTranslation();
            var t = jt.ToTranslation();

            Assert.That(t, Is.Not.Null);
            Assert.That(t.Value, Is.Null);
        }

        [Test]
        public void JsonTranslationDataSet_default_constructor_yields_null_components()
        {
            var jt = new JsonTranslationDataSet();
            var ds = jt.ToTranslationDataSet();

            Assert.That(ds.X, Is.Null);
            Assert.That(ds.Y, Is.Null);
            Assert.That(ds.Z, Is.Null);
        }

        [Test]
        public void JsonTranslation_ctor_with_null_input_keeps_default_values()
        {
            var jt = new JsonTranslation(null!);
            Assert.That(jt.Value, Is.Null);
        }

        [Test]
        public void JsonTranslationDataSet_ctor_with_null_input_keeps_default_values()
        {
            var jt = new JsonTranslationDataSet(null!);
            Assert.That(jt.X, Is.Null);
        }

        [Test]
        public void Both_Translation_and_Rotation_DataSet_round_trip_independently()
        {
            // Pin that Transformation can carry DataSet variants on both
            // independent xs:choice slots without one corrupting the other.
            var input = new Transformation
            {
                Translation = new TranslationDataSet { X = "1", Y = "2", Z = "3" },
                Rotation = new RotationDataSet { A = "10", B = "20", C = "30" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonTransformation(input));
            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var output = wire.ToTransformation();

            Assert.That(output.Translation, Is.InstanceOf<ITranslationDataSet>());
            Assert.That(output.Rotation, Is.InstanceOf<IRotationDataSet>());
            Assert.That(((ITranslationDataSet)output.Translation).X, Is.EqualTo("1"));
            Assert.That(((IRotationDataSet)output.Rotation).A, Is.EqualTo("10"));
        }
    }
}
