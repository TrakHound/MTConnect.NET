// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MTConnect.Devices.Configurations;
using MTConnect.Devices.Xml;
using MTConnect.Tests.XML.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices.Configurations
{
    /// <summary>
    /// Pins the vendor-extension round trip on
    /// <see cref="IConfiguration.VendorExtensions"/>. The MTConnect Standard's
    /// own vendor-extension mechanism for a component's Configuration is the
    /// XSD substitution group <c>AbstractConfiguration</c>: every standard
    /// child of <c>ComponentConfigurationType</c> (SensorConfiguration,
    /// Specifications, Relationships, CoordinateSystems, Motion, SolidModel,
    /// ImageFiles, PowerSources) is declared with
    /// <c>substitutionGroup='AbstractConfiguration'</c>, and vendors extend
    /// by publishing an XSD declaring a vendor-namespaced element that
    /// likewise substitutes for <c>AbstractConfiguration</c>. This surface
    /// carries those substitutions verbatim.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>XSD — <c>MTConnectDevices_2.7.xsd</c>
    /// <c>ComponentConfigurationType</c> declares
    /// <c>&lt;xs:element ref="AbstractConfiguration" minOccurs="0"
    /// maxOccurs="unbounded"/&gt;</c>; every standard child is a
    /// <c>substitutionGroup='AbstractConfiguration'</c> declaration.</item>
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML class <c>Configuration</c>).</item>
    /// <item>Prose — <see href="https://docs.mtconnect.org/"/> Part 2 (Devices)
    /// on Configuration and its extensibility.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class ConfigurationVendorExtensionsRoundTripTests
    {
        // ---------------- positive: emit ----------------

        /// <summary>Pins the behaviour expressed by the test name: single vendor extension serialises inside configuration element.</summary>
        [Test]
        public void Single_vendor_extension_serialises_inside_Configuration_element()
        {
            var configuration = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse(
                        "<mycorp:MyExtension xmlns:mycorp=\"urn:mycorp:mtconnect\">"
                        + "<Foo>bar</Foo>"
                        + "</mycorp:MyExtension>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, configuration, outputComments: false));

            Assert.That(xml, Does.StartWith("<Configuration>"));
            Assert.That(xml, Does.EndWith("</Configuration>"));
            Assert.That(xml, Does.Contain(
                "<mycorp:MyExtension xmlns:mycorp=\"urn:mycorp:mtconnect\">"));
            Assert.That(xml, Does.Contain("<Foo>bar</Foo>"));
            Assert.That(xml, Does.Contain("</mycorp:MyExtension>"));
        }

        /// <summary>Pins the behaviour expressed by the test name: multiple vendor extensions preserve order.</summary>
        [Test]
        public void Multiple_vendor_extensions_preserve_order()
        {
            var configuration = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse("<a:A xmlns:a=\"urn:a\">alpha</a:A>"),
                    XElement.Parse("<b:B xmlns:b=\"urn:b\">beta</b:B>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, configuration, outputComments: false));

            var aIndex = xml.IndexOf("<a:A", System.StringComparison.Ordinal);
            var bIndex = xml.IndexOf("<b:B", System.StringComparison.Ordinal);

            Assert.That(aIndex, Is.GreaterThanOrEqualTo(0), "First extension missing");
            Assert.That(bIndex, Is.GreaterThan(aIndex),
                "Second extension must serialise after the first to preserve author order");
        }

        /// <summary>Pins the behaviour expressed by the test name: vendor extensions serialise alongside standard children.</summary>
        [Test]
        public void Vendor_extensions_serialise_alongside_standard_children()
        {
            var configuration = new Configuration
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
                    XElement.Parse("<v:Vendor xmlns:v=\"urn:v\">payload</v:Vendor>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, configuration, outputComments: false));

            Assert.That(xml, Does.Contain("<Motion"));
            Assert.That(xml, Does.Contain("<v:Vendor xmlns:v=\"urn:v\">payload</v:Vendor>"));
        }

        // ---------------- positive: capture on read ----------------

        /// <summary>Pins the behaviour expressed by the test name: unrecognised child element is captured as vendor extension.</summary>
        [Test]
        public void Unrecognised_child_element_is_captured_as_VendorExtension()
        {
            const string xml =
                "<Configuration xmlns:mycorp=\"urn:mycorp:mtconnect\">"
                + "<mycorp:MyExtension><Payload>42</Payload></mycorp:MyExtension>"
                + "</Configuration>";

            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var configuration = wire.ToConfiguration();

            Assert.That(configuration.VendorExtensions, Is.Not.Null);
            var extensions = configuration.VendorExtensions.ToList();
            Assert.That(extensions, Has.Count.EqualTo(1));
            Assert.That(extensions[0].Name.LocalName, Is.EqualTo("MyExtension"));
            Assert.That(extensions[0].Name.NamespaceName, Is.EqualTo("urn:mycorp:mtconnect"));
            Assert.That(extensions[0].Element("Payload")?.Value, Is.EqualTo("42"));
        }

        /// <summary>Pins the behaviour expressed by the test name: full round trip preserves vendor extension content.</summary>
        [Test]
        public void Full_round_trip_preserves_vendor_extension_content()
        {
            var original = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse(
                        "<mycorp:Ext xmlns:mycorp=\"urn:mycorp\">"
                        + "<Foo attr=\"value\">child-text</Foo>"
                        + "</mycorp:Ext>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, original, outputComments: false));

            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var round = wire.ToConfiguration();

            Assert.That(round.VendorExtensions, Is.Not.Null);
            var extensions = round.VendorExtensions.ToList();
            Assert.That(extensions, Has.Count.EqualTo(1));

            var ext = extensions[0];
            Assert.That(ext.Name.LocalName, Is.EqualTo("Ext"));
            Assert.That(ext.Name.NamespaceName, Is.EqualTo("urn:mycorp"));

            var foo = ext.Element("Foo");
            Assert.That(foo, Is.Not.Null);
            Assert.That(foo!.Attribute("attr")?.Value, Is.EqualTo("value"));
            Assert.That(foo.Value, Is.EqualTo("child-text"));
        }

        /// <summary>Pins the behaviour expressed by the test name: two vendor extensions round trip correctly.</summary>
        [Test]
        public void Two_vendor_extensions_round_trip_correctly()
        {
            var original = new Configuration
            {
                VendorExtensions = new List<XElement>
                {
                    XElement.Parse("<a:First xmlns:a=\"urn:a\">one</a:First>"),
                    XElement.Parse("<b:Second xmlns:b=\"urn:b\">two</b:Second>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, original, outputComments: false));

            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var round = wire.ToConfiguration();

            var extensions = round.VendorExtensions.ToList();
            Assert.That(extensions, Has.Count.EqualTo(2));
            Assert.That(extensions[0].Value, Is.EqualTo("one"));
            Assert.That(extensions[1].Value, Is.EqualTo("two"));
            Assert.That(extensions[0].Name.LocalName, Is.EqualTo("First"));
            Assert.That(extensions[1].Name.LocalName, Is.EqualTo("Second"));
        }

        // ---------------- negative ----------------

        /// <summary>Pins the behaviour expressed by the test name: null vendor extensions emits no extra child.</summary>
        [Test]
        public void Null_VendorExtensions_emits_no_extra_child()
        {
            var configuration = new Configuration
            {
                VendorExtensions = null
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, configuration, outputComments: false));

            // XmlWriter collapses empty elements to the self-closing form
            // `<Configuration />` unless a child forces an expanded end tag —
            // either shape is a valid empty <Configuration> at the wire layer.
            Assert.That(
                xml,
                Is.EqualTo("<Configuration />").Or.EqualTo("<Configuration></Configuration>"));
        }

        /// <summary>Pins the behaviour expressed by the test name: empty vendor extensions collection emits no extra child.</summary>
        [Test]
        public void Empty_VendorExtensions_collection_emits_no_extra_child()
        {
            var configuration = new Configuration
            {
                VendorExtensions = System.Array.Empty<XElement>()
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, configuration, outputComments: false));

            // XmlWriter collapses empty elements to the self-closing form
            // `<Configuration />` unless a child forces an expanded end tag —
            // either shape is a valid empty <Configuration> at the wire layer.
            Assert.That(
                xml,
                Is.EqualTo("<Configuration />").Or.EqualTo("<Configuration></Configuration>"));
        }

        /// <summary>Pins the behaviour expressed by the test name: configuration with only standard children has null vendor extensions on read.</summary>
        [Test]
        public void Configuration_with_only_standard_children_has_null_VendorExtensions_on_read()
        {
            // A Configuration whose children are all standard ones bound to
            // strongly-typed slots must NOT accidentally capture them into
            // VendorExtensions — the [XmlAnyElement] attribute only fires on
            // elements the deserialiser has not otherwise bound.
            const string xml =
                "<Configuration>"
                + "<Motion id=\"m1\" type=\"PRISMATIC\" actuation=\"DIRECT\">"
                + "<Axis>1 2 3</Axis>"
                + "</Motion>"
                + "</Configuration>";

            var wire = XmlRoundTripHelper.Read<XmlConfiguration>(xml);
            var configuration = wire.ToConfiguration();

            Assert.That(configuration.VendorExtensions, Is.Null);
            Assert.That(configuration.Motion, Is.Not.Null);
        }
    }
}
