// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text;
using System.Xml;
using NUnit.Framework;

namespace MTConnect.Xml.Tests
{
    /// <summary>
    /// Pins the DoS-hardening guard rails on <see cref="XsdPreprocessor.StripXsd11Constructs(string)"/>.
    /// The preprocessor loads untrusted XSD text; without the hardening a hostile source can trigger
    /// the classic XML entity-expansion attacks (billion-laughs, quadratic blowup) or exhaust memory
    /// through a runaway payload.
    /// </summary>
    [TestFixture]
    [Category("XsdPreprocessorSecurity")]
    public class XsdPreprocessorSecurityTests
    {
        /// <summary>Pins that a source exceeding the character limit raises <see cref="XmlException"/> instead of allocating an unbounded XmlReader.</summary>
        [Test]
        public void StripXsd11Constructs_OversizedInput_RaisesXmlException()
        {
            // Cheap oversized payload: a single well-formed root element with padding pushing the
            // total length just past the guard. The padding lives inside the element so the XML is
            // still parseable — the size gate must fire before the parser sees it.
            var padding = new string('a', XsdPreprocessor.MaxSourceCharacters);
            var oversized =
                "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">"
                + $"<xs:annotation><xs:documentation>{padding}</xs:documentation></xs:annotation>"
                + "</xs:schema>";

            Assert.That(oversized.Length, Is.GreaterThan(XsdPreprocessor.MaxSourceCharacters),
                "the payload must straddle the size gate for the test to be meaningful");
            Assert.Throws<XmlException>(() => XsdPreprocessor.StripXsd11Constructs(oversized));
        }

        /// <summary>Pins that a source referencing an external DTD is rejected — the hardened reader has DtdProcessing set to Prohibit.</summary>
        [Test]
        public void StripXsd11Constructs_ExternalDtdReference_ReturnsSource_Unprocessed()
        {
            // A well-formed XML with a DOCTYPE declaration. The hardened reader must refuse to
            // process the DTD; the preprocessor catches the resulting XmlException and returns the
            // raw source (existing not-well-formed contract).
            const string withDtd =
                "<?xml version=\"1.0\"?>"
                + "<!DOCTYPE xs:schema SYSTEM \"http://example.invalid/malicious.dtd\">"
                + "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"/>";

            var result = XsdPreprocessor.StripXsd11Constructs(withDtd);

            // Not-well-formed / DTD-prohibited inputs land in the catch and return the raw source
            // for the downstream schema reader to error out consistently.
            Assert.That(result, Is.EqualTo(withDtd));
        }

        /// <summary>Pins that a source with an internal DTD entity expansion is rejected — the hardened reader has DtdProcessing set to Prohibit, so the entity is never expanded.</summary>
        [Test]
        public void StripXsd11Constructs_InternalDtdEntityExpansion_ReturnsSource_Unprocessed()
        {
            // Simplified billion-laughs shape: three levels of entity expansion. Any bytes-in-entities
            // pass would blow up the parser; the DTD prohibition means the parser never even sees
            // the definitions.
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\"?>");
            sb.Append("<!DOCTYPE xs:schema [");
            sb.Append("<!ENTITY a \"aaaaaaaaaaaaaaaaaaaaaaaa\">");
            sb.Append("<!ENTITY b \"&a;&a;&a;&a;&a;&a;&a;&a;&a;&a;\">");
            sb.Append("<!ENTITY c \"&b;&b;&b;&b;&b;&b;&b;&b;&b;&b;\">");
            sb.Append("]>");
            sb.Append("<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"><xs:element name=\"x\">&c;</xs:element></xs:schema>");
            var laugh = sb.ToString();

            var result = XsdPreprocessor.StripXsd11Constructs(laugh);

            Assert.That(result, Is.EqualTo(laugh),
                "the DTD-prohibited reader must refuse the entity-expansion payload and the " +
                "preprocessor must return the raw source unprocessed");
        }

        /// <summary>Pins that a well-formed, small XSD still round-trips through the hardened loader.</summary>
        [Test]
        public void StripXsd11Constructs_SmallWellFormedXsd_RoundTripsThroughHardenedLoader()
        {
            // Regression guard: the hardening MUST NOT break the happy path for the shipped
            // MTConnect XSDs, which are well under the size cap and carry no DTD.
            const string xsd =
                "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"urn:test\">"
                + "<xs:element name=\"Root\" type=\"xs:string\"/>"
                + "</xs:schema>";

            var result = XsdPreprocessor.StripXsd11Constructs(xsd);

            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(result, Does.Contain("<xs:element"),
                "the round-trip must preserve the schema's structural elements");
        }
    }
}
