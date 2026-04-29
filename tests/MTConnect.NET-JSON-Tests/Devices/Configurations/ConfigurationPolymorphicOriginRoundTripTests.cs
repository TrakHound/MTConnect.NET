// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the Origin polymorphism on <c>Motion</c> and
    /// <c>CoordinateSystem</c> in the standard JSON wire format.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractOrigin</c>, <c>Origin</c>,
    /// <c>OriginDataSet</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (the JSON shape mirrors XSD <c>MotionType</c> /
    /// <c>CoordinateSystemType</c> Origin choice).</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicOriginRoundTripTests
    {
        // ---------------- positive: Motion + simple Origin ----------------

        [Test]
        public void Simple_Origin_serialises_to_value_field_on_Motion()
        {
            var motion = new Motion
            {
                Id = "m1",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Origin = new Origin { Value = "1 2 3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(motion));

            Assert.That(json, Does.Contain("\"origin\":{\"value\":\"1 2 3\"}"));
            Assert.That(json, Does.Not.Contain("originDataSet"));
        }

        [Test]
        public void Simple_Origin_deserialises_to_IOrigin_on_Motion()
        {
            const string json = "{\"id\":\"m1\",\"type\":\"PRISMATIC\",\"actuation\":\"DIRECT\","
                + "\"origin\":{\"value\":\"4 5 6\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var motion = wire.ToMotion();

            Assert.That(motion.Origin, Is.InstanceOf<IOrigin>());
            Assert.That(((IOrigin)motion.Origin).Value, Is.EqualTo("4 5 6"));
        }

        // ---------------- positive: Motion + OriginDataSet ----------------

        [Test]
        public void OriginDataSet_serialises_to_xyz_object_on_Motion()
        {
            var motion = new Motion
            {
                Id = "m2",
                Type = MotionType.REVOLUTE,
                Actuation = MotionActuationType.DIRECT,
                Origin = new OriginDataSet { X = "1", Y = "2", Z = "3" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonMotion(motion));

            Assert.That(json, Does.Contain("\"originDataSet\":{\"x\":\"1\",\"y\":\"2\",\"z\":\"3\"}"));
        }

        [Test]
        public void OriginDataSet_deserialises_to_IOriginDataSet_on_Motion()
        {
            const string json = "{\"id\":\"m2\",\"type\":\"REVOLUTE\",\"actuation\":\"DIRECT\","
                + "\"originDataSet\":{\"x\":\"7\",\"y\":\"8\",\"z\":\"9\"}}";

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

            Assert.That(json, Does.Contain("\"origin\":{\"value\":\"10 20 30\"}"));
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

            Assert.That(json, Does.Contain("\"originDataSet\":{\"x\":\"100\",\"y\":\"200\",\"z\":\"300\"}"));
            Assert.That(output.Origin, Is.InstanceOf<IOriginDataSet>());
        }

        // ---------------- negative ----------------

        [Test]
        public void Both_origin_and_originDataSet_present_DataSet_wins_on_Motion()
        {
            const string json = "{\"id\":\"m\",\"type\":\"PRISMATIC\",\"actuation\":\"DIRECT\","
                + "\"origin\":{\"value\":\"1 2 3\"},"
                + "\"originDataSet\":{\"x\":\"9\",\"y\":\"0\",\"z\":\"0\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonMotion>(json)!;
            var motion = wire.ToMotion();

            Assert.That(motion.Origin, Is.InstanceOf<IOriginDataSet>());
            Assert.That(((IOriginDataSet)motion.Origin).X, Is.EqualTo("9"));
        }

        [Test]
        public void JsonOrigin_default_constructor_yields_null_value()
        {
            var jo = new JsonOrigin();
            var origin = jo.ToOrigin();

            Assert.That(origin, Is.Not.Null);
            Assert.That(origin.Value, Is.Null);
        }

        [Test]
        public void JsonOriginDataSet_default_constructor_yields_null_components()
        {
            var jo = new JsonOriginDataSet();
            var ds = jo.ToOriginDataSet();

            Assert.That(ds.X, Is.Null);
            Assert.That(ds.Y, Is.Null);
            Assert.That(ds.Z, Is.Null);
        }

        [Test]
        public void JsonOrigin_ctor_with_null_input_keeps_default_values()
        {
            var jo = new JsonOrigin(null!);
            Assert.That(jo.Value, Is.Null);
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
