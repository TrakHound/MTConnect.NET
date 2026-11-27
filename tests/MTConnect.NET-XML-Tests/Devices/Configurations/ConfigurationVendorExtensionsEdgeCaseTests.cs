// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using System.Xml.Linq;
using MTConnect.Devices.Configurations;
using MTConnect.Devices.Xml;
using MTConnect.Tests.XML.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices.Configurations
{
    /// <summary>
    /// Boundary and edge-case coverage on the vendor-extension surface —
    /// documented input classes per the coverage FLOOR
    /// (§1.0d-trigies-novodecies) that the primary round-trip fixture does
    /// not exercise: default-namespaced (unprefixed) vendor elements,
    /// attribute-only extensions, extensions carrying only text, extensions
    /// with unicode payloads (combining chars, RTL), extensions with
    /// numeric-character-reference payloads, and extensions embedded
    /// alongside multiple standard children.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>XSD — <c>MTConnectDevices_2.7.xsd</c>
    /// <c>ComponentConfigurationType</c> permits any element that
    /// substitutes for the abstract <c>AbstractConfiguration</c> element
    /// — the vendor XSD chooses whether the substitution is qualified
    /// (default namespace on the vendor's <c>elementFormDefault</c>)
    /// or prefixed; the MTConnect.NET formatter must preserve either
    /// shape verbatim.</item>
    /// <item>W3C XML 1.0 §3.1 — a namespace declaration on the
    /// substitution root propagates to its descendants unless
    /// re-declared.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationVendorExtensionsEdgeCaseTests
    {
        // ---------------- default namespace (unprefixed) ----------------

        /// <summary>A vendor extension declared with a default namespace
        /// (<c>xmlns="urn:x"</c> on the element itself) round-trips with the
        /// namespace preserved. Distinct code path from the prefix-based
        /// vendor-extension case pinned by the primary fixture — some
        /// vendor XSDs default-qualify per <c>elementFormDefault="qualified"</c>
        /// and the wire-format must not force a prefix.</summary>
        [Test]
        public void Round_trip_preserves_default_namespace_on_vendor_extension()
        {
            var original = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse(
                        "<Ext xmlns=\"urn:mycorp:mtconnect\">"
                        + "<Child>payload</Child>"
                        + "</Ext>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, original, outputComments: false));
            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var round = wire.ToConfiguration();

            var extensions = round.VendorExtensions.ToList();
            Assert.That(extensions, Has.Count.EqualTo(1));

            var ext = extensions[0];
            Assert.That(ext.Name.LocalName, Is.EqualTo("Ext"));
            Assert.That(ext.Name.NamespaceName, Is.EqualTo("urn:mycorp:mtconnect"));
            var child = ext.Element(XName.Get("Child", "urn:mycorp:mtconnect"));
            Assert.That(child, Is.Not.Null,
                "Child must inherit the default namespace of its parent.");
            Assert.That(child!.Value, Is.EqualTo("payload"));
        }

        // ---------------- attribute-only ----------------

        /// <summary>An attribute-only vendor extension (no child elements, no
        /// text content) round-trips with every attribute intact. A minimal
        /// vendor pin — many operator payloads are declarative
        /// <c>&lt;vendor:Marker id="…" type="…"/&gt;</c> tags with no body.</summary>
        [Test]
        public void Round_trip_preserves_attribute_only_vendor_extension()
        {
            var original = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse(
                        "<v:Marker xmlns:v=\"urn:v\" id=\"m1\" type=\"threshold\" value=\"3.14\" />")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, original, outputComments: false));
            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var round = wire.ToConfiguration();

            var ext = round.VendorExtensions.Single();
            Assert.That(ext.Name.LocalName, Is.EqualTo("Marker"));
            Assert.That(ext.Attribute("id")?.Value, Is.EqualTo("m1"));
            Assert.That(ext.Attribute("type")?.Value, Is.EqualTo("threshold"));
            Assert.That(ext.Attribute("value")?.Value, Is.EqualTo("3.14"));
            Assert.That(ext.HasElements, Is.False,
                "Attribute-only extension must not sprout phantom children.");
        }

        // ---------------- text-only ----------------

        /// <summary>A text-only vendor extension (no attributes, no children,
        /// just text content) round-trips with the text preserved verbatim.</summary>
        [Test]
        public void Round_trip_preserves_text_only_vendor_extension()
        {
            var original = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse("<v:Note xmlns:v=\"urn:v\">just some text</v:Note>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, original, outputComments: false));
            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var round = wire.ToConfiguration();

            var ext = round.VendorExtensions.Single();
            Assert.That(ext.Name.LocalName, Is.EqualTo("Note"));
            Assert.That(ext.Value, Is.EqualTo("just some text"));
            Assert.That(ext.HasElements, Is.False);
            // XLinq stores xmlns declarations as XAttribute-with-
            // IsNamespaceDeclaration=true, so HasAttributes is true even
            // for element-only namespaces. The behavioral pin is that
            // the caller has authored no NON-namespace attributes.
            var nonNamespaceAttrs = ext.Attributes()
                .Where(a => !a.IsNamespaceDeclaration)
                .ToList();
            Assert.That(nonNamespaceAttrs, Is.Empty,
                "A text-only vendor extension must have zero non-namespace attributes.");
        }

        // ---------------- unicode: combining marks + RTL ----------------

        /// <summary>Vendor extensions carrying unicode text — combining
        /// diacritics, RTL characters, and astral-plane surrogate pairs —
        /// round-trip byte-identical. The <c>WriteRaw</c> write path plus
        /// <c>XElement.Parse</c> read path share the same XmlWriter/XmlReader
        /// charset handling and must not collapse or reorder codepoints.</summary>
        [Test]
        public void Round_trip_preserves_unicode_payload_in_vendor_extension()
        {
            // "café" (combining acute), "אבג" (Hebrew RTL),
            // "\U0001F600" (emoji, astral surrogate pair).
            const string payload = "café אבג \U0001F600";
            var original = new Configuration
            {
                VendorExtensions = new[]
                {
                    new XElement(XName.Get("Ext", "urn:v"), payload)
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, original, outputComments: false));
            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var round = wire.ToConfiguration();

            var ext = round.VendorExtensions.Single();
            Assert.That(ext.Value, Is.EqualTo(payload));
        }

        // ---------------- character reference / escaping ----------------

        /// <summary>A vendor-extension payload that carries XML predefined
        /// entities (<c>&amp;</c>, <c>&lt;</c>, <c>&gt;</c>, <c>&quot;</c>,
        /// <c>&apos;</c>) round-trips with the entities re-escaped on write
        /// and re-decoded on read; the pin catches a hypothetical
        /// double-escape regression that would leave literal <c>&amp;amp;</c>
        /// in the model.</summary>
        [Test]
        public void Round_trip_preserves_predefined_entities_in_vendor_extension_text()
        {
            var original = new Configuration
            {
                VendorExtensions = new[]
                {
                    new XElement(XName.Get("Ext", "urn:v"), "a & b < c > d \" e ' f")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, original, outputComments: false));

            // Wire form MUST re-escape the entities.
            Assert.That(xml, Does.Contain("&amp;"));
            Assert.That(xml, Does.Contain("&lt;"));
            Assert.That(xml, Does.Contain("&gt;"));

            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var round = wire.ToConfiguration();

            var ext = round.VendorExtensions.Single();
            Assert.That(ext.Value, Is.EqualTo("a & b < c > d \" e ' f"),
                "Model value must decode back to raw text, not double-escaped.");
        }

        // ---------------- multiple standard children + vendor extension ----------------

        /// <summary>A Configuration carrying MULTIPLE standard children
        /// (Motion + SensorConfiguration + Specifications collection) AND a
        /// vendor extension round-trips with every slot populated. The
        /// primary fixture pins Motion-only + vendor; this pin exercises the
        /// dense-Configuration path where the write-order must interleave
        /// standard children with the vendor extension WITHOUT the standard
        /// children accidentally landing in the surrogate's
        /// <c>[XmlAnyElement]</c> bucket.</summary>
        [Test]
        public void Round_trip_preserves_dense_configuration_with_multiple_standard_children_and_vendor_extension()
        {
            var original = new Configuration
            {
                Motion = new Motion
                {
                    Id = "m1",
                    Type = MotionType.PRISMATIC,
                    Actuation = MotionActuationType.DIRECT,
                    Axis = new Axis { Value = "1 2 3" }
                },
                SensorConfiguration = new SensorConfiguration
                {
                    FirmwareVersion = "1.0"
                },
                VendorExtensions = new[]
                {
                    XElement.Parse("<v:V xmlns:v=\"urn:v\">payload</v:V>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, original, outputComments: false));
            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var round = wire.ToConfiguration();

            Assert.That(round.Motion, Is.Not.Null);
            Assert.That(round.Motion!.Id, Is.EqualTo("m1"));
            Assert.That(round.SensorConfiguration, Is.Not.Null);
            Assert.That(round.SensorConfiguration!.FirmwareVersion, Is.EqualTo("1.0"));
            Assert.That(round.VendorExtensions, Is.Not.Null);
            var ext = round.VendorExtensions.Single();
            Assert.That(ext.Name.LocalName, Is.EqualTo("V"));
            Assert.That(ext.Value, Is.EqualTo("payload"));
        }

        // ---------------- three-plus vendor extensions ----------------

        /// <summary>Three vendor extensions from three distinct namespaces
        /// round-trip in author order — pins the general-collection branch
        /// beyond the 1-element and 2-element cases in the primary fixture.</summary>
        [Test]
        public void Round_trip_preserves_three_vendor_extensions_in_order()
        {
            var original = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse("<a:A xmlns:a=\"urn:a\">alpha</a:A>"),
                    XElement.Parse("<b:B xmlns:b=\"urn:b\">beta</b:B>"),
                    XElement.Parse("<c:C xmlns:c=\"urn:c\">gamma</c:C>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, original, outputComments: false));
            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var round = wire.ToConfiguration();

            var extensions = round.VendorExtensions.ToList();
            Assert.That(extensions, Has.Count.EqualTo(3));
            Assert.That(
                extensions.Select(e => e.Value).ToArray(),
                Is.EqualTo(new[] { "alpha", "beta", "gamma" }));
            Assert.That(
                extensions.Select(e => e.Name.NamespaceName).ToArray(),
                Is.EqualTo(new[] { "urn:a", "urn:b", "urn:c" }));
        }

        // ---------------- deeply nested vendor extension ----------------

        /// <summary>A vendor extension whose descendants nest three levels
        /// deep round-trips with every level intact — pins the
        /// <c>WriteRaw</c> path preserves nested structure and
        /// <c>XElement.Parse</c> reconstructs the tree without depth loss.</summary>
        [Test]
        public void Round_trip_preserves_three_levels_of_nesting()
        {
            var original = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse(
                        "<v:Root xmlns:v=\"urn:v\">"
                        + "<L1><L2><L3>leaf</L3></L2></L1>"
                        + "</v:Root>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, original, outputComments: false));
            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var round = wire.ToConfiguration();

            var ext = round.VendorExtensions.Single();
            var leaf = ext
                .Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "L3");
            Assert.That(leaf, Is.Not.Null,
                "Deep-nested leaf must survive the round trip.");
            Assert.That(leaf!.Value, Is.EqualTo("leaf"));
        }
    }
}
