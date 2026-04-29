// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_cppagent_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the Translation polymorphism on <c>Transformation</c> in the
    /// cppagent v2 JSON dialect. Simple <c>Translation</c> serialises as a
    /// numeric array; <c>TranslationDataSet</c> as a flat PascalCase object
    /// with X/Y/Z string values.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractTranslation</c>, <c>Translation</c>,
    /// <c>TranslationDataSet</c>).</item>
    /// <item>cppagent reference implementation —
    /// <see href="https://github.com/mtconnect/cppagent"/>.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicTranslationRoundTripTests
    {
        // ---------------- positive: simple Translation ----------------

        [Test]
        public void Simple_Translation_serialises_as_numeric_array()
        {
            var t = new Transformation
            {
                Translation = new Translation { Value = "1 2 3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonTransformation(t));

            Assert.That(json, Does.Contain("\"Translation\":[1,2,3]"));
            Assert.That(json, Does.Not.Contain("TranslationDataSet"));
        }

        [Test]
        public void Simple_Translation_deserialises_to_ITranslation()
        {
            const string json = "{\"Translation\":[1,2,3]}";

            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var t = wire.ToTransformation();

            Assert.That(t.Translation, Is.InstanceOf<ITranslation>());
            Assert.That(((ITranslation)t.Translation).Value, Is.EqualTo("1 2 3"));
        }

        // ---------------- positive: TranslationDataSet ----------------

        [Test]
        public void TranslationDataSet_serialises_as_flat_pascalcase_object()
        {
            var t = new Transformation
            {
                Translation = new TranslationDataSet { X = "1", Y = "2", Z = "3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonTransformation(t));

            Assert.That(json, Does.Contain("\"TranslationDataSet\":{\"X\":\"1\",\"Y\":\"2\",\"Z\":\"3\"}"));
        }

        [Test]
        public void TranslationDataSet_deserialises_to_ITranslationDataSet()
        {
            const string json = "{\"TranslationDataSet\":{\"X\":\"7\",\"Y\":\"8\",\"Z\":\"9\"}}";

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
        public void Both_Translation_and_TranslationDataSet_present_DataSet_wins()
        {
            const string json = "{\"Translation\":[1,2,3],"
                + "\"TranslationDataSet\":{\"X\":\"9\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var t = wire.ToTransformation();

            Assert.That(t.Translation, Is.InstanceOf<ITranslationDataSet>());
            Assert.That(((ITranslationDataSet)t.Translation).X, Is.EqualTo("9"));
        }

        [Test]
        public void JsonTranslationDataSet_default_constructor_yields_null_components()
        {
            var jt = new JsonTranslationDataSet();
            var ds = jt.ToTranslationDataSet();

            Assert.That(ds.X, Is.Null);
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
