// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the Scale polymorphism on <c>SolidModel</c> in the standard JSON
    /// wire format.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractScale</c>, <c>Scale</c>, <c>ScaleDataSet</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (the JSON shape mirrors XSD <c>SolidModelType</c>'s Scale choice).</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicScaleRoundTripTests
    {
        // ---------------- positive: simple Scale ----------------

        [Test]
        public void Simple_Scale_serialises_to_value_field()
        {
            var sm = new SolidModel
            {
                Id = "sm1",
                MediaType = MediaType.STL,
                Scale = new Scale { Value = "1 1 1" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonSolidModel(sm));

            Assert.That(json, Does.Contain("\"scale\":{\"value\":\"1 1 1\"}"));
            Assert.That(json, Does.Not.Contain("scaleDataSet"));
        }

        [Test]
        public void Simple_Scale_deserialises_to_IScale()
        {
            const string json = "{\"id\":\"sm1\",\"mediaType\":\"STL\","
                + "\"scale\":{\"value\":\"2 2 2\"}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonSolidModel>(json)!;
            var sm = wire.ToSolidModel();

            Assert.That(sm.Scale, Is.InstanceOf<IScale>());
            Assert.That(((IScale)sm.Scale).Value, Is.EqualTo("2 2 2"));
        }

        // ---------------- positive: ScaleDataSet ----------------

        [Test]
        public void ScaleDataSet_serialises_to_xyz_object()
        {
            var sm = new SolidModel
            {
                Id = "sm2",
                MediaType = MediaType.OBJ,
                Scale = new ScaleDataSet { X = 1.5, Y = 2.5, Z = 3.5 }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonSolidModel(sm));

            Assert.That(json, Does.Contain("\"scaleDataSet\":{\"x\":1.5,\"y\":2.5,\"z\":3.5}"));
        }

        [Test]
        public void ScaleDataSet_deserialises_to_IScaleDataSet()
        {
            const string json = "{\"id\":\"sm2\",\"mediaType\":\"STL\","
                + "\"scaleDataSet\":{\"x\":1.5,\"y\":2.5,\"z\":3.5}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonSolidModel>(json)!;
            var sm = wire.ToSolidModel();

            Assert.That(sm.Scale, Is.InstanceOf<IScaleDataSet>());
            var ds = (IScaleDataSet)sm.Scale;
            Assert.That(ds.X, Is.EqualTo(1.5));
            Assert.That(ds.Y, Is.EqualTo(2.5));
            Assert.That(ds.Z, Is.EqualTo(3.5));
        }

        // ---------------- negative ----------------

        [Test]
        public void Both_scale_and_scaleDataSet_present_DataSet_wins()
        {
            const string json = "{\"id\":\"sm\",\"mediaType\":\"STL\","
                + "\"scale\":{\"value\":\"1 1 1\"},"
                + "\"scaleDataSet\":{\"x\":9,\"y\":0,\"z\":0}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonSolidModel>(json)!;
            var sm = wire.ToSolidModel();

            Assert.That(sm.Scale, Is.InstanceOf<IScaleDataSet>());
            Assert.That(((IScaleDataSet)sm.Scale).X, Is.EqualTo(9.0));
        }

        [Test]
        public void Null_scale_property_emits_no_scale_field()
        {
            var sm = new SolidModel
            {
                Id = "sm3",
                MediaType = MediaType.STL,
                Scale = null
            };

            var json = JsonRoundTripHelper.Serialize(new JsonSolidModel(sm));

            Assert.That(json, Does.Not.Contain("\"scale\""));
            Assert.That(json, Does.Not.Contain("scaleDataSet"));
        }

        [Test]
        public void JsonScale_default_constructor_yields_null_value()
        {
            var js = new JsonScale();
            var s = js.ToScale();

            Assert.That(s, Is.Not.Null);
            Assert.That(s.Value, Is.Null);
        }

        [Test]
        public void JsonScaleDataSet_default_constructor_yields_zero_components()
        {
            var js = new JsonScaleDataSet();
            var ds = js.ToScaleDataSet();

            Assert.That(ds.X, Is.EqualTo(0.0));
        }

        [Test]
        public void JsonScale_ctor_with_null_input_keeps_default_values()
        {
            var js = new JsonScale(null!);
            Assert.That(js.Value, Is.Null);
        }

        [Test]
        public void JsonScaleDataSet_ctor_with_null_input_keeps_default_values()
        {
            var js = new JsonScaleDataSet(null!);
            Assert.That(js.X, Is.EqualTo(0.0));
        }

        [Test]
        public void Null_solidModel_passed_to_ctor_keeps_default_values()
        {
            var jsm = new JsonSolidModel(null!);
            Assert.That(jsm.Scale, Is.Null);
            Assert.That(jsm.ScaleDataSet, Is.Null);
        }
    }
}
