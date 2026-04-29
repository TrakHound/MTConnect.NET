// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_cppagent_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the Axis polymorphism on <c>Motion</c> in the cppagent v2 JSON
    /// dialect: the simple <c>Axis</c> leaf serialises as a numeric array
    /// (<c>"Axis": [1.0, 2.0, 3.0]</c>) inline on the parent, while
    /// <c>AxisDataSet</c> serialises as a flat PascalCase object
    /// (<c>"AxisDataSet": {"X": 1.0, "Y": 2.0, "Z": 3.0}</c>).
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractAxis</c>, <c>Axis</c>, <c>AxisDataSet</c>).</item>
    /// <item>cppagent reference implementation —
    /// <see href="https://github.com/mtconnect/cppagent"/> JSON v2 output for
    /// the Configuration sub-element family.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicAxisRoundTripTests
    {
        // ---------------- positive: simple Axis ----------------

        [Test]
        public void Simple_Axis_serialises_as_numeric_array()
        {
            var motion = new Motion
            {
                Id = "m1",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Axis = new Axis { Value = "1 2 3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(motion));

            Assert.That(json, Does.Contain("\"Axis\":[1,2,3]"));
            Assert.That(json, Does.Not.Contain("AxisDataSet"));
        }

        [Test]
        public void Simple_Axis_round_trips_through_numeric_array()
        {
            var input = new Motion
            {
                Id = "m1",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Axis = new Axis { Value = "1 2 3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(input));
            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var output = wire.ToMotion();

            Assert.That(output.Axis, Is.InstanceOf<IAxis>());
            Assert.That(output.Axis, Is.Not.InstanceOf<IAxisDataSet>());
            Assert.That(((IAxis)output.Axis).Value, Is.EqualTo("1 2 3"));
        }

        // ---------------- positive: AxisDataSet ----------------

        [Test]
        public void AxisDataSet_serialises_as_flat_pascalcase_object()
        {
            var motion = new Motion
            {
                Id = "m2",
                Type = MotionType.REVOLUTE,
                Actuation = MotionActuationType.VIRTUAL,
                Axis = new AxisDataSet { X = 1.0, Y = 2.0, Z = 3.0 }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(motion));

            Assert.That(json, Does.Contain("\"AxisDataSet\":{\"X\":1,\"Y\":2,\"Z\":3}"));
            Assert.That(json, Does.Not.Contain("\"Axis\":["));
        }

        [Test]
        public void AxisDataSet_round_trips_through_flat_object()
        {
            const string json = "{\"id\":\"m\",\"type\":\"REVOLUTE\",\"actuation\":\"DIRECT\","
                + "\"AxisDataSet\":{\"X\":7,\"Y\":8,\"Z\":9}}";

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
        public void Both_array_and_dataset_present_DataSet_wins()
        {
            const string json = "{\"id\":\"m\",\"type\":\"PRISMATIC\",\"actuation\":\"DIRECT\","
                + "\"Axis\":[1,2,3],"
                + "\"AxisDataSet\":{\"X\":9,\"Y\":0,\"Z\":0}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var motion = wire.ToMotion();

            Assert.That(motion.Axis, Is.InstanceOf<IAxisDataSet>());
            Assert.That(((IAxisDataSet)motion.Axis).X, Is.EqualTo(9.0));
        }

        [Test]
        public void Null_axis_property_emits_neither_field()
        {
            var motion = new Motion
            {
                Id = "m3",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Axis = null
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(motion));

            Assert.That(json, Does.Not.Contain("\"Axis\""));
            Assert.That(json, Does.Not.Contain("AxisDataSet"));
        }

        [Test]
        public void JsonAxisDataSet_default_constructor_yields_zero_components()
        {
            var ja = new JsonAxisDataSet();
            var ds = ja.ToAxisDataSet();

            Assert.That(ds.X, Is.EqualTo(0.0));
            Assert.That(ds.Y, Is.EqualTo(0.0));
            Assert.That(ds.Z, Is.EqualTo(0.0));
        }

        [Test]
        public void JsonAxisDataSet_ctor_with_null_input_keeps_default_values()
        {
            var ja = new JsonAxisDataSet(null!);
            Assert.That(ja.X, Is.EqualTo(0.0));
        }

        [Test]
        public void Null_motion_passed_to_ctor_keeps_default_values()
        {
            var jm = new JsonMotion(null!);
            Assert.That(jm.Axis, Is.Null);
            Assert.That(jm.AxisDataSet, Is.Null);
        }

        [Test]
        public void Empty_array_axis_deserialises_to_null_simple_axis()
        {
            // Cppagent JsonHelper.ToAxis returns null for null/empty value
            // collections — the parent then leaves Motion.Axis null rather
            // than fabricating an empty Axis.
            const string json = "{\"id\":\"m\",\"type\":\"PRISMATIC\",\"actuation\":\"DIRECT\","
                + "\"Axis\":[]}";

            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var motion = wire.ToMotion();

            Assert.That(motion.Axis, Is.Null);
        }

        [Test]
        public void Numeric_array_round_trip_normalises_invariant_culture()
        {
            // JsonHelper.JoinValues uses CultureInfo.InvariantCulture; pin
            // that fractional values format with '.' regardless of host
            // culture.
            var input = new Motion
            {
                Id = "m",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Axis = new Axis { Value = "1.5 2.5 3.5" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(input));
            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var output = wire.ToMotion();

            Assert.That(json, Does.Contain("\"Axis\":[1.5,2.5,3.5]"));
            Assert.That(((IAxis)output.Axis).Value.Split(' ').Length, Is.EqualTo(3));
            Assert.That(((IAxis)output.Axis).Value.Split(' ').All(p => double.Parse(p, System.Globalization.CultureInfo.InvariantCulture) > 0), Is.True);
        }
    }
}
