// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_cppagent_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the Scale polymorphism on <c>SolidModel</c> in the cppagent v2
    /// JSON dialect. Simple <c>Scale</c> serialises as a numeric array;
    /// <c>ScaleDataSet</c> as a flat PascalCase object with double values.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractScale</c>, <c>Scale</c>,
    /// <c>ScaleDataSet</c>).</item>
    /// <item>cppagent reference implementation —
    /// <see href="https://github.com/mtconnect/cppagent"/>.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicScaleRoundTripTests
    {
        // ---------------- positive: simple Scale ----------------

        /// <summary>Pins the behaviour expressed by the test name: simple scale serialises as numeric array.</summary>
        [Test]
        public void Simple_Scale_serialises_as_numeric_array()
        {
            var sm = new SolidModel
            {
                Id = "sm1",
                MediaType = MediaType.STL,
                Scale = new Scale { Value = "1 1 1" }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonSolidModel(sm));

            Assert.That(json, Does.Contain("\"Scale\":[1,1,1]"));
            Assert.That(json, Does.Not.Contain("ScaleDataSet"));
        }

        /// <summary>Pins the behaviour expressed by the test name: simple scale deserialises to i scale.</summary>
        [Test]
        public void Simple_Scale_deserialises_to_IScale()
        {
            const string json = "{\"id\":\"sm\",\"mediaType\":\"STL\","
                + "\"Scale\":[2,2,2]}";

            var wire = JsonRoundTripHelper.Deserialize<JsonSolidModel>(json)!;
            var sm = wire.ToSolidModel();

            Assert.That(sm.Scale, Is.InstanceOf<IScale>());
            Assert.That(((IScale)sm.Scale).Value, Is.EqualTo("2 2 2"));
        }

        // ---------------- positive: ScaleDataSet ----------------

        /// <summary>Pins the behaviour expressed by the test name: scale data set serialises as flat pascalcase object.</summary>
        [Test]
        public void ScaleDataSet_serialises_as_flat_pascalcase_object()
        {
            var sm = new SolidModel
            {
                Id = "sm2",
                MediaType = MediaType.OBJ,
                Scale = new ScaleDataSet { X = 1.5, Y = 2.5, Z = 3.5 }
            };

            var json = JsonRoundTripHelper.Serialize(new JsonSolidModel(sm));

            Assert.That(json, Does.Contain("\"ScaleDataSet\":{\"X\":1.5,\"Y\":2.5,\"Z\":3.5}"));
        }

        /// <summary>Pins the behaviour expressed by the test name: scale data set deserialises to i scale data set.</summary>
        [Test]
        public void ScaleDataSet_deserialises_to_IScaleDataSet()
        {
            const string json = "{\"id\":\"sm\",\"mediaType\":\"STL\","
                + "\"ScaleDataSet\":{\"X\":1.5,\"Y\":2.5,\"Z\":3.5}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonSolidModel>(json)!;
            var sm = wire.ToSolidModel();

            Assert.That(sm.Scale, Is.InstanceOf<IScaleDataSet>());
            var ds = (IScaleDataSet)sm.Scale;
            Assert.That(ds.X, Is.EqualTo(1.5));
            Assert.That(ds.Y, Is.EqualTo(2.5));
            Assert.That(ds.Z, Is.EqualTo(3.5));
        }

        // ---------------- negative ----------------

        /// <summary>Pins the behaviour expressed by the test name: both scale and scale data set present data set wins.</summary>
        [Test]
        public void Both_Scale_and_ScaleDataSet_present_DataSet_wins()
        {
            const string json = "{\"id\":\"sm\",\"mediaType\":\"STL\","
                + "\"Scale\":[1,1,1],"
                + "\"ScaleDataSet\":{\"X\":9}}";

            var wire = JsonRoundTripHelper.Deserialize<JsonSolidModel>(json)!;
            var sm = wire.ToSolidModel();

            Assert.That(sm.Scale, Is.InstanceOf<IScaleDataSet>());
            Assert.That(((IScaleDataSet)sm.Scale).X, Is.EqualTo(9.0));
        }

        /// <summary>Pins the behaviour expressed by the test name: null scale property emits neither field.</summary>
        [Test]
        public void Null_scale_property_emits_neither_field()
        {
            var sm = new SolidModel
            {
                Id = "sm3",
                MediaType = MediaType.STL,
                Scale = null
            };

            var json = JsonRoundTripHelper.Serialize(new JsonSolidModel(sm));

            Assert.That(json, Does.Not.Contain("\"Scale\""));
            Assert.That(json, Does.Not.Contain("ScaleDataSet"));
        }

        /// <summary>Pins the behaviour expressed by the test name: json scale data set default constructor yields zero components.</summary>
        [Test]
        public void JsonScaleDataSet_default_constructor_yields_zero_components()
        {
            var js = new JsonScaleDataSet();
            var ds = js.ToScaleDataSet();

            Assert.That(ds.X, Is.EqualTo(0.0));
        }

        /// <summary>Pins the behaviour expressed by the test name: json scale data set ctor with null input keeps default values.</summary>
        [Test]
        public void JsonScaleDataSet_ctor_with_null_input_keeps_default_values()
        {
            var js = new JsonScaleDataSet(null!);
            Assert.That(js.X, Is.EqualTo(0.0));
        }

        /// <summary>Pins the behaviour expressed by the test name: null solid model passed to ctor keeps default values.</summary>
        [Test]
        public void Null_solidModel_passed_to_ctor_keeps_default_values()
        {
            var jsm = new JsonSolidModel(null!);
            Assert.That(jsm.Scale, Is.Null);
            Assert.That(jsm.ScaleDataSet, Is.Null);
        }
    }
}
