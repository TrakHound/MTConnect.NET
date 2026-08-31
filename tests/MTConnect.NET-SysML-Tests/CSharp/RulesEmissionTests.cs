using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MTConnect.SysML;
using MTConnect.SysML.CSharp;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using NUnit.Framework;

namespace MTConnect.Tests.SysML.CSharp
{
    /// <summary>
    /// Byte-level golden-fixture coverage on the <c>Rules[]</c> emission
    /// path — one static readonly <c>string[]</c> per generated class,
    /// carrying the raw OCL bodies preserved from
    /// <see cref="UmlConstraint.Body"/> (vendored from mtconnect/
    /// MtconnectTranspiler v2.8). Each test pairs an input model
    /// (either constructed in-memory or hydrated from an XMI fragment
    /// via <see cref="XmlSerializer"/>) with an expected <c>.g.cs</c>
    /// fixture and asserts byte-identical output.
    /// </summary>
    /// <remarks>
    /// Emission variants covered:
    /// <list type="bullet">
    ///   <item>Empty rules — the Rules[] block is elided entirely.</item>
    ///   <item>Single rule — one entry, no trailing comma.</item>
    ///   <item>Multiple rules with escaping — backslashes, double-quotes,
    ///         newlines, and tabs round-trip through the C# string-literal
    ///         escape filter.</item>
    ///   <item>Rules on ComponentType / DataItemType / InterfaceDataItemType
    ///         templates — the alternate concrete-class templates emit
    ///         the same block shape.</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class RulesEmissionTests
    {
        private static string FixturesRoot => Path.Combine(
            TestContext.CurrentContext.TestDirectory, "Fixtures", "Rules");

        private static string ReadFixture(string name) =>
            File.ReadAllText(Path.Combine(FixturesRoot, name));

        // ---- Render-side: Model.scriban ----

        [Test]
        public void Model_scriban_omits_Rules_block_when_Rules_empty()
        {
            var model = new ClassModel
            {
                Id = "Devices.Bar",
                UmlId = "uml-r-1",
                Name = "Bar",
                Description = "A Bar with no rules.",
                Rules = Array.Empty<string>(),
            };

            var rendered = model.RenderModel();
            var expected = ReadFixture("model-empty-rules.expected.g.cs");
            Assert.That(rendered, Is.EqualTo(expected));
        }

        [Test]
        public void Model_scriban_emits_single_rule()
        {
            var model = new ClassModel
            {
                Id = "Devices.Bar",
                UmlId = "uml-r-1",
                Name = "Bar",
                Description = "A Bar with a single rule.",
                Rules = new[] { "self.foo->size() > 0" },
            };

            var rendered = model.RenderModel();
            var expected = ReadFixture("model-single-rule.expected.g.cs");
            Assert.That(rendered, Is.EqualTo(expected));
        }

        [Test]
        public void Model_scriban_emits_multiple_rules_with_escaping()
        {
            var model = new ClassModel
            {
                Id = "Devices.Bar",
                UmlId = "uml-r-1",
                Name = "Bar",
                Description = "A Bar with multiple rules including escaped characters.",
                Rules = new[]
                {
                    "self.name <> ''",
                    "self.description.contains(\"quoted\")",
                    "self.path = 'C:\\\\folder\\\\file'",
                    "line1\nline2\ttabbed",
                },
            };

            var rendered = model.RenderModel();
            var expected = ReadFixture("model-multi-rules-escaped.expected.g.cs");
            Assert.That(rendered, Is.EqualTo(expected));
        }

        [Test]
        public void Model_scriban_omits_new_on_Rules_when_parent_has_no_Rules()
        {
            // Regression coverage for the Axis.g.cs / CS0109 bug (PR #233):
            // a class with a parent (ParentName set) whose parent does NOT
            // itself declare a Rules[] field must NOT get the `new` modifier
            // on its own Rules[] declaration — `new` with nothing to hide
            // raises CS0109 ("does not hide an accessible member"). This
            // mirrors AbstractAxis (no Rules) <- Axis (Rules, wrongly `new`).
            //
            // Deliberately does NOT set ParentHasRules — the default (false)
            // is exactly the state a parent-without-Rules leaves the flag
            // in, so this test exercises the render path using only the
            // fields a real renderer pass would produce for such a class.
            var model = new ClassModel
            {
                Id = "Devices.Configurations.TestAxis",
                UmlId = "uml-r-axis",
                Name = "TestAxis",
                ParentName = "AbstractTestAxis",
                Description = "A TestAxis whose parent has no rules.",
                Rules = new[] { "self.value->size() > 0" },
            };

            var rendered = model.RenderModel();
            var expected = ReadFixture("model-rules-parent-without-rules.expected.g.cs");
            Assert.That(rendered, Is.EqualTo(expected));
            Assert.That(rendered, Does.Not.Contain("new static readonly string[] Rules"),
                "Emitting 'new' here hides nothing on AbstractTestAxis and would raise CS0109.");
        }

        [Test]
        public void Model_scriban_emits_new_on_Rules_when_parent_has_Rules()
        {
            // Symmetric positive case: when the ancestor chain genuinely
            // does declare Rules[], the child's redeclaration DOES need
            // `new` to suppress CS0108 ("hides inherited member"). Proves
            // the fix is a real distinction and not a blanket "never emit
            // new" shortcut.
            var model = new ClassModel
            {
                Id = "Devices.Configurations.TestAxis",
                UmlId = "uml-r-axis",
                Name = "TestAxis",
                ParentName = "AbstractTestAxis",
                ParentHasRules = true,
                Description = "A TestAxis whose parent also has rules.",
                Rules = new[] { "self.value->size() > 0" },
            };

            var rendered = model.RenderModel();
            var expected = ReadFixture("model-rules-parent-with-rules.expected.g.cs");
            Assert.That(rendered, Is.EqualTo(expected));
            Assert.That(rendered, Does.Contain("new static readonly string[] Rules"),
                "Parent declares Rules too, so the redeclaration must hide it via 'new'.");
        }

        // ---- Render-side: Devices.ComponentType.scriban ----

        [Test]
        public void ComponentType_scriban_emits_Rules_block_when_set()
        {
            var model = new ComponentType
            {
                Id = "Devices.Components.RuledComponent",
                UmlId = "uml-cr-1",
                Name = "RuledComponent",
                Type = "RULED",
                DefaultName = "ruled",
                Description = "A component with rules.",
                Rules = new[] { "self.nested->notEmpty()" },
            };

            var rendered = model.RenderModel();
            var expected = ReadFixture("component-with-rules.expected.g.cs");
            Assert.That(rendered, Is.EqualTo(expected));
        }

        // ---- Render-side: Devices.DataItemType.scriban ----

        [Test]
        public void DataItemType_scriban_emits_Rules_block_when_set()
        {
            var model = new DataItemType
            {
                Id = "Devices.DataItems.RuledDataItem",
                UmlId = "uml-drr-1",
                Name = "RuledDataItem",
                Type = "RULED_DI",
                Category = "EVENT",
                Description = "A data item with rules.",
                Rules = new[] { "self.value >= 0" },
                SubTypes = new List<MTConnectDataItemSubType>(),
            };

            var rendered = model.RenderModel();
            var expected = ReadFixture("dataitem-with-rules.expected.g.cs");
            Assert.That(rendered, Is.EqualTo(expected));
        }

        // ---- Render-side: Interfaces.InterfaceDataItemType.scriban ----

        [Test]
        public void InterfaceDataItemType_scriban_emits_Rules_block_when_set()
        {
            var model = new InterfaceDataItemType
            {
                Id = "Interfaces.InterfaceDataItems.RuledInterfaceDataItem",
                UmlId = "uml-iri-1",
                Name = "RuledInterfaceDataItem",
                Type = "RULED_IDI",
                Category = "EVENT",
                Description = "An interface data item with rules.",
                Rules = new[] { "self.value >= 0" },
                SubTypes = new List<MTConnectDataItemSubType>(),
            };

            var rendered = model.RenderModel();
            var expected = ReadFixture("interface-dataitem-with-rules.expected.g.cs");
            Assert.That(rendered, Is.EqualTo(expected));
        }

        // ---- Parse-side wiring ----

        [Test]
        public void Parse_pulls_UmlConstraint_Body_into_MTConnectClassModel_Rules()
        {
            // XMI fixture path: XmlSerializer round-trip of a UmlConstraint
            // exercises the same Body / Language accessors production callers
            // use. The class is then hand-assembled onto a UmlClass so we can
            // drive MTConnectClassModel's constructor deterministically —
            // the model constructor is what emits into Rules from the
            // constraint chain.
            var xmi = File.ReadAllText(Path.Combine(FixturesRoot, "uml-constraint-body.xmi"));
            var constraints = DeserializeConstraints(xmi);
            Assert.That(constraints, Has.Length.EqualTo(3),
                "Fixture must supply three constraints (two populated + one empty).");
            Assert.That(constraints[0].Body, Is.EqualTo("self.name <> ''"));
            Assert.That(constraints[1].Body, Is.EqualTo("self.value >= 0"));
            Assert.That(string.IsNullOrEmpty(constraints[2].Body), Is.True,
                "Third constraint's body should be empty so the parser filters it out.");

            var umlClass = new UmlClass
            {
                Id = "cls-with-rules",
                Name = "RuledClass",
                Constraints = constraints,
            };
            var doc = new XmiDocument();
            var model = new MTConnectClassModel(doc, "Test.RuledClass", umlClass);

            // The two constraints with populated bodies land in Rules; the
            // empty-body constraint is filtered out at parse time so the
            // downstream template never emits an empty entry.
            Assert.That(model.Rules, Is.EqualTo(new[]
            {
                "self.name <> ''",
                "self.value >= 0",
            }));
        }

        [Test]
        public void Parse_yields_empty_Rules_when_class_has_no_constraints()
        {
            var umlClass = new UmlClass
            {
                Id = "cls-noconstraints",
                Name = "PlainClass",
                Constraints = null,
            };
            var doc = new XmiDocument();

            var model = new MTConnectClassModel(doc, "Test.PlainClass", umlClass);
            Assert.That(model.Rules, Is.EqualTo(Array.Empty<string>()));
        }

        [Test]
        public void MTConnectClassModel_Rules_default_is_empty_array()
        {
            Assert.That(new MTConnectClassModel().Rules, Is.EqualTo(Array.Empty<string>()));
        }

        // ---- Helpers ----

        private static UmlConstraint[] DeserializeConstraints(string xml)
        {
            // The fixture wraps the constraints under an <xmi:XMI> ->
            // <uml:Model> -> <packagedElement xmi:type='uml:Class'>. Parse
            // the class fragment and lift its Constraints[] straight out.
            using var reader = new StringReader(xml);
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(reader);
            var classNode = xmlDoc.GetElementsByTagName("packagedElement")[0];
            Assert.That(classNode, Is.Not.Null,
                "Fixture must contain a packagedElement uml:Class.");
            var classXml = classNode!.OuterXml;

            var serializer = new XmlSerializer(typeof(UmlClass));
            using var classReader = new StringReader(classXml);
            var cls = (UmlClass)serializer.Deserialize(classReader)!;
            return cls.Constraints ?? Array.Empty<UmlConstraint>();
        }
    }
}
