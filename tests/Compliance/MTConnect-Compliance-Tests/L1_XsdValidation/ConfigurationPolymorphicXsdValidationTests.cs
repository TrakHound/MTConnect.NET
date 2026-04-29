// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L1_XsdValidation
{
    /// <summary>
    /// Validates synthetic <c>MTConnectDevices</c> envelopes containing every
    /// Configuration polymorphism combination (<c>&lt;Motion&gt;</c> with
    /// <c>&lt;Axis&gt;</c> or <c>&lt;AxisDataSet&gt;</c>;
    /// <c>&lt;CoordinateSystem&gt;</c> and <c>&lt;Motion&gt;</c> with
    /// <c>&lt;Origin&gt;</c> or <c>&lt;OriginDataSet&gt;</c>;
    /// <c>&lt;Transformation&gt;</c> with <c>&lt;Rotation&gt;</c> or
    /// <c>&lt;RotationDataSet&gt;</c> and with <c>&lt;Translation&gt;</c> or
    /// <c>&lt;TranslationDataSet&gt;</c>; <c>&lt;SolidModel&gt;</c> with
    /// <c>&lt;Scale&gt;</c> or <c>&lt;ScaleDataSet&gt;</c>) against the
    /// versioned MTConnectDevices XSDs for v2.5, v2.6, and v2.7.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/></item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (and the v2.5 / v2.6 siblings).</item>
    /// </list>
    /// </para>
    /// <para>
    /// Verified 2026-04-30: forcing this fixture through the test runner
    /// (<c>--filter "FullyQualifiedName~ConfigurationPolymorphicXsdValidationTests"</c>)
    /// fails all 36 cases (12 combinations x 3 versions) with
    /// <c>System.Xml.Schema.XmlSchemaException : The 'notNamespace' attribute
    /// is not supported in this context.</c> at
    /// <c>XmlSchemaSet.Add(...)</c> — the v2.5 / v2.6 / v2.7
    /// <c>MTConnectDevices_*.xsd</c> files use XSD 1.1 features (notably
    /// <c>notNamespace</c> on <c>xs:anyAttribute</c>) that .NET's XSD 1.0
    /// <c>XmlSchema</c> cannot load. The fixture sits in the
    /// <c>XsdLoadStrict</c> category so it participates in the opt-in
    /// strict-load sweep but is excluded from the default test run. A
    /// follow-up XSD 1.1 validator PR unblocks these and any similar
    /// XSD-driven gates.
    /// </para>
    /// </remarks>
    [TestFixture]
    [Category("XsdLoadStrict")]
    public class ConfigurationPolymorphicXsdValidationTests
    {
        private const string ResourcePrefix = "MTConnect.Compliance.Tests.Schemas.";

        private static readonly string[] Versions = { "2.5", "2.6", "2.7" };

        // Minimum-viable Configuration sub-element shapes spliced into the
        // synthetic envelope's Linear-axis Configuration block.
        private static readonly (string Name, string Body)[] Combinations =
        {
            ("AxisSimple", "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><Axis>1 2 3</Axis></Motion>"),
            ("AxisDataSet", "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><AxisDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry><Entry key=\"Z\">3</Entry></AxisDataSet></Motion>"),
            ("OriginSimpleOnMotion", "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><Origin>1 2 3</Origin></Motion>"),
            ("OriginDataSetOnMotion", "<Motion id=\"mx\" type=\"PRISMATIC\" actuation=\"DIRECT\" coordinateSystemIdRef=\"mc\"><OriginDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry><Entry key=\"Z\">3</Entry></OriginDataSet></Motion>"),
            ("OriginSimpleOnCoordinateSystem", "<CoordinateSystem id=\"cs1\" type=\"MACHINE\"><Origin>1 2 3</Origin></CoordinateSystem>"),
            ("OriginDataSetOnCoordinateSystem", "<CoordinateSystem id=\"cs1\" type=\"MACHINE\"><OriginDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry><Entry key=\"Z\">3</Entry></OriginDataSet></CoordinateSystem>"),
            ("RotationSimpleOnTransformation", "<Transformation><Rotation>10 20 30</Rotation></Transformation>"),
            ("RotationDataSetOnTransformation", "<Transformation><RotationDataSet><Entry key=\"A\">10</Entry><Entry key=\"B\">20</Entry><Entry key=\"C\">30</Entry></RotationDataSet></Transformation>"),
            ("TranslationSimpleOnTransformation", "<Transformation><Translation>1 2 3</Translation></Transformation>"),
            ("TranslationDataSetOnTransformation", "<Transformation><TranslationDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry><Entry key=\"Z\">3</Entry></TranslationDataSet></Transformation>"),
            ("ScaleSimpleOnSolidModel", "<SolidModel id=\"sm1\" mediaType=\"STL\"><Scale>1 1 1</Scale></SolidModel>"),
            ("ScaleDataSetOnSolidModel", "<SolidModel id=\"sm1\" mediaType=\"STL\"><ScaleDataSet><Entry key=\"X\">1</Entry><Entry key=\"Y\">2</Entry><Entry key=\"Z\">3</Entry></ScaleDataSet></SolidModel>"),
        };

        public static IEnumerable<TestCaseData> AllCombinations()
        {
            foreach (var version in Versions)
            {
                foreach (var combo in Combinations)
                {
                    yield return new TestCaseData(version, combo.Name, combo.Body)
                        .SetName($"v{version} / {combo.Name}");
                }
            }
        }

        [TestCaseSource(nameof(AllCombinations))]
        public void Configuration_polymorphism_combination_is_xsd_valid(string version, string comboName, string body)
        {
            var schemaResource = $"{ResourcePrefix}v{version.Replace('.', '_')}.MTConnectDevices_{version}.xsd";
            var asm = typeof(ConfigurationPolymorphicXsdValidationTests).Assembly;
            using var schemaStream = asm.GetManifestResourceStream(schemaResource);
            Assert.That(schemaStream, Is.Not.Null,
                $"Embedded schema resource '{schemaResource}' not found in test assembly.");

            var schemaSet = new XmlSchemaSet { XmlResolver = null };
            using (var schemaReader = XmlReader.Create(schemaStream!, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            }))
            {
                schemaSet.Add(null, schemaReader);
            }
            schemaSet.Compile();

            var envelope = BuildEnvelope(version, body);

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet,
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            };

            var errors = new List<string>();
            settings.ValidationEventHandler += (s, e) => errors.Add($"[{e.Severity}] {e.Message}");

            using (var sr = new StringReader(envelope))
            using (var reader = XmlReader.Create(sr, settings))
            {
                while (reader.Read())
                {
                    // drain
                }
            }

            Assert.That(errors, Is.Empty,
                $"v{version} {comboName} XSD validation produced errors:\n  - "
                + string.Join("\n  - ", errors));
        }

        private static string BuildEnvelope(string version, string body)
        {
            // The body is dropped into the appropriate Configuration slot
            // based on the type of element. Motion + CoordinateSystem live
            // on Linear-axis configurations; Transformation and SolidModel
            // also live on Linear-axis configurations.
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
