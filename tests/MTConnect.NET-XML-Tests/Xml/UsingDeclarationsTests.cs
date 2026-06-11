// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NUnit.Framework;

namespace MTConnect.Tests.XML.Xml
{
    // Pins the C# 8.0 `using` declaration code path in
    // MTConnect.NET-XML. The production site lives in
    // XsdPreprocessor.StripXsd11Constructs:
    //
    //     using var reader = new StringReader(xsdSourceXml);
    //
    // C# 8.0 added the using-declaration form; on netstandard2.0 the
    // compiler's default LangVersion is 7.3, which rejects that syntax
    // with CS8370. PR #194 pins the project's LangVersion to 8.0 so the
    // Release pack survives the full TFM matrix; this fixture exercises
    // the path so a regression that breaks the preprocessor's load step
    // surfaces as a test failure rather than only as a build break.
    //
    // The fixture compiles and runs under net8.0 — the only TFM every
    // test project targets — so the test passes regardless of
    // LangVersion. Its value is to ensure the production code path
    // stays exercised, matching the brief's TDD shape for paths whose
    // pre-fix surface is already correct on the test TFM.
    /// <summary>Pins the C# 8.0 using-declaration path in XsdPreprocessor.</summary>
    [TestFixture]
    [Category("MultiTfmCompat")]
    public class UsingDeclarationsTests
    {
        /// <summary>XsdPreprocessor.StripXsd11Constructs accepts a minimal well-formed XSD.</summary>
        [Test]
        public void StripXsd11Constructs_round_trips_minimal_xsd()
        {
            // A minimal well-formed XSD 1.0 schema with no 1.1-only
            // constructs round-trips through StripXsd11Constructs
            // unchanged structurally. The path exercised here is the
            // using-declaration body that disposes the StringReader.
            const string minimalXsd =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"" +
                " targetNamespace=\"urn:test\" elementFormDefault=\"qualified\">\n" +
                "  <xs:element name=\"Root\" type=\"xs:string\"/>\n" +
                "</xs:schema>";

            var result = XsdPreprocessor.StripXsd11Constructs(minimalXsd);

            Assert.That(result, Is.Not.Null,
                "StripXsd11Constructs must return a non-null result for valid input.");
            Assert.That(result, Does.Contain("Root"),
                "The minimal schema's element name must round-trip through the preprocessor.");
        }

        /// <summary>XsdPreprocessor.StripXsd11Constructs strips the 1.1-only xs:assert element.</summary>
        [Test]
        public void StripXsd11Constructs_removes_xs_assert_elements()
        {
            // Direct coverage of the xs:assert removal branch — the
            // path enters StripXsd11Constructs, drives XDocument.Load
            // through the using-declared StringReader, and exits with
            // the xs:assert element removed.
            const string xsdWithAssert =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"" +
                " targetNamespace=\"urn:test\" elementFormDefault=\"qualified\">\n" +
                "  <xs:complexType name=\"WithAssert\">\n" +
                "    <xs:sequence>\n" +
                "      <xs:element name=\"Inner\" type=\"xs:string\"/>\n" +
                "    </xs:sequence>\n" +
                "    <xs:assert test=\"Inner != ''\"/>\n" +
                "  </xs:complexType>\n" +
                "</xs:schema>";

            var result = XsdPreprocessor.StripXsd11Constructs(xsdWithAssert);

            Assert.That(result, Does.Not.Contain("xs:assert"),
                "xs:assert must be stripped by the preprocessor.");
            Assert.That(result, Does.Contain("WithAssert"),
                "The enclosing complexType must survive the assertion stripping.");
        }
    }
}
