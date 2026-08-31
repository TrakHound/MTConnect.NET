// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MTConnect.Devices.Configurations;
using MTConnect.Devices.Xml;
using MTConnect.Tests.XML.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices.Configurations
{
    /// <summary>
    /// Pins the branches of <see cref="XmlConfiguration.WriteXml"/> and
    /// <see cref="XmlConfiguration.ToConfiguration"/> that the primary
    /// vendor-extension round-trip fixture does NOT exercise:
    /// <list type="bullet">
    /// <item>the outer <c>if (configuration != null)</c> false branch on the
    /// write path — a null <see cref="IConfiguration"/> must produce no
    /// output at all, not an empty <c>&lt;Configuration/&gt;</c> shell;</item>
    /// <item>the <paramref name="outputComments"/> true branch — the
    /// explanatory comment must precede the <c>&lt;Configuration&gt;</c>
    /// element with the standard's declared description;</item>
    /// <item>the <c>VendorExtensions.Length &gt; 0</c> read-side guard on an
    /// explicitly zero-length <see cref="XmlElement"/>[] array (distinct
    /// from the null-array branch);</item>
    /// <item>the inner <c>extensions.Count &gt; 0</c> read-side guard when
    /// the surrogate array contains only null entries — the projected list
    /// stays empty and the model must NOT expose an empty
    /// <see cref="IConfiguration.VendorExtensions"/> enumerable;</item>
    /// <item>the read-side mixed valid+null case — the write-side null-skip
    /// is exercised by the primary fixture, but the ToConfiguration
    /// null-skip on the read side is not.</item>
    /// </list>
    /// Coverage-FLOOR §1.0d-trigies-novodecies: every branch on the surface
    /// under test has an explicit pin.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>XSD — <c>MTConnectDevices_2.7.xsd</c>
    /// <c>ComponentConfigurationType</c>.</item>
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// UML class <c>Configuration</c>.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class XmlConfigurationBranchCoverageTests
    {
        // ---------------- WriteXml — null-configuration guard ----------------

        /// <summary>Pins the outer null-guard on <see cref="XmlConfiguration.WriteXml"/>
        /// — a null <see cref="IConfiguration"/> must produce zero output, not
        /// even an empty <c>&lt;Configuration /&gt;</c> shell.</summary>
        [Test]
        public void WriteXml_with_null_configuration_produces_no_output()
        {
            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, configuration: null, outputComments: false));

            Assert.That(xml, Is.EqualTo(string.Empty));
        }

        /// <summary>Pins the outer null-guard covers the comment path too — a
        /// null configuration with <c>outputComments: true</c> must NOT emit a
        /// stray comment either, because the guard wraps both branches.</summary>
        [Test]
        public void WriteXml_with_null_configuration_and_outputComments_true_still_produces_no_output()
        {
            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, configuration: null, outputComments: true));

            Assert.That(xml, Is.EqualTo(string.Empty));
        }

        // ---------------- WriteXml — outputComments: true ----------------

        /// <summary>Pins the <c>outputComments: true</c> branch — the standard's
        /// declared description of a Configuration precedes the
        /// <c>&lt;Configuration&gt;</c> element as an XML comment.</summary>
        [Test]
        public void WriteXml_with_outputComments_true_precedes_element_with_description_comment()
        {
            var configuration = new Configuration
            {
                VendorExtensions = new[]
                {
                    XElement.Parse("<v:V xmlns:v=\"urn:v\">payload</v:V>")
                }
            };

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, configuration, outputComments: true));

            Assert.That(xml, Does.StartWith("<!--"),
                "outputComments=true must emit a comment before <Configuration>.");
            Assert.That(xml, Does.Contain("Configuration : "),
                "The comment must carry the 'Configuration : ' prefix declared by the writer.");
            Assert.That(xml, Does.Contain(Configuration.DescriptionText),
                "The comment must carry the standard-declared DescriptionText.");
            var commentEnd = xml.IndexOf("-->", System.StringComparison.Ordinal);
            var elemStart = xml.IndexOf("<Configuration", System.StringComparison.Ordinal);
            Assert.That(commentEnd, Is.GreaterThan(-1), "Comment must be closed.");
            Assert.That(elemStart, Is.GreaterThan(commentEnd),
                "The <Configuration> element must appear AFTER the comment.");
            Assert.That(xml, Does.Contain("<v:V xmlns:v=\"urn:v\">payload</v:V>"),
                "The comment path must not disturb vendor-extension serialization.");
        }

        /// <summary>Comment path with a bare configuration (no vendor extensions,
        /// no standard children) still emits the description comment and a
        /// well-formed empty configuration element.</summary>
        [Test]
        public void WriteXml_with_outputComments_true_on_bare_configuration_emits_comment_and_empty_element()
        {
            var configuration = new Configuration();

            var xml = XmlRoundTripHelper.Write(w =>
                XmlConfiguration.WriteXml(w, configuration, outputComments: true));

            Assert.That(xml, Does.StartWith("<!--"));
            Assert.That(xml, Does.Contain(Configuration.DescriptionText));
            Assert.That(
                xml,
                Does.Contain("<Configuration />").Or.Contain("<Configuration></Configuration>"));
        }

        // ---------------- ToConfiguration — zero-length array guard ----------------

        /// <summary>An explicitly zero-length <see cref="XmlElement"/>[] on the
        /// surrogate MUST short-circuit the projection loop via the
        /// <c>Length &gt; 0</c> guard, leaving <see cref="IConfiguration.VendorExtensions"/>
        /// null on the model — distinct from the null-array branch already
        /// covered by <c>Configuration_with_only_standard_children_has_null_VendorExtensions_on_read</c>.</summary>
        [Test]
        public void ToConfiguration_treats_zero_length_VendorExtensions_array_as_absent()
        {
            var wire = new XmlConfiguration
            {
                VendorExtensions = System.Array.Empty<XmlElement>()
            };

            var configuration = wire.ToConfiguration();

            Assert.That(configuration.VendorExtensions, Is.Null,
                "A zero-length surrogate array must not project onto a non-null model collection.");
        }

        // ---------------- ToConfiguration — inner Count > 0 guard ----------------

        /// <summary>A surrogate array populated entirely with null entries
        /// leaves the projected list empty; the inner
        /// <c>if (extensions.Count &gt; 0)</c> guard must keep
        /// <see cref="IConfiguration.VendorExtensions"/> null on the model, so
        /// downstream consumers see the "no vendor extensions" contract, not
        /// an empty enumerable that suggests the caller supplied zero.</summary>
        [Test]
        public void ToConfiguration_ignores_all_null_entries_and_leaves_VendorExtensions_null()
        {
            var wire = new XmlConfiguration
            {
                VendorExtensions = new XmlElement[] { null!, null! }
            };

            var configuration = wire.ToConfiguration();

            Assert.That(configuration.VendorExtensions, Is.Null,
                "An all-null surrogate array must not project onto an empty non-null model collection.");
        }

        // ---------------- ToConfiguration — mixed valid + null read path ----------------

        /// <summary>A surrogate array with valid entries interleaved with
        /// nulls projects onto the model with the null slots dropped and the
        /// valid entries preserved in author order. The write-side null-skip
        /// is pinned by the primary fixture; this pins the READ-side mirror.</summary>
        [Test]
        public void ToConfiguration_captures_valid_and_ignores_null_on_read()
        {
            var doc = new XmlDocument();
            var first = doc.CreateElement("a", "First", "urn:a");
            first.InnerText = "one";
            var second = doc.CreateElement("b", "Second", "urn:b");
            second.InnerText = "two";

            var wire = new XmlConfiguration
            {
                VendorExtensions = new XmlElement[] { first, null!, second }
            };

            var configuration = wire.ToConfiguration();

            Assert.That(configuration.VendorExtensions, Is.Not.Null);
            var extensions = configuration.VendorExtensions.ToList();
            Assert.That(extensions, Has.Count.EqualTo(2),
                "Exactly two extensions must project — the middle null slot is skipped.");
            Assert.That(extensions[0].Name.LocalName, Is.EqualTo("First"));
            Assert.That(extensions[0].Value, Is.EqualTo("one"));
            Assert.That(extensions[1].Name.LocalName, Is.EqualTo("Second"));
            Assert.That(extensions[1].Value, Is.EqualTo("two"));
        }
    }
}
