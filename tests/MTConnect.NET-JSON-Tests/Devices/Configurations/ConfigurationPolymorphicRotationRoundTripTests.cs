// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the Rotation polymorphism on <c>Transformation</c> in the
    /// standard JSON wire format. RotationDataSet uses a/b/c keys per
    /// the ABC dataset shape.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractRotation</c>, <c>Rotation</c>,
    /// <c>RotationDataSet</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (the JSON shape mirrors XSD <c>TransformationType</c>'s Rotation choice).</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicRotationRoundTripTests
    {
        // ---------------- positive: simple Rotation ----------------

        [Test]
        public void Simple_Rotation_serialises_to_value_field()
        {
            var t = new Transformation
            {
                Rotation = new Rotation { Value = "10 20 30" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonTransformation(t));

            Assert.That(json, Does.Contain("\"rotation\":{\"value\":\"10 20 30\"}"));
            Assert.That(json, Does.Not.Contain("rotationDataSet"));
        }

        [Test]
        public void Simple_Rotation_deserialises_to_IRotation()
        {
            const string json = "{\"rotation\":{\"value\":\"10 20 30\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var t = wire.ToTransformation();

            Assert.That(t.Rotation, Is.InstanceOf<IRotation>());
            Assert.That(((IRotation)t.Rotation).Value, Is.EqualTo("10 20 30"));
        }

        // ---------------- positive: RotationDataSet ----------------

        [Test]
        public void RotationDataSet_serialises_to_abc_object()
        {
            var t = new Transformation
            {
                Rotation = new RotationDataSet { A = "10", B = "20", C = "30" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonTransformation(t));

            Assert.That(json, Does.Contain("\"rotationDataSet\":{\"a\":\"10\",\"b\":\"20\",\"c\":\"30\"}"));
        }

        [Test]
        public void RotationDataSet_deserialises_to_IRotationDataSet()
        {
            const string json = "{\"rotationDataSet\":{\"a\":\"10\",\"b\":\"20\",\"c\":\"30\"}}";

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
        public void Both_rotation_and_rotationDataSet_present_DataSet_wins()
        {
            const string json = "{\"rotation\":{\"value\":\"1 2 3\"},"
                + "\"rotationDataSet\":{\"a\":\"9\",\"b\":\"0\",\"c\":\"0\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonTransformation>(json)!;
            var t = wire.ToTransformation();

            Assert.That(t.Rotation, Is.InstanceOf<IRotationDataSet>());
            Assert.That(((IRotationDataSet)t.Rotation).A, Is.EqualTo("9"));
        }

        [Test]
        public void JsonRotation_default_constructor_yields_null_value()
        {
            var jr = new JsonRotation();
            var r = jr.ToRotation();

            Assert.That(r, Is.Not.Null);
            Assert.That(r.Value, Is.Null);
        }

        [Test]
        public void JsonRotationDataSet_default_constructor_yields_null_components()
        {
            var jr = new JsonRotationDataSet();
            var ds = jr.ToRotationDataSet();

            Assert.That(ds.A, Is.Null);
            Assert.That(ds.B, Is.Null);
            Assert.That(ds.C, Is.Null);
        }

        [Test]
        public void JsonRotation_ctor_with_null_input_keeps_default_values()
        {
            var jr = new JsonRotation(null!);
            Assert.That(jr.Value, Is.Null);
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

            Assert.That(json, Does.Not.Contain("\"rotation\""));
            Assert.That(json, Does.Not.Contain("rotationDataSet"));
            Assert.That(json, Does.Contain("\"translation\":{\"value\":\"1 2 3\"}"));
        }
    }
}
