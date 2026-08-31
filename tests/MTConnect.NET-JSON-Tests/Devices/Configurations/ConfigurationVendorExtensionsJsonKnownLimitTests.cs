// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using System.Xml.Linq;
using MTConnect.Devices.Configurations;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_JSON_Tests.Devices.Configurations
{
    /// <summary>
    /// Pins the CURRENT known limitation of the JSON formatter with respect
    /// to <see cref="IConfiguration.VendorExtensions"/>: the JSON surrogate
    /// <see cref="JsonConfiguration"/> does NOT carry a vendor-extension
    /// slot — it exists only on the XML wire format. A model instance
    /// serialized through the JSON formatter silently drops any
    /// <c>VendorExtensions</c> payload, and a round trip through the JSON
    /// formatter returns null in that slot. This is a deliberate limitation
    /// (JSON has no XSD substitution-group semantics; the MTConnect JSON
    /// shape does not spec a "raw XML" slot), and it is pinned here so any
    /// future change to that behavior must be a deliberate edit to these
    /// tests — never a silent regression.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>XSD — <c>MTConnectDevices_2.7.xsd</c>
    /// <c>ComponentConfigurationType</c> defines vendor extension via
    /// XSD substitution group <c>AbstractConfiguration</c>; that
    /// mechanism has no direct JSON analogue on the wire.</item>
    /// <item>Interface docblock —
    /// <see cref="IConfiguration.VendorExtensions"/> explicitly names the
    /// MTConnect.NET XML formatter as the surface that honors vendor
    /// extensions verbatim.</item>
    /// </list>
    /// If a future change adds a first-class JSON representation for
    /// vendor extensions (e.g. a <c>vendorExtensions</c> array carrying
    /// stringified XML), replace the negative assertions in this fixture
    /// with positive round-trip assertions and update the interface
    /// docblock in the same commit.
    /// </remarks>
    [TestFixture]
    public class ConfigurationVendorExtensionsJsonKnownLimitTests
    {
        /// <summary>The JSON surrogate CTOR silently drops
        /// <see cref="IConfiguration.VendorExtensions"/> — the serialized
        /// JSON must not carry a <c>vendorExtensions</c> key.</summary>
        [Test]
        public void JsonConfiguration_ctor_currently_drops_VendorExtensions()
        {
            var model = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse("<v:V xmlns:v=\"urn:v\">payload</v:V>")
                }
            };

            var wire = new JsonConfiguration(model);
            var json = JsonRoundTripHelper.Serialize(wire);

            Assert.That(json, Does.Not.Contain("vendorExtensions"),
                "The JSON surrogate has no vendor-extension slot today — regression alert if that changes.");
            Assert.That(json, Does.Not.Contain("payload"),
                "The extension payload must not leak into an unrelated JSON key.");
        }

        /// <summary>A round trip through the JSON formatter returns a model
        /// with <see cref="IConfiguration.VendorExtensions"/> null, because
        /// the wire form carries no vendor-extension slot.</summary>
        [Test]
        public void JsonConfiguration_round_trip_returns_null_VendorExtensions()
        {
            var model = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse("<v:V xmlns:v=\"urn:v\">payload</v:V>")
                }
            };

            var wire = new JsonConfiguration(model);
            var json = JsonRoundTripHelper.Serialize(wire);
            var back = JsonRoundTripHelper.Deserialize<JsonConfiguration>(json)!;
            var round = back.ToConfiguration();

            Assert.That(round.VendorExtensions, Is.Null,
                "The JSON round trip drops the vendor-extension payload — pinned known limitation.");
        }

        /// <summary>The <c>JsonConfiguration</c> surrogate type surface does
        /// not declare a public <c>VendorExtensions</c> property today —
        /// pins the type surface so a hypothetical addition surfaces as a
        /// test-file edit rather than a silent JSON-wire shape change.</summary>
        [Test]
        public void JsonConfiguration_surface_does_not_declare_VendorExtensions_property_today()
        {
            var property = typeof(JsonConfiguration).GetProperty("VendorExtensions");
            Assert.That(property, Is.Null,
                "JsonConfiguration must NOT declare a VendorExtensions slot until a first-class JSON representation is designed. "
                + "If you are intentionally adding one, update this test AND the IConfiguration.VendorExtensions docblock in the same commit.");
        }

        /// <summary>Standard children on the JSON surrogate continue to
        /// round-trip normally when VendorExtensions is set on the model —
        /// pins that the current drop does not disturb sibling slots.</summary>
        [Test]
        public void JsonConfiguration_preserves_standard_children_when_VendorExtensions_dropped()
        {
            var model = new Configuration
            {
                Motion = new Motion
                {
                    Id = "m1",
                    Type = MotionType.PRISMATIC,
                    Actuation = MotionActuationType.DIRECT,
                    Axis = new Axis { Value = "1 2 3" }
                },
                VendorExtensions = new[]
                {
                    XElement.Parse("<v:V xmlns:v=\"urn:v\">payload</v:V>")
                }
            };

            var wire = new JsonConfiguration(model);
            var json = JsonRoundTripHelper.Serialize(wire);

            Assert.That(json, Does.Contain("\"motion\":"),
                "Motion must serialize even when a dropped VendorExtensions payload is present on the model.");

            var back = JsonRoundTripHelper.Deserialize<JsonConfiguration>(json)!;
            var round = back.ToConfiguration();

            Assert.That(round.Motion, Is.Not.Null);
            Assert.That(round.Motion!.Id, Is.EqualTo("m1"));
            Assert.That(round.VendorExtensions, Is.Null);
        }

        /// <summary>Symmetry — a model whose only Configuration content is
        /// vendor extensions produces a bare JSON object (all standard
        /// slots omitted per <c>WhenWritingNull</c>).</summary>
        [Test]
        public void JsonConfiguration_from_vendor_extensions_only_model_produces_empty_object()
        {
            var model = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse("<v:V xmlns:v=\"urn:v\">payload</v:V>")
                }
            };

            var wire = new JsonConfiguration(model);
            var json = JsonRoundTripHelper.Serialize(wire);

            Assert.That(json, Is.EqualTo("{}"),
                "With only vendor extensions and no standard children, the JSON surrogate serializes to a bare object.");
        }
    }
}
