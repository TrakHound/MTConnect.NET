// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_cppagent_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the Rotation polymorphism on <c>Transformation</c> in the
    /// cppagent v2 JSON dialect. Simple <c>Rotation</c> serialises as a
    /// numeric array; <c>RotationDataSet</c> as a flat PascalCase object
    /// with A/B/C string values.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractRotation</c>, <c>Rotation</c>,
    /// <c>RotationDataSet</c>).</item>
    /// <item>cppagent reference implementation —
    /// <see href="https://github.com/mtconnect/cppagent"/>.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicRotationRoundTripTests
    {
        // ---------------- positive: simple Rotation ----------------

        [Test]
        public void Simple_Rotation_serialises_as_numeric_array()
        {
            var t = new Transformation
            {
                Rotation = new Rotation { Value = "10 20 30" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonTransformation(t));

            Assert.That(json, Does.Contain("\"Rotation\":[10,20,30]"));
            Assert.That(json, Does.Not.Contain("RotationDataSet"));
        }

        [Test]
        public void Simple_Rotation_deserialises_to_IRotation()
        {
            const string json = "{\"Rotation\":[10,20,30]}";

            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var t = wire.ToTransformation();

            Assert.That(t.Rotation, Is.InstanceOf<IRotation>());
            Assert.That(((IRotation)t.Rotation).Value, Is.EqualTo("10 20 30"));
        }

        // ---------------- positive: RotationDataSet ----------------

        [Test]
        public void RotationDataSet_serialises_as_flat_pascalcase_object()
        {
            var t = new Transformation
            {
                Rotation = new RotationDataSet { A = "10", B = "20", C = "30" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonTransformation(t));

            Assert.That(json, Does.Contain("\"RotationDataSet\":{\"A\":\"10\",\"B\":\"20\",\"C\":\"30\"}"));
        }

        [Test]
        public void RotationDataSet_deserialises_to_IRotationDataSet()
        {
            const string json = "{\"RotationDataSet\":{\"A\":\"10\",\"B\":\"20\",\"C\":\"30\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var t = wire.ToTransformation();

            Assert.That(t.Rotation, Is.InstanceOf<IRotationDataSet>());
            var ds = (IRotationDataSet)t.Rotation;
            Assert.That(ds.A, Is.EqualTo("10"));
            Assert.That(ds.B, Is.EqualTo("20"));
            Assert.That(ds.C, Is.EqualTo("30"));
        }

        // ---------------- negative ----------------

        [Test]
        public void Both_Rotation_and_RotationDataSet_present_DataSet_wins()
        {
            const string json = "{\"Rotation\":[1,2,3],"
                + "\"RotationDataSet\":{\"A\":\"9\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var t = wire.ToTransformation();

            Assert.That(t.Rotation, Is.InstanceOf<IRotationDataSet>());
            Assert.That(((IRotationDataSet)t.Rotation).A, Is.EqualTo("9"));
        }

        [Test]
        public void JsonRotationDataSet_default_constructor_yields_null_components()
        {
            var jr = new JsonRotationDataSet();
            var ds = jr.ToRotationDataSet();

            Assert.That(ds.A, Is.Null);
        }

        [Test]
        public void JsonRotationDataSet_ctor_with_null_input_keeps_default_values()
        {
            var jr = new JsonRotationDataSet(null!);
            Assert.That(jr.A, Is.Null);
        }

        [Test]
        public void Null_transformation_passed_to_ctor_keeps_default_values()
        {
            var jt = new JsonTransformation(null!);
            Assert.That(jt.Rotation, Is.Null);
            Assert.That(jt.RotationDataSet, Is.Null);
        }

        [Test]
        public void Null_rotation_property_emits_no_rotation_field()
        {
            var t = new Transformation
            {
                Rotation = null,
                Translation = new Translation { Value = "1 2 3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonTransformation(t));

            Assert.That(json, Does.Not.Contain("\"Rotation\""));
            Assert.That(json, Does.Not.Contain("RotationDataSet"));
            Assert.That(json, Does.Contain("\"Translation\":[1,2,3]"));
        }
    }
}
