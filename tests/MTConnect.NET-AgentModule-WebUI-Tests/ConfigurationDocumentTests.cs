// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Modules.WebUI;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace MTConnect.AgentModule.WebUI.Tests
{
    /// <summary>
    /// Tests for the comment-preserving, key-by-key editing of agent.config.yaml.
    /// </summary>
    public class ConfigurationDocumentTests
    {
        private static string Y(params string[] lines) => string.Join("\n", lines) + "\n";

        private static readonly string Sample = Y(
            "# - Device Configuration -",
            "devices: devices",
            "",
            "# - Modules -",
            "modules:",
            "",
            "- http-server: # - Add HTTP Server module",
            "    port: 5000",
            "    indentOutput: true",
            "    # documentFormat: json",
            "",
            "# - shdr-adapter: # commented-out example",
            "#     port: 7000",
            "",
            "- shdr-adapter: # Okuma",
            "    deviceKey: Okuma",
            "    port: 7878 # adapter port",
            "",
            "- shdr-adapter: # Mazak",
            "    deviceKey: Mazak",
            "    port: 7879",
            "",
            "- web-ui:",
            "",
            "# The maximum number of Observations the agent can hold in its buffer",
            "observationBufferSize: 150000");


        [Test]
        public void NoChanges_ReturnsIdenticalText()
        {
            var document = ConfigurationDocument.Parse(Sample);
            Assert.That(document.Apply(), Is.EqualTo(Sample));
        }

        [Test]
        public void SampleAgentConfig_RoundTripsUnchanged()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", "agent.config.yaml");
            var text = File.ReadAllText(path);

            var document = ConfigurationDocument.Parse(text);
            Assert.That(document.Apply(), Is.EqualTo(text));
            Assert.That(document.GetEntries("modules").Select(o => o.TypeId), Does.Contain("http-server"));
        }

        [Test]
        public void ChangeRootValue_OnlyChangesThatValue()
        {
            var document = ConfigurationDocument.Parse(Sample);
            document.SetRootValue("observationBufferSize", "200000");

            Assert.That(document.Apply(), Is.EqualTo(Sample.Replace("observationBufferSize: 150000", "observationBufferSize: 200000")));
        }

        [Test]
        public void ChangeEntryValue_KeepsTrailingComment()
        {
            var document = ConfigurationDocument.Parse(Sample);
            var okuma = document.GetEntries("modules")[1];
            document.SetEntryValue(okuma, "port", "7900");

            Assert.That(document.Apply(), Is.EqualTo(Sample.Replace("port: 7878 # adapter port", "port: 7900 # adapter port")));
        }

        [Test]
        public void ChangeEntryValue_AppliesToTheRightInstance()
        {
            var document = ConfigurationDocument.Parse(Sample);
            var mazak = document.GetEntries("modules")[2];
            document.SetEntryValue(mazak, "port", "7999");

            var result = document.Apply();
            Assert.That(result, Does.Contain("port: 7878 # adapter port"));
            Assert.That(result, Does.Contain("deviceKey: Mazak\n    port: 7999\n"));
        }

        [Test]
        public void RemoveEntryValue_RemovesOnlyThatLine()
        {
            var document = ConfigurationDocument.Parse(Sample);
            var http = document.GetEntries("modules")[0];
            document.SetEntryValue(http, "indentOutput", null);

            Assert.That(document.Apply(), Is.EqualTo(Sample.Replace("    indentOutput: true\n", "")));
        }

        [Test]
        public void AddEntryValue_UsesExistingIndentation()
        {
            var document = ConfigurationDocument.Parse(Sample);
            var http = document.GetEntries("modules")[0];
            document.SetEntryValue(http, "allowPut", "true");

            Assert.That(document.Apply(), Is.EqualTo(Sample.Replace("    indentOutput: true\n", "    indentOutput: true\n    allowPut: true\n")));
        }

        [Test]
        public void AddEntryValue_ToEntryWithoutSettings_CreatesBlock()
        {
            var document = ConfigurationDocument.Parse(Sample);
            var webUi = document.GetEntries("modules")[3];
            document.SetEntryValue(webUi, "port", "5081");
            document.SetEntryValue(webUi, "server", "localhost");

            var result = document.Apply();
            Assert.That(result, Does.Contain("- web-ui:\n    port: 5081\n    server: localhost\n"));

            // The result is still valid and the new settings belong to the web-ui entry
            var reparsed = ConfigurationDocument.Parse(result);
            reparsed.GetValues(reparsed.GetEntries("modules")[3].ValueNode, out var values, out _);
            Assert.That(values["port"], Is.EqualTo("5081"));
        }

        [Test]
        public void RemoveLastValueAndAddAnother_DoNotOverlap()
        {
            var document = ConfigurationDocument.Parse(Sample);
            var mazak = document.GetEntries("modules")[2];
            document.SetEntryValue(mazak, "port", null);
            document.SetEntryValue(mazak, "heartbeat", "1000");

            var result = document.Apply();
            Assert.That(result, Does.Contain("deviceKey: Mazak\n    heartbeat: 1000\n"));
            Assert.That(result, Does.Not.Contain("port: 7879"));
        }

        [Test]
        public void AddRootValue_AppendsAtEndOfFile()
        {
            var text = "devices: devices\n# trailing comment";
            var document = ConfigurationDocument.Parse(text);
            document.SetRootValue("enableValidation", "true");

            Assert.That(document.Apply(), Is.EqualTo("devices: devices\n# trailing comment\nenableValidation: true\n"));
        }

        [Test]
        public void SetValueOfKeyWithoutValue_InsertsAfterColon()
        {
            var text = Y("sender:  # set by the agent", "devices: devices");
            var document = ConfigurationDocument.Parse(text);
            document.SetRootValue("sender", "machine-01");

            Assert.That(document.Apply(), Is.EqualTo(Y("sender: machine-01  # set by the agent", "devices: devices")));
        }

        [Test]
        public void WindowsLineEndings_ArePreserved()
        {
            var text = Sample.Replace("\n", "\r\n");
            var document = ConfigurationDocument.Parse(text);
            document.SetEntryValue(document.GetEntries("modules")[0], "allowPut", "true");
            document.SetRootValue("observationBufferSize", null);

            var result = document.Apply();
            Assert.That(result.Replace("\r\n", ""), Does.Not.Contain("\n"));
            Assert.That(result, Does.Contain("    indentOutput: true\r\n    allowPut: true\r\n"));
            Assert.That(result, Does.Not.Contain("observationBufferSize"));
        }

        [Test]
        public void AddEntry_GoesAfterTheLastEntry()
        {
            var document = ConfigurationDocument.Parse(Sample);
            document.AddEntry("modules", "shdr-adapter", new[]
            {
                new System.Collections.Generic.KeyValuePair<string, string>("deviceKey", "Haas"),
                new System.Collections.Generic.KeyValuePair<string, string>("port", "7880")
            });

            var result = document.Apply();
            Assert.That(result, Is.EqualTo(Sample.Replace(
                "- web-ui:\n\n# The maximum",
                "- web-ui:\n\n- shdr-adapter:\n    deviceKey: Haas\n    port: 7880\n\n# The maximum")));

            var entries = ConfigurationDocument.Parse(result).GetEntries("modules");
            Assert.That(entries.Select(o => o.TypeId), Is.EqualTo(new[] { "http-server", "shdr-adapter", "shdr-adapter", "web-ui", "shdr-adapter" }));
        }

        [Test]
        public void AddEntry_WithoutSettings()
        {
            var document = ConfigurationDocument.Parse(Y("modules:", "- http-server:", "    port: 5000"));
            document.AddEntry("modules", "web-ui", null);

            Assert.That(document.Apply(), Is.EqualTo(Y("modules:", "- http-server:", "    port: 5000", "", "- web-ui:")));
        }

        [Test]
        public void AddEntry_KeepsIndentedListStyle()
        {
            var document = ConfigurationDocument.Parse(Y("modules:", "  - http-server:", "      port: 5000"));
            document.AddEntry("modules", "web-ui", new[] { new System.Collections.Generic.KeyValuePair<string, string>("port", "5081") });

            var result = document.Apply();
            Assert.That(result, Is.EqualTo(Y("modules:", "  - http-server:", "      port: 5000", "", "  - web-ui:", "      port: 5081")));
            Assert.That(ConfigurationDocument.Parse(result).GetEntries("modules").Count, Is.EqualTo(2));
        }

        [Test]
        public void AddEntry_CreatesTheSectionWhenMissing()
        {
            var document = ConfigurationDocument.Parse("devices: devices");
            document.AddEntry("modules", "http-server", new[] { new System.Collections.Generic.KeyValuePair<string, string>("port", "5000") });

            var result = document.Apply();
            Assert.That(result, Is.EqualTo("devices: devices\n\nmodules:\n- http-server:\n    port: 5000\n"));
            Assert.That(ConfigurationDocument.Parse(result).GetEntries("modules")[0].TypeId, Is.EqualTo("http-server"));
        }

        [Test]
        public void AddEntry_ToEmptySection()
        {
            var document = ConfigurationDocument.Parse(Y("modules: # none yet", "devices: devices"));
            document.AddEntry("modules", "web-ui", null);

            var result = document.Apply();
            Assert.That(result, Is.EqualTo(Y("modules: # none yet", "- web-ui:", "devices: devices")));
            Assert.That(ConfigurationDocument.Parse(result).GetEntries("modules")[0].TypeId, Is.EqualTo("web-ui"));
        }

        [Test]
        public void AddEntry_AndChangeLastEntry_KeepsOrder()
        {
            var document = ConfigurationDocument.Parse(Y("modules:", "- http-server:", "    port: 5000"));
            document.SetEntryValue(document.GetEntries("modules")[0], "allowPut", "true");
            document.AddEntry("modules", "web-ui", null);

            Assert.That(document.Apply(), Is.EqualTo(Y("modules:", "- http-server:", "    port: 5000", "    allowPut: true", "", "- web-ui:")));
        }

        [Test]
        public void GetEntryText_IsDedentedAndReplaceable()
        {
            var document = ConfigurationDocument.Parse(Sample);
            var okuma = document.GetEntries("modules")[1];

            var yaml = document.GetEntryText(okuma);
            Assert.That(yaml, Is.EqualTo("shdr-adapter: # Okuma\n  deviceKey: Okuma\n  port: 7878 # adapter port"));

            document.ReplaceEntryText(okuma, "shdr-adapter: # Okuma\n  deviceKey: Okuma\n  port: 7878 # adapter port\n  heartbeat: 500\n");
            Assert.That(document.Apply(), Does.Contain("    port: 7878 # adapter port\n    heartbeat: 500\n\n- shdr-adapter: # Mazak"));
        }

        [Test]
        public void ReplaceEntryText_RejectsMoreThanOneTypeKey()
        {
            var document = ConfigurationDocument.Parse(Sample);
            var okuma = document.GetEntries("modules")[1];

            Assert.Throws<ConfigurationEditException>(() => document.ReplaceEntryText(okuma, "shdr-adapter:\n  port: 1\nhttp-server:\n  port: 2"));
        }

        [Test]
        public void ChangingMultilineValue_IsRejected()
        {
            var text = Y("devices: devices", "modules:", "- http-server:", "    files:", "    - path: schemas", "      location: schemas");
            var document = ConfigurationDocument.Parse(text);

            Assert.Throws<ConfigurationEditException>(() => document.SetEntryValue(document.GetEntries("modules")[0], "files", "x"));
        }

        [Test]
        public void FlowStyleEntry_IsNotEditable()
        {
            var document = ConfigurationDocument.Parse(Y("modules:", "- http-server: { port: 5000 }"));
            Assert.That(document.GetEntries("modules")[0].IsEditable, Is.False);
        }

        [TestCase("localhost", "localhost")]
        [TestCase(@"C:\mtconnect\agent", @"C:\mtconnect\agent")]
        [TestCase("MTConnect/Document", "MTConnect/Document")]
        [TestCase("2.5", "'2.5'")]
        [TestCase("true", "'true'")]
        [TestCase("off", "'off'")]
        [TestCase("two words", "'two words'")]
        [TestCase("it's", "'it''s'")]
        [TestCase("#hash", "'#hash'")]
        [TestCase("a: b", "'a: b'")]
        [TestCase("", "''")]
        public void QuoteString_QuotesWhenNeeded(string value, string expected)
        {
            Assert.That(ConfigurationDocument.QuoteString(value), Is.EqualTo(expected));
        }

        [Test]
        public void QuotedValues_ReadBackAsTheOriginalString()
        {
            foreach (var value in new[] { "2.5", "true", "two words", "it's", "#hash", "a: b", @"C:\x\y" })
            {
                var document = ConfigurationDocument.Parse("sender: x\n");
                document.SetRootValue("sender", ConfigurationDocument.QuoteString(value));

                var reparsed = ConfigurationDocument.Parse(document.Apply());
                reparsed.GetValues(reparsed.Root, out var values, out _);
                Assert.That(values["sender"], Is.EqualTo(value), value);
            }
        }
    }
}
