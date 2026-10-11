// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using MTConnect;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Configurations;
using MTConnect.Devices.Xml;
using MTConnect.Headers;
using MTConnect.Tests.XML.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices.Configurations
{
    /// <summary>
    /// Crown-jewel XSD-strict validation for
    /// <see cref="IConfiguration.VendorExtensions"/>: builds a
    /// <c>MTConnectDevices</c> probe envelope whose Linear component's
    /// <c>Configuration</c> carries a vendor-namespaced element (routed
    /// through the wire-format's <c>WriteRaw</c> path), loads the v2.7
    /// MTConnect Devices XSD alongside a synthetic vendor XSD that declares
    /// the vendor element with <c>substitutionGroup='mtc:AbstractConfiguration'</c>,
    /// and asserts XSD 1.0 validation passes with zero errors. Symmetric
    /// negative case: an unqualified (default-namespaced) foreign element
    /// that does NOT substitute into <c>AbstractConfiguration</c> must be
    /// rejected by the same schema set.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>XSD — <c>MTConnectDevices_2.7.xsd</c> line 4812 declares
    /// <c>&lt;xs:element abstract='true' name='AbstractConfiguration'
    /// type='AbstractConfigurationType'/&gt;</c>. Every standard child of
    /// <c>ComponentConfigurationType</c> (SensorConfiguration,
    /// Specifications, Relationships, CoordinateSystems, Motion,
    /// SolidModel, ImageFiles, PowerSources) is declared with
    /// <c>substitutionGroup='AbstractConfiguration'</c>. Vendor
    /// extensions substitute into the same slot.</item>
    /// <item>Prose — <see href="https://docs.mtconnect.org/"/> Part 2
    /// (Devices) on Configuration extensibility.</item>
    /// </list>
    /// The MTConnect v2.7 Devices XSD uses XSD 1.1 constructs
    /// (<c>notNamespace</c>, <c>maxOccurs&gt;1 on xs:all</c>) that the
    /// .NET BCL <c>XmlSchemaSet</c> rejects without preprocessing — this
    /// fixture routes through <see cref="XsdPreprocessor"/> the same way
    /// <see cref="XsdValidationHelper"/> does, and is tagged
    /// <c>[Category("XsdLoadStrict")]</c> for the opt-in strict-load
    /// sweep per §1.0d-trigies-nonies.
    /// </remarks>
    [TestFixture]
    public class ConfigurationVendorExtensionsXsdStrictValidationTests
    {
        private const string MtcDevicesNs = "urn:mtconnect.org:MTConnectDevices:2.7";
        private const string VendorNs = "urn:mycorp:mtconnect:vendor";

        /// <summary>Crown-jewel positive: a probe envelope emitted by the
        /// MTConnect.NET XML formatter with a vendor-namespaced element in a
        /// Configuration slot VALIDATES against the MTConnect v2.7 Devices
        /// XSD when a companion vendor XSD declares the element as a
        /// substitution of <c>AbstractConfiguration</c>. Pins the end-to-end
        /// XSD-strictness contract of the vendor-extension surface.</summary>
        [Test]
        [Category("XsdLoadStrict")]
        public void Emitted_probe_with_vendor_extension_validates_against_MTConnect_XSD_with_vendor_XSD()
        {
            // Prefix the Payload child too so it lands in the vendor
            // namespace — the vendor XSD declares elementFormDefault="qualified"
            // so unqualified children would be picked up by the ancestor
            // MTConnect default namespace and fail validation. Vendor
            // XSD authors either prefix every descendant or declare
            // xmlns="urn:vendor" on the root — this test pins the
            // prefixed form.
            var vendorElement = XElement.Parse(
                "<v:Ext xmlns:v=\"" + VendorNs + "\" vendorId=\"42\">"
                + "<v:Payload>hello</v:Payload>"
                + "</v:Ext>");

            var envelope = BuildProbeEnvelope(vendorElement);
            var vendorXsd = BuildVendorSubstitutionXsd();

            var errors = ValidateAgainstMTConnectPlusVendor(envelope, vendorXsd);

            Assert.That(errors, Is.Empty,
                "XSD-strict validation must pass for a vendor-namespaced element declared as substitutionGroup='AbstractConfiguration'. Errors:\n  - "
                + string.Join("\n  - ", errors));
        }

        /// <summary>Crown-jewel negative: an unqualified foreign element
        /// (no <c>substitutionGroup</c> declaration, no vendor namespace)
        /// injected into the same slot MUST be rejected — pinning that the
        /// XSD infrastructure is genuinely enforcing the abstract-
        /// substitution rule, not silently passing everything.</summary>
        [Test]
        [Category("XsdLoadStrict")]
        public void Emitted_probe_with_foreign_element_lacking_substitution_group_fails_MTConnect_XSD()
        {
            // Foreign element in the default (MTConnect) namespace, no
            // matching schema declaration. This is what would land in the
            // Configuration slot if a caller supplied a naked XElement with
            // no vendor namespace and no XSD declaration. It must fail.
            var foreign = new XElement("BogusUnknownElement", "payload");

            var envelope = BuildProbeEnvelope(foreign);

            var errors = ValidateAgainstMTConnectPlusVendor(
                envelope,
                vendorXsdSource: null);

            Assert.That(errors, Is.Not.Empty,
                "XSD-strict validation must REJECT a foreign element that lacks a substitutionGroup declaration.");
        }

        /// <summary>Structural pin — the emitted probe body actually
        /// contains the vendor element inside the Configuration envelope
        /// (guarding the crown-jewel test itself against an emit-time
        /// regression that would otherwise silently produce an empty
        /// envelope and let the XSD assertion vacuously pass).</summary>
        [Test]
        public void Emitted_probe_envelope_actually_contains_vendor_extension_element_inside_Configuration()
        {
            // Prefix the Payload child too so it lands in the vendor
            // namespace — the vendor XSD declares elementFormDefault="qualified"
            // so unqualified children would be picked up by the ancestor
            // MTConnect default namespace and fail validation. Vendor
            // XSD authors either prefix every descendant or declare
            // xmlns="urn:vendor" on the root — this test pins the
            // prefixed form.
            var vendorElement = XElement.Parse(
                "<v:Ext xmlns:v=\"" + VendorNs + "\" vendorId=\"42\">"
                + "<v:Payload>hello</v:Payload>"
                + "</v:Ext>");

            var envelope = BuildProbeEnvelope(vendorElement);
            var doc = XDocument.Parse(envelope);

            var configElements = doc
                .Descendants()
                .Where(e => e.Name.LocalName == "Configuration")
                .ToList();

            Assert.That(configElements, Is.Not.Empty,
                "Envelope must contain a <Configuration> element on the seeded Linear component.");

            var vendorInsideConfig = configElements
                .SelectMany(c => c.Elements())
                .FirstOrDefault(e => e.Name.LocalName == "Ext" && e.Name.NamespaceName == VendorNs);

            Assert.That(vendorInsideConfig, Is.Not.Null,
                "Envelope's <Configuration> must carry the vendor-namespaced <Ext> element as an immediate child (WriteRaw path).");
            Assert.That(
                vendorInsideConfig!.Attribute("vendorId")?.Value,
                Is.EqualTo("42"),
                "The vendor element's authored attribute must survive the emit path.");
            Assert.That(
                vendorInsideConfig.Element(XName.Get("Payload", VendorNs)),
                Is.Not.Null,
                "The vendor element's <v:Payload> child must survive the emit path in the vendor namespace.");
        }

        // ---------------- helpers ----------------

        // Constructs a minimum-viable v2.7 MTConnectDevices envelope by
        // routing a Device model whose Linear component's Configuration
        // carries the supplied vendor element through the strongly-typed
        // XML wire format. This exercises the same WriteRaw path as
        // production probe responses.
        private static string BuildProbeEnvelope(XElement vendorElement)
        {
            var device = new Device
            {
                Id = "d1",
                Name = "dev",
                Uuid = "uuid-1"
            };
            device.AddDataItem(new DataItem(DataItemCategory.EVENT, "AVAILABILITY", null, "avail"));

            var axes = new AxesComponent { Id = "ax", Name = "Axes" };
            var linear = new LinearComponent
            {
                Id = "x",
                Name = "X",
                Configuration = new Configuration
                {
                    VendorExtensions = new[] { vendorElement }
                }
            };
            linear.AddDataItem(new DataItem(DataItemCategory.SAMPLE, "POSITION", null, "xpos") { Units = "MILLIMETER" });
            axes.AddComponent(linear);
            device.AddComponent(axes);

            var document = new DevicesResponseDocument
            {
                Header = new MTConnectDevicesHeader
                {
                    InstanceId = 1,
                    Version = "2.7.0.0",
                    SchemaVersion = "2.7",
                    Sender = "vendor-ext-xsd-strict",
                    AssetBufferSize = 1024,
                    AssetCount = 0,
                    BufferSize = 8192,
                    DeviceModelChangeTime = "2026-01-01T00:00:00Z",
                    CreationTime = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
                },
                Devices = new IDevice[] { device },
                Version = new System.Version(2, 7, 0, 0)
            };

            // Use the production XML formatter so we exercise the same
            // WriteRaw path as an actual /probe response.
            var bytes = XmlDevicesResponseDocument.ToXmlBytes(document, indent: false, outputComments: false);
            Assert.That(bytes, Is.Not.Null, "XmlDevicesResponseDocument.ToXmlBytes returned null — envelope emit failed.");
            return Encoding.UTF8.GetString(bytes);
        }

        // Builds a companion vendor XSD in urn:mycorp:mtconnect:vendor
        // that imports the MTConnect Devices namespace and declares
        // <v:Ext> as substitutionGroup='mtc:AbstractConfiguration'. The
        // shape is the minimum needed to make an in-envelope <v:Ext>
        // schema-valid inside a <Configuration>.
        private static string BuildVendorSubstitutionXsd()
        {
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema""
           xmlns:v=""{VendorNs}""
           xmlns:mtc=""{MtcDevicesNs}""
           targetNamespace=""{VendorNs}""
           elementFormDefault=""qualified""
           attributeFormDefault=""unqualified"">
  <xs:import namespace=""{MtcDevicesNs}""/>
  <xs:complexType name=""ExtType"">
    <xs:complexContent>
      <xs:extension base=""mtc:AbstractConfigurationType"">
        <xs:sequence>
          <xs:element name=""Payload"" type=""xs:string"" minOccurs=""0""/>
        </xs:sequence>
        <xs:attribute name=""vendorId"" type=""xs:string""/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:element name=""Ext"" type=""v:ExtType"" substitutionGroup=""mtc:AbstractConfiguration""/>
</xs:schema>";
        }

        // Validates the envelope against the v2.7 MTConnect Devices XSD
        // (routed through XsdPreprocessor for XSD 1.1 stripping) plus the
        // supplied vendor XSD, if any. Uses the same defense-in-depth
        // reader settings as XsdValidationHelper (no external entity
        // resolution). Returns collected schema and validation errors.
        private static IReadOnlyList<string> ValidateAgainstMTConnectPlusVendor(string envelope, string vendorXsdSource)
        {
            var errors = new List<string>();

            var readerSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            };

            var schemaSet = new XmlSchemaSet
            {
                XmlResolver = null
            };
            schemaSet.ValidationEventHandler += (s, e) =>
                errors.Add($"[{e.Severity}] {e.Message}");

            // W3C xml.xsd + xlink.xsd — pre-load so the MTConnect XSD's
            // <xs:import namespace="…/xlink"> resolves by target-namespace.
            var mtcXsdPath = XsdValidationHelper.GetSchemaPath("2.7", "Devices");
            Assume.That(File.Exists(mtcXsdPath), $"MTConnect XSD missing: {mtcXsdPath}");
            var schemaVersionDir = Path.GetDirectoryName(mtcXsdPath);
            Assert.That(schemaVersionDir, Is.Not.Null, "Cannot derive schema-version directory.");
            var schemasRoot = Path.GetDirectoryName(schemaVersionDir);
            Assert.That(schemasRoot, Is.Not.Null, "Cannot derive Schemas root.");
            var w3cDir = Path.Combine(schemasRoot, "w3c");
            AddSchemaFromFile(schemaSet, readerSettings, Path.Combine(w3cDir, "xml.xsd"));
            AddSchemaFromFile(schemaSet, readerSettings, Path.Combine(w3cDir, "xlink.xsd"));

            // Preprocess the MTConnect XSD to strip XSD 1.1 constructs the
            // BCL cannot handle (same path as XsdValidationHelper).
            var preprocessedXsd = XsdPreprocessor.StripXsd11Constructs(File.ReadAllText(mtcXsdPath));
            using (var sr = new StringReader(preprocessedXsd))
            using (var xr = XmlReader.Create(sr, readerSettings))
            {
                schemaSet.Add(null, xr);
            }

            // Add the vendor XSD (if any) so its substitutionGroup
            // declaration binds into the compiled schema set.
            if (vendorXsdSource != null)
            {
                using var vr = new StringReader(vendorXsdSource);
                using var vxr = XmlReader.Create(vr, readerSettings);
                schemaSet.Add(VendorNs, vxr);
            }

            schemaSet.Compile();

            var validationSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null,
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet
            };
            validationSettings.Schemas.XmlResolver = null;
            validationSettings.ValidationEventHandler += (s, e) =>
                errors.Add($"[{e.Severity}] {e.Message}");

            using (var sr = new StringReader(envelope))
            using (var reader = XmlReader.Create(sr, validationSettings))
            {
                while (reader.Read())
                {
                    // drain
                }
            }

            return errors;
        }

        private static void AddSchemaFromFile(XmlSchemaSet schemaSet, XmlReaderSettings settings, string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = XmlReader.Create(fs, settings, path);
            schemaSet.Add(null, reader);
        }
    }
}
