// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using MTConnect.Agents;
using MTConnect.Configurations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins the contract that <see cref="DeterministicAgentUuid.TryValidate"/>
    /// gates every source of the Agent meta-device UUID (operator-supplied
    /// override AND persisted <c>agent.information.json</c> state) so that
    /// malformed values — anything that does not parse as an RFC 4122 UUID —
    /// are rejected on the way in and the resolution falls through to the
    /// next path (persisted → derived).
    ///
    /// <para>
    /// Silently forwarding a non-UUID string violates MTConnect Part 1, which
    /// types the <c>uuid</c> attribute as the <c>UUID</c> DataType (RFC 4122
    /// enumerated string token). Any downstream XSD-validating consumer
    /// (cppagent parity, MQTT/JSON-cppagent transport) rejects the resulting
    /// wire content on typed enum/decimal DataItems.
    /// </para>
    ///
    /// <para>
    /// Fixture drives <see cref="AgentUuidResolver.Resolve"/> directly — the
    /// same method <c>MTConnectAgentApplication.StartAgent</c> calls — so
    /// production and tests cannot silently diverge on branch order, guard
    /// semantics, or normalisation output.
    /// </para>
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class AgentUuidValidationTests
    {
        private string _stateFilePath = null!;
        private string? _backupStateFile;

        /// <summary>Sets up the fixture before each test.</summary>
        [SetUp]
        public void SetUp()
        {
            _stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MTConnectAgentInformation.Filename);
            _backupStateFile = null;

            // Sweep orphan .valbak.* files from a prior crashed test run so
            // successive TearDowns cannot restore stale state.
            var directory = Path.GetDirectoryName(_stateFilePath);
            if (directory != null && Directory.Exists(directory))
            {
                foreach (var stale in Directory.EnumerateFiles(directory, MTConnectAgentInformation.Filename + ".valbak.*"))
                {
                    try { File.Delete(stale); } catch { /* best-effort — do not fail the test */ }
                }
            }

            // Back up any pre-existing state file so we do not perturb other
            // tests or the developer environment.
            if (File.Exists(_stateFilePath))
            {
                _backupStateFile = _stateFilePath + ".valbak." + Guid.NewGuid().ToString("N");
                File.Move(_stateFilePath, _backupStateFile);
            }
        }

        /// <summary>Tears down the fixture after each test.</summary>
        [TearDown]
        public void TearDown()
        {
            // Remove any state file we left behind, then restore the backup.
            if (File.Exists(_stateFilePath))
            {
                File.Delete(_stateFilePath);
            }
            if (_backupStateFile != null && File.Exists(_backupStateFile))
            {
                File.Move(_backupStateFile, _stateFilePath);
                _backupStateFile = null;
            }
        }

        // ------------------------------------------------------------------
        // Unit tests — DeterministicAgentUuid.TryValidate low-level contract
        // ------------------------------------------------------------------

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> rejects
        /// <see langword="null"/>, empty, and whitespace-only inputs.
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t")]
        [TestCase("\n")]
        public void TryValidate_null_or_whitespace_returns_false(string input)
        {
            var ok = DeterministicAgentUuid.TryValidate(input, out var normalized);

            Assert.That(ok, Is.False);
            Assert.That(normalized, Is.Null);
        }

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> rejects strings
        /// that do not parse as RFC 4122 UUIDs.
        /// </summary>
        [TestCase("not-a-uuid")]
        [TestCase("fixture-stable-uuid-001")]
        [TestCase("agent_1234567890abcdef")]
        [TestCase("123")]
        [TestCase("6ba7b810-9dad-11d1-80b4-00c04fd430c8-extra")]
        public void TryValidate_unparseable_returns_false(string input)
        {
            var ok = DeterministicAgentUuid.TryValidate(input, out var normalized);

            Assert.That(ok, Is.False);
            Assert.That(normalized, Is.Null);
        }

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> accepts the
        /// canonical hyphenated "D" form and returns it unchanged.
        /// </summary>
        [Test]
        public void TryValidate_canonical_hyphenated_form_returns_true_unchanged()
        {
            const string Canonical = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";

            var ok = DeterministicAgentUuid.TryValidate(Canonical, out var normalized);

            Assert.That(ok, Is.True);
            Assert.That(normalized, Is.EqualTo(Canonical));
        }

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> normalizes the
        /// braced "B", parenthesised "P", and bare-hex "N" forms to the
        /// canonical hyphenated "D" form so the wire representation stays
        /// stable regardless of the input format.
        /// </summary>
        [TestCase("{6ba7b810-9dad-11d1-80b4-00c04fd430c8}", "6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [TestCase("(6ba7b810-9dad-11d1-80b4-00c04fd430c8)", "6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [TestCase("6ba7b8109dad11d180b400c04fd430c8", "6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        public void TryValidate_non_canonical_format_normalizes_to_hyphenated(string input, string expected)
        {
            var ok = DeterministicAgentUuid.TryValidate(input, out var normalized);

            Assert.That(ok, Is.True);
            Assert.That(normalized, Is.EqualTo(expected));
        }

        // ------------------------------------------------------------------
        // Integration tests — AgentUuidResolver.Resolve three-path algorithm
        // ------------------------------------------------------------------

        /// <summary>
        /// Boot-simulation regression — malformed
        /// <c>AgentApplicationConfiguration.AgentUuid</c> on a fresh boot
        /// (no <c>agent.information.json</c>) must fall through to the
        /// derived UUID rather than being silently stored verbatim.
        /// </summary>
        [Test]
        public void Malformed_AgentUuid_on_fresh_boot_falls_through_to_derived()
        {
            const string MalformedInput = "not-a-uuid";
            const string ServiceName = "test-agent-malformed-fresh";
            var hostname = Environment.MachineName;

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = MalformedInput,
                ServiceName = ServiceName,
            };

            var resolved = SimulateFreshBoot(configuration, hostname);

            var expectedDerived = DeterministicAgentUuid.Derive(ServiceName, hostname, port: 0);
            Assert.That(resolved, Is.EqualTo(expectedDerived),
                "Malformed operator override on a fresh boot must fall through to the derived UUID.");
            Assert.That(resolved, Is.Not.EqualTo(MalformedInput),
                "The malformed input must NOT be stored verbatim (Part-1 UuidType violation).");
        }

        /// <summary>
        /// Boot-simulation regression — malformed
        /// <c>AgentApplicationConfiguration.AgentUuid</c> when a valid
        /// <c>agent.information.json</c> exists must preserve the persisted
        /// UUID rather than being silently stored verbatim.
        /// </summary>
        [Test]
        public void Malformed_AgentUuid_with_valid_persisted_state_preserves_persisted()
        {
            const string MalformedInput = "not-a-uuid";
            const string PersistedUuid = "cfbff0d1-9375-5685-968a-48ce8b50a653";

            // Pre-write the state file with a valid UUID.
            var preexisting = new MTConnectAgentInformation(PersistedUuid);
            preexisting.Save();

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = MalformedInput,
                ServiceName = "test-agent-malformed-persisted",
            };

            var resolved = SimulateWarmBoot(configuration, Environment.MachineName);

            Assert.That(resolved, Is.EqualTo(PersistedUuid),
                "Malformed operator override with valid persisted state must keep the persisted UUID.");
            Assert.That(resolved, Is.Not.EqualTo(MalformedInput),
                "The malformed input must NOT overwrite the persisted UUID.");
        }

        /// <summary>
        /// Path-2 hardening — malformed persisted state (e.g. a pre-hardening
        /// agent version wrote a non-UUID string, or the file was hand-edited)
        /// must ALSO fall through to the derived UUID rather than flowing on
        /// to the wire. Prevents the exact XSD-validation failure the PR was
        /// written to prevent, closed on the persisted-state axis too.
        /// </summary>
        [Test]
        public void Malformed_persisted_state_with_no_override_falls_through_to_derived()
        {
            const string MalformedPersisted = "agent_1234567890abcdef";
            const string ServiceName = "test-agent-malformed-persisted-path2";
            var hostname = Environment.MachineName;

            var preexisting = new MTConnectAgentInformation(MalformedPersisted);
            preexisting.Save();

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = null,
                ServiceName = ServiceName,
            };

            var resolved = SimulateWarmBoot(configuration, hostname);

            var expectedDerived = DeterministicAgentUuid.Derive(ServiceName, hostname, port: 0);
            Assert.That(resolved, Is.EqualTo(expectedDerived),
                "Malformed persisted UUID with no override must fall through to derived — Path 2 must validate.");
            Assert.That(resolved, Is.Not.EqualTo(MalformedPersisted),
                "The malformed persisted value must NOT reach the wire.");
        }

        /// <summary>
        /// Boot-simulation regression — valid non-canonical
        /// <c>AgentApplicationConfiguration.AgentUuid</c> (e.g. braced form)
        /// is accepted and normalized to the canonical hyphenated form so the
        /// wire representation stays stable across boots.
        /// </summary>
        [Test]
        public void Valid_non_canonical_AgentUuid_is_normalized_to_hyphenated()
        {
            const string BracedInput = "{6ba7b810-9dad-11d1-80b4-00c04fd430c8}";
            const string ExpectedCanonical = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = BracedInput,
                ServiceName = "test-agent-canonicalization",
            };

            var resolved = SimulateFreshBoot(configuration, Environment.MachineName);

            Assert.That(resolved, Is.EqualTo(ExpectedCanonical),
                "Non-canonical valid UUID inputs must be normalized to the hyphenated D-form.");
        }

        /// <summary>
        /// Two-boot regression — first boot with no override and no state file
        /// derives + persists a UUID; second boot with no override reads the
        /// persisted UUID from <c>agent.information.json</c> and adopts it
        /// verbatim (Path 2). Bit-identical across the two boots.
        /// </summary>
        [Test]
        public void Persisted_UUID_is_adopted_on_second_boot_when_no_override()
        {
            const string ServiceName = "test-agent-persisted-second-boot";
            var hostname = Environment.MachineName;

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = null,
                ServiceName = ServiceName,
            };

            // Boot 1 — no override, no state file → derive + persist.
            var boot1Uuid = SimulateFreshBoot(configuration, hostname);
            Assert.That(File.Exists(_stateFilePath), Is.True,
                "Boot 1 must persist agent.information.json with the resolved UUID.");
            var persistedAfterBoot1 = MTConnectAgentInformation.Read();
            Assert.That(persistedAfterBoot1, Is.Not.Null);
            Assert.That(persistedAfterBoot1!.Uuid, Is.EqualTo(boot1Uuid),
                "Boot 1's on-disk UUID must equal the resolved value.");

            // Boot 2 — no override, but persisted state exists → adopt persisted.
            var boot2Uuid = SimulateWarmBoot(configuration, hostname);
            Assert.That(boot2Uuid, Is.EqualTo(boot1Uuid),
                "Boot 2 must adopt the persisted UUID bit-identically (Path 2).");
        }

        /// <summary>
        /// First-boot persistence regression — no override, no state file →
        /// Path 3 derives the UUID AND <see cref="MTConnectAgentInformation.Save"/>
        /// writes it to <c>agent.information.json</c> so subsequent boots hit
        /// Path 2. Guards against a regression where the resolve step returns
        /// the derived value but the persist step is dropped.
        /// </summary>
        [Test]
        public void First_boot_persists_derived_UUID_to_agent_information_json()
        {
            const string ServiceName = "test-agent-first-boot-persist";
            var hostname = Environment.MachineName;

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = null,
                ServiceName = ServiceName,
            };

            var resolved = SimulateFreshBoot(configuration, hostname);

            Assert.That(File.Exists(_stateFilePath), Is.True,
                "agent.information.json must exist after the first boot.");
            var persisted = MTConnectAgentInformation.Read();
            Assert.That(persisted, Is.Not.Null);
            Assert.That(persisted!.Uuid, Is.EqualTo(resolved),
                "Persisted UUID on disk must equal the resolved value.");
            var expectedDerived = DeterministicAgentUuid.Derive(ServiceName, hostname, port: 0);
            Assert.That(persisted.Uuid, Is.EqualTo(expectedDerived),
                "Persisted UUID must equal the deterministic derivation for the given ServiceName.");
        }

        // ------------------------------------------------------------------
        // Boot-simulation helpers — thin wrappers around AgentUuidResolver.
        // ------------------------------------------------------------------

        /// <summary>
        /// Replays the fresh-boot UUID resolution slice of
        /// <c>MTConnectAgentApplication.StartAgent</c> — no
        /// <c>agent.information.json</c> on disk — via
        /// <see cref="AgentUuidResolver.Resolve"/> (the exact call production
        /// makes) followed by <see cref="MTConnectAgentInformation.Save"/> so
        /// on-disk state assertions can verify persistence.
        /// </summary>
        private string SimulateFreshBoot(
            AgentApplicationConfiguration configuration,
            string hostname)
        {
            var existing = MTConnectAgentInformation.Read();
            var freshlyConstructed = existing == null;
            var info = existing ?? new MTConnectAgentInformation();

            info.Uuid = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: configuration.AgentUuid,
                persistedUuid: freshlyConstructed ? null : info.Uuid,
                agentName: configuration.ServiceName,
                hostname: hostname);

            info.Save();
            return info.Uuid;
        }

        /// <summary>
        /// Replays the warm-boot UUID resolution slice — pre-existing
        /// <c>agent.information.json</c> is read first — via
        /// <see cref="AgentUuidResolver.Resolve"/>. Semantically identical to
        /// <see cref="SimulateFreshBoot"/>; two named helpers make the
        /// per-test intent (fresh vs warm) unambiguous at the call site.
        /// </summary>
        private string SimulateWarmBoot(
            AgentApplicationConfiguration configuration,
            string hostname) => SimulateFreshBoot(configuration, hostname);
    }
}
