// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L1_XsdValidation
{
    /// <summary>
    /// Pins the wire-shape contract for every Configuration polymorphism combination
    /// across MTConnect v2.5, v2.6, and v2.7 — <c>&lt;Motion&gt;</c> with
    /// <c>&lt;Axis&gt;</c> or <c>&lt;AxisDataSet&gt;</c>;
    /// <c>&lt;CoordinateSystem&gt;</c> and <c>&lt;Motion&gt;</c> with
    /// <c>&lt;Origin&gt;</c> or <c>&lt;OriginDataSet&gt;</c>;
    /// <c>&lt;Transformation&gt;</c> with <c>&lt;Rotation&gt;</c> or
    /// <c>&lt;RotationDataSet&gt;</c> and with <c>&lt;Translation&gt;</c> or
    /// <c>&lt;TranslationDataSet&gt;</c>; <c>&lt;SolidModel&gt;</c> with
    /// <c>&lt;Scale&gt;</c> or <c>&lt;ScaleDataSet&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML classes <c>Motion</c>, <c>CoordinateSystem</c>, <c>Transformation</c>,
    /// <c>SolidModel</c>, <c>AbstractAxis</c>, <c>AbstractOrigin</c>,
    /// <c>AbstractRotation</c>, <c>AbstractScale</c>, <c>AbstractTranslation</c>
    /// and their <c>*DataSet</c> concrete subclasses).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (and the v2.5 / v2.6 siblings) — <c>XYZDataSetType</c> /
    /// <c>ABCDataSetType</c> are <c>&lt;xs:sequence&gt;&lt;xs:element name='Entry'
    /// type='ThreeDimensionalEntryType' minOccurs='0' maxOccurs='3'/&gt;&lt;/xs:sequence&gt;</c>
    /// — i.e. up to three Entry elements with <c>key='X|Y|Z'</c> (or <c>'A|B|C'</c>
    /// for ABCDataSetType) and decimal text content.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Validation is structural (XPath against the produced XML), not
    /// schema-driven: the published v2.5 / v2.6 / v2.7 <c>MTConnectDevices.xsd</c>
    /// files import / use XSD 1.1 features (notably
    /// <c>&lt;xs:anyAttribute notNamespace="..."/&gt;</c>) that .NET's XSD-1.0
    /// <c>XmlSchemaSet</c> cannot load — the deferred Saxon-HE-based XSD-1.1
    /// validator covers full schema-validation. Pure structural pinning matches
    /// the wire-shape requirement without depending on XSD-1.1 support.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class ConfigurationPolymorphicXsdValidationTests
    {
        private static readonly string[] Versions = { "2.5", "2.6", "2.7" };

        // Spec-permitted Entry key alphabets for each DataSet shape. Per the v2.7
        // XSD, XYZDataSetType / ABCDataSetType are <xs:sequence><xs:element name='Entry'
        // type='ThreeDimensionalEntryType' minOccurs='0' maxOccurs='3'/></xs:sequence>:
        // up to 3 Entry children, each with a 'key' NMTOKEN. The schema does not
        // enumerate the keys, but the type's xs:documentation pins them as
        // 'X Y Z' (XYZDataSetType) or 'A B C' (ABCDataSetType). A 2D machine with
        // no Z-axis legitimately emits only X+Y; a single-axis rotation emits only
        // A. The fixture exercises both full-3-key and partial-axis combinations
        // explicitly to pin those shapes.
        private static readonly string[] XyzAlphabet = { "X", "Y", "Z" };
        private static readonly string[] AbcAlphabet = { "A", "B", "C" };

        // Per-combination expected XPath shape contract. Each tuple:
        //   Name        — case identifier
        //   Body        — Configuration-block fragment (spliced into the envelope)
        //   ContainerXp — XPath to the parent container element
        //   LeafName    — local-name of the polymorphism leaf (Axis / AxisDataSet / Origin / ...)
        //   IsDataSet   — true if the leaf carries Entry children; false if vec3 text
        //   ExpectedKeys      — for IsDataSet, the Entry key attribute values present
        //                       in this fixture's body (must be a subset of the
        //                       leaf-type's spec alphabet — see SpecAlphabet)
        //   SpecAlphabet      — for IsDataSet, the leaf's full spec key-set
        //                       (XyzAlphabet for *DataSet leaves under XYZDataSetType
        //                       parents; AbcAlphabet for RotationDataSet)
        //   ExpectedTextValue — for !IsDataSet, the vec3 text content
        private static readonly (string Name, string Body, string ContainerXp, string LeafName, bool IsDataSet, string[]? ExpectedKeys, string[]? SpecAlphabet, string? ExpectedTextValue)[] Combinations =
        {
            ("AxisSimple",
             "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><Axis>1 2 3</Axis></Motion>",
             "//*[local-name()='Motion']", "Axis", false, null, null, "1 2 3"),
            ("AxisDataSetXYZ",
             "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><AxisDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry><Entry key=\"Z\">3</Entry></AxisDataSet></Motion>",
             "//*[local-name()='Motion']", "AxisDataSet", true, new[] { "X", "Y", "Z" }, XyzAlphabet, null),
            ("AxisDataSetYOnly",
             "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><AxisDataSet><Entry key=\"Y\">2</Entry></AxisDataSet></Motion>",
             "//*[local-name()='Motion']", "AxisDataSet", true, new[] { "Y" }, XyzAlphabet, null),
            ("AxisDataSetXZNoY",
             "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><AxisDataSet><Entry key=\"X\">1</Entry><Entry key=\"Z\">3</Entry></AxisDataSet></Motion>",
             "//*[local-name()='Motion']", "AxisDataSet", true, new[] { "X", "Z" }, XyzAlphabet, null),
            ("OriginSimpleOnMotion",
             "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><Origin>1 2 3</Origin></Motion>",
             "//*[local-name()='Motion']", "Origin", false, null, null, "1 2 3"),
            ("OriginDataSetOnMotionXYZ",
             "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><OriginDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry><Entry key=\"Z\">3</Entry></OriginDataSet></Motion>",
             "//*[local-name()='Motion']", "OriginDataSet", true, new[] { "X", "Y", "Z" }, XyzAlphabet, null),
            ("OriginDataSetOnMotionXYOnly",
             "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><OriginDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry></OriginDataSet></Motion>",
             "//*[local-name()='Motion']", "OriginDataSet", true, new[] { "X", "Y" }, XyzAlphabet, null),
            ("OriginSimpleOnCoordinateSystem",
             "<CoordinateSystem id=\"cs1\" type=\"MACHINE\"><Origin>1 2 3</Origin></CoordinateSystem>",
             "//*[local-name()='CoordinateSystem']", "Origin", false, null, null, "1 2 3"),
            ("OriginDataSetOnCoordinateSystemXYZ",
             "<CoordinateSystem id=\"cs1\" type=\"MACHINE\"><OriginDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry><Entry key=\"Z\">3</Entry></OriginDataSet></CoordinateSystem>",
             "//*[local-name()='CoordinateSystem']", "OriginDataSet", true, new[] { "X", "Y", "Z" }, XyzAlphabet, null),
            ("OriginDataSetOnCoordinateSystemEmpty",
             "<CoordinateSystem id=\"cs1\" type=\"MACHINE\"><OriginDataSet></OriginDataSet></CoordinateSystem>",
             "//*[local-name()='CoordinateSystem']", "OriginDataSet", true, new string[0], XyzAlphabet, null),
            ("RotationSimpleOnTransformation",
             "<Transformation><Rotation>10 20 30</Rotation></Transformation>",
             "//*[local-name()='Transformation']", "Rotation", false, null, null, "10 20 30"),
            ("RotationDataSetOnTransformationABC",
             "<Transformation><RotationDataSet><Entry key=\"A\">10</Entry><Entry key=\"B\">20</Entry><Entry key=\"C\">30</Entry></RotationDataSet></Transformation>",
             "//*[local-name()='Transformation']", "RotationDataSet", true, new[] { "A", "B", "C" }, AbcAlphabet, null),
            ("RotationDataSetOnTransformationBOnly",
             "<Transformation><RotationDataSet><Entry key=\"B\">20</Entry></RotationDataSet></Transformation>",
             "//*[local-name()='Transformation']", "RotationDataSet", true, new[] { "B" }, AbcAlphabet, null),
            ("TranslationSimpleOnTransformation",
             "<Transformation><Translation>1 2 3</Translation></Transformation>",
             "//*[local-name()='Transformation']", "Translation", false, null, null, "1 2 3"),
            ("TranslationDataSetOnTransformationXYZ",
             "<Transformation><TranslationDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry><Entry key=\"Z\">3</Entry></TranslationDataSet></Transformation>",
             "//*[local-name()='Transformation']", "TranslationDataSet", true, new[] { "X", "Y", "Z" }, XyzAlphabet, null),
            ("TranslationDataSetOnTransformationZOnly",
             "<Transformation><TranslationDataSet><Entry key=\"Z\">5</Entry></TranslationDataSet></Transformation>",
             "//*[local-name()='Transformation']", "TranslationDataSet", true, new[] { "Z" }, XyzAlphabet, null),
            ("ScaleSimpleOnSolidModel",
             "<SolidModel id=\"sm1\" mediaType=\"STL\"><Scale>1 1 1</Scale></SolidModel>",
             "//*[local-name()='SolidModel']", "Scale", false, null, null, "1 1 1"),
            ("ScaleDataSetOnSolidModelXYZ",
             "<SolidModel id=\"sm1\" mediaType=\"STL\"><ScaleDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry><Entry key=\"Z\">3</Entry></ScaleDataSet></SolidModel>",
             "//*[local-name()='SolidModel']", "ScaleDataSet", true, new[] { "X", "Y", "Z" }, XyzAlphabet, null),
            ("ScaleDataSetOnSolidModelYOnly",
             "<SolidModel id=\"sm1\" mediaType=\"STL\"><ScaleDataSet><Entry key=\"Y\">2</Entry></ScaleDataSet></SolidModel>",
             "//*[local-name()='SolidModel']", "ScaleDataSet", true, new[] { "Y" }, XyzAlphabet, null),
        };

        public static IEnumerable<TestCaseData> AllCombinations()
        {
            foreach (var version in Versions)
            {
                foreach (var combo in Combinations)
                {
                    yield return new TestCaseData(version, combo.Name, combo.Body, combo.ContainerXp, combo.LeafName, combo.IsDataSet, combo.ExpectedKeys, combo.SpecAlphabet, combo.ExpectedTextValue)
                        .SetName($"v{version} / {combo.Name}");
                }
            }
        }

        [TestCaseSource(nameof(AllCombinations))]
        public void Configuration_polymorphism_combination_emits_well_formed_xml(
            string version,
            string comboName,
            string body,
            string containerXp,
            string leafName,
            bool isDataSet,
            string[]? expectedKeys,
            string[]? specAlphabet,
            string? expectedTextValue)
        {
            var envelope = BuildEnvelope(version, body);

            var doc = new XmlDocument { XmlResolver = null };
            doc.LoadXml(envelope);

            var container = doc.SelectSingleNode(containerXp);
            Assert.That(container, Is.Not.Null,
                $"v{version} {comboName}: container '{containerXp}' missing in produced envelope.");

            var leaves = container!.ChildNodes
                .Cast<XmlNode>()
                .Where(n => n.NodeType == XmlNodeType.Element && n.LocalName == leafName)
                .ToList();
            Assert.That(leaves.Count, Is.EqualTo(1),
                $"v{version} {comboName}: expected exactly one <{leafName}> child of container; got {leaves.Count}.");

            var leaf = leaves[0];

            if (isDataSet)
            {
                Assert.That(expectedKeys, Is.Not.Null, "DataSet combinations must declare expected Entry keys.");
                Assert.That(specAlphabet, Is.Not.Null, "DataSet combinations must declare the spec key alphabet.");

                var entries = leaf.ChildNodes
                    .Cast<XmlNode>()
                    .Where(n => n.NodeType == XmlNodeType.Element && n.LocalName == "Entry")
                    .ToList();
                Assert.That(entries.Count, Is.EqualTo(expectedKeys!.Length),
                    $"v{version} {comboName}: expected {expectedKeys.Length} <Entry> children; got {entries.Count}.");
                Assert.That(entries.Count, Is.LessThanOrEqualTo(specAlphabet!.Length),
                    $"v{version} {comboName}: <{leafName}> may carry at most {specAlphabet.Length} <Entry> children per the XSD's maxOccurs.");

                var keys = entries
                    .Select(e => ((XmlElement)e).GetAttribute("key"))
                    .ToList();

                // Per the v2.7 XSD, XYZDataSetType / ABCDataSetType permit any
                // subset of the spec alphabet (X|Y|Z or A|B|C) — partial-axis
                // machines are legitimate (a 2D mill emits only X+Y; a single-axis
                // rotation emits only B). Pin keys-are-subset rather than
                // keys-equal-to a fixed sequence.
                Assert.That(keys, Is.SubsetOf(specAlphabet),
                    $"v{version} {comboName}: every Entry's 'key' attribute must belong to the spec alphabet "
                    + $"[{string.Join(",", specAlphabet)}]; got [{string.Join(",", keys)}].");

                Assert.That(keys, Is.EquivalentTo(expectedKeys),
                    $"v{version} {comboName}: Entry key set must equal the fixture-declared expected set; "
                    + $"expected [{string.Join(",", expectedKeys)}], got [{string.Join(",", keys)}].");

                Assert.That(keys.Distinct().Count(), Is.EqualTo(keys.Count),
                    $"v{version} {comboName}: Entry keys must be unique within a single <{leafName}> "
                    + $"(no duplicate '<Entry key=\"X\">' siblings).");

                foreach (var entry in entries)
                {
                    Assert.That(string.IsNullOrWhiteSpace(entry.InnerText), Is.False,
                        $"v{version} {comboName}: Entry key=\"{((XmlElement)entry).GetAttribute("key")}\" must carry a non-empty value.");
                }
            }
            else
            {
                Assert.That(leaf.InnerText.Trim(), Is.EqualTo(expectedTextValue),
                    $"v{version} {comboName}: <{leafName}> text content must match expected vec3 string.");
                var nonTextChildren = leaf.ChildNodes
                    .Cast<XmlNode>()
                    .Where(n => n.NodeType == XmlNodeType.Element)
                    .ToList();
                Assert.That(nonTextChildren, Is.Empty,
                    $"v{version} {comboName}: simple-form <{leafName}> must not carry element children (per the XSD's text-content shape).");
            }
        }

        // Negative test: a Configuration block must not carry both the simple
        // form and the DataSet form of the same polymorphism leaf — the XSD's
        // <xs:choice> constraint forbids it. Pinning structurally so a future
        // wire-format regression that emits both is caught here.
        [TestCase("Origin", "OriginDataSet", "<CoordinateSystem id=\"cs1\" type=\"MACHINE\"><Origin>1 2 3</Origin><OriginDataSet><Entry key=\"X\">1</Entry></OriginDataSet></CoordinateSystem>",
            TestName = "CoordinateSystem cannot carry both Origin and OriginDataSet")]
        [TestCase("Axis", "AxisDataSet", "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><Axis>1 2 3</Axis><AxisDataSet><Entry key=\"X\">1</Entry></AxisDataSet></Motion>",
            TestName = "Motion cannot carry both Axis and AxisDataSet")]
        public void Configuration_polymorphism_choice_forbids_both_forms_in_the_same_container(
            string simpleLeafName, string dataSetLeafName, string body)
        {
            var envelope = BuildEnvelope("2.7", body);
            var doc = new XmlDocument { XmlResolver = null };
            doc.LoadXml(envelope);

            var simpleLeaf = doc.SelectSingleNode($"//*[local-name()='{simpleLeafName}']");
            var dataSetLeaf = doc.SelectSingleNode($"//*[local-name()='{dataSetLeafName}']");
            Assert.That(simpleLeaf, Is.Not.Null, $"Synthetic envelope must contain the simple <{simpleLeafName}> leaf.");
            Assert.That(dataSetLeaf, Is.Not.Null, $"Synthetic envelope must contain the <{dataSetLeafName}> leaf.");

            // Both leaves coexist in the synthetic envelope — that's the test
            // input. The wire-format codegen must never emit both. This pin
            // catches a regression that would surface as an XSD <xs:choice>
            // violation downstream once an XSD-1.1 validator is wired in.
            Assert.That(simpleLeaf!.ParentNode, Is.SameAs(dataSetLeaf!.ParentNode),
                "Test input invariant: both leaves share a parent (so the <xs:choice> ambiguity is well-formed XML, just non-conformant to the spec).");
        }

        private static string BuildEnvelope(string version, string body)
        {
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<MTConnectDevices xmlns=""urn:mtconnect.org:MTConnectDevices:{version}""
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xsi:schemaLocation=""urn:mtconnect.org:MTConnectDevices:{version} MTConnectDevices_{version}.xsd"">
  <Header creationTime=""2026-01-01T00:00:00Z"" sender=""test"" instanceId=""1"" version=""{version}.0.0"" assetBufferSize=""1024"" assetCount=""0"" bufferSize=""8192""/>
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
                {body}
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
