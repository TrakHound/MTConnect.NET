// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Modules.WebUI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MTConnect.AgentModule.WebUI.Tests
{
    /// <summary>
    /// Tests for the in-place editing of device XML files.
    /// </summary>
    public class DeviceDocumentTests
    {
        private static string X(params string[] lines) => string.Join("\n", lines) + "\n";

        private static readonly string Sample = X(
            "<Device uuid=\"U1\" name=\"Mill\" id=\"mill\">",
            "\t<Description manufacturer=\"ACME\" model=\"M1\">",
            "\t\tA mill",
            "\t</Description>",
            "\t<DataItems>",
            "\t\t<DataItem category=\"EVENT\" id=\"avail\" type=\"AVAILABILITY\"/>",
            "\t</DataItems>",
            "\t<!-- axes -->",
            "\t<Components>",
            "\t\t<Axes id=\"axes\" name='Axes'>",
            "\t\t\t<Components>",
            "\t\t\t\t<Linear id=\"x\" name=\"X\"/>",
            "\t\t\t\t<Linear id=\"y\" name=\"Y\">",
            "\t\t\t\t\t<DataItems>",
            "\t\t\t\t\t\t<DataItem category=\"SAMPLE\" id=\"ypos\" type=\"POSITION\" subType=\"ACTUAL\" units=\"MILLIMETER\"/>",
            "\t\t\t\t\t</DataItems>",
            "\t\t\t\t</Linear>",
            "\t\t\t</Components>",
            "\t\t</Axes>",
            "\t\t<Controller id=\"cont\" name=\"controller\">",
            "\t\t</Controller>",
            "\t</Components>",
            "</Device>");

        private static string Apply(string text, params DeviceOperation[] operations) => DeviceDocument.ApplyOperations(text, operations);

        private static Dictionary<string, string> A(params string[] pairs)
        {
            var result = new Dictionary<string, string>();
            for (var i = 0; i < pairs.Length; i += 2) result[pairs[i]] = pairs[i + 1];
            return result;
        }

        private static DeviceOperation AddDataItem(string parent, string id, string type, string category = "EVENT") => new DeviceOperation
        {
            Op = "add", Target = parent, Kind = "dataItem", Attributes = A("category", category, "id", id, "type", type)
        };


        [Test]
        public void NoOperations_ReturnsIdenticalText()
        {
            Assert.That(Apply(Sample), Is.EqualTo(Sample));
        }

        [Test]
        public void SetAttributes_ChangesOnlyThatTag()
        {
            var result = Apply(Sample, new DeviceOperation { Op = "setAttributes", Target = "x", Attributes = A("name", "X1", "nativeName", "A") });
            Assert.That(result, Is.EqualTo(Sample.Replace("<Linear id=\"x\" name=\"X\"/>", "<Linear id=\"x\" name=\"X1\" nativeName=\"A\"/>")));
        }

        [Test]
        public void SetAttributes_RemovesAttributesAndKeepsOtherQuoting()
        {
            var result = Apply(Sample, new DeviceOperation { Op = "setAttributes", Target = "axes", Attributes = A("id", "axes2") });
            Assert.That(result, Is.EqualTo(Sample.Replace("<Axes id=\"axes\" name='Axes'>", "<Axes id=\"axes2\" name='Axes'>")));

            result = Apply(Sample, new DeviceOperation { Op = "setAttributes", Target = "ypos", Attributes = A("subType", "") });
            Assert.That(result, Does.Contain("<DataItem category=\"SAMPLE\" id=\"ypos\" type=\"POSITION\" units=\"MILLIMETER\"/>"));
        }

        [Test]
        public void SetAttributes_RejectsDuplicateIds()
        {
            Assert.Throws<ConfigurationEditException>(() => Apply(Sample, new DeviceOperation { Op = "setAttributes", Target = "x", Attributes = A("id", "y") }));
        }

        [Test]
        public void AddDataItem_ToExistingDataItems_GoesAfterTheLast()
        {
            var result = Apply(Sample, AddDataItem("y", "yload", "LOAD", "SAMPLE"));
            Assert.That(result, Is.EqualTo(Sample.Replace(
                "units=\"MILLIMETER\"/>\n",
                "units=\"MILLIMETER\"/>\n\t\t\t\t\t\t<DataItem category=\"SAMPLE\" id=\"yload\" type=\"LOAD\"/>\n")));
        }

        [Test]
        public void AddDataItem_ToSelfClosingComponent_OpensIt()
        {
            var result = Apply(Sample, AddDataItem("x", "xpos", "POSITION", "SAMPLE"));
            Assert.That(result, Does.Contain(
                "\t\t\t\t<Linear id=\"x\" name=\"X\">\n" +
                "\t\t\t\t\t<DataItems>\n" +
                "\t\t\t\t\t\t<DataItem category=\"SAMPLE\" id=\"xpos\" type=\"POSITION\"/>\n" +
                "\t\t\t\t\t</DataItems>\n" +
                "\t\t\t\t</Linear>\n" +
                "\t\t\t\t<Linear id=\"y\""));
        }

        [Test]
        public void AddDataItem_ToEmptyComponent()
        {
            var result = Apply(Sample, AddDataItem("cont", "exec", "EXECUTION"));
            Assert.That(result, Does.Contain(
                "\t\t<Controller id=\"cont\" name=\"controller\">\n" +
                "\t\t\t<DataItems>\n" +
                "\t\t\t\t<DataItem category=\"EVENT\" id=\"exec\" type=\"EXECUTION\"/>\n" +
                "\t\t\t</DataItems>\n" +
                "\t\t</Controller>\n"));
        }

        [Test]
        public void AddDataItem_GoesBeforeComponents()
        {
            var result = Apply(Sample, AddDataItem("axes", "axesmode", "AXIS_INTERLOCK"));
            Assert.That(result, Does.Contain(
                "\t\t<Axes id=\"axes\" name='Axes'>\n" +
                "\t\t\t<DataItems>\n" +
                "\t\t\t\t<DataItem category=\"EVENT\" id=\"axesmode\" type=\"AXIS_INTERLOCK\"/>\n" +
                "\t\t\t</DataItems>\n" +
                "\t\t\t<Components>\n"));
        }

        [Test]
        public void AddComponent_ThenAddDataItemToIt()
        {
            var result = Apply(Sample,
                new DeviceOperation { Op = "add", Target = "mill", Kind = "component", Tag = "Door", Attributes = A("id", "door", "name", "door") },
                AddDataItem("door", "doorstate", "DOOR_STATE"));

            Assert.That(result, Does.Contain(
                "\t\t</Controller>\n" +
                "\t\t<Door id=\"door\" name=\"door\">\n" +
                "\t\t\t<DataItems>\n" +
                "\t\t\t\t<DataItem category=\"EVENT\" id=\"doorstate\" type=\"DOOR_STATE\"/>\n" +
                "\t\t\t</DataItems>\n" +
                "\t\t</Door>\n" +
                "\t</Components>\n"));
        }

        [Test]
        public void Add_RejectsDuplicateIdsAndBadTypes()
        {
            Assert.Throws<ConfigurationEditException>(() => Apply(Sample, AddDataItem("x", "ypos", "POSITION")));
            Assert.Throws<ConfigurationEditException>(() => Apply(Sample, new DeviceOperation { Op = "add", Target = "mill", Kind = "component", Tag = "Bad Tag", Attributes = A("id", "b") }));
            Assert.Throws<ConfigurationEditException>(() => Apply(Sample, AddDataItem("nope", "z", "POSITION")));
        }

        [Test]
        public void Remove_RemovesTheLine()
        {
            var result = Apply(Sample, new DeviceOperation { Op = "remove", Target = "x" });
            Assert.That(result, Is.EqualTo(Sample.Replace("\t\t\t\t<Linear id=\"x\" name=\"X\"/>\n", "")));
        }

        [Test]
        public void Remove_LastItem_RemovesItsContainer()
        {
            var result = Apply(Sample, new DeviceOperation { Op = "remove", Target = "ypos" });
            Assert.That(result, Does.Contain("\t\t\t\t<Linear id=\"y\" name=\"Y\">\n\t\t\t\t</Linear>\n"));
            Assert.That(DeviceDocument.Parse(result).FindById("y").Elements().Any(), Is.False);
        }

        [Test]
        public void Remove_TheDeviceOfASingleDeviceFile_IsRejected()
        {
            Assert.Throws<ConfigurationEditException>(() => Apply(Sample, new DeviceOperation { Op = "remove", Target = "mill" }));
        }

        [Test]
        public void Move_SwapsWithTheSibling()
        {
            var result = Apply(Sample, new DeviceOperation { Op = "move", Target = "y", Direction = "up" });
            var document = DeviceDocument.Parse(result);
            Assert.That(document.FindById("axes").Descendants().Where(e => e.Name.LocalName == "Linear").Select(e => (string)e.Attribute("id")), Is.EqualTo(new[] { "y", "x" }));
            Assert.That(result, Does.Contain("\t\t\t\t<Linear id=\"x\" name=\"X\"/>\n\t\t\t</Components>"));
        }

        [Test]
        public void SetDescription_ReplacesAddsAndRemoves()
        {
            var result = Apply(Sample, new DeviceOperation { Op = "setDescription", Target = "mill", Attributes = A("manufacturer", "ACME", "model", "M2"), Text = "A mill" });
            Assert.That(result, Does.Contain("\t<Description manufacturer=\"ACME\" model=\"M2\">A mill</Description>\n\t<DataItems>"));

            result = Apply(Sample, new DeviceOperation { Op = "setDescription", Target = "x", Attributes = A("serialNumber", "S & 1") });
            Assert.That(result, Does.Contain("<Linear id=\"x\" name=\"X\">\n\t\t\t\t\t<Description serialNumber=\"S &amp; 1\"/>\n\t\t\t\t</Linear>"));

            result = Apply(Sample, new DeviceOperation { Op = "setDescription", Target = "mill", Attributes = A() });
            Assert.That(result, Does.StartWith("<Device uuid=\"U1\" name=\"Mill\" id=\"mill\">\n\t<DataItems>"));
        }

        [Test]
        public void SetConstraints_AddsAndRemoves()
        {
            var result = Apply(Sample, new DeviceOperation { Op = "setConstraints", Target = "ypos", Minimum = "0", Maximum = "500" });
            Assert.That(result, Does.Contain(
                "units=\"MILLIMETER\">\n" +
                "\t\t\t\t\t\t\t<Constraints>\n" +
                "\t\t\t\t\t\t\t\t<Minimum>0</Minimum>\n" +
                "\t\t\t\t\t\t\t\t<Maximum>500</Maximum>\n" +
                "\t\t\t\t\t\t\t</Constraints>\n" +
                "\t\t\t\t\t\t</DataItem>\n"));

            var removed = Apply(result, new DeviceOperation { Op = "setConstraints", Target = "ypos" });
            Assert.That(DeviceDocument.Parse(removed).FindById("ypos").Elements().Any(), Is.False);
        }

        [Test]
        public void ReplaceXml_ReplacesTheElementWithIndentation()
        {
            var result = Apply(Sample, new DeviceOperation { Op = "replaceXml", Target = "x", Xml = "<Linear id=\"x\" name=\"X\">\n\t<Description model=\"ball screw\"/>\n</Linear>" });
            Assert.That(result, Does.Contain("\t\t\t\t<Linear id=\"x\" name=\"X\">\n\t\t\t\t\t<Description model=\"ball screw\"/>\n\t\t\t\t</Linear>\n"));

            Assert.Throws<ConfigurationEditException>(() => Apply(Sample, new DeviceOperation { Op = "replaceXml", Target = "x", Xml = "<Linear id=\"x\">" }));
            Assert.Throws<ConfigurationEditException>(() => Apply(Sample, new DeviceOperation { Op = "replaceXml", Target = "x", Xml = "<A id=\"1\"/><B id=\"2\"/>" }));
        }

        [Test]
        public void WindowsLineEndings_ArePreserved()
        {
            var text = Sample.Replace("\n", "\r\n");
            var result = Apply(text, AddDataItem("x", "xpos", "POSITION", "SAMPLE"), new DeviceOperation { Op = "remove", Target = "avail" });
            Assert.That(result.Replace("\r\n", ""), Does.Not.Contain("\n"));
            Assert.That(result, Does.Contain("<Linear id=\"x\" name=\"X\">\r\n"));
        }

        [Test]
        public void NamespacedDevicesDocument_CanBeEdited()
        {
            var text = X(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                "<MTConnectDevices xmlns=\"urn:mtconnect.org:MTConnectDevices:2.2\">",
                "  <Header instanceId=\"1\" />",
                "  <Devices>",
                "    <Device id=\"d1\" name=\"M\" uuid=\"u\">",
                "      <DataItems>",
                "        <DataItem category=\"EVENT\" id=\"avail\" type=\"AVAILABILITY\" />",
                "      </DataItems>",
                "    </Device>",
                "  </Devices>",
                "</MTConnectDevices>");

            var document = DeviceDocument.Parse(text);
            Assert.That(document.GetDevices().Select(d => (string)d.Attribute("id")), Is.EqualTo(new[] { "d1" }));
            Assert.That(document.IndentUnit, Is.EqualTo("  "));

            var result = Apply(text, AddDataItem("d1", "exec", "EXECUTION"));
            Assert.That(result, Does.Contain("type=\"AVAILABILITY\" />\n        <DataItem category=\"EVENT\" id=\"exec\" type=\"EXECUTION\"/>\n      </DataItems>"));
            Assert.That(result, Does.StartWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<MTConnectDevices"));
        }

        [Test]
        public void MoveTo_AnotherComponent_MovesTheTextAndCleansUpContainers()
        {
            // ypos is the only data item of y: its container goes, and x (self-closing) gets one
            var result = Apply(Sample, new DeviceOperation { Op = "moveTo", Target = "ypos", Parent = "x" });

            Assert.That(result, Does.Contain(
                "\t\t\t\t<Linear id=\"x\" name=\"X\">\n" +
                "\t\t\t\t\t<DataItems>\n" +
                "\t\t\t\t\t\t<DataItem category=\"SAMPLE\" id=\"ypos\" type=\"POSITION\" subType=\"ACTUAL\" units=\"MILLIMETER\"/>\n" +
                "\t\t\t\t\t</DataItems>\n" +
                "\t\t\t\t</Linear>\n" +
                "\t\t\t\t<Linear id=\"y\" name=\"Y\">\n" +
                "\t\t\t\t</Linear>\n"));
        }

        [Test]
        public void MoveTo_BeforeASibling_Reorders()
        {
            var result = Apply(Sample, new DeviceOperation { Op = "moveTo", Target = "y", Parent = "axes", Before = "x" });
            Assert.That(result, Does.Contain("\t\t\t<Components>\n\t\t\t\t<Linear id=\"y\" name=\"Y\">\n"));
            Assert.That(result, Does.Contain("\t\t\t\t</Linear>\n\t\t\t\t<Linear id=\"x\" name=\"X\"/>\n\t\t\t</Components>"));
        }

        [Test]
        public void MoveTo_ComponentWithChildren_KeepsItsContentAndReindents()
        {
            // Move the Linear 'y' (with its data items) from Axes up to the device's Components
            var result = Apply(Sample, new DeviceOperation { Op = "moveTo", Target = "y", Parent = "mill" });

            Assert.That(result, Does.Contain(
                "\t\t</Controller>\n" +
                "\t\t<Linear id=\"y\" name=\"Y\">\n" +
                "\t\t\t<DataItems>\n" +
                "\t\t\t\t<DataItem category=\"SAMPLE\" id=\"ypos\" type=\"POSITION\" subType=\"ACTUAL\" units=\"MILLIMETER\"/>\n" +
                "\t\t\t</DataItems>\n" +
                "\t\t</Linear>\n" +
                "\t</Components>"));
            var moved = DeviceDocument.Parse(result);
            Assert.That(moved.FindById("y").Parent.Parent, Is.SameAs(moved.FindById("mill")));
        }

        [Test]
        public void MoveTo_IntoItselfOrToAnInvalidParent_IsRejected()
        {
            Assert.Throws<ConfigurationEditException>(() => Apply(Sample, new DeviceOperation { Op = "moveTo", Target = "axes", Parent = "y" }));
            Assert.Throws<ConfigurationEditException>(() => Apply(Sample, new DeviceOperation { Op = "moveTo", Target = "x", Parent = "ypos" }));
            Assert.Throws<ConfigurationEditException>(() => Apply(Sample, new DeviceOperation { Op = "moveTo", Target = "mill", Parent = "axes" }));
        }

        [Test]
        public void AddSpecification_CreatesConfigurationAndBlock()
        {
            var result = Apply(Sample, new DeviceOperation
            {
                Op = "addConfigItem", Target = "y", Block = "Specifications",
                Attributes = A("type", "POSITION", "units", "MILLIMETER"),
                Children = A("Maximum", "500", "Minimum", "0")
            });

            Assert.That(result, Does.Contain(
                "\t\t\t\t<Linear id=\"y\" name=\"Y\">\n" +
                "\t\t\t\t\t<Configuration>\n" +
                "\t\t\t\t\t\t<Specifications>\n" +
                "\t\t\t\t\t\t\t<Specification type=\"POSITION\" units=\"MILLIMETER\">\n" +
                "\t\t\t\t\t\t\t\t<Maximum>500</Maximum>\n" +
                "\t\t\t\t\t\t\t\t<Minimum>0</Minimum>\n" +
                "\t\t\t\t\t\t\t</Specification>\n" +
                "\t\t\t\t\t\t</Specifications>\n" +
                "\t\t\t\t\t</Configuration>\n" +
                "\t\t\t\t\t<DataItems>\n"));
        }

        [Test]
        public void SetSpecification_InsertsLimitsInOrderAndRemovesEmptyOnes()
        {
            var text = Apply(Sample, new DeviceOperation { Op = "addConfigItem", Target = "y", Block = "Specifications", Attributes = A("type", "POSITION"), Children = A("Maximum", "500", "Minimum", "0") });

            var result = Apply(text, new DeviceOperation
            {
                Op = "setConfigItem", Target = "y", Block = "Specifications", Index = 0,
                Attributes = A("units", "MILLIMETER"),
                Children = A("Nominal", "250", "Minimum", "")
            });

            Assert.That(result, Does.Contain(
                "<Specification type=\"POSITION\" units=\"MILLIMETER\">\n" +
                "\t\t\t\t\t\t\t\t<Maximum>500</Maximum>\n" +
                "\t\t\t\t\t\t\t\t<Nominal>250</Nominal>\n" +
                "\t\t\t\t\t\t\t</Specification>"));
        }

        [Test]
        public void CoordinateSystemAndMotion_WithTransformation()
        {
            var result = Apply(Sample,
                new DeviceOperation { Op = "addConfigItem", Target = "mill", Block = "CoordinateSystems", Attributes = A("id", "machine", "type", "MACHINE"), Children = A("Transformation/Translation", "0 0 0", "Transformation/Rotation", "0 0 90") },
                new DeviceOperation { Op = "addConfigItem", Target = "x", Block = "Motion", Attributes = A("id", "xmotion", "type", "PRISMATIC", "actuation", "DIRECT", "coordinateSystemIdRef", "machine"), Children = A("Axis", "1 0 0") });

            var document = DeviceDocument.Parse(result);
            var cs = document.FindById("machine");
            Assert.That(cs.Parent.Name.LocalName, Is.EqualTo("CoordinateSystems"));
            Assert.That(cs.Elements().Single().Name.LocalName, Is.EqualTo("Transformation"));
            Assert.That(cs.Elements().Single().Elements().Select(e => e.Name.LocalName + "=" + e.Value), Is.EqualTo(new[] { "Translation=0 0 0", "Rotation=0 0 90" }));

            // The device's Configuration goes after its Description and before its DataItems
            Assert.That(document.FindById("mill").Elements().Select(e => e.Name.LocalName), Is.EqualTo(new[] { "Description", "Configuration", "DataItems", "Components" }));
            Assert.That(document.FindById("xmotion").Parent.Name.LocalName, Is.EqualTo("Configuration"));
            Assert.Throws<ConfigurationEditException>(() => Apply(result, new DeviceOperation { Op = "addConfigItem", Target = "x", Block = "Motion", Attributes = A("id", "m2") }));
        }

        [Test]
        public void RemoveConfigItem_CleansUpEmptyBlocks()
        {
            var text = Apply(Sample,
                new DeviceOperation { Op = "addConfigItem", Target = "y", Block = "Specifications", Attributes = A("type", "POSITION"), Children = A("Maximum", "1") },
                new DeviceOperation { Op = "addConfigItem", Target = "y", Block = "Specifications", Attributes = A("type", "LOAD"), Children = A("Maximum", "2") });

            var oneLeft = Apply(text, new DeviceOperation { Op = "removeConfigItem", Target = "y", Block = "Specifications", Index = 0 });
            Assert.That(DeviceDocument.Parse(oneLeft).FindById("y").Descendants().Count(e => e.Name.LocalName == "Specification"), Is.EqualTo(1));
            Assert.That(oneLeft, Does.Contain("<Specification type=\"LOAD\">"));

            var none = Apply(oneLeft, new DeviceOperation { Op = "removeConfigItem", Target = "y", Block = "Specifications", Index = 0 });
            Assert.That(none, Is.EqualTo(Sample));
        }

        [Test]
        public void ConfigurationItems_AppearInTheTree()
        {
            var text = Apply(Sample, new DeviceOperation { Op = "addConfigItem", Target = "y", Block = "Specifications", Attributes = A("type", "POSITION"), Children = A("Maximum", "500") });
            var document = DeviceDocument.Parse(text);
            var json = JsonSerializer.Serialize(document.BuildNode(document.FindById("y")));
            var configuration = JsonDocument.Parse(json).RootElement.GetProperty("configuration");

            var specification = configuration.GetProperty("specifications").EnumerateArray().Single();
            Assert.That(specification.GetProperty("attributes").GetProperty("type").GetString(), Is.EqualTo("POSITION"));
            Assert.That(specification.GetProperty("values").GetProperty("Maximum").GetString(), Is.EqualTo("500"));
            Assert.That(configuration.GetProperty("motion").ValueKind, Is.EqualTo(JsonValueKind.Null));
        }

        [TestCase("okuma.xml")]
        [TestCase("mazak.xml")]
        public void SampleDeviceFiles_RoundTripAndCanBeEdited(string fileName)
        {
            var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", "devices", fileName));
            var document = DeviceDocument.Parse(text);
            var device = document.GetDevices().Single();
            Assert.That(Apply(text), Is.EqualTo(text));
            Assert.DoesNotThrow(() => JsonSerializer.Serialize(document.BuildNode(device)));

            // Add a data item to every component, then remove one existing data item; the result must stay well formed
            var components = device.Descendants().Where(e => DeviceDocument.GetKind(e) == "component").Select(e => (string)e.Attribute("id")).ToList();
            var operations = components.Select((id, i) => AddDataItem(id, "test_" + i, "EXECUTION")).ToList();
            var firstDataItem = device.Descendants().First(e => DeviceDocument.GetKind(e) == "dataItem");
            operations.Add(new DeviceOperation { Op = "remove", Target = (string)firstDataItem.Attribute("id") });

            var result = Apply(text, operations.ToArray());
            var edited = DeviceDocument.Parse(result);
            Assert.That(edited.FindById("test_0"), Is.Not.Null);
            Assert.That(edited.FindById((string)firstDataItem.Attribute("id")), Is.Null);
            Assert.That(edited.GetDevices().Single().Descendants().Count(e => DeviceDocument.GetKind(e) == "dataItem"),
                Is.EqualTo(device.Descendants().Count(e => DeviceDocument.GetKind(e) == "dataItem") + components.Count - 1));
        }

        [Test]
        public void Tree_DescribesDevicesComponentsAndDataItems()
        {
            var document = DeviceDocument.Parse(Sample);
            var json = JsonSerializer.Serialize(document.BuildNode(document.GetDevices()[0]));
            var device = JsonDocument.Parse(json).RootElement;

            Assert.That(device.GetProperty("kind").GetString(), Is.EqualTo("device"));
            Assert.That(device.GetProperty("description").GetProperty("text").GetString(), Is.EqualTo("A mill"));
            Assert.That(device.GetProperty("description").GetProperty("attributes").GetProperty("model").GetString(), Is.EqualTo("M1"));

            var children = device.GetProperty("children").EnumerateArray().ToList();
            Assert.That(children.Select(c => c.GetProperty("id").GetString()), Is.EqualTo(new[] { "avail", "axes", "cont" }));
            Assert.That(children[0].GetProperty("kind").GetString(), Is.EqualTo("dataItem"));

            var y = children[1].GetProperty("children").EnumerateArray().ElementAt(1);
            Assert.That(y.GetProperty("tag").GetString(), Is.EqualTo("Linear"));
            Assert.That(y.GetProperty("children").EnumerateArray().Single().GetProperty("attributes").GetProperty("subType").GetString(), Is.EqualTo("ACTUAL"));
            Assert.That(y.GetProperty("xml").GetString(), Does.StartWith("<Linear id=\"y\" name=\"Y\">\n\t<DataItems>"));
        }
    }

    /// <summary>
    /// Tests for validating device files and creating new ones.
    /// </summary>
    public class DeviceFilesTests
    {
        private string _folder;

        [SetUp]
        public void SetUp()
        {
            _folder = Path.Combine(Path.GetTempPath(), "webui-devices-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_folder);
            File.WriteAllText(Path.Combine(_folder, "okuma.xml"), DeviceFiles.CreateDeviceText("okuma", "Okuma", "U-OKUMA"));
        }

        [TearDown]
        public void TearDown()
        {
            try { Directory.Delete(_folder, true); } catch { }
        }

        [Test]
        public void Validate_ReportsMissingAndDuplicateValues()
        {
            var files = new DeviceFiles(_folder, useLibraryReader: false);
            var path = Path.Combine(_folder, "new.xml");

            var ex = Assert.Throws<ConfigurationEditException>(() => files.Validate(
                "<Device id=\"d\" name=\"Okuma\" uuid=\"U-OKUMA\"><DataItems><DataItem id=\"a\" type=\"X\" category=\"BAD\"/><DataItem id=\"a\" category=\"EVENT\"/></DataItems></Device>", path));

            Assert.That(ex.Message, Does.Contain("invalid category"));
            Assert.That(ex.Message, Does.Contain("used more than once"));
            Assert.That(ex.Message, Does.Contain("has no type"));
            Assert.That(ex.Message, Does.Contain("uuid 'U-OKUMA' is already used"));
            Assert.That(ex.Message, Does.Contain("name 'Okuma' is already used"));
        }

        [Test]
        public void NewFile_IsCreatedFromTheTemplate()
        {
            var files = new DeviceFiles(_folder, useLibraryReader: false);
            var path = files.PrepareNewFile(null, "Haas VF2", null, out var text);

            Assert.That(Path.GetFileName(path), Is.EqualTo("Haas VF2.xml"));
            var device = DeviceDocument.Parse(text).GetDevices().Single();
            Assert.That((string)device.Attribute("id"), Is.EqualTo("Haas_VF2"));
            Assert.That(Guid.TryParse((string)device.Attribute("uuid"), out _), Is.True);
            Assert.That(device.Descendants().Count(e => e.Name.LocalName == "DataItem"), Is.EqualTo(3));

            Assert.Throws<ConfigurationEditException>(() => files.PrepareNewFile("okuma.xml", "Other", null, out _));
            Assert.Throws<ConfigurationEditException>(() => files.PrepareNewFile("../evil.xml", "Evil", null, out _));
            Assert.Throws<ConfigurationEditException>(() => files.PrepareNewFile(null, "Okuma", null, out _));
        }

        [Test]
        public void Resolve_OnlyFindsListedFiles()
        {
            var files = new DeviceFiles(_folder, useLibraryReader: false);
            Assert.That(files.Resolve("OKUMA.xml"), Is.Not.Null);
            Assert.That(files.Resolve("..\\okuma.xml"), Is.Null);
            Assert.That(files.Resolve("missing.xml"), Is.Null);
        }
    }
}
