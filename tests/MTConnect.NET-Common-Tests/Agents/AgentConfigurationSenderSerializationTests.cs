// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using MTConnect.Configurations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins the JSON / YAML serialization contract for
    /// <see cref="AgentConfiguration.Sender"/>. The property must round-trip
    /// through both formats via <c>SaveJson</c> / <c>ReadJson</c> and
    /// <c>SaveYaml</c> / <c>ReadYaml</c>, and it must be tagged with the
    /// <c>sender</c> wire-name that operator YAML / JSON authors write, so an
    /// operator-supplied <c>sender: foo-plant-a</c> in <c>agent.config.yaml</c>
    /// binds through to <see cref="MTConnect.Agents.MTConnectAgent.Sender"/>
    /// on startup.
    /// </summary>
    [TestFixture]
    public class AgentConfigurationSenderSerializationTests
    {
        private string _workingDirectory = string.Empty;

        /// <summary>Creates a per-test working directory so file writes do not
        /// contend across parallel fixtures.</summary>
        [SetUp]
        public void SetUp()
        {
            _workingDirectory = Path.Combine(
                Path.GetTempPath(),
                "mtconnect-sender-serialization-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_workingDirectory);
        }

        /// <summary>Removes the per-test working directory.</summary>
        [TearDown]
        public void TearDown()
        {
            if (_workingDirectory != null && Directory.Exists(_workingDirectory))
            {
                try
                {
                    Directory.Delete(_workingDirectory, recursive: true);
                }
                catch
                {
                    // Non-fatal cleanup swallow.
                }
            }
        }

        /// <summary>Default value of <c>Sender</c> on a freshly constructed
        /// configuration is null, so the agent falls back to
        /// <see cref="System.Net.Dns.GetHostName"/> until an operator opts in.</summary>
        [Test]
        public void Sender_default_value_is_null()
        {
            var configuration = new AgentConfiguration();

            Assert.That(configuration.Sender, Is.Null);
        }

        /// <summary>The <see cref="AgentConfiguration.Sender"/> property carries
        /// the <c>[JsonPropertyName("sender")]</c> attribute so authored JSON
        /// binds under the lowercase wire-name.</summary>
        [Test]
        public void Sender_property_wire_name_is_lowercase_sender()
        {
            var property = typeof(AgentConfiguration).GetProperty(
                nameof(AgentConfiguration.Sender),
                BindingFlags.Public | BindingFlags.Instance);

            Assert.That(property, Is.Not.Null);
            var jsonAttribute = property!.GetCustomAttribute<JsonPropertyNameAttribute>();

            Assert.That(jsonAttribute, Is.Not.Null);
            Assert.That(jsonAttribute!.Name, Is.EqualTo("sender"));
        }

        /// <summary>Round-trips <see cref="AgentConfiguration.Sender"/> through
        /// the JSON save / read pipeline, pinning that
        /// <see cref="AgentConfiguration.SaveJson"/> emits the value and
        /// <see cref="AgentConfiguration.ReadJson{T}"/> parses it back.</summary>
        [Test]
        public void Sender_JSON_roundtrips_through_SaveJson_and_ReadJson()
        {
            const string PinnedSender = "plant-a-aggregator";
            var jsonPath = Path.Combine(_workingDirectory, "agent.config.json");

            var original = new AgentConfiguration
            {
                Sender = PinnedSender
            };
            original.SaveJson(jsonPath, createBackup: false);

            var loaded = AgentConfiguration.ReadJson<AgentConfiguration>(jsonPath);

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo(PinnedSender));
            Assert.That(loaded.Path, Is.EqualTo(jsonPath));
        }

        /// <summary>Round-trips <see cref="AgentConfiguration.Sender"/> through
        /// the YAML save / read pipeline, pinning both directions of the
        /// operator-facing YAML surface.</summary>
        [Test]
        public void Sender_YAML_roundtrips_through_SaveYaml_and_ReadYaml()
        {
            const string PinnedSender = "plant-b-aggregator";
            var yamlPath = Path.Combine(_workingDirectory, "agent.config.yaml");

            var original = new AgentConfiguration
            {
                Sender = PinnedSender
            };
            original.SaveYaml(yamlPath, createBackup: false);

            var loaded = AgentConfiguration.ReadYaml<AgentConfiguration>(yamlPath);

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo(PinnedSender));
            Assert.That(loaded.Path, Is.EqualTo(yamlPath));
        }

        /// <summary>Authored JSON payloads carrying <c>"sender": "…"</c> bind
        /// straight through to <see cref="AgentConfiguration.Sender"/>.</summary>
        [Test]
        public void Sender_reads_from_authored_JSON_payload()
        {
            const string PinnedSender = "gateway-north-1";
            var jsonPath = Path.Combine(_workingDirectory, "authored.json");
            File.WriteAllText(
                jsonPath,
                "{\n  \"sender\": \"" + PinnedSender + "\",\n"
                + "  \"observationBufferSize\": 4096\n}\n");

            var loaded = AgentConfiguration.ReadJson<AgentConfiguration>(jsonPath);

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo(PinnedSender));
            Assert.That(loaded.ObservationBufferSize, Is.EqualTo(4096));
        }

        /// <summary>Authored YAML payloads carrying <c>sender: …</c> under the
        /// camel-case naming convention bind to
        /// <see cref="AgentConfiguration.Sender"/>.</summary>
        [Test]
        public void Sender_reads_from_authored_YAML_payload()
        {
            const string PinnedSender = "gateway-north-2";
            var yamlPath = Path.Combine(_workingDirectory, "authored.yaml");
            File.WriteAllText(
                yamlPath,
                "sender: " + PinnedSender + "\n"
                + "observationBufferSize: 8192\n");

            var loaded = AgentConfiguration.ReadYaml<AgentConfiguration>(yamlPath);

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo(PinnedSender));
            Assert.That(loaded.ObservationBufferSize, Is.EqualTo(8192));
        }

        /// <summary>Empty-string <see cref="AgentConfiguration.Sender"/> round-trips
        /// as empty — the fallback in <see cref="MTConnect.Agents.MTConnectAgent"/>
        /// treats null and empty identically.</summary>
        [Test]
        public void Sender_empty_string_roundtrips_as_empty_string()
        {
            var jsonPath = Path.Combine(_workingDirectory, "empty-sender.json");
            var original = new AgentConfiguration { Sender = string.Empty };
            original.SaveJson(jsonPath, createBackup: false);

            var loaded = AgentConfiguration.ReadJson<AgentConfiguration>(jsonPath);

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo(string.Empty));
        }

        /// <summary>Multiline / whitespace-preserving values round-trip
        /// byte-for-byte through the JSON pipeline. Operators occasionally
        /// author trimmed identifiers with embedded structure; the wire
        /// format preserves them verbatim.</summary>
        [Test]
        public void Sender_with_special_characters_roundtrips_verbatim()
        {
            const string PinnedSender = "plant/a::region-north:agent-42";
            var jsonPath = Path.Combine(_workingDirectory, "special-sender.json");
            var original = new AgentConfiguration { Sender = PinnedSender };
            original.SaveJson(jsonPath, createBackup: false);

            var loaded = AgentConfiguration.ReadJson<AgentConfiguration>(jsonPath);

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo(PinnedSender));

            var text = File.ReadAllText(jsonPath);
            using var document = JsonDocument.Parse(text);
            Assert.That(document.RootElement.GetProperty("sender").GetString(),
                Is.EqualTo(PinnedSender));
        }

        // ---------------- surface coverage: Read / Save alternates ----------------

        /// <summary>The non-generic <c>ReadJson(Type, path)</c> overload
        /// deserialises into the specified runtime type and populates the
        /// operator path field, matching the generic overload's contract.</summary>
        [Test]
        public void Sender_reads_from_authored_JSON_via_non_generic_Type_overload()
        {
            const string PinnedSender = "gateway-nongeneric-json";
            var jsonPath = Path.Combine(_workingDirectory, "type-json.json");
            File.WriteAllText(
                jsonPath,
                "{\n  \"sender\": \"" + PinnedSender + "\"\n}\n");

            var loaded = AgentConfiguration.ReadJson(typeof(AgentConfiguration), jsonPath);

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo(PinnedSender));
            Assert.That(loaded.Path, Is.EqualTo(jsonPath));
        }

        /// <summary>The non-generic <c>ReadYaml(Type, path)</c> overload
        /// deserialises into the specified runtime type and populates the
        /// operator path field.</summary>
        [Test]
        public void Sender_reads_from_authored_YAML_via_non_generic_Type_overload()
        {
            const string PinnedSender = "gateway-nongeneric-yaml";
            var yamlPath = Path.Combine(_workingDirectory, "type-yaml.yaml");
            File.WriteAllText(yamlPath, "sender: " + PinnedSender + "\n");

            var loaded = AgentConfiguration.ReadYaml(typeof(AgentConfiguration), yamlPath);

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo(PinnedSender));
            Assert.That(loaded.Path, Is.EqualTo(yamlPath));
        }

        /// <summary>The <c>Read</c> entry-point treats an explicit path as
        /// YAML per the resolver contract; the value flows through even when
        /// the file has no extension.</summary>
        [Test]
        public void Sender_reads_via_top_level_Read_with_explicit_path()
        {
            const string PinnedSender = "read-top-level";
            var path = Path.Combine(_workingDirectory, "explicit-path.yaml");
            File.WriteAllText(path, "sender: " + PinnedSender + "\n");

            var loaded = AgentConfiguration.Read(path);

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo(PinnedSender));
        }

        /// <summary>ReadJson returns null when the target path does not exist —
        /// pinning the file-missing branch that quietly returns null rather
        /// than throwing.</summary>
        [Test]
        public void ReadJson_returns_null_when_target_file_is_missing()
        {
            var missingPath = Path.Combine(_workingDirectory, "does-not-exist.json");

            var loaded = AgentConfiguration.ReadJson<AgentConfiguration>(missingPath);

            Assert.That(loaded, Is.Null);
        }

        /// <summary>ReadJson returns null when the target file is empty.</summary>
        [Test]
        public void ReadJson_returns_null_when_target_file_is_empty()
        {
            var emptyPath = Path.Combine(_workingDirectory, "empty.json");
            File.WriteAllText(emptyPath, string.Empty);

            var loaded = AgentConfiguration.ReadJson<AgentConfiguration>(emptyPath);

            Assert.That(loaded, Is.Null);
        }

        /// <summary>ReadJson swallows deserialisation failures and returns
        /// null when the payload is malformed JSON.</summary>
        [Test]
        public void ReadJson_returns_null_when_payload_is_malformed_JSON()
        {
            var badPath = Path.Combine(_workingDirectory, "malformed.json");
            File.WriteAllText(badPath, "{ this is not valid json ");

            var loaded = AgentConfiguration.ReadJson<AgentConfiguration>(badPath);

            Assert.That(loaded, Is.Null);
        }

        /// <summary>ReadYaml returns null when the target file is missing.</summary>
        [Test]
        public void ReadYaml_returns_null_when_target_file_is_missing()
        {
            var missingPath = Path.Combine(_workingDirectory, "does-not-exist.yaml");

            var loaded = AgentConfiguration.ReadYaml<AgentConfiguration>(missingPath);

            Assert.That(loaded, Is.Null);
        }

        /// <summary>ReadYaml returns null when the target file is empty.</summary>
        [Test]
        public void ReadYaml_returns_null_when_target_file_is_empty()
        {
            var emptyPath = Path.Combine(_workingDirectory, "empty.yaml");
            File.WriteAllText(emptyPath, string.Empty);

            var loaded = AgentConfiguration.ReadYaml<AgentConfiguration>(emptyPath);

            Assert.That(loaded, Is.Null);
        }

        /// <summary>SaveJson with createBackup: true creates a copy of the
        /// pre-existing target file into the conventional backup directory
        /// before overwriting.</summary>
        [Test]
        public void SaveJson_with_createBackup_copies_existing_target_to_backup_directory()
        {
            var jsonPath = Path.Combine(_workingDirectory, "backup.json");
            File.WriteAllText(jsonPath, "{\"sender\": \"pre-existing\"}\n");

            var updated = new AgentConfiguration { Sender = "post-backup" };
            updated.SaveJson(jsonPath, createBackup: true);

            var loaded = AgentConfiguration.ReadJson<AgentConfiguration>(jsonPath);
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo("post-backup"));
        }

        /// <summary>SaveYaml with createBackup: true copies the pre-existing
        /// target file into the conventional backup directory before
        /// overwriting.</summary>
        [Test]
        public void SaveYaml_with_createBackup_copies_existing_target_to_backup_directory()
        {
            var yamlPath = Path.Combine(_workingDirectory, "backup.yaml");
            File.WriteAllText(yamlPath, "sender: pre-existing\n");

            var updated = new AgentConfiguration { Sender = "post-backup" };
            updated.SaveYaml(yamlPath, createBackup: true);

            var loaded = AgentConfiguration.ReadYaml<AgentConfiguration>(yamlPath);
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Sender, Is.EqualTo("post-backup"));
        }

        /// <summary>DefaultVersionValue getter returns the canonical
        /// <see cref="Version"/>.ToString() of the current
        /// <see cref="AgentConfiguration.DefaultVersion"/>.</summary>
        [Test]
        public void DefaultVersionValue_getter_reflects_DefaultVersion()
        {
            var configuration = new AgentConfiguration
            {
                DefaultVersion = new Version(2, 7)
            };

            Assert.That(configuration.DefaultVersionValue, Is.EqualTo("2.7"));
        }

        /// <summary>DefaultVersionValue setter parses a valid version string
        /// into <see cref="AgentConfiguration.DefaultVersion"/>.</summary>
        [Test]
        public void DefaultVersionValue_setter_parses_valid_version_string()
        {
            var configuration = new AgentConfiguration
            {
                DefaultVersionValue = "2.6"
            };

            Assert.That(configuration.DefaultVersion, Is.EqualTo(new Version(2, 6)));
        }

        /// <summary>DefaultVersionValue setter silently drops an unparseable
        /// value, leaving <see cref="AgentConfiguration.DefaultVersion"/>
        /// at its previous state.</summary>
        [Test]
        public void DefaultVersionValue_setter_drops_unparseable_value()
        {
            var configuration = new AgentConfiguration();
            var originalVersion = configuration.DefaultVersion;

            configuration.DefaultVersionValue = "not-a-version";

            Assert.That(configuration.DefaultVersion, Is.EqualTo(originalVersion));
        }

        /// <summary>DefaultVersionValue setter accepts a null string as a
        /// no-op, matching the guard on the setter's input branch.</summary>
        [Test]
        public void DefaultVersionValue_setter_treats_null_as_noop()
        {
            var configuration = new AgentConfiguration();
            var originalVersion = configuration.DefaultVersion;

            configuration.DefaultVersionValue = null;

            Assert.That(configuration.DefaultVersion, Is.EqualTo(originalVersion));
        }
    }
}
