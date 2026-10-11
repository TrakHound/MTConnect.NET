// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// Pins the fix for the `MTConnectVersion.GetByNamespace` dispatch-chain
// omission: the switch capped out at `Namespaces.Version25.Match(ns)` and
// fell through to `return new Version()` (empty, "0.0") for any namespace
// declared by a document newer than v2.5 — even though `Namespaces.Version26`
// / `Namespaces.Version27` and `MTConnectVersions.Version26` /
// `MTConnectVersions.Version27` both already existed. A document declaring
// `urn:mtconnect.org:MTConnectStreams:2.7` (or `:2.6`) therefore resolved to
// an empty version instead of its real one.
//
// Fix (libraries/MTConnect.NET-XML/MTConnectVersion.cs):
//   - Prepended `Namespaces.Version27.Match(ns)` / `Namespaces.Version26.Match(ns)`
//     branches ahead of `Namespaces.Version25.Match(ns)`, so v2.6/v2.7
//     namespaces resolve highest-first, mirroring the existing pattern.
//   - Changed the fallback from `new Version()` to `MTConnectVersions.Max`
//     so an unrecognised namespace defaults to the latest supported release
//     rather than an empty version.
//
// `MTConnectVersion` and `Namespaces` are `internal` to
// MTConnect.NET-XML; this fixture reaches them via the assembly's
// `InternalsVisibleTo` grant to MTConnect.NET-XML-Tests.

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MTConnect.Tests.XML
{
    /// <summary>Pins the namespace-to-version dispatch chain in <see cref="MTConnectVersion.GetByNamespace"/>.</summary>
    [TestFixture]
    public class MTConnectVersionDispatchTests
    {
        /// <summary>Every currently-declared MTConnect Devices namespace paired with the version it must resolve to.</summary>
        /// <returns>The (namespace, expected version) pairs.</returns>
        public static IEnumerable<TestCaseData> DevicesNamespacesByVersion()
        {
            yield return new TestCaseData(Namespaces.Version10.Devices, MTConnectVersions.Version10).SetName("GetByNamespace_resolves_Devices_1_0");
            yield return new TestCaseData(Namespaces.Version11.Devices, MTConnectVersions.Version11).SetName("GetByNamespace_resolves_Devices_1_1");
            yield return new TestCaseData(Namespaces.Version12.Devices, MTConnectVersions.Version12).SetName("GetByNamespace_resolves_Devices_1_2");
            yield return new TestCaseData(Namespaces.Version13.Devices, MTConnectVersions.Version13).SetName("GetByNamespace_resolves_Devices_1_3");
            yield return new TestCaseData(Namespaces.Version14.Devices, MTConnectVersions.Version14).SetName("GetByNamespace_resolves_Devices_1_4");
            yield return new TestCaseData(Namespaces.Version15.Devices, MTConnectVersions.Version15).SetName("GetByNamespace_resolves_Devices_1_5");
            yield return new TestCaseData(Namespaces.Version16.Devices, MTConnectVersions.Version16).SetName("GetByNamespace_resolves_Devices_1_6");
            yield return new TestCaseData(Namespaces.Version17.Devices, MTConnectVersions.Version17).SetName("GetByNamespace_resolves_Devices_1_7");
            yield return new TestCaseData(Namespaces.Version18.Devices, MTConnectVersions.Version18).SetName("GetByNamespace_resolves_Devices_1_8");
            yield return new TestCaseData(Namespaces.Version20.Devices, MTConnectVersions.Version20).SetName("GetByNamespace_resolves_Devices_2_0");
            yield return new TestCaseData(Namespaces.Version21.Devices, MTConnectVersions.Version21).SetName("GetByNamespace_resolves_Devices_2_1");
            yield return new TestCaseData(Namespaces.Version22.Devices, MTConnectVersions.Version22).SetName("GetByNamespace_resolves_Devices_2_2");
            yield return new TestCaseData(Namespaces.Version23.Devices, MTConnectVersions.Version23).SetName("GetByNamespace_resolves_Devices_2_3");
            yield return new TestCaseData(Namespaces.Version24.Devices, MTConnectVersions.Version24).SetName("GetByNamespace_resolves_Devices_2_4");
            yield return new TestCaseData(Namespaces.Version25.Devices, MTConnectVersions.Version25).SetName("GetByNamespace_resolves_Devices_2_5");
            yield return new TestCaseData(Namespaces.Version26.Devices, MTConnectVersions.Version26).SetName("GetByNamespace_resolves_Devices_2_6");
            yield return new TestCaseData(Namespaces.Version27.Devices, MTConnectVersions.Version27).SetName("GetByNamespace_resolves_Devices_2_7");
        }

        /// <summary>Every currently-declared v2.6/v2.7 namespace (Assets, Devices, Error, Streams) paired with the version it must resolve to.</summary>
        /// <returns>The (namespace, expected version) pairs.</returns>
        public static IEnumerable<TestCaseData> V26AndV27NamespacesByKind()
        {
            yield return new TestCaseData(Namespaces.Version26.Assets, MTConnectVersions.Version26).SetName("GetByNamespace_resolves_Assets_2_6");
            yield return new TestCaseData(Namespaces.Version26.Devices, MTConnectVersions.Version26).SetName("GetByNamespace_resolves_Devices_2_6_ByKind");
            yield return new TestCaseData(Namespaces.Version26.Error, MTConnectVersions.Version26).SetName("GetByNamespace_resolves_Error_2_6");
            yield return new TestCaseData(Namespaces.Version26.Streams, MTConnectVersions.Version26).SetName("GetByNamespace_resolves_Streams_2_6");

            yield return new TestCaseData(Namespaces.Version27.Assets, MTConnectVersions.Version27).SetName("GetByNamespace_resolves_Assets_2_7");
            yield return new TestCaseData(Namespaces.Version27.Devices, MTConnectVersions.Version27).SetName("GetByNamespace_resolves_Devices_2_7_ByKind");
            yield return new TestCaseData(Namespaces.Version27.Error, MTConnectVersions.Version27).SetName("GetByNamespace_resolves_Error_2_7");
            yield return new TestCaseData(Namespaces.Version27.Streams, MTConnectVersions.Version27).SetName("GetByNamespace_resolves_Streams_2_7");
        }

        /// <summary>Every currently-declared MTConnect namespace (Assets, Devices, Error, Streams) across every version, paired with the version it must resolve to. Pins every enum-arm of every <c>Namespaces.Version{XX}.Match</c> disjunction, not just the Devices arm.</summary>
        /// <returns>The (namespace, expected version) pairs.</returns>
        public static IEnumerable<TestCaseData> AllKindsAllVersions()
        {
            // v1.0 and v1.1: no Assets namespace declared (Match is Devices || Error || Streams).
            yield return new TestCaseData(Namespaces.Version10.Devices, MTConnectVersions.Version10).SetName("GetByNamespace_resolves_Devices_1_0_AllKinds");
            yield return new TestCaseData(Namespaces.Version10.Error, MTConnectVersions.Version10).SetName("GetByNamespace_resolves_Error_1_0");
            yield return new TestCaseData(Namespaces.Version10.Streams, MTConnectVersions.Version10).SetName("GetByNamespace_resolves_Streams_1_0");

            yield return new TestCaseData(Namespaces.Version11.Devices, MTConnectVersions.Version11).SetName("GetByNamespace_resolves_Devices_1_1_AllKinds");
            yield return new TestCaseData(Namespaces.Version11.Error, MTConnectVersions.Version11).SetName("GetByNamespace_resolves_Error_1_1");
            yield return new TestCaseData(Namespaces.Version11.Streams, MTConnectVersions.Version11).SetName("GetByNamespace_resolves_Streams_1_1");

            // v1.2+ declare Assets.
            yield return new TestCaseData(Namespaces.Version12.Assets, MTConnectVersions.Version12).SetName("GetByNamespace_resolves_Assets_1_2");
            yield return new TestCaseData(Namespaces.Version12.Error, MTConnectVersions.Version12).SetName("GetByNamespace_resolves_Error_1_2");
            yield return new TestCaseData(Namespaces.Version12.Streams, MTConnectVersions.Version12).SetName("GetByNamespace_resolves_Streams_1_2");

            yield return new TestCaseData(Namespaces.Version13.Assets, MTConnectVersions.Version13).SetName("GetByNamespace_resolves_Assets_1_3");
            yield return new TestCaseData(Namespaces.Version13.Error, MTConnectVersions.Version13).SetName("GetByNamespace_resolves_Error_1_3");
            yield return new TestCaseData(Namespaces.Version13.Streams, MTConnectVersions.Version13).SetName("GetByNamespace_resolves_Streams_1_3");

            yield return new TestCaseData(Namespaces.Version14.Assets, MTConnectVersions.Version14).SetName("GetByNamespace_resolves_Assets_1_4");
            yield return new TestCaseData(Namespaces.Version14.Error, MTConnectVersions.Version14).SetName("GetByNamespace_resolves_Error_1_4");
            yield return new TestCaseData(Namespaces.Version14.Streams, MTConnectVersions.Version14).SetName("GetByNamespace_resolves_Streams_1_4");

            yield return new TestCaseData(Namespaces.Version15.Assets, MTConnectVersions.Version15).SetName("GetByNamespace_resolves_Assets_1_5");
            yield return new TestCaseData(Namespaces.Version15.Error, MTConnectVersions.Version15).SetName("GetByNamespace_resolves_Error_1_5");
            yield return new TestCaseData(Namespaces.Version15.Streams, MTConnectVersions.Version15).SetName("GetByNamespace_resolves_Streams_1_5");

            yield return new TestCaseData(Namespaces.Version16.Assets, MTConnectVersions.Version16).SetName("GetByNamespace_resolves_Assets_1_6");
            yield return new TestCaseData(Namespaces.Version16.Error, MTConnectVersions.Version16).SetName("GetByNamespace_resolves_Error_1_6");
            yield return new TestCaseData(Namespaces.Version16.Streams, MTConnectVersions.Version16).SetName("GetByNamespace_resolves_Streams_1_6");

            yield return new TestCaseData(Namespaces.Version17.Assets, MTConnectVersions.Version17).SetName("GetByNamespace_resolves_Assets_1_7");
            yield return new TestCaseData(Namespaces.Version17.Error, MTConnectVersions.Version17).SetName("GetByNamespace_resolves_Error_1_7");
            yield return new TestCaseData(Namespaces.Version17.Streams, MTConnectVersions.Version17).SetName("GetByNamespace_resolves_Streams_1_7");

            yield return new TestCaseData(Namespaces.Version18.Assets, MTConnectVersions.Version18).SetName("GetByNamespace_resolves_Assets_1_8");
            yield return new TestCaseData(Namespaces.Version18.Error, MTConnectVersions.Version18).SetName("GetByNamespace_resolves_Error_1_8");
            yield return new TestCaseData(Namespaces.Version18.Streams, MTConnectVersions.Version18).SetName("GetByNamespace_resolves_Streams_1_8");

            yield return new TestCaseData(Namespaces.Version20.Assets, MTConnectVersions.Version20).SetName("GetByNamespace_resolves_Assets_2_0");
            yield return new TestCaseData(Namespaces.Version20.Error, MTConnectVersions.Version20).SetName("GetByNamespace_resolves_Error_2_0");
            yield return new TestCaseData(Namespaces.Version20.Streams, MTConnectVersions.Version20).SetName("GetByNamespace_resolves_Streams_2_0");

            yield return new TestCaseData(Namespaces.Version21.Assets, MTConnectVersions.Version21).SetName("GetByNamespace_resolves_Assets_2_1");
            yield return new TestCaseData(Namespaces.Version21.Error, MTConnectVersions.Version21).SetName("GetByNamespace_resolves_Error_2_1");
            yield return new TestCaseData(Namespaces.Version21.Streams, MTConnectVersions.Version21).SetName("GetByNamespace_resolves_Streams_2_1");

            yield return new TestCaseData(Namespaces.Version22.Assets, MTConnectVersions.Version22).SetName("GetByNamespace_resolves_Assets_2_2");
            yield return new TestCaseData(Namespaces.Version22.Error, MTConnectVersions.Version22).SetName("GetByNamespace_resolves_Error_2_2");
            yield return new TestCaseData(Namespaces.Version22.Streams, MTConnectVersions.Version22).SetName("GetByNamespace_resolves_Streams_2_2");

            yield return new TestCaseData(Namespaces.Version23.Assets, MTConnectVersions.Version23).SetName("GetByNamespace_resolves_Assets_2_3");
            yield return new TestCaseData(Namespaces.Version23.Error, MTConnectVersions.Version23).SetName("GetByNamespace_resolves_Error_2_3");
            yield return new TestCaseData(Namespaces.Version23.Streams, MTConnectVersions.Version23).SetName("GetByNamespace_resolves_Streams_2_3");

            yield return new TestCaseData(Namespaces.Version24.Assets, MTConnectVersions.Version24).SetName("GetByNamespace_resolves_Assets_2_4");
            yield return new TestCaseData(Namespaces.Version24.Error, MTConnectVersions.Version24).SetName("GetByNamespace_resolves_Error_2_4");
            yield return new TestCaseData(Namespaces.Version24.Streams, MTConnectVersions.Version24).SetName("GetByNamespace_resolves_Streams_2_4");

            yield return new TestCaseData(Namespaces.Version25.Assets, MTConnectVersions.Version25).SetName("GetByNamespace_resolves_Assets_2_5");
            yield return new TestCaseData(Namespaces.Version25.Error, MTConnectVersions.Version25).SetName("GetByNamespace_resolves_Error_2_5");
            yield return new TestCaseData(Namespaces.Version25.Streams, MTConnectVersions.Version25).SetName("GetByNamespace_resolves_Streams_2_5");
        }

        /// <summary>Pins that every currently-declared Devices namespace resolves to its matching version.</summary>
        /// <param name="ns">The namespace under test.</param>
        /// <param name="expected">The version <paramref name="ns"/> must resolve to.</param>
        [TestCaseSource(nameof(DevicesNamespacesByVersion))]
        public void GetByNamespace_returns_matching_version_for_every_declared_namespace(string ns, Version expected)
        {
            var actual = MTConnectVersion.GetByNamespace(ns);
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins that v2.6 and v2.7 resolve correctly for every document kind (Assets, Devices, Error, Streams), not just Devices.</summary>
        /// <param name="ns">The namespace under test.</param>
        /// <param name="expected">The version <paramref name="ns"/> must resolve to.</param>
        [TestCaseSource(nameof(V26AndV27NamespacesByKind))]
        public void GetByNamespace_returns_matching_version_for_v26_and_v27_across_document_kinds(string ns, Version expected)
        {
            var actual = MTConnectVersion.GetByNamespace(ns);
            Assert.That(actual, Is.EqualTo(expected));
        }

        // Regression guard for the specific bug: before the fix, a v2.7
        // namespace fell all the way through the chain (Version25 was the
        // highest branch present) and returned `new Version()` — equal to
        // "0.0", not `MTConnectVersions.Version27`.
        /// <summary>Pins that a v2.7 namespace does not fall through to an empty version.</summary>
        [Test]
        public void GetByNamespace_v27_namespace_does_not_fall_through_to_empty_version()
        {
            var actual = MTConnectVersion.GetByNamespace(Namespaces.Version27.Streams);
            Assert.That(actual, Is.Not.EqualTo(new Version()));
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Version27));
        }

        /// <summary>Pins that an unrecognised namespace defaults to the latest supported version rather than an empty one.</summary>
        [Test]
        public void GetByNamespace_unknown_namespace_defaults_to_Max()
        {
            var actual = MTConnectVersion.GetByNamespace("urn:mtconnect.org:MTConnectStreams:99.9");
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
            Assert.That(actual, Is.Not.EqualTo(new Version()));
        }

        /// <summary>Pins that a <see langword="null"/> namespace also defaults to the latest supported version rather than an empty one.</summary>
        [Test]
        public void GetByNamespace_null_namespace_defaults_to_Max()
        {
            var actual = MTConnectVersion.GetByNamespace(null);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins that every declared Assets/Devices/Error/Streams namespace across every supported version resolves to its matching version. Exercises every enum-arm of every <c>Namespaces.Version{XX}.Match</c> disjunction, not just the Devices arm.</summary>
        /// <param name="ns">The namespace under test.</param>
        /// <param name="expected">The version <paramref name="ns"/> must resolve to.</param>
        [TestCaseSource(nameof(AllKindsAllVersions))]
        public void GetByNamespace_returns_matching_version_for_every_kind_of_every_version(string ns, Version expected)
        {
            var actual = MTConnectVersion.GetByNamespace(ns);
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins that an empty-string namespace (the value <c>XmlDocument.LoadXml</c> yields for a document with no <c>xmlns</c>) defaults to the latest supported version rather than an empty one.</summary>
        [Test]
        public void GetByNamespace_empty_string_defaults_to_Max()
        {
            var actual = MTConnectVersion.GetByNamespace(string.Empty);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins that a whitespace-only namespace defaults to the latest supported version. Whitespace-only strings do not equal any declared namespace constant, so they must fall through the dispatch chain to the Max fallback.</summary>
        /// <param name="ns">The whitespace-only namespace under test.</param>
        [TestCase(" ")]
        [TestCase("\t")]
        [TestCase("\n")]
        [TestCase("   \t\n  ")]
        public void GetByNamespace_whitespace_only_defaults_to_Max(string ns)
        {
            var actual = MTConnectVersion.GetByNamespace(ns);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins that the dispatch is case-sensitive per the XML namespace-URI spec: an upper-case variant of a canonical namespace URI does not match, and falls through to Max. Guards against a future well-intentioned case-fold refactor that would silently accept malformed documents.</summary>
        /// <param name="ns">The case-variant namespace under test.</param>
        [TestCase("URN:MTCONNECT.ORG:MTCONNECTSTREAMS:2.7")]
        [TestCase("Urn:Mtconnect.Org:MTConnectStreams:2.7")]
        [TestCase("urn:mtconnect.org:mtconnectstreams:2.7")]
        [TestCase("urn:mtconnect.org:MTConnectSTREAMS:2.6")]
        public void GetByNamespace_case_variant_of_declared_namespace_defaults_to_Max(string ns)
        {
            var actual = MTConnectVersion.GetByNamespace(ns);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins that a namespace with leading/trailing whitespace does not match a declared constant (string equality is exact) and therefore falls through to Max.</summary>
        /// <param name="ns">The padded namespace under test.</param>
        [TestCase(" urn:mtconnect.org:MTConnectStreams:2.7")]
        [TestCase("urn:mtconnect.org:MTConnectStreams:2.7 ")]
        [TestCase(" urn:mtconnect.org:MTConnectStreams:2.7 ")]
        public void GetByNamespace_padded_namespace_defaults_to_Max(string ns)
        {
            var actual = MTConnectVersion.GetByNamespace(ns);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins that a namespace containing non-ASCII characters (unicode combining marks, an RTL script run, and a surrogate-pair emoji) does not match any declared namespace constant and falls through to Max without throwing. Guards the string-equality dispatch chain against the unicode boundary class required by the coverage FLOOR.</summary>
        /// <param name="ns">The unicode-bearing namespace under test.</param>
        [TestCase("urn:mtconnect.org:MTConnectStréams:2.7")] // combining acute accent (é as e + U+0301)
        [TestCase("urn:mtconnect.org:MTConnectStreams:2.7‏العربية")] // trailing RTL mark + Arabic run
        [TestCase("urn:mtconnect.org:MTConnectStreams:2.7😀")] // trailing surrogate-pair emoji (U+1F600)
        public void GetByNamespace_unicode_namespace_defaults_to_Max(string ns)
        {
            var actual = MTConnectVersion.GetByNamespace(ns);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins the public <see cref="MTConnectVersion.Get(string)"/> entry point: a well-formed XML document declaring a canonical MTConnect namespace resolves through <c>Namespaces.Get</c> to the matching version. Exercises the full public-API surface of the class, not just the <c>GetByNamespace</c> internal.</summary>
        /// <param name="xmlNamespace">The namespace URI to embed as the root element's default namespace.</param>
        /// <param name="expected">The version <paramref name="xmlNamespace"/> must resolve to.</param>
        [TestCase("urn:mtconnect.org:MTConnectStreams:1.0", "1.0")]
        [TestCase("urn:mtconnect.org:MTConnectStreams:2.5", "2.5")]
        [TestCase("urn:mtconnect.org:MTConnectStreams:2.6", "2.6")]
        [TestCase("urn:mtconnect.org:MTConnectStreams:2.7", "2.7")]
        [TestCase("urn:mtconnect.org:MTConnectDevices:2.7", "2.7")]
        [TestCase("urn:mtconnect.org:MTConnectAssets:2.7", "2.7")]
        [TestCase("urn:mtconnect.org:MTConnectError:2.7", "2.7")]
        public void Get_extracts_namespace_from_xml_and_dispatches_to_matching_version(string xmlNamespace, string expected)
        {
            var xml = "<MTConnectStreams xmlns=\"" + xmlNamespace + "\" />";
            var actual = MTConnectVersion.Get(xml);
            Assert.That(actual, Is.EqualTo(Version.Parse(expected)));
        }

        /// <summary>Pins that <see cref="MTConnectVersion.Get(string)"/> against a well-formed XML document with no namespace declaration defaults to Max — exercises the empty-string boundary of <c>GetByNamespace</c> reached from the <c>Get(xml)</c> entry point.</summary>
        [Test]
        public void Get_returns_Max_when_xml_has_no_namespace()
        {
            var xml = "<MTConnectStreams />";
            var actual = MTConnectVersion.Get(xml);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins that <see cref="MTConnectVersion.Get(string)"/> resolves an XML document declaring a v2.7 namespace to <c>Version27</c>, not to an empty version — the exact bug the PR fixes, re-asserted through the <c>Get(xml)</c> entry point instead of just <c>GetByNamespace</c>.</summary>
        [Test]
        public void Get_v27_xml_does_not_fall_through_to_empty_version()
        {
            var xml = "<MTConnectStreams xmlns=\"urn:mtconnect.org:MTConnectStreams:2.7\" />";
            var actual = MTConnectVersion.Get(xml);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Version27));
            Assert.That(actual.Major, Is.EqualTo(2));
            Assert.That(actual.Minor, Is.EqualTo(7));
        }

        /// <summary>Pins that <see cref="MTConnectVersion.Get(string)"/> rejects a document that declares a DTD — the hardened <c>Namespaces.Get</c> sets <c>DtdProcessing = Prohibit</c>, so any DOCTYPE-carrying payload (including billion-laughs entity-expansion attempts) is refused rather than parsed. Guards the XXE / entity-expansion surface introduced by <c>XmlDocument.LoadXml</c>'s historical default settings.</summary>
        [Test]
        public void Get_rejects_document_with_dtd_and_defaults_to_Max()
        {
            var xml = "<?xml version=\"1.0\"?>" +
                      "<!DOCTYPE root [<!ELEMENT root ANY>]>" +
                      "<root xmlns=\"urn:mtconnect.org:MTConnectStreams:2.7\" />";
            var actual = MTConnectVersion.Get(xml);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins that <see cref="MTConnectVersion.Get(string)"/> rejects a billion-laughs entity-expansion payload without parsing it — the hardened <c>Namespaces.Get</c> refuses the DOCTYPE up-front, so the exponentially-expanding entity chain never materialises. Regression guard for the XXE / entity-expansion surface.</summary>
        [Test]
        public void Get_rejects_billion_laughs_payload_and_defaults_to_Max()
        {
            var xml = "<?xml version=\"1.0\"?>" +
                      "<!DOCTYPE lolz [" +
                      "  <!ENTITY lol \"lol\">" +
                      "  <!ENTITY lol2 \"&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;\">" +
                      "  <!ENTITY lol3 \"&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;\">" +
                      "]>" +
                      "<lolz>&lol3;</lolz>";
            var actual = MTConnectVersion.Get(xml);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins that <see cref="MTConnectVersion.Get(string)"/> rejects a document that references an external entity without resolving it — the hardened <c>Namespaces.Get</c> sets <c>XmlResolver = null</c>, so file:// / http:// references cannot exfiltrate host data. Regression guard for the classic XXE surface.</summary>
        [Test]
        public void Get_rejects_external_entity_reference_and_defaults_to_Max()
        {
            var xml = "<?xml version=\"1.0\"?>" +
                      "<!DOCTYPE foo [<!ENTITY xxe SYSTEM \"file:///etc/passwd\">]>" +
                      "<foo>&xxe;</foo>";
            var actual = MTConnectVersion.Get(xml);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins that <see cref="MTConnectVersion.Get(string)"/> returns the Max fallback on malformed input rather than propagating an <c>XmlException</c>. Guards downstream callers from having to catch XML-parse errors on every dispatch call.</summary>
        [Test]
        public void Get_returns_Max_on_malformed_xml()
        {
            var actual = MTConnectVersion.Get("<not-well-formed");
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }

        /// <summary>Pins that <see cref="MTConnectVersion.Get(string)"/> returns the Max fallback on a null or empty input rather than throwing an <c>ArgumentException</c>.</summary>
        /// <param name="xml">The input under test.</param>
        [TestCase(null)]
        [TestCase("")]
        public void Get_returns_Max_on_null_or_empty_input(string xml)
        {
            var actual = MTConnectVersion.Get(xml);
            Assert.That(actual, Is.EqualTo(MTConnectVersions.Max));
        }
    }
}
