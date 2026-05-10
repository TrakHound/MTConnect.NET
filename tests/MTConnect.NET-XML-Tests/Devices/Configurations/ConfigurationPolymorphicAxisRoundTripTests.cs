// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Linq;
using MTConnect.Devices.Configurations;
using MTConnect.Devices.Xml;
using MTConnect.Tests.XML.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices.Configurations
{
    /// <summary>
    /// Pins the <c>&lt;Motion&gt;</c> Axis polymorphism in MTConnect v2.7's
    /// XML wire shape: <c>&lt;Axis&gt;</c> (simple ThreeSpaceValueType text)
    /// vs <c>&lt;AxisDataSet&gt;</c> (XYZ Entry-keyed dataset). Both are
    /// alternatives in a single <c>xs:choice</c>.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>AbstractAxis</c>, <c>Axis</c>, <c>AxisDataSet</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (elements <c>&lt;Axis&gt;</c>, <c>&lt;AxisDataSet&gt;</c> in
    /// <c>MotionType</c>).</item>
    /// <item>Prose — <see href="https://docs.mtconnect.org/"/> Part 2 (Devices)
    /// section on Configuration / Motion.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicAxisRoundTripTests
    {
        // ---------------- positive: simple Axis ----------------

        [Test]
        public void Simple_Axis_serialises_to_text_element()
        {
            var motion = new Motion
            {
                Id = "m1",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Axis = new Axis { Value = "1 2 3" }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlMotion.WriteXml(w, motion));

            Assert.That(xml, Does.Contain("<Axis>1 2 3</Axis>"));
            Assert.That(xml, Does.Not.Contain("<AxisDataSet"));
        }

        [Test]
        public void Simple_Axis_deserialises_to_IAxis()
        {
            const string xml = "<Motion id=\"m1\" type=\"PRISMATIC\" actuation=\"DIRECT\">"
                + "<Axis>4 5 6</Axis></Motion>";

            var wire = XmlRoundTripHelper.Read<XmlMotion>(xml);
            var motion = wire.ToMotion();

            Assert.That(motion.Axis, Is.InstanceOf<IAxis>());
            Assert.That(motion.Axis, Is.Not.InstanceOf<IAxisDataSet>());
            var simple = (IAxis)motion.Axis;
            Assert.That(simple.Value, Is.EqualTo("4 5 6"));
        }

        // ---------------- positive: AxisDataSet ----------------

        [Test]
        public void AxisDataSet_serialises_to_keyed_entries()
        {
            var motion = new Motion
            {
                Id = "m2",
                Type = MotionType.REVOLUTE,
                Actuation = MotionActuationType.VIRTUAL,
                Axis = new AxisDataSet { X = 1.0, Y = 2.0, Z = 3.0 }
            };

            var xml = XmlRoundTripHelper.Write(w => XmlMotion.WriteXml(w, motion));

            Assert.That(xml, Does.Contain("<AxisDataSet>"));
            Assert.That(xml, Does.Contain("<Entry key=\"X\">1</Entry>"));
            Assert.That(xml, Does.Contain("<Entry key=\"Y\">2</Entry>"));
            Assert.That(xml, Does.Contain("<Entry key=\"Z\">3</Entry>"));
            Assert.That(xml, Does.Not.Contain("<Axis>"));
        }

        [Test]
        public void AxisDataSet_deserialises_to_IAxisDataSet()
        {
            const string xml = "<Motion id=\"m2\" type=\"REVOLUTE\" actuation=\"DIRECT\">"
                + "<AxisDataSet>"
                + "<Entry key=\"X\">7</Entry>"
                + "<Entry key=\"Y\">8</Entry>"
                + "<Entry key=\"Z\">9</Entry>"
                + "</AxisDataSet></Motion>";

            var wire = XmlRoundTripHelper.Read<XmlMotion>(xml);
            var motion = wire.ToMotion();

            Assert.That(motion.Axis, Is.InstanceOf<IAxisDataSet>());
            var ds = (IAxisDataSet)motion.Axis;
            Assert.That(ds.X, Is.EqualTo(7.0));
            Assert.That(ds.Y, Is.EqualTo(8.0));
            Assert.That(ds.Z, Is.EqualTo(9.0));
        }

        // ---------------- positive: XSD validation ----------------
        // The v2.7 MTConnectDevices XSD uses XSD 1.1 features (notNamespace)
        // that .NET's XmlSchema (XSD 1.0) cannot load. These tests live in
        // the XsdLoadStrict category so they participate in the opt-in
        // strict-load sweep once an XSD 1.1 validator wires in.

        [Test]
        [Category("XsdLoadStrict")]
        public void Simple_Axis_inside_devices_envelope_is_xsd_valid()
        {
            var envelope = MotionEnvelope("<Axis>1 2 3</Axis>");
            var xsd = XsdValidationHelper.GetSchemaPath("2.7", "Devices");
            Assume.That(File.Exists(xsd), $"XSD missing: {xsd}");

            var errors = XsdValidationHelper.Validate(envelope, xsd);

            Assert.That(errors, Is.Empty,
                "XSD validation produced errors:\n  - " + string.Join("\n  - ", errors));
        }

        [Test]
        [Category("XsdLoadStrict")]
        public void AxisDataSet_inside_devices_envelope_is_xsd_valid()
        {
            var envelope = MotionEnvelope(
                "<AxisDataSet>"
                + "<Entry key=\"X\">1</Entry>"
                + "<Entry key=\"Y\">2</Entry>"
                + "<Entry key=\"Z\">3</Entry>"
                + "</AxisDataSet>");
            var xsd = XsdValidationHelper.GetSchemaPath("2.7", "Devices");
            Assume.That(File.Exists(xsd), $"XSD missing: {xsd}");

            var errors = XsdValidationHelper.Validate(envelope, xsd);

            Assert.That(errors, Is.Empty,
                "XSD validation produced errors:\n  - " + string.Join("\n  - ", errors));
        }

        // ---------------- negative ----------------

        [Test]
        public void AxisDataSet_with_illegal_W_key_is_dropped_and_does_not_corrupt_xyz()
        {
            // ThreeDimensionalEntryType keys are X|Y|Z; an Entry@key='W' is
            // outside the spec-defined KeyType. The deserialiser must not
            // accept it as X/Y/Z and must not blow up on it.
            const string xml = "<AxisDataSet>"
                + "<Entry key=\"W\">99</Entry>"
                + "<Entry key=\"X\">1</Entry>"
                + "</AxisDataSet>";

            var wire = XmlRoundTripHelper.Read<XmlAxisDataSet>(xml);
            var ds = wire.ToAxisDataSet();

            Assert.That(ds.X, Is.EqualTo(1.0));
            Assert.That(ds.Y, Is.EqualTo(0.0));
            Assert.That(ds.Z, Is.EqualTo(0.0));
        }

        [Test]
        [Category("XsdLoadStrict")]
        public void AxisDataSet_with_W_key_inside_devices_envelope_fails_xsd()
        {
            // The XSD's KeyType enumeration {X|Y|Z|A|B|C} rejects 'W' at the
            // schema-validation layer.
            var envelope = MotionEnvelope(
                "<AxisDataSet>"
                + "<Entry key=\"W\">99</Entry>"
                + "</AxisDataSet>");
            var xsd = XsdValidationHelper.GetSchemaPath("2.7", "Devices");
            Assume.That(File.Exists(xsd), $"XSD missing: {xsd}");

            var errors = XsdValidationHelper.Validate(envelope, xsd);

            Assert.That(errors, Is.Not.Empty,
                "XSD must reject Entry@key='W' for an XYZ DataSet but produced no errors.");
            Assert.That(string.Join("\n", errors), Does.Contain("W").Or.Contain("KeyType"));
        }

        [Test]
        public void Empty_Axis_yields_empty_string_value()
        {
            const string xml = "<Axis></Axis>";

            var wire = XmlRoundTripHelper.Read<XmlAxis>(xml);
            var axis = wire.ToAxis();

            Assert.That(axis.Value, Is.Null.Or.Empty);
        }

        [Test]
        public void Null_axis_property_emits_no_Axis_element()
        {
            var motion = new Motion
            {
                Id = "m3",
                Type = MotionType.PRISMATIC,
                Actuation = MotionActuationType.DIRECT,
                Axis = null
            };

            var xml = XmlRoundTripHelper.Write(w => XmlMotion.WriteXml(w, motion));

            Assert.That(xml, Does.Not.Contain("<Axis"));
            Assert.That(xml, Does.Not.Contain("<AxisDataSet"));
        }

        [Test]
        public void Both_Axis_and_AxisDataSet_present_DataSet_wins()
        {
            // The spec's xs:choice rejects both being present; the deserialiser
            // accepts loosely (XmlSerializer fills both properties) and the
            // ToMotion() narrow prefers the DataSet form. Pin that ordering
            // so a future refactor cannot silently swap winners.
            const string xml = "<Motion id=\"m\" type=\"PRISMATIC\" actuation=\"DIRECT\">"
                + "<Axis>1 2 3</Axis>"
                + "<AxisDataSet>"
                + "<Entry key=\"X\">9</Entry>"
                + "</AxisDataSet></Motion>";

            var wire = XmlRoundTripHelper.Read<XmlMotion>(xml);
            var motion = wire.ToMotion();

            Assert.That(motion.Axis, Is.InstanceOf<IAxisDataSet>());
            Assert.That(((IAxisDataSet)motion.Axis).X, Is.EqualTo(9.0));
        }

        [Test]
        [Category("XsdLoadStrict")]
        public void Both_Axis_and_AxisDataSet_present_inside_envelope_fails_xsd()
        {
            // xs:choice (Axis | AxisDataSet) — having both inside <Motion>
            // must fail XSD validation per the v2.7 schema.
            var inner = "<Axis>1 2 3</Axis>"
                + "<AxisDataSet>"
                + "<Entry key=\"X\">1</Entry>"
                + "</AxisDataSet>";
            var envelope = MotionEnvelope(inner);
            var xsd = XsdValidationHelper.GetSchemaPath("2.7", "Devices");
            Assume.That(File.Exists(xsd), $"XSD missing: {xsd}");

            var errors = XsdValidationHelper.Validate(envelope, xsd);

            Assert.That(errors.Any(), Is.True,
                "XSD must reject both <Axis> and <AxisDataSet> in the same <Motion>.");
        }

        [Test]
        public void Null_motion_writes_nothing()
        {
            var xml = XmlRoundTripHelper.Write(w => XmlMotion.WriteXml(w, null));

            Assert.That(xml, Is.Empty.Or.EqualTo(string.Empty));
        }

        // Builds a minimum-viable v2.7 MTConnectDevices envelope around a
        // single Device whose Configuration carries a Motion sub-element. The
        // <inner> blob is spliced into the Motion's Axis-choice slot so a
        // single fixture can validate every shape (simple, dataset, both).
        private static string MotionEnvelope(string axisChoiceInner)
        {
            return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<MTConnectDevices xmlns=""urn:mtconnect.org:MTConnectDevices:2.7""
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xsi:schemaLocation=""urn:mtconnect.org:MTConnectDevices:2.7 MTConnectDevices_2.7.xsd"">
  <Header creationTime=""2026-01-01T00:00:00Z"" sender=""test"" instanceId=""1"" version=""2.7.0.0"" assetBufferSize=""1024"" assetCount=""0"" bufferSize=""8192""/>
  <Devices>
    <Device id=""d1"" name=""dev"" uuid=""uuid-1"">
      <DataItems>
        <DataItem id=""avail"" type=""AVAILABILITY"" category=""EVENT""/>
      </DataItems>
      <Components>
        <Axes id=""ax"" name=""Axes"">
          <Components>
            <Linear id=""x"" name=""X"">
              <Configuration>
                <Motion id=""mx"" type=""PRISMATIC"" actuation=""DIRECT"" coordinateSystemIdRef=""mc"">
                  " + axisChoiceInner + @"
                </Motion>
              </Configuration>
              <DataItems>
                <DataItem id=""xpos"" type=""POSITION"" category=""SAMPLE"" units=""MILLIMETER""/>
              </DataItems>
            </Linear>
          </Components>
        </Axes>
      </Components>
    </Device>
  </Devices>
</MTConnectDevices>";
        }
    }
}
