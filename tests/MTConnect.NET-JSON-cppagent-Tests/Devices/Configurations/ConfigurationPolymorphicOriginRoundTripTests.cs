// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_cppagent_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the Origin polymorphism on <c>Motion</c> and
    /// <c>CoordinateSystem</c> in the cppagent v2 JSON dialect.
    /// Simple <c>Origin</c> serialises as a numeric array;
    /// <c>OriginDataSet</c> as a flat PascalCase object with string values.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractOrigin</c>, <c>Origin</c>,
    /// <c>OriginDataSet</c>).</item>
    /// <item>cppagent reference implementation —
    /// <see href="https://github.com/mtconnect/cppagent"/>.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicOriginRoundTripTests
    {
        // ---------------- positive: Motion + simple Origin ----------------

        [Test]
        public void Simple_Origin_serialises_as_numeric_array_on_Motion()
        {
            var motion = new Motion
            {
                Id = "m1",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Origin = new Origin { Value = "1 2 3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(motion));

            Assert.That(json, Does.Contain("\"Origin\":[1,2,3]"));
            Assert.That(json, Does.Not.Contain("OriginDataSet"));
        }

        [Test]
        public void Simple_Origin_round_trips_through_numeric_array_on_Motion()
        {
            var input = new Motion
            {
                Id = "m1",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Origin = new Origin { Value = "1 2 3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(input));
            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var output = wire.ToMotion();

            Assert.That(output.Origin, Is.InstanceOf<IOrigin>());
            Assert.That(((IOrigin)output.Origin).Value, Is.EqualTo("1 2 3"));
        }

        // ---------------- positive: Motion + OriginDataSet ----------------

        [Test]
        public void OriginDataSet_serialises_as_flat_pascalcase_object_on_Motion()
        {
            var motion = new Motion
            {
                Id = "m2",
                Type = MotionType.REVOLUTE,
                Actuation = MotionActuationType.DIRECT,
                Origin = new OriginDataSet { X = "1", Y = "2", Z = "3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(motion));

            Assert.That(json, Does.Contain("\"OriginDataSet\":{\"X\":\"1\",\"Y\":\"2\",\"Z\":\"3\"}"));
        }

        [Test]
        public void OriginDataSet_deserialises_to_IOriginDataSet_on_Motion()
        {
            const string json = "{\"id\":\"m\",\"type\":\"REVOLUTE\",\"actuation\":\"DIRECT\","
                + "\"OriginDataSet\":{\"X\":\"7\",\"Y\":\"8\",\"Z\":\"9\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var motion = wire.ToMotion();

            Assert.That(motion.Origin, Is.InstanceOf<IOriginDataSet>());
            var ds = (IOriginDataSet)motion.Origin;
            Assert.That(ds.X, Is.EqualTo("7"));
            Assert.That(ds.Y, Is.EqualTo("8"));
            Assert.That(ds.Z, Is.EqualTo("9"));
        }

        // ---------------- positive: CoordinateSystem ----------------

        [Test]
        public void Simple_Origin_round_trips_on_CoordinateSystem()
        {
            var cs = new CoordinateSystem
            {
                Id = "cs1",
                Type = CoordinateSystemType.MACHINE,
                Origin = new Origin { Value = "10 20 30" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonCoordinateSystem(cs));
            var wire = JsonRoundTripHelper.Deserialize<JsonCoordinateSystem>(json)!;
            var output = wire.ToCoordinateSystem();

            Assert.That(json, Does.Contain("\"Origin\":[10,20,30]"));
            Assert.That(output.Origin, Is.InstanceOf<IOrigin>());
            Assert.That(((IOrigin)output.Origin).Value, Is.EqualTo("10 20 30"));
        }

        [Test]
        public void OriginDataSet_round_trips_on_CoordinateSystem()
        {
            var cs = new CoordinateSystem
            {
                Id = "cs2",
                Type = CoordinateSystemType.WORLD,
                Origin = new OriginDataSet { X = "100", Y = "200", Z = "300" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonCoordinateSystem(cs));
            var wire = JsonRoundTripHelper.Deserialize<JsonCoordinateSystem>(json)!;
            var output = wire.ToCoordinateSystem();

            Assert.That(json, Does.Contain("\"OriginDataSet\":{\"X\":\"100\",\"Y\":\"200\",\"Z\":\"300\"}"));
            Assert.That(output.Origin, Is.InstanceOf<IOriginDataSet>());
        }

        // ---------------- negative ----------------

        [Test]
        public void Both_Origin_and_OriginDataSet_present_DataSet_wins()
        {
            const string json = "{\"id\":\"m\",\"type\":\"PRISMATIC\",\"actuation\":\"DIRECT\","
                + "\"Origin\":[1,2,3],"
                + "\"OriginDataSet\":{\"X\":\"9\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var motion = wire.ToMotion();

            Assert.That(motion.Origin, Is.InstanceOf<IOriginDataSet>());
            Assert.That(((IOriginDataSet)motion.Origin).X, Is.EqualTo("9"));
        }

        [Test]
        public void JsonOriginDataSet_default_constructor_yields_null_components()
        {
            var jo = new JsonOriginDataSet();
            var ds = jo.ToOriginDataSet();

            Assert.That(ds.X, Is.Null);
        }

        [Test]
        public void JsonOriginDataSet_ctor_with_null_input_keeps_default_values()
        {
            var jo = new JsonOriginDataSet(null!);
            Assert.That(jo.X, Is.Null);
        }

        [Test]
        public void Null_coordinateSystem_passed_to_ctor_keeps_default_values()
        {
            var jcs = new JsonCoordinateSystem(null!);
            Assert.That(jcs.Origin, Is.Null);
            Assert.That(jcs.OriginDataSet, Is.Null);
        }
    }
}
