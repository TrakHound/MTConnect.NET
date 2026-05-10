// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the Axis polymorphism on <c>Motion</c> in the standard JSON
    /// wire format: <c>axis</c> (simple) vs <c>axisDataSet</c> (XYZ
    /// flat object).
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractAxis</c>, <c>Axis</c>, <c>AxisDataSet</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (the JSON shape mirrors XSD <c>MotionType</c>'s Axis choice).</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicAxisRoundTripTests
    {
        // ---------------- positive: simple Axis ----------------

        [Test]
        public void Simple_Axis_serialises_to_value_field()
        {
            var motion = new Motion
            {
                Id = "m1",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Axis = new Axis { Value = "1 2 3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(motion));

            Assert.That(json, Does.Contain("\"axis\":{\"value\":\"1 2 3\"}"));
            Assert.That(json, Does.Not.Contain("axisDataSet"));
        }

        [Test]
        public void Simple_Axis_deserialises_to_IAxis()
        {
            const string json = "{\"id\":\"m1\",\"type\":\"PRISMATIC\",\"actuation\":\"DIRECT\","
                + "\"axis\":{\"value\":\"4 5 6\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var motion = wire.ToMotion();

            Assert.That(motion.Axis, Is.InstanceOf<IAxis>());
            Assert.That(motion.Axis, Is.Not.InstanceOf<IAxisDataSet>());
            Assert.That(((IAxis)motion.Axis).Value, Is.EqualTo("4 5 6"));
        }

        // ---------------- positive: AxisDataSet ----------------

        [Test]
        public void AxisDataSet_serialises_to_xyz_flat_object()
        {
            var motion = new Motion
            {
                Id = "m2",
                Type = MotionType.REVOLUTE,
                Actuation = MotionActuationType.VIRTUAL,
                Axis = new AxisDataSet { X = 1.0, Y = 2.0, Z = 3.0 }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(motion));

            Assert.That(json, Does.Contain("\"axisDataSet\":{\"x\":1,\"y\":2,\"z\":3}"));
            Assert.That(json, Does.Not.Contain("\"axis\":{"));
        }

        [Test]
        public void AxisDataSet_deserialises_to_IAxisDataSet()
        {
            const string json = "{\"id\":\"m2\",\"type\":\"REVOLUTE\",\"actuation\":\"DIRECT\","
                + "\"axisDataSet\":{\"x\":7,\"y\":8,\"z\":9}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var motion = wire.ToMotion();

            Assert.That(motion.Axis, Is.InstanceOf<IAxisDataSet>());
            var ds = (IAxisDataSet)motion.Axis;
            Assert.That(ds.X, Is.EqualTo(7.0));
            Assert.That(ds.Y, Is.EqualTo(8.0));
            Assert.That(ds.Z, Is.EqualTo(9.0));
        }

        // ---------------- negative ----------------

        [Test]
        public void Null_axis_property_emits_no_axis_field()
        {
            var motion = new Motion
            {
                Id = "m3",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Axis = null
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(motion));

            Assert.That(json, Does.Not.Contain("\"axis\""));
            Assert.That(json, Does.Not.Contain("axisDataSet"));
        }

        [Test]
        public void Both_axis_and_axisDataSet_present_DataSet_wins()
        {
            // Pin that ToMotion narrows the DataSet form regardless of the
            // simple field's content.
            const string json = "{\"id\":\"m\",\"type\":\"PRISMATIC\",\"actuation\":\"DIRECT\","
                + "\"axis\":{\"value\":\"1 2 3\"},"
                + "\"axisDataSet\":{\"x\":9,\"y\":0,\"z\":0}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var motion = wire.ToMotion();

            Assert.That(motion.Axis, Is.InstanceOf<IAxisDataSet>());
            Assert.That(((IAxisDataSet)motion.Axis).X, Is.EqualTo(9.0));
        }

        [Test]
        public void Default_JsonAxis_constructor_yields_null_value()
        {
            var ja = new JsonAxis();
            var ax = ja.ToAxis();

            Assert.That(ax, Is.Not.Null);
            Assert.That(ax.Value, Is.Null);
        }

        [Test]
        public void Default_JsonAxisDataSet_constructor_yields_zero_components()
        {
            var ja = new JsonAxisDataSet();
            var ds = ja.ToAxisDataSet();

            Assert.That(ds.X, Is.EqualTo(0.0));
            Assert.That(ds.Y, Is.EqualTo(0.0));
            Assert.That(ds.Z, Is.EqualTo(0.0));
        }

        [Test]
        public void JsonAxis_ctor_with_null_input_keeps_default_values()
        {
            var ja = new JsonAxis(null!);

            Assert.That(ja.Value, Is.Null);
        }

        [Test]
        public void JsonAxisDataSet_ctor_with_null_input_keeps_default_values()
        {
            var ja = new JsonAxisDataSet(null!);

            Assert.That(ja.X, Is.EqualTo(0.0));
        }

        [Test]
        public void Round_trip_preserves_AxisDataSet_components()
        {
            var input = new Motion
            {
                Id = "m",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Axis = new AxisDataSet { X = 1.5, Y = 2.5, Z = 3.5 }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(input));
            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var output = wire.ToMotion();

            Assert.That(output.Axis, Is.InstanceOf<IAxisDataSet>());
            var ds = (IAxisDataSet)output.Axis;
            Assert.That(ds.X, Is.EqualTo(1.5));
            Assert.That(ds.Y, Is.EqualTo(2.5));
            Assert.That(ds.Z, Is.EqualTo(3.5));
        }

        [Test]
        public void Null_motion_passed_to_JsonMotion_ctor_keeps_default_values()
        {
            var jm = new JsonMotion(null!);

            Assert.That(jm.Axis, Is.Null);
            Assert.That(jm.AxisDataSet, Is.Null);
        }
    }
}
