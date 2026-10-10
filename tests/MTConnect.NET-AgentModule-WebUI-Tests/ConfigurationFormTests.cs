// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Modules.WebUI;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace MTConnect.AgentModule.WebUI.Tests
{
    /// <summary>A sample module settings class.</summary>
    public class TestModuleConfiguration
    {
        /// <summary>Severity used by the test module.</summary>
        public enum TestLevel { Low, Medium, High }

        /// <summary>The port to listen on.</summary>
        public int Port { get; set; } = 7000;

        /// <summary>The <see cref="Port"/> host name.</summary>
        public string Server { get; set; }

        /// <summary>Whether the module is enabled.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>The severity.</summary>
        public TestLevel Level { get; set; } = TestLevel.Medium;

        /// <summary>A list value (edited as YAML).</summary>
        public List<string> Tags { get; set; }

        /// <summary>A password.</summary>
        public string CertificatePassword { get; set; }

        /// <summary>Stored under an alias.</summary>
        [YamlMember(Alias = "renamed")]
        public string Original { get; set; }

        /// <summary>Not stored in the file.</summary>
        [YamlIgnore]
        public string Hidden { get; set; }

        /// <summary>A map of names to values.</summary>
        public Dictionary<string, string> Accept { get; set; }

        /// <summary>A list of objects.</summary>
        public List<TestFileConfiguration> Files { get; set; }

        /// <summary>A nested object with a nested object.</summary>
        public TestTlsConfiguration Tls { get; set; }

        /// <summary>Not describable on the form (stays YAML).</summary>
        public Dictionary<string, object> Engine { get; set; }
    }

    /// <summary>A list item settings class.</summary>
    public class TestFileConfiguration
    {
        /// <summary>The request path.</summary>
        public string Path { get; set; }

        /// <summary>The file location.</summary>
        public string Location { get; set; }
    }

    /// <summary>A nested settings class.</summary>
    public class TestTlsConfiguration
    {
        /// <summary>Certificate files.</summary>
        public TestPemConfiguration Pem { get; set; }

        /// <summary>Whether to verify clients.</summary>
        public bool VerifyClientCertificate { get; set; }
    }

    /// <summary>A nested-nested settings class.</summary>
    public class TestPemConfiguration
    {
        /// <summary>The certificate.</summary>
        public string CertificatePath { get; set; }

        /// <summary>The key password.</summary>
        public string PrivateKeyPassword { get; set; }
    }

    /// <summary>
    /// Tests for building the form model and applying form changes.
    /// </summary>
    public class ConfigurationFormTests
    {
        private static string Y(params string[] lines) => string.Join("\n", lines) + "\n";

        private static readonly string Sample = Y(
            "devices: devices # device files",
            "modules:",
            "- test-module: # first",
            "    port: 7878",
            "    tags:",
            "    - a",
            "    - b",
            "    customKey: kept",
            "- unknown-module:",
            "    anything: 1",
            "processors:",
            "- python:",
            "    directory: processors",
            "observationBufferSize: 150000");

        private static ConfigurationForm CreateForm()
        {
            var testFields = ConfigurationSchema.GetFields(typeof(TestModuleConfiguration));
            var moduleTypes = new List<ConfigurationEntryType>
            {
                new ConfigurationEntryType { TypeId = "test-module", SettingsType = typeof(TestModuleConfiguration), Fields = testFields }
            };
            var processorTypes = new List<ConfigurationEntryType>
            {
                new ConfigurationEntryType { TypeId = "python" }
            };
            return new ConfigurationForm(typeof(AgentApplicationConfiguration), moduleTypes, processorTypes);
        }

        private static ConfigurationFormRequest ModuleChange(int index, Dictionary<string, string> values) => new ConfigurationFormRequest
        {
            Entries = new List<ConfigurationEntryChange> { new ConfigurationEntryChange { Section = "modules", Index = index, Values = values } }
        };


        [Test]
        public void Fields_MirrorTheYamlKeysTheAgentReads()
        {
            var fields = ConfigurationSchema.GetFields(typeof(TestModuleConfiguration));
            var keys = fields.Select(o => o.Key).ToList();

            Assert.That(keys, Is.EqualTo(new[] { "port", "server", "enabled", "level", "tags", "certificatePassword", "renamed", "accept", "files", "tls", "engine" }));
            Assert.That(fields.Single(o => o.Key == "port").Type, Is.EqualTo(ConfigurationField.TypeInteger));
            Assert.That(fields.Single(o => o.Key == "port").Default, Is.EqualTo("7000"));
            Assert.That(fields.Single(o => o.Key == "enabled").Default, Is.EqualTo("true"));
            Assert.That(fields.Single(o => o.Key == "level").Options, Is.EqualTo(new[] { "Low", "Medium", "High" }));
            Assert.That(fields.Single(o => o.Key == "certificatePassword").Secret, Is.True);

            // Lists, maps and nested objects are described for the structured editors
            Assert.That(fields.Single(o => o.Key == "tags").Type, Is.EqualTo(ConfigurationField.TypeList));
            Assert.That(fields.Single(o => o.Key == "tags").ItemType, Is.EqualTo(ConfigurationField.TypeString));
            Assert.That(fields.Single(o => o.Key == "accept").Type, Is.EqualTo(ConfigurationField.TypeMap));
            Assert.That(fields.Single(o => o.Key == "files").ItemFields.Select(f => f.Key), Is.EqualTo(new[] { "path", "location" }));
            var tls = fields.Single(o => o.Key == "tls");
            Assert.That(tls.Type, Is.EqualTo(ConfigurationField.TypeObject));
            Assert.That(tls.Fields.Single(f => f.Key == "pem").Fields.Single(f => f.Key == "privateKeyPassword").Secret, Is.True);
            Assert.That(fields.Single(o => o.Key == "engine").Type, Is.EqualTo(ConfigurationField.TypeComplex));
        }

        [Test]
        public void AgentFields_IncludeCommonSettingsButNotSections()
        {
            var keys = ConfigurationSchema.GetAgentFields(typeof(AgentApplicationConfiguration)).Select(o => o.Key).ToList();

            Assert.That(keys, Does.Contain("observationBufferSize"));
            Assert.That(keys, Does.Contain("defaultVersion"));
            Assert.That(keys, Does.Contain("enableValidation"));
            Assert.That(keys, Does.Not.Contain("modules"));
            Assert.That(keys, Does.Not.Contain("processors"));
            Assert.That(keys, Does.Not.Contain("path"));
        }

        [Test]
        public void ChangingSettings_UpdatesOnlyThoseKeys()
        {
            var form = CreateForm();
            var result = form.Apply(Sample, ModuleChange(0, new Dictionary<string, string>
            {
                ["port"] = "7900",
                ["level"] = "high",
                ["server"] = "machine 1"
            }));

            Assert.That(result, Is.EqualTo(Sample
                .Replace("    port: 7878\n", "    port: 7900\n")
                .Replace("    customKey: kept\n", "    customKey: kept\n    level: High\n    server: 'machine 1'\n")));
        }

        [Test]
        public void EmptyValue_RemovesTheKey()
        {
            var form = CreateForm();
            var result = form.Apply(Sample, ModuleChange(0, new Dictionary<string, string> { ["port"] = "" }));

            Assert.That(result, Is.EqualTo(Sample.Replace("    port: 7878\n", "")));
        }

        [Test]
        public void AgentSettings_CanBeChanged()
        {
            var form = CreateForm();
            var result = form.Apply(Sample, new ConfigurationFormRequest
            {
                Agent = new Dictionary<string, string> { ["observationBufferSize"] = "200000", ["enableValidation"] = "true" }
            });

            Assert.That(result, Is.EqualTo(Sample
                .Replace("observationBufferSize: 150000\n", "observationBufferSize: 200000\nenableValidation: true\n")));
        }

        [TestCase("port", "abc")]
        [TestCase("enabled", "maybe")]
        [TestCase("level", "Extreme")]
        public void InvalidValues_AreRejected(string key, string value)
        {
            var form = CreateForm();
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, ModuleChange(0, new Dictionary<string, string> { [key] = value })));
        }

        [Test]
        public void ComplexAndUnknownKeys_CannotBeEditedOnTheForm()
        {
            var form = CreateForm();
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, ModuleChange(0, new Dictionary<string, string> { ["tags"] = "x" })));
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, ModuleChange(0, new Dictionary<string, string> { ["customKey"] = "x" })));
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, Structured(0, "engine", "{\"a\": \"1\"}")));
        }

        private static ConfigurationFormRequest Structured(int index, string key, string json) => new ConfigurationFormRequest
        {
            Entries = new List<ConfigurationEntryChange>
            {
                new ConfigurationEntryChange
                {
                    Section = "modules", Index = index,
                    Structured = new Dictionary<string, System.Text.Json.JsonElement> { [key] = System.Text.Json.JsonDocument.Parse(json).RootElement.Clone() }
                }
            }
        };

        [Test]
        public void StructuredList_ReplacesOnlyThatKey()
        {
            var form = CreateForm();
            var result = form.Apply(Sample, Structured(0, "tags", "[\"a\", \"two words\", \"\", \"c\"]"));
            Assert.That(result, Is.EqualTo(Sample.Replace("    tags:\n    - a\n    - b\n", "    tags:\n    - a\n    - 'two words'\n    - c\n")));
        }

        [Test]
        public void StructuredValues_AreAddedInTheFileStyle()
        {
            var form = CreateForm();
            var request = Structured(0, "accept", "{\"text/xml\": \"xml\", \"application/json\": \"json\"}");
            request.Entries[0].Structured["files"] = System.Text.Json.JsonDocument.Parse("[{\"path\": \"schemas\", \"location\": \"schemas\"}, {\"path\": \"styles\"}]").RootElement.Clone();
            request.Entries[0].Structured["tls"] = System.Text.Json.JsonDocument.Parse("{\"pem\": {\"certificatePath\": \"c:\\\\certs\\\\a.crt\", \"privateKeyPassword\": \"p@ss word\"}, \"verifyClientCertificate\": \"true\"}").RootElement.Clone();

            var result = form.Apply(Sample, request);
            Assert.That(result, Does.Contain(
                "    customKey: kept\n" +
                "    accept:\n" +
                "      text/xml: xml\n" +
                "      application/json: json\n" +
                "    files:\n" +
                "    - path: schemas\n" +
                "      location: schemas\n" +
                "    - path: styles\n" +
                "    tls:\n" +
                "      pem:\n" +
                "        certificatePath: c:\\certs\\a.crt\n" +
                "        privateKeyPassword: 'p@ss word'\n" +
                "      verifyClientCertificate: true\n" +
                "- unknown-module:"));

            // The written YAML reads back into the settings class
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance).IgnoreUnmatchedProperties().Build();
            var document = ConfigurationDocument.Parse(result);
            var settings = deserializer.Deserialize<Dictionary<string, TestModuleConfiguration>>(document.GetEntryText(document.GetEntries("modules")[0]))["test-module"];
            Assert.That(settings.Accept["application/json"], Is.EqualTo("json"));
            Assert.That(settings.Files.Select(f => f.Path), Is.EqualTo(new[] { "schemas", "styles" }));
            Assert.That(settings.Tls.Pem.PrivateKeyPassword, Is.EqualTo("p@ss word"));
            Assert.That(settings.Tls.VerifyClientCertificate, Is.True);
        }

        [Test]
        public void StructuredValues_EmptyOrNullRemovesTheKey()
        {
            var form = CreateForm();
            Assert.That(form.Apply(Sample, Structured(0, "tags", "[]")), Is.EqualTo(Sample.Replace("    tags:\n    - a\n    - b\n", "")));
            Assert.That(form.Apply(Sample, Structured(0, "tags", "null")), Is.EqualTo(Sample.Replace("    tags:\n    - a\n    - b\n", "")));
        }

        [Test]
        public void StructuredValues_AreValidated()
        {
            var form = CreateForm();
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, Structured(0, "files", "[{\"nope\": \"x\"}]")));
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, Structured(0, "tls", "{\"verifyClientCertificate\": \"maybe\"}")));
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, Structured(0, "tags", "{\"a\": \"b\"}")));
        }

        [Test]
        public void Model_IncludesStructuredDataAndNestedFields()
        {
            var form = CreateForm();
            var json = System.Text.Json.JsonSerializer.Serialize(form.BuildModel(Sample, "agent.config.yaml", true));
            var model = System.Text.Json.JsonDocument.Parse(json).RootElement;

            var entry = model.GetProperty("entries")[0];
            Assert.That(entry.GetProperty("data").GetProperty("tags").EnumerateArray().Select(o => o.GetString()), Is.EqualTo(new[] { "a", "b" }));

            var tls = model.GetProperty("types").GetProperty("modules")[0].GetProperty("fields").EnumerateArray().Single(f => f.GetProperty("key").GetString() == "tls");
            Assert.That(tls.GetProperty("type").GetString(), Is.EqualTo("object"));
            Assert.That(tls.GetProperty("fields").EnumerateArray().Select(f => f.GetProperty("key").GetString()), Is.EqualTo(new[] { "pem", "verifyClientCertificate" }));
        }

        [Test]
        public void RemoveEntry_RemovesItsLines()
        {
            var form = CreateForm();
            var result = form.Apply(Sample, new ConfigurationFormRequest { Remove = new List<ConfigurationEntryRef> { new ConfigurationEntryRef { Section = "modules", Index = 1 } } });
            Assert.That(result, Is.EqualTo(Sample.Replace("- unknown-module:\n    anything: 1\n", "")));

            // A removed entry cannot also be changed
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, new ConfigurationFormRequest
            {
                Remove = new List<ConfigurationEntryRef> { new ConfigurationEntryRef { Section = "modules", Index = 0 } },
                Entries = new List<ConfigurationEntryChange> { new ConfigurationEntryChange { Section = "modules", Index = 0, Values = new Dictionary<string, string> { ["port"] = "1" } } }
            }));
        }

        [Test]
        public void RemoveLastEntryAndAddAnother_InOneChange()
        {
            var text = Y("modules:", "", "- test-module:", "    port: 1", "", "- unknown-module:", "    anything: 1", "", "observationBufferSize: 10");
            var form = CreateForm();
            var result = form.Apply(text, new ConfigurationFormRequest
            {
                Remove = new List<ConfigurationEntryRef> { new ConfigurationEntryRef { Section = "modules", Index = 1 } },
                Add = new List<ConfigurationEntryAdd> { new ConfigurationEntryAdd { Section = "modules", TypeId = "test-module", Values = new Dictionary<string, string> { ["port"] = "2" } } }
            });

            Assert.That(result, Is.EqualTo(Y("modules:", "", "- test-module:", "    port: 1", "", "- test-module:", "    port: 2", "", "observationBufferSize: 10")));
        }

        [Test]
        public void AddProcessor()
        {
            var form = CreateForm();
            var result = form.Apply(Sample, new ConfigurationFormRequest
            {
                Add = new List<ConfigurationEntryAdd> { new ConfigurationEntryAdd { Section = "processors", TypeId = "python" } }
            });
            Assert.That(ConfigurationDocument.Parse(result).GetEntries("processors").Select(e => e.TypeId), Is.EqualTo(new[] { "python", "python" }));
        }

        [Test]
        public void EntriesWithoutAForm_AreEditedAsYaml()
        {
            var form = CreateForm();
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, ModuleChange(1, new Dictionary<string, string> { ["anything"] = "2" })));

            var result = form.Apply(Sample, new ConfigurationFormRequest
            {
                Entries = new List<ConfigurationEntryChange>
                {
                    new ConfigurationEntryChange { Section = "modules", Index = 1, Yaml = "unknown-module:\n  anything: 2\n  more: true" }
                }
            });

            Assert.That(result, Does.Contain("- unknown-module:\n    anything: 2\n    more: true\nprocessors:"));
        }

        [Test]
        public void YamlReplacement_IsValidatedAgainstTheSettingsClass()
        {
            var form = CreateForm();
            var request = new ConfigurationFormRequest
            {
                Entries = new List<ConfigurationEntryChange>
                {
                    new ConfigurationEntryChange { Section = "modules", Index = 0, Yaml = "test-module:\n  port: not-a-number" }
                }
            };

            var ex = Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, request));
            Assert.That(ex.Message, Does.Contain("test-module"));
        }

        [Test]
        public void AddModule_WithSettings_IsValidatedAndAppended()
        {
            var form = CreateForm();
            var result = form.Apply(Sample, new ConfigurationFormRequest
            {
                Add = new List<ConfigurationEntryAdd>
                {
                    new ConfigurationEntryAdd
                    {
                        Section = "modules",
                        TypeId = "test-module",
                        Values = new Dictionary<string, string> { ["port"] = "7900", ["server"] = "", ["enabled"] = "false" }
                    }
                }
            });

            // Empty values are left out so their defaults apply
            Assert.That(result, Does.Contain("    anything: 1\n\n- test-module:\n    port: 7900\n    enabled: false\nprocessors:"));
        }

        [Test]
        public void AddModule_RejectsUnknownTypesAndInvalidValues()
        {
            var form = CreateForm();
            ConfigurationFormRequest Add(string typeId, string key, string value) => new ConfigurationFormRequest
            {
                Add = new List<ConfigurationEntryAdd>
                {
                    new ConfigurationEntryAdd { Section = "modules", TypeId = typeId, Values = new Dictionary<string, string> { [key] = value } }
                }
            };

            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, Add("not-loaded", "port", "1")));
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, Add("test-module", "port", "abc")));
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, Add("test-module", "tags", "x")));
        }

        [Test]
        public void UnknownEntryIndex_IsRejected()
        {
            var form = CreateForm();
            Assert.Throws<ConfigurationEditException>(() => form.Apply(Sample, ModuleChange(9, new Dictionary<string, string> { ["port"] = "1" })));
        }

        [Test]
        public void Model_ListsEntriesWithFormsValuesAndOtherKeys()
        {
            var form = CreateForm();
            var json = System.Text.Json.JsonSerializer.Serialize(form.BuildModel(Sample, "agent.config.yaml", true));
            var model = System.Text.Json.JsonDocument.Parse(json).RootElement;

            var entries = model.GetProperty("entries").EnumerateArray().ToList();
            Assert.That(entries.Count, Is.EqualTo(3));

            var test = entries[0];
            Assert.That(test.GetProperty("hasForm").GetBoolean(), Is.True);
            Assert.That(test.GetProperty("values").GetProperty("port").GetString(), Is.EqualTo("7878"));
            Assert.That(test.GetProperty("complex").GetProperty("tags").GetString(), Is.EqualTo("- a\n- b"));
            Assert.That(test.GetProperty("otherKeys").EnumerateArray().Select(o => o.GetString()), Is.EqualTo(new[] { "customKey" }));

            Assert.That(entries[1].GetProperty("known").GetBoolean(), Is.False);
            Assert.That(entries[1].GetProperty("hasForm").GetBoolean(), Is.False);
            Assert.That(entries[2].GetProperty("section").GetString(), Is.EqualTo("processors"));
            Assert.That(entries[2].GetProperty("hasForm").GetBoolean(), Is.False);

            Assert.That(model.GetProperty("agent").GetProperty("values").GetProperty("devices").GetString(), Is.EqualTo("devices"));
        }
    }
}
